# AsciiDoc

> language: asciidoc

Prose

    one two                 ¦                                         -> one two three four five ¦
    three four five six seven                                            six seven

Separate unordered items

    * one two               ¦                                         -> * one two three four
    three four five six seven                                              five six seven        ¦
    * eight nine                                                         * eight nine ten
    ten

Nested lists and checklists

    * parent                ¦                                         -> * parent                ¦
    ** child one                                                         ** child one two
    two                                                                  * [x] done one two
    * [x] done one                                                       - dash one two
    two
    - dash one
    two

Ordered and callout lists

    . one two               ¦                                         -> . one two three         ¦
    three                                                                .. nested one two
    .. nested one                                                        1. numbered one two
    two                                                                  <1> callout one two
    1. numbered one
    two
    <1> callout one
    two

Description lists

    Term:: one two          ¦                                         -> Term:: one two three    ¦
    three                                                                Other::: four five
    Other::: four                                                        Last;; six seven
    five
    Last;; six
    seven

Metadata and headings

    [[anchor]]              ¦                                         -> [[anchor]]              ¦
    == Section                                                           == Section
    [#id.role]                                                           [#id.role]
    .A block title                                                       .A block title
    one two                                                              one two three
    three                                                                === Next
    === Next                                                             four five six
    four five
    six

Document header

    = Title                 ¦                                         -> = Title                 ¦
    Doc Writer <a@b.test>                                                Doc Writer <a@b.test>
    v1.0, 2026-10-29                                                     v1.0, 2026-10-29
    :toc:                                                                :toc:
                            ¦                                                                    ¦
    one two                                                              one two three
    three

Quote block

    [quote,An Author]       ¦                                         -> [quote,An Author]       ¦
    ____                                                                 ____
    one two                                                              one two three
    three
                                                                         four five six
    four five                                                            ____
    six                                                                  one two three
    ____
    one two
    three

Quote paragraph and attribution

    [quote,An Author]       ¦                                         -> [quote,An Author]       ¦
    one two                                                              one two three
    three
                                                                         "four five six"
    "four five                                                           -- An Author
    six"
    -- An Author

Verse block

    [verse,An Author]       ¦                                         -> [verse,An Author]       ¦
    ____                                                                 ____
    one two                                                              one two
    three                                                                three
    ____                                                                 ____
    one two                                                              one two three
    three

Styled literal paragraph

    [source,ruby]           ¦                                         -> [source,ruby]           ¦
    puts "hello"                                                         puts "hello"
    puts "world"                                                         puts "world"
                            ¦                                                                    ¦
    one two                                                              one two three
    three

Indented literal paragraph

      one two               ¦                                         ->   one two               ¦
      three                                                                three
                            ¦                                                                    ¦
    one two                                                              one two three
    three

Compound lists

    * one                   ¦                                         -> * one two               ¦
    two                                                                  +
    +                                                                    ----
    ----                                                                 code one
    code one                                                             code two
    code two                                                             ----
    ----                                                                 +
    +                                                                    three four
    three                                                                * five six
    four
    * five
    six

Explicit hard breaks

    one two +               ¦                                         -> one two +               ¦
    three four                                                           three four
                            ¦                                                                    ¦
    five six                                                             five six seven
    seven

Directives and attributes

    :name: a value          ¦                                         -> :name: a value          ¦
    include::file.adoc[]                                                 include::file.adoc[]
    ifdef::name[]                                                        ifdef::name[]
    one two                                                              one two three
    three                                                                endif::[]
    endif::[]                                                            image::pic.png[Caption]
    image::pic.png[Caption]                                              four five six
    four five
    six

Markdown-style quotes stay structured

    > one                   ¦                                         -> > one                   ¦
    > two                                                                > two
    > -- Author                                                          > -- Author
                            ¦                                                                    ¦
    one two                                                              one two three
    three

Admonitions

    NOTE: one two           ¦                                         -> NOTE: one two three     ¦
    three
                                                                         four five six
    four five
    six

Partial selection preserves table

    |===                    ¦                                         -> |===                    ¦
    |«one                                                                |one
    |two»                                                                |two
    |===                                                                 |===
                            ¦                                                                    ¦
    one two                                                              one two
    three                                                                three

Cursor wraps only its item

    * one                   ¦                                         -> * one                   ¦
    two                                                                  two
    * three«»                                                            * three four
    four                                                                 * five
    * five                                                               six
    six

Protected |=== block

    |===                    ¦                                         -> |===                    ¦
    one two                                                              one two
    three four                                                           three four
    |===                                                                 |===
    five six                                                             five six seven
    seven

Protected !=== block

    !===                    ¦                                         -> !===                    ¦
    one two                                                              one two
    three four                                                           three four
    !===                                                                 !===
    five six                                                             five six seven
    seven

Protected ,=== block

    ,===                    ¦                                         -> ,===                    ¦
    one two                                                              one two
    three four                                                           three four
    ,===                                                                 ,===
    five six                                                             five six seven
    seven

Protected :=== block

    :===                    ¦                                         -> :===                    ¦
    one two                                                              one two
    three four                                                           three four
    :===                                                                 :===
    five six                                                             five six seven
    seven

Protected ---- block

    ----                    ¦                                         -> ----                    ¦
    one two                                                              one two
    three four                                                           three four
    ----                                                                 ----
    five six                                                             five six seven
    seven

Protected .... block

    ....                    ¦                                         -> ....                    ¦
    one two                                                              one two
    three four                                                           three four
    ....                                                                 ....
    five six                                                             five six seven
    seven

Protected ++++ block

    ++++                    ¦                                         -> ++++                    ¦
    one two                                                              one two
    three four                                                           three four
    ++++                                                                 ++++
    five six                                                             five six seven
    seven

Protected //// block

    ////                    ¦                                         -> ////                    ¦
    one two                                                              one two
    three four                                                           three four
    ////                                                                 ////
    five six                                                             five six seven
    seven

Compound ==== block

    ====                    ¦                                         -> ====                    ¦
    one two                                                              one two three
    three                                                                ====
    ====                                                                 four five six
    four five
    six

Compound **** block

    ****                    ¦                                         -> ****                    ¦
    one two                                                              one two three
    three                                                                ****
    ****                                                                 four five six
    four five
    six

Compound -- block

    --                      ¦                                         -> --                      ¦
    one two                                                              one two three
    three                                                                --
    --                                                                   four five six
    four five
    six

Compound ____ block

    ____                    ¦                                         -> ____                    ¦
    one two                                                              one two three
    three                                                                ____
    ____                                                                 four five six
    four five
    six

Compound ===== block

    =====                   ¦                                         -> =====                   ¦
    one two                                                              one two three
    three                                                                =====
    =====                                                                four five six
    four five
    six

Unterminated table

    one two                 ¦                                         -> one two three           ¦
    three                                                                |===
    |===                                                                 |one
    |one                                                                 |two
    |two

Exact fence lengths

    ----                    ¦                                         -> ----                    ¦
    code one                                                             code one
    -----                                                                -----
    code two                                                             code two
    ----                                                                 ----
    one two                                                              one two three
    three

Nested compound blocks

    ====                    ¦                                         -> ====                    ¦
    =====                                                                =====
    one two                                                              one two three
    three                                                                =====
    =====                                                                four five six
    four five                                                            ====
    six
    ====

Inline macros stay together

    one two link:x[three four five] six                               -> one two                 ¦
                            ¦                                            link:x[three four five]
                                                                         six
