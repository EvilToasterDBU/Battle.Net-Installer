using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using BNetInstaller.Models;
using Newtonsoft.Json.Linq;

namespace BNetInstaller.Endpoints.Update;

public class UpdateEndpoint(Requester requester) : BaseEndpoint("update", requester)
{
    public ProductPriorityModel Model { get; } = new() { Priority = { Value = 699 } };
    public ProductEndpoint Product { get; private set; }

    public async Task<JToken> Get()
    {
        using var response = await Requester.SendAsync(Endpoint, HttpVerb.Get);
        return await Deserialize(response);
    }

    public async Task<JToken> Post()
    {
        using HttpResponseMessage response = await Requester.SendAsync(Endpoint, HttpVerb.Post, Model);
        JToken content = await Deserialize(response);
        Product = ProductEndpoint.CreateFromResponse(content, Requester);
        return content;
    }
}