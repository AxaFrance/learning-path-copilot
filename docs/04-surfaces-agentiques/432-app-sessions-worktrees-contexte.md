---
id: app-sessions-worktrees-contexte
title: "432 — GitHub Copilot app : sessions, worktrees et contexte"
sidebar_position: 432
description: "Isoler les sessions avec des git worktrees, choisir Current checkout ou New worktree, et fournir le bon contexte avec @, # et /."
---

# 432 — GitHub Copilot app : sessions, worktrees et contexte

**Durée** : ~35 min · **Complexité** : ⭐⭐ · **Pré-requis** : [431 — Modes, modèles et permissions](./431-app-modes-modeles-permissions.md)

> *Une session sérieuse mérite son propre espace de travail. Les worktrees empêchent deux agents de mélanger leurs diffs dans le même checkout.*

## Objectif

À la fin de ce module, tu sais :

- expliquer ce qu'est un **git worktree** ;
- choisir entre **New worktree** et **Current checkout** ;
- lancer plusieurs sessions en parallèle sans mélanger les branches ;
- ajouter du contexte avec `@`, `#` et `/` ;
- identifier les limites d'isolation : ports, bases locales, processus ;
- mener deux sessions worktree en parallèle sur `samples/mon-sinistre`.

## Ce que tu vas apprendre

1. Session, branche, worktree
2. New worktree vs Current checkout
3. Travailler en parallèle sans collision
4. Attacher le bon contexte : `@`, `#`, `/`
5. Exercice : deux sessions isolées

## Contenu pédagogique

### Session, branche, worktree

Un **git worktree** est un autre dossier de travail connecté au même dépôt Git. Il peut être positionné sur une autre branche que ton checkout principal.

| Terme | Définition débutant |
|---|---|
| **Session** | Espace de travail Copilot app avec conversation, plan, diff, terminal et historique |
| **Branche** | Ligne de travail Git dédiée à une correction ou une exploration |
| **Worktree** | Dossier séparé lié au même dépôt, souvent associé à une branche de session |

> Source : [copilot-app-for-beginners — appendices/git-worktrees.md](https://github.com/github/copilot-app-for-beginners/blob/main/appendices/git-worktrees.md).

Avec un worktree, ton dossier principal peut rester propre pendant qu'une session corrige D1 et qu'une autre analyse D4. Les diffs, terminaux et aperçus restent attachés à la bonne session.

### Où la session travaille

Quand tu démarres une session, le sélecteur de workspace propose plusieurs options.

| Option | Ce que ça veut dire | À choisir quand |
|---|---|---|
| **New worktree** | Copilot crée un dossier et une branche dédiés | C'est le défaut sûr pour les exercices avec modification |
| **Current checkout** | Copilot travaille dans ton clone actuel | Visite rapide, exploration sans changement, ou setup très contrôlé |
| **Cloud** | Copilot travaille dans un sandbox hébergé par GitHub si disponible | Tu veux isoler la machine locale ou déléguer une tâche lourde |

Dans cette série, utilise **New worktree** dès qu'une modification est possible. **Current checkout** était acceptable au module 430 parce que la consigne était explicitement « ne modifie aucun fichier ».

### Le parallèle a des limites

Les worktrees isolent les **fichiers** et les **branches**. Ils n'isolent pas automatiquement :

- les ports (`http://localhost:5213`) ;
- les bases locales ;
- les conteneurs ;
- les caches ;
- les variables d'environnement ;
- les processus déjà lancés.

Si deux sessions lancent `MonSinistre.Web` sur le port 5213, une seule peut l'utiliser. Pour une deuxième preview, demande à Copilot d'utiliser un port différent ou arrête le premier serveur.

### Ajouter du contexte

L'app fournit trois raccourcis essentiels dans le prompt.

| Syntaxe | Sert à | Exemple Mon Sinistre |
|---|---|---|
| `@` | attacher fichiers ou dossiers | `@samples/mon-sinistre/src/MonSinistre.Domain` |
| `#` | attacher issues ou PR | `#` puis choisir une issue créée par `seed.sh` |
| `/` | lancer une commande ou action de l'app | `/context`, `/chronicle standup`, `/rubber-duck` si disponible |

Le bon contexte est **petit et pertinent**. Attacher tout le dépôt pour une règle métier simple surcharge inutilement la session. Pour D1, `CalculIndemnisation.cs` et les tests suffisent souvent. Pour D4, il faut comparer le domaine et le CLI.

### Vérifier la session

Les informations de session indiquent :

- le nom de branche ;
- le chemin du worktree ;
- le projet ;
- l'identifiant de session ;
- parfois l'état du contrôle à distance ou du sandbox.

Prends l'habitude de les lire avant de lancer un serveur ou d'accepter un diff. Si tu ne sais pas dans quel dossier l'agent travaille, tu ne sais pas vraiment ce que tu valides.

## Exercice

**Énoncé** : lance deux sessions en parallèle, chacune dans son worktree, sans modifier `main`.

### Session A — D1, indemnisation négative

1. Démarre une session sur ton projet avec **New worktree**.
2. Mets le mode sur **Plan**.
3. Envoie :

```text
Analyse le défaut D1 dans @samples/mon-sinistre.
Le sinistre SIN-1004 affiche une indemnisation négative alors qu'elle ne doit jamais être inférieure à 0.
Propose un plan de correction et de validation. Ne modifie aucun fichier.
```

4. Note le nom de branche et le chemin du worktree.

### Session B — D4, formatage dupliqué

1. Sans fermer la session A, démarre une deuxième session avec **New worktree**.
2. Mets le mode sur **Plan**.
3. Envoie :

```text
Analyse le défaut D4 dans @samples/mon-sinistre.
Cherche les endroits où le formatage des montants est dupliqué entre le domaine et le CLI.
Propose un plan de factorisation et les validations à lancer. Ne modifie aucun fichier.
```

4. Note le nom de branche et le chemin du deuxième worktree.

### Comparaison

1. Vérifie que les deux sessions ont des branches différentes.
2. Vérifie que les deux chemins de worktree sont différents.
3. Ouvre le panneau **Changes** de chaque session : aucun diff ne doit être présent, car tu n'as demandé qu'une analyse.
4. Dans chaque session, envoie `/context` si disponible et observe la quantité de contexte attachée.

## Validation

- [ ] Tu peux expliquer ce qu'un worktree isole et ce qu'il n'isole pas.
- [ ] Les sessions A et B ont deux branches différentes.
- [ ] Les sessions A et B ont deux chemins de worktree différents.
- [ ] Tu as utilisé `@` pour attacher un dossier pertinent.
- [ ] Aucun diff n'a été créé.
- [ ] Tu sais pourquoi deux previews web peuvent entrer en conflit sur le port 5213.

## Pour aller plus loin

- [433 — Boucle de développement](./433-app-boucle-developpement.md)
- [Git worktrees — appendice du cours GitHub](https://github.com/github/copilot-app-for-beginners/blob/main/appendices/git-worktrees.md)
- [Slash commands for the GitHub Copilot app](https://docs.github.com/en/copilot/reference/github-copilot-app-reference/slash-commands)

## Source

- [github/copilot-app-for-beginners — 02 Sessions, Worktrees, and Context](https://github.com/github/copilot-app-for-beginners/blob/main/02-sessions-worktrees-context/README.md)
- [github/copilot-app-for-beginners — Git Worktrees appendix](https://github.com/github/copilot-app-for-beginners/blob/main/appendices/git-worktrees.md)
- [GitHub Docs — Working with agent sessions](https://docs.github.com/en/copilot/how-tos/github-copilot-app/agent-sessions)
