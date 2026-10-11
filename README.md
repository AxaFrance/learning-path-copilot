# GitHub Copilot — Learning Path

> Guide francophone pour maîtriser GitHub Copilot : prompts, skills, agents, workflows et ingénierie de contexte.

---

## Structure du handbook

### Bloc 01 — Fondations

| # | Module |
|---|--------|
| 100 | Setup & posture |
| 101 | Instructions |
| 102 | Prompts |
| 103 | Skills |
| 104 | Agents personnalisés |
| 105 | Hooks |
| 106 | MCP |
| 107 | Choix de modèles |

### Bloc 02 — Composition

| # | Module |
|---|--------|
| 207 | APM |
| 208 | Workflows |
| 209 | Plugins |
| 210 | Copilot CLI |
| 211 | Pipeline agents handbook |
| 212 | LSP |
| 213 | Distribuer ses primitives |
| 214 | Dynamic workflows |

### Bloc 03 — Ingénierie de contexte

| # | Module |
|---|--------|
| 310 | Evals |
| 311 | Tokens & contexte |
| 312 | Patterns de sobriété |
| 313 | Outils de réduction |
| 314 | Auto-research |
| 316 | Déplacer le contexte entre modèles |
| 317 | Orchestrer des subagents |
| 318 | Mesurer & optimiser sa consommation |
| 319 | Chronicle expérimental — tirer parti de son historique de sessions |

### Bloc 04 — Surfaces agentiques

Bloc autonome : Copilot CLI et GitHub Copilot app en pratique, sur le projet d'exemple [`samples/mon-sinistre`](samples/mon-sinistre/README.md) (C# / .NET 10, données fictives).

| # | Module |
|---|--------|
| 420 | Choisir sa surface Copilot |
| 421 | Copilot CLI — installer et se connecter |
| 422 | Copilot CLI — modes et slash commands |
| 423 | Copilot CLI — contexte et conversations |
| 424 | Copilot CLI — workflows de dev |
| 425 | Copilot CLI — agents personnalisés et instructions |
| 426 | Copilot CLI — skills |
| 427 | Copilot CLI — serveurs MCP |
| 428 | Copilot CLI — mode non interactif |
| 429 | Copilot CLI — atelier de bout en bout |
| 430 | Copilot app — installer et faire le tour |
| 431 | Copilot app — modes, modèles et permissions |
| 432 | Copilot app — sessions, worktrees et contexte |
| 433 | Copilot app — boucle de développement |
| 434 | Copilot app — workflows GitHub |
| 435 | Copilot app — skills et agents personnalisés |
| 436 | Copilot app — MCP et plugins |
| 437 | Copilot app — canvases |
| 438 | Copilot app — automations |

---

## Développement local

**Prérequis** : Ruby 3.3+ (via Homebrew recommandé)

```bash
export PATH="/opt/homebrew/opt/ruby/bin:$PATH"
bundle install
bundle exec jekyll serve --port 4000
```

Le site est disponible sur <http://localhost:4000/learning-path-copilot/>.

---

## Déploiement

Le workflow [`.github/workflows/deploy-pages.yml`](.github/workflows/deploy-pages.yml) déploie automatiquement sur GitHub Pages à chaque push sur `main` touchant `docs/`, `_config.yml`, `index.md` ou `Gemfile`.

---

## Contribution

Les modules sont dans `docs/<bloc>/`. Chaque fichier Markdown suit la convention `NNN-slug.md`.

Les projets d'exemple sont dans `samples/` (exclus du site Jekyll). Avant de publier une modification de `samples/mon-sinistre`, lance `./samples/mon-sinistre/scripts/verify.sh` (ou `verify.ps1`).

### Agents dédiés (`.github/agents/`)

Le pipeline de rédaction est entièrement piloté par des agents Copilot. Activez-les depuis le chat GitHub Copilot avec le mode **Agent**.

| Agent | Rôle | Exemple de déclencheur |
|---|---|---|
| `handbook-module-orchestrator` | Pilote le pipeline complet : writer → linter → glossary keeper, avec boucle d'alignement (max 3 rounds) | _"produis le module 104 de bout en bout"_ |
| `handbook-module-writer` | Rédige un module à partir du catalogue (spec 02) et du template (spec 01) | _"rédige le module skills"_ |
| `handbook-linter` | Vérifie la conformité d'un module aux conventions éditoriales (spec 01 + spec 11) | _"lint module 103"_ |
| `handbook-glossary-keeper` | Maintient `docs/ressources/glossaire-fr.md` après publication d'un module | _"mets à jour le glossaire"_ |
| `handbook-chapter-orchestrator` | Pipeline end-to-end pour un **nouveau chapitre** hors catalogue : placement → writer → reviewer | _"construis un chapitre sur les evals"_ |
| `handbook-chapter-architect` | Détermine où insérer un nouveau chapitre dans la taxonomie | _"où placer un chapitre sur X"_ |
| `handbook-chapter-writer` | Rédige un chapitre avec sources vérifiées (jamais depuis la mémoire d'entraînement) | _"rédige le chapitre sur X avec sources"_ |
| `handbook-chapter-reviewer` | Fact-check chaque affirmation du draft, émet un verdict CERTIFY / REVISE / REJECT | _"valide module Z avant publication"_ |
| `handbook-eval-builder` | Génère les scénarios d'eval (triggers + content) pour un agent ou skill | _"génère les evals pour l'agent X"_ |

**Point d'entrée recommandé** : utilisez toujours `handbook-module-orchestrator` (pour les modules du catalogue) ou `handbook-chapter-orchestrator` (pour tout nouveau sujet). Les autres agents sont invoqués automatiquement par l'orchestrateur.

Les artefacts de suivi (plans, rounds de review) sont persistés dans `.spec-handbook-copilot/runtime/<slug>/`.
