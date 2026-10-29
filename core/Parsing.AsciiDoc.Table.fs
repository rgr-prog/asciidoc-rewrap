/// Wrap PSV cell prose without normalizing table markers or render attributes.
module internal Parsing.AsciiDocTable

open System
open System.Text.RegularExpressions
open Prelude
open Rewrap
open Parsing_
open Line

type private Paragraph = { first: int; finish: int }

let private attribute name (attributes: string) =
  let m = Regex("(?:^|[,\\[])\\s*" + name + "\\s*=\\s*(?:\"([^\"]*)\"|'([^']*)'|([^,\\]]*))").Match(attributes)
  if not m.Success then None else
    [1; 2; 3] |> List.tryPick (fun i -> if m.Groups[i].Success then Some m.Groups[i].Value else None)

let private style (specifier: string) =
  let m = Regex(@"[adehlmsv]$").Match(specifier)
  if m.Success then m.Value else ""

let private protectedStyle value = List.contains value ["a"; "l"; "v"]

// A specifier belongs to the following separator, never to the preceding cell.
let private specifier = Regex(@"(?:\d+(?:\.\d+)?[+*]|\.\d+\+)?[<^>]?(?:\.[<^>])?[adehlmsv]?$")

let private paragraphs (text: string) (first: int) (finish: int) =
  let result = ResizeArray<Paragraph>()
  let mutable pos = first
  let mutable start = first
  while pos < finish do
    let eol = text.IndexOf('\n', pos)
    let stop = if eol < 0 || eol > finish then finish else eol
    if String.IsNullOrWhiteSpace(text.Substring(pos, stop - pos)) then
      if start < pos then result.Add({first = start; finish = pos})
      start <- min finish (stop + 1)
    pos <- min finish (stop + 1)
  if start < finish then result.Add({first = start; finish = finish})
  result |> Seq.toList

/// Source lines sharing a cell belong to one block. Selected subranges retain
/// their original cell styles and boundaries, even when the marker is unselected.
type private CellsBlock(original: Line[], spans: Paragraph list, freeze: string -> string) =
  inherit NewBlock(BlockType.Wrap)
  let source = original |> Array.map Line.toString |> String.concat "\n"
  let offsets = original |> Array.scan (fun n l -> n + (Line.toString l).Length + 1) 0

  override _.output ctx selected =
    let selectedLines = selected |> Seq.toArray
    let firstLine = original |> Array.findIndex (fun line -> line = selectedLines[0])
    let first = offsets[firstLine]
    let finish = offsets[firstLine + selectedLines.Length] - 1
    let mutable result = ""
    let mutable consumed = first
    for span in spans do
      let mutable a = max first span.first
      let mutable b = min finish span.finish
      while a < b && Char.IsWhiteSpace source[a] do a <- a + 1
      while b > a && Char.IsWhiteSpace source[b - 1] do b <- b - 1
      if a < b then
        result <- result + source.Substring(consumed, a - consumed)
        let content = source.Substring(a, b - a)
        let contentLines = content.Split('\n')
        // Directives and hard breaks are line-oriented, even inside cells.
        let sensitive = contentLines |> Array.exists (fun s ->
          s.TrimEnd().EndsWith(" +") || Regex.IsMatch(s, @"^\s*(?://|(?:include|ifdef|ifndef|ifeval|endif)::)"))
        if sensitive then result <- result + content else
          let split = result.LastIndexOf('\n') + 1
          let prefix = result.Substring(split)
          let firstWord = Regex(@"^\S+").Match(content).Value
          let width = ctx.settings.column
          // If a previous cell has filled the line, leave the next marker in
          // place and begin its prose below it, instead of overflowing the limit.
          let overflow = width > 0 && prefix.Trim().Length > 1 &&
                         strWidth ctx.settings.tabWidth (prefix + firstWord) > width
          let head = if overflow then "" else prefix
          let buffer = Wrapping.OutputBuffer(ctx.settings)
          buffer.wrap(Nonempty(head, [""]), contentLines |> Array.map (fun s -> freeze (s.Trim())) |> Nonempty.fromSeqUnsafe)
          let wrapped = buffer.toEdit().lines |> String.concat "\n"
          result <- if overflow then result + "\n" + wrapped else result.Substring(0, split) + wrapped
        consumed <- b
    result <- result + source.Substring(consumed, finish - consumed)
    ctx.output.replace(selectedLines.Length, result.Split('\n'))

/// None means an unsupported data format or ambiguous metadata: preserve it.
/// Literal/AsciiDoc column models and preprocessed table data stay untouched.
let tryParse (freeze: string -> string) (attributes: string) (fence: string) (body: string[]) : Block list option =
  let format = attribute "format" attributes |> Option.defaultValue "psv"
  let separator = attribute "separator" attributes |> Option.defaultValue (fence.Substring(0, 1))
  let columns = attribute "cols" attributes |> Option.defaultValue ""
  let protectedColumns = columns.Split(',') |> Array.exists (style >> protectedStyle)
  if format <> "psv" || separator.Length <> 1 || String.IsNullOrWhiteSpace separator || protectedColumns ||
     Regex.IsMatch(attributes, @"\[(?:csv|dsv|tsv)(?:,|\])|hardbreaks|format\s*=\s*\{") ||
     Regex.IsMatch(columns, @"[{}]") ||
     (body |> Array.exists (fun s -> Regex.IsMatch(s, @"^\s*(?://|(?:include|ifdef|ifndef|ifeval|endif)::)"))) then None else
  let text = String.concat "\n" body
  let sep = separator[0]
  let markers = ResizeArray<int * int * string>()
  for i in 0 .. text.Length - 1 do
    if text[i] = sep && (i = 0 || text[i - 1] <> '\\') then
      let lineStart = text.LastIndexOf('\n', max 0 (i - 1)) + 1
      let spec = specifier.Match(text.Substring(lineStart, i - lineStart))
      // Only a standalone token is a cell specifier. A word ending in 'a'
      // immediately before a pipe is still content, not an AsciiDoc style.
      let start = lineStart + spec.Index
      let valid = start = lineStart || Char.IsWhiteSpace text[start - 1]
      let markerStart = if valid then start else i
      markers.Add(markerStart, i + 1, if valid then style spec.Value else "")
  if markers.Count = 0 then None else
  let firstRow = body |> Array.tryFindIndex (not << String.IsNullOrWhiteSpace) |> Option.defaultValue 0
  let implicitHeader = firstRow + 1 < body.Length && String.IsNullOrWhiteSpace body[firstRow + 1] &&
                       not (Regex.IsMatch(attributes, @"(?:\bheader\b|%header)"))
  // Asciidoctor infers the header (and sometimes column count) from the first
  // physical row. Wrapping it without explicit metadata changes rendering.
  let protectedPrefix =
    if columns = "" || implicitHeader then body |> Array.take (firstRow + 1) |> Array.sumBy (fun s -> s.Length + 1)
    else 0
  let spans = ResizeArray<Paragraph>()
  for n in 0 .. markers.Count - 1 do
    let _, first, cellStyle = markers[n]
    let finish = if n + 1 < markers.Count then let start, _, _ = markers[n + 1] in start else text.Length
    if not (protectedStyle cellStyle) then
      for span in paragraphs text (min finish (max first protectedPrefix)) finish do spans.Add(span)
  // Split only at a cell boundary at the beginning of a source line. This
  // leaves cells written on one physical row together, preserving their layout.
  let starts = ResizeArray<int>()
  starts.Add(0)
  let mutable line = 0
  let mutable offset = 0
  for s in body do
    let beginsCell =
      markers |> Seq.exists (fun (start, _, _) ->
        start >= offset && start <= offset + s.Length &&
        String.IsNullOrWhiteSpace(text.Substring(offset, start - offset)))
    if line > 0 && beginsCell then
      starts.Add(line)
    offset <- offset + s.Length + 1
    line <- line + 1
  starts.Add(body.Length)
  [for n in 0 .. starts.Count - 2 do
    let a, b = starts[n], starts[n + 1]
    let lines = body[a .. b - 1] |> Array.map (fun s -> Line("", s))
    let first = body |> Array.take a |> Array.sumBy (fun s -> s.Length + 1)
    let finish = first + (lines |> Array.sumBy (fun l -> l.content.Length + 1)) - 1
    let local = spans |> Seq.choose (fun p ->
      let x, y = max first p.first, min finish p.finish
      if x < y then Some {first = x - first; finish = y - first} else None) |> Seq.toList
    yield NBlock(CellsBlock(lines, local, freeze), Nonempty.fromSeqUnsafe lines)] |> Some
