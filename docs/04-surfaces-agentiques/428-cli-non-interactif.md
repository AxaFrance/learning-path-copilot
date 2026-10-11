---
id: cli-non-interactif
title: "428 — Copilot CLI : mode non interactif et garde-fous"
sidebar_position: 428
description: "Utiliser `copilot -p` dans un script local, encadrer les permissions d'outils et expliquer une ouverture CI sans fournir de workflow complet."
---

# 428 — Copilot CLI : mode non interactif et garde-fous

**Durée** : ~35 min · **Complexité** : ⭐⭐ · **Pré-requis** : [427 — MCP CLI](./427-cli-mcp.md)

> *Le mode non interactif est pratique parce qu'il sort du chat. Il est risqué pour la même raison : si tu ne peux pas approuver à la main, tu dois borner ce que Copilot peut lire, exécuter et modifier.*

## Objectif

À la fin de ce module, tu sais utiliser `copilot -p` pour une tâche ponctuelle, choisir des permissions d'outils adaptées, écrire un petit script local qui appelle Copilot puis `verify.sh`, et expliquer comment une équipe pourrait l'intégrer dans sa CI sans copier un workflow YAML complet.

> Source : [copilot-cli-for-beginners — Chapter 01: First Steps, « Programmatic Mode »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md) et [GitHub Copilot CLI](https://docs.github.com/copilot/concepts/agents/about-copilot-cli)

## Ce que tu vas apprendre

1. Quand choisir le mode programmatique
2. Comment fonctionne `copilot -p` / `--prompt`
3. Pourquoi les permissions automatiques exigent des garde-fous
4. Comment chaîner Copilot et `verify.sh` localement
5. Pourquoi ce module n'embarque pas de workflow GitHub Actions complet

## Contenu pédagogique

### `copilot -p` : une réponse puis sortie

Copilot CLI propose une interface interactive et une interface programmatique. Avec `-p` ou `--prompt`, tu donnes une seule consigne en ligne de commande ; le CLI traite la demande, écrit sa sortie, puis termine.

> Source : [GitHub Copilot CLI — Programmatic interface](https://docs.github.com/copilot/concepts/agents/about-copilot-cli) et [copilot-cli-for-beginners — Chapter 01, « Mode 3: Programmatic Mode »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md)

```bash
copilot -p "Résume les règles métier de @samples/mon-sinistre/README.md en 5 puces."
```

Ce mode est utile pour une vérification rapide, une synthèse, un script local ou une automatisation bornée. Il est moins adapté aux tâches floues qui nécessitent des clarifications, une revue de plan ou plusieurs décisions humaines.

> Source : [copilot-cli-for-beginners — Chapter 01, « Which Mode Should I Start With? »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md)

### Permissions : ne pas donner plus que nécessaire

En mode interactif, Copilot peut demander confirmation avant de lire, exécuter ou modifier. En mode programmatique, il n'y a pas toujours un humain disponible pour approuver ; la documentation recommande donc d'utiliser des options d'approbation explicites quand tu veux autoriser des outils.

> Source : [GitHub Copilot CLI — Programmatic interface](https://docs.github.com/copilot/concepts/agents/about-copilot-cli)

Le cours montre `--allow-all` comme solution pratique pour scripts, mais avertit que cette option saute les demandes de permission. La documentation GitHub ajoute qu'une option d'approbation automatique donne à Copilot le même accès que toi aux fichiers et commandes autorisés par ton environnement.

> Source : [copilot-cli-for-beginners — Chapter 01, « About --allow-all »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md) et [GitHub Copilot CLI — Security considerations](https://docs.github.com/copilot/concepts/agents/about-copilot-cli)

Préfère donc une règle simple : `--allow-all` uniquement dans un dépôt de confiance, sur un prompt écrit par toi, avec une portée courte, jamais avec un texte non fiable provenant d'une issue, d'une PR ou d'un ticket externe.

> Source : [copilot-cli-for-beginners — Chapter 01, « About --allow-all »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md)

### Script local : Copilot puis validation

Pour Mon Sinistre, l'automatisation de base ne doit pas pousser, merger ou publier. Elle peut demander une revue ciblée, écrire un rapport dans la sortie standard, puis lancer la validation locale.

> Source : [Mon Sinistre — README, « Critère de réussite des exercices »](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)

Exemple de script à créer dans ton fork, par exemple `scripts/revue-copilot-locale.sh` à la racine de ton fork :

```bash
#!/usr/bin/env bash
set -euo pipefail

PROMPT="""
Relis @samples/mon-sinistre/src/MonSinistre.Domain et @samples/mon-sinistre/tests.
Signale uniquement les écarts probables aux règles métier documentées.
Ne modifie aucun fichier. Ne crée pas de commit. Réponds en français avec : constat, fichier, preuve, test conseillé.
"""

copilot --allow-all -p "$PROMPT"

cd samples/mon-sinistre
./scripts/verify.sh
```

> Source : [copilot-cli-for-beginners — Chapter 01, « Going Further: Using Programmatic Mode in Scripts »](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md)

Même si le prompt demande de ne rien modifier, inspecte toujours `git status` après l'exécution. Un script local sûr doit rendre visibles les preuves : sortie Copilot, statut Git, puis résultat de `verify.sh`.

> Source : [copilot-app-for-beginners — Troubleshooting Reference, « General rule »](https://github.com/github/copilot-app-for-beginners/blob/main/appendices/troubleshooting-reference.md)

### Ouverture CI : explication seulement

Le cours CLI contient un appendice qui montre des workflows GitHub Actions pour automatiser des revues Copilot sur pull request. Dans ce handbook, le projet Mon Sinistre est volontairement un sample sans CI fournie : tu expliques l'idée, mais tu ne fournis pas de workflow YAML complet.

> Source : [copilot-cli-for-beginners — Appendix: CI/CD Integration](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/ci-cd-integration.md) et [Mon Sinistre — README, « Pas de CI fournie »](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)

Une équipe peut ajouter son propre workflow plus tard : checkout du dépôt, installation du SDK .NET, installation éventuelle du CLI, authentification conforme à sa politique, appel borné à Copilot, puis `./samples/mon-sinistre/scripts/verify.sh`. Mais ce module ne doit pas livrer ce YAML, car les permissions, secrets, runners et règles de sécurité dépendent de l'organisation.

> Source : [copilot-cli-for-beginners — Appendix: CI/CD Integration, « Best Practices »](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/ci-cd-integration.md)

:::note Une équipe peut ajouter son propre workflow
Si Mon Sinistre devient un vrai projet, une équipe peut créer son propre workflow GitHub Actions pour lancer `verify.sh` et, si sa politique l'autorise, une revue Copilot bornée. Ici, tu gardes l'exercice local et tu ne copies pas de workflow complet.
:::

### Check-list de garde-fous

| Risque | Garde-fou |
|---|---|
| Prompt issu d'une issue malveillante | Ne pas utiliser `--allow-all` avec du contenu non fiable |
| Modification silencieuse | Demander lecture seule et vérifier `git status` |
| Validation oubliée | Chaîner `./scripts/verify.sh` dans le script |
| Sortie trop large | Limiter les chemins `@samples/mon-sinistre/src/...` |
| CI trop permissive | Définir outils, secrets et permissions dans la politique d'équipe |

> Source : [GitHub Copilot CLI — Programmatic interface](https://docs.github.com/copilot/concepts/agents/about-copilot-cli) et [copilot-cli-for-beginners — Appendix: CI/CD Integration](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/ci-cd-integration.md)

## Exercice

**Énoncé** : dans ton fork, crée un script local qui demande une revue non interactive puis lance la validation. Ne crée pas ce script dans le dépôt du handbook.

1. Depuis la racine de ton fork, crée `scripts/revue-copilot-locale.sh` avec le modèle ci-dessus.
2. Rends-le exécutable :

```bash
chmod +x scripts/revue-copilot-locale.sh
```

3. Lance-le :

```bash
./scripts/revue-copilot-locale.sh
```

4. Vérifie l'état du dépôt :

```bash
git status --short
```

5. Si Copilot signale un défaut, ne corrige rien automatiquement dans cet exercice. Note seulement quel test il faudrait ajouter.
6. Vérifie que `verify.sh` reste vert :

```bash
cd samples/mon-sinistre
./scripts/verify.sh
```

:::warning Prompt non fiable
Ne construis pas un prompt `copilot -p` en injectant directement le corps d'une issue ou d'un commentaire de PR avec `--allow-all`. Résume d'abord, borne la tâche, puis donne seulement les chemins nécessaires.
:::

## Validation

- [ ] Le script local appelle `copilot -p` avec une consigne bornée.
- [ ] Le script demande explicitement de ne modifier aucun fichier.
- [ ] Tu comprends pourquoi `--allow-all` est dangereux hors dépôt de confiance.
- [ ] `git status --short` ne montre pas de modification inattendue.
- [ ] `./scripts/verify.sh` est vert depuis `samples/mon-sinistre`.
- [ ] Tu sais expliquer comment une équipe pourrait intégrer l'idée en CI sans copier de YAML prêt à l'emploi.

## Pour aller plus loin

- [Module 421 — Installer et se connecter](./421-cli-installer-se-connecter.md) — prérequis CLI.
- [Module 427 — MCP CLI](./427-cli-mcp.md) — outils externes en mode interactif.
- [Module 429 — Atelier bout en bout](./429-cli-atelier-bout-en-bout.md) — mobiliser la série complète.

## Source

- [github/copilot-cli-for-beginners — Chapter 01](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md) — cours GitHub sous licence MIT, adapté en français.
- [github/copilot-cli-for-beginners — Appendix: CI/CD Integration](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/ci-cd-integration.md) — patterns CI/CD, adaptés en explication uniquement.
- [GitHub Copilot CLI](https://docs.github.com/copilot/concepts/agents/about-copilot-cli) · [GitHub Copilot CLI command reference](https://docs.github.com/en/copilot/reference/cli-command-reference)
