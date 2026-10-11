---
id: cli-mcp
title: "427 — Copilot CLI : connecter des outils avec MCP"
sidebar_position: 427
description: "Comprendre et gérer les serveurs MCP dans Copilot CLI, utiliser le serveur GitHub intégré et cadrer un serveur de documentation sans exposer inutilement le code."
---

# 427 — Copilot CLI : connecter des outils avec MCP

**Durée** : ~40 min · **Complexité** : ⭐⭐ · **Pré-requis** : [426 — Skills CLI](./426-cli-skills.md), [106 — MCP](../01-fondations/106-mcp.md)

> *Le contexte ne vient pas toujours d'un fichier attaché avec `@`. MCP donne à Copilot des connecteurs explicites : GitHub, documentation, systèmes internes. Ton rôle est de choisir ceux qui sont utiles, puis de vérifier leurs preuves.*

## Objectif

À la fin de ce module, tu sais expliquer ce qu'un serveur MCP ajoute à Copilot CLI, lister les serveurs disponibles, utiliser le serveur GitHub intégré pour interroger ton fork Mon Sinistre, et décider quand ajouter un serveur de documentation plutôt que coller de longs extraits dans le prompt.

## Ce que tu vas apprendre

1. Ce qu'un serveur MCP apporte au CLI
2. Comment voir, ajouter et configurer des serveurs MCP
3. Ce que fournit le serveur GitHub intégré
4. Comment l'utiliser avec les issues créées par `seed.sh`
5. Comment limiter les données envoyées aux serveurs externes

## Contenu pédagogique

### MCP : des outils externes déclarés

Le Model Context Protocol est un standard qui permet à une application IA de connecter des outils et sources de données externes à un modèle. Dans Copilot CLI, un serveur MCP peut fournir des outils pour consulter GitHub, interroger de la documentation, accéder à des données ou appeler un service.

Sans MCP, tu dois souvent attacher explicitement le contexte avec `@` ou copier une information. Avec MCP, Copilot peut utiliser un outil configuré pour récupérer une issue, une PR, un statut d'action ou une page de documentation, selon les droits du serveur.

### Gérer les serveurs dans Copilot CLI

En session interactive, `/mcp show` affiche les serveurs configurés et leur état. Tu peux ajouter un serveur avec `/mcp add` ou depuis le terminal avec `copilot mcp add`.

```bash
copilot
> /mcp show
> /mcp add
```

```bash
copilot mcp add context7 -- npx -y @upstash/context7-mcp
copilot mcp add --transport http docs https://mcp.example.invalid/mcp
```

Les configurations peuvent être personnelles ou projet. Le cours CLI cite `~/.copilot/mcp-config.json`, `.mcp.json` et `.github/mcp.json`; la documentation GitHub précise que `.mcp.json` et `.github/mcp.json` servent aux configurations par dépôt, avec précédence locale selon l'emplacement.

### Le serveur GitHub intégré

Copilot CLI inclut déjà un serveur MCP GitHub. Une fois authentifié, il permet de travailler avec des ressources GitHub comme dépôts, issues, pull requests, recherche de code et statuts d'actions, sans ajouter de configuration GitHub supplémentaire.

Pour Mon Sinistre, c'est suffisant pour l'exercice : `scripts/seed.sh` crée des issues et PR d'entraînement dans ton fork, avec le label `mon-sinistre`.

### MCP ne remplace pas la vérification

MCP donne accès à une source, mais ne rend pas automatiquement la réponse vraie. Tu dois inspecter l'activité d'outil, lire les liens ou objets retournés, et séparer ce que l'outil a réellement fourni de ce que l'agent déduit.

Cette règle vaut particulièrement pour les serveurs de documentation. Une bonne pratique consiste à poser une question générale au serveur externe, puis à comparer localement avec le code sans envoyer inutilement les chemins ou extraits du dépôt.

```mermaid
flowchart TD
    A[Besoin d'information] --> B{Source locale suffisante ?}
    B -- Oui --> L[@ fichier / dossier]
    B -- Non --> C{Source externe utile ?}
    C -- GitHub --> G[Serveur GitHub intégré]
    C -- Docs --> D[Serveur doc MCP]
    G --> V[Vérifier l'activité outil]
    D --> V
    V --> R[Comparer avec le code local]
```

### Ajouter un serveur de doc : option, pas réflexe

Le chapitre CLI présente Context7 comme un serveur qui donne accès à de la documentation à jour pour des frameworks et bibliothèques. La documentation GitHub montre aussi les serveurs distants HTTP et les serveurs locaux STDIO.

Dans Mon Sinistre, tu n'as pas besoin d'un serveur de documentation pour comprendre les règles métier : elles sont dans le README et les tests. Tu peux en ajouter un seulement si tu dois vérifier une API externe, par exemple une documentation .NET, et si ta politique d'organisation l'autorise.

## Exercice

**Énoncé principal** : utilise le serveur GitHub intégré pour lister les issues de ton fork après `seed.sh`.

1. Depuis la racine de ton fork, assure-toi que GitHub CLI est authentifié et que ton dépôt local pointe vers ton fork.
2. Lance les données d'entraînement :

```bash
./samples/mon-sinistre/scripts/seed.sh
```

3. Ouvre Copilot CLI et vérifie MCP :

```text
/mcp show
```

4. Demande :

```text
Avec le serveur GitHub MCP intégré, liste les issues ouvertes de ce dépôt qui portent le label mon-sinistre. Pour chacune, donne le numéro, le titre, le défaut D1/D2/D3 concerné si visible, et ne modifie rien.
```

5. Compare la réponse avec les issues visibles sur GitHub dans ton fork.
6. Choisis une issue et demande un plan sans modification :

```text
Pour l'issue #<numero>, prépare un plan de correction local pour Mon Sinistre. N'édite aucun fichier. Termine par les commandes de validation attendues.
```

**Option documentation** : si ton organisation autorise l'ajout de serveurs, ajoute un serveur de documentation et pose-lui seulement une question générale. N'envoie pas le code Mon Sinistre au serveur externe.

:::warning Pas de secret dans la configuration
Ne mets jamais de token ou secret dans un prompt ou un fichier commité. Si un serveur demande un jeton, utilise le mécanisme prévu par le CLI ou ton gestionnaire d'identifiants.
:::

## Validation

- [ ] `/mcp show` affiche au moins le serveur GitHub intégré ou son équivalent disponible.
- [ ] Les issues `mon-sinistre` créées par `seed.sh` sont listées depuis Copilot CLI.
- [ ] Tu as vérifié la réponse avec la page GitHub de ton fork.
- [ ] Aucun fichier du sample n'a été modifié pour cet exercice.
- [ ] Si tu as ajouté un serveur de documentation, la question externe est générale et ne contient pas de code du dépôt.
- [ ] `./scripts/verify.sh` reste vert depuis `samples/mon-sinistre` si tu as lancé une correction après le plan.

## Pour aller plus loin

- [Module 106 — MCP](../01-fondations/106-mcp.md) — modèle conceptuel.
- [Module 209 — Plugins](../02-composition/209-plugins.md) — autre forme d'extension.
- [Module 428 — Mode non interactif](./428-cli-non-interactif.md) — automatiser un appel CLI avec garde-fous.

## Sources

- [github/copilot-cli-for-beginners — Chapter 06](https://github.com/github/copilot-cli-for-beginners/blob/main/06-mcp-servers/README.md) — cours GitHub sous licence MIT, adapté en français.
- [github/copilot-app-for-beginners — Chapter 05](https://github.com/github/copilot-app-for-beginners/blob/main/05-mcp-plugins/README.md) — vérification des outils MCP, adapté en français.
- [Adding MCP servers for GitHub Copilot CLI](https://docs.github.com/copilot/how-tos/copilot-cli/customize-copilot/add-mcp-servers) · [Invoking custom agents](https://docs.github.com/copilot/how-tos/copilot-cli/use-copilot-cli/invoke-custom-agents)
- [Mon Sinistre — README, « Script de seed »](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
