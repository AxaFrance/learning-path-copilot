---
id: app-skills-agents
title: "435 — GitHub Copilot app : skills et agents personnalisés"
sidebar_position: 435
description: "Créer des instructions réutilisables, un agent de relecture en lecture seule et un skill métier pour Mon Sinistre dans la GitHub Copilot app."
---

# 435 — GitHub Copilot app : skills et agents personnalisés

**Durée** : ~50 min · **Complexité** : ⭐⭐ · **Pré-requis** : [434 — Workflows GitHub dans l'app](./434-app-workflows-github.md), [103 — Skills](../01-fondations/103-skills.md), [104 — Agents](../01-fondations/104-agents.md)

> *Quand tu répètes la même consigne de revue, ce n'est plus un prompt : c'est une règle d'atelier. L'app permet de la ranger au bon endroit, puis de confier la lecture à un agent aux outils limités.*

## Objectif

À la fin de ce module, tu sais choisir entre instructions de dépôt, skill et agent personnalisé dans la GitHub Copilot app, puis les combiner pour cadrer une revue métier sur **Mon Sinistre**.

> Source : [copilot-app-for-beginners — Chapter 04 « Skills and Custom Agents »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md) et [Customizing the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/customize-github-copilot-app)

## Ce que tu vas apprendre

1. Placer les règles durables dans `.github/copilot-instructions.md`.
2. Écrire un skill de revue métier réutilisable.
3. Créer un agent personnalisé **lecture seule**.
4. Vérifier que la personnalisation a vraiment été chargée.

## Contenu pédagogique

### Trois niveaux de personnalisation

Dans l'app, les instructions de dépôt s'appliquent à toutes les sessions du projet, un skill donne une méthode pour une tâche répétée, et un agent personnalisé définit un rôle avec ses outils disponibles.

> Source : [copilot-app-for-beginners — Chapter 04 « Choose the Feature That Fits the Task »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md) et [About agent skills](https://docs.github.com/en/copilot/concepts/agents/about-agent-skills)

| Besoin | Support | Exemple Mon Sinistre |
|---|---|---|
| Règles toujours vraies | `.github/copilot-instructions.md` | Montants en centimes, domaine C#/.NET 10, validation par `verify.sh` |
| Checklist répétée | `.github/skills/<nom>/SKILL.md` | Revue métier couverture / indemnisation / délai / statut |
| Rôle spécialisé | `.github/agents/<nom>.agent.md` | Relecteur conformité qui lit et explique sans modifier |

Le point important : un skill guide le raisonnement, mais ne retire pas les droits d'édition de l'agent. Le fichier d'agent, lui, peut déclarer une liste d'outils comme `read` et `search` pour limiter ce rôle.

> Source : [copilot-app-for-beginners — Chapter 04 « A skill tells Copilot how to approach a task »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md) et [Customizing the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/customize-github-copilot-app)

### Le défaut D5 : pas d'instructions projet

Dans `samples/mon-sinistre`, le défaut D5 est volontaire : il n'existe pas encore de `.github/copilot-instructions.md`. Copilot ne reçoit donc pas automatiquement le vocabulaire métier ni la convention de stockage des montants en centimes.

> Source : [README Mon Sinistre — Défauts volontaires](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md#défauts-volontaires)

Une bonne instruction projet reste courte, vérifiable et stable. Elle ne doit pas remplacer un ticket ni raconter toute l'architecture. Pour Mon Sinistre, elle peut rappeler : application pédagogique fictive, domaine assurance auto, montants en centimes (`long`), `verify.sh` comme critère de réussite, et interdiction de corriger les défauts hors exercice.

> Source : [README Mon Sinistre — Règles métier et critère de réussite](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md#règles-métier)

### Un skill de revue métier

Un skill est un dossier contenant un `SKILL.md`. Le front matter décrit son nom et son déclencheur ; le Markdown décrit la méthode de travail. Les skills projet peuvent vivre dans `.github/skills`, et les skills personnels dans le répertoire utilisateur.

> Source : [About agent skills](https://docs.github.com/en/copilot/concepts/agents/about-agent-skills) et [copilot-app-for-beginners — Chapter 04 « Skills: Save a Review Checklist »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md)

Pour Mon Sinistre, un skill utile ne dit pas seulement « relis le code ». Il impose une grille : lire les règles métier, relier chaque observation à un fichier, distinguer bug confirmé et hypothèse, puis demander `./scripts/verify.sh` avant de considérer une correction terminée.

```markdown
---
name: revue-metier-mon-sinistre
description: Relire une modification Mon Sinistre sur les règles métier, les tests et la validation locale.
---

Avant de conclure :
- vérifie les montants en centimes ;
- contrôle couverture, indemnisation, délai de déclaration et transition de statut ;
- cite les fichiers lus ;
- sépare bug confirmé, risque et suggestion ;
- demande `./scripts/verify.sh` depuis `samples/mon-sinistre`.
```

### Un agent « relecteur-conformite » en lecture seule

Un agent personnalisé a un nom, une description, une liste d'outils et des instructions. L'exemple du cours crée un agent explicateur avec seulement `read` et `search`, afin qu'il puisse lire le code sans lancer de commandes ni éditer les fichiers.

> Source : [copilot-app-for-beginners — Chapter 04 « Custom Agents: Define a Role and Its Tools »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md)

Pour Mon Sinistre, le rôle « relecteur-conformite » doit rester une aide de lecture : il inspecte les règles et les tests, signale les écarts, mais ne répare pas lui-même. C'est volontaire : on sépare la relecture de l'implémentation pour éviter qu'un avis de conformité ne devienne une modification non relue.

```markdown
---
name: relecteur-conformite
description: Relire Mon Sinistre sous l'angle conformité métier sans modifier les fichiers.
tools: ["read", "search"]
---

Lis les fichiers pertinents avant de répondre.
Ne modifie aucun fichier, ne lance aucune commande et n'utilise aucun service externe.
Réponds en quatre parties : règle attendue, preuves lues, écarts éventuels, prochaine action humaine.
```

### Vérifier l'usage réel

L'app affiche les activités de skill et l'agent sélectionné. Le cours insiste sur ce point : il faut vérifier l'activité et le résultat, pas seulement croire la phrase « j'ai utilisé le skill ».

> Source : [copilot-app-for-beginners — Chapter 04 « How It Works »](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md)

Le réflexe de validation est simple : recharge les skills avec `/skills reload`, sélectionne l'agent avec `/agent` ou le sélecteur, demande une réponse qui cite les fichiers lus, puis ouvre l'onglet **Changes** pour confirmer qu'un agent lecture seule n'a rien modifié.

## Exercice

**Énoncé** : corrige le défaut D5 pour le projet d'exemple, puis fais relire les règles par un agent lecture seule.

1. Dans une session app sur ton fork, crée `.github/copilot-instructions.md` si le fichier n'existe pas.
2. Ajoute des règles courtes : domaine assurance auto fictif, C#/.NET 10, montants en centimes, validation depuis `samples/mon-sinistre`, pas de correction des défauts hors exercice.
3. Crée `.github/agents/relecteur-conformite.agent.md` avec seulement les outils `read` et `search`.
4. Crée `.github/skills/revue-metier-mon-sinistre/SKILL.md` avec la checklist métier ci-dessus.
5. Lance `/skills reload`, sélectionne `/agent relecteur-conformite`, puis demande :

```text
Utilise la revue métier Mon Sinistre pour relire les règles de délai de déclaration.
Lis les fichiers nécessaires, cite-les, ne modifie rien et ne lance aucune commande.
```

6. Reviens à l'agent par défaut, inspecte le diff, puis lance :

```bash
cd samples/mon-sinistre
./scripts/verify.sh        # Windows : pwsh ./scripts/verify.ps1
```

## Validation

- [ ] `.github/copilot-instructions.md` existe et décrit les conventions stables de Mon Sinistre.
- [ ] Le skill de revue métier est présent sous `.github/skills/` et se recharge avec `/skills reload`.
- [ ] L'agent `relecteur-conformite` déclare uniquement `read` et `search`.
- [ ] La réponse de relecture cite les fichiers consultés et ne prétend pas avoir exécuté les tests.
- [ ] `./scripts/verify.sh` est vert depuis `samples/mon-sinistre`.

## Pour aller plus loin

- [103 — Skills](../01-fondations/103-skills.md) — la primitive réutilisable.
- [104 — Agents](../01-fondations/104-agents.md) — le rôle spécialisé.
- [425 — CLI agents et instructions](./425-cli-agents-instructions.md) — le même défaut D5 côté CLI.
- [436 — MCP et plugins dans l'app](./436-app-mcp-plugins.md) — ajouter des capacités externes.

## Source

- [github/copilot-app-for-beginners — Chapter 04](https://github.com/github/copilot-app-for-beginners/blob/main/04-skills-custom-agents/README.md) — cours GitHub sous licence MIT, adapté en français.
- [Customizing the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/customize-github-copilot-app)
- [About agent skills](https://docs.github.com/en/copilot/concepts/agents/about-agent-skills)
