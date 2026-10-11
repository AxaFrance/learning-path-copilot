using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonSinistre.Domain;

/// <summary>Jeu de données fictif : contrats et sinistres chargés depuis un fichier JSON.</summary>
public sealed class Portefeuille
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true,
    };

    public List<Contrat> Contrats { get; init; } = [];

    public List<Sinistre> Sinistres { get; init; } = [];

    public Contrat? TrouverContrat(string numero) =>
        Contrats.FirstOrDefault(c => c.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));

    public Sinistre? TrouverSinistre(string numero) =>
        Sinistres.FirstOrDefault(s => s.Numero.Equals(numero, StringComparison.OrdinalIgnoreCase));

    public static Portefeuille Charger(string chemin)
    {
        using var flux = File.OpenRead(chemin);
        return JsonSerializer.Deserialize<Portefeuille>(flux, Options)
            ?? throw new InvalidOperationException($"Fichier de données vide : {chemin}");
    }

    public void Enregistrer(string chemin)
    {
        File.WriteAllText(chemin, JsonSerializer.Serialize(this, Options));
    }
}
