# 引き継ぎメモ（一時・後で捨てる）

> このファイルは規約(convention)語彙の横展開作業の一時的な引き継ぎメモです。
> 作業が片付いたら削除してください。

## いまどこまで済んだか

opinio 側で規約検査の語彙(vocabulary)を大きく磨き上げ、その正本を animo の
`develop` ブランチに横展開している途中です。

opinio の正本(master に push 済み)の要点:

+ 語彙を出自ごとに分割・命名統一した。`basic_words`(Ogden Basic English 850)、
  `plain_words`(プロジェクト固有の平易語)、`lang_words`(言語キーワード・技術名)、
  `letter_words`(頭字語・全大文字表記)、`unit_words`、`single_words`、
  `project_words`、`tech_terms`(定義辞書)の 8 分類。
+ `extern` メソッドのパラメータ名(`wparam`/`lparam` 等の外部由来)を検査除外する
  `in_extern_member` を実装した。
+ 語彙を「原形＋活用形の横並び」形式にした
  (例 `+ close closes closed closing closer closest`)。
  `load_words` をスペース区切りの複数語対応にした。
+ 助動詞・be動詞・代名詞・不規則動詞の活用を集約した
  (`be is are was were been being am`、`can could`、`child children`、
  `go goes went gone going` 等)。原形が欠けていた語に原形を立てた
  (`thinking`→`think`、`bytes`→`byte bytes`)。
+ 活用形の生成方法は opinio の `docs/standard/word_forms.md` に仕様として集約した。
  生成器プログラムは作らない。この仕様書を LLM へのプロンプトとして使い、
  語を足す・直すときはチャットで LLM に頼む運用。

## animo への横展開でいま起きていること（develop ブランチ）

最新 opinio 正本を animo develop に移植してテストを回したところ、命名違反 364 件・
ファイル名違反 6 件が出た。これは想定どおりの「破壊」で、内訳は 2 種類:

+ animo コードの略語汚染 = `src`(9)、`kv`(3)、`infs`(3)、`dup`(2)、`inf`(1)、
  `deg`(1)、`adj`(1)、`topo`(1)。これは animo のコードをフルスペルに是正すべき対象。
+ 正当な語だが正典に無い = `meta`、`thresholds`(threshold)、`incoming`。
  これは隔離ファイルに足すべき正当語。

## 次にやること（ここから再開）

各リポジトリ固有の未知語は、正典(basic/plain)にいきなり混ぜず、
`draft_words.md`(下書き=正典入り前の隔離ファイル)で隔離する運用を確立中。

+ animo に `draft_words.md` を作る。置き場は他の語彙ファイルと同じ
  `Tests~/EditModeTests/Convention/vocabulary/`。
+ そこに**正当な語だけ**を隔離する。今回なら `threshold`(thresholds)・
  `incoming`・`meta` の 3 語。
+ **略語(src/kv/inf/infs/dup/deg/adj/topo)は draft に入れない。**
  これらは animo のコードを是正する対象。draft に入れたら水増しになる
  (前任の失敗の再発)。
+ `ConventionRules.cs` に `draft_words.md` の読み込みを追加する。
+ draft の語を精査し、汎用的に正しいと合格したものだけ opinio 正典に昇格させる。
  この「隔離 → 精査 → 昇格」のフローを、あとで opinio の `word_forms.md`
  (または別の standard 文書)に明文化する。

`draft_words.md` を作って正当 3 語を隔離し、`ConventionRules.cs` に読み込みを
追加する作業の直前で中断した。ここから再開する。

## テストの回し方

opinio・animo とも Convention テストはサンドボックスで回せる。

+ animo: `dotnet test Tests~/EditModeTests/Animo.Tests.EditMode.csproj
  -p:UseSdkRoslyn=true`
+ nuget オフライン対策に `-p:UseSdkRoslyn=true` が必須。
+ ローカル nupkg フィードは `/home/claude/localnuget/nupkg`。
+ `nuget.config` は環境依存(絶対パス)なので gitignore 済み。コミットしない。

## リポジトリの状態

+ opinio は master に push 済み(語彙・仕様・ConventionRules すべて反映済み)。
+ animo は `develop` ブランチに最新正本の移植を適用済みだが未コミット。
  master は手つかず・日常運用中。master には触っていない。
