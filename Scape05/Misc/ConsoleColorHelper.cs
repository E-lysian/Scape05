using Scape05.Entities;

namespace Scape05.Misc;

public abstract class ConsoleColorHelper
{
    public static void HandleConsoleHelper(IEntity? Attacker)
    {
        if (Attacker == null) throw new NullReferenceException("Attacker is null.");
        switch (Attacker)
        {
            case Player:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case NPC:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
        }
    }

    public static void Broadcast(int type, string message)
    {
        switch (type)
        {
            case 0:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                break;
            case 1:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                break;
            case 2:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(message);
                break;
        }

        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void ResetColor()
    {
        Console.ForegroundColor = ConsoleColor.White;
    }
}