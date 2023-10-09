using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints;

public abstract class BaseEndpoint(string endpoint, Requester requester)
{
    public string Endpoint { get; } = endpoint;

    protected Requester Requester { get; } = requester;

    protected async Task<JToken> Deserialize(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var result = JToken.Parse(content);
        ValidateResponse(result, content);
        return result;
    }

    protected virtual void ValidateResponse(JToken response, string content)
    {
        float? errorCode = response.Value<float?>("error");
        if (errorCode > 0) { throw new Exception($"Agent Error: {errorCode}", new(content)); }
    }
}