using System.Net.Http.Headers;
using System.Reflection;
using Apache.Ignite.Compute;

namespace IgniteCompute;

public class ManagementApi
{
     private const string BaseUri = "http://localhost:10300";

    public static async Task<DeploymentUnit> UnitDeploy(string unitId, string unitVersion, IList<string> unitContent)
    {
        var url = GetUnitUrl(unitId, unitVersion);

        var content = new MultipartFormDataContent();
        foreach (var file in unitContent)
        {
            // HttpClient will close the file.
            var fileContent = new StreamContent(File.OpenRead(file));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "unitContent", fileName: Path.GetFileName(file));
        }

        var request = new HttpRequestMessage(HttpMethod.Post, url.ToString())
        {
            Content = content
        };

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var client = new HttpClient();
        HttpResponseMessage response = await client.SendAsync(request);
        string resContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to deploy unit. Status code: {response.StatusCode}, Content: {resContent}");
        }

        return new DeploymentUnit(unitId, unitVersion);
    }

    public static async Task UnitUndeploy(DeploymentUnit unit)
    {
        using var client = new HttpClient();
        await client.DeleteAsync(GetUnitUrl(unit.Name, unit.Version).Uri);
    }

    public static async Task<DeploymentUnit> DeployAssembly<T>(string unitId, string? unitVersion = null)
    {
        var dllFile = typeof(T).Assembly.Location;

        var unitVersion0 = unitVersion ?? GetRandomUnitVersion();

        return await UnitDeploy(
            unitId: unitId,
            unitVersion: unitVersion0,
            unitContent: [dllFile]);
    }

    public static string GetRandomUnitVersion() => DateTime.Now.TimeOfDay.ToString(@"m\.s\.f");

    private static UriBuilder GetUnitUrl(string unitId, string unitVersion) =>
        new(BaseUri) { Path = $"/management/v1/deployment/units/{Uri.EscapeDataString(unitId)}/{Uri.EscapeDataString(unitVersion)}" };
}
