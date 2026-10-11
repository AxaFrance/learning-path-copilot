---
id: app-mcp-plugins
title: "436 — GitHub Copilot app : MCP et plugins"
sidebar_position: 436
description: "Brancher une source documentaire via MCP, comprendre les plugins qui fournissent des skills, et cadrer l'usage optionnel du design system AXA dans l'app."
---

# 436 — GitHub Copilot app : MCP et plugins

**Durée** : ~45 min · **Complexité** : ⭐⭐ · **Pré-requis** : [435 — Skills et agents personnalisés](./435-app-skills-agents.md), [106 — MCP](../01-fondations/106-mcp.md), [209 — Plugins](../02-composition/209-plugins.md), [213 — APM vs plugins](../02-composition/213-apm-vs-plugins.md)

> *Un agent utile ne sait pas tout par magie. Parfois tu lui donnes une règle, parfois tu lui branches une source, parfois tu installes un paquet qui apporte plusieurs capacités d'un coup.*

## Objectif

À la fin de ce module, tu sais distinguer un serveur MCP d'un plugin dans la GitHub Copilot app, vérifier les preuves d'un appel outil, et décider quand brancher une capacité d'équipe comme `axa-fr/design-system`.

## Ce que tu vas apprendre

1. Ce qu'un serveur MCP ajoute à une session.
2. Ce qu'un plugin peut empaqueter.
3. Comment garder les questions documentaires générales.
4. Comment vérifier qu'une capacité a vraiment tourné.

## Contenu pédagogique

### MCP : connecter une source ou un outil

Le Model Context Protocol permet à l'app de connecter l'agent à des outils ou des sources de données externes. Dans le cours GitHub, l'exemple ajoute Context7 pour récupérer de la documentation React à jour, puis compare localement cette documentation avec le code.

La règle de sobriété est importante : on n'envoie pas le code du dépôt à une source documentaire si une question générale suffit. Tu peux demander « quelle est la règle officielle pour un libellé de champ ? », puis faire la comparaison avec les fichiers locaux dans un second temps.

Dans Mon Sinistre, un MCP documentaire serait utile pour consulter une documentation d'API interne, une règle de calcul publiée, ou une base de connaissance d'architecture. Il ne doit pas devenir un raccourci pour exposer des données sensibles : les données du sample sont fictives, mais le réflexe doit rester celui du moindre partage.

### Plugin : un paquet de capacités

Un plugin est un paquet installable qui peut contenir des skills, agents, hooks, configurations MCP ou extensions de canvas. La documentation GitHub distingue les plugins au format Agent Plugins 1.0, plus portables, et les anciens plugins Copilot.

Dans l'app, les plugins se gèrent depuis **Customize > Plugins**. Installer un plugin n'exécute pas automatiquement son skill : il faut encore l'invoquer, observer son activité, et vérifier les sources ou fichiers qu'il cite.

| Question | MCP | Plugin |
|---|---|---|
| Ce que ça ajoute | Un ou plusieurs outils connectés | Un paquet de capacités |
| Exemple du cours | Context7 pour la documentation | Modern Web Guidance qui fournit un skill |
| Preuve à vérifier | Activité outil et lien source | Skill chargé, sortie et source du guide |
| Risque principal | Envoyer trop d'information externe | Installer plus de capacités que nécessaire |

### Une méthode en deux temps

Le bon usage suit une séquence courte : d'abord la source externe générale, ensuite la comparaison locale. Cela limite la fuite de contexte et force l'agent à produire une preuve exploitable.

Pour Mon Sinistre, la demande peut ressembler à ceci :

```text
Utilise le serveur documentaire configuré pour retrouver la règle générale de calcul des jours ouvrés.
Pose une question générale, sans envoyer de chemins ni de code du dépôt.
Ensuite seulement, compare localement avec @samples/mon-sinistre/src/MonSinistre.Domain/DelaiDeclaration.cs.
Ne modifie rien.
```

Le fichier `DelaiDeclaration.cs` indique une intention de 5 jours ouvrés, mais son implémentation soustrait seulement les `DayNumber`, donc compte les jours calendaires.

:::tip DS AXA — ce que vous pourriez brancher
Le design system AXA n'est pas intégré dans `samples/mon-sinistre`. Une équipe peut brancher un plugin `axa-fr/design-system` pour fournir à Copilot des instructions, composants et skills de revue UI cohérents avec la charte, comme expliqué dans le [module 209 — Plugins](../02-composition/209-plugins.md). Garde cet ajout optionnel : il ne doit pas être nécessaire pour réussir les exercices du Bloc 04.
:::

### Lire les permissions avant d'élargir

Le cours rappelle qu'un worktree isole les changements de fichiers, pas forcément les outils installés. Un serveur MCP ou un plugin activé peut être visible dans d'autres sessions de l'app.

Avant d'ajouter une capacité, demande-toi : quelle question ne peut pas être résolue avec les fichiers déjà présents ? quelle donnée sort de la machine ? quel résultat prouve que l'outil a travaillé ? Si tu ne peux pas répondre, commence sans intégration.

## Exercice

**Énoncé** : préparer une revue documentaire du délai de déclaration, sans modifier le code.

1. Ouvre une session GitHub Copilot app sur ton fork.
2. Vérifie les intégrations disponibles dans **Customize > MCP** et **Customize > Plugins**. N'ajoute rien si ton organisation le bloque.
3. Si un serveur documentaire approuvé est disponible, demande une règle générale sur les jours ouvrés sans mentionner le dépôt.
4. Ensuite, demande une comparaison locale avec `samples/mon-sinistre/src/MonSinistre.Domain/DelaiDeclaration.cs` et les tests associés.
5. Si ton équipe dispose du plugin `axa-fr/design-system`, inspecte ce qu'il fournirait pour l'interface web, mais ne l'applique pas dans ce module.
6. Termine par la validation locale habituelle :

```bash
cd samples/mon-sinistre
./scripts/verify.sh        # Windows : pwsh ./scripts/verify.ps1
```

Le but n'est pas de corriger D2 ici. Le but est de produire une preuve : source externe consultée, comparaison locale, aucune modification de fichier.

## Validation

- [ ] Tu sais expliquer la différence entre MCP et plugin.
- [ ] La réponse cite l'activité outil ou dit clairement que l'intégration était indisponible.
- [ ] Aucune donnée de dépôt inutile n'a été envoyée à une source documentaire externe.
- [ ] L'onglet **Changes** ne montre pas de modification pour cet exercice de revue.
- [ ] `./scripts/verify.sh` reste vert depuis `samples/mon-sinistre`.

## Pour aller plus loin

- [106 — MCP](../01-fondations/106-mcp.md) — la primitive de connexion.
- [209 — Plugins](../02-composition/209-plugins.md) — empaqueter des capacités.
- [213 — APM vs plugins](../02-composition/213-apm-vs-plugins.md) — ne pas confondre orchestrateur et paquet.
- [437 — Canvases](./437-app-canvases.md) — rendre visible le plan et les preuves.

## Sources

- [github/copilot-app-for-beginners — Chapter 05](https://github.com/github/copilot-app-for-beginners/blob/main/05-mcp-plugins/README.md) — cours GitHub sous licence MIT, adapté en français.
- [Customizing the GitHub Copilot app](https://docs.github.com/en/copilot/how-tos/github-copilot-app/customize-github-copilot-app)
- [About GitHub Copilot plugins](https://docs.github.com/en/copilot/concepts/agents/about-plugins)
- [README Mon Sinistre — Toutes les données sont fictives](https://github.com/AxaFrance/learning-path-copilot/blob/main/samples/mon-sinistre/README.md)
