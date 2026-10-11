using MonSinistre.Domain;

namespace MonSinistre.Domain.Tests;

public class DelaiDeclarationTests
{
    [Fact]
    public void Declaration_le_lendemain_est_dans_les_delais()
    {
        Assert.True(DelaiDeclaration.EstDansLesDelais(new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 2)));
    }

    [Fact]
    public void Declaration_avant_survenance_est_refusee()
    {
        Assert.False(DelaiDeclaration.EstDansLesDelais(new DateOnly(2026, 9, 2), new DateOnly(2026, 9, 1)));
    }
}
