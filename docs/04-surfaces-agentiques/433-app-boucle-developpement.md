---
id: app-boucle-developpement
title: "433 — GitHub Copilot app : boucle de développement"
sidebar_position: 433
description: "Utiliser diff, terminal, tests, aperçu navigateur, debug et ouverture VS Code pour valider une correction locale avec Mon Sinistre."
---

# 433 — GitHub Copilot app : boucle de développement

**Durée** : ~50 min · **Complexité** : ⭐⭐ · **Pré-requis** : [432 — Sessions, worktrees et contexte](./432-app-sessions-worktrees-contexte.md)

> *Une réponse convaincante ne suffit pas. Dans l'app, tu valides avec le diff, les tests, le terminal et l'aperçu web avant de garder la correction.*

## Objectif

À la fin de ce module, tu sais :

- inspecter le diff d'une session avant acceptation ;
- lancer `dotnet test` et `./scripts/verify.sh` depuis le terminal de session ;
- ouvrir l'aperçu navigateur de `MonSinistre.Web` sur `http://localhost:5213` ;
- utiliser l'app comme boucle debug → correction → validation ;
- ouvrir le worktree dans VS Code si tu veux reprendre la main ;
- corriger D1 et vérifier que `SIN-1004` affiche une indemnisation à 0 dans la page web.

## Ce que tu vas apprendre

1. La boucle locale : comprendre, changer, inspecter, tester, prévisualiser
2. Lire le diff avant de faire confiance
3. Terminal : `dotnet test` et `verify.sh`
4. Browser preview : `MonSinistre.Web`
5. Debug et ouverture dans VS Code
6. Exercice D1

## Contenu pédagogique

### La boucle locale

La boucle de développement dans l'app suit toujours le même rythme :

1. comprendre la tâche ;
2. demander un plan ou une correction ciblée ;
3. inspecter le diff ;
4. lancer les validations ;
5. observer le comportement ;
6. itérer si l'évidence contredit la réponse.

> Source : [copilot-app-for-beginners — 03 Development Workflows](https://github.com/github/copilot-app-for-beginners/blob/main/03-development-workflows/README.md), section « Inner Loop ».

Le point clé : **l'évidence gagne contre le texte de l'agent**. Si Copilot dit « les tests passent » mais que le terminal ne le montre pas, tu n'as pas encore de preuve.

### Diff : la première preuve

Le panneau **Changes** montre les fichiers modifiés. Avant de lancer les tests, vérifie :

- le changement est limité au défaut demandé ;
- aucun fichier généré ou binaire n'est ajouté ;
- les tests ne sont pas affaiblis pour rendre le bug invisible ;
- le vocabulaire métier reste clair : franchise, indemnisation, sinistre, contrat ;
- les montants restent en centimes dans le domaine.

Pour D1, la correction devrait concerner le calcul d'indemnisation et idéalement un test associé. Une modification de l'interface web seule serait suspecte : elle cacherait le symptôme sans corriger la règle métier.

### Terminal : tests ciblés et verify

Dans le panneau de revue, ouvre **Terminal**. Depuis la racine du dépôt :

```bash
cd samples/mon-sinistre
dotnet test MonSinistre.sln
```

Puis lance la validation complète du sample :

```bash
./scripts/verify.sh        # Windows : pwsh ./scripts/verify.ps1
```

`verify.sh` restaure les dépendances, compile avec avertissements traités comme erreurs, puis lance les tests. C'est le critère commun des exercices du bloc.

### Browser preview : MonSinistre.Web

Pour observer l'application web :

```bash
cd samples/mon-sinistre
dotnet run --project src/MonSinistre.Web
```

Le sample écoute sur :

```text
http://localhost:5213
```

Dans l'app, ouvre un onglet **Browser** dans le panneau de revue et saisis cette URL. Tu peux alors chercher `SIN-1004` et comparer l'affichage avant/après correction.

Si deux sessions web tournent en parallèle, le port 5213 peut être occupé. Arrête le serveur avec `Ctrl+C` ou demande à la deuxième session d'utiliser un autre port.

### Debug et reprise en éditeur

L'app suffit pour beaucoup de corrections. Mais tu peux ouvrir le worktree dans VS Code lorsque :

- tu veux poser des points d'arrêt ;
- tu veux naviguer plus finement dans les symboles C# ;
- tu veux utiliser les outils .NET habituels ;
- tu veux relire plusieurs fichiers côte à côte.

Depuis une session, utilise l'action d'ouverture dans l'éditeur si elle est proposée, ou copie le chemin du worktree depuis les détails de session et ouvre ce dossier dans VS Code. Vérifie toujours que VS Code affiche la branche de session, pas `main`.

:::tip Design system AXA optionnel
L'interface `MonSinistre.Web` utilise volontairement un CSS minimal. Une équipe AXA peut brancher le design system présenté au [module 209 — Plugins](../02-composition/209-plugins.md), mais ce n'est pas nécessaire pour corriger D1. Ne mélange pas correction métier et refonte graphique dans le même diff.
:::

## Exercice

**Énoncé** : corrige D1 et observe `SIN-1004` avec une indemnisation à 0 dans la page web.

### 1. Créer la session

1. Démarre une session **New worktree**.
2. Mets le mode sur **Plan**.
3. Envoie :

```text
Corrige le défaut D1 dans @samples/mon-sinistre.
Le sinistre SIN-1004 a un montant estimé inférieur à la franchise : l'indemnisation doit être 0, jamais négative.
Commence par analyser la règle métier, les fichiers à changer et les validations à lancer.
Ne modifie rien avant d'avoir proposé le plan.
```

4. Lis le plan. Il doit viser la règle métier, pas seulement l'affichage.
5. Demande l'implémentation :

```text
Implémente le plan avec le plus petit changement utile.
Ajoute ou ajuste un test qui prouve qu'une indemnisation ne descend jamais sous 0.
```

### 2. Inspecter le diff

Dans **Changes**, vérifie que :

- la règle d'indemnisation applique un plancher à 0 ;
- le test couvre un montant estimé inférieur à la franchise ;
- aucun autre défaut volontaire n'est corrigé par accident.

### 3. Valider en terminal

Dans **Terminal** :

```bash
cd samples/mon-sinistre
dotnet test MonSinistre.sln
./scripts/verify.sh
```

Les deux commandes doivent réussir.

### 4. Valider dans le navigateur

Lance le web :

```bash
dotnet run --project src/MonSinistre.Web
```

Dans **Browser**, ouvre :

```text
http://localhost:5213
```

Cherche ou ouvre le dossier `SIN-1004`. L'indemnisation doit être **0,00 €** ou équivalent, pas `-80,00 €`.

Arrête le serveur avec `Ctrl+C` quand tu as terminé.

## Validation

- [ ] Le diff corrige la règle métier, pas seulement l'affichage.
- [ ] Un test couvre le plancher à 0.
- [ ] `dotnet test MonSinistre.sln` réussit.
- [ ] `./scripts/verify.sh` réussit.
- [ ] La page web sur `http://localhost:5213` montre `SIN-1004` avec une indemnisation à 0.
- [ ] Le serveur web a été arrêté après validation.

## Pour aller plus loin

- [434 — Workflows GitHub](./434-app-workflows-github.md)
- [Mon Sinistre — README](../../samples/mon-sinistre/README.md)
- [GitHub Docs — Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions)

## Sources

- [github/copilot-app-for-beginners — 03 Development Workflows](https://github.com/github/copilot-app-for-beginners/blob/main/03-development-workflows/README.md)
- [samples/mon-sinistre/README.md](../../samples/mon-sinistre/README.md)
