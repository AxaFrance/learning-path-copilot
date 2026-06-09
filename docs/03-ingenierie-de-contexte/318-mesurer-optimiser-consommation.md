---
id: mesurer-optimiser-consommation
title: "318 — Mesurer & optimiser sa consommation"
sidebar_position: 318
description: "On n'optimise que ce qu'on mesure. Les Agent Debug Logs montrent la consommation réelle de tokens ; le Cache Explorer révèle quelle part est servie par le cache et comment les prompts sont structurés."
---

# 318 — Mesurer & optimiser sa consommation

**Durée** : ~45 min · **Complexité** : ⭐⭐⭐ · **Pré-requis** : [107 — Choix de modèles](../01-fondations/107-choix-de-modeles.md), [311 — Tokens & contexte](./311-tokens-contexte.md)

> *On n'optimise que ce qu'on mesure. Les Agent Debug Logs montrent votre consommation réelle de tokens, session par session ; le Cache Explorer révèle quelle part est servie par le cache — donc bien moins chère — et comment vos prompts sont structurés sous le capot.*

## Objectif

À la fin de ce module, tu sais :

- Lire les Agent Debug Logs pour voir ta consommation réelle de tokens, session par session.
- Interpréter le taux de hit de cache et identifier où le cache casse.
- Lire la signature du prompt (système, outils, messages) pour comprendre ce qui reste en cache.
- Garder un préfixe stable pour maximiser les hits de cache et réduire la facture.

## Ce que tu vas apprendre

1. Les Agent Debug Logs : visualiser la consommation réelle
2. Le Cache Explorer : performance du cache et point de rupture
3. La signature du prompt : ce qui change vs ce qui reste en cache
4. La règle : maximiser les hits de cache = facture réduite

## Contenu pédagogique

### Les Agent Debug Logs — visualiser sa consommation réelle

Les Agent Debug Logs te donnent un snapshot par session. Voici un exemple réel (session locale, 0 erreur) :

| Métrique | Valeur |
|---|---|
| Tours du modèle | 26 |
| Appels d'outils | 21 |
| Tokens d'entrée | 1 159 021 |
| Tokens de sortie | 21 653 |
| Tokens d'entrée en cache | 1 016 681 |
| Tokens au total | 1 180 674 |
| Erreurs | 0 |
| Usage Copilot (AIC) | 115,93 |

La ligne qui compte :

> **1 016 681 / 1 159 021 tokens d'entrée servis par le cache — ≈ 88 % de l'entrée déjà payée une fois.**

Autrement dit, sur 1,16 M de tokens d'entrée, ≈ 88 % sont réutilisés depuis le cache — donc bien moins chers. La consommation visible (AIC : 115,93) est tirée par les 12 % restants et par les tokens de sortie. **C'est là que se joue l'optimisation.**

### Le Cache Explorer — où le cache casse

Le Cache Explorer montre la performance du cache requête par requête et révèle le point exact où il casse.

```text
PERFORMANCE DU CACHE
Requête courante : 77,33 % de hit de cache   (requête précédente : 96,63 %)
33 746 / 43 638 tokens d'entrée réutilisés
```

Le taux a chuté de 96,63 % à 77,33 %. Pourquoi ? Le Cache Explorer le dit :

```text
OÙ LE CACHE CASSE — Au premier message
Message utilisateur redimensionné à 22 767 caractères
→ tout ce qui suit ce point repart au plein tarif
Perdu : 9 892 tokens (22,66 %)
0 identique · 7 modifiés · 28 supprimés
```

Une modification au **premier message** (`messages[0]`) a invalidé tout ce qui suit. Résultat : 9 892 tokens (22,66 %) repassent au plein tarif. **Plus la rupture est en amont, plus elle coûte cher.**

### La signature du prompt

La signature du prompt montre ce qui change d'une requête à l'autre — et ce qui reste en cache. Trois zones : **système**, **outils**, **messages**.

```text
Légende : système 92 188 · outils 377 951 · messages (en cache) · messages (cassé)

PRÉCÉDENTE  (551 578 car.)  | système 92 188 | outils 377 951 | messages en cache 81 439
COURANTE    (498 592 car.)  | système 92 188 | outils 377 951 | messages cassé    28 453
→ 470 139 / 498 592 caractères réutilisés — rupture à messages[0]
```

Le préfixe stable (système + outils = 470 139 caractères) est réutilisé intégralement. La rupture est localisée dans les **messages**, au tout début (`messages[0]`). Tout ce qui précède la rupture est gratuit ; tout ce qui suit repasse au plein tarif.

### Maximiser les hits de cache = facture réduite

> **Maximisez les hits de cache = facture réduite.** Gardez le préfixe stable : système + catalogue d'outils + début de conversation. Toute modification en amont (éditer un ancien message, réinjecter un gros bloc) casse le cache — et tout ce qui suit le point de rupture repasse au plein tarif.

La règle pratique :

- **Ne touche pas au préfixe.** Système et catalogue d'outils doivent rester identiques d'une requête à l'autre.
- **N'édite pas un ancien message.** Modifier `messages[0]` invalide toute la suite.
- **Évite de réinjecter de gros blocs en amont.** Ajoute le nouveau contexte à la fin, pas au début.

## Exercice

**Énoncé** — En 20 minutes, tu vas auditer une session réelle et identifier ta plus grosse rupture de cache.

**Étapes guidées** :

1. Ouvre les Agent Debug Logs sur une session active. Note le ratio *tokens d'entrée en cache / tokens d'entrée*.
2. Ouvre le Cache Explorer et compare le hit de cache de la requête courante à la précédente.
3. Si le taux a chuté, repère où le cache casse (quel `messages[N]`).
4. Identifie l'action qui a causé la rupture (édition d'un ancien message, gros bloc réinjecté).
5. Refais la même tâche en gardant le préfixe stable et compare le hit de cache.

**Critère de réussite** : tu as identifié une rupture de cache concrète et réduit sa perte de tokens sur une seconde passe.

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais ouvrir les Agent Debug Logs et lire le ratio de tokens servis par le cache.
- [ ] Tu sais interpréter un taux de hit de cache et repérer où il casse.
- [ ] Tu sais lire la signature du prompt (système / outils / messages).
- [ ] Tu sais nommer trois actions qui cassent le cache et comment les éviter.

## Pour aller plus loin

- [Module 311 — Tokens & contexte](./311-tokens-contexte.md) — le coût d'un token au-delà de la facture.
- [Module 312 — Patterns de sobriété](./312-patterns-sobriete.md) — structurer son code pour consommer moins.
- [Module 313 — Outils de réduction](./313-outils-reduction.md) — filtrer la sortie shell avant le contexte.

## Source

Contenu issu du retour d'expérience d'**Ousmane BARRY**, *MVP Microsoft Foundry* — guide pratique « Développement assisté par IA » ([publication LinkedIn](https://www.linkedin.com/posts/ousmanebarry_depuis-le-1er-juin-on-a-tous-vu-et-senti-ugcPost-7468560023623901184-K69e/)).
