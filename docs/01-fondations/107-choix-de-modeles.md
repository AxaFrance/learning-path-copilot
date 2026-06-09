---
id: choix-de-modeles
title: "107 — Choix de modèles"
sidebar_position: 107
description: "Comment choisir entre GPT-4.1, Claude, Gemini dans GitHub Copilot — critères de sélection, configuration dans VS Code, et impact sur les coûts."
---

# 107 — Choix de modèles

**Durée** : ~40 min · **Complexité** : ⭐⭐ · **Pré-requis** : [100 — Setup & posture](./100-setup-posture.md), [104 — Agents](./104-agents.md)

> *Copilot ne se limite plus à un seul modèle. Savoir lequel choisir pour chaque tâche, c'est le premier levier d'optimisation que tu peux actionner sans écrire une seule ligne de code.*

## Objectif

À la fin de ce module, tu sais :

- Identifier les familles de modèles disponibles dans Copilot (OpenAI, Anthropic, Google) et leurs forces respectives.
- Choisir un modèle adapté à ta tâche grâce à la grille officielle par catégorie.
- Changer de modèle dans le *model picker* de VS Code et configurer le mode *Auto*.
- Forcer un modèle spécifique dans un agent personnalisé via la clé `model` du frontmatter `.agent.md`.
- Estimer l'impact d'un choix de modèle sur ta consommation de crédits.

## Ce que tu vas apprendre

1. Le paysage multi-modèles de Copilot en 2026
2. Les quatre catégories de tâches et les modèles recommandés pour chacune
3. Le *model picker* : changer, épingler, masquer un modèle
4. Le mode *Auto* et la sélection automatique
5. Le *thinking effort* des modèles de raisonnement
6. Forcer un modèle dans un `.agent.md`
7. Coûts et crédits : comprendre la tarification par token

## Contenu pédagogique

### Le paysage multi-modèles

GitHub Copilot donne accès à plusieurs modèles de langage intégrés, chacun optimisé pour un type de tâche différent. Tu peux changer de modèle pour le *chat*, les suggestions *inline*, et les tâches utilitaires.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "Visual Studio Code gives you access to multiple built-in language models, each optimized for different tasks."
> Fetched: 2026-05-28

Trois fournisseurs principaux alimentent Copilot :

<!-- round 2 response — P2: gloss "contexte long" on first use -->

| Fournisseur | Modèles principaux | Profil |
|---|---|---|
| OpenAI | GPT-4.1, GPT-5 mini, GPT-5.2, GPT-5.5 | Du *lightweight* au raisonnement profond |
| Anthropic | Claude Haiku 4.5, Claude Sonnet 4.5/4.6, Claude Opus 4.6/4.7 | Raisonnement sophistiqué et tâches agentiques |
| Google | Gemini 2.5 Pro, Gemini 3 Flash, Gemini 3.1 Pro, Gemini 3.5 Flash | Contexte long (taille de la fenêtre de texte que le modèle peut traiter) et debugging |

> Source: https://docs.github.com/en/copilot/reference/ai-models/model-comparison
> Citation: "GPT-4.1 — General-purpose coding and writing — Fast, accurate code completions and explanations"
> Fetched: 2026-05-28

<!-- round 2 response — P1: define "fine-tuned" on first use -->

Deux modèles *fine-tuned* (affinés sur des données spécifiques) par GitHub complètent le catalogue : **Raptor mini** (suggestions *inline* rapides) et **Goldeneye** (raisonnement complexe).

> Source: https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
> Citation: "Raptor mini — Public preview — Versatile"
> Fetched: 2026-05-28

### Les quatre catégories de tâches

La documentation officielle classe les modèles en quatre catégories de tâches. C'est ta grille de décision principale.

**1. General-purpose coding and writing** — le choix par défaut quand tu n'as pas de besoin spécifique. Ces modèles équilibrent qualité, vitesse et coût.

Modèles recommandés : GPT-5.3-Codex, GPT-5 mini, Raptor mini.

Cas d'usage : écrire ou relire des fonctions, générer de la documentation, expliquer des erreurs rapidement.

> Source: https://docs.github.com/en/copilot/reference/ai-models/model-comparison
> Citation: "Use these models for common development tasks that require a balance of quality, speed, and cost efficiency."
> Fetched: 2026-05-28

**2. Fast help with simple or repetitive tasks** — des modèles optimisés pour la vitesse et la réactivité.

Modèle recommandé : Claude Haiku 4.5.

Cas d'usage : éditer de petites fonctions, questions de syntaxe, prototypage rapide.

> Source: https://docs.github.com/en/copilot/reference/ai-models/model-comparison
> Citation: "Balances fast responses with quality output. Ideal for small tasks and lightweight code explanations."
> Fetched: 2026-05-28

**3. Deep reasoning and debugging** — les modèles conçus pour le raisonnement étape par étape et la prise de décision complexe.

Modèles recommandés : GPT-5 mini, GPT-5.5, Claude Sonnet 4.6, Claude Opus 4.7, Gemini 3.1 Pro, Goldeneye.

Cas d'usage : debugger des problèmes complexes multi-fichiers, refactorer un codebase interconnecté, planifier des features ou de l'architecture, analyser des logs ou des performances.

> Source: https://docs.github.com/en/copilot/reference/ai-models/model-comparison
> Citation: "Debug complex issues with context across multiple files. Refactor large or interconnected codebases."
> Fetched: 2026-05-28

**4. Working with visuals** — pour les questions sur des captures d'écran, diagrammes, ou composants UI.

Modèles recommandés : GPT-5 mini, Claude Sonnet 4.6, Gemini 3.1 Pro.

> Source: https://docs.github.com/en/copilot/reference/ai-models/model-comparison
> Citation: "Use these models when you want to ask questions about screenshots, diagrams, UI components, or other visual input."
> Fetched: 2026-05-28

En résumé, voici la matrice de décision rapide :

| Tu veux… | Choisis… |
|---|---|
| Un bon défaut polyvalent | GPT-4.1 ou GPT-5 mini |
| De la vitesse pure | Claude Haiku 4.5 ou Gemini 3 Flash |
| Du raisonnement profond | Claude Opus 4.7, GPT-5.5, ou Gemini 3.1 Pro |
| Analyser une capture d'écran | GPT-5 mini ou Claude Sonnet 4.6 |

### Le model picker dans VS Code

Pour changer de modèle, ouvre le *model picker* dans le champ de saisie du chat. Le sélecteur se trouve en bas de la vue chat.

> Source: https://docs.github.com/en/copilot/how-tos/use-ai-models/change-the-chat-model
> Citation: "At the bottom of the chat view, select the CURRENT-MODEL dropdown menu, then click the AI model of your choice."
> Fetched: 2026-05-28

Tu peux aussi gérer la visibilité des modèles via l'éditeur *Language Models*. Ouvre-le avec la commande `Chat: Manage Language Models` ou en cliquant sur l'icône engrenage dans le *model picker*.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "The editor lists all models available to you, showing key information such as the model capabilities, context size, billing details, and visibility status."
> Fetched: 2026-05-28

Tu peux **épingler** tes modèles favoris pour les garder en haut du sélecteur, et **masquer** ceux que tu n'utilises jamais.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "Pin models to keep them in a fixed position at the top of the model picker."
> Fetched: 2026-05-28

### Le mode Auto

Le mode *Auto* est le choix le plus simple : Copilot évalue la complexité de ta requête et la disponibilité des modèles en temps réel pour router chaque demande vers le modèle optimal.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "VS Code evaluates task complexity and real-time model availability to route each request to the optimal model."
> Fetched: 2026-05-28

Pour l'activer, sélectionne **Auto** dans le *model picker*. Tu peux voir quel modèle a été utilisé en survolant la réponse dans le chat.

C'est un bon point de départ si tu ne sais pas quel modèle choisir. Mais dès que tu sais ce que tu veux — du raisonnement profond ou de la vitesse — un choix explicite sera plus prévisible.

### Le thinking effort

Certains modèles supportent un *thinking effort* configurable, qui contrôle la quantité de raisonnement appliquée à chaque requête.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "Some models support configurable thinking effort, which controls how much reasoning the model applies to each request."
> Fetched: 2026-05-28

Les modèles non-raisonneurs (GPT-4.1, GPT-4o) n'affichent pas ce sous-menu. Pour les modèles qui le supportent (Claude Sonnet 4.6, Claude Opus 4.7, etc.), tu peux choisir un niveau d'effort.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "Non-reasoning models, such as GPT-4.1 and GPT-4o, do not show the thinking effort submenu."
> Fetched: 2026-05-28

<!-- round 2 response — U2: add citation for adaptive reasoning default -->

Par défaut, VS Code active le raisonnement adaptatif : le modèle détermine lui-même combien réfléchir en fonction de la complexité de la requête.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "By default, VS Code sets recommended effort levels and has adaptive reasoning enabled, where the model dynamically determines how much to think based on the complexity of each request."
> Fetched: 2026-05-28

### Forcer un modèle dans un agent

Tu as vu dans le [module 104](./104-agents.md) que le frontmatter `.agent.md` accepte une clé `model` optionnelle. C'est le mécanisme pour associer un modèle spécifique à un rôle.

Prenons un agent de triage qui doit être rapide et peu coûteux :

```diff
  # .github/agents/triage.agent.md

  ---
  name: triage
  description: "Classe les issues par priorité et effort."
  tools:
    - githubRepo
+ model: gpt-5-mini
  ---
```

Et un agent architecte qui a besoin de raisonnement profond :

```diff
  # .github/agents/architect.agent.md

  ---
  name: architect
  description: "Analyse architecturale — ne modifie aucun fichier."
  tools:
    - codebase
+ model: claude-opus-4.7
  ---
```

> Source: docs/01-fondations/104-agents.md
> Citation: "Spécifier un modèle est utile quand le rôle de l'agent exige un modèle particulier"
> Fetched: 2026-05-28

Si tu omets la clé `model`, l'agent utilise le modèle sélectionné dans le *model picker* global. C'est le comportement par défaut et souvent le plus flexible.

### Modèles pour les suggestions inline

Le modèle utilisé pour le *chat* et celui pour les suggestions *inline* sont indépendants. Pour changer le modèle des complétions :

1. Ouvre le menu Chat dans la barre de titre de VS Code
2. Sélectionne **Configure Inline Suggestions…**
3. Choisis **Change Completions Model…**

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "To change the language model that is used for generating inline suggestions in the editor"
> Fetched: 2026-05-28

<!-- round 2 response — U3: add citation for inline model availability -->

Les modèles disponibles pour les suggestions *inline* peuvent différer de ceux du chat — la liste évolue au fil du temps.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "The models that are available for inline suggestions might evolve over time as we add support for more models."
> Fetched: 2026-05-28

Les complétions de code ne sont pas facturées en crédits AI — elles restent illimitées pour tous les plans payants.

> Source: https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
> Citation: "Code completions and next edit suggestions are not billed in AI credits."
> Fetched: 2026-05-28

### Comprendre les coûts

À partir du 1er juin 2026, GitHub passe d'une facturation par requête à une facturation à l'usage basée sur les tokens.

> Source: https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
> Citation: "Starting June 1, 2026, GitHub is moving Copilot from request-based billing to usage-based billing."
> Fetched: 2026-05-28

Le coût d'une interaction dépend de deux facteurs : le modèle utilisé et le nombre de tokens consommés (entrée, sortie, cache). Les tokens sont convertis en crédits AI, où 1 crédit = $0.01 USD.

Voici un extrait de la grille tarifaire (prix par million de tokens) :

| Modèle | Catégorie | Input | Output |
|---|---|---|---|
| GPT-4.1 | Versatile | $2.00 | $8.00 |
| GPT-5 mini | Lightweight | $0.25 | $2.00 |
| GPT-5.5 | Powerful | $5.00 | $30.00 |
| Claude Haiku 4.5 | Versatile | $1.00 | $5.00 |
| Claude Sonnet 4.6 | Versatile | $3.00 | $15.00 |
| Claude Opus 4.7 | Powerful | $5.00 | $25.00 |
| Gemini 2.5 Pro | Powerful | $1.25 | $10.00 |
| Gemini 3 Flash | Lightweight | $0.50 | $3.00 |

> Source: https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
> Citation: "Each token is priced based on the model used, and the total is converted into AI credits, where 1 AI credit = $0.01 USD."
> Fetched: 2026-05-28

<!-- round 2 response — U1: fix 120× to 15× ($30/$2) and add citation -->

La leçon est claire : un modèle *Powerful* comme GPT-5.5 coûte 15× plus en *output* qu'un GPT-5 mini ($30 vs $2 par million de tokens). Utiliser systématiquement le modèle le plus puissant pour des questions simples revient à prendre un semi-remorque pour aller chercher le pain.

> Source: https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
> Citation: "GPT-5.5 … Output: $30.00" / "GPT-5 mini … Output: $2.00"
> Fetched: 2026-05-28

### Un seul modèle par session

> **Garde le même modèle sur toute une session.** Changer de modèle en cours de session casse le cache de prompt, produit des résultats incohérents et gaspille des tokens — une session doit rester dédiée à un seul modèle.

Le *cache de prompt* réutilise les tokens d'entrée déjà traités d'une requête à l'autre — ils sont alors facturés bien moins cher. Changer de modèle en cours de route invalide ce cache : tout le préfixe repasse au plein tarif. Si la phase suivante exige un autre modèle, tu ne changes pas de modèle dans la session — tu transfères le contexte proprement (voir [Module 316 — Déplacer le contexte entre modèles](../03-ingenierie-de-contexte/316-deplacer-contexte-modeles.md)).

### BYOK — Bring Your Own Key

Si tu veux utiliser un modèle qui n'est pas intégré nativement, ou contrôler l'hébergement, tu peux apporter ta propre clé API.

> Source: https://code.visualstudio.com/docs/copilot/language-models
> Citation: "you can bring your own language model API key (BYOK) to use models from other providers or to run models locally."
> Fetched: 2026-05-28

Pour les utilisateurs Copilot Business ou Enterprise, l'administrateur doit activer la politique *Bring Your Own Language Model Key in VS Code* dans les paramètres de politique Copilot sur GitHub.com.

> Source: https://docs.github.com/en/copilot/how-tos/use-ai-models/change-the-chat-model
> Citation: "the Bring Your Own Language Model Key in VS Code policy must be enabled."
> Fetched: 2026-05-28

## Exercice

**Énoncé** — En 15 minutes, tu vas explorer les modèles disponibles et configurer ton *model picker* pour ton workflow quotidien.

**Étapes guidées** :

1. Ouvre le *model picker* dans le chat Copilot. Note combien de modèles sont disponibles.

2. Épingle tes 2-3 modèles préférés. Suggestion : un modèle rapide (GPT-5 mini ou Claude Haiku 4.5) et un modèle de raisonnement (Claude Sonnet 4.6 ou GPT-5.5).

3. Masque les modèles que tu n'utiliseras pas via l'éditeur *Language Models* (`Chat: Manage Language Models`).

4. Pose la même question à deux modèles différents — un rapide et un de raisonnement — et compare les réponses :

```
Explique-moi les trade-offs entre un monorepo et un multi-repo
pour une équipe de 5 développeurs.
```

5. Crée un agent qui force un modèle spécifique :

```diff
+ # .github/agents/quick-helper.agent.md
+
+ ---
+ name: quick-helper
+ description: "Réponses rapides aux questions simples."
+ model: gpt-5-mini
+ ---
+
+ Tu es un assistant concis. Réponds en 3 phrases maximum.
+ Pas d'exemples de code sauf si explicitement demandé.
```

6. Active le mode **Auto** et pose trois questions de complexité croissante. Survole chaque réponse pour voir quel modèle a été sélectionné.

**Critère de réussite** : tu as épinglé au moins 2 modèles, masqué au moins 1 modèle, et ton agent `quick-helper` répond avec le modèle spécifié.

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais nommer au moins 3 modèles de fournisseurs différents disponibles dans Copilot.
- [ ] Tu sais changer de modèle dans le *model picker* sans chercher.
- [ ] Tu sais expliquer la différence de coût entre un modèle *Lightweight* et un modèle *Powerful*.
- [ ] Tu as testé la clé `model` dans un `.agent.md` et vérifié qu'elle s'applique.
- [ ] Tu comprends quand utiliser *Auto* et quand forcer un choix explicite.

## Pour aller plus loin

- [Comparaison officielle des modèles](https://docs.github.com/en/copilot/reference/ai-models/model-comparison) — la grille complète par catégorie de tâche.
- [Tarification par modèle](https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing) — détail des prix par million de tokens.
- [Language Models dans VS Code](https://code.visualstudio.com/docs/copilot/language-models) — documentation complète du *model picker*, BYOK, et modèles utilitaires.
- Le module sur les tokens et le contexte (bloc 3) approfondit l'impact du choix de modèle sur la fenêtre de contexte et la sobriété d'usage.

## Module suivant

**Suivant** : [207 — APM](../02-composition/207-apm.md)

## Sources

1. https://code.visualstudio.com/docs/copilot/language-models
2. https://docs.github.com/en/copilot/how-tos/use-ai-models/change-the-chat-model
3. https://docs.github.com/en/copilot/reference/ai-models/model-comparison
4. https://docs.github.com/en/copilot/reference/copilot-billing/models-and-pricing
5. docs/01-fondations/104-agents.md
6. Retour d'expérience d'Ousmane BARRY (MVP Microsoft Foundry) — [publication LinkedIn](https://www.linkedin.com/posts/ousmanebarry_depuis-le-1er-juin-on-a-tous-vu-et-senti-ugcPost-7468560023623901184-K69e/) (règle « un seul modèle par session »)
