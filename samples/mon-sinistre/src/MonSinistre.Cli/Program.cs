using System.Globalization;
using MonSinistre.Domain;

var cheminDonnees = Environment.GetEnvironmentVariable("MON_SINISTRE_DATA")
    ?? Path.Combine(AppContext.BaseDirectory, "data", "sinistres.json");

if (!File.Exists(cheminDonnees))
{
    Console.Error.WriteLine($"Fichier de données introuvable : {cheminDonnees}");
    return 1;
}

var portefeuille = Portefeuille.Charger(cheminDonnees);
var commande = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (commande)
{
    case "list":
        Lister(portefeuille);
        return 0;
    case "show" when args.Length > 1:
        return Afficher(portefeuille, args[1]);
    case "declare" when args.Length > 4:
        return Declarer(portefeuille, cheminDonnees, args[1], args[2], args[3], args[4]);
    default:
        AfficherAide();
        return commande == "help" ? 0 : 1;
}

static void Lister(Portefeuille portefeuille)
{
    Console.WriteLine($"{"Numéro",-10} {"Contrat",-8} {"Type",-12} {"Montant",14} {"Statut",-12}");
    foreach (var s in portefeuille.Sinistres)
    {
        // Formatage local du montant (voir aussi CalculIndemnisation.FormaterMontant).
        var euros = s.MontantEstimeCentimes / 100;
        var centimes = Math.Abs(s.MontantEstimeCentimes % 100);
        var montant = euros.ToString("N0", CultureInfo.GetCultureInfo("fr-FR")) + "," + centimes.ToString("00") + " €";
        Console.WriteLine($"{s.Numero,-10} {s.NumeroContrat,-8} {s.Type,-12} {montant,14} {s.Statut,-12}");
    }
}

static int Afficher(Portefeuille portefeuille, string numero)
{
    var sinistre = portefeuille.TrouverSinistre(numero);
    if (sinistre is null)
    {
        Console.Error.WriteLine($"Sinistre introuvable : {numero}");
        return 1;
    }

    var contrat = portefeuille.TrouverContrat(sinistre.NumeroContrat);
    if (contrat is null)
    {
        Console.Error.WriteLine($"Contrat introuvable : {sinistre.NumeroContrat}");
        return 1;
    }

    Console.WriteLine($"Sinistre        : {sinistre.Numero}");
    Console.WriteLine($"Contrat         : {contrat.Numero} ({contrat.Assure}, formule {contrat.Formule})");
    Console.WriteLine($"Type            : {sinistre.Type}");
    Console.WriteLine($"Survenance      : {sinistre.DateSurvenance:yyyy-MM-dd}");
    Console.WriteLine($"Déclaration     : {sinistre.DateDeclaration:yyyy-MM-dd}");
    Console.WriteLine($"Dans les délais : {(DelaiDeclaration.EstDansLesDelais(sinistre.DateSurvenance, sinistre.DateDeclaration) ? "oui" : "non")}");
    Console.WriteLine($"Couvert         : {(RegleCouverture.EstCouvert(contrat.Formule, sinistre.Type) ? "oui" : "non")}");
    Console.WriteLine($"Montant estimé  : {CalculIndemnisation.FormaterMontant(sinistre.MontantEstimeCentimes)}");
    Console.WriteLine($"Franchise       : {CalculIndemnisation.FormaterMontant(contrat.FranchiseCentimes)}");
    Console.WriteLine($"Indemnisation   : {CalculIndemnisation.FormaterMontant(CalculIndemnisation.Calculer(contrat, sinistre))}");
    Console.WriteLine($"Statut          : {sinistre.Statut}");
    return 0;
}

static int Declarer(Portefeuille portefeuille, string chemin, string numeroContrat, string type, string dateSurvenance, string montantEuros)
{
    if (portefeuille.TrouverContrat(numeroContrat) is null)
    {
        Console.Error.WriteLine($"Contrat introuvable : {numeroContrat}");
        return 1;
    }

    if (!Enum.TryParse<TypeSinistre>(type, ignoreCase: true, out var typeSinistre)
        || !DateOnly.TryParseExact(dateSurvenance, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var survenance)
        || !decimal.TryParse(montantEuros, NumberStyles.Number, CultureInfo.InvariantCulture, out var montant)
        || montant < 0)
    {
        Console.Error.WriteLine("Paramètres invalides. Exemple : declare CTR-002 Vol 2026-10-01 1250.50");
        return 1;
    }

    var numero = $"SIN-{1001 + portefeuille.Sinistres.Count}";
    var sinistre = new Sinistre(
        numero,
        numeroContrat.ToUpperInvariant(),
        typeSinistre,
        survenance,
        DateOnly.FromDateTime(DateTime.Today),
        (long)Math.Round(montant * 100),
        StatutSinistre.Declare);

    portefeuille.Sinistres.Add(sinistre);
    portefeuille.Enregistrer(chemin);
    Console.WriteLine($"Sinistre {numero} déclaré.");
    return 0;
}

static void AfficherAide()
{
    Console.WriteLine("""
        Mon Sinistre — CLI d'exemple (données fictives)

        Usage :
          dotnet run --project src/MonSinistre.Cli -- list
          dotnet run --project src/MonSinistre.Cli -- show <numéro-sinistre>
          dotnet run --project src/MonSinistre.Cli -- declare <numéro-contrat> <BrisDeGlace|Vol|Collision> <aaaa-mm-jj> <montant-en-euros>
        """);
}
