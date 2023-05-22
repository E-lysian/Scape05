using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Scape05.Entities;
using Scape05.Misc;

namespace Scape05.Engine;

public class GameEngine
{
    private bool _isRunning;
    private TcpListener _listener;
    private readonly Server _server;

    public GameEngine()
    {
        _server = new Server();
    }

    public void Start()
    {
        Initialize();
        var stopwatch = new Stopwatch();
        while (_isRunning)
        {
            stopwatch.Start();
            Tick();
            stopwatch.Stop();

            var sleepDelta = ServerConfig.TICK_RATE - (int)stopwatch.ElapsedMilliseconds;
            if (sleepDelta > 0)
                Thread.Sleep(sleepDelta);
            else
                Console.WriteLine($"Server can't keep up!\nElapsed: {(int)stopwatch.ElapsedMilliseconds}\nElapsed: {sleepDelta}");
            stopwatch.Reset();
        }
    }

    private void Initialize()
    {
        _listener = new TcpListener(IPAddress.Loopback, ServerConfig.PORT);
        _listener.Start(10);
        _isRunning = true;

        Console.WriteLine( $"[{DateTime.Now}] Server has started!");
    }

    private void Tick()
    {
        AcceptClients();
        ProcessWorld();
    }

    private void AcceptClients()
    {
        ConnectionHandler.AcceptClients(_listener);
    }

    private void ProcessWorld()
    {
        _server.Process();
    }
}