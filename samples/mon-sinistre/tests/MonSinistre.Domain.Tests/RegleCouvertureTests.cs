using MonSinistre.Domain;

namespace MonSinistre.Domain.Tests;

public class RegleCouvertureTests
{
    [Theory]
    [InlineData(TypeSinistre.BrisDeGlace)]
    [InlineData(TypeSinistre.Vol)]
    [InlineData(TypeSinistre.Collision)]
    public void Tous_risques_couvre_tous_les_types(TypeSinistre type)
    {
        Assert.True(RegleCouverture.EstCouvert(Formule.TousRisques, type));
    }

    [Fact]
    public void Tiers_ne_couvre_pas_la_collision()
    {
        Assert.False(RegleCouverture.EstCouvert(Formule.Tiers, TypeSinistre.Collision));
    }
}
