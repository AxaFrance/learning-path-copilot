# Vérifie que le projet Mon Sinistre compile et que ses tests passent.
# Usage : pwsh ./scripts/verify.ps1   (depuis n'importe quel dossier)
$ErrorActionPreference = 'Stop'

$racine = Split-Path -Parent $PSScriptRoot
Push-Location $racine
try {
    function Invoke-Etape([string] $titre, [scriptblock] $commande) {
        Write-Host "==> $titre"
        & $commande
        if ($LASTEXITCODE -ne 0) {
            throw "Échec de l'étape : $titre (code $LASTEXITCODE)"
        }
    }

    Invoke-Etape 'SDK .NET' { dotnet --version }
    Invoke-Etape 'Restauration des dépendances' { dotnet restore MonSinistre.sln }
    Invoke-Etape 'Compilation (avertissements traités comme des erreurs)' { dotnet build MonSinistre.sln --no-restore -warnaserror }
    Invoke-Etape 'Tests' { dotnet test MonSinistre.sln --no-build }

    Write-Host '✅ verify.ps1 : build et tests OK'
}
catch {
    Write-Error $_
    exit 1
}
finally {
    Pop-Location
}
