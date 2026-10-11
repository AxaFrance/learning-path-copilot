# Mon Sinistre — projet d'exemple du Bloc 04

> Projet **pédagogique** utilisé par les modules 420 à 438 du handbook (Copilot CLI et GitHub Copilot app).
> **Toutes les données sont fictives.** Aucun code, API ou donnée client AXA n'est utilisé.

Mon Sinistre gère des **sinistres auto** : un assuré a un contrat (formule + franchise), il déclare un sinistre (bris de glace, vol, collision), le dossier suit un cycle de statuts jusqu'à l'indemnisation ou au refus.

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (le fichier `global.json` accepte toute version 10.0.1xx ou supérieure)
- Bash (macOS, Linux, WSL, Git Bash) **ou** PowerShell 7+
- Optionnel, pour `scripts/seed.sh` : [GitHub CLI](https://cli.github.com/) authentifié (`gh auth login`)

## Structure

```text
samples/mon-sinistre/
├── MonSinistre.sln
├── data/sinistres.json               # jeu de données fictif (3 contrats, 5 sinistres)
├── src/
│   ├── MonSinistre.Domain/           # entités et règles métier, sans dépendance
│   ├── MonSinistre.Cli/              # application console : list, show, declare
│   └── MonSinistre.Web/              # minimal API ASP.NET Core + page HTML statique
├── tests/MonSinistre.Domain.Tests/   # tests xUnit des règles métier
└── scripts/
    ├── verify.sh / verify.ps1        # restore + build (-warnaserror) + test
    └── seed.sh                       # issues et PR d'entraînement dans ton fork
```

## Démarrer

Toutes les commandes se lancent depuis `samples/mon-sinistre/`.

```bash
# 1. Vérifier que tout compile et que les tests passent
./scripts/verify.sh            # ou : pwsh ./scripts/verify.ps1

# 2. Utiliser la console
dotnet run --project src/MonSinistre.Cli -- list
dotnet run --project src/MonSinistre.Cli -- show SIN-1004
dotnet run --project src/MonSinistre.Cli -- declare CTR-002 Vol 2026-10-01 1250.50

# 3. Lancer l'interface web (http://localhost:5213)
dotnet run --project src/MonSinistre.Web
```

> La console écrit les déclarations dans la copie de `data/sinistres.json` située dans `bin/` : le fichier source n'est jamais modifié.
> L'interface web garde les déclarations **en mémoire** : elles disparaissent au redémarrage.

## Règles métier

| Règle | Fichier | Attendu métier |
|---|---|---|
| Couverture | `RegleCouverture.cs` | Tiers : aucun dommage propre · Tiers+ : bris de glace et vol · Tous risques : tout |
| Indemnisation | `CalculIndemnisation.cs` | Montant estimé − franchise, **jamais négatif** |
| Délai de déclaration | `DelaiDeclaration.cs` | **5 jours ouvrés** après la survenance |
| Transitions de statut | `TransitionStatut.cs` | Déclaré → En expertise / Refusé · En expertise → Indemnisé / Refusé · Indemnisé et Refusé sont **définitifs** |

Les montants sont stockés en **centimes** (`long`) pour éviter les erreurs d'arrondi.

## Défauts volontaires

Au départ, `verify.sh` est **vert**. Pourtant le code contient des défauts que les tests ne couvrent pas : c'est à toi de les trouver avec Copilot au fil des modules. **Ne les corrige pas en dehors des exercices.**

| # | Défaut | Symptôme observable | Exercices |
|---|---|---|---|
| D1 | Franchise appliquée sans plancher | `show SIN-1004` affiche une indemnisation de **-80,00 €** | 424, 433 |
| D2 | Délai compté en jours calendaires | `show SIN-1004` (vendredi → vendredi suivant) affiche « Dans les délais : non » | 424, 429 |
| D3 | Transition « Refusé → Indemnisé » autorisée | `TransitionStatut.EstAutorisee(Refuse, Indemnise)` renvoie `true` | 422, 431 |
| D4 | Formatage des montants dupliqué | Même logique dans `CalculIndemnisation.FormaterMontant` et dans `Program.cs` du CLI | 423, 434 |
| D5 | Pas de `copilot-instructions.md` | Copilot ne connaît ni le vocabulaire métier ni la convention « montants en centimes » | 425, 435 |

## Critère de réussite des exercices

Un exercice est terminé quand :

1. le comportement demandé est observable (`show`, page web ou test) ;
2. **`./scripts/verify.sh` est vert** ;
3. tu as relu le diff proposé par Copilot avant de l'accepter.

## Pas de CI fournie

Ce projet est un **sample** : il n'embarque volontairement **aucun workflow GitHub Actions**. Les exercices s'appuient sur `verify.sh` en local.

> 💡 Une équipe qui reprend ce projet peut ajouter son propre workflow (par exemple un job qui lance `./samples/mon-sinistre/scripts/verify.sh` sur chaque PR). Ce n'est pas nécessaire pour suivre le Bloc 04.

## Brancher le design system AXA (optionnel)

L'interface web utilise volontairement un CSS minimal (`src/MonSinistre.Web/wwwroot/site.css`). **Le design system AXA n'est pas intégré.**

Une équipe AXA peut le mettre en place :

- en installant le plugin `axa-fr/design-system` présenté au [module 209 — Plugins](../../docs/02-composition/209-plugins.md), qui apporte à Copilot les skills et instructions de la charte ;
- puis en demandant à Copilot de remplacer `site.css` et le balisage de `index.html` par les composants du design system.

## Script de seed (modules 434 et 438)

```bash
# depuis la racine de TON fork
./samples/mon-sinistre/scripts/seed.sh
```

Il crée dans ton fork, avec le label `mon-sinistre` :

- 3 issues (une par défaut D1, D2, D3) ;
- 3 PR d'entraînement : une **prête à merger**, une qui **casse `verify.sh`**, une avec un **commentaire de revue** à traiter.

Le script refuse de tourner si ton dépôt local a des modifications non commitées, et te demande confirmation si le dépôt n'est pas un fork.
