---
id: prompts-personnalises
title: "102 — Prompts personnalisés"
sidebar_position: 102
description: "Créer des fichiers .prompt.md réutilisables pour lancer des tâches Copilot en un clic."
---

# 102 — Prompts personnalisés

Durée estimée : 30 min

> Les instructions disent à Copilot *comment* se comporter en permanence. Les `prompt` lui disent *quoi faire* à la demande.

## Pourquoi ce module

Dans le module précédent, tu as appris à configurer le comportement de Copilot avec des fichiers d'instructions. Ces fichiers s'appliquent à *toutes* les conversations — impossible de les déclencher ponctuellement pour une tâche précise.

Les fichiers `.prompt.md` comblent ce manque. Un prompt est une commande réutilisable que tu lances depuis la palette de commandes de VS Code. Tu l'écris une fois, tu le commites dans ton dépôt, et toute ton équipe en profite — exactement comme une instruction, mais activé à la demande.

:::caution Évolution à venir
Le format `.prompt.md` est amené à être **déprécié**. La direction prise par l'écosystème Copilot est de remplacer les prompts par des **skills à invocation utilisateur** : un `SKILL.md` dont on désactive l'invocation par le modèle (`model invocation: disabled`) pour qu'il ne se déclenche que lorsque l'utilisateur le demande explicitement. Tu trouveras les détails dans un module dédié après le module Skills.
:::

Ce module reste utile : les `.prompt.md` fonctionnent aujourd'hui, et comprendre leur logique (mode, variables, déclenchement à la demande) te donnera les bases pour migrer vers les skills à invocation utilisateur quand le moment viendra.

À la fin de ce module, tu sais :

- Créer un fichier `.prompt.md` avec son frontmatter YAML.
- Choisir le bon `mode` (`agent`, `ask` ou `edit`) selon la tâche.
- Utiliser les variables dynamiques (`${selection}`, `${file}`, `${input:name}`).
- Distinguer un prompt réutilisable d'une instruction persistante.

## Pré-requis

- [Module 101 — Instructions personnalisées](./101-instructions.md)
- VS Code avec l'extension GitHub Copilot Chat activée.
- Un projet contenant au moins un fichier TypeScript (pour l'exercice).

## Concepts clés

### Qu'est-ce qu'un fichier `.prompt.md` ?

Un fichier `.prompt.md` est une commande réutilisable que tu déclenches à la demande dans Copilot. Contrairement à une instruction (fichier `.instructions.md`, chargée automatiquement à chaque conversation), un prompt ne s'active que lorsque tu le sélectionnes explicitement dans la palette de commandes.

Tu places tes prompts dans le dossier `.github/prompts/` à la racine de ton dépôt. Chaque fichier correspond à une commande distincte visible dans la palette de VS Code. Le nom du fichier (sans l'extension) devient le nom de la commande : `explain-file.prompt.md` apparaît sous le libellé « explain-file ».

### Le frontmatter

Chaque `.prompt.md` commence par un bloc YAML délimité par `---` qui contrôle le comportement de Copilot :

| Clé | Rôle | Valeurs possibles |
|---|---|---|
| `mode` | Surface d'exécution | `agent`, `ask`, `edit` |
| `description` | Texte affiché dans la palette | Phrase courte |
| `tools` | Outils autorisés (optionnel) | Liste d'identifiants |

Les trois modes en détail :

- **`ask`** — Copilot répond dans le `chat` sans modifier aucun fichier. Idéal pour expliquer, résumer ou analyser du code.
- **`edit`** — Copilot modifie le fichier ouvert en place. Utile pour refactorer, corriger ou formater une portion de code ciblée.
- **`agent`** — Copilot peut lire, écrire et exécuter des commandes dans le `workspace`. C'est le mode le plus puissant : il permet de générer de nouveaux fichiers, lancer des tests ou orchestrer plusieurs étapes.

Le choix du mode est la décision la plus importante lors de la création d'un prompt. Un mode trop permissif (par exemple `agent` pour une simple explication) ajoute de la latence et des appels d'outils inutiles. Un mode trop restreint (`ask` quand tu veux modifier un fichier) empêche Copilot d'agir.

### Les variables

Les variables permettent d'injecter du contexte dynamique dans le corps du prompt au moment de l'exécution :

| Variable | Résolution |
|---|---|
| `${selection}` | Le texte sélectionné dans l'éditeur |
| `${file}` | Le contenu du fichier actif |
| `${input:name}` | Demande une saisie à l'utilisateur au lancement |

La variable `${input:name}` est particulièrement utile pour créer des prompts paramétrables. Le `name` que tu choisis devient le libellé affiché dans la boîte de dialogue : `${input:langue}` affichera « langue » à l'utilisateur avant l'exécution.

Tu peux aussi référencer des fichiers avec la syntaxe `#file:path/to/file` dans le corps du prompt pour injecter le contenu d'un fichier spécifique en contexte.

### Prompt vs instruction : quand utiliser quoi ?

| Critère | Instruction (`.instructions.md`) | Prompt (`.prompt.md`) |
|---|---|---|
| Chargement | Automatique | Manuel (palette) |
| Persistance | Toutes les conversations | Une seule exécution |
| Modification de fichiers | Non | Oui (mode `edit` / `agent`) |
| Cas d'usage | Convention d'équipe, style de code | Tâche ponctuelle, commande réutilisable |

**Règle simple** : si tu veux que Copilot applique une règle *à chaque conversation*, c'est une instruction. Si tu veux lancer une action *à la demande*, c'est un prompt.

Un exemple concret : « réponds toujours en français » est une instruction (tu veux que ça s'applique partout). « Génère un test Vitest pour la sélection » est un prompt (tu le lances quand tu en as besoin).

## Démonstration

### Étape 1 — Créer un prompt basique

Crée le fichier `.github/prompts/explain-file.prompt.md` avec le contenu suivant :

```yaml
---
mode: ask
description: "Explique le fichier courant en français"
---
```

Puis ajoute le corps du prompt sous le frontmatter :

```markdown
Explique le contenu du fichier #file en français.
Résume son rôle en 3 phrases maximum.
```

Ouvre la palette de commandes (Cmd+Shift+P sur macOS, Ctrl+Shift+P sur Windows/Linux) et cherche « explain-file ». Le prompt apparaît avec sa description. Lance-le sur un fichier ouvert pour vérifier qu'il fonctionne.

### Étape 2 — Ajouter une variable d'entrée

On veut rendre la langue paramétrable. Modifie le fichier :

```diff
  # .github/prompts/explain-file.prompt.md
  
  ---
  mode: ask
- description: "Explique le fichier courant en français"
+ description: "Explique le fichier courant dans la langue choisie"
  ---
  
- Explique le contenu du fichier #file en français.
+ Explique le contenu du fichier #file en ${input:langue}.
  Résume son rôle en 3 phrases maximum.
```

Désormais, Copilot te demande la langue souhaitée avant d'exécuter le prompt. Teste avec « français » puis « anglais » pour voir la différence.

### Étape 3 — Passer en mode agent

Crée un second prompt qui génère un résumé dans un fichier séparé :

```yaml
---
mode: agent
description: "Génère un résumé du fichier dans SUMMARY.md"
tools:
  - createFile
---
```

```markdown
Lis le fichier #file et génère un résumé structuré.
Écris le résumé dans un fichier SUMMARY.md à côté du fichier source.
```

En mode `agent`, Copilot peut utiliser l'outil `createFile` pour écrire le résumé sur disque. En mode `ask`, il se serait contenté d'afficher le texte dans le chat sans créer de fichier. Remarque la clé `tools` dans le frontmatter : elle restreint les outils disponibles à `createFile` uniquement, ce qui évite que Copilot exécute des commandes non souhaitées.

## Exercice ⭐

**Énoncé** — Crée un prompt qui génère un test Vitest pour le code sélectionné.

**Étapes guidées** :

1. Crée le fichier `.github/prompts/generate-vitest.prompt.md` :

```yaml
---
mode: agent
description: "Génère un test Vitest pour la sélection"
---
```

2. Ajoute le corps du prompt sous le frontmatter :

```markdown
Génère un fichier de test Vitest pour le code suivant :

${selection}

Utilise `describe` et `it`. Place le fichier de test à côté du fichier source avec le suffixe `.test.ts`.
```

3. Ouvre un fichier TypeScript, sélectionne une fonction.
4. Ouvre la palette de commandes et lance le prompt `generate-vitest`.
5. Répète l'opération sur un second fichier pour vérifier que le prompt est générique.

**Critère de réussite** : le prompt apparaît dans la palette et produit un test valide sur deux fichiers différents.

## Validation

- [ ] Ton dépôt contient le fichier `.github/prompts/generate-vitest.prompt.md`.
- [ ] Tu sais choisir entre `ask`, `edit` et `agent` selon la tâche.
- [ ] Tu as utilisé au moins une variable (`${selection}`, `${file}` ou `${input:name}`).
- [ ] Tu sais expliquer en une phrase la différence entre un prompt et une instruction.

## Pièges & anti-patterns

- **Utiliser `agent` pour tout** — Le mode `agent` est puissant mais plus lent. Si ton prompt ne fait que répondre à une question, utilise `ask`. Réserve `agent` aux cas où Copilot doit créer ou modifier des fichiers.
- **Oublier la `description`** — Sans description dans le frontmatter, le prompt apparaît dans la palette avec un libellé vide. L'utilisateur ne sait pas à quoi il sert et ne le lancera jamais.
- **Écrire un prompt qui devrait être une instruction** — Si la même consigne doit s'appliquer à *chaque* conversation (par exemple « réponds toujours en français »), place-la dans un fichier `.instructions.md`, pas dans un prompt que tu devrais relancer manuellement à chaque fois.
- **Ne pas tester sur plusieurs fichiers** — Un prompt qui ne fonctionne que sur un type de fichier spécifique est trop fragile. Teste toujours sur au moins deux fichiers de structures différentes pour valider que le prompt est générique.
- **Hardcoder des chemins absolus** — Utilise les variables `${file}` ou `#file` plutôt que des chemins en dur. Ton prompt doit rester portable d'un projet à l'autre.

## Pour aller plus loin

- [Module 103 — Skills](./103-skills.md) : quand un prompt ne suffit plus et qu'il faut une procédure conditionnelle avec un fichier `SKILL.md`.
- **Skills à invocation utilisateur** : après avoir compris les skills (module 03), tu découvriras comment remplacer tes `.prompt.md` par des skills dont on désactive `model invocation` — le prompt n'existe plus comme primitive séparée, il devient un skill que seul l'utilisateur déclenche. Ce module viendra après le module Skills dans le parcours.
- `docs/reference/prompt-anatomy.md` — page de référence à créer.

**Suivant** : [103 — Skills](./103-skills.md)

## Sources

- [GitHub Docs — Creating prompt files](https://docs.github.com/en/copilot/customizing-copilot/adding-custom-instructions/creating-prompt-files)
- [VS Code Docs — Reusable prompts](https://code.visualstudio.com/docs/copilot/copilot-customization#_reusable-prompts)
