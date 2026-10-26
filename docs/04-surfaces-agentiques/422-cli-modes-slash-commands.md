---
id: cli-modes-slash-commands
title: "422 — Copilot CLI : modes et slash commands"
sidebar_position: 422
description: "Choisir entre les modes Interactive, Plan et Programmatic, utiliser les slash commands essentielles, changer de modèle et planifier la correction de D3."
---

# 422 — Copilot CLI : modes et slash commands

**Durée** : ~35 min · **Complexité** : ⭐⭐ · **Pré-requis** : [421 — Copilot CLI : installer et se connecter](./421-cli-installer-se-connecter.md)

> *Le CLI devient vraiment utile quand tu sais le piloter : conversation pour explorer, plan pour cadrer, mode programmatique pour automatiser, slash commands pour reprendre la main.*

## Objectif

À la fin de ce module, tu sais :

- choisir entre Interactive, Plan et Programmatic ;
- utiliser les slash commands essentielles par famille ;
- changer de modèle pour la session ou le défaut persistant ;
- demander un plan sûr pour corriger D3 sans modifier le code trop tôt.

## Ce que tu vas apprendre

1. Le mode Interactive
2. Le mode Plan
3. Le mode Programmatic
4. Les slash commands par famille
5. Le choix de modèle
6. Un exercice Plan mode sur D3

## Contenu pédagogique

### Trois modes pour trois intentions

GitHub Copilot CLI propose une interface interactive et une interface programmatique. Dans l'interface interactive, le mode par défaut sert à discuter et agir progressivement ; le mode Plan construit un plan avant d'écrire du code.

Le cours pour débutants présente trois usages : **Interactive** pour explorer et itérer, **Plan** pour cartographier une tâche complexe avant exécution, et **Programmatic** avec `-p` pour une réponse ponctuelle qui sort ensuite.

| Situation dans Mon Sinistre | Mode conseillé | Pourquoi |
|---|---|---|
| Comprendre `Program.cs` | Interactive | Tu poses des questions de suivi. |
| Corriger D3 proprement | Plan | Tu veux relire l'approche avant modification. |
| Obtenir une commande ou un résumé court | Programmatic | Tu veux une réponse sans ouvrir une session longue. |
| Automatiser un contrôle dans un script | Programmatic avec prudence | Le prompt est explicite et borné. |

### Mode Interactive : la conversation de travail

Tu démarres le mode Interactive avec :

```bash
copilot
```

Dans ce mode, chaque message enrichit la conversation : tu peux demander une analyse, répondre à une question, ajouter un fichier avec `@`, demander une reformulation, puis décider si tu acceptes une modification.

Exemple adapté :

```text
@src/MonSinistre.Domain/TransitionStatut.cs
Explique les transitions autorisées et repère celles qui rendent un statut définitif.
```

Le domaine Mon Sinistre définit `Indemnise` et `Refuse` comme des statuts définitifs dans son README métier.

### Mode Plan : penser avant de coder

Le mode Plan aide à produire une approche étape par étape avant toute écriture de code. Le cours indique qu'on peut l'activer avec `/plan`, avec `copilot --plan`, ou en cyclant les modes avec `Shift+Tab`.

Pour une anomalie métier, Plan mode est le bon réflexe : tu demandes à Copilot d'identifier les fichiers, les tests à ajouter, la correction minimale, puis les commandes de validation. Tu ne demandes pas « corrige tout » au premier message.

```text
/plan Corriger D3 dans Mon Sinistre :
- TransitionStatut.EstAutorisee(Refuse, Indemnise) ne doit pas être autorisée.
- Ajoute un test xUnit qui prouve que Refuse est définitif.
- Ne change que les fichiers nécessaires.
- Termine par ./scripts/verify.sh.
```

Le défaut D3 du sample est précisément que `TransitionStatut.EstAutorisee(Refuse, Indemnise)` renvoie `true`.

### Mode Programmatic : une question, une sortie

Le mode programmatique s'utilise avec `-p` ou `--prompt`. La CLI traite la demande puis quitte.

Exemples :

```bash
copilot -p "Explique en une phrase ce que fait src/MonSinistre.Domain/TransitionStatut.cs"
copilot -p "Donne la commande dotnet test pour cette solution"
```

La documentation de démarrage indique aussi `-s` pour ne sortir que la réponse de Copilot, sans informations d'usage additionnelles.

:::caution Prudence avec l'approbation automatique
La documentation avertit que les options d'approbation automatique donnent à Copilot les mêmes accès que toi sur ton ordinateur pour les fichiers et commandes autorisés. Ne les utilise que sur des prompts écrits par toi, dans un dossier de confiance.
:::

### Slash commands essentielles par famille

Les slash commands sont le tableau de bord de la session interactive. Le cours liste les commandes de base et la référence GitHub détaille les commandes disponibles.

| Famille | Commandes à retenir | Usage dans Mon Sinistre |
|---|---|---|
| Aide et sortie | `/help`, `/exit` | Voir les commandes, quitter proprement. |
| Conversation | `/ask`, `/clear`, `/new`, `/compact` | Question rapide, changement de sujet, réduction du contexte. |
| Planification | `/plan`, `Shift+Tab` | Préparer D3 avant modification. |
| Modèle et configuration | `/model`, `/config` | Changer de modèle ponctuellement ou par défaut. |
| Code | `/review`, `/diff`, `/pr`, `/research` | Revue du diff, PR, recherche avant décision. |
| Contexte et session | `/context`, `/session`, `/resume`, `/rename` | Comprendre les tokens, nommer et reprendre une session. |
| Permissions | `/permissions`, `/allow-all`, `/add-dir`, `/list-dirs`, `/yolo` | Contrôler les accès et éviter les dossiers inutiles. |

`/ask` pose une question sans polluer l'historique de la conversation en cours, tandis que `/clear` abandonne la session courante et repart à neuf.

### Changer de modèle

La commande `/model` affiche les modèles disponibles et permet de changer le modèle de la session courante. La commande `/config model` permet de définir un défaut persistant pour les sessions futures.

```text
/model
/config model
```

Le cours précise que les modèles disponibles dépendent de ton abonnement et de ta région, et que `Auto` laisse Copilot choisir le modèle adapté.

Pour Mon Sinistre, garde une règle simple : modèle standard ou `Auto` pour expliquer un fichier ; modèle plus fort seulement si le plan devient transversal, ambigu ou risqué.

## Exercice

**Énoncé** : utilise Plan mode pour préparer la correction de D3, sans appliquer automatiquement le patch.

1. Depuis `samples/mon-sinistre`, vérifie l'état de départ :
   ```bash
   ./scripts/verify.sh
   ```
2. Ouvre Copilot CLI :
   ```bash
   copilot
   ```
3. Demande le plan :
   ```text
   /plan Corriger D3 :
   @src/MonSinistre.Domain/TransitionStatut.cs
   @tests/MonSinistre.Domain.Tests/TransitionStatutTests.cs
   La transition Refuse -> Indemnise doit être interdite.
   Propose un plan minimal avec test xUnit, correction, puis ./scripts/verify.sh.
   ```
4. Vérifie que le plan contient :
   - un test qui prouve que `Refuse` est définitif ;
   - la suppression de la cible `Indemnise` depuis `Refuse` ;
   - la validation `./scripts/verify.sh`.
5. Si le plan est trop large, réponds :
   ```text
   Réduis le plan : pas de refactor, pas de changement hors TransitionStatut et son test.
   ```

## Validation

- [ ] Tu sais expliquer la différence entre Interactive, Plan et Programmatic.
- [ ] Tu as utilisé `/plan` pour cadrer D3 avant modification.
- [ ] Tu as identifié les deux fichiers concernés par D3.
- [ ] Tu sais changer le modèle avec `/model`.
- [ ] `./scripts/verify.sh` ou `pwsh ./scripts/verify.ps1` est vert avant ou après l'exercice.

## Pour aller plus loin

- [Module 107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md) — décider selon coût, latence et risque.
- [Module 423 — Copilot CLI : contexte et conversations](./423-cli-contexte-conversations.md) — donner les bons fichiers à Copilot.
- [GitHub Docs — Copilot CLI command reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference)

## Sources

- [github/copilot-cli-for-beginners — 01 First Steps](https://github.com/github/copilot-cli-for-beginners/blob/main/01-setup-and-first-steps/README.md) — cours GitHub sous licence MIT, adapté en français.
- [GitHub Docs — GitHub Copilot CLI](https://docs.github.com/en/copilot/concepts/copilot-surfaces/copilot-cli)
- [GitHub Docs — Copilot CLI command reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference)
- [Mon Sinistre — README](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
- [`TransitionStatut.cs`](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/src/MonSinistre.Domain/TransitionStatut.cs)
- [GitHub Docs — Getting started with GitHub Copilot CLI](https://docs.github.com/en/copilot/get-started/cli-quickstart)
