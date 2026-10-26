#!/usr/bin/env bash
# Vérifie que le projet Mon Sinistre compile et que ses tests passent.
# Usage : ./scripts/verify.sh   (depuis n'importe quel dossier)
set -euo pipefail

racine="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$racine"

echo "==> SDK .NET"
dotnet --version

echo "==> Restauration des dépendances"
dotnet restore MonSinistre.sln

echo "==> Compilation (avertissements traités comme des erreurs)"
dotnet build MonSinistre.sln --no-restore -warnaserror

echo "==> Tests"
dotnet test MonSinistre.sln --no-build

echo "✅ verify.sh : build et tests OK"
