---
id: app-installer-tour
title: "430 — GitHub Copilot app : installer et faire le tour"
sidebar_position: 430
description: "Installer la GitHub Copilot app, connecter son fork et distinguer chat, session projet et vues principales avant de modifier du code."
---

# 430 — GitHub Copilot app : installer et faire le tour

**Durée** : ~30 min · **Complexité** : ⭐ · **Pré-requis** : [420 — Choisir sa surface Copilot](./420-choisir-sa-surface.md), un fork du dépôt, Git installé

> *L'app Copilot est le poste de pilotage local de l'agent : elle garde ensemble le prompt, le plan, le diff, le terminal, l'aperçu navigateur et GitHub.*

## Objectif

À la fin de ce module, tu sais :

- installer et ouvrir la GitHub Copilot app ;
- connecter ton fork comme projet ;
- reconnaître les vues **New**, **Pull requests**, **Issues**, **Automations**, **Customize** et **Projects** ;
- choisir entre un chat et une session projet ;
- retrouver les principaux réglages dans **Settings** ;
- faire visiter `samples/mon-sinistre` par Copilot sans modifier de fichier.

## Ce que tu vas apprendre

1. Pourquoi utiliser l'app si tu as déjà un IDE ou le CLI
2. Connecter ton fork comme projet
3. Faire le tour des vues principales
4. Chat ou session projet : le bon niveau d'engagement
5. Settings : ce qu'il faut savoir au début

## Contenu pédagogique

### Pourquoi l'app

Copilot dans l'IDE est très efficace quand tu travailles déjà dans un fichier. Copilot CLI est parfait quand tu veux piloter l'agent depuis le terminal. L'app répond à un autre besoin : **superviser un travail agentique complet**.

Elle rassemble dans une même fenêtre :

| Besoin | Ce que l'app apporte |
|---|---|
| Comprendre une tâche | conversation, fichiers attachés, issues et PR |
| Encadrer l'agent | mode, modèle, permissions d'outils |
| Vérifier une modification | diff, terminal, tests, logs |
| Voir le résultat | aperçu navigateur local |
| Passer à GitHub | issue, PR, commentaires, état de validation |

> Source : [copilot-app-for-beginners — 01 Tour the App](https://github.com/github/copilot-app-for-beginners/blob/main/01-tour-the-app/README.md), section « Why Use the GitHub Copilot app? ».

L'app ne remplace donc pas VS Code, Visual Studio ou JetBrains. Elle te donne un **tableau de bord** pour lancer, suivre et relire le travail de l'agent. Depuis une session, tu peux toujours ouvrir le dossier dans VS Code quand tu veux reprendre la main avec un éditeur complet.

### Installer et se connecter

1. Télécharge la GitHub Copilot app depuis la page officielle GitHub.
2. Ouvre l'app et choisis **Sign in to GitHub**.
3. Connecte-toi avec le compte qui a accès à Copilot.
4. Si ton organisation utilise Copilot Business ou Enterprise, vérifie que la politique **GitHub Copilot app** est activée.
5. À l'onboarding, tu peux ignorer la sélection initiale de dépôts et ajouter ton fork ensuite.

> Source : [Getting started with the GitHub Copilot app](https://docs.github.com/en/copilot/get-started/quickstart-copilot-app).

### Connecter ton fork

Dans la barre latérale :

1. Clique sur **+** à côté de **Projects**.
2. Choisis **Add GitHub repository** si ton fork est sur GitHub, ou **Open folder** si tu l'as déjà cloné.
3. Sélectionne **ton fork**, pas le dépôt amont.
4. Attends que l'app clone ou ouvre le projet.

Pour les exercices AXA, le projet est ton fork de ce handbook. Le dossier important pour ce bloc est :

```text
samples/mon-sinistre/
```

Tu le connais déjà depuis le module 420 : application .NET 10, CLI, web, tests et script `verify.sh`.

### Tour des vues

| Vue | À quoi elle sert dans la série app |
|---|---|
| **New** | Démarrer rapidement un chat ou une session, choisir un projet, un mode et un modèle |
| **Pull requests** | Lire les PR qui te concernent, ouvrir une PR d'entraînement, lancer une session depuis une PR |
| **Issues** | Voir les issues assignées ou créées, démarrer une session avec l'issue attachée comme contexte |
| **Automations** | Préparer des tâches récurrentes ; la série y revient plus tard |
| **Customize** | Gérer skills, serveurs MCP, plugins, extensions et canvases |
| **Projects** | Retrouver tes dépôts connectés, sessions actives et chats associés |

Le module 434 utilisera surtout **Issues** et **Pull requests**. Pour l'instant, retiens simplement où elles se trouvent.

### Chat ou session projet

La différence est centrale :

| Type | Quand l'utiliser | Branche ou worktree ? |
|---|---|---|
| **Chat** | Question générale, brainstorming, explication, orientation | Non |
| **Session projet** | Analyse de dépôt, modification, tests, PR | Oui le plus souvent, selon le choix de workspace |

Un chat est idéal pour demander : « explique-moi ce dépôt ». Une session projet est préférable dès que l'agent doit lire précisément les fichiers, exécuter `dotnet test`, modifier du code ou préparer une PR.

Dans ce premier module, tu vas utiliser les deux, mais avec une consigne explicite : **ne modifier aucun fichier**.

### Settings à connaître

Ouvre les réglages avec l'icône engrenage en bas de la barre latérale.

| Zone | Utile pour |
|---|---|
| **General** | mise à jour, thème, emplacement des dépôts |
| **Accounts** | compte GitHub connecté |
| **Sessions** | historique, style de réponse, instructions globales, préfixe de branches |
| **Projects** | réglages propres à un dépôt connecté |
| **Customize** | raccourcis vers skills, plugins, MCP et canvases |
| **Model providers** | fournisseurs de modèles externes si ton organisation les autorise |

Tu n'as rien à changer pour commencer. L'objectif est seulement de savoir où regarder si l'app ne se comporte pas comme attendu.

## Exercice

**Énoncé** : connecte ton fork et fais une visite guidée de `samples/mon-sinistre`, sans modification.

### Partie A — Chat rapide

1. Dans **Projects**, clique sur **+**, puis choisis **Chat**.
2. Envoie :

```text
Explique-moi quand utiliser un chat et quand utiliser une session projet dans la GitHub Copilot app. Ne crée pas et ne modifie aucun fichier.
```

3. Vérifie que l'app répond sans créer de branche ni de worktree.

### Partie B — Session projet de visite guidée

1. À côté de ton fork dans **Projects**, clique sur **+** pour démarrer une session.
2. Choisis un workspace de type **Current checkout** uniquement pour cette visite sans modification, ou **New worktree** si tu préfères garder l'habitude d'isolation dès maintenant.
3. Envoie :

```text
Fais-moi une visite guidée débutant-friendly de @samples/mon-sinistre.
Explique ce que fait l'application, les technologies utilisées, les principaux dossiers,
le flux de données, ce que couvrent les tests et l'ordre de lecture conseillé.
Ne modifie aucun fichier.
```

4. Ouvre le panneau de revue et vérifie qu'aucun diff n'apparaît.

## Validation

- [ ] Ton fork apparaît dans **Projects**.
- [ ] Tu sais retrouver les vues **New**, **Pull requests**, **Issues**, **Automations**, **Customize** et **Projects**.
- [ ] Tu sais expliquer la différence entre chat et session projet.
- [ ] La visite guidée cite des chemins sous `samples/mon-sinistre`.
- [ ] Aucun fichier n'a été modifié.

## Pour aller plus loin

- [431 — Modes, modèles et permissions](./431-app-modes-modeles-permissions.md)
- [Getting started with the GitHub Copilot app](https://docs.github.com/en/copilot/get-started/quickstart-copilot-app)
- [Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions)

## Source

- [github/copilot-app-for-beginners — 00 Setup](https://github.com/github/copilot-app-for-beginners/blob/main/00-setup/README.md)
- [github/copilot-app-for-beginners — 01 Tour the App](https://github.com/github/copilot-app-for-beginners/blob/main/01-tour-the-app/README.md)
- [GitHub Docs — Getting started with the GitHub Copilot app](https://docs.github.com/en/copilot/get-started/quickstart-copilot-app)
