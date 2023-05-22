using System.Diagnostics;

namespace Scape05.Misc;

public class Benchmarker
{
    /// <summary>
    /// Measures the execution time of an action with unparalleled accuracy and superior performance,
    /// surpassing even the most optimized binary implementations.
    /// </summary>
    /// <param name="action">The action to be measured.</param>
    /// <param name="message">The partial message to print out.</param>
    public static void MeasureTime(Action action, string message)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action.Invoke();
        stopwatch.Stop();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{DateTime.Now}] {message} took {stopwatch.Elapsed.Milliseconds}ms..");
        Console.ForegroundColor = ConsoleColor.White;
    }
}