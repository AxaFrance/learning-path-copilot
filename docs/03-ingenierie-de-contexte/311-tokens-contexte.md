---
id: tokens-contexte
title: "311 — Tokens & fenêtre de contexte"
sidebar_position: 311
description: "Comprendre pourquoi les tokens comptent, choisir le bon modèle selon la tâche, et auditer régulièrement ses primitives."
---

# 311 — Tokens & fenêtre de contexte

Durée estimée : 45 min · Complexité : ⭐⭐⭐ · Pré-requis : [Module 103](../01-fondations/103-skills.md), [Module 208](../02-composition/208-workflows.md)

> Chaque token que tu envoies a un coût, une latence et un effet sur la qualité de la réponse. Ce module t'apprend à raisonner en tokens avant d'optimiser.

## Pourquoi ce module

Tu sais créer des `skill`, des `agent`, des workflows orchestrés et des `eval` binaires. Tu maîtrises les primitives. Mais une question reste en suspens : combien ces primitives *coûtent-elles* — en tokens, en latence, en qualité de signal ?

La sobriété LLM n'est pas une contrainte budgétaire — c'est une discipline de conception. Ce module pose le cadre : pourquoi les tokens comptent, comment choisir le bon modèle, et comment auditer ses primitives. Les modules suivants ([312](./312-patterns-sobriete.md) et [313](./313-outils-reduction.md)) approfondissent les leviers concrets.

À la fin de ce module, tu sais :

- expliquer pourquoi les tokens comptent au-delà du simple coût financier ;
- choisir le bon modèle selon la tâche avec une grille de décision explicite ;
- auditer tes skills pour éliminer le bruit dans le routeur sémantique ;
- auditer tes agents pour abaisser le modèle au minimum viable.

## Pré-requis

- [Module 103 — Skills](../01-fondations/103-skills.md) — tu dois savoir créer et déclencher un skill.
- [Module 208 — Workflows](../02-composition/208-workflows.md) — tu dois maîtriser l'orchestration par sous-agents.
- VS Code avec l'extension GitHub Copilot activée.
- Un dépôt Git contenant au moins deux agents et trois skills fonctionnels.

## Concepts clés

### 1 Pourquoi les tokens comptent

Un token n'est pas un caractère. C'est l'unité atomique que le modèle traite — en moyenne 3 à 4 caractères en anglais, souvent moins en français (le français consomme donc [~1.3× plus de tokens à contenu égal](https://blog.yenniejun.com/p/all-languages-are-not-created-tokenized)). Chaque token envoyé a quatre conséquences :

- **Coût direct** — les modèles facturent à l'entrée et à la sortie, par million de tokens. Un skill de 800 tokens chargé dans chaque conversation coûte à chaque échange, pas une seule fois.
- **Latence** — plus le prompt est long, plus le temps de réponse augmente. Sur une complétion inline, 200 ms de latence supplémentaire suffisent à casser le flow de développement.
- **Fenêtre de contexte limitée** — chaque modèle a un plafond. Un contexte saturé d'instructions inutiles évince les fichiers source qui comptent vraiment.
- **Dilution du signal** — le modèle accorde de l'attention à chaque token. Plus tu envoies de bruit, moins il se concentre sur le signal. C'est l'effet le plus pernicieux : même avec une fenêtre de contexte géante, la qualité se dégrade quand le ratio signal/bruit baisse.

La sobriété n'est pas une restriction — c'est un multiplicateur de qualité.

### 2 Choisir le bon modèle

Tous les modèles ne se valent pas — et le plus gros n'est pas toujours le meilleur. La grille de décision suivante t'aide à choisir :

| Tâche | Modèle recommandé | Pourquoi |
|---|---|---|
| Complétion inline | petit / rapide | La latence compte plus que la profondeur de raisonnement |
| Refactor intra-fichier | moyen | Équilibre entre coût et qualité |
| Génération multi-fichiers | grand | Raisonnement long nécessaire |
| Design architecture | premium | Qualité de réflexion prime sur le coût ponctuel |
| Lint déterministe | petit | Tâche fermée, `eval` binaires suffisent |

La règle d'or : fais passer les `eval` avec le plus petit modèle avant d'escalader. Si un modèle moyen réussit 100 % des evals, il n'y a aucune raison de payer pour un modèle grand.

Un point souvent ignoré : les coûts se mesurent en prix d'entrée + sortie par million de tokens, pas en abonnement plat. Un agent qui consomme 50 000 tokens d'entrée par invocation coûte significativement plus cher sur un modèle premium que sur un modèle moyen. Multiplie par le nombre d'invocations quotidiennes et l'écart devient concret.

Le champ `model:` dans un `.agent.md` te permet de fixer le modèle par agent :

```yaml
# .agents/commit-writer.agent.md
---
model: gpt-4o-mini
description: "Rédige des messages de commit conventionnels"
---
```

### 3 Audit pratique

L'audit de sobriété est un exercice régulier, pas un one-shot. Il porte sur deux axes :

**Axe 1 — Auditer les skills.** Liste tous les skills de ton `workspace`. Pour chacun, pose trois questions :

1. Le delta `with_skill` / `without_skill` est-il supérieur à 30 % ?
2. Le skill se déclenche-t-il au moins une fois par semaine ?
3. Le skill ajoute-t-il une information que le modèle ne connaît pas ?

Classe chaque skill dans une des trois catégories : **garde**, **fusionne** (avec un skill proche), **supprime**.

**Axe 2 — Auditer les modèles.** Pour chaque `agent`, vérifie que le champ `model:` dans le `.agent.md` est le plus petit modèle qui passe les `eval`. Si un agent utilise un modèle premium mais que ses evals passent avec un modèle moyen, abaisse le modèle.

## Mise en pratique

### Étape 1 — Inventorier les skills

Liste tous les skills présents dans ton workspace :

```bash
find .agents/skills -name "SKILL.md" | sort
```

Pour chaque skill trouvé, note sa description (première ligne) et estime sa fréquence de déclenchement.

### Étape 2 — Mesurer le delta d'un skill suspect

Choisis un skill dont tu doutes de l'utilité. Exécute tes fixtures en mode `with_skill` puis `without_skill` (voir [Module 315](./315-evals.md)) :

```yaml
# Résultat attendu
with_skill:    8/10 pass
without_skill: 7/10 pass
delta:         +1   # → le skill n'apporte presque rien
```

Un delta de +1 sur 10 fixtures est un signal clair : ce skill est un candidat à la suppression.

### Étape 3 — Abaisser le modèle d'un agent

Identifie un agent qui utilise un modèle grand ou premium. Modifie temporairement son `model:` pour utiliser un modèle d'un cran en dessous :

```diff
  # .agents/spec-writer.agent.md
  ---
- model: claude-sonnet-4-20250514
+ model: gpt-4o-mini
  description: "Rédige des spécifications techniques"
  ---
```

Relance les evals de cet agent. Si le score ne chute pas, le modèle inférieur suffit. Si le score chute, remets le modèle supérieur et documente la raison.

## Pièges & anti-patterns

| Piège | Pourquoi c'est un problème | Solution |
|---|---|---|
| Choisir le modèle le plus gros par défaut | Surcoût sans gain, latence inutile | Commencer par le plus petit, escalader si les evals échouent |
| Ignorer les tokens d'instructions | Un skill de 800 tokens est chargé à chaque échange | Mesurer la taille de chaque skill, supprimer les inutiles |
| Auditer une seule fois | L'écosystème évolue, les modèles changent | Audit mensuel minimum |
| Confondre fenêtre grande et qualité | Un contexte de 200k tokens dilué produit de mauvaises réponses | Le ratio signal/bruit prime sur la taille de fenêtre |

## Exercice ⭐⭐⭐

### Énoncé

**Partie 1 — Audit de skills**

Choisis 5 skills dans ton `workspace`. Pour chacun :

1. Exécute les evals en mode `with_skill` et `without_skill`.
2. Calcule le delta.
3. Classe le skill : garde / fusionne / supprime.
4. Documente ta décision dans un tableau.

| Skill | with_skill | without_skill | Delta | Décision | Motif |
|---|---|---|---|---|---|
| `writing-commit-message` | 9/10 | 3/10 | +6 | garde | Fort delta, usage quotidien |
| `generic-typescript` | 7/10 | 7/10 | 0 | supprime | Aucune valeur ajoutée |
| ... | ... | ... | ... | ... | ... |

**Partie 2 — Optimisation de modèle**

Pour 2 agents de ton workspace :

1. Note le modèle actuel et exécute les evals.
2. Abaisse le modèle d'un cran (par exemple de `claude-sonnet-4-20250514` à `gpt-4o-mini`).
3. Relance les evals.
4. Compare les scores et estime le delta de coût.

```yaml
# Exemple de rapport
agent: spec-writer
model_actuel: claude-sonnet-4-20250514
model_teste: gpt-4o-mini
evals_actuel: 10/10
evals_teste: 9/10
decision: abaisser (1 fixture non critique échoue)
```

## Validation

Tu as réussi ce module si :

- Le ratio skills/effets est divisé par 2 : tu as documenté la suppression ou la fusion d'au moins la moitié des skills audités, avec les deltas `with_skill` / `without_skill` comme preuve.
- Au moins un agent a vu son modèle abaissé sans dégradation de ses evals : le rapport avant/après est versionné.

## Pour aller plus loin

- **Fenêtres de contexte** — explore les limites exactes des modèles que tu utilises. La documentation de chaque fournisseur publie la taille de fenêtre en tokens. Croise cette donnée avec la taille de tes skills + instructions + fichiers chargés pour estimer le taux d'occupation de ta fenêtre.
- **Coût par workflow** — instrumente un workflow complet ([Module 208](../02-composition/208-workflows.md)) pour mesurer le nombre total de tokens consommés par exécution.
- [Module 312 — Patterns de sobriété](./312-patterns-sobriete.md) — les principes de design (DDD, Object Calisthenics, SOLID) comme leviers de réduction.
- [Module 313 — Outils de réduction](./313-outils-reduction.md) — sous-agents, minimal-context-tools, snip.
- **Monitoring continu** — mets en place un suivi régulier (hebdomadaire) de ton ratio skills/effets et de tes choix de modèles.

## Sources

- GitHub Blog — articles sur l'efficience des tokens dans Copilot (source d'inspiration).
- Documentation des fournisseurs de modèles — tailles de fenêtre de contexte et tarification par million de tokens.
- OpenAI — [Tokenizer](https://platform.openai.com/tokenizer) — outil interactif pour visualiser le découpage en tokens par langue.
- OpenAI Cookbook — [How to count tokens with tiktoken](https://cookbook.openai.com/examples/how_to_count_tokens_with_tiktoken) — documente la règle « 1 token ≈ 4 caractères en anglais ».
- Yennie Jun (2023) — [All languages are NOT created (tokenized) equal](https://blog.yenniejun.com/p/all-languages-are-not-created-tokenized) — analyse comparative montrant que le français nécessite ~1.3× plus de tokens que l'anglais à contenu sémantique égal.
