---
id: cli-contexte-conversations
title: "423 — Copilot CLI : contexte et conversations"
sidebar_position: 423
description: "Donner à Copilot CLI les bons fichiers avec @, gérer les conversations et le contexte, reprendre une session, et repérer D4 par comparaison."
---

# 423 — Copilot CLI : contexte et conversations

**Durée** : ~40 min · **Complexité** : ⭐⭐ · **Pré-requis** : [422 — Copilot CLI : modes et slash commands](./422-cli-modes-slash-commands.md)

> *Un bon agent n'est pas devin. Le contexte que tu fournis décide souvent de la qualité de sa réponse : trop peu, il hallucine ; trop large, il se disperse ; bien ciblé, il raisonne.*

## Objectif

À la fin de ce module, tu sais :

- référencer un fichier, un dossier ou plusieurs fichiers avec `@` ;
- comparer deux fichiers pour détecter une duplication ;
- nommer, reprendre et nettoyer une session ;
- surveiller le contexte avec `/context`, `/compact`, `/new` et `/clear` ;
- relier cette pratique aux tokens du module 311.

## Ce que tu vas apprendre

1. La syntaxe `@fichier` et `@dossier`
2. L'analyse multi-fichiers
3. Sessions, reprise et mémoire
4. Gestion du contexte et tokens
5. Permissions multi-dossiers
6. Un exercice sur D4

## Contenu pédagogique

### La syntaxe `@` : montrer le bon objet

Dans Copilot CLI, le symbole `@` sert à référencer des fichiers et dossiers dans un prompt. Le cours donne trois motifs de base : `@file.py`, `@folder/` et `@file1.py @file2.py`.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, The @ Syntax](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

Dans Mon Sinistre, tu peux commencer petit :

```text
Explique @src/MonSinistre.Domain/CalculIndemnisation.cs.
```

Puis élargir quand la question traverse plusieurs fichiers :

```text
@src/MonSinistre.Cli/Program.cs
@src/MonSinistre.Domain/CalculIndemnisation.cs
Compare le formatage des montants. Y a-t-il une duplication ?
```

Le défaut D4 du sample est justement une duplication de formatage entre `CalculIndemnisation.FormaterMontant` et la méthode `Lister` du CLI.

> Source : [Mon Sinistre — README, Défauts volontaires](../../samples/mon-sinistre/README.md)  
> Source : [`CalculIndemnisation.cs`](../../samples/mon-sinistre/src/MonSinistre.Domain/CalculIndemnisation.cs)  
> Source : [`Program.cs`](../../samples/mon-sinistre/src/MonSinistre.Cli/Program.cs)

### Fichier, dossier ou plusieurs fichiers ?

Le cours explique que l'analyse d'un seul fichier est utile, mais que l'analyse multi-fichiers révèle les flux de données, duplications et problèmes d'architecture invisibles isolément.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Cross-File Intelligence](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

| Besoin | Prompt conseillé | Pourquoi |
|---|---|---|
| Comprendre une fonction | `@src/.../CalculIndemnisation.cs` | Contexte minimal. |
| Comparer deux implémentations | deux fichiers précis | Évite de charger toute la solution. |
| Cartographier le sample | `@src/` ou `@src/MonSinistre.Domain/` | Utile pour une première vue. |
| Chercher un défaut connu | fichiers ciblés + symptôme | Meilleur signal. |

Le cours recommande d'être spécifique : charger le fichier utile plutôt qu'un dossier entier préserve le contexte.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Understanding Context Windows](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

:::tip Lien avec le module 311
Le module [311 — Tokens & contexte](../03-ingenierie-de-contexte/311-tokens-contexte.md) explique pourquoi chaque fichier ajouté consomme une partie de la fenêtre de contexte. Ici, tu appliques cette théorie dans le CLI : tu charges ce qui sert à la décision, pas tout ce qui existe.
:::

### Multi-fichiers : demander une comparaison, pas une intuition

Un prompt multi-fichiers doit nommer l'objectif de comparaison. « Regarde ces fichiers » est moins utile que « compare les responsabilités » ou « détecte les duplications ».

```text
@src/MonSinistre.Domain/CalculIndemnisation.cs
@src/MonSinistre.Cli/Program.cs

Compare uniquement le formatage des montants :
- où est-il implémenté ?
- les règles sont-elles identiques ?
- quel fichier devrait porter cette responsabilité ?
- propose une correction minimale, sans l'appliquer.
```

Dans `CalculIndemnisation.FormaterMontant`, le domaine convertit des centimes en euros avec culture `fr-FR`, virgule et suffixe `€`. Dans `Program.cs`, la méthode `Lister` refait localement une logique proche sur `MontantEstimeCentimes`.

> Source : [`CalculIndemnisation.cs`](../../samples/mon-sinistre/src/MonSinistre.Domain/CalculIndemnisation.cs)  
> Source : [`Program.cs`](../../samples/mon-sinistre/src/MonSinistre.Cli/Program.cs)

Cette duplication ne casse pas `verify.sh`, car les défauts volontaires du sample ne sont pas tous couverts par les tests.

> Source : [Mon Sinistre — README, Défauts volontaires](../../samples/mon-sinistre/README.md)

### Conversations multi-tours

Le cours montre que la conversation devient puissante quand les prompts se construisent les uns sur les autres : analyse, amélioration, puis tests.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Context-Aware Conversations](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

Exemple Mon Sinistre :

```text
@src/MonSinistre.Domain/CalculIndemnisation.cs
Explique les responsabilités de cette classe.

@src/MonSinistre.Cli/Program.cs
Maintenant compare avec le formatage utilisé dans Lister.

Propose un patch minimal qui remplace la duplication par FormaterMontant,
mais ne modifie rien tant que je n'ai pas validé.
```

Le point important : Copilot garde le fil de ce qui a déjà été lu et discuté. Si tu changes de sujet, nettoie la session.

### Sessions : nommer, reprendre, continuer

Les sessions Copilot CLI sont automatiquement sauvegardées. Tu peux reprendre la plus récente avec `copilot --continue`, choisir une session avec `copilot --resume`, ou reprendre par nom si tu l'as renommée.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Session Management](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

```bash
copilot --name mon-sinistre-d4

# dans la session
/rename mon-sinistre-d4
/exit

# plus tard
copilot --resume=mon-sinistre-d4
copilot --continue
```

Le cours précise que chaque terminal ouvert a sa propre session et que `/resume` peut aussi s'utiliser depuis une session active.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Session Management](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

### Gérer la fenêtre de contexte

La commande `/context` affiche l'usage de la fenêtre de contexte ; `/compact` résume l'historique pour libérer de la place ; `/new` termine la session en la sauvegardant et démarre une nouvelle conversation ; `/clear` abandonne la session courante sans historique sauvegardé.

> Source : [copilot-cli-for-beginners — 02 Context and Conversations, Check and Manage Context](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md)

```text
/context
/compact focus on D4 formatage des montants
/new
/clear
```

La documentation GitHub indique aussi que Copilot CLI gère automatiquement la compaction quand la conversation approche la limite de tokens, et que `/context` permet de visualiser l'usage.

> Source : [GitHub Docs — GitHub Copilot CLI, Automatic context management](https://docs.github.com/en/copilot/concepts/copilot-surfaces/copilot-cli#automatic-context-management)

### Permissions et contexte hors dossier

Par défaut, le cours explique que Copilot accède aux fichiers du dossier courant. Pour un autre dossier, tu peux ajouter un accès avec `copilot --add-dir` ou `/add-dir`, afficher les dossiers avec `/list-dirs`, ou lever les restrictions avec `--allow-all-paths`.

> Source : [copilot-cli-for-beginners — Appendix Additional Context, Permission Patterns](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/additional-context.md)

Dans ce bloc, tu n'as normalement pas besoin d'ajouter de dossier : tout l'exercice vit dans `samples/mon-sinistre`.

## Exercice

**Énoncé** : repère D4 en comparant deux fichiers, sans corriger le code.

1. Depuis `samples/mon-sinistre`, lance :
   ```bash
   ./scripts/verify.sh
   copilot --name mon-sinistre-d4
   ```
2. Donne exactement les deux fichiers utiles :
   ```text
   @src/MonSinistre.Domain/CalculIndemnisation.cs
   @src/MonSinistre.Cli/Program.cs

   Repère les duplications de formatage de montants.
   Réponds avec :
   - les méthodes concernées ;
   - la règle dupliquée ;
   - le risque de maintenance ;
   - une correction minimale sans l'appliquer.
   ```
3. Vérifie avec `/context` que tu n'as pas chargé toute la solution.
4. Renomme si besoin :
   ```text
   /rename mon-sinistre-d4-formatage
   ```
5. Quitte et reprends :
   ```bash
   copilot --continue
   ```
6. Demande :
   ```text
   Résume en 3 lignes ce que nous avons trouvé sur D4.
   ```

## Validation

- [ ] Tu as utilisé deux références `@` précises, pas tout le dossier.
- [ ] Tu as identifié `CalculIndemnisation.FormaterMontant` et la logique locale de `Lister`.
- [ ] Tu sais expliquer pourquoi D4 est un problème de maintenance.
- [ ] Tu as utilisé `/context` au moins une fois.
- [ ] `./scripts/verify.sh` ou `pwsh ./scripts/verify.ps1` est vert.

## Pour aller plus loin

- [Module 311 — Tokens & contexte](../03-ingenierie-de-contexte/311-tokens-contexte.md) — comprendre le coût du contexte.
- [Module 424 — Copilot CLI : workflows de développement](./424-cli-workflows-dev.md) — passer de l'analyse à la correction testée.
- [GitHub Docs — Using GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/copilot-cli/use-copilot-cli/overview)

## Source

- [github/copilot-cli-for-beginners — 02 Context and Conversations](https://github.com/github/copilot-cli-for-beginners/blob/main/02-context-conversations/README.md) — cours GitHub sous licence MIT, adapté en français.
- [github/copilot-cli-for-beginners — Appendix Additional Context](https://github.com/github/copilot-cli-for-beginners/blob/main/appendices/additional-context.md) — cours GitHub sous licence MIT, adapté en français.
- [GitHub Docs — GitHub Copilot CLI](https://docs.github.com/en/copilot/concepts/copilot-surfaces/copilot-cli)
- [Mon Sinistre — README](../../samples/mon-sinistre/README.md)
