using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using BNetInstaller.Models;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints;

public class ProductEndpoint(string endpoint, Requester requester) : BaseEndpoint(endpoint, requester)
{
    public ProductModel Model { get; } = new();

    public async Task<JToken> Get()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint, HttpVerb.Get);
        return await Deserialize(response);
    }

    public async Task<JToken> Post()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint, HttpVerb.Post, Model);
        return await Deserialize(response);
    }

    public static ProductEndpoint CreateFromResponse(JToken content, Requester requester)
    {
        string responseUri = content.Value<string>("response_uri");
        return !string.IsNullOrEmpty(responseUri) ? new(responseUri.TrimStart('/'), requester) : null;
    }
}