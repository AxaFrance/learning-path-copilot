namespace MonSinistre.Domain;

/// <summary>Cycle de vie d'un dossier sinistre.</summary>
public enum StatutSinistre
{
    Declare,
    EnExpertise,
    Indemnise,
    Refuse,
}
