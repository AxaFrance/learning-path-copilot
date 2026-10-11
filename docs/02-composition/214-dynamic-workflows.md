---
id: dynamic-workflows
title: "214 — Dynamic workflows — orchestrer en code"
sidebar_position: 214
description: "Passer d'un orchestrateur en prose à un dynamic workflow Copilot : la séquence en code, les agents là où il faut du jugement, des checkpoints humains."
---

# 214 — Dynamic workflows — orchestrer en code

Durée estimée : 60 min

> Dans le module 208, l'orchestrateur relit sa procédure à chaque run et décide, en prose, quel sous-agent appeler. Un *dynamic workflow* écrit cette procédure en code : le modèle ne travaille plus que là où il faut du jugement.

## Pourquoi ce module

Reprends l'orchestrateur `issue-to-code` du [module 208](./208-workflows.md). Sa séquence est fixe : `spec-writer`, puis `plan-writer`, puis `coder`. Pourtant, à chaque run, c'est un modèle de langage qui relit ces consignes, décide de l'étape suivante, et recopie la sortie d'un sous-agent dans le prompt du suivant. Tu paies du raisonnement pour exécuter quelque chose qui ne demande aucun jugement.

L'ADR-010 de SKRAFT (branche `integration/v2`, statut *Proposed* au moment de la rédaction) décrit ce coût : le pipeline tenait dans un agent en prose de 29 Ko, et chaque run payait la relecture de ce protocole.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "Every run spent reasoning tokens re-reading its protocol, calling `state.mjs` one subcommand at a time, and pasting handoff blocks."
> Fetched: 2026-10-11

La conclusion de l'ADR-010 est nette : une séquence déterministe n'a pas besoin d'un modèle pour s'exécuter.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "A deterministic sequence no longer needs a language model to execute it."
> Fetched: 2026-10-11

Une fois la séquence passée en code, le modèle ne travaille plus que dans les agents de phase.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "The sequence costs no reasoning tokens; the language model works only inside the phase agents."
> Fetched: 2026-10-11

À la fin de ce module, tu sais :

- expliquer ce qu'est un dynamic workflow et le distinguer d'Autopilot et de `/fleet` ;
- dire **où** il tourne (et où il ne tourne pas) ;
- extraire la séquence d'un orchestrateur en prose vers une extension Copilot (`defineWorkflow`, `ctx.agent`, `ctx.pause`) ;
- identifier quels hooks un dynamic workflow rend inutiles, et lesquels restent indispensables ;
- lancer un workflow depuis le chat ou depuis le terminal avec `copilot workflow run`.

## Pré-requis

- [Module 104 — Agents personnalisés](../01-fondations/104-agents.md) — les agents que le workflow va piloter.
- [Module 105 — Hooks](../01-fondations/105-hooks.md) — tu vas voir quels hooks disparaissent.
- [Module 208 — Workflows](./208-workflows.md) — l'orchestrateur en prose, point de départ de la conversion.
- [Module 210 — Copilot CLI](./210-copilot-cli.md) — la surface d'exécution principale.
- Copilot CLI installé.

## Concepts clés

### Qu'est-ce qu'un dynamic workflow ?

Un dynamic workflow est un **programme**. Il décrit comment une tâche se déroule : des étapes automatisées (lire un fichier, lancer une commande, appeler un service) et, entre elles, le travail d'un ou plusieurs agents.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "A dynamic workflow is a program that defines how a task is carried out."
> Fetched: 2026-10-11

La répartition des rôles est explicite : le code décide des étapes et de l'usage des résultats ; les agents traitent ce qui demande analyse ou jugement.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "Everything is defined in code: the steps, when to involve agents, and how to use their results."
> Fetched: 2026-10-11

Un workflow peut aussi s'arrêter à un **checkpoint** pour que tu relises les résultats, puis reprendre quand tu es prêt.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "Pause at a checkpoint so that you can review results and resume the run when you're ready."
> Fetched: 2026-10-11

Concrètement, le workflow vit dans une **extension Copilot** : un fichier `extension.mjs` qui l'enregistre auprès de la session.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "Dynamic workflows are defined in Copilot extensions."
> Fetched: 2026-10-11

Pour le partager avec ton équipe, tu places le dossier qui contient `extension.mjs` dans `.github/extensions/` du dépôt (ou dans `~/.copilot/extensions/` pour toi seul).

> Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
> Citation: "copy the directory containing the `extension.mjs` file into the `.github/extensions/` directory of your local copy of the repository."
> Fetched: 2026-10-11

### Autopilot, `/fleet` ou dynamic workflow ?

Les trois font travailler des agents, mais ils ne répartissent pas le contrôle de la même façon. La différence tient à **qui définit le processus**.

| Aspect | Autopilot | `/fleet` | Dynamic workflow |
|---|---|---|---|
| But principal | Laisser Copilot travailler en autonomie | Pousser Copilot à déléguer à des sous-agents | Exécuter un processus défini en code |
| Qui définit le processus ? | Copilot décide des étapes suivantes | Copilot décide comment découper et coordonner | L'auteur du workflow définit étapes, conditions et transmissions |
| Déroulement | Selon la tâche et les décisions de Copilot | Le travail indépendant peut être délégué en parallèle | Séquentiel, parallèle ou les deux, avec un ou plusieurs agents |

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "A dynamic workflow carries out a process defined in code. The workflow's author defines the steps, conditions, and handoffs."
> Fetched: 2026-10-11

Retiens aussi que le chat ne lance jamais un workflow de sa propre initiative : il faut le lui demander, ou qu'un skill ou une commande slash le fasse.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "Your chat agent only creates or runs a dynamic workflow when you explicitly ask it to, or when a skill or slash command instructs it to do so."
> Fetched: 2026-10-11

### Prose (module 208) vs code (ce module)

Dans le module 208, l'orchestrateur est un `.agent.md` dont l'outil clé est `runSubagent`, et qui transmet les résultats « en langage naturel ».

> Source: [208 — Workflows](./208-workflows.md)
> Citation: "L'orchestrateur n'a qu'un seul outil spécifique : `runSubagent`. Il coordonne la séquence et transmet le résultat d'un sous-agent au suivant."
> Fetched: 2026-10-11

Le dynamic workflow déplace exactement cette responsabilité dans du JavaScript. Le SDK fournit `ctx.agent(prompt, options)` pour lancer un sous-agent, et ses options permettent de cibler un agent nommé (`agent`), un modèle (`model`) ou d'exiger une sortie structurée (`schema`).

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Runs one workflow-owned subagent. Options are exactly `label`, `schema`, `model`, `agent`, `reasoningEffort`, and `contextTier`."
> Fetched: 2026-10-11

| | Orchestrateur en prose (208) | Dynamic workflow (214) |
|---|---|---|
| Où vit la séquence | Corps du `.agent.md` | `run(ctx)` dans `extension.mjs` |
| Qui l'exécute | Un modèle, à chaque run | Le runtime de l'extension |
| Lancer un sous-agent | `runSubagent` décidé par le modèle | `await ctx.agent(...)` |
| Passer le résultat | Le modèle recopie la sortie dans le prompt suivant | Une variable JavaScript |
| Validation humaine | Une consigne « demande confirmation » | `await ctx.pause(key)`, durable et reprenable |
| Surveiller l'ordre | Un hook externe (ex. `PreToolUse`) | Un `if` avant chaque `ctx.agent` |
| Hôtes | Tout hôte qui lit `.agent.md` | Copilot app, Copilot CLI, Copilot SDK |

:::warning Compatibilité — à lire avant d'investir
**La documentation officielle liste trois surfaces de disponibilité : la Copilot app, Copilot CLI et le Copilot SDK.**

- **Déduction : non compatible avec Claude Code.** Ce n'est pas une phrase de la documentation GitHub, mais une conclusion tirée de deux prémisses sourcées ci-dessous :
  1. la liste officielle de disponibilité se limite à la Copilot app, Copilot CLI et le Copilot SDK — Claude Code n'y figure pas ;
  2. pour faire tourner le même pipeline sous Claude Code, SKRAFT compose **un second adaptateur**, un *mod Claude Code* séparé (`hooks/skraft-mod.mjs`), autour du même cas d'usage hôte-neutre `RunPipeline` : l'extension Copilot n'y est donc pas exécutée.

  Un *mod Claude Code* est un module de plugin exécuté à l'intérieur de la session Claude Code, qui pilote les agents par du code via des API comme `$.agent.spawn` ou `$.ui.ask`.
- **VS Code :** au moment de la rédaction (octobre 2026), VS Code n'apparaît pas dans la liste de disponibilité officielle. Ne compte pas dessus tant que la documentation ne le mentionne pas.
- **Preview :** la fonctionnalité est en *public preview*, et l'API du SDK est déclarée expérimentale. Sous Copilot CLI, il faut activer les fonctionnalités expérimentales (`--experimental` ou `/experimental on`).
- **Plans :** les abonnés Copilot Pro et Pro+ restés sur un plan annuel à facturation par *premium requests* (ancien modèle) n'y ont pas accès.
:::

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "Dynamic workflows are available in the Copilot app, Copilot CLI, and Copilot SDK."
> Fetched: 2026-10-11

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "except Copilot Pro and Copilot Pro+ subscribers on existing annual plans who remain on legacy premium request-based billing."
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "Claude Code mods (`$.agent.spawn`, `$.ui.ask`, `$.fs`, `$.process.run`, `$.state`) run plugin modules inside the session"
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "The hosts only compose: the Claude Code mod (`hooks/skraft-mod.mjs`) and the Copilot extension"
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Le même cas d'usage sert deux hôtes : un mod Claude Code et un workflow dynamique GitHub Copilot."
> Fetched: 2026-10-11

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Dynamic Workflows are extension-authored, session-scoped workflows that coordinate subagents and durable steps. The API is experimental."
> Fetched: 2026-10-11

> Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
> Citation: "you must enable experimental features by running the CLI with the `--experimental` command-line option, or by using `/experimental on`"
> Fetched: 2026-10-11

### Un dynamic workflow peut remplacer certains hooks

Le [module 105](../01-fondations/105-hooks.md) t'a montré les hooks comme garde-fous déterministes. Dans SKRAFT, plusieurs hooks n'existaient **que pour surveiller l'orchestrateur en prose** : rien d'autre ne l'empêchait de se tromper d'ordre ou d'oublier une entrée.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "G1 refused a phase agent dispatched out of order, G9 refused a dispatch whose prompt dropped a recorded input"
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "G6 reminded the orchestrator, after each agent returned, what to record next."
> Fetched: 2026-10-11

Quand c'est le code qui dispatche, ces vérifications deviennent de simples tests exécutés **avant** chaque `ctx.agent`, sur l'état que le code a lui-même écrit. Le comportement *fail-closed* est conservé : en cas de refus, le run s'arrête et rien n'est envoyé. Plus besoin de hook.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "A refusal stops the run `blocked`; nothing is sent. The checks stay fail-closed, as ADR-004 required, without a hook."
> Fetched: 2026-10-11

G6, lui, disparaît purement et simplement : il n'y a plus personne à qui rappeler quoi enregistrer, puisque le code enregistre lui-même.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/site/fr/explanation/hooks.md
> Citation: "G6 (le rappel PostToolUse des étapes à enregistrer) a disparu : le code enregistre artefacts et verdicts."
> Fetched: 2026-10-11

**La limite est importante.** Le workflow ne voit que ce qu'il fait lui-même : choisir l'agent, composer le prompt. Il ne voit pas ce que l'agent fait **à l'intérieur** de son tour — quels fichiers il écrit, quels skills il charge. Ces gardes-là restent des hooks.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/site/fr/explanation/hooks.md
> Citation: "Les hooks gardent ce qu'aucun chemin de code ne voit — les écritures des agents (G7/G8), les skills qu'ils chargent (G2/G3), qui dispatche qui (provenance)."
> Fetched: 2026-10-11

## Démonstration

Cette démonstration suit **la démarche de la branche v2 de SKRAFT, pas à pas**. Les extraits sont simplifiés pour tenir sur une page, mais ils respectent l'API du SDK. Quand un extrait est une simplification pédagogique et non le code réel du dépôt, il est marqué `// pseudo-code`.

### Étape 1 — Point de départ : un orchestrateur en prose, gardé par des hooks

Le pipeline SKRAFT enchaîne quatre phases (RESEARCH → DESIGN → DISTILL → DELIVER). Avant la v2, un agent en prose, `skraft-orchestrator`, portait la séquence, et des hooks la surveillaient de l'extérieur :

- **G1** (`PreToolUse`) : refuse un agent de phase lancé hors ordre ;
- **G9** (`PreToolUse`) : refuse un dispatch dont le prompt a perdu une entrée enregistrée ;
- **G6** (`PostToolUse`) : rappelle à l'orchestrateur ce qu'il doit enregistrer après chaque agent.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "The pipeline was carried by a 29 KB prose agent, `skraft-orchestrator`."
> Fetched: 2026-10-11

```jsonc
// pseudo-code — forme simplifiée du câblage, pas le hooks.json réel de SKRAFT
{
  "hooks": {
    "PreToolUse":  [{ "type": "command", "command": "node src/cli/hook.mjs" }],
    "PostToolUse": [{ "type": "command", "command": "node src/cli/hook.mjs" }]
  }
}
```

Le point clé : ces hooks n'existent que parce qu'**aucun code** ne se trouve entre la prose et un mauvais dispatch.

### Étape 2 — Extraire la définition du pipeline en code

Premier geste : sortir l'ordre des phases de la prose. Dans la v2, il vit dans un module du domaine, sans aucune dépendance, et l'agent lanceur ne le déclare plus.

```diff
+ // plugins/skraft-framework/src/domain/pipeline/pipeline-definition.mjs
+ export const PIPELINE_PHASES = Object.freeze(['RESEARCH', 'DESIGN', 'DISTILL', 'DELIVER'])
+ export const PIPELINE_DISPATCHER = 'skraft-pipeline'
+ export const PIPELINE_LAUNCHER = 'skraft-orchestrator'
+
+ export const isPipelineDispatcher = (reference) => reference === PIPELINE_DISPATCHER
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/src/domain/pipeline/pipeline-definition.mjs
> Citation: "The SKRAFT engineering pipeline, declared where it runs: in code"
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/src/domain/pipeline/pipeline-definition.mjs
> Citation: "Agents no longer declare it — the launcher agent only starts it."
> Fetched: 2026-10-11

Ce fichier a été introduit par le commit [`07214d4`](https://github.com/SebastienDegodez/skraft-plugin/commit/07214d4e1c8898fd6182e60f2ad21a89d3f20b32). Côté prose, l'agent de phase ne déclare plus « dispatché par l'orchestrateur », mais `dispatched_by: skraft-pipeline` : c'est le code qui dispatche.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Le pipeline est déclaré dans le code"
> Fetched: 2026-10-11

### Étape 3 — Un cas d'usage hôte-neutre, derrière des ports

Trois mots de vocabulaire d'abord. Un **port** est un contrat (une interface) dont le cas d'usage a besoin pour agir sur le monde extérieur ; un **adaptateur** est l'implémentation de ce contrat propre à un hôte donné ; un **cas d'usage hôte-neutre** est le code qui porte les règles et ne connaît que les ports, jamais l'hôte qui l'exécute.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Chaque hôte ne fait que brancher ses adaptateurs sur les ports ; toutes les décisions sont prises dans le domaine et les cas d'usage"
> Fetched: 2026-10-11

Avant de toucher au SDK Copilot, la v2 écrit la boucle **sans dépendre d'aucun hôte**. `RunPipeline` séquence les phases et parle au monde à travers des *ports* : `AgentRunner` pour lancer un agent, `HumanInteraction` pour poser une question, `PipelineProgress` pour signaler l'avancement, et une quinzaine d'autres pour l'état, git ou les fichiers.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Écrire un adaptateur par port propre à l'hôte : en général `AgentRunner`, `HumanInteraction` et `PipelineProgress`"
> Fetched: 2026-10-11

```js
// pseudo-code — esquisse simplifiée de RunPipeline (le vrai fichier est
// src/application/pipeline/run-pipeline.mjs)
import { PIPELINE_PHASES } from '../../domain/pipeline/pipeline-definition.mjs'

export const createRunPipeline = ({ agentRunner, humanInteraction, progress, state }) => ({
  async run({ slug, story }) {
    for (const phase of PIPELINE_PHASES) {
      progress.phase(phase)
      const agent = specialistFor(phase)
      const result = await agentRunner.run({ agent, label: `${phase}-specialist`, prompt: brief(phase, state) })
      if (!result.ok) return { status: 'blocked', phase, reason: result.error }
      // … reviewer, retries, checkpoints via humanInteraction.ask(...)
    }
    return { status: 'done' }
  },
})
```

Le bénéfice : ce cas d'usage se teste avec des doubles en mémoire, et **chaque hôte n'a plus qu'à brancher ses adaptateurs**. Le mod et l'extension ne portent aucune règle.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "One implementation for two hosts, unit-tested with in-memory doubles and an integration test per host."
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Le mod et l'extension ne contiennent aucune règle : ils construisent les dépendances et appellent un cas d'usage."
> Fetched: 2026-10-11

### Étape 4 — L'adaptateur Copilot : `defineWorkflow` + `joinSession`

Côté SDK, le principe tient en deux appels : `defineWorkflow` déclare le workflow, `joinSession` l'enregistre dans la session.

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Use `defineWorkflow` and pass the returned handle to `joinSession`"
> Fetched: 2026-10-11

Voici l'extension réelle de la v2, raccourcie. Elle enregistre en un seul `joinSession` le workflow, deux outils (`skraft_decide`, `skraft_close_phase`) et un *canvas* — une interface custom fournie par une extension dans la Copilot app :

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "In the Copilot app, an extension can provide a canvas—a custom interface—with controls to start the workflow."
> Fetched: 2026-10-11


```js
// plugins/skraft-framework/com.github.copilot/extensions/skraft-pipeline/extension.mjs (extrait)
import { CanvasError, createCanvas, defineWorkflow, joinSession } from '@github/copilot-sdk/extension'

const skraftPipeline = defineWorkflow({
  meta: SKRAFT_PIPELINE_META,
  run: (ctx) => runSkraftPipelineWorkflow(ctx, host),
})

session = await joinSession({
  workflows: [skraftPipeline],
  tools: [
    createSkraftDecideTool({ ...host, cwd: () => process.cwd() }),
    createSkraftClosePhaseTool({ ...host, cwd: () => process.cwd() }),
  ],
  canvases: [skraftCanvas],
})
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/com.github.copilot/extensions/skraft-pipeline/extension.mjs
> Citation: "registers with the SDK the `skraft-pipeline` dynamic workflow, the `skraft_decide` and `skraft_close_phase` tools, and the `skraft-pipeline` canvas"
> Fetched: 2026-10-11

`runSkraftPipelineWorkflow` construit les adaptateurs Copilot des ports. Les deux qui comptent pour toi : `AgentRunner` s'appuie sur `ctx.agent`, `HumanInteraction` sur `ctx.pause`. En simplifié :

```js
// pseudo-code — adaptateurs simplifiés (les vrais sont dans
// src/adapters/infrastructure/copilot-workflow/)
const agentRunner = (ctx) => ({
  async run({ agent, label, prompt }) {
    const text = await ctx.agent(prompt, { agent, label })
    return text === null ? { ok: false, error: `${label}: no result` } : { ok: true, text }
  },
})

const humanInteraction = (ctx) => ({
  async ask({ key, question }) {
    ctx.log(`${question} (clé : ${key})`)
    await ctx.pause(key) // 1re tentative : le run se met en pause ici
    return null          // à la reprise : la réponse est relue dans le DecisionStore (le port qui enregistre les réponses humaines)
  },
})
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "HI->>W: ctx.log(question, clé) puis ctx.pause(clé)"
> Fetched: 2026-10-11

Trois comportements du SDK expliquent ce code :

1. **Un échec ordinaire de `ctx.agent` ne lève pas d'exception, il rend `null`.** D'où le test avant tout usage du résultat.
2. **`ctx.pause` est durable.** À la reprise, le workflow repart du début, mais les étapes journalisées ne sont pas ré-exécutées : leur résultat est relu depuis le journal.
3. **`ctx.pause` ne s'appelle que depuis le flux principal**, jamais dans `ctx.parallel()` ou `ctx.pipeline()`.

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "An ordinary failure resolves to `null` — it does not throw."
> Fetched: 2026-10-11

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "On resume, the workflow starts from the beginning, reuses the journaled step, returns from the checkpoint, and continues."
> Fetched: 2026-10-11

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Call it only from the main workflow flow, not inside `ctx.parallel()` or `ctx.pipeline()`."
> Fetched: 2026-10-11

Pour les agents déjà terminés avant la pause, SKRAFT s'appuie sur le journal du SDK : ils ne sont pas relancés.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Les `ctx.agent` déjà terminés ne sont pas relancés à la reprise : le SDK rejoue leur résultat journalisé (même prompt et même `label`)."
> Fetched: 2026-10-11

Et côté Claude Code ? Le même `RunPipeline` est branché sur un mod Claude Code, avec `$.ui.ask` à la place de `ctx.pause` : c'est le second adaptateur sur lequel repose la déduction de l'encadré *Compatibilité* plus haut.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Claude Code : boîte de dialogue. `HumanInteraction` ouvre la question avec `$.ui.ask` et le pipeline continue dans la même exécution."
> Fetched: 2026-10-11

### Étape 5 — Déplacer G1/G9 dans le cas d'usage, supprimer les hooks

Maintenant que c'est le code qui dispatche, la vérification d'ordre et celle de complétude du handoff s'exécutent juste avant l'appel :

```diff
  // pseudo-code — RunPipeline, avant chaque dispatch
+ const order = evaluateDispatch(state, { phase, agent })        // G1
+ if (!order.ok) return { status: 'blocked', phase, reason: order.reason }
  const prompt = composeDispatchBrief(state, phase)
+ const handoff = evaluateHandoff(state, prompt)                  // G9
+ if (!handoff.ok) return { status: 'blocked', phase, reason: handoff.reason }
  const result = await agentRunner.run({ agent, label: `${phase}-specialist`, prompt })
+ recordArtifacts(state, result)                                  // remplace le rappel G6
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "Avant chaque dispatch : `evaluateDispatch` (G1 — l'agent appartient à la phase ouverte, le reviewer seulement après un artefact)"
> Fetched: 2026-10-11

Puis on retire les hooks devenus inutiles du câblage :

```diff
  // pseudo-code — câblage des hooks après ADR-010
  "PreToolUse": [
-   G1  ordre de dispatch
-   G9  complétude du handoff
    provenance du dispatch
    G7  état suivi protégé
    G8  écritures DELIVER
  ],
  "PostToolUse": [
-   G6  rappel des étapes à enregistrer
    G3  trace des chargements de skills
  ]
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "Provenance, G7/G8, G2/G3 and housekeeping stay: they watch what agents do, which no code path sees."
> Fetched: 2026-10-11

Le tableau avant/après croise deux sources : la colonne « Avant » reprend l'ADR-010 (section *Context* et point 3 de la *Decision*), la colonne « Après » la documentation des hooks SKRAFT, modifiée par le commit [`e21ffbc`](https://github.com/SebastienDegodez/skraft-plugin/commit/e21ffbc4a1b352fa6581285aa6772aa973f6ae12) :

| Garde | Avant (orchestrateur en prose) | Après (dynamic workflow) |
|---|---|---|
| G1 — ordre de dispatch | Hook `PreToolUse`, fail-closed | `RunPipeline`, avant chaque dispatch ; fail-closed, le run s'arrête `blocked` |
| G9 — complétude du handoff | Hook `PreToolUse` | `RunPipeline`, sur le prompt composé ; fail-closed |
| G6 — rappel de continuation | Hook `PostToolUse` | Supprimé : le code enregistre ce que rend l'agent |
| Provenance du dispatch | Hook `PreToolUse` | Reste un hook |
| G2/G3 — skills obligatoires et chargés | Hooks `SubagentStart`, `PostToolUse`, `SubagentStop` | Restent des hooks |
| G7/G8 — état suivi, écritures DELIVER | Hook `PreToolUse` | Restent des hooks |

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "ADR-004 made G1 a fail-closed `PreToolUse(Agent)` hook because nothing else stood between the prose and a wrong dispatch."
> Fetched: 2026-10-11

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/site/fr/explanation/hooks.md
> Citation: "G1 (ordre de dispatch) et G9 (complétude du handoff) tournent dans le cas d'usage avant chaque dispatch, sur l'état qu'il a lui-même écrit"
> Fetched: 2026-10-11

L'ADR résume le gain en une phrase : ce qui était un conseil autour de la prose devient un invariant du code.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "Guards that were advisory around prose are invariants of the code that dispatches."
> Fetched: 2026-10-11

### Étape 6 — Lancer : l'orchestrateur devient un lanceur

Deux façons de démarrer le même workflow. Dans le chat, en langage naturel :

```text
Run the skraft-pipeline dynamic workflow for slug checkout, issue 42
```

Ou directement depuis le terminal, avec les entrées en JSON :

```bash
copilot workflow run skraft-pipeline --args '{"slug":"checkout","issue":42}'
```

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/com.github.copilot/extensions/skraft-pipeline/extension.mjs
> Citation: "copilot workflow run skraft-pipeline --args '{"slug":"checkout","issue":42}'"
> Fetched: 2026-10-11

> Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
> Citation: "Use `--args` to supply inputs for the workflow as JSON."
> Fetched: 2026-10-11

Le run se suit avec `/workflows` dans une session Copilot CLI : tu y vois la phase courante, les sous-agents actifs et les crédits consommés. `P` met en pause, `R` reprend un run en pause.

> Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
> Citation: "In an active run, press P to pause, X to cancel the run."
> Fetched: 2026-10-11

Et l'ancien `skraft-orchestrator` ? Il ne disparaît pas : il devient un **lanceur**. Il démarre le workflow et relaie les checkpoints, sans plus rien décider du pipeline.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md
> Citation: "L'agent `skraft-orchestrator` n'est plus qu'un lanceur : il appelle l'entrée de son hôte et relaie les checkpoints."
> Fetched: 2026-10-11

## Pièges et anti-patterns

- **Oublier que c'est une preview.** Fonctionnalité en *public preview*, API du SDK déclarée expérimentale : signatures et comportements peuvent changer. Épingle ta version de Copilot CLI et garde un test d'intégration de ton extension.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "Copilot dynamic workflows and extensions are in public preview"
> Fetched: 2026-10-11

- **Croire que le workflow sera portable.** Ni Claude Code ni (au moment de la rédaction) VS Code ne figurent dans la liste officielle de disponibilité ; l'encadré *Compatibilité* plus haut en déduit qu'il faut un second adaptateur pour Claude Code. Si ton équipe l'utilise, prévois donc un cas d'usage hôte-neutre, comme `RunPipeline`, pour ne pas dupliquer la logique.

- **Lancer un agent à la main, hors workflow.** Les gardes déplacées dans le code ne protègent que les dispatchs qui passent par le code. Un agent de phase lancé directement depuis le chat échappe à G1/G9 ; seules la provenance et G7/G8, restées en hooks, s'appliquent encore.

> Source: https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md
> Citation: "A user who dispatches phase agents by hand, outside RunPipeline, is no longer stopped by G1/G9; provenance and G7/G8 still apply."
> Fetched: 2026-10-11

- **Sous-estimer les permissions.** Les sous-agents du workflow héritent des autorisations de ta session : ce que tu as déjà accordé s'applique à eux. Et `copilot workflow run` n'affiche aucune demande d'approbation : accorde les permissions (`--allow-tool`, `--allow-url`) **avant** de lancer, sinon les demandes sont refusées.

> Source: https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows
> Citation: "A dynamic workflow's subagents use the CLI's permission system and inherit the permission grants of the session that started them."
> Fetched: 2026-10-11

> Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
> Citation: "This command does not display permission approval prompts, even when you run it in a terminal."
> Fetched: 2026-10-11

- **Appeler deux fois `ctx.agent` avec le même prompt en attendant deux agents.** Les appels identiques sont mémoïsés en **un seul** sous-agent. Donne un `label` unique à chaque appel indépendant.

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Identical calls are memoized into one subagent."
> Fetched: 2026-10-11

- **Fixer des limites au hasard.** Un plafond (`maxAiCredits`, `maxTotalSubagents`…) deviné coupe un run sain en plein milieu, après qu'il a déjà dépensé des crédits. Fais d'abord un run réduit, mesure, puis fixe la limite.

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "Set a ceiling only from real knowledge of what the workflow costs, or because the user named one."
> Fetched: 2026-10-11

- **Garder en hook une vérification que le code peut faire.** *Retour d'expérience terrain, pas un fait documenté :* un hook `PreToolUse` ou `PostToolUse` est relancé **à chaque appel d'outil**. Déclaré en `node hook.mjs`, il paie à chaque fois le démarrage d'un processus Node ; sur une session qui enchaîne des dizaines d'outils, ce surcoût s'additionne et ralentit nettement le système. Si la vérification ne porte que sur l'ordre d'orchestration, elle a sa place dans le workflow, pas dans un hook. Le [module 105](../01-fondations/105-hooks.md) détaille ce piège et ses alternatives.

## Exercice ⭐⭐

**Énoncé** — Convertis l'orchestrateur `issue-to-code` du [module 208](./208-workflows.md) (`spec-writer` → `plan-writer` → `coder`) en dynamic workflow, avec un checkpoint humain **avant** l'étape de code.

**Étapes guidées** :

1. Garde tes trois sous-agents `.agent.md` du module 208 tels quels.
2. Active les fonctionnalités expérimentales de Copilot CLI (`/experimental on`).
3. Crée `.github/extensions/issue-to-code/extension.mjs`. Pars de ce squelette et complète les `TODO` :

   ```js
   import { defineWorkflow, joinSession } from '@github/copilot-sdk/extension'

   const issueToCode = defineWorkflow({
     meta: {
       name: 'issue-to-code',
       description: 'Issue → spec → plan → code. args: { issue: number }',
       phases: [{ title: 'Spec' }, { title: 'Plan' }, { title: 'Review' }, { title: 'Code' }],
       argsSchema: { type: 'object', required: ['issue'], properties: { issue: { type: 'integer' } } },
     },
     run: async (ctx) => {
       ctx.phase('Spec')
       const spec = await ctx.agent(`Rédige la spec de l'issue #${ctx.args.issue}`,
         { agent: 'spec-writer', label: 'spec' })
       if (spec === null) return { status: 'blocked', reason: 'spec vide' }

       ctx.phase('Plan')
       // TODO : appelle plan-writer avec la spec, garde le résultat, teste null

       ctx.phase('Review')
       // TODO : ctx.log(...) le plan, puis await ctx.pause('plan-approved')

       ctx.phase('Code')
       // TODO : appelle coder avec le plan, retourne un résumé JSON
     },
   })

   await joinSession({ workflows: [issueToCode] })
   ```

4. Redémarre la session, puis demande : « What dynamic workflows are available? ». `issue-to-code` doit apparaître.

   > Source: https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows
   > Citation: "Start a new session, or restart an existing session. When the session finishes loading, ask Copilot to list the available dynamic workflows"
   > Fetched: 2026-10-11

5. Lance-le sur une issue de test : `Run the issue-to-code dynamic workflow for issue 12`.
6. Quand le run se met en pause, ouvre `/workflows`, relis le plan dans les messages de progression, puis appuie sur `R` pour reprendre.

**Critère de réussite** : le run se met en pause après le plan et **avant** toute modification de fichier ; à la reprise, `spec-writer` et `plan-writer` ne sont pas relancés (leurs résultats sont rejoués depuis le journal), puis `coder` implémente le plan. L'orchestrateur `issue-to-code.agent.md` du module 208 n'est plus nécessaire pour exécuter la séquence.

> Source: https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md
> Citation: "A workflow that reads `ctx.args` should declare `meta.argsSchema`"
> Fetched: 2026-10-11

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais expliquer en une phrase pourquoi une séquence déterministe en prose coûte des tokens de raisonnement à chaque run.
- [ ] Tu sais distinguer Autopilot, `/fleet` et un dynamic workflow selon le critère « qui définit le processus ».
- [ ] Tu sais citer les trois surfaces où les dynamic workflows sont disponibles, et dire pourquoi un second adaptateur est nécessaire pour Claude Code.
- [ ] Ton extension `issue-to-code` apparaît quand tu demandes la liste des workflows disponibles.
- [ ] Ton workflow teste le résultat de chaque `ctx.agent` contre `null` avant de l'utiliser.
- [ ] Ton workflow se met en pause avec `ctx.pause` avant l'étape de code, et reprend sans relancer les étapes terminées.
- [ ] Tu sais dire quels hooks un dynamic workflow peut remplacer (ordre, handoff, rappel) et lesquels restent indispensables (écritures des agents, skills chargés, provenance).
- [ ] Tu sais lancer un workflow avec `copilot workflow run <nom> --args '<json>'` et pourquoi il faut accorder les permissions avant.

## Pour aller plus loin

- [Module 208 — Workflows](./208-workflows.md) : relis l'orchestrateur en prose après ce module ; tu verras ce qui relève du jugement et ce qui n'était que de la séquence.
- [Module 105 — Hooks](../01-fondations/105-hooks.md) : les gardes qui restent des hooks, et le piège des hooks Node sur les événements fréquents.
- [Module 210 — Copilot CLI](./210-copilot-cli.md) : la surface où tu lances et surveilles tes runs, y compris en script ou en CI.
- [Module 317 — Orchestrer des subagents](../03-ingenierie-de-contexte/317-orchestrer-subagents.md) : les principes d'orchestration qui guident le découpage en agents.
- [Module 311 — Tokens & fenêtre de contexte](../03-ingenierie-de-contexte/311-tokens-contexte.md) : pourquoi chaque token de raisonnement économisé compte.
- La documentation du SDK détaille `ctx.parallel`, `ctx.pipeline`, `ctx.step` et les limites (`maxConcurrentSubagents`, `timeoutSeconds`, `maxAiCredits`) que ce module n'a fait qu'effleurer.

## Sources

- [GitHub Docs — Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows)
- [GitHub Docs — Using dynamic workflows (how-to)](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows)
- [Copilot SDK — Dynamic Workflows (`nodejs/docs/workflows.md`)](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)
- [SKRAFT — ADR-010 : The SKRAFT pipeline runs as code](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md)
- [SKRAFT — `pipeline-definition.mjs`](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/src/domain/pipeline/pipeline-definition.mjs) (introduit par le commit [`07214d4`](https://github.com/SebastienDegodez/skraft-plugin/commit/07214d4e1c8898fd6182e60f2ad21a89d3f20b32))
- [SKRAFT — extension Copilot `skraft-pipeline/extension.mjs`](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/plugins/skraft-framework/com.github.copilot/extensions/skraft-pipeline/extension.mjs)
- [SKRAFT — Garde-fous (hooks), explication FR](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/site/fr/explanation/hooks.md) (modifié par le commit [`e21ffbc`](https://github.com/SebastienDegodez/skraft-plugin/commit/e21ffbc4a1b352fa6581285aa6772aa973f6ae12))
- [SKRAFT — RunPipeline, l'orchestrateur en code](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/run-pipeline.md)
- [Module 208 — Workflows](./208-workflows.md)

## Module suivant

**Suivant** : [310 — Tester ses primitives (evals binaires)](../03-ingenierie-de-contexte/310-evals.md)
