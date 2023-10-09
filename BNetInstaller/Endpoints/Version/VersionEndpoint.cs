using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using BNetInstaller.Models;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Version;

public class VersionEndpoint(Requester requester) : BaseEndpoint("version", requester)
{
    public UidModel Model { get; } = new();

    public async Task<JToken> Get()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint + "/" + Model.Uid, HttpVerb.Get);
        return await Deserialize(response);
    }

    public async Task<JToken> Post()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint, HttpVerb.Post, Model);
        return await Deserialize(response);
    }
}