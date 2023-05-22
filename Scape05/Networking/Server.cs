using Scape05.Entities.Packets;
using Scape05.Misc;

namespace Scape05.Entities;

public class Server
{
    public static Player[] Players { get; } = new Player[ServerConfig.MAX_PLAYERS];
    public static NPC[] NPCs { get; } = new NPC[ServerConfig.MAX_NPCS];

    public void Process()
    {
        FetchPackets();
        UpdatePlayers();
        FlushClients();
        ResetPlayers();
    }

    private void FetchPackets()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            for (var j = 0; j < 10; j++)
                player.PacketHandler.Fetch();

            for (int i = 0; i < 10; i++)
            {
                player.PacketHandler.Build();
            }

            player.MovementHandler.Process();
        }
    }

    /* Update players according to the new data that we've received from packets */
    private void UpdatePlayers()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.PlayerUpdater.UpdateLocalPlayer();
        }
    }

    /* Send packets that we've accumulated */
    private void FlushClients()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.DirectFlushStream();
        }
    }

    /* Reset player state */
    private void ResetPlayers()
    {
        foreach (var player in Players)
        {
            if (player == null)
                continue;

            player.Reset();
        }
    }

    public static void AddNPC(NPC npc)
    {
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null) continue;

            NPCs[i] = npc;
            break;
        }

        Console.WriteLine("Can't add anymore NPCs");
    }
}