using System;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using BNetInstaller.Models;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Install;

public class InstallEndpoint(Requester requester) : BaseEndpoint("install", requester)
{
    public InstallModel Model { get; } = new();
    public ProductEndpoint Product { get; private set; }

    public async Task<JToken> Post()
    {
        using var response = await Requester.SendAsync(Endpoint, HttpVerb.Post, Model);
        var content = await Deserialize(response);
        Product = ProductEndpoint.CreateFromResponse(content, Requester);
        return content;
    }

    protected override void ValidateResponse(JToken response, string content)
    {
        float? agentError = response.Value<float?>("error");

        if (agentError > 0)
        {
            // try to identify the erroneous section
            foreach (var section in SubSections)
            {
                var token = response["form"]?[section];
                var errorCode = token?.Value<float?>("error");
                if (errorCode > 0)
                    throw new Exception($"Agent Error: Unable to install - {errorCode} ({section}).", new(content));
            }

            // fallback to throwing a global error
            throw new Exception($"Agent Error: {agentError}", new(content));
        }
    }

    private static readonly string[] SubSections = new[]
    {
        "authentication",
        "game_dir",
        "min_spec"
    };
}