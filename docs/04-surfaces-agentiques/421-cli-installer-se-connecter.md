---
id: cli-installer-se-connecter
title: "421 — Copilot CLI : installer et se connecter"
sidebar_position: 421
description: "Installer GitHub Copilot CLI, l'authentifier, faire confiance au dossier Mon Sinistre et réussir un premier prompt de lecture de code."
---

# 421 — Copilot CLI : installer et se connecter

**Durée** : ~25 min · **Complexité** : ⭐ · **Pré-requis** : [420 — Choisir sa surface Copilot](./420-choisir-sa-surface.md), un compte GitHub avec accès Copilot, un terminal

> *Le premier bon usage du CLI n'est pas de lui demander de tout corriger. C'est de vérifier qu'il voit le bon dossier, qu'il est connecté au bon compte, et qu'il comprend déjà le projet que tu vas travailler.*

## Objectif

À la fin de ce module, tu sais :

- installer le binaire `copilot` avec la méthode adaptée à ton poste ;
- activer la complétion shell si elle est utile à ton quotidien ;
- te connecter avec `/login`, en comprenant la différence entre navigateur et device code ;
- faire confiance au dossier `samples/mon-sinistre` ;
- lancer ton premier prompt sur `Program.cs`.

## Ce que tu vas apprendre

1. Les méthodes d'installation disponibles
2. La complétion shell
3. La confiance accordée au dossier
4. L'authentification : navigateur ou device code
5. Le premier prompt dans Mon Sinistre
6. Le cas Codespaces

## Contenu pédagogique

### Installer le binaire `copilot`

:::caution Le CLI peut être désactivé par ton organisation
Il faut un abonnement Copilot actif. Si ta licence Copilot vient d'une **organisation** ou d'une **entreprise**, l'owner ou l'admin peut **désactiver Copilot CLI** dans les policies Copilot. Dans ce cas, le CLI n'est pas disponible pour toi, même s'il est installé. Avant de commencer, vérifie auprès de ton équipe plateforme que la policy Copilot CLI est activée. Sinon, suis la série app (430 → 438) ou utilise la cible de session Copilot dans VS Code (module 420), si elles sont autorisées.
:::

GitHub Copilot CLI se lance avec la commande `copilot` et permet d'utiliser Copilot directement depuis le terminal.

Le cours GitHub propose plusieurs chemins d'installation : `npm`, Homebrew, WinGet et un script d'installation macOS/Linux. La documentation GitHub met aussi en avant `npm`, WinGet et Homebrew ; dans les deux cas, le principe est le même : installer le binaire `copilot`, puis ouvrir une session interactive.

```bash
# npm, tous OS si Node.js est disponible
npm install -g @github/copilot

# macOS / Linux, Homebrew
brew install --cask copilot-cli

# Windows, WinGet
winget install GitHub.Copilot

# macOS / Linux, script d'installation du cours
curl -fsSL https://gh.io/copilot-install | bash
```

:::note Choisir simplement
Si ton poste a déjà Node.js, `npm` est souvent le chemin le plus direct. Si ton équipe standardise les postes avec Homebrew ou WinGet, préfère le gestionnaire déjà administré par l'équipe.
:::

Après installation, vérifie seulement que la commande répond :

```bash
copilot version
copilot help
```

La référence officielle liste aussi `copilot help`, `copilot login`, `copilot completion`, `copilot config`, `copilot init`, `copilot plugin`, `copilot skill`, `copilot mcp`, `copilot update` et `copilot version`.

### Activer la complétion shell

La commande `copilot completion SHELL` génère un script de complétion pour Bash, Zsh ou Fish. Elle complète les sous-commandes, options et certaines valeurs connues.

```bash
# Bash, session courante
source <(copilot completion bash)

# Bash, installation persistante Linux
copilot completion bash | sudo tee /etc/bash_completion.d/copilot

# Zsh
copilot completion zsh > "${fpath[1]}/_copilot"

# Fish
copilot completion fish > ~/.config/fish/completions/copilot.fish
```

Le cours précise que PowerShell peut exécuter Copilot CLI sur Windows, mais que la commande `copilot completion` couvre Bash, Zsh et Fish.

### Faire confiance au dossier

Au premier lancement dans un projet, le CLI peut te demander si tu fais confiance aux fichiers du dossier courant. Cette étape compte : Copilot va lire du code et peut proposer d'exécuter des commandes ou de modifier des fichiers, mais la documentation de démarrage rappelle qu'il ne modifie pas tes fichiers sans ton approbation explicite.

Dans ce bloc, lance Copilot depuis le projet d'exemple :

```bash
cd samples/mon-sinistre
copilot
```

Le projet **Mon Sinistre** est une solution .NET 10 avec un domaine métier, un CLI, une application web, des tests xUnit et des scripts `verify.sh` / `verify.ps1`.

:::caution Dossier de travail
Ne démarre pas une session au hasard dans ton répertoire personnel. Place-toi dans `samples/mon-sinistre` pour que les chemins `@src/...` soient courts, pertinents et limités au sample.
:::

### Se connecter : `/login`, navigateur ou device code

Dans une session interactive, la commande `/login` déclenche l'authentification GitHub. En local, le flux recommandé ouvre le navigateur ; en terminal distant ou sans navigateur, le CLI utilise le device code et te demande d'entrer un code sur `github.com/login/device`.

Tu peux aussi forcer le flux :

```bash
copilot login --web-flow
copilot login --device-code
```

La référence indique que `--web-flow` force le flux navigateur, que `--device-code` force le flux par code, et que les terminaux distants, Codespaces, dev containers ou CI basculent par défaut vers le device code.

### Premier prompt : lire le CLI Mon Sinistre

Une fois connecté, commence par une demande d'explication, pas par une modification :

```text
Que fait @src/MonSinistre.Cli/Program.cs ?
```

La syntaxe `@` permet de référencer un fichier, un dossier ou plusieurs fichiers dans une conversation Copilot CLI.

Le fichier `src/MonSinistre.Cli/Program.cs` charge le portefeuille depuis `data/sinistres.json` ou depuis `MON_SINISTRE_DATA`, expose les commandes `list`, `show` et `declare`, affiche les détails d'un sinistre, vérifie couverture et délai, puis calcule l'indemnisation via le domaine.

### Codespaces : zéro installation locale

Le cours GitHub présente Codespaces comme une option « zero setup » : tu forks le dépôt de cours, tu crées un Codespace, puis tu ouvres le terminal déjà préparé.

Pour ce handbook, l'équivalent est simple : ouvre ton fork du dépôt du handbook dans Codespaces, puis démarre dans `samples/mon-sinistre`. Tu devras quand même vérifier l'authentification et la confiance dossier.

:::warning Différent de l'ancienne extension `gh copilot`
Le module [210 — Copilot CLI](../02-composition/210-copilot-cli.md) parlait de l'ancienne expérience autour de GitHub CLI. Ici, on utilise le binaire `copilot`, avec ses commandes propres (`copilot login`, `copilot completion`, `copilot -p`, slash commands dans l'interface interactive). Ne mélange pas les exemples `gh copilot ...` et `copilot ...`.
:::

## Exercice

**Énoncé** : installe, connecte et vérifie Copilot CLI sur Mon Sinistre.

1. Depuis la racine du dépôt, va dans le sample :
   ```bash
   cd samples/mon-sinistre
   ```
2. Lance le critère de départ :
   ```bash
   ./scripts/verify.sh        # Windows : pwsh ./scripts/verify.ps1
   ```
3. Démarre Copilot CLI :
   ```bash
   copilot
   ```
4. Si nécessaire, exécute `/login`, choisis le flux adapté à ton terminal, puis fais confiance au dossier.
5. Pose ce prompt :
   ```text
   Que fait @src/MonSinistre.Cli/Program.cs ?
   ```
6. Demande ensuite :
   ```text
   Quelles commandes utilisateur ce CLI expose-t-il ? Réponds avec les commandes dotnet run exactes.
   ```

## Validation

- [ ] Tu as vérifié que la policy Copilot CLI de ton organisation est activée.
- [ ] `copilot version` ou `copilot help` répond dans ton terminal.
- [ ] Tu sais expliquer si ton login a utilisé le navigateur ou un device code.
- [ ] La session Copilot est ouverte depuis `samples/mon-sinistre`.
- [ ] Copilot sait résumer `@src/MonSinistre.Cli/Program.cs`.
- [ ] `./scripts/verify.sh` ou `pwsh ./scripts/verify.ps1` est vert.

## Pour aller plus loin

- [Module 210 — Copilot CLI](../02-composition/210-copilot-cli.md) — repère l'ancienne surface et évite la confusion de commandes.
- [Module 422 — Copilot CLI : modes et slash commands](./422-cli-modes-slash-commands.md) — choisir entre interactive, plan et programmatic.
- [GitHub Docs — Copilot CLI command reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference) — référence complète.

## Sources

- [github/copilot-cli-for-beginners — 00 Quick Start](https://github.com/github/copilot-cli-for-beginners/blob/main/00-quick-start/README.md) — cours GitHub sous licence MIT, adapté en français.
- GitHub Docs — [Installing GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/copilot-cli/set-up-copilot-cli/install-copilot-cli)
- GitHub Docs — [Managing policies and features for Copilot in your organization](https://docs.github.com/en/copilot/how-tos/administer-copilot/manage-for-organization/manage-policies)
- [GitHub Docs — Getting started with GitHub Copilot CLI](https://docs.github.com/en/copilot/get-started/cli-quickstart)
- [GitHub Docs — Copilot CLI command reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference)
- [Mon Sinistre — README](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
- [GitHub Docs — GitHub Copilot CLI](https://docs.github.com/en/copilot/concepts/copilot-surfaces/copilot-cli)
- [copilot-cli-for-beginners — 02 Context and Conversations](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)
- [`samples/mon-sinistre/src/MonSinistre.Cli/Program.cs`](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/src/MonSinistre.Cli/Program.cs)
