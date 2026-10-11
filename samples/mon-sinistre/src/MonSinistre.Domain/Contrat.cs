namespace MonSinistre.Domain;

/// <summary>Contrat auto (données fictives). Les montants sont exprimés en centimes.</summary>
public sealed record Contrat(
    string Numero,
    string Assure,
    Formule Formule,
    long FranchiseCentimes);
