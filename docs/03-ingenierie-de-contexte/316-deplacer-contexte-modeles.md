---
id: deplacer-contexte-modeles
title: "316 — Déplacer le contexte entre modèles"
sidebar_position: 316
description: "Un seul modèle par session. Quand la phase suivante exige un autre modèle, transférez le contexte proprement — forkez la conversation, ou passez par un court fichier de spec."
---

# 316 — Déplacer le contexte entre modèles

**Durée** : ~35 min · **Complexité** : ⭐⭐ · **Pré-requis** : [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md), [311 — Tokens & contexte](./311-tokens-contexte.md)

> *Un seul modèle par session. Quand la phase suivante exige un autre modèle, transférez le contexte proprement — forkez la conversation, ou passez par un court fichier de spec — au lieu de changer de modèle en cours de session.*

## Objectif

À la fin de ce module, tu sais :

- Séparer une tâche en deux phases : planifier avec un modèle puissant, implémenter avec un plus léger.
- Transférer le contexte d'une phase à l'autre par *fork* de conversation (Option A).
- Transférer le contexte par un fichier de spec Markdown (Option B).
- Choisir entre les deux approches selon le besoin de contexte.

## Ce que tu vas apprendre

1. Le workflow en deux phases : planifier puis implémenter
2. Option A — forker la conversation
3. Option B — transfert en Markdown
4. Quand choisir l'une plutôt que l'autre

## Contenu pédagogique

La règle de base ([Module 107](../01-fondations/107-choix-de-modeles.md)) : un seul modèle par session, pour garder le cache de prompt chaud. Mais une tâche réelle a souvent deux phases qui n'appellent pas le même palier : **planifier** (raisonnement profond) puis **implémenter** (ingénierie du quotidien). Le défi : passer de l'une à l'autre sans changer de modèle en cours de session.

Le workflow : planifier avec un modèle puissant · implémenter avec un plus léger · un modèle par session.

### Étape 1 — Planifier & architecturer

**Palier** : Raisonnement profond — Opus, GPT-5.5

Ouvre une session avec un modèle de raisonnement profond. Laisse-le explorer le problème, peser les compromis et produire le plan et l'architecture. C'est la réflexion coûteuse — fais-la une fois, fais-la bien.

À la fin de cette phase, tu as un plan prêt. Il faut maintenant le transmettre à la phase d'implémentation, qui tourne sur un modèle plus léger. **Deux approches.**

### Option A — Forker la conversation

*Dérive le chat actuel et change de modèle — le contexte suit.*

1. Arrive au point où le plan est prêt.
2. Clique sur « **Forker la conversation depuis ce point** ».
3. Sur la nouvelle branche, passe à Sonnet 4.6 et développe.

```text
Forker la conversation depuis ce point
```

✅ **Contexte complet hérité — rien à réexpliquer.**

La nouvelle branche hérite de tout l'historique de raisonnement. Tu ne perds rien, mais tu emportes aussi tout le contexte — y compris ce dont la phase d'implémentation n'a pas besoin.

### Option B — Transfert en Markdown

*Persiste le plan, puis démarre une session propre et dédiée.*

1. Enregistre le plan dans un fichier de spec (ex. `plan.md`).
2. Ouvre une toute nouvelle session.
3. Pointe Sonnet 4.6 vers le fichier et implémente.

```text
save → plan.md
```

✅ **Contexte minimal — le fichier `.md` fait foi.**

La nouvelle session démarre vierge, avec pour seul contexte le fichier de spec. Le cache de prompt repart de zéro mais reste minimal et propre : aucun bruit hérité de la phase de planification.

### Étape 2 — Implémenter

**Palier** : Ingénierie du quotidien — Sonnet 4.6

Un seul modèle pour toute la session d'implémentation. Le cache de prompt reste chaud, la sortie reste cohérente et tu dépenses bien moins de tokens — exactement ce que changer de modèle en cours de session casserait.

### Fork ou Markdown : comment choisir

| Critère | Option A — Fork | Option B — Markdown |
|---|---|---|
| Contexte transmis | Complet (tout l'historique) | Minimal (le fichier `.md`) |
| Réexplication | Aucune | Aucune (le `.md` fait foi) |
| Bruit hérité | Possible | Aucun |
| Idéal quand… | le raisonnement détaillé compte pour l'implémentation | le plan se résume proprement en un document |

## Exercice

**Énoncé** — En 20 minutes, tu vas exécuter le workflow en deux phases sur une petite feature.

**Étapes guidées** :

1. Ouvre une session avec un modèle de raisonnement profond et fais-lui produire un plan d'implémentation pour une feature simple.
2. Transmets le plan par **Option B** : enregistre-le dans `plan.md`.
3. Ouvre une nouvelle session, pointe un modèle d'ingénierie du quotidien vers `plan.md` et implémente.
4. Recommence avec **Option A** (fork) sur une autre feature, et compare la sensation : contexte hérité vs contexte minimal.

**Critère de réussite** : tu as livré une feature en gardant un seul modèle par session, sans jamais changer de modèle en cours de session.

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais découper une tâche en phase de planification et phase d'implémentation.
- [ ] Tu sais forker une conversation pour changer de modèle (Option A).
- [ ] Tu sais transférer un plan via un fichier `.md` vers une session propre (Option B).
- [ ] Tu sais expliquer quel transfert choisir selon le besoin de contexte.

## Pour aller plus loin

- [Module 317 — Orchestrer des subagents](./317-orchestrer-subagents.md) — laisser un runtime gérer le transfert de contexte automatiquement.
- [Module 318 — Mesurer & optimiser sa consommation](./318-mesurer-optimiser-consommation.md) — vérifier l'effet du cache de prompt sur la facture.

## Source

Contenu issu du retour d'expérience d'**Ousmane BARRY**, *MVP Microsoft Foundry* — guide pratique « Développement assisté par IA » ([publication LinkedIn](https://www.linkedin.com/posts/ousmanebarry_depuis-le-1er-juin-on-a-tous-vu-et-senti-ugcPost-7468560023623901184-K69e/)).

**Suivant** : [317 — Orchestrer des subagents](./317-orchestrer-subagents.md)
