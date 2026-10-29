# AsciiDoc table cell wrapping

> language: asciidoc

Wrap multiline cell prose

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one two                                                                            |one two three four five
    three four five six seven                                                           six seven
    |eight nine                                                                         |eight nine ten
    ten                                                                                 |===
    |===

Keep inline cell markers inline

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one two three four five six |seven eight nine ten eleven twelve                    |one two three four five
    |===                                                                                six |seven eight nine
                                                                                        ten eleven twelve
                                                                                        |===

Keep cell specs and literal cells

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    >s|one two three four five six                                                      >s|one two three four
    l|literal one                                                                       five six
      literal two                                                                       l|literal one
    |===                                                                                  literal two
                                                                                        |===

Keep paragraphs and row spacing

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one two                                                                            |one two three four
    three four
                                                                                        five six seven eight
    five six                                                                            |nine ten eleven
    seven eight                                                                         |===
    |nine ten
    eleven
    |===

Cursor wraps its cell

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one two«»                                                                          |one two three four
    three four                                                                          |five six
    |five six                                                                           seven eight
    seven eight                                                                         |===
    |===

Partial selection keeps marker outside selection

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one two                                                                            |one two
    «three                                                                              three four
    four»                                                                               |five six
    |five six                                                                           seven eight
    seven eight                                                                         |===
    |===

Partial selection retains literal style

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    l|literal one                                                                       l|literal one
    «  literal two                                                                        literal two
      literal three»                                                                      literal three
    |five six                                                                           |five six
    seven eight                                                                         seven eight
    |===                                                                                |===

Selections in multiple cells

    [cols="1,1"]            ¦                                                        -> [cols="1,1"]            ¦
    |===                                                                                |===
    |one«» two                                                                          |one two three four
    three four                                                                          |five six seven eight
    |five«» six                                                                         |===
    seven eight
    |===

Custom separator

    [cols="1,1",separator="!"]                                                       -> [cols="1,1",separator="!"]
    |===                    ¦                                                           |===                    ¦
    !one two                                                                            !one two three four
    three four                                                                          !five six seven eight
    !five six                                                                           |===
    seven eight
    |===

Preserve implicit header

    |===                    ¦                                                        -> |===                    ¦
    |one two three four five six |seven eight                                           |one two three four five six |seven eight
                            ¦                                                                                   ¦
    |nine ten                                                                           |nine ten eleven twelve
    eleven twelve                                                                       |thirteen fourteen
    |thirteen                                                                           |===
    fourteen
    |===

Preserve CSV row data

    [format=csv]            ¦                                                        -> [format=csv]            ¦
    |===                                                                                |===
    "one two three four five six",second                                                "one two three four five six",second
    |===                                                                                |===
                            ¦                                                                                   ¦
    one two                                                                             one two three
    three

Protected rich column model

    [cols="a,1"]            ¦                                                        -> [cols="a,1"]            ¦
    |===                                                                                |===
    |* first item                                                                       |* first item
    * second item                                                                       * second item
    |one two three four five six                                                        |one two three four five six
    |===                                                                                |===
                            ¦                                                                                   ¦
    one two                                                                             one two three
    three
