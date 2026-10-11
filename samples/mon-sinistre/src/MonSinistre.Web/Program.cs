using System.Text.Json.Serialization;
using MonSinistre.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var cheminDonnees = builder.Configuration["MonSinistre:Data"]
    ?? Path.Combine(AppContext.BaseDirectory, "data", "sinistres.json");
var portefeuille = Portefeuille.Charger(cheminDonnees);
var verrou = new Lock();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/sinistres", () =>
{
    lock (verrou)
    {
        return portefeuille.Sinistres.Select(s => Vue(portefeuille, s)).ToList();
    }
});

app.MapGet("/api/sinistres/{numero}", (string numero) =>
{
    lock (verrou)
    {
        var sinistre = portefeuille.TrouverSinistre(numero);
        return sinistre is null ? Results.NotFound() : Results.Ok(Vue(portefeuille, sinistre));
    }
});

app.MapPost("/api/sinistres", (DemandeDeclaration demande) =>
{
    if (demande.MontantEstimeEuros < 0)
    {
        return Results.BadRequest("Le montant doit être positif.");
    }

    lock (verrou)
    {
        if (portefeuille.TrouverContrat(demande.NumeroContrat) is null)
        {
            return Results.BadRequest($"Contrat introuvable : {demande.NumeroContrat}");
        }

        var sinistre = new Sinistre(
            $"SIN-{1001 + portefeuille.Sinistres.Count}",
            demande.NumeroContrat.ToUpperInvariant(),
            demande.Type,
            demande.DateSurvenance,
            DateOnly.FromDateTime(DateTime.Today),
            (long)Math.Round(demande.MontantEstimeEuros * 100),
            StatutSinistre.Declare);
        portefeuille.Sinistres.Add(sinistre);
        return Results.Created($"/api/sinistres/{sinistre.Numero}", Vue(portefeuille, sinistre));
    }
});

app.Run();

static VueSinistre Vue(Portefeuille portefeuille, Sinistre s)
{
    var contrat = portefeuille.TrouverContrat(s.NumeroContrat);
    var indemnisation = contrat is null ? 0 : CalculIndemnisation.Calculer(contrat, s);
    return new VueSinistre(
        s.Numero,
        s.NumeroContrat,
        contrat?.Formule.ToString() ?? "?",
        s.Type,
        s.DateSurvenance,
        s.DateDeclaration,
        DelaiDeclaration.EstDansLesDelais(s.DateSurvenance, s.DateDeclaration),
        CalculIndemnisation.FormaterMontant(s.MontantEstimeCentimes),
        CalculIndemnisation.FormaterMontant(indemnisation),
        s.Statut);
}

internal sealed record DemandeDeclaration(string NumeroContrat, TypeSinistre Type, DateOnly DateSurvenance, decimal MontantEstimeEuros);

internal sealed record VueSinistre(
    string Numero,
    string NumeroContrat,
    string Formule,
    TypeSinistre Type,
    DateOnly DateSurvenance,
    DateOnly DateDeclaration,
    bool DansLesDelais,
    string MontantEstime,
    string Indemnisation,
    StatutSinistre Statut);
