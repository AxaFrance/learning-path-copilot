namespace MonSinistre.Domain;

/// <summary>Indique si un type de sinistre est couvert par une formule.</summary>
public static class RegleCouverture
{
    public static bool EstCouvert(Formule formule, TypeSinistre type) => formule switch
    {
        Formule.Tiers => false,
        Formule.TiersPlus => type is TypeSinistre.BrisDeGlace or TypeSinistre.Vol,
        Formule.TousRisques => true,
        _ => false,
    };
}
