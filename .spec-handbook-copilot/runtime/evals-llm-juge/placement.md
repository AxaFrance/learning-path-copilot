# Placement — evals-llm-juge

**Date**: 2026-06-10
**Verdict**: INSERT

## Position
Entre `310-evals.md` et `311-tokens-contexte.md` dans le bloc `03-ingenierie-de-contexte` (« Ingénierie de contexte »), slug `evals-llm-juge`, numéro proposé `315`.

Le module prend `sidebar_position: 315`. Ce numéro est libre depuis le renumérotage de l'ancien `315-evals` en `310-evals`, et le module 310 conserve déjà un `redirect_from: /docs/03-ingenierie-de-contexte/315-evals.html` — le nouveau chapitre reprend donc un emplacement vacant sans collision d'historique.

## Justification
Le module 310 (« Tester ses primitives — evals binaires ») cite explicitement ce chapitre dans sa section « Pour aller plus loin » : *« Evals avancées (module à venir) : passer des assertions binaires aux évaluations par LLM-juge et aux métriques de qualité graduées »* — exactement le topic verbatim. Le chapitre prolonge donc 310 en passant du booléen `pass/fail` aux scores gradués et au LLM-juge ; 310 est son pré-requis strict et doit le précéder. Le numéro 315 le place après 310 tout en restant cohérent avec le bloc sobriété (311–318), qui traite d'un sujet orthogonal (tokens/coût) : les evals avancées appartiennent à la famille « test des primitives » de 310, pas à la chaîne sobriété, et 315 referme proprement ce sous-thème avant que 311 n'ouvre la séquence tokens. Insérer plus loin (ex. après 318) casserait la proximité pédagogique avec 310 et l'ordre des pré-requis annoncés par 311, qui dépend de 310 mais pas des evals avancées.

## Pré-requis
- `310-evals` (raison : il faut maîtriser les fixtures, les assertions binaires et le delta `with_skill`/`without_skill` avant de les généraliser en scores gradués)
- `103-skills` (raison : le sujet du test reste un `skill` — savoir le créer et le déclencher)
- `104-agents` (raison : le LLM-juge est lui-même un agent évaluateur configuré via `.agent.md`)

## Complexité
avancé

## Word count cible
2500-3500

## Path du draft
`docs/03-ingenierie-de-contexte/315-evals-llm-juge.md`

---

**Pour le writer** — slug requis : `evals-llm-juge` · path requis : `docs/03-ingenierie-de-contexte/315-evals-llm-juge.md` · frontmatter attendu : `id: evals-llm-juge`, `title: "315 — Evals avancées (LLM-juge & métriques graduées)"`, `sidebar_position: 315`. Mettre à jour le lien « module à venir » de 310 vers ce path et insérer un lien « Module suivant » cohérent.
