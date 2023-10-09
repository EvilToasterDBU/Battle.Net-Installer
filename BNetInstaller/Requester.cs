﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using BNetInstaller.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BNetInstaller;

public class Requester : IDisposable
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
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };
    }

    public void SetAuthorization(string authorization) => _client.DefaultRequestHeaders.Add("Authorization", authorization);

    public async Task<HttpResponseMessage> SendAsync(string endpoint, HttpVerb verb, string content = null)
    {
        string url = string.Format(BaseAddress, endpoint);
        HttpRequestMessage request = new (new HttpMethod(verb.ToString()), url);

        if (verb != HttpVerb.Get && !string.IsNullOrEmpty(content))
        { request.Content = new StringContent(content); }


        HttpResponseMessage response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode) { await HandleRequestFailure(response); }

        return response;
    }

    public async Task<HttpResponseMessage> SendAsync<T>(string endpoint, HttpVerb verb, T payload = null) where T : class
    {

        string content = payload != null ?
        JsonConvert.SerializeObject(payload, _serializerSettings) :
        null;

        return await SendAsync(endpoint, verb, content);
    }

    private static async Task HandleRequestFailure(HttpResponseMessage response)
    {
        var uri = response.RequestMessage?.RequestUri?.AbsolutePath ?? throw new Exception("Empty Uri");
        var statusCode = response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"{(int)statusCode} {statusCode}: {uri} {content}");
    }

    public void Dispose() => _client.Dispose();
}