using System;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static System.Windows.Forms.AxHost;

namespace BNetInstaller;

internal class Requester : IDisposable
{
    public string BaseAddress { get; }

    private readonly HttpClient _client;
    private readonly JsonSerializerSettings _serializerSettings;

    public Requester(int port)
    {
        BaseAddress = $"http://127.0.0.1:{port}/{{0}}";

        _client = new();
        _client.DefaultRequestHeaders.Add("User-Agent", "phoenix-agent/1.0");

        _serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
    }

    public void SetAuthorization(string authorization, string session,string opt_in_feedback, string user_id, string user_name, string region, string state, string type, string version)
    {
        _client.DefaultRequestHeaders.Add("Authorization", authorization);
        _client.DefaultRequestHeaders.Add("Opt_in_feedback", opt_in_feedback);
        _client.DefaultRequestHeaders.Add("Session", session);
        _client.DefaultRequestHeaders.Add("User_id", user_id);
        _client.DefaultRequestHeaders.Add("User_name", user_name);
        _client.DefaultRequestHeaders.Add("Region", region);
        _client.DefaultRequestHeaders.Add("State", state);
        _client.DefaultRequestHeaders.Add("Type", type);
        _client.DefaultRequestHeaders.Add("Version", version);
    }

    public async Task<HttpResponseMessage> SendAsync(string endpoint, HttpVerb verb, string content = null)
    {
        var url = string.Format(BaseAddress, endpoint);
        var request = new HttpRequestMessage(new(verb.ToString()), url);

        if (verb != HttpVerb.GET && !string.IsNullOrEmpty(content))
            request.Content = new StringContent(content);

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            await HandleRequestFailure(response);

        return response;
    }

    public async Task<HttpResponseMessage> SendAsync<T>(string endpoint, HttpVerb verb, T payload = null) where T : class
    {
       
            var content = payload != null ?
                JsonConvert.SerializeObject(payload, _serializerSettings) :
                null;

            return await SendAsync(endpoint, verb, content);
    }

    private static async Task HandleRequestFailure(HttpResponseMessage response)
    {
        var uri = response.RequestMessage.RequestUri.AbsolutePath;
        var statusCode = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"{(int)statusCode} {statusCode}: {uri} {content}");
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
