# Orchestration autonome — tous modules restants

## Modules déjà publiés
- 00-setup-posture → `docs/learning-path/`
- 01-instructions-personnalisees → `docs/learning-path/`

## Modules à produire
- 02-prompts-personnalises
- 03-skills
- 04-agents-personnalises
- 05-apm
- 06-mcp
- 07-autoresearch
- 08-workflows
- 09-evals
- 10-copilot-cli
- 11-tokens-hallucinations

## Situations nécessitant intervention humaine

### 1. Module 02 — Ajout notice de dépréciation `.prompt.md`
- **Contexte** : L'utilisateur a demandé d'ajouter que `.prompt.md` sera déprécié au profit de skills à invocation utilisateur (model invocation disabled).
- **Action prise** : Ajout d'un callout `:::caution` + mise à jour de « Pour aller plus loin ».
- **À décider** : Le catalogue (spec 02) ne contient pas de module dédié aux « skills à invocation utilisateur ». Il faudra soit ajouter un nouveau module au catalogue (entre 03 et 04 ?), soit intégrer ce contenu dans le module 03 (skills). Le lien dans le module 02 pointe vers un module qui n'existe pas encore.

### 2. Module 02 — Forward cross-link `03-skills.md`
- **Contexte** : Le linter échoue (check 9) sur le lien `./03-skills.md` car le module 03 n'existe pas encore.
- **Action prise** : Accepté comme effective PASS (8/9). Le lien se résoudra quand le module 03 sera créé.
- **À vérifier** : Relancer le linter sur 02 une fois 03 publié.

### 3. Module 02 — B10 HUMAN CHECKPOINT (glossaire)
- **Contexte** : Spec 12 §5 impose un checkpoint humain avant de lancer le glossary keeper.
- **Action prise** : Skipé — l'utilisateur a demandé d'enchaîner sans arrêt. Le glossary keeper sera lancé en batch à la fin de tous les modules.

### 4. Module 03 — Ajout section « Créer un skill avec assistance »
- **Contexte** : L'utilisateur a demandé d'étendre le module 03 pour couvrir `writing-skills` (superpowers, approche TDD) et `create-skill` (convention Anthropic) après la construction manuelle.
- **Action prise** : Ajout d'une « Étape 4 » dans la Démonstration couvrant les deux approches + tableau comparatif.
- **À vérifier** : Le linter n'a pas été relancé sur 03 après cet ajout. Le word count a augmenté (~1 800+).

### 5. Module 04 — Ajout section « Créer un agent avec assistance »
- **Contexte** : L'utilisateur a demandé d'ajouter `create-agent` et `agent-customization` (skill natif VS Code) au module 04.
- **Action prise** : Ajout d'une « Étape 5 » dans la Démonstration couvrant les deux approches + tableau comparatif.
- **À vérifier** : Le linter n'a pas été relancé sur 04 après cet ajout.

### 6. Module 05 — Ajout support Azure DevOps
- **Contexte** : L'utilisateur veut montrer que APM fonctionne aussi avec `dev.azure.com`, pas seulement GitHub.
- **Action prise** : Ajout du flag `--provider azure-devops` et exemple dans la section convention de nommage.
- **À vérifier** : Vérifier que le flag `--provider` existe réellement dans la CLI APM.

### 7. Module 06 — Ajout avertissement cybersécurité MCP
- **Contexte** : L'utilisateur veut un avertissement fort sur les risques de sécurité des serveurs MCP + une section CLI vs MCP pour le coût.
- **Action prise** : Ajout d'un callout `:::danger` après les objectifs + section « CLI natif vs MCP : le vrai coût » avec tableau comparatif tokens/sécurité/latence.
- **À vérifier** : Relinter les modules 05 et 06 après ces ajouts.

### 8. Module 07 — Correction autoresearch d'après source de vérité
- **Contexte** : L'utilisateur a fourni le vrai skill `autoresearch-skill` de Karpathy. Le module avait plusieurs erreurs (processus manuel vs boucle autonome, 10 evals vs 3-6, 100% x2 vs 95% x3, format changelog, absence de dashboard).
- **Action prise** : Réécriture des sections Concepts clés (cycle, evals, changelog, critère d'arrêt, structure artefacts) pour aligner avec la source de vérité.

### 9. Module 08 — Ajout pattern handoff
- **Contexte** : L'utilisateur veut une alternative au pattern Outside-In pour les workflows.
- **Action prise** : Ajout d'une section « Alternative : le pattern handoff » avec diagramme mermaid et tableau comparatif Outside-In vs Handoff.

### 10. Module 10 — Ajout mention IDEs en retard
- **Contexte** : L'utilisateur veut mentionner que les IDE comme IntelliJ/Eclipse sont en retard sur les fonctionnalités Copilot.
- **Action prise** : Ajout dans « Quand le CLI est plus pertinent que l'IDE » + mise à jour de l'accroche.

### 11. Tous modules — B10 HUMAN CHECKPOINT (glossaire)
- **Contexte** : Chaque module aurait dû passer par un checkpoint humain avant le glossary keeper.
- **Action prise** : Tous skippés. Le glossary keeper sera lancé en batch une fois que l'utilisateur aura validé tous les drafts.

### 12. Tous modules — Forward cross-links
- **Contexte** : Les linters échouent (check 9) sur les liens vers des modules pas encore en draft au moment du lint.
- **Action prise** : Accepté comme effective PASS. À relinter en batch une fois tous les drafts créés.

