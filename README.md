# AsciiDoc Rewrap

<img src="https://raw.githubusercontent.com/rgr-prog/asciidoc-rewrap/master/vscode/asciidoc-rewrap.png" width="128" height="128" alt="AsciiDoc Rewrap" />

A VS Code extension that hard-wraps text in `.adoc` files while respecting the AsciiDoc structure used by Asciidoctor. Support for the other languages from the upstream project is also retained. Maintained by **Rogério Ferreira da Cunha**. Forked from [Rewrap Revived](https://github.com/dnut/rewrap), which is based on [Rewrap](https://github.com/stkb/Rewrap).

## Usage

Open an `.adoc` file and press **Alt+Q**, or run **AsciiDoc Rewrap: Wrap Text**. With no selection, the command wraps the paragraph or list item at the cursor. Select a range or the entire document to wrap multiple blocks. Unsaved AsciiDoc documents are also supported.

**AsciiDoc Rewrap: Wrap / Unwrap at Column...** lets you choose a wrapping column. Leave it blank to join the lines within each paragraph while preserving protected blocks.

The wrapping width is taken from `asciidocRewrap.wrappingColumn`, `editor.rulers`, or `editor.wordWrapColumn`, in that order. If multiple rulers are configured, running the command repeatedly cycles through their widths.

```json
{
  "[asciidoc]": {
    "asciidocRewrap.wrappingColumn": 80,
    "asciidocRewrap.onSave": false,
    "asciidocRewrap.autoWrap.enabled": false
  }
}
```

The commands, wrap on save, and automatic wrapping remain available for the other languages inherited from Rewrap Revived, including Markdown and code comments. This extension can coexist with Rewrap Revived: its commands and settings use a separate namespace. If both extensions use Alt+Q, choose your preferred binding in VS Code's Keyboard Shortcuts editor.

## Supported syntax

- Paragraphs, simple and nested list items (`*`, `-`, `.`, `1.`, `<1>`), checklists, and description lists (`::`, `:::`, `::::`, `;;`). Each item keeps its marker and remains separate from adjacent items.
- Document and section titles, author and revision information in the document header, anchors (`[[id]]`), attributes, block titles, and metadata stay on their own lines.
- `[quote]` paragraphs and `____` quote blocks: prose is wrapped while delimiters and attribution are preserved. Prose inside example blocks, sidebars, and open blocks can also be wrapped.
- PSV tables (`|===` and `!===`): prose in each cell is wrapped at the configured column, including multiline cells and multiple cells on the same source line. Delimiters, separators, cell specifiers, paragraph spacing, and attributes such as `cols`, `options`, and alignment are preserved. Wrapping does not visually align columns or change their rendered widths. Explicit literal and AsciiDoc cells are preserved. CSV/DSV/TSV tables, tables containing directives, and tables with literal or AsciiDoc column models remain untouched. The first source line is preserved when needed to retain inferred headers or columns.
- Code, literal, passthrough, comment, and verse blocks remain untouched. Opening and closing delimiters must match; an unclosed verbatim block is protected through the end of the file.
- Paragraphs containing explicit line breaks (` +`), indented literal paragraphs, and `>` quotations are preserved.
- Directives, block macros, and attribute references are preserved. Spaces inside inline macros and backtick-delimited text are not turned into line breaks.

This parser identifies source structures for editing without executing includes, conditionals, or Asciidoctor extensions. Table cell wrapping is limited to prose in PSV tables; content models requiring different processing are preserved. Custom syntax and global attributes that change a block's content model may need additional rules. Parsers and support for the other languages remain available and are covered by the inherited regression suite.

Syntax references: [blocks](https://docs.asciidoctor.org/asciidoc/latest/blocks/), [delimiters](https://docs.asciidoctor.org/asciidoc/latest/blocks/delimited/), [compound list items](https://docs.asciidoctor.org/asciidoc/latest/lists/continuation/), and [blockquotes](https://docs.asciidoctor.org/asciidoc/latest/blocks/blockquotes/).

## Development and local installation

Prerequisites: .NET SDK 10, Node.js, and Python 3. The .NET runtime alone cannot compile the F# core.

```sh
npm ci
npm ci --prefix vscode
python do build
python do test
python do package
```

The package is created as `dist/asciidoc-rewrap-<version>.vsix`. Install it using **Extensions: Install from VSIX...** in VS Code. To debug the extension, open the project and launch the **Extension** configuration (F5).

The examples in [`docs/specs/content-types/asciidoc.md`](docs/specs/content-types/asciidoc.md) and [`asciidoc-tables.md`](docs/specs/content-types/asciidoc-tables.md) are executable tests. Comparisons against Asciidoctor check rendered output and stability when wrapping repeatedly at different widths. The other examples cover the inherited core. Integration tests exercise commands, selections, automatic wrapping, and support for the other languages.

## Project and publishing

- Repository: [rgr-prog/asciidoc-rewrap](https://github.com/rgr-prog/asciidoc-rewrap), a fork of Rewrap Revived.
- Author and maintainer: **Rogério Ferreira da Cunha**.
- Marketplace publisher: `RogerioCunha`.
- Extension ID: `RogerioCunha.asciidoc-rewrap`.
- Bugs and suggestions: [this fork's issue tracker](https://github.com/rgr-prog/asciidoc-rewrap/issues).

To publish on the Visual Studio Marketplace, build the VSIX with `python do package` and upload it through the **RogerioCunha** publisher dashboard. Once VSCE authentication is configured, `python do publish` publishes to the Marketplace. This command does not publish to other registries.

The icon combines the Rewrap artwork with an Asciidoctor badge in the lower-right corner to identify AsciiDoc support. The original assets are preserved; visual sources and acknowledgements are documented in [assets/README.md](https://github.com/rgr-prog/asciidoc-rewrap/blob/master/assets/README.md). Upstream history and credits are retained.

## Credits and license

Thanks to **Steve Baker (@stkb)**, the creator of Rewrap, and **Drew Nutter (@dnut)**, the maintainer of Rewrap Revived, for the wrapping engine, editor integration, and tests that this fork builds on.

This fork adds AsciiDoc processing, retains support for the other languages, and uses its own identity and settings. The original copyright notices and `LICENSE` files are preserved. The code remains under the upstream project's Apache 2.0 license. The `docs` directory and historical Visual Studio integration in `vs` are inherited material; this fork's VS Code extension lives in `vscode`.
