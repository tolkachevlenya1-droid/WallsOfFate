using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour
{
    public int maxCommands = 10;

    public List<CommandType> Commands = new List<CommandType>();

    public void Add(CommandType command)
    {
        if (Commands.Count >= maxCommands) return;

        Commands.Add(command);
    }

    public void RemoveLast()
    {
        if (Commands.Count == 0) return;

        Commands.RemoveAt(Commands.Count - 1);
    }

    public void Clear()
    {
        Commands.Clear();
    }
}