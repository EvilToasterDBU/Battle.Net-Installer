using System.Threading.Tasks;
using BNetInstaller.Constants;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Game;

public class GameEndpoint(Requester requester) : BaseEndpoint("game", requester)
{
    public async Task<JToken> Get(string uid)
    {
        using var response = await Requester.SendAsync(Endpoint + "/" + uid, HttpVerb.Get);
        return await Deserialize(response);
    }
}