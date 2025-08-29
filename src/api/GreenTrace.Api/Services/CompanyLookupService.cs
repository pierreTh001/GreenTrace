using System.Net.Http.Json;
using System.Text.Json;
using GreenTrace.Api.ViewModels.Companies;

namespace GreenTrace.Api.Services;

public class CompanyLookupService : ICompanyLookupService
{
    private readonly HttpClient _http;
    public CompanyLookupService(HttpClient http)
    {
        _http = http;
    }

    public async Task<IEnumerable<CompanyLookupItemViewModel>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return Enumerable.Empty<CompanyLookupItemViewModel>();

        var url = $"https://recherche-entreprises.api.gouv.fr/search?q={Uri.EscapeDataString(query)}&per_page=5";
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        using var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
            return Enumerable.Empty<CompanyLookupItemViewModel>();

        using var stream = await resp.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);
        var root = doc.RootElement;
        if (!root.TryGetProperty("results", out var results) || results.ValueKind != JsonValueKind.Array)
            return Enumerable.Empty<CompanyLookupItemViewModel>();

        var list = new List<CompanyLookupItemViewModel>();
        foreach (var item in results.EnumerateArray())
        {
            var name = item.GetPropertyOrDefault("nom_complet")
                       ?? item.GetPropertyOrDefault("nom_raison_sociale")
                       ?? item.GetPropertyOrDefault("denomination")
                       ?? item.GetPropertyOrDefault("nom_entreprise");
            var siren = item.GetPropertyOrDefault("siren");
            var naf = item.GetPropertyOrDefault("naf") ?? item.GetPropertyOrDefault("activite_principale");
            var natureJuridique = item.GetPropertyOrDefault("nature_juridique");

            string? siret = null, address = null, cp = null, city = null, country = "France";
            if (item.TryGetProperty("siege", out var siege) && siege.ValueKind == JsonValueKind.Object)
            {
                siret = siege.GetPropertyOrDefault("siret");
                address = siege.GetPropertyOrDefault("adresse");
                cp = siege.GetPropertyOrDefault("code_postal");
                city = siege.GetPropertyOrDefault("libelle_commune");
            }

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(siren))
            {
                list.Add(new CompanyLookupItemViewModel(
                    Name: name!,
                    LegalForm: natureJuridique,
                    Siren: siren!,
                    Siret: siret,
                    VatNumber: null,
                    NaceCode: naf,
                    AddressLine1: address,
                    PostalCode: cp,
                    City: city,
                    Country: country));
            }
        }

        return list;
    }
}

internal static class JsonExt
{
    public static string? GetPropertyOrDefault(this JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out var v))
        {
            if (v.ValueKind == JsonValueKind.String) return v.GetString();
            try { return v.ToString(); } catch { return null; }
        }
        return null;
    }
}
