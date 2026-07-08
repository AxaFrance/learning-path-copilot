---
id: chronicle-audit-cout-cli
title: "319 — Chronicle expérimental — tirer parti de son historique de sessions"
sidebar_position: 319
description: "Utiliser Chronicle, de manière expérimentale via Copilot CLI, pour relire son historique de sessions, distinguer un conseil prouvé d'une hypothèse, et transformer un constat en règle durable."
---

# 319 — Chronicle expérimental — tirer parti de son historique de sessions

**Durée** : ~30 min · **Complexité** : ⭐⭐ · **Pré-requis** : [210 — Copilot CLI](../02-composition/210-copilot-cli.md), [318 — Mesurer & optimiser sa consommation](./318-mesurer-optimiser-consommation.md)

> ⚠️ **Module expérimental dans notre contexte — usage via Copilot CLI.**
> Chronicle existe sur plusieurs surfaces (GitHub Copilot app, github.com, VS Code, JetBrains). Ici on l'aborde **dans notre pratique actuelle, de manière expérimentale via Copilot CLI**. Utilise-le comme outil d'analyse et d'apprentissage — **pas comme source budgétaire officielle**. Tu verras plus bas pourquoi : ses propres données de coût peuvent avoir plusieurs semaines de retard.

> *Chronicle transforme ton historique de sessions en matière exploitable : résumés, recommandations, pistes d'amélioration, instructions projet. Sa valeur n'est pas dans « comment il est construit », mais dans ce qu'il t'aide à faire après coup : comprendre ce que tu rejoues, ce que tu paies plusieurs fois, et ce que tu devrais encoder durablement.*

## Objectif

À la fin de ce module, tu sais :

- utiliser Chronicle pour interroger ton historique de sessions ;
- choisir le bon usage selon ton besoin : `standup`, conseils, recherche, amélioration d'instructions ;
- lire une réponse Chronicle avec esprit critique, en distinguant un constat **mesuré** d'une **hypothèse** ;
- transformer un constat utile en action durable pour ton projet.

## Ce que tu vas apprendre

1. À quoi sert Chronicle (démontré par un cas réel de `standup`)
2. Quand l'utiliser
3. Comment lire une bonne réponse (la grille « mesuré vs déduit »)
4. Comment transformer un résultat en règle durable

## Contenu pédagogique

### À quoi sert Chronicle

Chronicle sert à **exploiter l'historique de tes sessions Copilot**. Le meilleur exemple observé est `standup` : à partir de ton activité récente, il reconstruit un compte-rendu « fait / en cours ».

Ce qui rend l'outil crédible, c'est qu'il est **transparent sur sa mécanique**. Il lit un **store local de sessions en SQL** (tables `sessions`, `turns`, `events`, `session_refs`) puis croise ces sessions avec tes PR GitHub. Il affiche même ses requêtes :

```text
Sessions récentes sur 24h (session history)
│ SELECT id, repository, branch, summary, agent_name, created_at … FROM sessions …
└ 13 lignes

Refs des sessions récentes (session history)
│ SELECT session_id, ref_type, ref_value, turn_index FROM session_refs WHERE …
└ 0 ligne          ← session_refs VIDE
```

Concrètement, sur les dernières 24h il a récupéré **13 sessions**, les a rattachées à des PR réelles (via `gh pr list`) par timestamp et similarité de contenu, groupées **par branche**, et a produit :

```text
Standup du 2026-07-08 :
✅ Fait
  - Recompilation du pipeline pour un modèle retiré (fix/pipeline-model) — Mergée #94 …
  - Écriture d'état + config repo-wide (feature/token-reduce) — Mergée #92 …
  - Passe de revue complète du catalogue (copilot/review-catalogue) — Mergée #18 …
🚧 En cours
  - Fiche produit 14, section conversationnelle (aucune branche dédiée) — Aucune PR trouvée …
  - Nommage du catalogue, page d'accueil + pipeline de publication — Aucune PR trouvée …
```

Retiens deux choses. D'abord, ce mécanisme prouve que **Chronicle s'appuie sur un store de sessions, pas sur ta facturation** : ce n'est pas une source de coût officielle. Ensuite, la ligne « Fiche produit 14 … Aucune PR trouvée » est déjà un signal : une session de travail **rejouée mais jamais aboutie en PR**. C'est exactement le motif que tu vas apprendre à repérer.

### Quand l'utiliser

Chronicle devient utile quand tu as déjà un peu d'historique. Utilise-le par exemple quand :

- tu as enchaîné plusieurs sessions sur le même sujet ;
- tu sens que certains problèmes reviennent ;
- tu veux savoir quoi formaliser dans `AGENTS.md`, `.github/copilot-instructions.md`, un `skill` ou un `agent` ;
- tu veux faire un point rapide sur ce que tu as fait récemment.

Ne l'utilise pas seul pour mesurer une facture officielle, auditer une micro-session isolée, ou remplacer une analyse fine session par session. Pour ça, garde aussi [318 — Mesurer & optimiser sa consommation](./318-mesurer-optimiser-consommation.md).

### Comment l'utiliser dans notre pratique

Dans notre contexte, tu lances Chronicle depuis **Copilot CLI** avec une intention claire :

```bash
/chronicle standup      # résumer le travail récent
/chronicle tips         # conseils personnalisés sur tes habitudes
/chronicle cost-tips    # pistes de réduction de coût / tokens
/chronicle improve      # ce qu'il faut renforcer dans les instructions projet
/chronicle search "…"   # retrouver un fil de travail déjà fait
/chronicle reindex      # (re)construire l'index local des sessions
```

Le plus important n'est pas la commande. C'est **ce que tu demandes à Chronicle de prouver**. Les 5 `tips` produits sur notre historique en sont un bon modèle : chacun cite une session, un prompt ou un compte concret.

1. **🔁 Templatiser les dispatches répétés de génération de fiches produit** — preuve : 15+ prompts quasi identiques tapés à la main, des limites de mots qui dérivent (500 / 900 / 1500), et des doublons (fiche 5 renvoyée 3×, fiche 6 renvoyée 2×). Fix : un `skill`/`template` paramétré (fiche + limite de mots).
2. **⚡ Paralléliser les lots indépendants** — preuve : 15 fiches dispatchées **en série**, une session à la fois, sur ~2 heures. Des tâches indépendantes se lancent en parallèle.
3. **🏗️ Porter le pattern chaîne d'agents d'un repo à l'autre** — preuve : le `repo-plateforme` a déjà, sous `.github/agents/`, une **chaîne d'agents rédacteur + relecteur** ; le `repo-catalogue` fait le même type de travail sans ce scaffolding.
4. **✍️ Affûter les prompts sous-spécifiés** — preuve : des messages comme « Génère markdown », « Génère la fiche dans le dossier », « @copilot on peut mentionner les avis clients » = aucun critère d'acceptation → rounds de correction. Fix : un `/plan` d'abord, ou un « done when » en une ligne.
5. **📊 Vérifier où passent tes sessions interactives** — preuve : le store ne contient **aucune** session CLI locale → l'outil suggère lui-même `/chronicle reindex` ou `/session` pour vérifier la synchronisation.

Chaque `tip` pointe un motif répété, une session concrète, un fichier, ou une limite de données clairement signalée. C'est le contre-modèle du conseil générique.

### Comment lire une bonne réponse Chronicle

La commande `cost-tips` illustre la seule grille de lecture qui compte. Elle sépare explicitement ce qui est **mesuré** (`Exact usage`) de ce qui est **déduit** (`proxy`), et **auto-évalue sa confiance**.

Voici la couverture qu'elle a affichée sur notre historique :

```text
Couverture de l'analyse
- Source : usage réel de tokens (exact usage rows)
- Disponibilité : 6 069 events avec usage, mais uniquement 2025-11-26 → 2026-05-21
- Zéro usage sur les 7+ dernières semaines (malgré 96 sessions depuis)
- Périmètre : 2 types d'agents (Copilot Coding Agent, Copilot Code Review), aucune ligne CLI interactive
- Confiance : élevée sur la fenêtre historique — mais ce rapport ne peut rien dire de ton
  profil de coût actuel : traite les constats comme directionnels, pas comme un état courant.
```

Cette dernière ligne est décisive : Chronicle **dit lui-même** que ses données de coût s'arrêtent au 21/05 et qu'il y a 7+ semaines de trou. C'est la preuve directe que **ce n'est pas une vérité budgétaire**.

Ses 4 principaux postes de coût sont ensuite **étiquetés par force de preuve** :

| Poste de coût | Chiffre réel | Étiquette |
|---|---|---|
| Epics dispatchés en une seule session — « Epic — refonte du tunnel de paiement » | 122–164 tours, 13–16M input chacun (top 2 = 30,9M) | **Exact usage** |
| Pas de compaction en cours de session | 164 events, input/tour 65k → 153k, monotone | **Exact usage** (cause confirmée via `proxy`) |
| Coding Agent domine Code Review ~99:1 | 477,9M vs 5,5M input | **Exact usage** |
| Opus sur tâches larges | opus 212M (2 044 appels) vs sonnet 246M (3 439 appels) | Volume **exact** ; « aurait pu coûter moins » = `proxy` |

**La règle : un constat `Exact usage` est mesuré, tu peux décider dessus ; un constat `proxy` est une hypothèse, tu dois la valider avant d'agir.** Un bon conseil cite sa source.

Ses recommandations suivent une structure Contexte / Problème / Action / Impact — reproductible telle quelle dans tes propres notes :

```text
1. Découper les epics en petites issues avant dispatch          — Impact : High
   Contexte : les 2 plus grosses sessions = 30,9M d'input à elles seules.
   Problème : relation quasi-linéaire tours ↔ tokens, prouvée par l'usage réel.
   Action   : découper avant de déléguer ; un /plan en amont.

2. Sonnet par défaut, Opus réservé au travail étroit à fort enjeu — Impact : Medium
   (le volume est exact ; « fallait-il Opus » reste un proxy à valider)

3. Re-checker une tâche déléguée après ~50–60 tours              — Impact : Medium
   (la croissance est prouvée ; le seuil d'intervention est un jugement)
```

À l'inverse, méfie-toi d'un « compacte plus », « prends un modèle moins cher » ou « ouvre une nouvelle session » si rien ne prouve pourquoi ce conseil s'applique à **ton** historique.

> **Règle simple : pas de preuve, pas de décision.**

### Ce qu'on a appris de nos premiers essais

Sur nos premiers usages, Chronicle a surtout rendu visible **ce qui est rejoué**. La commande `search "fiche"` a retourné **46 résultats** en une seule requête, et les doublons sautent aux yeux — sans aucun calcul :

- « Fiche 5 … (retry) » à 22:09, 22:03, 21:44 ;
- « Fiche 6 … (retry) » ×3 ;
- un même plan de cadrage **re-dérivé plusieurs fois** (« plan 40–50 pages » ×2, « plan 10–15 pages » ×2).

Une seule recherche rassemble tout un fil de travail. Et le rejoué devient une matière première directe : les libellés `(retry)` et les plans dupliqués prouvent le motif « même réflexion rejouée » — exactement ce qu'un `skill` de génération de fiche paramétré aurait supprimé.

À noter aussi : dans cette sortie, le champ `repository` est **null partout**, et Chronicle **reconstruit** le repo par heuristique (à partir du 1er message). C'est cohérent avec `session_refs` vide et le backfill en cours — donc à lire avec esprit critique.

### Que faire après un résultat utile

La commande `improve` est la machine à **transformer un constat en règle durable**. Elle scanne les sessions d'un repo, cherche les violations récurrentes de règles, les croise avec tes fichiers d'instructions (`AGENTS.md`, `.github/copilot-instructions.md`), et propose des ajouts ciblés.

Le constat réel obtenu, transposé à notre app e-commerce : une règle métier — *« toujours valider le panier côté serveur avant paiement »* — est documentée dans **un seul doc d'équipe**, mais **absente du `.github/copilot-instructions.md`** (le fichier toujours chargé). Résultat : l'agent l'a **oubliée dans 2 sessions sur 5**, sous pression, lors de correctifs urgents. `improve` détecte le motif, remonte la cause (règle absente du fichier toujours chargé), et propose l'action durable : **l'ajouter aux instructions toujours chargées**, avec un fallback explicite « si tu ne peux pas l'appliquer, laisse une note plutôt que de deviner ».

C'est le bon réflexe : ne pas archiver le constat, mais l'**encoder** là où il sera systématiquement rechargé. Le workflow :

1. garde **un ou deux constats maximum** (ceux qui reviennent ou qui coûtent le plus) ;
2. reformule chacun en une règle courte ;
3. écris-la dans `.github/copilot-instructions.md`, `AGENTS.md`, un `skill` ou un `agent` ;
4. vérifie au prochain passage de Chronicle si le motif a disparu.

### Pré-requis et limites à connaître

Chronicle dépend de la **complétude de ton index local**, et cet index n'est pas garanti complet. Les faits observés :

- `reindex` a rendu : **325 sessions indexées, 462 en file de backfill, 17 échecs** → à un instant donné, la vue peut être **partielle** ;
- `session_refs` a renvoyé **0 ligne** → certaines tables sont vides, cohérent avec le backfill en cours ;
- le champ `repository` est **null** et **reconstruit par heuristique** → l'info affichée est une déduction ;
- les données d'`usage` sont **périmées** : 6 069 events, mais uniquement du 2025-11-26 au 2026-05-21, avec **7+ semaines de trou** → « directional, not current-state » ;
- sur un plan **individuel** (non-Business, sans sync admin), **aucune session CLI interactive** n'apparaît → la vue peut ignorer une partie de ton travail local.

Conclusion : Chronicle est un **outil d'exploration guidée**, pas une vérité budgétaire. Pour Copilot Business ou Enterprise, un administrateur peut devoir activer la synchronisation des sessions locales ; tes sessions restent **privées par défaut**.

### Pièges à éviter

- **Confondre `Exact usage` et `proxy`** — décide sur le mesuré, valide le déduit.
- **Traiter Chronicle comme une mesure officielle** — ses données de coût peuvent avoir 7+ semaines de retard.
- **Croire l'index complet** — backfill en attente, `session_refs` vide, `repository` reconstruit.
- **Vouloir tout corriger d'un coup** — prends 1 ou 2 motifs à fort levier (ex. découper les epics, impact High).
- **Ne rien écrire après l'audit** — sans règle durable dans le fichier toujours chargé, le même coût reviendra.

## Exercice

**Énoncé** — En 20 minutes, tu vas utiliser Chronicle pour transformer un constat en amélioration durable.

**Étapes guidées** :

1. Lance `/chronicle reindex` puis note ce qu'il rapporte (indexées / backfill / échecs) — ta vue est-elle complète ?
2. Lance `/chronicle cost-tips` (ou `search "…"`) et repère **un** constat réellement appuyé par tes traces.
3. Vérifie son étiquette : est-il `Exact usage` (mesuré) ou `proxy` (à valider) ?
4. Reformule ce constat en une règle courte, et écris-la dans `.github/copilot-instructions.md` (le fichier toujours chargé), un `skill` ou un `agent`.
5. Note ce que tu vérifieras au prochain passage de Chronicle (le motif a-t-il disparu ?).

**Critère de réussite** ⭐ — tu repars avec **une amélioration concrète écrite**, issue d'un constat `Exact usage` réellement prouvé.

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais à quoi sert Chronicle et sur quoi il s'appuie (store local de sessions en SQL, pas la facturation).
- [ ] Tu sais quand le lancer, et pour quel besoin (`standup`, `tips`, `cost-tips`, `improve`, `search`).
- [ ] Tu sais distinguer un constat `Exact usage` (mesuré) d'un `proxy` (hypothèse à valider).
- [ ] Tu sais nommer au moins deux limites réelles (index partiel, `usage` périmé, `repository` null…).
- [ ] Tu sais transformer un constat Chronicle en règle durable, écrite dans le fichier d'instructions toujours chargé.

## Pour aller plus loin

- [Module 210 — Copilot CLI](../02-composition/210-copilot-cli.md) — surface d'exécution de Chronicle dans notre pratique actuelle.
- [Module 318 — Mesurer & optimiser sa consommation](./318-mesurer-optimiser-consommation.md) — vérifier une intuition de coût sur une session précise.
- [Module 312 — Patterns de sobriété](./312-patterns-sobriete.md) — réduire le coût par le design du travail.
- [Module 313 — Outils de réduction](./313-outils-reduction.md) — éviter de gonfler le contexte inutilement.

## Source

Retour d'expérience interne sur l'usage réel des 6 commandes `/chronicle` (`reindex`, `standup`, `tips`, `cost-tips`, `search`, `improve`), complété par l'annonce GitHub : *« Gain insights across your agent sessions with /chronicle »* (GitHub Changelog, 2 juin 2026).
