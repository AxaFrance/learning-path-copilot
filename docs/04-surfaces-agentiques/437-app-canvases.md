---
id: app-canvases
title: "437 — GitHub Copilot app : canvases et preuves visibles"
sidebar_position: 437
description: "Utiliser les canvases de la GitHub Copilot app pour garder visibles le plan, le progrès et les preuves d'un correctif Mon Sinistre."
---

# 437 — GitHub Copilot app : canvases et preuves visibles

**Durée** : ~60 min · **Complexité** : ⭐⭐⭐ · **Pré-requis** : [436 — MCP et plugins](./436-app-mcp-plugins.md)

> *Un long chat raconte ce qui s'est passé. Un canvas montre où tu en es, ce qui est prouvé, et ce qui reste à décider.*

## Objectif

À la fin de ce module, tu sais créer un canvas de suivi avec `/create-canvas`, y faire apparaître un plan, un progrès et des preuves, puis l'utiliser pour piloter le correctif D2 de Mon Sinistre.

## Ce que tu vas apprendre

1. Quand un canvas vaut mieux qu'un fil de chat.
2. Comment créer un canvas avec `/create-canvas`.
3. Comment structurer un tableau plan / progrès / preuves.
4. Comment éviter de cocher sans preuve.

## Contenu pédagogique

### Le canvas comme surface partagée

Un canvas est une surface interactive dans le panneau latéral de l'app. Il est bidirectionnel : l'agent peut le mettre à jour pendant son travail, et tu peux modifier la même surface pour guider la suite.

Le cours conseille d'utiliser le chat pour une question courte, et un canvas quand le travail a de l'état visible, des étapes répétées ou des contrôles partagés entre humain et agent.

Dans Mon Sinistre, le défaut D2 est un bon cas : il faut comprendre la règle, écrire ou compléter des tests, corriger le calcul, vérifier la CLI et garder les preuves. Si tout reste dans le chat, le risque est de perdre le lien entre décision, test et résultat.

### Canvases communautaires et création locale

Le cours présente d'abord un canvas communautaire installé via plugin, puis la création d'un canvas avec `/create-canvas`. Certains canvases viennent de plugins ; d'autres peuvent être générés pour un workflow personnel.

Quand tu crées un canvas, tu peux choisir un périmètre utilisateur ou projet. Le cours privilégie le périmètre utilisateur pour les exercices, afin de ne pas committer d'extension expérimentale dans le dépôt.

Pour ce handbook, garde la même discipline : un canvas d'apprentissage peut vivre dans ton espace utilisateur. Le dépôt Mon Sinistre ne doit recevoir que les changements nécessaires au correctif, pas le code du canvas.

### Le tableau plan / progrès / preuves

Le cœur d'un bon canvas n'est pas son apparence. C'est la règle de preuve. Une étape ne devient verte que si elle pointe vers une sortie réelle : test, build, diff, vérification CLI ou note de blocage.

Pour D2, un tableau minimal suffit :

| Zone | Contenu attendu |
|---|---|
| Plan | Lire `DelaiDeclaration.cs`, ajouter le cas vendredi → vendredi, corriger le calcul, vérifier CLI |
| Progrès | À faire, en cours, fait, bloqué |
| Preuves | Test ajouté, `verify.sh`, sortie `show SIN-1004`, fichiers modifiés |

Le défaut cible est précis : `SIN-1004`, du vendredi 2026-09-18 au vendredi 2026-09-25, doit être considéré dans le délai de 5 jours ouvrés, mais le code initial compte les jours calendaires.

### Un prompt de création de canvas

Voici un prompt de départ à coller après `/create-canvas` :

```text
Crée un canvas utilisateur compact pour suivre le correctif Mon Sinistre D2 « délai en jours ouvrés ».

Le canvas contient trois zones :
1. Plan : étapes Lire, Tester, Corriger, Vérifier CLI, Vérifier final.
2. Progrès : statut de chaque étape (à faire, en cours, fait, bloqué).
3. Preuves : lignes courtes avec fichier, commande, résultat et horodatage de session.

Règles :
- ne modifie pas le dépôt pendant la création du canvas ;
- ne coche jamais une étape sans preuve enregistrée ;
- les preuves acceptées sont : diff inspecté, test xUnit, sortie `dotnet run ... show SIN-1004`, `./scripts/verify.sh` vert ;
- garde le canvas en user scope, pas dans `.github/extensions`.
```

Cette demande suit la logique du Feature Workbench du cours : proposer, planifier, établir une base, implémenter, valider, puis afficher les preuves.

### Reprendre la main si le canvas dérive

Les canvases générés peuvent être imparfaits. Le cours conseille des prompts de réparation ciblés : ajouter les parties manquantes, ne marquer que depuis les preuves, augmenter les délais des actions, ou continuer avec un fallback Markdown hors dépôt.

Pour Mon Sinistre, une réparation utile est :

```text
Garde le canvas actuel. Les étapes ne doivent passer à fait que si une preuve existe dans la zone Preuves.
Ajoute une preuve obligatoire pour `./scripts/verify.sh` avant de marquer Vérifier final.
Ne modifie pas le dépôt.
```

Si `/create-canvas` n'est pas disponible, garde le même tableau dans un artefact de session, pas dans le dépôt. Le brief du bloc interdit les fichiers de suivi ajoutés au repository pour cet exercice.

## Exercice

**Énoncé** : utiliser un canvas pour piloter le correctif D2 « délai en jours ouvrés ».

1. Lance une session app sur une branche dédiée.
2. Crée le canvas avec le prompt ci-dessus.
3. Demande un plan seulement : lire `DelaiDeclaration.cs` et les tests, proposer les cas de test, ne rien modifier.
4. Enregistre dans le canvas le plan approuvé.
5. Fais implémenter le correctif : ajouter au moins le cas vendredi 2026-09-18 → vendredi 2026-09-25, puis corriger le calcul.
6. Vérifie le comportement observable :

```bash
cd samples/mon-sinistre
dotnet run --project src/MonSinistre.Cli -- show SIN-1004
./scripts/verify.sh        # Windows : pwsh ./scripts/verify.ps1
```

7. Dans le canvas, note les preuves : fichiers modifiés, test ajouté, ligne « Dans les délais : oui », `verify.sh` vert.

## Validation

- [ ] Le canvas contient Plan, Progrès et Preuves.
- [ ] Aucune étape n'est marquée faite sans preuve.
- [ ] Le correctif D2 est couvert par un test xUnit.
- [ ] `show SIN-1004` affiche « Dans les délais : oui » après correction.
- [ ] `./scripts/verify.sh` est vert depuis `samples/mon-sinistre`.

## Pour aller plus loin

- [436 — MCP et plugins](./436-app-mcp-plugins.md) — installer les capacités avec prudence.
- [429 — Atelier CLI bout en bout](./429-cli-atelier-bout-en-bout.md) — autre exemple de boucle plan → code → tests.
- [438 — Automations](./438-app-automations.md) — transformer un prompt répétitif en run enregistré.

## Sources

- [github/copilot-app-for-beginners — Chapter 06](https://github.com/github/copilot-app-for-beginners/blob/main/06-canvases/README.md) — cours GitHub sous licence MIT, adapté en français.
- [Working with canvas extensions in the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/working-with-canvas-extensions)
- [README Mon Sinistre](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
