/// AsciiDoc-specific block boundaries. Adapted fork of Rewrap Revived.
module internal Parsing.AsciiDoc

open System
open System.Text.RegularExpressions
open Prelude
open Rewrap
open Parsing_

let private matches (pattern: string) (text: string) = Regex.IsMatch(text, pattern)

let private delimiter (text: string) =
  matches @"^(?:\|={3,}|!={3,}|,={3,}|:={3,}|={4,}|\*{4,}|_{4,}|-{4,}|\.{4,}|\+{4,}|/{4,}|--|`{3,}.*)$" text

let private opaqueDelimiter (text: string) =
  matches @"^(?:[|!,:]={3,}|-{4,}|\.{4,}|\+{4,}|/{4,}|`{3,}.*)$" text

let private metadata text = matches @"^(?:\[.*\]|\.[^ .\t].*)$" text

let private structural text =
  matches @"^(?:={1,6}\s+|//|:[^:]+:|\+\s*$|(?:'{3,}|<{3,})\s*$|--\s+|[A-Za-z][\w-]*::.*\[.*\]\s*$)" text

// Lists include nested bullets, ordered lists, callouts and description lists.
// Capture the marker separately so generated continuation lines never repeat it.
let private listMarker =
  Regex(@"^(\s*(?:\*+|-|\.+|\d+[.)]|<[\d.]+>)\s+(?:\[[ xX*]\]\s+)?|\s*.+?(?::{2,4}|;;)(?:\s+|$))")

let private verbatimStyle text =
  matches @"^\[(?:source|listing|literal|verse|pass|stem|latexmath|asciimath)(?:[,.%#\] ]|$)" text

// An inline macro's bracketed text and inline code may contain spaces that
// must not become newlines. Use the wrapping engine's reversible space escape.
let private freezeInline text =
  let frozen =
    Regex(@"(?:[A-Za-z][\w-]*:[^\s\[]*\[[^\]]*\]|<<[^>]+>>|`[^`]+`|\{[^}]+\})")
      .Replace(text, MatchEvaluator(fun m -> Wrapping.freezeSpaces m.Value))
  // A newly generated line must not turn inline text into a block attribute,
  // heading, list, directive or block title. Keep its first token attached to
  // the preceding word. Never add escapes that would change the source text.
  Regex(@"\s+(?=(?:[\[.>]|//|:[^:]+:|[=*+-]+(?:\s|$)|\d+[.)]\s|<\d+>\s|\S+(?::{2,4}|;;)(?:\s|$)|[A-Za-z][\w-]*::))")
    .Replace(frozen, MatchEvaluator(fun m -> Wrapping.freezeSpaces m.Value))

let parse (_settings: Settings) (input: Nonempty<string>) : Blocks =
  let lines = input |> Seq.toArray
  let blocks = ResizeArray<Block>()
  let mutable index = 0
  let mutable preserveNext = false
  let mutable headerPossible = true
  // Exact matching matters: a longer nested delimiter is a different block.
  let mutable containers : string list = []

  let noWrap first count =
    blocks.Add(NoWrap (lines[first .. first + count - 1] |> Nonempty.fromSeqUnsafe))

  let wrap first count (prefix: string) =
    let contents =
      lines[first .. first + count - 1]
      |> Array.mapi (fun n text ->
          (if n = 0 then text.Substring(prefix.Length) else text.TrimStart())
          |> freezeInline)
      |> Nonempty.fromSeqUnsafe
    let continuation = String(' ', prefix.Length)
    blocks.Add(Wrap ((prefix, continuation), contents))

  let boundary (text: string) =
    String.IsNullOrWhiteSpace text || delimiter text || metadata text
    || structural text || listMarker.IsMatch text || text.StartsWith(">")

  while index < lines.Length do
    let first = index
    let text = lines[index]
    if String.IsNullOrWhiteSpace text then
      noWrap index 1
      preserveNext <- false
      index <- index + 1
    elif headerPossible && matches @"^=\s+" text then
      // Author and revision lines immediately following a document title are
      // line-oriented metadata too. A blank line terminates the header.
      index <- index + 1
      while index < lines.Length && not (String.IsNullOrWhiteSpace lines[index]) do
        index <- index + 1
      noWrap first (index - first)
      headerPossible <- false
    elif delimiter text then
      headerPossible <- false
      let closes = List.tryHead containers = Some text
      if closes then
        containers <- List.tail containers
        noWrap index 1
        index <- index + 1
      elif opaqueDelimiter text || preserveNext then
        let closing = if text.StartsWith("```") then String('`', text |> Seq.takeWhile ((=) '`') |> Seq.length) else text
        index <- index + 1
        while index < lines.Length && lines[index] <> closing do index <- index + 1
        if index < lines.Length then index <- index + 1
        let table = matches @"^[|!]={3,}$" text && not preserveNext && index <= lines.Length && lines[index - 1] = closing
        let mutable metadataStart = first
        while metadataStart > 0 && metadata lines[metadataStart - 1] do metadataStart <- metadataStart - 1
        let attributes = lines[metadataStart .. first - 1] |> String.concat ","
        let parsed =
          if table && index > first + 2 then
            AsciiDocTable.tryParse freezeInline attributes text lines[first + 1 .. index - 2]
          else None
        match parsed with
        | Some cellBlocks ->
            noWrap first 1
            for block in cellBlocks do blocks.Add(block)
            noWrap (index - 1) 1
        | None -> noWrap first (index - first)
      else
        containers <- text :: containers
        noWrap index 1
        index <- index + 1
      preserveNext <- false
    elif metadata text then
      // Anchors, roles, titles and subsequent attribute lines may be stacked.
      preserveNext <- preserveNext || verbatimStyle text
      noWrap index 1
      index <- index + 1
    elif structural text then
      // Attribute entries can continue onto subsequent lines with a backslash.
      index <- index + 1
      if text.StartsWith(":") then
        while index < lines.Length && lines[index - 1].TrimEnd().EndsWith("\\") do
          index <- index + 1
      noWrap first (index - first)
    elif text.StartsWith(">") then
      // Asciidoctor also accepts > quotes. Preserve their nested structure,
      // including attribution and embedded lists, without invoking Markdown.
      headerPossible <- false
      index <- index + 1
      while index < lines.Length && lines[index].StartsWith(">") do index <- index + 1
      noWrap first (index - first)
      preserveNext <- false
    else
      headerPossible <- false
      let marker = listMarker.Match text
      let literal = preserveNext || (not marker.Success && matches @"^\s" text)
      index <- index + 1
      while index < lines.Length && not (boundary lines[index]) do index <- index + 1
      let hardBreak = lines[first .. index - 1] |> Array.exists (fun s -> s.TrimEnd().EndsWith(" +"))
      // Literal/verse paragraphs and explicit line breaks carry meaningful
      // whitespace. Protect the whole paragraph, even for partial selections.
      if literal || hardBreak then noWrap first (index - first)
      else wrap first (index - first) (if marker.Success then marker.Value else "")
      preserveNext <- false

  blocks |> Nonempty.fromSeqUnsafe

let document : DocumentProcessor = toNewDocProcessor parse
