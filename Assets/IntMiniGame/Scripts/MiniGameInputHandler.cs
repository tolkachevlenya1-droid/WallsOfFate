using UnityEngine;

public class MiniGameInputHandler : MonoBehaviour
{
    public CommandQueue queue;
    public ExecutionManager executor;

    void Update()
    {
        if (executor.IsExecuting) return;

        if (Input.GetKeyDown(KeyCode.W))
            queue.Add(CommandType.MoveUp);

        if (Input.GetKeyDown(KeyCode.S))
            queue.Add(CommandType.MoveDown);

        if (Input.GetKeyDown(KeyCode.A))
            queue.Add(CommandType.MoveLeft);

        if (Input.GetKeyDown(KeyCode.D))
            queue.Add(CommandType.MoveRight);

        if (Input.GetKeyDown(KeyCode.Space))
            queue.Add(CommandType.Pause);

        if (Input.GetKeyDown(KeyCode.Return))
            executor.StartExecution();

        if (Input.GetKeyDown(KeyCode.R))
            queue.RemoveLast();
    }
}