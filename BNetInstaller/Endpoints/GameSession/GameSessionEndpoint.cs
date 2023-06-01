using System.Threading.Tasks;
using BNetInstaller.Constants;
using BNetInstaller.Models;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Version;

internal class GameSessionEndpoint : BaseEndpoint
{
    public GameSessionModel Model { get; }
    public GameSessionEndpoint Uid { get; private set; }

    public GameSessionEndpoint(Requester requester) : base("gamesession", requester)
    {
        Model = new();
    }

    public async Task<JToken> Get()
    {
        using var response = await Requester.SendAsync(Endpoint + "/" + Model.uid, HttpVerb.GET);
        return await Deserialize(response);
    }

    public async Task<JToken> Post()
    {
        using var response = await Requester.SendAsync(Endpoint, HttpVerb.POST, Model);
        return await Deserialize(response);
    }
}
