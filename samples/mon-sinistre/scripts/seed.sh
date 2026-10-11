#!/usr/bin/env bash
# Crée des issues et des pull requests d'entraînement dans TON fork du dépôt,
# pour les exercices des modules 434 (workflows GitHub) et 438 (automations).
#
# Prérequis : gh authentifié (gh auth login), exécuté depuis un clone de ton fork.
# Usage    : ./samples/mon-sinistre/scripts/seed.sh
#
# Le script ne touche jamais à la branche par défaut : chaque PR part d'une branche dédiée.
set -euo pipefail

command -v gh >/dev/null || { echo "gh (GitHub CLI) est requis." >&2; exit 1; }
command -v git >/dev/null || { echo "git est requis." >&2; exit 1; }

# Le dépôt cible est toujours celui du remote "origin" (ton fork), jamais un éventuel "upstream".
url_origin="$(git remote get-url origin)"
depot="$(gh repo view "$url_origin" --json nameWithOwner -q .nameWithOwner)"
parent="$(gh repo view "$depot" --json parent -q '.parent.nameWithOwner // ""')"
if [[ -z "$parent" ]]; then
  echo "⚠️  $depot n'est pas un fork. Lance ce script dans TON fork pour ne pas polluer le dépôt d'origine." >&2
  read -r -p "Continuer quand même ? [o/N] " reponse
  [[ "$reponse" == "o" || "$reponse" == "O" ]] || exit 1
fi

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Le dépôt local a des modifications non commitées. Commit ou stash avant de lancer le seed." >&2
  exit 1
fi

racine="$(git rev-parse --show-toplevel)"
sample="samples/mon-sinistre"
branche_defaut="$(gh repo view "$depot" --json defaultBranchRef -q .defaultBranchRef.name)"
branche_initiale="$(git rev-parse --abbrev-ref HEAD)"
cd "$racine"
trap 'git checkout --quiet "$branche_initiale"' EXIT

git fetch --quiet origin "$branche_defaut"

echo "==> Labels"
gh label create --repo "$depot" "mon-sinistre" --color 00008F --description "Exercices du projet d'exemple Mon Sinistre" --force >/dev/null

echo "==> Issues"
gh issue create --repo "$depot" --label mon-sinistre --assignee "@me" \
  --title "[Mon Sinistre] L'indemnisation peut être négative" \
  --body "Le sinistre SIN-1004 (franchise 300 €, montant 220 €) affiche une indemnisation de -80 €. L'indemnisation ne doit jamais être négative."
gh issue create --repo "$depot" --label mon-sinistre --assignee "@me" \
  --title "[Mon Sinistre] Le délai de déclaration doit compter les jours ouvrés" \
  --body "Le délai de 5 jours est calculé en jours calendaires. SIN-1004 (vendredi → vendredi suivant) est signalé hors délai alors qu'il est dans les 5 jours ouvrés."
gh issue create --repo "$depot" --label mon-sinistre --assignee "@me" \
  --title "[Mon Sinistre] Un sinistre refusé ne doit pas pouvoir être indemnisé" \
  --body "TransitionStatut autorise Refuse → Indemnise. Cette transition doit être interdite."

creer_pr() {
  local branche="$1" titre="$2" corps="$3"
  git checkout --quiet -B "$branche" "origin/$branche_defaut"
  git add -A "$sample"
  git commit --quiet -m "$titre"
  git push --quiet --force -u origin "$branche"
  gh pr create --repo "$depot" --base "$branche_defaut" --head "$branche" --label mon-sinistre --title "$titre" --body "$corps"
}

echo "==> PR 1 : prête à merger"
git checkout --quiet -B entrainement/libelles "origin/$branche_defaut"
sed -i.bak 's/Projet d'"'"'exemple pédagogique — toutes les données sont fictives./Projet d'"'"'exemple pédagogique — données 100 % fictives./' "$sample/src/MonSinistre.Web/wwwroot/index.html" && rm -f "$sample/src/MonSinistre.Web/wwwroot/index.html.bak"
creer_pr entrainement/libelles "[Entraînement] Clarifier le bandeau de la page web" \
  "PR d'entraînement : changement de texte uniquement. \`verify.sh\` doit rester vert."

echo "==> PR 2 : casse verify.sh"
git checkout --quiet -B entrainement/franchise-cassee "origin/$branche_defaut"
sed -i.bak 's/Assert.Equal(220000, /Assert.Equal(250000, /' "$sample/tests/MonSinistre.Domain.Tests/CalculIndemnisationTests.cs" && rm -f "$sample/tests/MonSinistre.Domain.Tests/CalculIndemnisationTests.cs.bak"
creer_pr entrainement/franchise-cassee "[Entraînement] Ajuster le test de franchise" \
  "PR d'entraînement : cette PR casse volontairement \`./samples/mon-sinistre/scripts/verify.sh\`. Trouve pourquoi et corrige-la."

echo "==> PR 3 : appelle une revue"
git checkout --quiet -B entrainement/montants "origin/$branche_defaut"
cat >> "$sample/src/MonSinistre.Domain/CalculIndemnisation.cs" <<'CS'

/// <summary>Encore une fonction de formatage des montants.</summary>
public static class FormatMontant
{
    public static string EnEuros(long c) => (c / 100) + "," + (Math.Abs(c % 100)).ToString("00") + " EUR";
}
CS
creer_pr entrainement/montants "[Entraînement] Ajouter un utilitaire de formatage" \
  "PR d'entraînement : elle compile, mais duplique encore le formatage des montants. Fais-la relire."
numero_pr="$(gh pr view --repo "$depot" entrainement/montants --json number -q .number)"
gh pr comment --repo "$depot" "$numero_pr" --body "Revue : il existe déjà \`CalculIndemnisation.FormaterMontant\` (et une copie dans le CLI). Peut-on factoriser plutôt que d'ajouter une troisième version ?"

echo "✅ Seed terminé sur $depot : 3 issues et 3 PR avec le label 'mon-sinistre'."
