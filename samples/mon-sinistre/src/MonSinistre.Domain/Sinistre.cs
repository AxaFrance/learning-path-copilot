namespace MonSinistre.Domain;

/// <summary>Sinistre déclaré sur un contrat. Les montants sont exprimés en centimes.</summary>
public sealed record Sinistre(
    string Numero,
    string NumeroContrat,
    TypeSinistre Type,
    DateOnly DateSurvenance,
    DateOnly DateDeclaration,
    long MontantEstimeCentimes,
    StatutSinistre Statut);
