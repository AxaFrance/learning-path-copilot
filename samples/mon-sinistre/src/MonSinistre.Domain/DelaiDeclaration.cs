namespace MonSinistre.Domain;

/// <summary>Contrôle du délai de déclaration d'un sinistre (5 jours ouvrés).</summary>
public static class DelaiDeclaration
{
    public const int JoursOuvresMax = 5;

    public static bool EstDansLesDelais(DateOnly dateSurvenance, DateOnly dateDeclaration)
    {
        var jours = dateDeclaration.DayNumber - dateSurvenance.DayNumber;
        return jours >= 0 && jours <= JoursOuvresMax;
    }
}
