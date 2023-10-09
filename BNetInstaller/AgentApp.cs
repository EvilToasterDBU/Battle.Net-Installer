using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using BNetInstaller.Endpoints.Agent;
using BNetInstaller.Endpoints.Game;
using BNetInstaller.Endpoints.Install;
using BNetInstaller.Endpoints.Repair;
using BNetInstaller.Endpoints.Update;
using BNetInstaller.Endpoints.Version;

namespace BNetInstaller;

internal class AgentApp : IDisposable
{
    public readonly AgentEndpoint AgentEndpoint;
    public readonly InstallEndpoint InstallEndpoint;
    public readonly UpdateEndpoint UpdateEndpoint;
    public readonly RepairEndpoint RepairEndpoint;
    public readonly GameEndpoint GameEndpoint;
    public readonly VersionEndpoint VersionEndpoint;

    private readonly string _agentPath;
    private readonly int _port = 5050;

    private Process _process;
    private Requester _requester;

    public AgentApp()
    {
        _agentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Battle.net", "Agent", "Agent.exe");

        if (!StartProcess())
        {
            Console.WriteLine("Please ensure Battle.net is installed and has recently been opened.");
            Environment.Exit(0);
        }

        AgentEndpoint = new(_requester);
        InstallEndpoint = new(_requester);
        UpdateEndpoint = new(_requester);
        RepairEndpoint = new(_requester);
        GameEndpoint = new(_requester);
        VersionEndpoint = new(_requester);
    }

    private bool StartProcess()
    {
        if (!File.Exists(_agentPath))
        {
            Console.WriteLine("Unable to find Agent.exe.");
            return false;
        }

        try
        {
            _process = Process.Start(_agentPath, $"--port={_port}");
            _requester = new Requester(_port);
            return true;
        }
        catch (Win32Exception)
        {
            Console.WriteLine("Unable to start Agent.exe.");
            return false;
        }
    }

    public void Dispose()
    {
        if (_process?.HasExited == false)
            _process.Kill();

        _requester?.Dispose();
        _process?.Dispose();
    }
}