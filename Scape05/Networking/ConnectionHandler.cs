using System.Net.Sockets;
using Scape05.Handlers;
using Scape05.IO;
using Scape05.Misc;

namespace Scape05.Entities;

public class ConnectionHandler
{
    private const int MaxClientsPerCycle = 10;

    public static void AcceptClients(TcpListener tcpListener)
    {
        for (var i = 0; i < MaxClientsPerCycle; i++)
        {
            if (!tcpListener.Pending())
                continue;

            var tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine($"+ Incoming Connection From: {tcpClient.Client.RemoteEndPoint}");

            if (tcpClient.Client.Available < 2)
            {
                CloseTcpClient(tcpClient);
                return;
            }

            var client = InitializeClient(tcpClient);

            try
            {
                AssignAvailablePlayerSlot(client);
                RegisterPlayer(client);
                var player = LoginHandler.Invoke(client);
                player.Login();
                return; /* Don't mind this return statement, mkay? */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static void CloseTcpClient(TcpClient tcpClient)
    {
        tcpClient.Client.Close();
        tcpClient.Client.Dispose();
    }

    private static Player AssignAvailablePlayerSlot(Player client)
    {
        for (var i = 1; i < Server.Players.Length; i++)
        {
            if (Server.Players[i] == null)
            {
                client.Index = i;
                Console.WriteLine($"Incoming connection has been assigned index: {client.Index}!");
                return client;
            }
        }

        throw new Exception("Server is full!");
    }

    private static void RegisterPlayer(Player player)
    {
        Server.Players[player.Index] = player;
        Console.WriteLine($"+ [{player.Index}] has been registered!");
    }

    private static Player InitializeClient(TcpClient tcpClient)
    {
        var client = new Player
        {
            Socket = tcpClient,
            NetworkStream = tcpClient.GetStream(),
            Reader = new RSStream(new byte[ServerConfig.BUFFER_SIZE]),
            Writer = new RSStream(new byte[ServerConfig.BUFFER_SIZE])
        };

        return client;
    }
}
