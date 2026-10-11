using MonSinistre.Domain;

namespace MonSinistre.Domain.Tests;

public class CalculIndemnisationTests
{
    private static readonly Contrat ContratTousRisques = new("CTR-T", "Assuré fictif", Formule.TousRisques, 30000);

    [Fact]
    public void Indemnisation_deduit_la_franchise()
    {
        var sinistre = new Sinistre("SIN-T", "CTR-T", TypeSinistre.Collision, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 2), 250000, StatutSinistre.EnExpertise);

        Assert.Equal(220000, CalculIndemnisation.Calculer(ContratTousRisques, sinistre));
    }

    [Fact]
    public void Sinistre_non_couvert_n_est_pas_indemnise()
    {
        var contratTiers = ContratTousRisques with { Formule = Formule.Tiers };
        var sinistre = new Sinistre("SIN-T", "CTR-T", TypeSinistre.Collision, new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 2), 250000, StatutSinistre.Declare);

        Assert.Equal(0, CalculIndemnisation.Calculer(contratTiers, sinistre));
    }

    [Fact]
    public void Formatage_en_euros()
    {
        Assert.EndsWith(",50 €", CalculIndemnisation.FormaterMontant(125050));
    }
}
