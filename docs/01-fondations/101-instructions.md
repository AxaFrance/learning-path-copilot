---
id: 01-instructions-personnalisees
title: "101 — Instructions personnalisées"
sidebar_position: 101
description: "Donner à Copilot un cadre persistant avec .github/copilot-instructions.md et des instructions scopées via applyTo."
---

# 101 — Instructions personnalisées

**Durée** : ~45 min · **Complexité** : ⭐⭐ · **Pré-requis** : [Module 100 — Setup & posture](./100-setup-posture.md)

> Un *prompt* bien tourné fonctionne une fois. Une *instruction* bien posée cadre toutes tes interactions Copilot du repo, sans la répéter.

## Objectif

À la fin de ce module, tu sais :

- Créer un fichier `.github/copilot-instructions.md` qui s'applique à tout le repo.
- Ajouter une instruction scopée à un type de fichier via le champ *applyTo*.
- Vérifier l'impact d'une instruction en comparant un même *prompt* avant et après.

## Ce que tu vas apprendre

1. Distinguer instruction globale et instruction scopée.
2. Rédiger un `copilot-instructions.md` court et opérant.
3. Utiliser le *frontmatter* `applyTo` pour cibler un sous-ensemble de fichiers.
4. Observer le changement de comportement de Copilot sur un même *prompt*.
5. Choisir une instruction plutôt qu'un *prompt* quand la règle doit persister.

## Contenu pédagogique

### Instruction globale du repo

On veut que Copilot connaisse les conventions du projet dès qu'on ouvre une conversation. On crée pour cela `.github/copilot-instructions.md` à la racine du repo. Le fichier est chargé automatiquement par Copilot Chat et par l'agent mode — aucune activation manuelle nécessaire.

```diff
+ # .github/copilot-instructions.md
+ 
+ Tu assistes une équipe TypeScript sur une API Node.
+ 
+ ## Stack
+ - TypeScript en mode strict, jamais de `any` ni de `as unknown`.
+ - Tests avec Vitest, jamais Jest.
+ - Lint via Biome (pas ESLint).
+ 
+ ## Style
+ - Fonctions pures quand c'est possible.
+ - Pas de classes utilitaires, préfère des modules de fonctions.
```

Dès le prochain message dans le chat, Copilot intègre ces contraintes : il propose `vitest` au lieu de `jest`, refuse d'introduire `any`, et structure ses suggestions en modules de fonctions. La règle vit dans le repo, donc tout le monde en profite — y compris les agents.

### Instruction scopée par `applyTo`

L'instruction globale est utile mais grossière. Pour cibler un sous-ensemble (ex. les fichiers TypeScript uniquement, ou les composants React), on passe par une instruction scopée. On crée un fichier sous `.github/instructions/` avec un *frontmatter* qui contient `applyTo` — une glob qui décrit les fichiers concernés.

```diff
+ ---
+ applyTo: '**/*.ts'
+ ---
+ 
+ # .github/instructions/typescript.instructions.md
+ 
+ Quand tu écris du TypeScript dans ce repo :
+ 
+ - Toujours typer les retours de fonctions exportées.
+ - Préfère `type` à `interface` pour les objets de données.
+ - Importe les types avec `import type { … }` séparé des imports de valeurs.
```

Copilot ne charge ce fichier que lorsqu'un fichier `.ts` est ouvert ou évoqué dans la conversation. Tu peux multiplier ces instructions scopées (`**/*.tsx`, `tests/**`, `infra/**`) sans gonfler le contexte global. La règle est aussi proche que possible du code qu'elle régit.

### Mesurer l'effet — même *prompt*, avant / après

Pour valider qu'une instruction est *active*, on rejoue le même *prompt* avant et après. Mets une version vide du fichier en place, demande à Copilot une fonction de test, puis ajoute les contraintes et redemande la même chose.

```diff
  # .github/copilot-instructions.md
  
- (fichier vide)
+ Pour ce repo :
+ - Tests avec Vitest, jamais Jest.
+ - Une assertion par test, nom de test en français.
```

Avant : Copilot répond probablement avec `describe / it` en anglais et des imports `@jest/globals`. Après : il importe depuis `vitest`, nomme les tests en français et limite chaque `it` à une seule assertion. Ce delta observable est ta preuve que l'instruction est chargée. Si le delta n'apparaît pas, vérifie le chemin du fichier (toujours sous `.github/`) et que la session Chat a été relancée.

### Quand instruction, quand *prompt* ?

Règle simple : si tu dois répéter une consigne plus de deux fois dans des conversations différentes, elle mérite une instruction. Un *prompt* couvre l'intention d'aujourd'hui ; une instruction couvre la règle de toujours. Le module 02 traitera des *prompts* réutilisables — complémentaires, jamais redondants avec une instruction.

## Exercice

**Énoncé** — En 15 minutes, force Copilot à utiliser Vitest au lieu de Jest dans un repo neuf, puis ajoute une règle TypeScript scopée.

**Étapes guidées** :

1. Initialise un repo vide : `git init copilot-instructions-lab && cd copilot-instructions-lab`.
2. Crée `.github/copilot-instructions.md` avec la contrainte « tests Vitest, jamais Jest ».
3. Demande dans le chat : « écris-moi un test pour une fonction `sum(a, b)` ». Sauvegarde la réponse dans `sum.test.ts`.
4. Crée `.github/instructions/typescript.instructions.md` avec `applyTo: '**/*.ts'` qui impose des retours typés explicites.
5. Demande dans le chat la même fonction `sum`. Observe les retours typés et l'import depuis `vitest`.
6. Note dans `OBSERVATIONS.md` un avant/après concret pour chaque instruction.

**Critère de réussite** : `sum.test.ts` importe depuis `vitest` (pas `@jest/globals`) et la fonction `sum` a une signature `(a: number, b: number): number`.

## Validation

Tu peux passer au module suivant si :

- [ ] Ton repo contient un `.github/copilot-instructions.md` non vide et versionné.
- [ ] Tu as au moins une instruction scopée avec `applyTo` et tu sais quelle glob elle cible.
- [ ] Tu peux montrer un avant/après concret pour une de tes instructions.

## Pour aller plus loin

- [Customizing GitHub Copilot — repository instructions](https://docs.github.com/en/copilot/customizing-copilot/adding-repository-custom-instructions-for-github-copilot) — référence officielle GitHub Docs.
- [Customize chat responses in VS Code](https://code.visualstudio.com/docs/copilot/copilot-customization) — instructions et `applyTo` côté Microsoft.
- [Glossaire — instruction, prompt](../../ressources/glossaire-fr.md) — rappels de définitions courtes.

**Suivant** : [102 — Prompts personnalisés](./102-prompts.md)
