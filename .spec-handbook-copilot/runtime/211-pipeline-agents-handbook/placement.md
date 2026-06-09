# Placement — pipeline-agents-handbook

**Date**: 2026-05-28
**Verdict**: INSERT

## Position
Inséré en fin du bloc `02-composition`, après `210-copilot-cli.md`. Slug `pipeline-agents-handbook`, numéro proposé `211`. Le module se positionne comme capstone du bloc Composition : il prend les briques d'orchestration introduites en 208 et les montre vivantes sur le pipeline qui fabrique le handbook lui-même.

## Justification
Le catalogue (spec 02) range l'orchestration multi-agents dans le bloc 2 (Composition), avec `208-workflows` comme module fondateur du pattern *orchestrateur → sous-agents*. Ce chapitre n'est pas une nouvelle primitive (donc pas bloc 1) ni une optimisation de tokens (donc pas bloc 3) : c'est une étude de cas appliquée des concepts du bloc 2, doublée d'un guide contributeur. Aucune collision avec un module existant — `208-workflows` reste le module conceptuel, `211` devient l'exemple incarné, ce qui justifie INSERT plutôt que REPLACE. Pas besoin d'une nouvelle track : il s'agrège naturellement aux tracks D (Architecte d'agents) et E (Complet).

## Pré-requis
- `103-agents` (notion de `.agent.md`, délégation via l'outil `agent` — base du trio architect/writer/reviewer)
- `104-skills` (les sous-agents s'appuient sur des skills procéduraux — base du *quand/quoi/comment*)
- `208-workflows` (pattern orchestrateur + sous-agents, anti-pattern super-agent — le module rendu concret ici)
- `207-apm` (pour comprendre comment le pipeline pourrait être distribué/installé par un contributeur)

## Complexité
intermédiaire

## Word count cible
2500-3500

## Path du draft
`docs/learning-path/02-composition/211-pipeline-agents-handbook.md`
