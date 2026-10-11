using MonSinistre.Domain;

namespace MonSinistre.Domain.Tests;

public class TransitionStatutTests
{
    [Fact]
    public void Un_sinistre_declare_peut_passer_en_expertise()
    {
        Assert.True(TransitionStatut.EstAutorisee(StatutSinistre.Declare, StatutSinistre.EnExpertise));
    }

    [Fact]
    public void Un_sinistre_indemnise_est_definitif()
    {
        Assert.False(TransitionStatut.EstAutorisee(StatutSinistre.Indemnise, StatutSinistre.Refuse));
    }
}
