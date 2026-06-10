---
id: patterns-sobriete
title: "312 — Patterns de sobriété"
sidebar_position: 312
description: "Appliquer des principes de design (DDD, Object Calisthenics, SOLID, agents scopés) qui réduisent naturellement la consommation de tokens."
---

# 312 — Patterns de sobriété

Durée estimée : 45 min · Complexité : ⭐⭐⭐ · Pré-requis : [Module 311](./311-tokens-contexte.md)

> Un code bien structuré consomme moins de tokens et produit de meilleures réponses. Ce module connecte tes principes de design à la sobriété LLM.

## Pourquoi ce module

Tu sais désormais ([Module 311](./311-tokens-contexte.md)) que chaque token a un coût, une latence et un effet sur la qualité. Mais la sobriété ne se joue pas qu'au niveau du modèle ou des outils — elle commence dans la structure même de ton code et de tes primitives.

Un domaine bien nommé économise des tokens d'explication. Du code découpé en petites unités cohérentes tient dans la fenêtre de contexte. Des interfaces minimales réduisent la surface d'hallucination. Des agents scopés maximisent le ratio signal/bruit.

Ce module t'apprend à transformer tes principes de design en leviers concrets de réduction de tokens.

À la fin de ce module, tu sais :

- utiliser l'Ubiquitous Language du DDD pour réduire les tokens d'explication ;
- appliquer les Object Calisthenics pour produire du code que l'IA comprend mieux ;
- exploiter SRP, ISP et DIP comme leviers de sobriété ;
- distinguer agent scopé et agent généraliste en termes de consommation ;
- auditer tes skills pour éliminer le bruit dans le routeur sémantique.

## Pré-requis

- [Module 311 — Tokens & fenêtre de contexte](./311-tokens-contexte.md) — tu dois comprendre pourquoi les tokens comptent.
- [Module 103 — Skills](../01-fondations/103-skills.md) — tu dois savoir créer et déclencher un skill.
- Un dépôt Git contenant du code applicatif (au moins 3 fichiers de plus de 100 lignes).

## Concepts clés

### 1 DDD et Ubiquitous Language

Un domaine bien nommé consomme moins de tokens d'explication. Compare ces deux signatures :

```typescript
// Générique — Copilot a besoin de contexte pour comprendre
function process(data: any[], config: object): object {}

// Ubiquitous Language — l'intention est dans le nom
function calculateShippingCost(order: Order, policy: ShippingPolicy): Money {}
```

Dans le premier cas, tu dois fournir un commentaire, un docstring, ou un prompt plus long pour que Copilot comprenne l'intention. Dans le second, le vocabulaire métier *est* le contexte. Le modèle infère la logique attendue directement depuis les noms.

L'`Ubiquitous Language` du DDD n'est pas qu'une bonne pratique de conception — c'est un levier de sobriété. Chaque concept métier nommé explicitement est un token d'explication économisé dans chaque interaction future.

### 2 Object Calisthenics

Les Object Calisthenics sont un ensemble de contraintes de style qui produisent du code à granularité fine :

- Petites classes (pas plus de 50 lignes).
- Un seul niveau d'indentation par méthode.
- Pas de `else` — utiliser le retour précoce ou le polymorphisme.
- Encapsuler les primitives dans des objets métier (`Money` au lieu de `float`).

Pourquoi c'est un levier de sobriété ? Parce que l'IA comprend mieux du code découpé en petites unités cohérentes. Un fichier de 500 lignes avec six niveaux d'indentation force le modèle à maintenir un graphe de dépendances mentales complexe — et il hallucine quand il perd le fil.

Un fichier de 40 lignes avec une seule responsabilité tient entièrement dans la fenêtre de contexte. Le modèle n'a pas besoin de re-prompting pour le comprendre.

### 3 SOLID, en particulier SRP / ISP / DIP

Trois principes SOLID ont un impact direct sur la consommation de tokens :

**SRP (Single Responsibility Principle)** — un fichier = une intention. Quand chaque fichier a une seule responsabilité, Copilot charge moins de contexte pour comprendre ce qu'il doit modifier. Un fichier `OrderService` de 800 lignes qui gère la validation, le calcul de prix et l'envoi de mails force le modèle à tout charger. Trois fichiers séparés lui permettent de ne lire que celui qui est pertinent.

**ISP (Interface Segregation Principle)** — des interfaces minimales. Une interface avec 15 méthodes est une surface d'hallucination : le modèle invente des implémentations pour des méthodes qu'il ne connaît pas. Une interface de 3 méthodes laisse peu de place à l'improvisation.

**DIP (Dependency Inversion Principle)** — des abstractions stables. Quand le code dépend d'abstractions (ports) plutôt que d'implémentations (adapters), Copilot suggère le bon contrat. Il n'a pas besoin de deviner si tu utilises PostgreSQL, MongoDB ou un mock en mémoire — il travaille avec l'interface, et la suggestion est correcte quel que soit l'adapteur derrière.

### 4 Agents scopés vs agents généralistes

Un `agent` scopé a une responsabilité unique, un jeu d'outils restreint et des instructions ciblées. Un agent généraliste est un couteau suisse qui essaie de tout faire.

```yaml
# Agent scopé — 120 tokens d'instructions
# .agents/commit-writer.agent.md
---
description: "Rédige des messages de commit conventionnels"
tools: ["githubRepo"]
---
Tu rédiges des messages de commit au format Conventional Commits.
Tu ne fais rien d'autre.
```

```yaml
# Agent généraliste — 2000+ tokens d'instructions
# .agents/dev-assistant.agent.md
---
description: "Aide au développement, review, commit, deploy, documentation..."
tools: ["githubRepo", "editFiles", "runTerminal", "browser", "codeSearch"]
---
Tu es un assistant de développement complet. Tu peux:
- rédiger du code
- faire des reviews
- écrire des commits
- déployer
- rédiger de la documentation
# ... 80 lignes d'instructions supplémentaires
```

Le premier agent charge 120 tokens d'instructions et dispose d'un seul outil. Le signal est maximal. Le second charge 2000+ tokens et ouvre 5 outils — le modèle hésite entre plusieurs interprétations à chaque requête, et les tokens d'instructions pertinents sont noyés dans le reste.

Règle : préfère toujours un agent scopé sauf au sommet de l'orchestration (le `workflow` racine du [Module 208](../02-composition/208-workflows.md)).

### 5 Trop de skills nuit

Chaque `skill` chargé dans l'espace de travail ajoute du bruit au routeur sémantique. Trois situations rendent un skill nuisible :

1. **Le skill ré-explique ce que le modèle sait déjà.** Un skill "écrire du code TypeScript propre" est de la pollution — le modèle sait déjà le faire. Le skill ajoute des tokens sans valeur ajoutée.

2. **Le skill se déclenche rarement.** Un skill qui ne s'active qu'une fois par mois occupe une place dans le routeur sémantique en permanence. Chaque évaluation de description par le routeur est un coût implicite.

3. **Le skill dégrade les résultats.** Un skill mal calibré peut forcer le modèle dans une direction sous-optimale.

L'heuristique est simple. Si tu mesures le delta `with_skill` / `without_skill` avec la méthode des evals ([Module 310](./310-evals.md)) et que le score `without_skill` atteint 80 % ou plus, le skill est probablement inutile. Le modèle fait déjà bien le travail sans lui.

## Mise en pratique

### Étape 1 — Identifier un fichier à refactorer

Prends un fichier de plus de 200 lignes dans ton projet. Identifie :

- Les noms génériques (`data`, `process`, `handler`, `utils`) → candidats au renommage DDD.
- Les classes à responsabilités multiples → candidats au découpage SRP.
- Les interfaces trop larges → candidats à la ségrégation ISP.

### Étape 2 — Refactorer pour la sobriété

Applique les principes de ce module :

- Renomme les fonctions génériques avec un vocabulaire métier explicite.
- Extrais les responsabilités multiples dans des fichiers séparés.
- Réduis les interfaces trop larges.

Demande ensuite à Copilot d'intervenir sur le fichier refactoré — observe si la qualité de suggestion s'améliore et si le prompt nécessaire est plus court.

### Étape 3 — Auditer un agent généraliste

Si tu as un agent généraliste dans ton workspace, mesure ses tokens d'instructions. Propose un découpage en agents scopés et estime la réduction de tokens par invocation.

## Pièges & anti-patterns

| Piège | Pourquoi c'est un problème | Solution |
|---|---|---|
| Renommer sans vocabulaire métier validé | Le renommage cosmétique ne réduit pas les tokens d'explication | Valider le vocabulaire avec le domaine avant de renommer |
| Découper trop finement | Des fichiers de 5 lignes ajoutent du bruit de navigation | Viser 30–80 lignes par fichier, pas moins |
| Supprimer des skills sans mesurer le delta | Un skill peut sembler inutile mais avoir un delta significatif | Toujours mesurer `with_skill` / `without_skill` avant de supprimer |
| Confondre agent scopé et agent limité | Un agent scopé est expert, pas handicapé | Le scope restreint les outils et le domaine, pas la qualité |

## Exercice ⭐⭐⭐

### Énoncé

1. Choisis un fichier de plus de 200 lignes dans ton projet.
2. Applique 3 principes de ce module (DDD, Object Calisthenics, SRP).
3. Mesure avant/après : nombre de lignes, nombre de tokens estimés, qualité des suggestions Copilot (subjective, sur 5).
4. Si tu as des skills, mesure le delta `with_skill` / `without_skill` pour 3 d'entre eux et classe-les (garde / fusionne / supprime).

### Résultat attendu

| Métrique | Avant | Après |
|---|---|---|
| Lignes du fichier | 280 | 3 × 60 |
| Tokens d'explication nécessaires | 150 | 40 |
| Qualité suggestion Copilot | 3/5 | 4/5 |

## Validation

Tu as réussi ce module si :

- Tu as refactoré au moins un fichier en appliquant les principes DDD, Object Calisthenics et SOLID, avec une réduction documentée du nombre de lignes et de tokens.
- Tu sais expliquer pourquoi un agent scopé consomme moins de tokens qu'un agent généraliste.
- Si tu as des skills, tu as mesuré le delta pour au moins 3 et documenté ta décision (garde / fusionne / supprime).

## Pour aller plus loin

- **Object Calisthenics en pratique** — applique les 9 règles à un module existant de ton projet et mesure l'impact sur la qualité des suggestions Copilot avant/après refactoring.
- [Module 313 — Outils de réduction](./313-outils-reduction.md) — les outils qui implémentent la sobriété au niveau technique.
- [Module 311 — Tokens & fenêtre de contexte](./311-tokens-contexte.md) — le cadre théorique de la sobriété.

## Sources

- Eric Evans, *Domain-Driven Design: Tackling Complexity in the Heart of Software* — le chapitre Ubiquitous Language.
- Jeff Bay, "Object Calisthenics" in *The ThoughtWorks Anthology* — les 9 règles originales.
- Robert C. Martin, *Clean Architecture* — les principes SOLID appliqués à l'architecture logicielle.
