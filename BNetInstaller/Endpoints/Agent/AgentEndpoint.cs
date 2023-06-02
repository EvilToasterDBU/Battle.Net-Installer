using System;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Agent;

internal class AgentEndpoint : BaseEndpoint
{
    public AgentEndpoint(Requester requester) : base("agent", requester)
    {
    }

    public async Task Delete()
    {
        await Requester.SendAsync(Endpoint, HttpVerb.DELETE);
    }

    public async Task<JToken> Get()
    {
        using var response = await Requester.SendAsync(Endpoint, HttpVerb.GET);
        return await Deserialize(response);
    }

    protected override void ValidateResponse(JToken response, string content)
    {
        var token = response.Value<string>("authorization");
        var opt_in_feedback = response.Value<string>("opt_in_feedback");
        var pid = response.Value<string>("pid");
        var session = response.Value<string>("session");
        var user_id = response.Value<string>("user_id");
        var user_name = response.Value<string>("user_name");
        var region = response.Value<string>("region");
        var state = response.Value<string>("state");
        var type = response.Value<string>("type");
        var version = response.Value<string>("version");

        if (string.IsNullOrEmpty(token))
            throw new Exception("Agent Error: Unable to authenticate.", new(content));

        Requester.SetAuthorization(token, opt_in_feedback, pid, session, user_id, user_name, region, state, type, version);
        base.ValidateResponse(response, content);
    }
}
