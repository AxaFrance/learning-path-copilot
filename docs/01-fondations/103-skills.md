---
id: skills
title: "103 — Skills — savoir-faire procéduraux"
sidebar_position: 103
description: "Créer un SKILL.md pour que Copilot déclenche une procédure uniquement quand le contexte l'exige."
---

# 103 — Skills — savoir-faire procéduraux

Durée estimée : 90 min · Complexité : intermédiaire · Pré-requis : [Module 101 — Instructions personnalisées](./101-instructions.md)

> Les instructions disent à Copilot *comment* se comporter en permanence. Un `skill` lui dit *quoi faire* — et seulement *quand* c'est pertinent.

## Pourquoi ce module

Dans le module précédent, tu as appris à poser des règles permanentes via les instructions. Mais certaines consignes ne sont pertinentes que dans un contexte précis : rédiger un message de commit, générer un changelog, structurer un ADR. Charger ces procédures dans *toutes* les conversations gaspille du contexte et brouille le signal.

Un `skill` résout ce problème. C'est un fichier `SKILL.md` que Copilot charge *uniquement* quand sa description sémantique correspond à ta demande. À la fin de ce module, tu sais :

- expliquer le triplet *quand / quoi / comment* d'un skill ;
- créer un fichier `SKILL.md` avec une description qui sert de `trigger` sémantique ;
- distinguer un skill d'une instruction : procédure conditionnelle vs règle permanente ;
- vérifier que Copilot active ton skill au bon moment — et ne l'active pas sinon.

## Pré-requis

- [Module 101 — Instructions personnalisées](./101-instructions.md)
- VS Code avec l'extension GitHub Copilot activée.
- Un dépôt Git avec au moins un fichier source.

## Concepts clés

### Qu'est-ce qu'un skill ?

Un `skill` est un savoir-faire procédural que Copilot active *conditionnellement*. Contrairement à une instruction (chargée à chaque conversation), un skill ne se déclenche que lorsque sa `description` sémantique correspond à la demande de l'utilisateur.

Le mécanisme repose sur un triplet :

- **Quand** — la description du skill décrit la situation de déclenchement.
- **Quoi** — le nom du skill identifie le savoir-faire.
- **Comment** — le corps du fichier `SKILL.md` contient la procédure à suivre.

### Anatomie d'un SKILL.md

Un skill vit dans son propre dossier sous `.agents/skills/`. La structure minimale :

```text
.agents/
  skills/
    writing-commit-message/
      SKILL.md
```

Le nom du skill est dérivé du nom de son dossier parent (`writing-commit-message`). Le fichier `SKILL.md` lui-même se compose de deux parties :

- **Description** — le premier paragraphe du fichier. C'est la phrase que le routeur sémantique de Copilot évalue pour décider d'activer ou non le skill.
- **Corps procédural** — les étapes que Copilot doit suivre lorsque le skill est activé.

Exemple minimal :

```markdown
# writing-commit-message

Use when the user asks to write, draft, or generate a commit message.

## Procedure

1. Read the staged diff with `git diff --cached`.
2. Write a commit message following Conventional Commits format:
   - `type(scope): description` on the first line.
   - Types allowed: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`.
3. Keep the first line under 72 characters.
4. Add a body only if the diff touches more than one concern.
```

### La description : un trigger, pas de la documentation

C'est le point le plus contre-intuitif. La description d'un skill **n'est pas une documentation pour les humains** — c'est une phrase que le routeur sémantique de Copilot compare à la requête de l'utilisateur pour décider d'activer ou non le skill.

Une bonne description répond à la question : « *dans quelle situation* Copilot doit-il charger cette procédure ? »

| Mauvaise description | Bonne description |
|---|---|
| « Ce skill gère les commits. » | « Use when the user asks to write, draft, or generate a commit message. » |
| « Formatage Conventional Commits » | « Use when committing code and the message must follow Conventional Commits format. » |

La description est écrite en anglais par convention — c'est la langue du routeur sémantique de Copilot. Elle commence typiquement par « Use when… ».

### Placement et découverte

Le chemin canonique est :

```text
.agents/skills/<nom-du-skill>/SKILL.md
```

Copilot découvre automatiquement tous les fichiers `SKILL.md` présents dans `.agents/skills/`. Aucune déclaration dans un fichier de configuration n'est nécessaire — la seule présence du fichier suffit.

### Skill vs instruction

| Critère | Instruction (`.instructions.md`) | Skill (`SKILL.md`) |
|---|---|---|
| Chargement | Automatique, à chaque conversation | Conditionnel, par le routeur sémantique |
| Nature | Règle permanente | Procédure conditionnelle |
| Granularité | Convention d'équipe, style, contraintes | Tâche précise, savoir-faire métier |
| Risque si trop nombreux | Bruit modéré (contexte permanent) | Bruit sévère (pollution du routeur) |

**Règle simple** : si la consigne doit s'appliquer *à chaque conversation*, c'est une instruction. Si elle ne doit se déclencher que *dans un contexte précis*, c'est un skill.

### Pièges courants

- **Description trop vague** — « Ce skill gère les commits » ne donne pas assez de signal au routeur sémantique. Sois explicite sur les verbes d'action : « write, draft, generate a commit message ».
- **Skill qui duplique une instruction** — Si tu gardes la même règle dans `.instructions.md` *et* dans un skill, Copilot reçoit l'information deux fois. Choisis un seul endroit.
- **Trop de skills** — Chaque skill est évalué par le routeur sémantique à chaque requête. Dix skills bien ciblés valent mieux que cinquante skills vagues. Si un skill ne se déclenche jamais, supprime-le.
- **Procédure trop longue** — Un skill qui fait 200 lignes est probablement un `agent` déguisé. Si la procédure nécessite une identité, un jeu d'outils restreint ou une longue conversation, passe à un fichier `.agent.md` (module 103).
- **Confondre description et contenu** — La description dit *quand*. Le corps dit *comment*. Ne mets pas de procédure dans la description, et ne mets pas de conditions de déclenchement dans le corps.

## Démonstration — d'une instruction à un skill

### Étape 1 — Le problème : une instruction trop large

Tu veux que Copilot utilise le format *Conventional Commits* pour les messages de commit. Premier réflexe : créer une instruction.

```diff
+ # .github/copilot-instructions.md
+ 
+ ## Messages de commit
+ 
+ Quand tu rédiges un message de commit :
+ - Utilise le format Conventional Commits : `type(scope): description`.
+ - Types autorisés : feat, fix, docs, style, refactor, test, chore.
+ - Première ligne < 72 caractères.
```

Le problème : cette instruction est chargée dans *toutes* les conversations. Quand tu demandes à Copilot d'expliquer un algorithme ou de générer un test, il traîne en contexte des règles sur les commits qui ne servent à rien — et qui consomment des tokens inutilement.

### Étape 2 — Extraire en skill

Supprime la section de l'instruction :

```diff
  # .github/copilot-instructions.md
  
  Tu es un assistant de code pour ce projet.
- 
- ## Messages de commit
- 
- Quand tu rédiges un message de commit :
- - Utilise le format Conventional Commits : `type(scope): description`.
- - Types autorisés : feat, fix, docs, style, refactor, test, chore.
- - Première ligne < 72 caractères.
```

Puis crée le fichier du skill :

```diff
+ # .agents/skills/writing-commit-message/SKILL.md
+ 
+ # writing-commit-message
+ 
+ Use when the user asks to write, draft, or generate a commit message.
+ 
+ ## Procedure
+ 
+ 1. Read the staged diff with `git diff --cached`.
+ 2. Write a commit message following Conventional Commits format:
+    - `type(scope): description` on the first line.
+    - Types allowed: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`.
+ 3. Keep the first line under 72 characters.
+ 4. Add a body only if the diff touches more than one concern.
```

Désormais, Copilot ne charge la procédure de commit que lorsque tu lui demandes explicitement de rédiger un message. Toutes les autres conversations restent légères.

### Étape 3 — Vérifier le déclenchement

Teste dans deux conversations séparées :

1. **Conversation A** — « Rédige un message de commit pour mes changements stagés. »
   → Copilot doit activer le skill et produire un message au format `type(scope): description`.

2. **Conversation B** — « Explique-moi cette fonction. »
   → Copilot ne doit **pas** charger le skill. Aucune mention de Conventional Commits dans la réponse.

Si le skill se déclenche dans la conversation B, ta description est trop large — affine-la.

### Étape 4 — Créer un skill avec assistance

Tu sais maintenant construire un skill à la main. En pratique, tu ne partiras pas toujours de zéro : des outils existent pour scaffolder ou affiner un skill de manière assistée.

**`writing-skills` (superpowers)** — Ce skill applique le **TDD au skill lui-même**. Le cycle est :

1. **RED** — lance un scénario de pression avec un sous-agent *sans* le skill. Observe comment l'agent échoue ou rationalise.
2. **GREEN** — rédige le `SKILL.md` qui corrige précisément les manquements observés.
3. **REFACTOR** — relance le scénario, repère les nouvelles rationalisations, bouche les failles, re-vérifie.

L'idée centrale : si tu n'as pas vu l'agent *échouer* sans le skill, tu ne sais pas si le skill enseigne la bonne chose.

Pour l'installer via APM (tu verras APM en détail au module 207) :

```bash
apm install superpowers-marketplace/superpowers/skills/writing-skills --target copilot
```

Une fois installé, demande simplement à Copilot « crée un skill pour [ta tâche] » — il chargera automatiquement le skill `writing-skills` et suivra le cycle RED → GREEN → REFACTOR.

**`/create-skill` (convention Anthropic)** — Claude Code propose la commande `/create-skill` qui scaffolde un `SKILL.md` en suivant la convention Anthropic :

- frontmatter YAML avec `name` et `description` (max 1024 caractères au total) ;
- description en troisième personne, commençant par « Use when… » — décrit uniquement les conditions de déclenchement ;
- structure du corps : `## Overview`, `## When to Use`, puis la procédure.

Les deux approches convergent sur le même principe : la `description` est un **trigger sémantique**, pas de la documentation.

**Quand utiliser quoi ?**

| Approche | Quand |
|---|---|
| Manuel (étapes 1–3) | Tu apprends, tu veux comprendre chaque décision |
| `writing-skills` (superpowers) | Tu veux valider que le skill marche vraiment (cycle TDD) |
| Convention Anthropic | Tu publies un skill dans un écosystème multi-agent Anthropic |

## Exercice ⭐⭐

**Énoncé** — Crée de zéro un skill `writing-commit-message` qui force le format Conventional Commits.

**Étapes guidées** :

1. Crée le dossier `.agents/skills/writing-commit-message/`.
2. Crée le fichier `SKILL.md` avec :
   - Une description qui précise *quand* le skill doit se déclencher.
   - Une procédure qui impose le format `type(scope): description`.
   - La liste des types autorisés.
   - La contrainte de 72 caractères sur la première ligne.
3. Stage quelques modifications dans ton dépôt (`git add .`).
4. Demande à Copilot : « Rédige un message de commit pour mes changements. »
5. Vérifie que le message respecte le format imposé.
6. Ouvre une nouvelle conversation et demande autre chose (par exemple « Explique ce fichier »). Vérifie que le skill ne se déclenche pas.

**Critère de réussite** : Copilot produit un message Conventional Commits quand on lui demande un commit, et ne mentionne pas ce format dans les autres conversations.

## Validation

Tu peux passer au module suivant si :

- [ ] Ton dépôt contient un fichier `.agents/skills/writing-commit-message/SKILL.md`.
- [ ] La description du skill décrit une situation de déclenchement, pas une documentation.
- [ ] Tu sais expliquer la différence entre instruction et skill en une phrase.
- [ ] Tu as vérifié que le skill s'active quand on demande un commit et reste inactif sinon.

## Pour aller plus loin

- [Module 104 — Agents personnalisés](./104-agents.md) : quand un skill ne suffit plus et qu'il faut packager un rôle complet avec identité et outils restreints.
- [Module 310 — Tester ses primitives](../03-ingenierie-de-contexte/310-evals.md) : mesurer objectivement si un skill se déclenche au bon moment avec des `eval` binaires.
- `docs/reference/skill-anatomy.md` — page de référence à créer.

## Module suivant

**Suivant** : [104 — Agents](./104-agents.md)
