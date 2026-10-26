namespace MonSinistre.Domain;

/// <summary>Transitions de statut autorisées pour un dossier sinistre.</summary>
public static class TransitionStatut
{
    private static readonly Dictionary<StatutSinistre, StatutSinistre[]> Autorisees = new()
    {
        [StatutSinistre.Declare] = [StatutSinistre.EnExpertise, StatutSinistre.Refuse],
        [StatutSinistre.EnExpertise] = [StatutSinistre.Indemnise, StatutSinistre.Refuse],
        [StatutSinistre.Indemnise] = [],
        [StatutSinistre.Refuse] = [StatutSinistre.Indemnise],
    };

    public static bool EstAutorisee(StatutSinistre depuis, StatutSinistre vers) =>
        Autorisees.TryGetValue(depuis, out var cibles) && cibles.Contains(vers);
}
