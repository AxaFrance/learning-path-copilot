# Lint — docs/_drafts/learning-path/00-setup-posture.md (Round 2)

**Verdict global** : PASS (avec warnings non bloquants)
**Score** : 9 / 9 sur les vérifications structurelles dures
**Références** : spec 01 (gabarit) + spec 11 (conventions)

---

## Failures

Aucune.

## Warnings (2, non bloquants)

### W1 — L10 — cohérence complexité ↔ contenu
L'en-tête déclare `Complexité : ⭐` (« Lecture passive, aucune commande à lancer ») mais l'exercice (L102–L112) impose d'installer une extension, créer trois fichiers, rédiger un `OBSERVATIONS.md` et basculer en agent mode. Cela correspond plutôt à ⭐⭐ (« Quelques commandes shell, un fichier à créer »). Arbitrage éditorial : soit passer en ⭐⭐ et étoffer la prose pour entrer dans 700–1200 mots, soit alléger l'exercice pour rester ⭐.

### W2 — L86–L96 — diffs git fichiers neufs sans contexte
Les diffs de la surface 3 (`fizzbuzz-agent.ts`, `fizzbuzz.test.ts`) sont des fichiers neufs présentés en `+` pur. Conforme à la règle (la ligne de contexte ne s'applique pas aux fichiers neufs), mais aérer avec une ligne de commentaire d'en-tête non préfixée homogénéiserait avec les diffs L37–L46 et L56–L65.

---

## Metrics

- Mots (prose hors code) : ~560 (cible ⭐ : 400–700)
- Frontmatter : 4/4 champs requis, description 122 c (≤ 160)
- En-tête : Durée / Complexité / Pré-requis + blockquote présents
- Sections obligatoires : 7/7 dans l'ordre canonique spec 01 §3
- Blocs ```diff : 5
- Mermaid : 0 (non requis)
- Emojis hors étoile/coche/croix : 0
- Tutoiement systématique, aucun vouvoiement, aucun marqueur anglais
- Lien interne : 1/1 valide (`./01-instructions-personnalisees.md`)
- Liens externes : 3 (docs.github.com, code.visualstudio.com)
- Anglicismes en italique 1ʳᵉ occurrence : inline suggestion, chat, agent mode, prompt, instruction

---

## Décision pipeline

PASS → exit loop. Le brouillon est éligible à la promotion vers `docs/learning-path/` après checkpoint humain (spec 12 §5) et passage éventuel du glossary keeper.
