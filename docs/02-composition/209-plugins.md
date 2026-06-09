---
id: plugins
title: "209 — Plugins — découvrir et installer des extensions"
sidebar_position: 209
description: "Installer des plugins tiers pour étendre Copilot avec des skills, agents et outils pré-packagés."
---

# 209 — Plugins — découvrir et installer des extensions

Durée estimée : 45 min

> Tu sais créer des primitives et les orchestrer dans un workflow. Mais pourquoi repartir de zéro quand quelqu'un a déjà packagé exactement ce dont tu as besoin ?

## Pourquoi ce module

Dans les modules précédents, tu as appris à créer des skills, des agents, à les distribuer avec APM et à les orchestrer en workflows. Chaque fois, tu partais d'une page blanche. C'est formateur — mais sur un projet réel, tu veux avancer vite. Quelqu'un a déjà packagé un kit de productivité pour le TDD ? Un toolkit complet pour migrer vers React 19 ? Un ensemble d'outils CLI légers pour réduire le contexte envoyé au modèle ? Tu veux pouvoir les installer en une commande et commencer à les utiliser immédiatement.

C'est exactement ce que font les `plugin`. Un plugin est un bundle de skills, agents, hooks et/ou serveurs MCP, distribué via un dépôt Git et installable directement dans VS Code. Le concept existe aussi chez Claude Code et Cursor, avec des variantes que tu verras dans ce module.

Le `marketplace` officiel Copilot — `github/awesome-copilot` — référence plus de 60 plugins couvrant des domaines variés : migration React, bonnes pratiques sécurité, développement Java, C#, Go, Python, planification de projet, automatisation de tests, et bien d'autres.

À la fin de ce module, tu sais :

- expliquer ce qu'est un plugin et en quoi il diffère d'une primitive individuelle distribuée via APM ;
- naviguer dans le marketplace `github/awesome-copilot` et choisir un plugin adapté à ton besoin ;
- installer un plugin dans VS Code et vérifier qu'il est actif ;
- distinguer les scopes d'installation (user vs project) ;
- créer ton propre plugin minimal contenant un skill et un agent ;
- évaluer la sécurité d'un plugin avant de l'installer.

## Pré-requis

- [Module 207 — APM — installer et partager](./207-apm.md)
- [Module 208 — Workflows — orchestrer plusieurs primitives](./208-workflows.md)
- VS Code (Insiders ou stable) avec l'extension GitHub Copilot activée.
- Un dépôt Git hébergé sur GitHub.

## Concepts clés

### Qu'est-ce qu'un plugin ?

Un plugin est un **paquet de primitives agentiques** — skills, agents, et potentiellement hooks ou serveurs MCP — distribué sous forme de dépôt Git et installable d'un bloc dans ton environnement. Si APM te permet d'installer une primitive individuelle (`apm install owner/repo/skills/mon-skill`), un plugin te livre un **ensemble cohérent** de primitives conçues pour fonctionner ensemble.

Prenons un exemple concret. Le plugin `react19-upgrade` ne contient pas un seul skill isolé — il embarque plusieurs skills spécialisés (`react19-concurrent-patterns`, `react19-source-patterns`) et des agents dédiés, le tout formant un toolkit complet pour migrer une application vers React 19. Tu installes le plugin une fois, et tu récupères tout le kit.

### Anatomie d'un plugin

Un plugin vit dans un dépôt Git et expose un manifeste `.github/plugin/plugin.json` qui décrit son contenu :

```json
{
  "name": "react19-upgrade",
  "description": "Enterprise React 19 migration toolkit...",
  "version": "1.0.0",
  "keywords": ["react19", "migration"],
  "agents": ["./agents"],
  "skills": [
    "./skills/react19-concurrent-patterns",
    "./skills/react19-source-patterns"
  ]
}
```

Les champs clés :

| Champ | Rôle |
|---|---|
| `name` | Identifiant unique du plugin |
| `description` | Description affichée dans le marketplace et utilisée par le routeur sémantique |
| `version` | Version sémantique du plugin |
| `keywords` | Mots-clés pour la recherche dans le marketplace |
| `agents` | Chemin(s) vers les dossiers contenant les `.agent.md` |
| `skills` | Chemin(s) vers les dossiers contenant les `SKILL.md` |

La structure de fichiers typique d'un plugin ressemble à ceci :

```text
mon-plugin/
├── .github/
│   └── plugin/
│       └── plugin.json
├── agents/
│   └── migration-guide.agent.md
└── skills/
    ├── concurrent-patterns/
    │   └── SKILL.md
    └── source-patterns/
        └── SKILL.md
```

### Le marketplace `github/awesome-copilot`

Le dépôt `github/awesome-copilot` est le marketplace officiel de plugins Copilot. Il référence plus de 60 plugins classés par domaine. Quelques exemples notables :

| Plugin | Domaine |
|---|---|
| `react19-upgrade` | Migration React 19 |
| `security-best-practices` | Sécurité applicative |
| `java-development` | Développement Java |
| `csharp-dotnet-development` | Développement C# / .NET |
| `go-mcp-development` | Développement Go + MCP |
| `python-mcp-development` | Développement Python + MCP |
| `project-planning` | Planification de projet |
| `testing-automation` | Automatisation de tests |
| `software-engineering-team` | Équipe d'ingénierie logicielle |

Le marketplace n'est pas un registre binaire comme npm ou NuGet — c'est un **catalogue de liens vers des dépôts Git**. Chaque entrée pointe vers le dépôt source du plugin. L'installation se fait depuis VS Code, pas depuis le site web.

### Installation d'un plugin

Tu installes un plugin depuis la palette de commandes VS Code. Copilot clone le dépôt du plugin et l'enregistre dans le fichier `installed.json`.

Après installation, le plugin est stocké dans le dossier des plugins de l'agent :

```text
~/.vscode-insiders/agent-plugins/github.com/<owner>/<repo>/
```

Pour VS Code stable, remplace `vscode-insiders` par `vscode`.

Le fichier `installed.json` (dans le même répertoire parent) référence chaque plugin installé avec son `pluginUri` et sa source `marketplace`. C'est l'équivalent du `package-lock.json` pour les plugins Copilot — il garantit la reproductibilité de ton environnement.

#### Scopes d'installation

Un plugin peut être installé à deux niveaux :

| Scope | Portée | Cas d'usage |
|---|---|---|
| **user** | Actif dans tous tes workspaces | Outils de productivité personnels (brainstorming, TDD, debugging) |
| **project** | Actif uniquement dans le workspace courant | Plugins spécifiques au stack du projet (React 19, Java, etc.) |

Un plugin installé en scope `user` est disponible partout — pratique pour tes outils quotidiens. Un plugin en scope `project` n'intervient que dans le contexte du projet concerné, ce qui évite de polluer tes autres workspaces avec des skills inutiles.

### Plugins communautaires notables

Au-delà du marketplace officiel, des plugins communautaires méritent ton attention :

**`obra/superpowers`** — Un kit de skills de productivité couvrant le brainstorming, le TDD, le debugging systématique, la code review, les agents parallèles, les git worktrees, et la rédaction de plans. C'est un bon exemple de plugin « couteau suisse » qui enrichit ton workflow quotidien sans cibler un stack technique particulier.

**`axa-fr/design-system`** — Un plugin qui embarque le design system d'AXA sous forme de skills et d'instructions Copilot. L'agent génère des composants UI conformes à la charte (tokens de design, espacements, couleurs, typographies) sans que le développeur ait besoin de consulter la documentation manuellement. C'est un bon exemple de plugin d'entreprise qui encode les conventions internes d'un produit — directement lié aux préoccupations du [Module 311](../03-ingenierie-de-contexte/311-tokens-contexte.md) : plus les conventions sont explicites dans le contexte, moins le modèle hallucine.

### Comparaison multi-agents : Copilot, Claude Code, Cursor

Le concept de plugin n'est pas propre à Copilot. Claude Code et Cursor proposent des mécanismes similaires avec des différences notables :

| Aspect | Copilot | Claude Code | Cursor |
|---|---|---|---|
| Manifeste | `.github/plugin/plugin.json` | `.claude-plugin/plugin.json` | `.cursor-plugin/plugin.json` |
| Marketplace | `github/awesome-copilot` | `anthropics/claude-plugins-official` + communauté | Pas de marketplace centralisé |
| Installation | VS Code UI / `installed.json` | `/plugin install <name>@marketplace` | `.cursor-plugin` local |
| Scope | user / project | user / project / local | project |
| Contenu | skills, agents | skills, agents, hooks, MCP servers, LSP | rules, skills |

Les différences clés à retenir :

- **Copilot** s'intègre dans VS Code avec une gestion UI et un marketplace GitHub centralisé.
- **Claude Code** propose le scope le plus riche (hooks, MCP servers, LSP) et une installation en CLI.
- **Cursor** reste le plus simple — pas de marketplace, tout est local au projet.

Si tu travailles avec plusieurs agents, cette grille te permet de transposer tes connaissances d'un outil à l'autre. Les concepts sont les mêmes — seuls le format du manifeste et le mécanisme d'installation changent.

### Sécurité : auditer avant d'installer

Un plugin n'est pas un paquet inerte. Contrairement à un serveur MCP qui s'exécute dans un processus externe avec ses propres permissions, un plugin opère **dans le contexte de l'agent**. Les skills et agents qu'il contient s'exécutent avec les mêmes permissions que tes propres primitives — y compris la lecture et l'écriture de fichiers, l'exécution de commandes en terminal, et l'accès à tes dépôts GitHub.

Avant d'installer un plugin, applique ces vérifications :

1. **Lis le code source** — Consulte le dépôt Git du plugin. Lis chaque `SKILL.md` et `.agent.md`. Vérifie qu'ils ne contiennent pas d'instructions suspectes (exfiltration de données, exécution de scripts arbitraires, accès réseau non justifié).
2. **Vérifie l'auteur** — Un plugin publié par `github/` ou une organisation connue inspire plus de confiance qu'un dépôt anonyme sans historique.
3. **Inspecte les outils déclarés** — Un skill de documentation qui demande `runInTerminal` est suspect. Les outils doivent correspondre à la fonction annoncée.
4. **Préfère les versions taguées** — Un tag `v1.0.0` est plus sûr qu'un pointeur vers `main` qui peut changer à tout moment.

### Créer son propre plugin

Tu as déjà des skills et des agents dans ton dépôt ? Les transformer en plugin distribuable ne demande qu'un fichier supplémentaire : le manifeste `plugin.json`.

La structure minimale :

```text
mon-plugin/
├── .github/
│   └── plugin/
│       └── plugin.json
├── agents/
│   └── mon-agent.agent.md
└── skills/
    └── mon-skill/
        └── SKILL.md
```

Le manifeste correspondant :

```json
{
  "name": "mon-plugin",
  "description": "Description concise de ce que fait le plugin.",
  "version": "1.0.0",
  "keywords": ["mot-cle-1", "mot-cle-2"],
  "agents": ["./agents"],
  "skills": ["./skills/mon-skill"]
}
```

Pousse le dépôt sur GitHub — ton plugin est installable. Un collègue peut l'installer directement depuis le marketplace ou via la palette de commandes VS Code en pointant vers ton dépôt.

## Démonstration

### Étape 1 — Explorer le marketplace

Rends-toi sur le dépôt `github/awesome-copilot`. Parcours la liste des plugins disponibles. Repère un plugin correspondant à ton stack technique — par exemple `java-development` si tu travailles en Java, ou `testing-automation` si tu veux enrichir ton workflow de tests.

### Étape 2 — Installer un plugin depuis VS Code

Ouvre VS Code. Utilise la palette de commandes (`Cmd+Shift+P` sur macOS) et cherche la commande d'installation de plugin Copilot. Sélectionne le plugin que tu as repéré à l'étape précédente.

Après installation, vérifie que le plugin est bien présent :

```bash
ls ~/.vscode-insiders/agent-plugins/github.com/
```

Tu dois voir un dossier correspondant à `<owner>/<repo>` du plugin installé.

### Étape 3 — Utiliser le plugin

Ouvre Copilot Chat en mode Agent. Formule une requête qui devrait déclencher l'un des skills du plugin installé. Par exemple, si tu as installé `testing-automation`, demande à Copilot de générer une suite de tests pour un fichier de ton projet. Le routeur sémantique doit activer le skill correspondant du plugin.

### Étape 4 — Créer un plugin minimal

Crée un nouveau dépôt pour ton plugin :

```bash
mkdir mon-premier-plugin && cd mon-premier-plugin
git init
```

Crée la structure du plugin :

```bash
mkdir -p .github/plugin agents skills/greeting
```

Crée le manifeste `.github/plugin/plugin.json` :

```json
{
  "name": "mon-premier-plugin",
  "description": "Plugin minimal avec un skill et un agent.",
  "version": "1.0.0",
  "keywords": ["demo", "tutorial"],
  "agents": ["./agents"],
  "skills": ["./skills/greeting"]
}
```

Crée un skill `skills/greeting/SKILL.md` :

```markdown
# greeting

## Description

Use when the user asks to greet someone or generate a welcome message.

## Procedure

1. Ask the user for the person's name if not provided.
2. Generate a warm, professional greeting message.
3. Propose the message in both French and English.
```

Crée un agent `agents/greeter.agent.md` :

```markdown
---
name: greeter
description: "Génère des messages d'accueil personnalisés."
tools:
  - editFiles
---

Tu es un rédacteur spécialisé dans les messages d'accueil.
Quand on te demande de saluer quelqu'un, utilise le skill greeting
pour produire un message chaleureux et professionnel.
```

Pousse le dépôt sur GitHub :

```bash
git add .
git commit -m "feat: initial plugin with greeting skill and greeter agent"
git remote add origin git@github.com:<ton-user>/mon-premier-plugin.git
git push -u origin main
```

Un collègue peut maintenant installer ton plugin depuis VS Code.

## Exercice ⭐⭐

**Énoncé** — Installe un plugin depuis `awesome-copilot`, utilise-le sur un cas concret, puis crée ton propre plugin minimal contenant un skill et un agent.

**Partie 1 — Installer et utiliser un plugin existant** :

1. Explore le marketplace `github/awesome-copilot` et choisis un plugin adapté à un projet sur lequel tu travailles.
2. Installe le plugin depuis la palette de commandes VS Code.
3. Vérifie que le plugin apparaît dans `~/.vscode-insiders/agent-plugins/`.
4. Ouvre Copilot Chat en mode Agent et déclenche un skill du plugin avec une requête adaptée.
5. Vérifie que le skill s'active et produit un résultat pertinent.

**Partie 2 — Créer ton propre plugin** :

1. Crée un dépôt Git avec la structure `plugin.json` + un skill + un agent.
2. Le skill doit couvrir un besoin réel de ton quotidien (convention de nommage, template de PR, checklist de review, etc.).
3. L'agent doit utiliser le skill dans son workflow.
4. Pousse le dépôt sur GitHub.
5. Demande à un collègue d'installer ton plugin et de vérifier que le skill se déclenche correctement.

**Critère de réussite** : le plugin installé fonctionne dans ton workspace, et ton propre plugin est installable et fonctionnel chez un collègue.

## Validation

Tu peux passer au module suivant si :

- [ ] Tu as installé au moins un plugin depuis le marketplace `github/awesome-copilot`.
- [ ] Le plugin installé apparaît dans `~/.vscode-insiders/agent-plugins/github.com/<owner>/<repo>/`.
- [ ] Un skill du plugin installé se déclenche correctement quand tu formules une requête adaptée.
- [ ] Tu as créé un plugin minimal avec un `plugin.json`, un skill et un agent.
- [ ] Ton plugin est poussé sur GitHub et installable par un tiers.
- [ ] Tu sais expliquer la différence entre un plugin installé en scope `user` et en scope `project`.
- [ ] Tu sais quels points vérifier avant d'installer un plugin tiers (audit de sécurité).

## Pour aller plus loin

- [Module 315 — Evals](../03-ingenierie-de-contexte/315-evals.md) : tester automatiquement que les skills apportés par un plugin fonctionnent comme attendu.
- [Module 311 — Tokens, hallucinations et sobriété LLM](../03-ingenierie-de-contexte/311-tokens-contexte.md) : comprendre comment les plugins `minimal-context-tools` réduisent la consommation de tokens.
- [Module 207 — APM](./207-apm.md) : revoir la distribution de primitives individuelles vs plugins.
- [Module 208 — Workflows](./208-workflows.md) : revoir comment orchestrer les primitives qu'un plugin apporte.
- La documentation du dépôt `github/awesome-copilot` pour découvrir de nouveaux plugins.
