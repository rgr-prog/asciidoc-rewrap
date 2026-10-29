// Compare Asciidoctor output, rather than just matching a wrapping algorithm.
import assert from 'node:assert/strict'
import {convert} from '@asciidoctor/core'
import {rewrap, noCustomMarkers} from '../dist/dev/Main.js'

const render = async text => (await convert(text, {header_footer: false}))
  .replace(/\s+/g, ' ').trim()
const wrap = (text, column, selections = []) => {
  const lines = text.split('\n')
  const edit = rewrap(
    {language: 'asciidoc', path: 'test.adoc', getMarkers: () => noCustomMarkers},
    {column, tabWidth: 4, doubleSentenceSpacing: false, reformat: false, wholeComment: true},
    selections, i => lines[i] ?? null)
  if (edit.isEmpty) return text
  return [...lines.slice(0, edit.startLine), ...edit.lines,
    ...lines.slice(edit.endLine + 1)].join('\n')
}

const documents = [
  'A paragraph with enough words to wrap across several different column widths.',
  '* First item has several words\nand a continuation.\n* Second item also has words.\n** Nested item has more words.',
  '. First ordered item has several words.\n. Second item has some more words.',
  'Term:: A description with enough words to wrap.\nOther:: A second description with more words.',
  '[quote,Some Author]\n____\nThis is a quotation with enough words to wrap.\n\nAnother paragraph inside the quotation.\n____',
  '[verse,Some Author]\n____\nFirst line of the poem\nSecond line of the poem\n____',
  '= A document title\nDoc Writer\nv1.0, 2026-10-29\n:toc:\n\n== Section\n\nA paragraph with several words to wrap.',
  '[[an-id]]\n.A block title\n[example]\n====\nProse inside an example block with enough words to wrap.\n====',
  '[cols="1,1"]\n|===\n|First cell\n|Second cell\n\n|More cells |And more text\n|===',
  '* Item with prose and several words to wrap\n+\n----\ncode with meaningful indentation\n  more code\n----\n+\nAn attached paragraph with more prose.',
  'NOTE: An admonition paragraph with several words to wrap.',
  'Prose with link:https://example.org[link text with spaces] and `inline code with spaces`.',
  'Prose with *emphasis with spaces* and _more emphasis with spaces_.',
  'One explicit line break +\nfollowed by another line with several words.',
  'A paragraph that mentions * a literal asterisk among several ordinary words.',
  'A paragraph that mentions . an isolated dot among several ordinary words.',
  'A paragraph that mentions == a pair of equals signs among ordinary words.',
  'A paragraph that mentions term:: a description marker among ordinary words.',
  'A paragraph that mentions NOTE: a label among ordinary words.',
  'A paragraph that ends with a bracketed word [literal]',
  '[cols="1,1",options="header"]\n|===\n|A rather long heading |Another rather long heading\n\n|A long first cell with words that need wrapping. |A second cell with several more words.\n|===',
  '|===\n|An implicitly sized column heading |Another implicit column heading\n\n|A long cell containing a sentence with several words |Another cell with more words\n|===',
  '[cols="1,1"]\n|===\n|First cell has a long line and more prose\nwhich continues on the following line.\n|Second cell has another long sentence.\n|===',
  '[cols="1,1"]\n|===\n|First paragraph in the cell with a long sentence.\n\nSecond paragraph in the same cell with more prose.\n|Another cell with several words to wrap.\n|===',
  '[cols="1,1"]\n|===\n|A prose cell with an escaped \\| separator and {vbar} references.\n|Another long prose cell with more words.\n|===',
  '[cols="2*"]\n|===\n2+|Spanning two columns with a long sentence and several words.\n>s|A bold cell with more prose. .^e|An emphasized cell with more prose.\n|===',
  '[cols="2*"]\n|===\n2*|Duplicated prose with several words to wrap in the source.\n|===',
  '[cols="1,1"]\n|===\nl|Literal lines must remain\n  exactly as they were written.\n|Ordinary prose with enough words to wrap.\n|===',
  '[cols="1,1"]\n|===\na|* A list inside an AsciiDoc cell\n* Another item\n\n----\n  code must remain intact\n----\n|Ordinary prose with enough words to wrap.\n|===',
  '[cols="l,1"]\n|===\n|Literal column with a long line that must not wrap.\n|Another column with several words.\n|===',
  '[cols="1,1",separator="!"]\n|===\n!First cell contains a long sentence with a literal | pipe.\n!Second cell contains more prose to wrap.\n|===',
  '[cols="1"]\n|===\na|\n[cols="1,1"]\n!===\n!Nested table first cell with several words to wrap.\n!Nested table second cell with more prose to wrap.\n!===\n|===',
  '[cols="1,1",format="csv"]\n|===\n"A quoted CSV value with several words",other\n|===',
  '[cols="1,1",format="dsv"]\n|===\nA long delimited value:other\n|===',
  '[cols="1,1"]\n|===\n|Explicit line break in a cell +\nkeeps these lines separate.\n|Other cell with several words.\n|===',
  '[cols="1,1"]\n|===\n|\nText in a cell that starts on its own line and needs wrapping.\n|\nMore text with several words to wrap.\n|===',
  '[cols="1,1"]\n|===\n|First cell\n|\n\n|Empty cells remain empty. |Another long cell with several words.\n|===',
  '[cols="1,1"]\n|===\n|A sentence with link:https://example.org[label with spaces] and `inline code with spaces`.\n|Another cell with several words.\n|===',
]

let checks = 0
for (const text of documents) {
  for (const column of [12, 24, 40, 72, 0]) {
    const result = wrap(text, column)
    assert.equal(await render(result), await render(text), `Meaning changed at column ${column}:\n${text}\n=>\n${result}`)
    assert.equal(wrap(result, column), result, `Wrapping is not idempotent at column ${column}:\n${result}`)
    checks++
  }
}
console.log(`AsciiDoc render equivalence and idempotence: ${checks} cases passed.`)
