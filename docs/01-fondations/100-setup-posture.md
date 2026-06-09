---
id: 00-setup-posture
title: "100 — Setup & posture"
sidebar_position: 100
description: "Installer Copilot dans VS Code, connecter ton compte et distinguer suggestion inline, chat et agent mode."
---

# 100 — Setup & posture

**Durée** : ~20 min · **Complexité** : ⭐ · **Pré-requis** : aucun

> Avant de personnaliser Copilot, il faut savoir où il vit dans ton IDE. Ce module pose la posture : trois surfaces, trois usages, zéro confusion.

## Objectif

À la fin de ce module, tu sais :

- Installer l'extension *GitHub Copilot* dans VS Code et connecter ton compte.
- Distinguer les trois surfaces : *inline suggestion*, *chat*, *agent mode*.
- Choisir la bonne surface selon l'intention (compléter, dialoguer, déléguer).
- Différencier un *prompt* ponctuel d'une *instruction* persistante.

## Ce que tu vas apprendre

1. Recommander Copilot à ton repo via `.vscode/extensions.json`.
2. Vérifier que ton compte GitHub est connecté à l'extension.
3. Déclencher une suggestion inline sur une fonction simple.
4. Obtenir le même résultat via le chat.
5. Activer l'agent mode et le laisser créer plusieurs fichiers.
6. Nommer ce qui change entre prompt et instruction.

## Contenu pédagogique

### Surface 1 — installer l'extension

On veut que VS Code recommande Copilot à toute personne qui ouvre le repo.

```diff
  // .vscode/extensions.json
  {
    "recommendations": [
+     "GitHub.copilot",
+     "GitHub.copilot-chat"
    ]
  }
```

Au prochain `code .`, VS Code propose d'installer les deux extensions. Une fois la commande `GitHub Copilot: Sign in` validée, l'icône Copilot apparaît dans la barre d'état.

### Surface 2 — suggestion inline

On écrit le squelette d'une fonction et on laisse Copilot compléter la ligne grisée.

```diff
  // fizzbuzz.ts
  export function fizzBuzz(n: number): string {
-   // TODO
+   if (n % 15 === 0) return "FizzBuzz";
+   if (n % 3 === 0) return "Fizz";
+   if (n % 5 === 0) return "Buzz";
+   return String(n);
  }
```

Tu valides chaque ligne avec `Tab`. Copilot ne voit que le fichier ouvert : zéro contexte projet, zéro test généré. Surface idéale pour du code local et trivial.

### Surface 3 — agent mode

On bascule en agent mode dans le chat et on demande la même fonction **avec ses tests**. L'agent crée deux fichiers en un tour.

```diff
+ // fizzbuzz-agent.ts
+ export function fizzBuzz(n: number): string {
+   if (n % 15 === 0) return "FizzBuzz";
+   if (n % 3 === 0) return "Fizz";
+   if (n % 5 === 0) return "Buzz";
+   return String(n);
+ }
```

```diff
+ // fizzbuzz.test.ts
+ import { describe, it, expect } from "vitest";
+ import { fizzBuzz } from "./fizzbuzz-agent";
+
+ describe("fizzBuzz", () => {
+   it("retourne FizzBuzz pour 15", () => {
+     expect(fizzBuzz(15)).toBe("FizzBuzz");
+   });
+ });
```

L'agent a lu l'arborescence, choisi un emplacement, créé deux fichiers et proposé un diff que tu acceptes ou refuses globalement. Surface idéale pour une tâche multi-fichiers cadrée.

### Prompt vs instruction

Un *prompt* est une demande ponctuelle (« génère-moi ces tests »). Une *instruction* est une règle persistante chargée à chaque interaction (« utilise Vitest, jamais Jest »). Les modules 01 et 02 traitent chacun de ces leviers.

## Exercice

**Énoncé** — En 10 minutes, génère la fonction `fizzBuzz` en TypeScript via les trois surfaces et compare les résultats.

**Étapes guidées** :

1. Installe l'extension Copilot et connecte ton compte GitHub.
2. Crée `fizzbuzz-inline.ts` et laisse la suggestion inline compléter le corps.
3. Ouvre le chat, demande `fizzBuzz` en TypeScript, sauvegarde le résultat dans `fizzbuzz-chat.ts`.
4. Passe en agent mode, demande la fonction **plus un test Vitest**.
5. Note dans un fichier `OBSERVATIONS.md` une différence concrète par surface.

**Critère de réussite** : les trois fichiers existent et `OBSERVATIONS.md` cite au moins une différence par surface.

## Validation

Tu peux passer au module suivant si :

- [ ] L'icône Copilot est verte dans la barre d'état VS Code.
- [ ] Tu as déclenché une suggestion inline, une réponse chat et une action agent dans la même session.
- [ ] Tu sais expliquer en une phrase quand utiliser chaque surface.
- [ ] Tu différencies prompt ponctuel et instruction persistante.

## Pour aller plus loin

- [Quickstart GitHub Copilot](https://docs.github.com/en/copilot/quickstart) — installation et sign-in pas-à-pas.
- [Copilot dans VS Code — vue d'ensemble](https://code.visualstudio.com/docs/copilot/overview) — les trois surfaces côté Microsoft.
- [Agent mode dans VS Code](https://code.visualstudio.com/docs/copilot/chat/chat-agent-mode) — référence du mode multi-fichiers.
- Module 10 — *Copilot CLI* (à venir).

**Suivant** : [101 — Instructions personnalisées](./101-instructions.md)
