---
id: app-modes-modeles-permissions
title: "431 — GitHub Copilot app : modes, modèles et permissions"
sidebar_position: 431
description: "Choisir Interactive, Plan ou Autopilot, régler modèle/effort/contexte, comprendre Auto, Retry, permissions d'outils et historique de prompts."
---

# 431 — GitHub Copilot app : modes, modèles et permissions

**Durée** : ~35 min · **Complexité** : ⭐⭐ · **Pré-requis** : [430 — Installer et faire le tour](./430-app-installer-tour.md), [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md)

> *Avant de lancer un agent, règle son niveau d'autonomie, sa capacité de raisonnement et ses permissions. Ces trois leviers valent mieux qu'un long prompt défensif.*

## Objectif

À la fin de ce module, tu sais :

- distinguer **Interactive**, **Plan** et **Autopilot** ;
- expliquer pourquoi **Auto** n'est pas **Autopilot** ;
- choisir un modèle, un effort de raisonnement et une fenêtre de contexte ;
- utiliser **Retry** quand une réponse doit être retentée autrement ;
- régler les permissions d'outils : **Always ask**, **Assisted**, **Approve all** ;
- retrouver et réutiliser l'historique des prompts ;
- utiliser le mode **Plan** pour analyser le défaut D3 sans modifier de code.

## Ce que tu vas apprendre

1. Les trois modes de session
2. Auto modèle ≠ Autopilot autonomie
3. Modèle, effort et fenêtre de contexte
4. Permissions d'outils
5. Retry et historique de prompts
6. Exercice guidé sur le défaut D3

## Contenu pédagogique

### Les modes de session

Le mode définit **combien l'agent peut faire seul**.

| Mode | Intention | À utiliser quand |
|---|---|---|
| **Interactive** | L'agent collabore étape par étape | Tu veux discuter, corriger, valider souvent |
| **Plan** | L'agent propose un plan avant d'agir | Le problème est ambigu ou touche plusieurs fichiers |
| **Autopilot** | L'agent exécute de bout en bout | La tâche est bien définie et le risque maîtrisé |

> Source : [Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions), section « Choosing a session mode ».

Pour un débutant, **Plan** est souvent le meilleur réflexe : il force Copilot à exposer son hypothèse, les fichiers concernés et les validations prévues avant le premier diff.

### Auto n'est pas Autopilot

Les deux mots se ressemblent, mais ils ne contrôlent pas la même chose.

| Contrôle | Ce qu'il pilote |
|---|---|
| **Auto** dans le sélecteur de modèle | le choix du modèle par Copilot selon la complexité de la tâche |
| **Autopilot** dans le sélecteur de mode | le niveau d'autonomie de l'agent pendant la session |

Tu peux donc avoir une session **Plan** avec modèle **Auto**, ou une session **Autopilot** avec un modèle choisi explicitement. Dans le premier cas, l'app choisit le modèle, mais l'agent attend ton approbation du plan. Dans le second, le modèle est fixé, mais l'agent travaille avec beaucoup d'autonomie.

### Modèle, effort et fenêtre de contexte

Le module [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md) explique les familles de modèles et leurs coûts. Dans l'app, tu retrouves les mêmes leviers sous le prompt :

| Réglage | Effet pratique |
|---|---|
| **Model** | Qualité, vitesse, coût et spécialisation de la réponse |
| **Effort** | Temps de raisonnement accordé à certains modèles |
| **Context window** | Quantité de texte que le modèle peut garder en mémoire : conversation, fichiers, issues, outils |

Pour une question simple sur `samples/mon-sinistre`, garde **Auto** ou le modèle par défaut. Pour un bug métier comme D3, passe en **Plan** et choisis éventuellement un modèle plus fort ou un effort plus élevé. Plus d'effort ne garantit pas la vérité : il augmente surtout le temps consacré au raisonnement.

### Retry

Le bouton **Retry** sous une réponse permet de relancer la même demande avec un autre modèle, un autre effort ou une autre fenêtre de contexte. Utilise-le quand :

- la réponse est trop vague ;
- le plan ignore un fichier important ;
- l'agent a raisonné trop vite ;
- tu veux comparer une réponse rapide avec une réponse plus approfondie.

Ne l'utilise pas pour contourner une validation qui échoue. Si `verify.sh` échoue, il faut comprendre l'échec, pas « retenter jusqu'à ce que ça passe ».

### Permissions d'outils

Le menu de mode contient aussi **Tool permissions**. Il contrôle les demandes d'autorisation avant l'usage d'outils comme le terminal.

| Réglage | Comportement | Usage conseillé |
|---|---|---|
| **Always ask** | Copilot demande avant chaque outil | Début d'apprentissage, dépôts sensibles, commandes inconnues |
| **Assisted** | Copilot demande surtout quand une action déclenche un contrôle de sécurité | Bon compromis pour les exercices |
| **Approve all** | Copilot exécute sans demander | Tâches très bornées, environnement jetable, confiance élevée |

Pour ce handbook, commence avec **Always ask** ou **Assisted**. L'objectif pédagogique est de voir quelles commandes l'agent veut lancer : `dotnet test`, `./scripts/verify.sh`, `git diff`, etc.

### Historique de prompts

L'historique évite de réécrire les mêmes consignes. Dans l'app, utilise **Ctrl+R** ou **Cmd+R** selon ton système pour rechercher des prompts récents. C'est utile pour réutiliser :

- une consigne de plan ;
- une demande de validation ;
- un prompt de résumé PR ;
- une formule qui interdit explicitement les modifications.

Comme pour les snippets de code, relis toujours un prompt repris depuis l'historique : le contexte du module précédent n'est pas forcément celui de la session courante.

## Exercice

**Énoncé** : utilise **Plan** mode pour analyser D3, sans implémenter la correction.

D3 est décrit dans `samples/mon-sinistre/README.md` : `TransitionStatut.EstAutorisee(Refuse, Indemnise)` renvoie `true`, alors qu'un sinistre refusé doit rester définitif.

1. Démarre une session projet sur ton fork.
2. Choisis **New worktree** si l'option est disponible.
3. Mets la session en **Plan**.
4. Garde **Auto** pour le modèle, ou choisis un modèle de raisonnement si tu veux comparer.
5. Mets les permissions d'outils sur **Always ask** ou **Assisted**.
6. Envoie :

```text
Analyse le défaut D3 dans @samples/mon-sinistre.
Concentre-toi sur la règle de transition de statut : un sinistre Refuse ne doit pas pouvoir devenir Indemnise.
Identifie les fichiers probablement concernés, propose un plan de correction et les commandes de validation.
Ne modifie aucun fichier.
```

7. Lis le plan. Il devrait citer au minimum `TransitionStatut.cs` et les tests associés.
8. Si le plan est trop flou, utilise **Retry** avec un effort plus élevé ou demande :

```text
Rends le plan plus précis : nomme les méthodes, le test à ajouter ou modifier, et la commande exacte pour valider.
Toujours sans modifier de fichier.
```

9. Ouvre l'historique de prompts et vérifie que tu peux retrouver ta demande.

## Validation

- [ ] Tu sais expliquer **Interactive**, **Plan** et **Autopilot**.
- [ ] Tu sais expliquer **Auto ≠ Autopilot**.
- [ ] Tu as localisé les réglages modèle, effort et contexte.
- [ ] Tu as vu où régler les permissions d'outils.
- [ ] Le plan D3 cite les fichiers pertinents et ne crée aucun diff.
- [ ] Tu sais retrouver le prompt dans l'historique.

## Pour aller plus loin

- [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md)
- [432 — Sessions, worktrees et contexte](./432-app-sessions-worktrees-contexte.md)
- [Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions)

## Sources

- [github/copilot-app-for-beginners — 01 Tour the App](https://github.com/github/copilot-app-for-beginners/blob/main/01-tour-the-app/README.md)
- [GitHub Docs — Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions)
- [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md)
