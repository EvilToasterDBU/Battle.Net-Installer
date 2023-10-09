using System;
using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Agent;

internal class AgentEndpoint(Requester requester) : BaseEndpoint("agent", requester)
{
    public async Task Delete() => await Requester.SendAsync(Endpoint, HttpVerb.Delete);

    public async Task<JToken> Get()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint, HttpVerb.Get);
        return await Deserialize(response);
    }

    protected override void ValidateResponse(JToken response, string content)
    {
        string token = response.Value<string>("authorization");

        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Agent Error: Unable to authenticate.", new(content));
        }

        Requester.SetAuthorization(token);
        base.ValidateResponse(response, content);
    }
}