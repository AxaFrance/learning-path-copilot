---
id: evals-llm-juge
title: "315 — Evals avancées (LLM-juge & métriques graduées)"
sidebar_position: 315
description: "Dépasser le pass/fail binaire avec des métriques graduées et un LLM-juge — et apprendre à le calibrer contre des labels humains."
---

# 315 — Evals avancées (LLM-juge & métriques graduées)

Durée estimée : 90 min · Complexité : ⭐⭐⭐⭐ · Pré-requis : [Module 310 — Evals binaires](./310-evals.md)

> Au module 310, tu as appris à répondre « oui ou non ». Mais comment évalues-tu si une réponse est *utile*, *fidèle à la source* ou *dans le bon ton* — quand aucune regex ne peut trancher ?

## Pourquoi ce module

Le module 310 t'a donné une discipline solide : une `eval` binaire pose une assertion booléenne — `contains "feat"`, `matches ^feat\(.+\):`, `max_length 72` — et le verdict est `pass` ou `fail`, jamais « plutôt bien ». Cette rigueur est précieuse parce qu'elle rend les résultats comparables et détecte les régressions au caractère près.

Mais elle a un angle mort. Beaucoup des qualités qui comptent vraiment dans une sortie de LLM ne sont pas mécaniques. Une réponse peut contenir tous les bons mots-clés et rester froide, condescendante ou hors sujet. Un résumé peut passer ta regex et trahir le document source. Un message de support peut respecter ta longueur maximale et être inutilement agressif. Le ton, l'utilité (*helpfulness*), la fidélité à une source (*faithfulness*), le respect d'un style — ce sont des dimensions **ouvertes et subjectives** qu'aucune assertion `contains` ne capture.

C'est exactement le problème qu'attaque la recherche sur l'évaluation des assistants conversationnels : les benchmarks classiques mesurent mal les préférences humaines sur des questions ouvertes.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "Evaluating large language model (LLM) based chat assistants is challenging due to their broad capabilities and the inadequacy of existing benchmarks in measuring human preferences."
> Fetched: 2026-06-10

Deux outils répondent à ce vide, et ce module les enseigne tous les deux : les **métriques graduées** (un score sur une échelle, par exemple 1 à 5, au lieu d'un booléen) et le **LLM-juge** (*LLM-as-a-judge*) — un modèle qu'on charge d'attribuer ce score en suivant une grille. Tu vas aussi apprendre la partie que tout le monde oublie : un juge non calibré est une opinion, pas une mesure.

À la fin de ce module, tu sais :

- décider quand une dimension de qualité justifie un score gradué plutôt qu'une assertion binaire ;
- écrire une **grille de notation** (`rubric`) sur une échelle 1–5 et fixer un seuil de réussite ;
- configurer un LLM-juge pointwise et comprendre les variantes pairwise et reference-guided ;
- nommer et neutraliser les biais connus du juge (position, verbosité, self-preference, non-déterminisme) ;
- calibrer ton juge contre un *golden set* de labels humains avant de lui faire confiance.

## Pré-requis

- [Module 310 — Evals binaires](./310-evals.md) — tu dois maîtriser les `fixtures`, les assertions binaires et le delta `with_skill`/`without_skill`. Ce module les **généralise**, il ne les remplace pas.
- [Module 103 — Skills](../01-fondations/103-skills.md) — le sujet sous test reste un `skill` que tu sais créer et déclencher.
- [Module 104 — Agents](../01-fondations/104-agents.md) — le LLM-juge est lui-même un agent évaluateur ; tu vas le configurer dans un fichier `.agent.md`.
- Un dossier `evals/` déjà en place, hérité du module 310.

## Concepts clés

### Pourquoi le binaire ne suffit plus

Une assertion binaire répond à une question fermée : *le format est-il respecté ?*. Tant que ta condition de réussite peut s'écrire sous forme de regex, de présence de chaîne ou de longueur, le binaire est le bon outil — il est déterministe, gratuit et instantané.

Le problème commence quand la condition de réussite est un **jugement**. « Cette réponse est-elle empathique ? », « ce résumé est-il fidèle à l'article ? », « le ton est-il adapté à un public de support client ? » — ces questions n'ont pas de frontière nette. Tu peux tenter de les approximer avec des proxys binaires (« contient le mot *désolé* »), mais ces proxys sont fragiles et facilement contournables : un skill peut apprendre à insérer « désolé » sans être réellement empathique. La documentation d'Anthropic le formule directement quand elle distingue les notations mécaniques des notations qui exigent un véritable jugement.

> Source: https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests
> Citation: "LLM-based grading: Fast and flexible, scalable and suitable for complex judgement."
> Fetched: 2026-06-10

L'idée centrale du module : quand la qualité est subjective, on ne change pas seulement *qui* note (un modèle au lieu d'une regex) — on change *la forme du verdict*. On passe d'un booléen à un **score gradué**.

### Les métriques graduées : la grille (rubric)

Une métrique graduée attribue un score sur une échelle continue ou ordinale au lieu d'un booléen. La plus courante est l'échelle de Likert 1–5, mais on trouve aussi des scores normalisés entre `0.0` et `1.0`. La documentation d'Anthropic recommande explicitement de demander au juge soit une classification binaire, soit une note sur une échelle 1–5 selon la dimension mesurée.

> Source: https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests
> Citation: "instruct the LLM to output only 'correct' or 'incorrect', or to judge from a scale of 1-5."
> Fetched: 2026-06-10

Ce qui fait la qualité d'une grille, ce n'est pas l'échelle — c'est la **description de chaque niveau**. Une grille vague (« note la clarté de 1 à 5 ») produit des scores instables : le juge n'a aucun point d'ancrage. Une bonne grille ancre chaque palier dans un comportement observable. La documentation de promptfoo illustre ce principe avec une grille où chaque niveau est défini par un repère concret.

> Source: https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/llm-rubric/
> Citation: "Grade it on a scale of 0.0 to 1.0, where: Score of 0.1: Only a slight smile. Score of 0.5: Laughing out loud."
> Fetched: 2026-06-10

Anthropic insiste sur le même point : une grille doit être détaillée et sans ambiguïté pour produire des verdicts reproductibles.

> Source: https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests
> Citation: "Have detailed, clear rubrics"
> Fetched: 2026-06-10

Voici une grille graduée pour la dimension *fidélité* d'un skill de résumé, à poser à côté de tes fixtures binaires du module 310 :

```yaml
# evals/summarize-article/rubric.yml

dimension: faithfulness
scale: [1, 5]
levels:
  1: "Le résumé contient au moins une affirmation absente ou contredite par la source."
  2: "Le résumé reste globalement fidèle mais déforme une nuance importante."
  3: "Le résumé est fidèle ; aucune erreur factuelle, mais omet un point clé."
  4: "Le résumé est fidèle et couvre tous les points majeurs."
  5: "Le résumé est fidèle, complet, et préserve les nuances de la source."
threshold: 4   # une fixture passe si le score >= 4
```

**Agrégation et seuil.** Un score gradué ne se lit pas seul : tu fixes un **seuil** (`threshold`) qui le retransforme en pass/fail au moment de décider si une fixture est verte, puis tu **agrèges** les scores du jeu (moyenne, médiane) pour suivre la qualité globale dans le temps. promptfoo formalise ce mécanisme : le seuil compare le score numérique à une valeur minimale.

> Source: https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/llm-rubric/
> Citation: "the output must achieve a score greater than or equal to the threshold to pass"
> Fetched: 2026-06-10

Le seuil est ce qui réconcilie ce module avec le module 310 : en haut, tu raisonnes en scores gradués pour suivre des tendances fines ; en bas, le seuil redonne un verdict binaire pour ta CI. Tu gardes la rigueur du booléen tout en gagnant la granularité du score.

### Le LLM-juge : faire noter par un modèle

Qui attribue le score gradué ? Pour une dimension subjective, ni une regex ni un humain à chaque exécution ne conviennent — l'un est trop bête, l'autre trop cher. La réponse est le **LLM-juge** : un modèle qu'on charge d'évaluer une sortie contre une grille. C'est précisément la proposition de Zheng et al. : utiliser de forts LLM comme juges sur des questions ouvertes.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "we explore using strong LLMs as judges to evaluate these models on more open-ended questions."
> Fetched: 2026-06-10

Concrètement, un juge est un **agent** au sens du module 104 : un persona avec une seule mission — lire une sortie et la noter — défini dans un fichier `.agent.md`. C'est un *prompt de jugement* (*judge prompt*) qui contient la grille, la sortie à évaluer, et le format de réponse attendu (typiquement un JSON avec un score et une justification).

promptfoo nomme ce patron *LLM as a judge* et le matérialise dans son assertion `llm-rubric`, un évaluateur générique piloté par une grille.

> Source: https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/llm-rubric/
> Citation: "llm-rubric is promptfoo's general-purpose grader for \"LLM as a judge\" evaluation."
> Fetched: 2026-06-10

Le juge ne renvoie pas qu'un nombre : il renvoie un score **et** un *reason*, ce qui rend le verdict explicable et débogable.

> Source: https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/llm-rubric/
> Citation: "\"reason\": \"<Analysis of the rubric and the output>\", \"score\": 0.5, // 0.0-1.0 \"pass\": true"
> Fetched: 2026-06-10

#### Trois manières de noter : pointwise, pairwise, reference-guided

Zheng et al. décrivent trois variantes de jugement, et le choix entre elles structure tout ton design d'eval.

**1. Pointwise (single answer grading).** Le juge note une seule sortie dans l'absolu, sur ton échelle. C'est le plus simple et celui qui se branche directement sur tes fixtures du module 310.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "Single answer grading. Alternatively, an LLM judge is asked to directly assign a score to a single answer."
> Fetched: 2026-06-10

**2. Pairwise comparison.** Le juge reçoit deux réponses (par exemple `with_skill` vs `without_skill`) et désigne la meilleure, ou déclare une égalité. C'est exactement le delta du module 310, mais arbitré par un juge au lieu d'un compteur de `pass`.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "An LLM judge is presented with a question and two answers, and tasked to determine which one is better or declare a tie."
> Fetched: 2026-06-10

**3. Reference-guided grading.** On fournit au juge une réponse de référence (un *golden output*) et il note la sortie par rapport à elle. Indispensable quand il existe une bonne réponse connue — calculs, extractions structurées, faits vérifiables.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "Reference-guided grading. In certain cases, it may be beneficial to provide a reference solution if applicable."
> Fetched: 2026-06-10

Quand une réponse de référence existe en texte libre, tu peux aussi mesurer la proximité par des métriques de similarité plutôt que par un juge : OpenAI propose des *text similarity graders* pour exactement ce cas.

> Source: https://platform.openai.com/docs/guides/graders
> Citation: "text similarity graders ... evaluate how close the model-generated output is to the reference, scored with various evaluation frameworks."
> Fetched: 2026-06-10

Ces *graders* de similarité couvrent une famille de métriques classiques que tu peux combiner avec ton juge selon la dimension.

> Source: https://platform.openai.com/docs/guides/graders
> Citation: "fuzzy_match | bleu | gleu | meteor | cosine | rouge_1 | rouge_2 | rouge_3 | rouge_4 | rouge_5 | rouge_l"
> Fetched: 2026-06-10

**Choisir.** Pairwise est plus stable que pointwise (les scores absolus fluctuent plus que les comparaisons relatives) mais il passe mal à l'échelle : le nombre de paires croît de manière quadratique avec le nombre de candidats.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "the pairwise comparison may lack scalability when the number of players increases, given that the number of possible pairs grows quadratically"
> Fetched: 2026-06-10

Règle pratique : **pointwise** pour suivre une qualité absolue en CI, **pairwise** pour départager deux versions d'un skill, **reference-guided** dès qu'un golden output existe.

#### Le patron form-filling + chain-of-thought

Un juge qui crache un chiffre nu est instable. Le framework G-Eval montre qu'on obtient de bien meilleures corrélations avec l'humain en demandant au juge de **raisonner d'abord** (chain-of-thought) puis de **remplir un formulaire** de notation (form-filling).

> Source: https://arxiv.org/abs/2303.16634
> Citation: "G-Eval, a framework of using large language models with chain-of-thoughts (CoT) and a form-filling paradigm, to assess the quality of NLG outputs."
> Fetched: 2026-06-10

Sur la tâche de résumé, G-Eval avec GPT-4 atteint une corrélation de Spearman de 0,514 avec l'humain, surpassant nettement les métriques antérieures — la preuve qu'un juge bien construit approche le jugement humain.

> Source: https://arxiv.org/abs/2303.16634
> Citation: "G-Eval with GPT-4 as the backbone model achieves a Spearman correlation of 0.514 with human on summarization task, outperforming all previous methods by a large margin."
> Fetched: 2026-06-10

C'est pourquoi ton judge prompt demande toujours le *reason* avant le *score*.

### Les biais du juge — et leurs parades

Un LLM-juge n'est pas un instrument neutre. Zheng et al. cataloguent plusieurs biais systématiques qu'il faut connaître pour ne pas se mentir.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "We examine the usage and limitations of LLM-as-a-judge, including position, verbosity, and self-enhancement biases, as well as limited reasoning ability."
> Fetched: 2026-06-10

**Biais de position** — en mode pairwise, le juge favorise une position (souvent la première) indépendamment du contenu.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "Position bias is when an LLM exhibits a propensity to favor certain positions over others."
> Fetched: 2026-06-10

*Parade* : appeler le juge deux fois en inversant l'ordre, et ne déclarer un gagnant que s'il l'emporte dans les deux sens.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "call a judge twice by swapping the order of two answers and only declare a win when an answer is preferred in both orders."
> Fetched: 2026-06-10

**Biais de verbosité** — le juge préfère les réponses longues, même quand elles sont moins claires ou moins exactes.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "Verbosity bias is when an LLM judge favors longer, verbose responses, even if they are not as clear, high-quality, or accurate as shorter alternatives."
> Fetched: 2026-06-10

*Parade* : inscrire explicitement dans la grille que la longueur n'est pas un critère, et ajouter un palier qui pénalise le délayage.

**Biais d'auto-préférence (self-preference)** — un juge tend à favoriser les réponses produites par lui-même ou par un modèle de sa famille.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "describe the effect that LLM judges may favor the answers generated by themselves."
> Fetched: 2026-06-10

Le framework G-Eval observe le même travers de manière indépendante.

> Source: https://arxiv.org/abs/2303.16634
> Citation: "highlight the potential issue of LLM-based evaluators having a bias towards the LLM-generated texts."
> Fetched: 2026-06-10

*Parade* : utiliser comme juge un modèle d'une **famille différente** de celui qui produit la sortie évaluée.

**Non-déterminisme** — un juge LLM échantillonne ; relancé sur la même sortie, il peut changer d'avis. *Parade* : noter à température basse, demander plusieurs votes et agréger, et fournir quelques exemples (*few-shot*) qui stabilisent le verdict.

> Source: https://arxiv.org/abs/2306.05685
> Citation: "the few-shot judge can significantly increase the consistency"
> Fetched: 2026-06-10

### Calibrer le juge contre des labels humains

Voici la règle d'or que personne ne respecte assez : **un juge non calibré n'est pas une mesure, c'est une opinion automatisée.** Avant de faire confiance à ton juge, tu dois vérifier qu'il est d'accord avec des humains sur un *golden set* — un échantillon de sorties que tu as toi-même notées à la main.

Le bénéfice est démontré : sur MT-bench, l'accord entre GPT-4 et les humains atteint 85 %, soit davantage que l'accord entre humains eux-mêmes (81 %).

> Source: https://arxiv.org/abs/2306.05685
> Citation: "the agreement under setup S2 (w/o tie) between GPT-4 and humans reaches 85%, which is even higher than the agreement among humans (81%)."
> Fetched: 2026-06-10

Mais ce résultat n'est pas un chèque en blanc : il vaut pour *leur* tâche et *leur* grille. Sur la tienne, tu dois le re-démontrer. La consigne d'Anthropic est sans appel : on teste la fiabilité du juge avant de le déployer à grande échelle.

> Source: https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests
> Citation: "Test to ensure reliability first then scale."
> Fetched: 2026-06-10

**Quand faire confiance au juge ?** Quand son accord avec ton golden set dépasse un seuil que tu fixes (par exemple ≥ 80 % de concordance sur le pass/fail dérivé du seuil). En dessous, tu n'automatises pas : tu corriges d'abord la grille ou tu changes de modèle-juge.

### Où ça s'insère dans `evals/` et `.agent.md`

Rien de ce module ne remplace la structure du module 310 — tout s'y ajoute. Le dossier `evals/` accueille les grilles et les golden sets à côté des fixtures binaires :

```text
evals/
  summarize-article/
    fixtures.yml          # assertions binaires (module 310)
    rubric.yml            # grille graduée (ce module)
    golden-set.yml        # sorties notées à la main pour calibrer
    results/
      2026-06-10-graded.json
```

Le juge, lui, est un agent. Tu le déclares dans un `.agent.md` selon la convention du module 104 — un `name`, une `description`, et surtout une liste `tools` volontairement vide pour qu'il ne puisse **que** juger, jamais modifier de fichier.

> Source: docs/01-fondations/104-agents.md
> Citation: "Si tu spécifies une liste vide (`tools: []`), l'agent n'a accès à aucun outil — il ne peut que dialoguer."
> Fetched: 2026-06-10

## Mise en pratique

Tu vas équiper le skill `summarize-article` d'une eval graduée pilotée par un LLM-juge, puis le calibrer.

### Étape 1 — Écrire la grille graduée

Ancre chaque palier dans un comportement observable. Ajoute le fichier à côté de tes fixtures existantes :

```diff
+ # evals/summarize-article/rubric.yml
+
+ dimension: faithfulness
+ scale: [1, 5]
+ levels:
+   1: "Contient une affirmation absente ou contredite par la source."
+   3: "Fidèle mais omet un point clé."
+   5: "Fidèle, complet, nuances préservées."
+ threshold: 4
```

### Étape 2 — Déclarer le juge dans `.agent.md`

Le juge est un agent en lecture seule. La clé `tools: []` garantit qu'il ne touche à rien.

```diff
+ # .agents/faithfulness-judge.agent.md
+ ---
+ name: faithfulness-judge
+ description: "Note la fidélité d'un résumé contre sa source — ne modifie jamais de fichier."
+ tools: []
+ model: claude-sonnet-4
+ ---
+
+ Tu es un juge d'évaluation. On te fournit une SOURCE, un RÉSUMÉ et une GRILLE.
+ Procède en deux temps :
+ 1. Raisonne d'abord à voix haute (chain-of-thought) sur les écarts.
+ 2. Puis remplis le formulaire JSON : { "reason": "...", "score": <1-5> }.
+ La longueur du résumé n'est PAS un critère. Ne te fie qu'à la fidélité.
```

Note deux décisions de design directement issues des concepts : le **form-filling après chain-of-thought** (G-Eval) et la **neutralisation explicite du biais de verbosité** dans la dernière ligne.

### Étape 3 — Choisir un juge d'une autre famille

Le skill évalué produit ses résumés avec un modèle ; pour éviter le biais d'auto-préférence, fais juger par un modèle d'une **famille différente**. Si tes sorties viennent d'un modèle OpenAI, prends un juge Anthropic (ou l'inverse). La clé `model:` du `.agent.md` te le permet sans toucher au reste.

### Étape 4 — Neutraliser le non-déterminisme

Lance le juge **trois fois** sur chaque fixture à température basse et prends la médiane des scores. Si les trois votes divergent de plus d'un palier, c'est le signal que ta grille est ambiguë — retourne à l'étape 1.

```diff
+ # evals/summarize-article/results/2026-06-10-graded.json
+ {
+   "fixture": "article-long",
+   "votes": [4, 4, 5],
+   "score": 4,          // médiane
+   "pass": true          // score >= threshold(4)
+ }
```

### Étape 5 — Construire le golden set

Prends 10 à 20 sorties réelles du skill et **note-les toi-même** à la main avec la même grille. C'est ta vérité terrain. Range-les dans `golden-set.yml`.

```diff
+ # evals/summarize-article/golden-set.yml
+ - id: gold-01
+   source_ref: fixtures.yml#article-long
+   human_score: 4
+ - id: gold-02
+   source_ref: fixtures.yml#article-court
+   human_score: 2
```

### Étape 6 — Calibrer et décider

Fais juger le golden set par ton LLM-juge, puis compare ses verdicts (après seuil) aux tiens. Calcule le taux de concordance.

- **Concordance ≥ 80 %** → tu peux faire confiance au juge et l'intégrer à ta CI.
- **Concordance < 80 %** → n'automatise pas. Resserre la grille, change de modèle-juge, ou ajoute des exemples few-shot, puis recommence.

### Étape 7 — Versionner

Commite la grille, le golden set, la config du juge et les résultats. Le jour où tu changes le skill *ou* le modèle-juge, tu relances la calibration : une dérive de concordance apparaît immédiatement dans le diff, exactement comme une régression binaire au module 310.

## Pièges & anti-patterns

| Piège | Symptôme | Parade |
|---|---|---|
| Juger de la même famille que le générateur | Scores anormalement hauts | Juge d'une **autre** famille de modèles (self-preference) |
| Grille vague (« note la clarté de 1 à 5 ») | Scores instables d'un run à l'autre | Ancrer chaque palier dans un comportement observable |
| Faire confiance au juge sans le calibrer | « 87 % » qui ne corrèle avec rien | Golden set humain + seuil de concordance avant déploiement |
| Comparaison pairwise sur une seule passe | Le gagnant change si on inverse l'ordre | Double appel avec inversion ; gagnant seulement si vainqueur des deux côtés |
| Score nu sans justification | Verdict impossible à déboguer | Exiger `reason` avant `score` (form-filling + CoT) |
| Un seul vote du juge | Le verdict bouge à chaque exécution | 3 votes à température basse + médiane |
| Tout passer en gradué | Coût et latence explosent | Garder le binaire pour le format ; le juge pour le subjectif |
| Pairwise pour comparer 10 versions | Explosion quadratique des paires | Pointwise pour le classement absolu |

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais distinguer une dimension binaire d'une dimension qui exige un score gradué.
- [ ] Ton dossier `evals/` contient une `rubric.yml` dont chaque palier décrit un comportement observable, avec un `threshold`.
- [ ] Ton juge est déclaré dans un `.agent.md` avec `tools: []` et un modèle d'une autre famille que le générateur.
- [ ] Ton judge prompt demande un raisonnement (CoT) avant le score (form-filling).
- [ ] Tu nommes les quatre biais (position, verbosité, self-preference, non-déterminisme) et leur parade.
- [ ] Tu as un `golden-set.yml` noté à la main et tu connais le taux de concordance de ton juge.
- [ ] Tu n'automatises le juge en CI que si la concordance dépasse ton seuil.

## Pour aller plus loin

- [Module 310 — Evals binaires](./310-evals.md) — la base que ce module généralise ; reviens-y si ton format doit rester binaire.
- [Module 104 — Agents](../01-fondations/104-agents.md) — pour affiner la configuration `.agent.md` de ton juge.
- Zheng et al., *Judging LLM-as-a-Judge with MT-Bench and Chatbot Arena* (arXiv:2306.05685) — la référence sur les biais et l'accord juge/humain.
- Liu et al., *G-Eval* (arXiv:2303.16634) — le patron chain-of-thought + form-filling pour des juges mieux corrélés à l'humain.
- [promptfoo — model-graded metrics](https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/) — `llm-rubric`, `factuality`, `g-eval` prêts à l'emploi.
- [Anthropic — Develop tests & evals](https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests) — méthodologie de notation LLM et calibration.

## Sources

- [Zheng et al., *Judging LLM-as-a-Judge with MT-Bench and Chatbot Arena* — arXiv:2306.05685](https://arxiv.org/abs/2306.05685)
- [Liu et al., *G-Eval: NLG Evaluation using GPT-4 with Better Human Alignment* — arXiv:2303.16634](https://arxiv.org/abs/2303.16634)
- [promptfoo — model-graded metrics (overview)](https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/)
- [promptfoo — `llm-rubric`](https://www.promptfoo.dev/docs/configuration/expected-outputs/model-graded/llm-rubric/)
- [Anthropic — Develop tests (success criteria & grading)](https://docs.anthropic.com/en/docs/test-and-evaluate/develop-tests)
- [OpenAI — Graders (score model, text similarity)](https://platform.openai.com/docs/guides/graders)
- [Module 104 — Agents (`.agent.md`)](../01-fondations/104-agents.md) — `docs/01-fondations/104-agents.md`
- [Module 310 — Evals binaires](./310-evals.md) — `docs/03-ingenierie-de-contexte/310-evals.md`

## Module suivant

**Suivant** : [311 — Tokens & fenêtre de contexte](./311-tokens-contexte.md)
