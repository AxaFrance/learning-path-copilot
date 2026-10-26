namespace MonSinistre.Domain;

/// <summary>Calcule le montant indemnisé à l'assuré.</summary>
public static class CalculIndemnisation
{
    public static long Calculer(Contrat contrat, Sinistre sinistre)
    {
        if (!RegleCouverture.EstCouvert(contrat.Formule, sinistre.Type))
        {
            return 0;
        }

        return sinistre.MontantEstimeCentimes - contrat.FranchiseCentimes;
    }

    public static string FormaterMontant(long centimes)
    {
        var euros = centimes / 100;
        var reste = Math.Abs(centimes % 100);
        return euros.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("fr-FR")) + "," + reste.ToString("00") + " €";
    }
}
