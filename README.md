# AsciiDoc Rewrap

<img src="https://raw.githubusercontent.com/rgr-prog/asciidoc-rewrap/master/vscode/asciidoc-rewrap.png" width="128" height="128" alt="AsciiDoc Rewrap" />

Extensão para VS Code com suporte específico para ajustar a largura de texto em arquivos `.adoc`, respeitando a estrutura do AsciiDoc usado pelo Asciidoctor. O suporte às outras linguagens do projeto de origem também foi mantido. Mantido por **Rogério Ferreira da Cunha**. Fork de [Rewrap Revived](https://github.com/dnut/rewrap), que deriva do [Rewrap](https://github.com/stkb/Rewrap).

## Uso

Abra um arquivo `.adoc` e pressione **Alt+Q**, ou execute **AsciiDoc Rewrap: Wrap Text**. Sem seleção, ajusta o parágrafo ou item onde está o cursor. Selecione um trecho ou o documento inteiro para ajustar vários blocos. Documentos AsciiDoc ainda não salvos também são aceitos.

**AsciiDoc Rewrap: Wrap / Unwrap at Column...** permite informar uma coluna; deixe em branco para juntar as linhas de cada parágrafo, respeitando os blocos protegidos.

A largura segue, nesta ordem, `asciidocRewrap.wrappingColumn`, `editor.rulers` e `editor.wordWrapColumn`. Com vários rulers, repetir o comando alterna entre as larguras.

```json
{
  "[asciidoc]": {
    "asciidocRewrap.wrappingColumn": 80,
    "asciidocRewrap.onSave": false,
    "asciidocRewrap.autoWrap.enabled": false
  }
}
```

Os comandos, ajustes ao salvar e auto-wrap também continuam disponíveis nas outras linguagens herdadas do Rewrap Revived, incluindo Markdown e comentários de código. A extensão pode coexistir com Rewrap Revived: os comandos e as configurações têm um namespace próprio. Se ambas usarem Alt+Q, configure a preferência nos atalhos do VS Code.

## Sintaxe tratada

- Parágrafos, itens de listas simples e aninhadas (`*`, `-`, `.`, `1.`, `<1>`), checklists e listas de descrição (`::`, `:::`, `::::`, `;;`). Cada item mantém seu marcador e sua separação.
- Títulos de documento e seções, autor e revisão no cabeçalho, âncoras (`[[id]]`), atributos, títulos de bloco e metadados permanecem em linhas próprias.
- Citações `[quote]` e blocos `____`: ajusta o texto e preserva os delimitadores e a atribuição. Blocos de exemplo, sidebar e open blocks também permitem ajustar a prosa interna.
- Tabelas PSV (`|===` e `!===`): ajusta a prosa de cada célula à coluna configurada, incluindo células com várias linhas e várias células na mesma linha. Preserva delimitadores, separadores, especificadores, espaçamento entre parágrafos e atributos como `cols`, `options` e alinhamento. Não alinha visualmente as colunas nem redistribui a largura de renderização. Células explicitamente literais ou AsciiDoc são preservadas. Tabelas CSV/DSV/TSV, tabelas com diretivas ou modelos de coluna literais/AsciiDoc permanecem intactas. A primeira linha é preservada quando necessária para manter a inferência do cabeçalho ou das colunas.
- Blocos de código, literais, passthrough, comentários e versos ficam intactos. Delimitadores de abertura e fechamento precisam coincidir; um bloco literal sem fechamento é protegido até o fim do arquivo.
- Parágrafos com quebra explícita ` +`, parágrafos literais indentados e citações com `>` são preservados.
- Diretivas, macros de bloco e referências de atributos são preservadas; espaços dentro de macros inline e trechos entre crases não viram quebras de linha.

Este é um parser de estruturas para edição do texto fonte, sem executar includes, condicionais ou extensões do Asciidoctor. O wrap de células se limita à prosa em tabelas PSV, preservando os modelos de conteúdo que exigem outro processamento. Sintaxes personalizadas e atributos globais que alteram o modelo de conteúdo podem exigir regras adicionais. Os parsers e o suporte às outras linguagens permanecem disponíveis na extensão e são cobertos pela suíte de regressão herdada.

Referências usadas: [estrutura de blocos](https://docs.asciidoctor.org/asciidoc/latest/blocks/), [delimitadores](https://docs.asciidoctor.org/asciidoc/latest/blocks/delimited/), [listas compostas](https://docs.asciidoctor.org/asciidoc/latest/lists/continuation/) e [citações](https://docs.asciidoctor.org/asciidoc/latest/blocks/blockquotes/).

## Desenvolvimento e instalação local

Pré-requisitos: .NET SDK 10, Node.js e Python 3. O runtime do .NET sozinho não compila o núcleo F#.

```sh
npm ci
npm ci --prefix vscode
python do build
python do test
python do package
```

O pacote será criado em `dist/asciidoc-rewrap-0.1.2.vsix`. Instale pelo comando **Extensions: Install from VSIX...** do VS Code. Para depurar, abra o projeto e use a configuração **Extension** (F5).

Os exemplos em `docs/specs/content-types/asciidoc.md` e `asciidoc-tables.md` são testes executáveis. A comparação com o Asciidoctor verifica a renderização e a estabilidade ao repetir o wrap em diferentes larguras. Os demais exemplos verificam o comportamento do núcleo herdado. Os testes de integração verificam comandos, seleções, auto-wrap e o suporte às outras linguagens.

## Identidade e publicação

- Repositório: [rgr-prog/asciidoc-rewrap](https://github.com/rgr-prog/asciidoc-rewrap), fork de Rewrap Revived.
- Autor e mantenedor: **Rogério Ferreira da Cunha**.
- Publisher do Marketplace: `RogerioCunha`.
- ID da extensão: `RogerioCunha.asciidoc-rewrap`.
- Bugs e sugestões: [issues deste fork](https://github.com/rgr-prog/asciidoc-rewrap/issues).

Para publicar no Visual Studio Marketplace, gere o VSIX com `python do package` e envie-o pelo painel do publisher **RogerioCunha**. Com a autenticação VSCE configurada, `python do publish` publica no Marketplace. O comando não publica em outros registries.

O ícone combina o desenho do Rewrap com um selo do Asciidoctor no canto inferior direito para identificar o suporte ao formato AsciiDoc. Os arquivos originais foram preservados; as fontes e os créditos visuais estão em [assets/README.md](https://github.com/rgr-prog/asciidoc-rewrap/blob/master/assets/README.md). O histórico e os créditos dos projetos de origem foram preservados.

## Créditos e licença

Agradecimentos a **Steve Baker (@stkb)**, criador do Rewrap, e **Drew Nutter (@dnut)**, mantenedor do Rewrap Revived, pelo mecanismo de wrapping, integração com os editores e testes usados como base.

Esta derivação modifica o processamento de AsciiDoc, mantém o suporte às demais linguagens e adota identidade e configurações próprias. Os avisos de copyright e os arquivos `LICENSE` originais foram preservados. O código segue a licença Apache 2.0 do projeto de origem. A pasta `docs` e a integração histórica com Visual Studio em `vs` são material herdado; a extensão deste fork é a de `vscode`.
