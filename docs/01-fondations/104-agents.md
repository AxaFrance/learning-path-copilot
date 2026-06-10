---
id: agents-personnalises
title: "104 — Agents (.agent.md)"
sidebar_position: 104
description: "Packager un rôle réutilisable dans un fichier .agent.md avec identité, outils et modèle dédiés."
---

# 104 — Agents (`.agent.md`)

Durée estimée : 60 min

> Un `skill` dit à Copilot *quoi faire* dans un contexte précis. Un `agent` va plus loin : il lui donne une *identité*, un jeu d'outils restreint, et la capacité de mener une conversation longue dans un rôle défini.

## Pourquoi ce module

Dans le module précédent, tu as appris à encapsuler un savoir-faire procédural dans un `SKILL.md`. Un skill fonctionne très bien pour des tâches courtes et ciblées : rédiger un commit, formater un changelog, structurer un ADR. Mais certaines situations demandent davantage.

Imagine un relecteur de code qui doit parcourir plusieurs fichiers, poser des questions, revenir sur un point précédent, et surtout *ne jamais écrire de code lui-même*. Un skill ne suffit plus : il manque la notion d'identité persistante, la possibilité de restreindre les outils disponibles, et la capacité de maintenir un dialogue sur plusieurs échanges.

C'est exactement ce que résout un fichier `.agent.md`. Il définit un **mode de conversation** complet : un nom, une description, un persona, une liste d'outils autorisés, et éventuellement un modèle spécifique.

À la fin de ce module, tu sais :

- expliquer la différence entre un `skill` et un `agent` ;
- créer un fichier `.agent.md` avec son frontmatter complet ;
- restreindre les outils disponibles pour un agent ;
- déléguer une tâche à un agent via `runSubagent` ;
- choisir entre skill et agent selon le besoin.

## Pré-requis

- [Module 103 — Skills](./103-skills.md)
- VS Code avec l'extension GitHub Copilot activée.
- Un dépôt Git avec au moins un fichier source.

## Concepts clés

### Qu'est-ce qu'un agent ?

Un `agent` est un mode de conversation personnalisé dans Copilot. Contrairement à un skill (qui s'active conditionnellement le temps d'une tâche), un agent définit une **identité complète** que tu sélectionnes explicitement avant de démarrer une conversation. Une fois activé, l'agent conserve son rôle, ses contraintes et son jeu d'outils tout au long de l'échange.

Concrètement, un agent apparaît dans le sélecteur de mode en haut du panneau `chat` de VS Code, aux côtés des modes natifs (Ask, Edit, Agent). Tu le choisis comme tu choisirais un interlocuteur spécialisé.

### Anatomie d'un `.agent.md`

Un agent vit à la racine du dossier `.agents/` de ton dépôt :

```text
.agents/
  pair-reviewer.agent.md
  tech-writer.agent.md
  skills/
    writing-commit-message/
      SKILL.md
```

Le fichier `.agent.md` se compose de deux parties : un frontmatter YAML et un corps en Markdown qui contient les instructions du persona.

```markdown
---
name: pair-reviewer
description: "Relecture de code uniquement — ne modifie jamais de fichier."
tools:
  - codebase
  - githubRepo
model: gpt-4.1
---

Tu es un relecteur de code senior. Ton rôle est d'analyser le code
qu'on te soumet et de fournir des commentaires constructifs.

## Règles absolues

- Tu ne modifies **jamais** de fichier. Pas de création, pas d'édition.
- Tu n'exécutes **jamais** de commande terminal.
- Si on te demande d'écrire du code, tu refuses poliment et tu expliques
  que ton rôle se limite à la relecture.
```

### Le frontmatter en détail

Chaque clé du frontmatter contrôle un aspect du comportement de l'agent :

| Clé | Rôle | Obligatoire |
|---|---|---|
| `name` | Identifiant unique de l'agent, affiché dans le sélecteur de mode | oui |
| `description` | Phrase courte décrivant le rôle, visible dans l'interface | oui |
| `tools` | Liste des outils autorisés — tout outil absent est interdit | non |
| `model` | Modèle à utiliser pour cet agent (ex. `gpt-4.1`, `claude-sonnet-4`) | non |

La clé `tools` est le levier le plus puissant. Par défaut, Copilot en mode Agent a accès à tous les outils : lecture/écriture de fichiers, terminal, recherche dans le codebase, etc. En spécifiant une liste explicite, tu **restreins** l'agent aux seuls outils nommés. C'est exactement ce qui permet de créer un relecteur qui ne peut pas modifier de fichiers : il suffit de ne pas inclure `editFiles` ni `runInTerminal` dans la liste.

Quelques identifiants d'outils courants :

| Identifiant | Capacité |
|---|---|
| `codebase` | Recherche sémantique dans le code du workspace |
| `editFiles` | Création et modification de fichiers |
| `runInTerminal` | Exécution de commandes dans le terminal |
| `githubRepo` | Accès aux issues, PRs et métadonnées du dépôt |
| `fetch` | Requêtes HTTP vers des URLs externes |

Si tu omets la clé `tools`, l'agent a accès à **tous** les outils disponibles. Si tu spécifies une liste vide (`tools: []`), l'agent n'a accès à aucun outil — il ne peut que dialoguer.

La clé `model` est optionnelle. Si tu ne la précises pas, l'agent utilise le modèle sélectionné dans les paramètres globaux de Copilot. Spécifier un modèle est utile quand le rôle de l'agent exige un modèle particulier — par exemple un modèle plus puissant pour de l'analyse architecturale, ou un modèle plus rapide pour du triage.

### Agent vs chatmode (historique)

Avant l'introduction du format `.agent.md`, VS Code proposait un mécanisme similaire via des fichiers `.chatmode.md`. Ce format est désormais obsolète. Si tu rencontres des fichiers `.chatmode.md` dans un dépôt existant, la migration est directe : renomme le fichier en `.agent.md` et vérifie que le frontmatter utilise les clés documentées ci-dessus. Le corps du fichier reste identique.

Ne crée jamais de nouveau fichier `.chatmode.md` — utilise exclusivement `.agent.md`.

### Délégation avec `runSubagent`

Un agent peut déléguer une sous-tâche à un autre agent via le mécanisme `runSubagent`. C'est utile quand un workflow nécessite plusieurs rôles distincts : par exemple, un agent orchestrateur qui demande une review à l'agent `pair-reviewer` avant de merger.

Dans le corps d'un agent, tu décris la délégation en langage naturel :

```markdown
## Workflow

1. Implémente la fonctionnalité demandée.
2. Avant de conclure, délègue une review du code modifié
   à l'agent `pair-reviewer` via `runSubagent`.
3. Si le reviewer identifie des problèmes, corrige-les
   et relance la review.
```

Copilot interprète cette instruction et utilise l'outil `runSubagent` pour créer une sous-conversation avec l'agent cible. L'agent délégué hérite de son propre contexte (ses outils, son modèle, ses contraintes) — il n'a pas accès au contexte de l'agent appelant au-delà de ce qui lui est explicitement transmis.

Quelques points importants sur `runSubagent` :

- L'agent cible doit exister dans le dossier `.agents/` du workspace.
- Chaque sous-agent s'exécute avec ses propres contraintes d'outils — un agent sans `editFiles` ne pourra pas modifier de fichiers, même si l'agent appelant le peut.
- La sous-conversation est isolée : le sous-agent ne voit pas l'historique de la conversation principale.

### Quand choisir un agent plutôt qu'un skill

Le choix entre skill et agent repose sur quatre critères :

| Critère | Skill (`SKILL.md`) | Agent (`.agent.md`) |
|---|---|---|
| Durée de l'interaction | Tâche courte, ponctuelle | Conversation longue, multi-échanges |
| Identité | Aucune — Copilot reste « lui-même » | Persona défini (relecteur, rédacteur, architecte) |
| Outils | Tous les outils disponibles | Jeu d'outils restreint possible |
| Activation | Automatique par le routeur sémantique | Manuelle par le sélecteur de mode ou `runSubagent` |

**Règle simple** : si la tâche tient en une seule réponse et ne nécessite pas de restreindre les outils, un skill suffit. Si tu as besoin d'une identité persistante, d'une conversation multi-tours, ou de contrôler finement les outils disponibles, crée un agent.

Un exemple concret : « formater un commit message » est un skill. « Relire du code sur plusieurs fichiers en refusant d'écrire quoi que ce soit » est un agent.

## Démonstration

### Étape 1 — Créer un agent minimal

Crée le fichier `.agents/pair-reviewer.agent.md` à la racine de ton dépôt :

```diff
+ # .agents/pair-reviewer.agent.md
+
+ ---
+ name: pair-reviewer
+ description: "Relecture de code — ne modifie jamais de fichier."
+ tools:
+   - codebase
+   - githubRepo
+ ---
+
+ Tu es un relecteur de code senior. Ton rôle est d'analyser le code
+ qu'on te soumet et de fournir des commentaires constructifs.
+
+ ## Contraintes
+
+ - Tu ne modifies jamais de fichier.
+ - Tu n'exécutes jamais de commande terminal.
+ - Si on te demande d'écrire du code, tu refuses poliment en expliquant
+   que ton rôle se limite à la relecture.
+ - Tu structures tes retours par fichier, puis par ordre de sévérité
+   (bloquant, important, suggestion).
```

### Étape 2 — Activer l'agent

Ouvre le panneau Copilot Chat dans VS Code. En haut du panneau, clique sur le sélecteur de mode (qui affiche par défaut « Agent » ou « Ask »). Ton agent `pair-reviewer` apparaît dans la liste. Sélectionne-le.

La conversation qui suit utilise désormais l'identité et les contraintes de ton agent.

### Étape 3 — Tester les contraintes

Envoie deux types de requêtes dans la conversation :

**Requête 1** — « Relis le fichier `src/utils/parser.ts` et donne-moi tes commentaires. »

L'agent doit parcourir le fichier via l'outil `codebase` et fournir une analyse structurée sans modifier quoi que ce soit.

**Requête 2** — « Corrige le bug à la ligne 42 de `parser.ts`. »

L'agent doit **refuser poliment** : il n'a pas l'outil `editFiles` dans sa liste, et ses instructions lui interdisent de modifier des fichiers. Il peut en revanche *suggérer* le correctif dans sa réponse.

### Étape 4 — Délégation depuis un autre agent

Crée un second agent `.agents/feature-dev.agent.md` qui délègue la review :

```markdown
---
name: feature-dev
description: "Développeur qui implémente et demande une review avant de conclure."
tools:
  - codebase
  - editFiles
  - runInTerminal
  - runSubagent
---

Tu es un développeur. Quand on te demande d'implémenter une fonctionnalité :

1. Implémente le code demandé.
2. Avant de conclure, délègue une review à l'agent `pair-reviewer`
   via `runSubagent`.
3. Si le reviewer identifie des problèmes bloquants, corrige-les
   et relance une review.
4. Termine en résumant les modifications et le résultat de la review.
```

Active l'agent `feature-dev` et demande-lui d'implémenter une petite fonctionnalité. Observe qu'il crée une sous-conversation avec `pair-reviewer` pour valider son travail avant de conclure.

### Étape 5 — Créer un agent avec assistance

Tu sais maintenant construire un agent à la main. En pratique, tu peux aussi demander à Copilot de le scaffolder pour toi grâce à des outils dédiés.

**`agent-customization` (skill natif VS Code)** — VS Code embarque un skill capable de créer, modifier et déboguer les fichiers `.agent.md`. Pour l'utiliser, demande simplement :

> « Crée un agent `tech-writer` qui rédige de la documentation technique sans toucher au code. »

Copilot génère le fichier `.agents/tech-writer.agent.md` avec un frontmatter complet (`name`, `description`, `tools`) et un corps de persona adapté à ta demande. Tu peux ensuite affiner le résultat manuellement.

Ce skill est particulièrement utile pour :

- scaffolder rapidement un agent sans se souvenir de la syntaxe exacte du frontmatter ;
- déboguer un agent qui ne se comporte pas comme attendu (le skill analyse le fichier et propose des corrections) ;
- configurer les patterns `applyTo` ou les restrictions d'outils.

**`create-agent` (convention Anthropic)** — Anthropic propose une convention structurée pour la création d'agents :

- le fichier est placé dans `.agents/` avec l'extension `.agent.md` ;
- le frontmatter YAML contient `name`, `description`, `tools` et optionnellement `model` ;
- la description suit le même principe que pour les skills : elle décrit *quand* utiliser l'agent, pas ce qu'il fait en détail ;
- le corps utilise des sections structurées (`## Règles`, `## Workflow`, `## Contraintes`) pour cadrer le comportement.

Les deux approches sont complémentaires :

| Approche | Quand |
|---|---|
| Manuel (étapes 1–4) | Tu apprends, tu veux comprendre chaque clé du frontmatter |
| `agent-customization` (VS Code) | Tu veux scaffolder vite ou déboguer un agent existant |
| Convention `create-agent` (Anthropic) | Tu publies un agent dans un écosystème multi-agent standardisé |

## Exercice ⭐⭐

**Énoncé** — Crée de zéro un agent `pair-reviewer` qui ne fait que de la relecture de code et refuse toute demande d'écriture.

**Étapes guidées** :

1. Crée le fichier `.agents/pair-reviewer.agent.md`.
2. Rédige le frontmatter avec :
   - `name` et `description` explicites.
   - Une liste `tools` qui exclut `editFiles` et `runInTerminal`.
3. Rédige le corps avec les contraintes du persona :
   - Rôle de relecteur senior.
   - Interdiction de modifier des fichiers ou d'exécuter des commandes.
   - Refus poli si on demande d'écrire du code.
   - Structure des retours : par fichier, par sévérité.
4. Sélectionne l'agent dans le panneau Chat et teste avec :
   - Une demande de review sur un fichier existant.
   - Une demande d'écriture de code (doit être refusée).
5. Crée un agent `feature-dev` qui utilise `runSubagent` pour déléguer une review à `pair-reviewer`.
6. Vérifie que la délégation fonctionne : `feature-dev` implémente, puis `pair-reviewer` relit.

**Critère de réussite** : l'agent `pair-reviewer` fournit des commentaires structurés sur le code soumis et refuse poliment toute demande de modification. L'agent `feature-dev` parvient à déléguer une review via `runSubagent`.

## Validation

Tu peux passer au module suivant si :

- [ ] Ton dépôt contient un fichier `.agents/pair-reviewer.agent.md` avec un frontmatter valide.
- [ ] L'agent apparaît dans le sélecteur de mode du panneau Chat.
- [ ] L'agent refuse poliment les demandes d'écriture de code.
- [ ] La liste `tools` exclut `editFiles` et `runInTerminal`.
- [ ] Tu sais expliquer la différence entre un skill et un agent en une phrase.
- [ ] `runSubagent` peut invoquer l'agent depuis un autre agent.

## Pour aller plus loin

- [Module 208 — Workflows](../02-composition/208-workflows.md) : combiner plusieurs agents dans un workflow complexe avec des points de décision.
- [Module 310 — Tester ses primitives](../03-ingenierie-de-contexte/310-evals.md) : vérifier automatiquement qu'un agent refuse ou accepte les bonnes requêtes avec des `eval` binaires.
- `docs/reference/agent-anatomy.md` — page de référence à créer.
- La documentation officielle VS Code sur les agents personnalisés : recherche « custom chat modes » dans la doc VS Code.

## Module suivant

**Suivant** : [105 — Hooks](./105-hooks.md)
