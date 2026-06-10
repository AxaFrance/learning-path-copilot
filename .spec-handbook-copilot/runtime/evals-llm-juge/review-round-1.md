# Review round 1 — evals-llm-juge

**Date**: 2026-06-10
**Verdict**: CERTIFY

## Summary
Every one of the 27 citation blocks was independently re-fetched and matched verbatim against its source, with surrounding context supporting the claim in all cases — including the high-risk numerical claims (MT-Bench 85%/81% agreement, G-Eval Spearman 0.514), the named biases, the promptfoo `llm-rubric` JSON shape and threshold semantics, and the OpenAI text-similarity grader metric list. The draft correctly builds on module 310 without re-teaching binary evals, and frontmatter/navigation (`id`, `sidebar_position: 315`, `Suivant` → 311) are all correct. Two light pedagogical notes (Spearman, Likert used without explicit definition) and one borderline uncited reasoning claim do not breach the CERTIFY thresholds.

## Audit table

| ID | Type | Section | Claim (résumé FR) | Source cited | Status | Note |
|----|------|---------|-------------------|--------------|--------|------|
| C1 | citation | ## Pourquoi ce module | Benchmarks existants mesurent mal les préférences humaines | arxiv.org/abs/2306.05685 | VERIFIED | Verbatim in abstract |
| C2 | citation | ### Pourquoi le binaire ne suffit plus | LLM-based grading rapide/flexible/scalable | docs.anthropic.com/.../develop-tests | VERIFIED | Verbatim |
| C3 | citation | ### Les métriques graduées | output 'correct'/'incorrect' ou note 1-5 | anthropic develop-tests | VERIFIED | Verbatim |
| C4 | citation | ### Les métriques graduées | échelle 0.0–1.0, paliers smile/laughing | promptfoo llm-rubric | VERIFIED | Verbatim |
| C5 | citation | ### Les métriques graduées | grilles détaillées et claires | anthropic develop-tests | VERIFIED | Verbatim ("Have detailed, clear rubrics") |
| C6 | citation | ### Les métriques graduées (seuil) | score ≥ threshold pour passer | promptfoo llm-rubric | VERIFIED | Verbatim |
| C7 | citation | ### Le LLM-juge | strong LLMs as judges on open-ended questions | arxiv 2306.05685 | VERIFIED | Verbatim in abstract |
| C8 | citation | ### Le LLM-juge | llm-rubric = general-purpose grader "LLM as a judge" | promptfoo llm-rubric | VERIFIED | Verbatim |
| C9 | citation | ### Le LLM-juge | JSON {reason, score 0.0-1.0, pass} | promptfoo llm-rubric | VERIFIED | Verbatim subset of JSON object |
| C10 | citation | #### Trois manières (pointwise) | single answer grading assigns score to one answer | arxiv 2306.05685 | VERIFIED | Verbatim (body, §3.1) |
| C11 | citation | #### Trois manières (pairwise) | juge reçoit 2 réponses, désigne la meilleure / tie | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C12 | citation | #### Trois manières (reference) | reference-guided: fournir une solution de référence | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C13 | citation | #### Trois manières | text similarity graders mesurent proximité à la référence | platform.openai.com/docs/guides/graders | VERIFIED | Verbatim (ellipsis elides source typo "when to") |
| C14 | citation | #### Trois manières | liste fuzzy_match…rouge_l | openai graders | VERIFIED | Verbatim enum of `evaluation_metric` |
| C15 | citation | #### Trois manières (choisir) | pairwise scale mal, paires quadratiques | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C16 | citation | #### form-filling + CoT | G-Eval = LLM + CoT + form-filling | arxiv 2303.16634 | VERIFIED | Verbatim in abstract |
| C17 | citation | #### form-filling + CoT | Spearman 0.514 sur summarization, surpasse l'antérieur | arxiv 2303.16634 | VERIFIED | Verbatim (number exact) |
| C18 | citation | ### Les biais du juge | position, verbosity, self-enhancement biases | arxiv 2306.05685 | VERIFIED | Verbatim in abstract |
| C19 | citation | ### Les biais (position) | position bias = favorise certaines positions | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C20 | citation | ### Les biais (parade position) | double appel en inversant l'ordre | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C21 | citation | ### Les biais (verbosité) | verbosity bias = favorise réponses longues | arxiv 2306.05685 | VERIFIED | Verbatim (body) |
| C22 | citation | ### Les biais (self-preference) | juges favorisent leurs propres réponses | arxiv 2306.05685 | VERIFIED | Verbatim (body); paper's term is "self-enhancement" — see note P3 |
| C23 | citation | ### Les biais (self-preference) | G-Eval observe le biais pro-LLM-generated | arxiv 2303.16634 | VERIFIED | Verbatim in abstract |
| C24 | citation | ### Les biais (non-déterminisme) | few-shot augmente la consistance | arxiv 2306.05685 | VERIFIED | Verbatim; context (65.0%→77.5%) supports stability claim |
| C25 | citation | ### Calibrer le juge | accord GPT-4/humains 85% > humains 81% | arxiv 2306.05685 | VERIFIED | Verbatim (body); S2 w/o tie exact |
| C26 | citation | ### Calibrer le juge | tester la fiabilité avant de scaler | anthropic develop-tests | VERIFIED | Verbatim |
| C27 | citation | ### Où ça s'insère | tools: [] → agent ne peut que dialoguer | docs/01-fondations/104-agents.md | VERIFIED | Verbatim, line 103 |
| U1 | uncited | #### Trois manières (choisir) | "Pairwise est plus stable que pointwise" | — | UNCITED | Reasoning claim; scalability half cited (C15), stability half not directly sourced. Within tolerance |
| P1 | pedagogy | #### form-filling + CoT | terme "corrélation de Spearman" | — | PEDAGOGICAL-GAP | Statistical term not defined; contextually glossed as "avec l'humain". Mild |
| P2 | pedagogy | ### Les métriques graduées | terme "échelle de Likert" | — | PEDAGOGICAL-GAP | Not defined, but immediately glossed by "1–5". Mild |
| P3 | terminology | ### Les biais / Validation | draft labels bias "self-preference"; paper names it "self-enhancement" | arxiv 2306.05685 | NOTE | Citation accurate; "self-preference" is the draft's own synonym label. Not a citation error |

## Cross-reference & frontmatter checks (all pass)
- Frontmatter `id: evals-llm-juge` ✓ · `sidebar_position: 315` ✓ · title matches placement spec ✓
- `**Suivant**` → `./311-tokens-contexte.md` — target file exists ✓
- Module 310 "Pour aller plus loin" now links `./315-evals-llm-juge.md` (placeholder replaced) ✓
- Draft does **not** re-teach binary evals — frames itself as generalizing 310 ✓
- Prerequisites (310, 103, 104) match the placement plan ✓

## Findings (non-blocking, optional polish)
1. **U1** — Consider a one-clause hedge or citation for "pairwise est plus stable que pointwise". Optional.
2. **P1 / P2** — A 4-word inline gloss for "Spearman" and "Likert" would help a 310/103/104 reader. Polish, not a blocker.
3. **P3** — Optional: note the paper's term is "self-enhancement bias" when introducing "self-preference".

**Verdict rationale**: 0 UNSUPPORTED, 0 CHERRY-PICKED, 0 OUTDATED, 0 UNFETCHABLE, 1 UNCITED (≤2), 2 PEDAGOGICAL-GAP (≤2). All thresholds for CERTIFY are met.
