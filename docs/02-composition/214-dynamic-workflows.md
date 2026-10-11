---
id: dynamic-workflows
title: "214 — Dynamic workflows — orchestrer en code"
sidebar_position: 214
description: "Convertir l'orchestrateur issue-to-code du module 208 en dynamic workflow Copilot : séquence et gardes en code, agents pour le jugement, pause humaine."
---

# 214 — Dynamic workflows — orchestrer en code

Durée estimée : 60 min

> Dans le module 208, l'orchestrateur relit sa procédure à chaque run et décide, en prose, quel sous-agent appeler. Un *dynamic workflow* écrit cette procédure en code : le modèle ne travaille plus que là où il faut du jugement.

## Pourquoi ce module

Reprends l'orchestrateur `issue-to-code` du [module 208](./208-workflows.md). Sa séquence est fixe : `spec-writer`, puis `plan-writer`, puis `coder`. Pourtant, à chaque run, c'est un modèle de langage qui relit ces consignes, décide de l'étape suivante et recopie la sortie d'un sous-agent dans le prompt du suivant.

Autrement dit, tu paies du raisonnement pour exécuter quelque chose qui ne demande aucun jugement : l'ordre `spec → plan → code` ne change jamais. Et comme rien n'empêche un modèle de sauter une étape ou d'oublier de transmettre le plan, on finit souvent par ajouter des hooks qui surveillent l'orchestrateur de l'extérieur.

Un dynamic workflow inverse la logique. La séquence, les transmissions et les vérifications deviennent du code, exécuté à l'identique à chaque run. Les agents ne sont appelés que pour ce qui demande vraiment de l'analyse : écrire la spec, concevoir le plan, coder.

À la fin de ce module, tu sais :

- expliquer ce qu'est un dynamic workflow et le distinguer d'Autopilot et de `/fleet` ;
- dire **où** il tourne (et où il ne tourne pas) ;
- convertir l'orchestrateur en prose `issue-to-code` en une extension Copilot (`defineWorkflow`, `ctx.agent`, `ctx.pause`) ;
- séparer la logique de pilotage de l'hôte qui l'exécute, pour la tester sans Copilot ;
- identifier quels hooks un dynamic workflow rend inutiles, et lesquels restent indispensables ;
- lancer un workflow depuis le chat ou depuis le terminal avec `copilot workflow run`.

*Sources : [208 — Workflows](./208-workflows.md)*

## Pré-requis

- [Module 104 — Agents personnalisés](../01-fondations/104-agents.md) — les agents que le workflow va piloter.
- [Module 105 — Hooks](../01-fondations/105-hooks.md) — tu vas voir quels hooks disparaissent.
- [Module 208 — Workflows](./208-workflows.md) — l'orchestrateur en prose `issue-to-code` et ses trois sous-agents, point de départ de la conversion.
- [Module 210 — Copilot CLI](./210-copilot-cli.md) — le terminal comme surface de travail avec Copilot. Attention : ce module lance les workflows avec la CLI agentique `copilot` (installation : [Installing GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/copilot-cli/set-up-copilot-cli/install-copilot-cli)), qui n'est pas l'extension `gh copilot` présentée en 210.
- Copilot CLI installé, Node.js disponible pour lancer les tests de l'étape 3.

## Concepts clés

### Qu'est-ce qu'un dynamic workflow ?

Un dynamic workflow est un **programme**. Il décrit comment une tâche se déroule : des étapes automatisées (lire un fichier, lancer une commande, appeler un service) et, entre elles, le travail d'un ou plusieurs agents.

La répartition des rôles est explicite : le code décide des étapes et de l'usage des résultats ; les agents traitent ce qui demande analyse ou jugement.

Un workflow peut aussi s'arrêter à un **checkpoint** pour que tu relises les résultats, puis reprendre quand tu es prêt.

Concrètement, le workflow vit dans une **extension Copilot** : un fichier `extension.mjs` qui l'enregistre auprès de la session.

Pour le partager avec ton équipe, tu places le dossier qui contient `extension.mjs` dans `.github/extensions/` du dépôt (ou dans `~/.copilot/extensions/` pour toi seul).

*Sources : [Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows) · [Using dynamic workflows](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows)*

### Autopilot, `/fleet` ou dynamic workflow ?

Les trois font travailler des agents, mais ils ne répartissent pas le contrôle de la même façon. La différence tient à **qui définit le processus**.

| Aspect | Autopilot | `/fleet` | Dynamic workflow |
|---|---|---|---|
| But principal | Laisser Copilot travailler en autonomie | Pousser Copilot à déléguer à des sous-agents | Exécuter un processus défini en code |
| Qui définit le processus ? | Copilot décide des étapes suivantes | Copilot décide comment découper et coordonner | L'auteur du workflow définit étapes, conditions et transmissions |
| Déroulement | Selon la tâche et les décisions de Copilot | Le travail indépendant peut être délégué en parallèle | Séquentiel, parallèle ou les deux, avec un ou plusieurs agents |

Retiens aussi que le chat ne lance jamais un workflow de sa propre initiative : il faut le lui demander, ou qu'un skill ou une commande slash le fasse.

*Sources : [Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows)*

### Prose (module 208) vs code (ce module)

Dans le module 208, l'orchestrateur est un `.agent.md` dont l'outil clé est `runSubagent`, et qui transmet les résultats « en langage naturel ».

Le dynamic workflow déplace exactement cette responsabilité dans du JavaScript. Le SDK fournit `ctx.agent(prompt, options)` pour lancer un sous-agent ; ses options permettent de cibler un agent nommé (`agent`), un modèle (`model`) ou d'exiger une sortie structurée (`schema`).

| | Orchestrateur en prose (208) | Dynamic workflow (214) |
|---|---|---|
| Où vit la séquence | Corps de `issue-to-code.agent.md` | Un module JavaScript (`pipeline.mjs`) |
| Qui l'exécute | Un modèle, à chaque run | Le runtime de l'extension |
| Lancer un sous-agent | `runSubagent`, décidé par le modèle | `await ctx.agent(...)` |
| Passer le résultat | Le modèle recopie la sortie dans le prompt suivant | Une variable JavaScript |
| Validation humaine | Une consigne « demande confirmation » | `await ctx.pause(key)`, durable et reprenable |
| Surveiller l'ordre | Un hook externe (ex. `PreToolUse`) | Un `if` avant chaque `ctx.agent` |
| Hôtes | Tout hôte qui lit `.agent.md` | Copilot app, Copilot CLI, Copilot SDK |

:::warning Compatibilité — à lire avant d'investir
**La documentation officielle liste trois surfaces de disponibilité : la Copilot app, Copilot CLI et le Copilot SDK.**

- **Déduction : non compatible avec Claude Code.** Ce n'est pas une phrase de la documentation GitHub, mais une conclusion tirée de deux prémisses sourcées ci-dessous :
  1. la liste officielle de disponibilité ne cite que la Copilot app, Copilot CLI et le Copilot SDK — Claude Code n'y figure pas ;
  2. une extension s'appuie sur `@github/copilot-sdk/extension` (`defineWorkflow`, `joinSession`), un runtime propre à Copilot.

  Si ton équipe utilise aussi Claude Code, il te faudra **un second adaptateur** écrit avec les mécanismes de Claude Code, autour de la **même fonction de pilotage**. C'est tout l'intérêt de l'étape 3 de la démonstration : la logique ne dépend pas de l'hôte, seul l'adaptateur change.
- **VS Code :** au moment de la rédaction (octobre 2026), VS Code n'apparaît pas dans la liste de disponibilité officielle. Ne compte pas dessus tant que la documentation ne le mentionne pas.
- **Preview :** la fonctionnalité est en *public preview*, et l'API du SDK est déclarée expérimentale. Sous Copilot CLI, il faut activer les fonctionnalités expérimentales (`--experimental` ou `/experimental on`).
- **Plans :** les abonnés Copilot Pro et Pro+ restés sur un plan annuel à facturation par *premium requests* (ancien modèle) n'y ont pas accès.
:::

*Sources : [208 — Workflows](./208-workflows.md) · [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md) · [Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows) · [Using dynamic workflows](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows)*

### Un dynamic workflow peut remplacer certains hooks

Le [module 105](../01-fondations/105-hooks.md) t'a montré les hooks comme garde-fous déterministes. Avec un orchestrateur en prose, certains hooks n'existent **que pour surveiller l'orchestrateur** : rien d'autre ne l'empêche de lancer `coder` avant que le plan existe, ou d'oublier de recopier le plan dans le prompt.

Quand c'est le code qui dispatche, ces vérifications deviennent de simples tests exécutés **avant** chaque `ctx.agent`, sur des variables que le code a lui-même remplies. Le comportement reste *fail-closed* : en cas de refus, le run s'arrête et rien n'est envoyé à l'agent. Trois familles de gardes passent ainsi dans le code :

- **l'ordre des étapes** — `coder` ne part pas tant que `plan-writer` n'a rien produit ;
- **la complétude du handoff** — le prompt envoyé contient bien l'artefact de l'étape précédente ;
- **le rappel « qu'enregistrer ensuite »** — inutile : le code range lui-même le résultat de chaque agent dans une variable.

**La limite est importante.** Le workflow ne voit que ce qu'il fait lui-même : choisir l'agent, composer le prompt, lire la réponse. Sans `schema`, `ctx.agent` lui rend seulement le texte final du sous-agent, pas le détail de son tour.

Ce qui se passe **à l'intérieur** du tour de l'agent — quels fichiers il écrit, quels skills il charge, quels autres agents il lance — échappe donc au code du workflow. Ces gardes-là restent des hooks, déclenchés au moment de l'appel d'outil.

*Sources : [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)*

## Démonstration

On convertit pas à pas l'orchestrateur `issue-to-code` du module 208. Le **contenu** des trois sous-agents (`spec-writer.agent.md`, `plan-writer.agent.md`, `coder.agent.md`) ne change pas, mais leur **emplacement**, si : le module 208 les range dans `.agents/`, alors que Copilot CLI ne cherche les agents personnalisés que dans `~/.copilot/agents/`, `.github/agents/` et `.claude/agents/`. Déplace-les dans `.github/agents/`, sinon `ctx.agent(prompt, { agent: 'spec-writer' })` ne les trouvera pas.

Tout le code est montré en entier ; à la fin, ton dépôt contient :

```text
.github/agents/
  spec-writer.agent.md          # contenu du module 208, déplacé depuis .agents/
  plan-writer.agent.md          # contenu du module 208, déplacé depuis .agents/
  coder.agent.md                # contenu du module 208, déplacé depuis .agents/
  issue-to-code.agent.md        # réduit à un lanceur (étape 6), ou supprimé
.github/extensions/issue-to-code/
  pipeline.mjs                  # étape 2 — la séquence déclarée
  run-issue-to-code.mjs         # étape 3 — la fonction de pilotage
  run-issue-to-code.test.mjs    # étape 3 — ses tests
  extension.mjs                 # étape 4 — l'adaptateur Copilot
  guards.mjs                    # étape 5 — les gardes d'ordre et de handoff
```

*Sources : [CLI — référence des commandes](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference)*

### Étape 1 — Point de départ : l'orchestrateur en prose, surveillé par un hook

Voici l'orchestrateur du module 208. Sa séquence n'existe que dans du texte :

```markdown
<!-- .github/agents/issue-to-code.agent.md (contenu du module 208) -->
---
name: issue-to-code
description: "Workflow complet : issue GitHub → specs → plan → code."
tools:
  - runSubagent
  - githubRepo
---

Tu es un orchestrateur de workflow. Quand on te donne un numéro
d'issue GitHub, tu coordonnes trois sous-agents dans l'ordre :

## Workflow

1. Récupère le contenu de l'issue via `githubRepo`.
2. Délègue la rédaction des specs à l'agent `spec-writer`
   en lui transmettant le contenu de l'issue.
   Récupère les specs produites.
3. Délègue la rédaction du plan à l'agent `plan-writer`
   en lui transmettant les specs.
   Récupère le plan produit.
4. Délègue l'implémentation à l'agent `coder`
   en lui transmettant le plan.
5. Résume le travail accompli : specs, plan, fichiers modifiés.

## Règles

- Tu ne modifies jamais de fichier toi-même.
- Tu ne codes jamais toi-même.
- Si un sous-agent échoue, rapporte l'erreur et arrête le workflow.
```

Rien ne garantit que le modèle respecte l'étape 3 avant l'étape 4. Pour s'en assurer, une équipe ajoute typiquement un hook `PreToolUse` (voir module 105) qui inspecte chaque délégation et refuse `coder` si son prompt ne contient pas de plan. L'exemple ci-dessous est **pédagogique** : les noms exacts des champs de `tool_input` (nom de l'agent, prompt) dépendent de l'hôte — journalise d'abord le JSON reçu sur `stdin` pour les vérifier.

```bash
#!/usr/bin/env bash
# .github/hooks/guard-issue-to-code.sh — exemple pédagogique
payload="$(cat)"
tool="$(printf '%s' "$payload" | jq -r '.tool_name // empty')"
agent="$(printf '%s' "$payload" | jq -r '.tool_input.agentName // empty')"
prompt="$(printf '%s' "$payload" | jq -r '.tool_input.prompt // empty')"

if [ "$tool" = "runSubagent" ] && [ "$agent" = "coder" ] \
   && ! printf '%s' "$prompt" | grep -q '^## Plan'; then
  cat <<'JSON'
{
  "hookSpecificOutput": {
    "hookEventName": "PreToolUse",
    "permissionDecision": "deny",
    "permissionDecisionReason": "coder refusé : aucun plan dans le prompt. Lance plan-writer d'abord."
  }
}
JSON
fi
```

```json
// .github/hooks/issue-to-code.json
{
  "hooks": {
    "PreToolUse": [
      { "type": "command", "command": ".github/hooks/guard-issue-to-code.sh", "timeout": 5 }
    ]
  }
}
```

Remarque le point clé : ce hook existe uniquement parce qu'**aucun code** ne se trouve entre la prose et un mauvais dispatch. Or un `PreToolUse` se déclenche avant l'exécution d'un outil, et la configuration ci-dessus ne le restreint à aucun outil : c'est le script lui-même qui écarte, après coup, tout ce qui n'est pas `runSubagent`. Conséquence directe de cette configuration : il est lancé avant chaque appel d'outil de la session, pour ne surveiller qu'une seule décision.

*Sources : [105 — Hooks](../01-fondations/105-hooks.md)*

### Étape 2 — Déclarer la séquence en code

Premier geste : sortir l'ordre des étapes de la prose. Un petit module, sans aucune dépendance, liste les étapes dans l'ordre, l'agent qui s'en charge, l'artefact qu'il doit produire et celui dont il a besoin. La fonction `brief` compose le prompt de chaque étape à partir des artefacts déjà produits.

```js
// .github/extensions/issue-to-code/pipeline.mjs
export const STEPS = Object.freeze([
  Object.freeze({ id: 'spec', phase: 'Spec', agent: 'spec-writer', needs: null, produces: 'spec' }),
  Object.freeze({ id: 'plan', phase: 'Plan', agent: 'plan-writer', needs: 'spec', produces: 'plan' }),
  Object.freeze({ id: 'code', phase: 'Code', agent: 'coder', needs: 'plan', produces: 'summary', checkpoint: 'plan-approved' }),
])

export function brief(step, issue, artifacts) {
  switch (step.id) {
    case 'spec':
      return `Lis l'issue #${issue} du dépôt courant et rédige sa spécification technique.`
    case 'plan':
      return `Transforme cette spécification en plan d'implémentation.\n\n## Spec\n\n${artifacts.spec}`
    case 'code':
      return `Implémente ce plan, puis lance les tests.\n\n## Plan\n\n${artifacts.plan}`
    default:
      throw new Error(`Étape inconnue : ${step.id}`)
  }
}
```

Deux écarts assumés par rapport au module 208 : ici, c'est `spec-writer` qui lit l'issue lui-même (il dispose déjà d'un outil GitHub dans le module 208), au lieu que l'orchestrateur la lui transmette ; et l'étape « Résume le travail accompli » disparaît, car le résumé produit par `coder` (`summary`) devient la valeur de retour du workflow.

Ce que l'orchestrateur en prose exprimait par « délègue… puis récupère… puis transmets… » tient maintenant dans un tableau gelé. Le champ `checkpoint` indique qu'un humain doit valider **avant** l'étape `code`, c'est-à-dire avant toute modification de fichier.

### Étape 3 — Une fonction de pilotage indépendante de l'hôte

Avant de toucher au SDK Copilot, on écrit la boucle **sans dépendre d'aucun hôte**. Deux mots de vocabulaire, au sens le plus simple :

- un **port** est ce dont la fonction a besoin pour agir sur le monde, décrit comme un simple paramètre : « donne-moi une fonction `runAgent` qui lance un agent et me rend son texte ». La fonction ne sait pas qui la fournit ;
- un **adaptateur** est l'implémentation concrète de ce port pour un hôte donné : sous Copilot, `runAgent` appellera `ctx.agent` ; dans un test, une fonction en mémoire qui rend un texte fixe.

Ici, quatre ports suffisent : `runAgent` (lancer un agent), `setPhase` (annoncer l'étape en cours), `askHuman` (faire valider un humain) et `log` (signaler l'avancement). `setPhase` est volontairement séparé de `runAgent` : la fonction de pilotage l'appelle elle-même, dans sa boucle principale, et jamais depuis une tâche concurrente (tu verras pourquoi à l'étape 4).

```js
// .github/extensions/issue-to-code/run-issue-to-code.mjs (version initiale)
import { STEPS, brief } from './pipeline.mjs'

const isFilled = (value) => typeof value === 'string' && value.trim() !== ''

export async function runIssueToCode({ runAgent, setPhase, askHuman, log }, args) {
  const artifacts = {}

  for (const step of STEPS) {
    const prompt = brief(step, args.issue, artifacts)

    if (step.checkpoint) {
      await askHuman(step.checkpoint, `Relis le plan de l'issue #${args.issue}, puis reprends le run.\n\n${artifacts.plan}`)
    }

    setPhase(step.phase)
    const output = await runAgent({ agent: step.agent, label: step.id, prompt })
    artifacts[step.produces] = output
    log(`${step.agent} : ${isFilled(output) ? 'terminé' : 'aucun résultat'}`)
  }

  if (!isFilled(artifacts.summary)) {
    return { status: 'blocked', step: 'code', reason: "coder n'a rien produit." }
  }
  return { status: 'done', issue: args.issue, summary: artifacts.summary }
}
```

Le bénéfice est immédiat : la fonction se teste avec des **doubles en mémoire**, sans Copilot, sans réseau, sans crédit consommé. Le test ci-dessous utilise uniquement le *test runner* intégré à Node.js (`node --test`).

```js
// .github/extensions/issue-to-code/run-issue-to-code.test.mjs
import { test } from 'node:test'
import assert from 'node:assert/strict'
import { runIssueToCode } from './run-issue-to-code.mjs'

function fakeHost(outputs) {
  const calls = []
  return {
    calls,
    runAgent: async ({ agent, prompt }) => {
      calls.push({ agent, prompt })
      return outputs[agent] ?? null
    },
    setPhase: () => {},
    askHuman: async (key) => { calls.push({ pause: key }) },
    log: () => {},
  }
}

test('enchaîne spec, plan, pause, puis code', async () => {
  const host = fakeHost({ 'spec-writer': 'SPEC', 'plan-writer': 'PLAN', coder: 'FAIT' })
  const result = await runIssueToCode(host, { issue: 42 })

  assert.equal(result.status, 'done')
  assert.deepEqual(
    host.calls.map((c) => c.agent ?? `pause:${c.pause}`),
    ['spec-writer', 'plan-writer', 'pause:plan-approved', 'coder'],
  )
  assert.match(host.calls.at(-1).prompt, /PLAN/)
})
```

```bash
cd .github/extensions/issue-to-code && node --test run-issue-to-code.test.mjs
```

Ce test passe dès maintenant. Il fixe le contrat : l'ordre des agents, la pause avant `coder`, et le plan transmis dans le prompt de `coder`.

### Étape 4 — L'adaptateur Copilot : `defineWorkflow` + `joinSession`

Côté SDK, le principe tient en deux appels : `defineWorkflow` déclare le workflow, `joinSession` l'enregistre dans la session.

Les métadonnées contiennent un `name` stable, une `description`, les `phases` déclarées et, si le workflow lit `ctx.args`, un `argsSchema`.

L'extension n'est qu'un **adaptateur** : elle traduit les quatre ports en appels au SDK, puis délègue tout à `runIssueToCode`.

```js
// .github/extensions/issue-to-code/extension.mjs
import { defineWorkflow, joinSession } from '@github/copilot-sdk/extension'
import { runIssueToCode } from './run-issue-to-code.mjs'

const issueToCode = defineWorkflow({
  meta: {
    name: 'issue-to-code',
    description: 'Issue GitHub → spec → plan → code, avec une pause avant le code. args: { issue: integer }',
    phases: [{ title: 'Spec' }, { title: 'Plan' }, { title: 'Code' }],
    argsSchema: {
      type: 'object',
      required: ['issue'],
      properties: { issue: { type: 'integer' } },
    },
  },
  run: async (ctx) => {
    if (!Number.isInteger(ctx.args.issue) || ctx.args.issue <= 0) {
      return { status: 'blocked', reason: 'args.issue doit être un entier positif.' }
    }

    return runIssueToCode(
      {
        runAgent: ({ agent, label, prompt }) => ctx.agent(prompt, { agent, label }),
        setPhase: (title) => ctx.phase(title),
        askHuman: async (key, message) => {
          ctx.log(message)
          await ctx.pause(key)
        },
        log: (message) => ctx.log(message),
      },
      ctx.args,
    )
  },
})

await joinSession({ workflows: [issueToCode] })
```

Quatre comportements du SDK expliquent ce code.

**1. `argsSchema` ne valide pas tout.** Le schéma contrôle la structure (types, propriétés requises), mais pas les contraintes fines ; le SDK recommande donc de revalider `ctx.args`. D'où le test `Number.isInteger(...) && > 0` en tête de `run`.

**2. Un échec ordinaire de `ctx.agent` ne lève pas d'exception, il rend `null`.** Il faut donc toujours tester le résultat avant de l'utiliser — c'est ce que fera la garde de l'étape 5.

**3. `ctx.pause` est durable.** Au premier passage, le run enregistre le checkpoint et se met en pause. À la reprise, le workflow repart du début et le même checkpoint rend la main. Ne capture pas l'erreur levée par la pause dans un `try/catch` : c'est elle qui arrête proprement la première tentative.

Repartir du début ne relance pas `spec-writer` ni `plan-writer` : chaque appel à `ctx.agent` est journalisé selon son prompt et ses options, et un appel identique rend le résultat déjà enregistré.

**4. `ctx.pause` ne s'appelle que depuis le flux principal**, jamais dans `ctx.parallel()` (plusieurs tâches lancées en même temps) ou `ctx.pipeline()` (une liste d'éléments qui traversent une suite d'étapes, chacun à son rythme). C'est le cas ici : `askHuman` est appelé dans la boucle de `runIssueToCode`, pas dans une tâche concurrente.

Enfin, `ctx.phase` alimente l'affichage de la progression. C'est une valeur unique pour tout le run : on l'appelle à chaque changement d'étape, depuis le flux principal — d'où le port `setPhase`, distinct de `runAgent`. Le travail concurrent se distingue par son `label`, pas par une phase.

*Sources : [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)*

### Étape 5 — Les gardes quittent le hook

Maintenant que c'est le code qui dispatche, la vérification d'ordre et celle de complétude du handoff s'exécutent juste avant chaque `ctx.agent`. On les isole dans un module pur, testable comme le reste :

```js
// .github/extensions/issue-to-code/guards.mjs
import { STEPS } from './pipeline.mjs'

const filled = (value) => typeof value === 'string' && value.trim() !== ''

export function checkDispatch(step, artifacts, prompt) {
  const index = STEPS.indexOf(step)
  if (index === -1) return { ok: false, reason: `Étape inconnue : ${step?.id}` }

  // Ordre : chaque étape précédente a produit un artefact non vide.
  for (const previous of STEPS.slice(0, index)) {
    if (!filled(artifacts[previous.produces])) {
      return { ok: false, reason: `${step.agent} refusé : ${previous.agent} n'a rien produit.` }
    }
  }

  // Handoff : l'artefact attendu figure bien dans le prompt envoyé.
  if (step.needs && !prompt.includes(artifacts[step.needs])) {
    return { ok: false, reason: `${step.agent} refusé : le prompt ne contient pas « ${step.needs} ».` }
  }

  return { ok: true }
}

export const isFilled = filled
```

Puis on branche la garde dans la fonction de pilotage. En cas de refus, le run s'arrête avec le statut `blocked` : **fail-closed**, rien n'est envoyé à l'agent, et la pause humaine n'est même pas proposée sur un plan vide.

```diff
  // .github/extensions/issue-to-code/run-issue-to-code.mjs
  import { STEPS, brief } from './pipeline.mjs'
+ import { checkDispatch, isFilled } from './guards.mjs'

- const isFilled = (value) => typeof value === 'string' && value.trim() !== ''
-
  export async function runIssueToCode({ runAgent, setPhase, askHuman, log }, args) {
    const artifacts = {}

    for (const step of STEPS) {
      const prompt = brief(step, args.issue, artifacts)

+     const check = checkDispatch(step, artifacts, prompt)
+     if (!check.ok) {
+       log(check.reason)
+       return { status: 'blocked', step: step.id, reason: check.reason }
+     }
+
      if (step.checkpoint) {
```

Ajoute le test du cas refusé ; il remplace le test manuel du hook :

```diff
  // .github/extensions/issue-to-code/run-issue-to-code.test.mjs
+
+ test('refuse coder quand plan-writer ne rend rien', async () => {
+   const host = fakeHost({ 'spec-writer': 'SPEC', 'plan-writer': '   ' })
+   const result = await runIssueToCode(host, { issue: 42 })
+
+   assert.equal(result.status, 'blocked')
+   assert.equal(result.step, 'code')
+   assert.ok(!host.calls.some((c) => c.agent === 'coder'))
+   assert.ok(!host.calls.some((c) => c.pause))
+ })
```

Les deux tests passent avec `node --test`. Le hook d'ordre n'a plus de raison d'être : supprime-le.

```diff
- .github/hooks/guard-issue-to-code.sh
- .github/hooks/issue-to-code.json
```

Le bilan, en termes génériques :

| Garde | Avant (orchestrateur en prose) | Après (dynamic workflow) |
|---|---|---|
| Ordre des étapes | Hook `PreToolUse`, à chaque appel d'outil | `checkDispatch`, avant chaque `ctx.agent` ; fail-closed, le run s'arrête `blocked` |
| Complétude du handoff (artefact présent, non vide, transmis) | Hook `PreToolUse` qui inspecte le prompt | `checkDispatch`, sur le prompt composé par le code ; fail-closed |
| Rappel « qu'enregistrer ensuite » | Hook `PostToolUse` qui relance l'orchestrateur | Supprimé : le code range lui-même le résultat dans `artifacts` |
| Écritures de fichiers sensibles | Hook `PreToolUse` | Reste un hook : le code ne voit pas les outils appelés pendant le tour de l'agent |
| Chargement de skills obligatoires | Hooks | Restent des hooks, pour la même raison |
| Qui a le droit de dispatcher qui | Hook `PreToolUse` | Reste un hook : un agent peut aussi être lancé hors du workflow |

Ce qui était un garde-fou autour de la prose devient un invariant du code, testé comme n'importe quelle fonction.

### Étape 6 — Lancer : l'orchestrateur devient un lanceur

Active les fonctionnalités expérimentales (`/experimental on`), redémarre la session, puis vérifie que le workflow est chargé :

```text
What dynamic workflows are available?
```

Deux façons de démarrer le même workflow. Dans le chat, en langage naturel :

```text
Run the issue-to-code dynamic workflow for issue 42
```

Ou directement depuis le terminal, avec les entrées en JSON :

```bash
copilot workflow run issue-to-code \
   --args '{"issue":42}' \
   --allow-tool='read, write, shell, github'
```

Attention : en ligne de commande, aucune demande d'autorisation ne s'affiche. Accorde **avant** le lancement les permissions dont les agents ont besoin, avec `--allow-tool` (plusieurs valeurs : une liste entre guillemets, séparée par des virgules) :

- `read` — lire les fichiers, pour `plan-writer` et `coder` ;
- `write` et `shell` — modifier les fichiers et lancer les tests, pour `coder` ;
- `github` — si ton `spec-writer` lit l'issue via le serveur MCP GitHub, autorise ce serveur sous son nom configuré (ici `github`). Le module 208 déclare l'outil VS Code `githubRepo` : vérifie avec `/agent` quel outil ton agent obtient réellement sous la CLI, et ajuste cette valeur.

Une demande qui ne peut pas être approuvée automatiquement est refusée. Pour serrer la vis, `shell` et `write` acceptent un filtre entre parenthèses, par exemple `shell(npm test)` pour n'autoriser que cette commande exacte.

Ton extension est une extension **de projet** (dans `.github/extensions/`). Si tu lances la commande dans un dossier que Copilot CLI n'a pas encore marqué comme approuvé, l'extension n'est pas chargée : ajoute `GITHUB_COPILOT_PROMPT_MODE_EXTENSIONS=true` pour cette invocation. Cela autorise l'exécution du code d'extension du dépôt — **à n'utiliser que pour du code de confiance** — et ne dispense pas d'accorder les permissions des agents.

```bash
GITHUB_COPILOT_PROMPT_MODE_EXTENSIONS=true copilot workflow run issue-to-code \
   --args '{"issue":42}' \
   --allow-tool='read, write, shell, github'
```

En ligne de commande, quand le run se met en pause ou s'arrête sans terminer, la commande rend la main en affichant le **statut** du run et son **run ID**. La documentation ne décrit pas de commande pour reprendre ce run depuis le terminal : ne compte pas dessus.

**Pour la démo du checkpoint, lance le run depuis le chat** d'une session interactive. C'est dans cette session que tu le suis et le reprends avec `/workflows` : tu y vois la phase courante, les sous-agents actifs et les crédits IA consommés (*AI credits*, l'unité de mesure d'usage commune à tout GitHub Copilot). Quand le run s'arrête au checkpoint `plan-approved`, relis le plan dans les messages de progression, ouvre `/workflows`, sélectionne le run en pause et appuie sur `R` pour le reprendre.

Et l'orchestrateur `issue-to-code.agent.md` ? Il ne décide plus rien. Le plus simple est de le supprimer. Si ton équipe a l'habitude de sélectionner cet agent, garde-le comme simple **lanceur** :

```diff
  ---
  name: issue-to-code
- description: "Workflow complet : issue GitHub → specs → plan → code."
- tools:
-   - runSubagent
-   - githubRepo
+ description: "Lance le dynamic workflow issue-to-code pour une issue donnée."
  ---

- Tu es un orchestrateur de workflow. Quand on te donne un numéro
- d'issue GitHub, tu coordonnes trois sous-agents dans l'ordre :
- …
+ Quand on te donne un numéro d'issue, lance le dynamic workflow
+ `issue-to-code` avec `{ "issue": <numéro> }`. Ne délègue à aucun
+ sous-agent toi-même. Quand le run se met en pause, rappelle à
+ l'utilisateur de relire le plan puis de reprendre le run.
```

*Sources : [Using dynamic workflows](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows) · [CLI — référence programmatique](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-programmatic-reference) · [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)*

## Pièges et anti-patterns

- **Oublier que c'est une preview.** Fonctionnalité en *public preview*, API du SDK déclarée expérimentale : signatures et comportements peuvent changer. Épingle ta version de Copilot CLI et garde les tests de ta fonction de pilotage, qui eux ne dépendent pas du SDK.

- **Croire que le workflow sera portable.** Ni Claude Code ni (au moment de la rédaction) VS Code ne figurent dans la liste officielle de disponibilité ; l'encadré *Compatibilité* en déduit qu'il faut un second adaptateur pour Claude Code. Garde toute la logique dans `runIssueToCode` et `guards.mjs` pour que ce second adaptateur reste mince.

- **Lancer un agent à la main, hors workflow.** Les gardes déplacées dans le code ne protègent que les dispatchs qui passent par le code. Si quelqu'un sélectionne `coder` directement dans le chat, `checkDispatch` ne s'exécute pas : seuls les hooks restants (écritures sensibles, provenance) s'appliquent encore.

- **Sous-estimer les permissions.** Les sous-agents du workflow héritent des autorisations de ta session : ce que tu as déjà accordé s'applique à eux. Et `copilot workflow run` n'affiche aucune demande d'approbation.

- **Appeler deux fois `ctx.agent` avec le même prompt en attendant deux agents.** Les appels identiques sont mémoïsés en **un seul** sous-agent. Donne un `label` unique à chaque appel indépendant — c'est pourquoi l'adaptateur passe `label: step.id`.

- **Fixer des limites au hasard.** Un plafond (`maxAiCredits`, `maxTotalSubagents`…) deviné coupe un run sain en plein milieu, après qu'il a déjà dépensé des crédits. Fais d'abord un run réduit, mesure, puis fixe la limite.

- **Garder en hook une vérification que le code peut faire.** *Retour d'expérience terrain, pas un fait documenté :* un hook `PreToolUse` ou `PostToolUse` est relancé **à chaque appel d'outil**. Déclaré en `node hook.mjs`, il paie à chaque fois le démarrage d'un processus Node ; sur une session qui enchaîne des dizaines d'outils, ce surcoût s'additionne et ralentit nettement le système. Si la vérification ne porte que sur l'ordre d'orchestration, elle a sa place dans le workflow, pas dans un hook. Le [module 105](../01-fondations/105-hooks.md) détaille ce piège et ses alternatives.

*Sources : [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md) · [Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows) · [Using dynamic workflows](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows)*

## Exercice ⭐⭐

**Énoncé** — Étends le workflow `issue-to-code` avec une étape de **relecture** après `coder` : deux relectures indépendantes lancées en parallèle (sécurité et lisibilité), une garde en code qui exige deux verdicts `OK`, un seul renvoi au `coder` en cas d'échec (boucle bornée), et une pause humaine si le second passage échoue encore.

**Étapes guidées** :

1. Crée `.github/agents/reviewer.agent.md` sur le modèle des sous-agents du module 208 : outil `codebase` uniquement, aucune modification de fichier, et une sortie qui commence par `OK` ou `KO` suivie des remarques.
2. Dans `run-issue-to-code.mjs`, après l'étape `code`, appelle **une fois** `setPhase('Review')` dans le flux principal (ajoute `{ title: 'Review' }` aux `phases` de l'extension), puis lance les deux relectures avec `ctx.parallel` (expose-le comme un cinquième port, par exemple `runInParallel`). Les *thunks* (des fonctions sans argument qui lancent chacune une tâche, par exemple `() => runAgent(…)`) passés à `runInParallel` n'appellent que `runAgent`, qui ne touche pas à la phase : appeler `ctx.phase` depuis des tâches concurrentes crée une course sur une valeur unique du run. Donne un `label` distinct à chaque appel (`review-securite`, `review-lisibilite`), sinon les deux appels seront mémoïsés en un seul agent.

3. Écris dans `guards.mjs` une garde `checkReviews(reviews)` qui renvoie `{ ok: true }` seulement si les deux verdicts existent et commencent par `OK`. Un verdict `null` compte comme un échec : `ctx.parallel` transforme en `null` une tâche qui a échoué.

4. Si la garde échoue, renvoie **une seule fois** au `coder` les remarques des relecteurs, avec un label différent (`code-2`), puis relance les deux relectures (`review-securite-2`, `review-lisibilite-2`). Borne la boucle avec une constante `MAX_ROUNDS = 2`.
5. Si le second passage échoue encore, appelle `askHuman('review-failed', …)` **depuis le flux principal** — jamais à l'intérieur de `ctx.parallel`.
6. Ajoute deux tests en mémoire : (a) deux `OK` au premier passage → `done` sans second `coder` ; (b) un `KO` deux fois de suite → exactement deux appels à `coder`, puis une pause `review-failed`.

Squelette de départ, à compléter :

```js
// extrait à ajouter dans runIssueToCode, après la boucle des STEPS
// (runInParallel s'ajoute aux ports : { runAgent, setPhase, askHuman, log, runInParallel })
const MAX_ROUNDS = 2
setPhase('Review') // une seule fois, dans le flux principal — jamais dans les thunks
for (let round = 1; round <= MAX_ROUNDS; round += 1) {
  const suffix = round === 1 ? '' : `-${round}`
  const reviews = await runInParallel([
    () => runAgent({ agent: 'reviewer', label: `review-securite${suffix}`, prompt: /* TODO */ '' }),
    () => runAgent({ agent: 'reviewer', label: `review-lisibilite${suffix}`, prompt: /* TODO */ '' }),
  ])
  // TODO : checkReviews(reviews) ; si ok → return done
  // TODO : si round < MAX_ROUNDS → relancer coder avec label `code-${round + 1}` et les remarques
}
// TODO : askHuman('review-failed', …) puis return { status: 'blocked', … }
```

**Critère de réussite** : `node --test` passe avec les quatre tests ; sur une vraie issue, `/workflows` montre deux relecteurs actifs en même temps ; `coder` n'est jamais appelé plus de deux fois ; un double échec de relecture met le run en pause au checkpoint `review-failed` au lieu de boucler.

*Sources : [SDK — workflows](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)*

## Validation

Tu peux passer au module suivant si :

- [ ] Tu sais expliquer en une phrase pourquoi une séquence déterministe écrite en prose coûte du raisonnement à chaque run.
- [ ] Tu sais distinguer Autopilot, `/fleet` et un dynamic workflow selon le critère « qui définit le processus ».
- [ ] Tu sais citer les trois surfaces où les dynamic workflows sont disponibles, et expliquer pourquoi un second adaptateur est nécessaire pour Claude Code.
- [ ] Ta fonction `runIssueToCode` se teste avec `node --test`, sans Copilot.
- [ ] Ton extension `issue-to-code` apparaît quand tu demandes la liste des workflows disponibles.
- [ ] Ta garde refuse `coder` quand le plan est absent ou vide, et le run s'arrête `blocked`.
- [ ] Ton workflow se met en pause avec `ctx.pause` avant l'étape de code, et reprend sans relancer `spec-writer` ni `plan-writer`.
- [ ] Tu sais dire quels hooks un dynamic workflow peut remplacer (ordre, handoff, rappel) et lesquels restent indispensables (écritures des agents, skills chargés, provenance).
- [ ] Tu sais lancer un workflow avec `copilot workflow run <nom> --args '<json>' --allow-tool=…` et pourquoi il faut accorder les permissions avant.
- [ ] Tu sais pourquoi la démo du checkpoint se lance depuis le chat : c'est dans la session interactive que `/workflows` + `R` reprend le run en pause.

## Pour aller plus loin

- [Module 208 — Workflows](./208-workflows.md) : relis l'orchestrateur en prose après ce module ; tu verras ce qui relevait du jugement et ce qui n'était que de la séquence.
- [Module 105 — Hooks](../01-fondations/105-hooks.md) : les gardes qui restent des hooks, et le piège des hooks Node sur les événements fréquents.
- [Module 210 — Copilot CLI](./210-copilot-cli.md) : le terminal comme surface de travail ; pour `copilot workflow run` en script ou en CI, voir la référence programmatique citée dans les sources.
- [Module 317 — Orchestrer des subagents](../03-ingenierie-de-contexte/317-orchestrer-subagents.md) : les principes d'orchestration qui guident le découpage en agents.
- [Module 311 — Tokens & fenêtre de contexte](../03-ingenierie-de-contexte/311-tokens-contexte.md) : pourquoi chaque token de raisonnement économisé compte.
- La documentation du SDK détaille `ctx.pipeline` (faire traverser une suite d'étapes à chaque élément d'une liste, sans attendre les autres), `ctx.step` et les limites (`maxConcurrentSubagents`, `timeoutSeconds`, `maxAiCredits`) que ce module n'a fait qu'effleurer.

## Sources

- [GitHub Docs — Dynamic workflows (concepts)](https://docs.github.com/en/copilot/concepts/agents/dynamic-workflows)
- [GitHub Docs — Using dynamic workflows (how-to)](https://docs.github.com/en/copilot/how-tos/use-copilot-agents/use-dynamic-workflows)
- [Copilot SDK — Dynamic Workflows (`nodejs/docs/workflows.md`)](https://github.com/github/copilot-sdk/blob/main/nodejs/docs/workflows.md)
- [GitHub Docs — Copilot CLI programmatic reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-programmatic-reference)
- [GitHub Docs — Copilot CLI command reference](https://docs.github.com/en/copilot/reference/copilot-cli-reference/cli-command-reference) — emplacements des agents personnalisés.
- [GitHub Docs — Installing GitHub Copilot CLI](https://docs.github.com/en/copilot/how-tos/copilot-cli/set-up-copilot-cli/install-copilot-cli)
- [Module 208 — Workflows](./208-workflows.md)
- [Module 105 — Hooks](../01-fondations/105-hooks.md)
- Inspiration de la démarche — SKRAFT (expérience / POC), branche `integration/v2` : retour d'expérience d'un pipeline passé de la prose au code. Lecture facultative, non nécessaire pour suivre ce module :
  - [ADR-010 — le pipeline s'exécute en code](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/adr/adr-010-pipeline-runs-as-code.md)
  - [Garde-fous (hooks) — explication FR](https://github.com/SebastienDegodez/skraft-plugin/blob/integration/v2/docs/site/fr/explanation/hooks.md)

## Module suivant

**Suivant** : [310 — Tester ses primitives (evals binaires)](../03-ingenierie-de-contexte/310-evals.md)
