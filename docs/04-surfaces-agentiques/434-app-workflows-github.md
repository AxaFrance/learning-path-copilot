---
id: app-workflows-github
title: "434 — GitHub Copilot app : workflows GitHub"
sidebar_position: 434
description: "Démarrer des sessions depuis issues et PR, répondre aux commentaires de revue et traiter une PR qui casse verify.sh en local."
---

# 434 — GitHub Copilot app : workflows GitHub

**Durée** : ~60 min · **Complexité** : ⭐⭐⭐ · **Pré-requis** : [433 — Boucle de développement](./433-app-boucle-developpement.md), GitHub CLI authentifié

> *La boucle externe commence sur GitHub : issue, PR, commentaire, validation, décision humaine. L'app te permet de revenir au code sans perdre le contexte.*

## Objectif

À la fin de ce module, tu sais :

- préparer les issues et PR d'entraînement avec `seed.sh` dans ton fork ;
- démarrer une session depuis une issue ;
- démarrer une session depuis une pull request ;
- traiter un commentaire de revue ;
- analyser une PR qui casse `verify.sh` en local ;
- comprendre pourquoi ce sample n'a pas de CI fournie ;
- expliquer comment une équipe peut ajouter son propre workflow GitHub Actions.

## Ce que tu vas apprendre

1. Préparer le terrain avec `seed.sh`
2. Issues : partir du besoin
3. Pull requests : relire, corriger, répondre
4. Check en échec dans ce sample : `verify.sh` local
5. Les trois PR d'entraînement, dont D4
6. Ajouter un workflow d'équipe si nécessaire

## Prérequis : seed de ton fork

Ce module utilise les éléments créés par :

```bash
./samples/mon-sinistre/scripts/seed.sh
```

Lance ce script **depuis la racine de ton fork**. Il nécessite `gh` authentifié et refuse de continuer si ton dépôt local contient des modifications non commitées.

Le script crée :

- 3 issues avec le label `mon-sinistre` ;
- 3 pull requests d'entraînement ;
- des branches dédiées pour éviter de toucher ta branche par défaut.

## Contenu pédagogique

### Issue → session

Une issue est une bonne source de contexte parce qu'elle décrit le problème du point de vue métier. Dans l'app :

1. ouvre **Issues** ;
2. filtre sur ton fork ;
3. ouvre une issue `mon-sinistre` ;
4. lance **New session** ou attache l'issue avec `#` dans une session ;
5. demande un plan avant modification.

Exemple de prompt :

```text
Utilise l'issue attachée comme source de vérité.
Analyse @samples/mon-sinistre, propose le plus petit plan de correction et les validations à lancer.
Ne modifie rien avant le plan.
```

### PR → session

Une pull request contient déjà un diff. Le travail de l'agent n'est plus seulement « produire du code », mais **relire une proposition** :

- comprendre l'intention de la PR ;
- lire les fichiers modifiés ;
- vérifier les commentaires ;
- lancer les validations ;
- proposer un correctif ciblé si nécessaire.

Dans **Pull requests**, filtre sur ton fork, ouvre une PR d'entraînement, puis lance une session depuis la PR. L'app transporte le contexte GitHub : titre, description, branche, commentaires et état de validation disponible.

### Commentaires de revue

Un commentaire de revue doit être traité comme une exigence précise, pas comme une suggestion vague. Demande à Copilot :

```text
Traite le commentaire de revue attaché avec le plus petit changement utile.
Explique le diff prévu et la validation avant de modifier.
```

Après correction, relis toi-même le diff et la réponse au commentaire. Ne laisse pas l'agent marquer « résolu » si tu n'as pas vérifié que le point est réellement traité.

### Check en échec : adaptation Mon Sinistre

Le cours GitHub d'origine utilise des checks CI. Ici, `samples/mon-sinistre` est volontairement un sample **sans workflow GitHub Actions**. Le signal d'échec est donc :

```bash
cd samples/mon-sinistre
./scripts/verify.sh
```

Dans ce module, quand une PR est dite « en échec », lis-le comme : **la PR casse `verify.sh` en local**. L'agent doit analyser la PR, lancer le script dans le worktree de session, comprendre l'échec et proposer un correctif sans affaiblir les tests.

### Les trois PR d'entraînement

Le script de seed crée trois scénarios.

| PR | Intention | Travail attendu |
|---|---|---|
| Clarifier le bandeau de la page web | Changement texte prêt à merger | Relire le diff et vérifier que `verify.sh` reste vert |
| Ajuster le test de franchise | PR qui casse volontairement `verify.sh` | Trouver pourquoi le test est devenu faux et corriger sans masquer le métier |
| Ajouter un utilitaire de formatage | PR avec commentaire de revue lié à D4 | Traiter la duplication de formatage au lieu d'ajouter une troisième version |

D4 est le défaut de duplication : le formatage des montants existe déjà dans le domaine et dans le CLI. La PR d'entraînement ajoute encore une fonction. Le bon réflexe est de demander une **factorisation** ou une suppression ciblée de la duplication, pas d'empiler un utilitaire de plus.

:::tip Workflow d'équipe optionnel
Le sample n'inclut pas de CI pour rester léger. Une équipe qui réutilise ce projet peut ajouter son propre workflow GitHub Actions, par exemple un job qui lance `./samples/mon-sinistre/scripts/verify.sh` sur chaque PR. Dans ce handbook, tu simules ce check localement pour garder l'exercice autonome.
:::

## Exercice

**Énoncé** : utilise l'app pour traiter les 3 PR d'entraînement créées dans ton fork.

### 0. Préparation

Depuis la racine de ton fork :

```bash
./samples/mon-sinistre/scripts/seed.sh
```

Puis ouvre la GitHub Copilot app, va dans **Pull requests**, filtre sur ton fork et repère les trois PR avec le label `mon-sinistre`.

### PR 1 — prête à merger

1. Ouvre la PR de clarification du bandeau web.
2. Lance une session depuis la PR.
3. Demande :

```text
Relis cette PR d'entraînement.
Explique ce qui change, vérifie que le diff est limité au texte attendu,
puis indique la commande locale à lancer pour valider.
```

4. Dans le terminal de session :

```bash
cd samples/mon-sinistre
./scripts/verify.sh
```

5. La PR est acceptable si le diff est bien limité et si `verify.sh` passe.

### PR 2 — verify.sh cassé

1. Ouvre la PR qui ajuste le test de franchise.
2. Lance une session depuis la PR.
3. Demande :

```text
Analyse cette PR : elle casse ./samples/mon-sinistre/scripts/verify.sh en local.
Lance la validation, explique l'échec, identifie si le test ou le code métier est incorrect,
et propose le plus petit correctif sans affaiblir la règle métier.
```

4. Vérifie que Copilot ne modifie pas le code métier pour satisfaire une attente de test fausse.
5. Relance :

```bash
cd samples/mon-sinistre
./scripts/verify.sh
```

### PR 3 — commentaire de revue et D4

1. Ouvre la PR d'ajout d'utilitaire de formatage.
2. Lis le commentaire de revue : il signale qu'il existe déjà du formatage des montants.
3. Lance une session depuis la PR.
4. Demande :

```text
Traite le commentaire de revue sur cette PR.
Le sujet est D4 : éviter une troisième implémentation de formatage des montants.
Compare le domaine et le CLI, propose une correction ciblée, puis valide avec verify.sh.
```

5. Inspecte le diff : il doit réduire ou éviter la duplication, pas l'aggraver.
6. Relance `./scripts/verify.sh`.

## Validation

- [ ] `seed.sh` a créé les issues et PR dans ton fork.
- [ ] Tu sais filtrer **Issues** et **Pull requests** sur ton dépôt.
- [ ] PR 1 : le diff est relu et `verify.sh` passe.
- [ ] PR 2 : l'échec local est expliqué et corrigé sans affaiblir le métier.
- [ ] PR 3 : le commentaire de revue D4 est traité et la duplication n'augmente pas.
- [ ] Tu sais expliquer pourquoi ce sample parle de `verify.sh` local plutôt que de CI.

## Pour aller plus loin

- [GitHub Docs — Managing issues and pull requests with the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/managing-issues-and-pull-requests)
- [GitHub Actions — Quickstart](https://docs.github.com/en/actions/writing-workflows/quickstart)
- [Mon Sinistre — README](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)

## Sources

- [github/copilot-app-for-beginners — 03 Development Workflows](https://github.com/github/copilot-app-for-beginners/blob/main/03-development-workflows/README.md)
- [`samples/mon-sinistre/README.md`](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
- [`samples/mon-sinistre/scripts/seed.sh`](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/scripts/seed.sh)
