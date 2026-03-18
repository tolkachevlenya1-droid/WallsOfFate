using UnityEngine;
using System.Collections;

public class ExecutionManager : MonoBehaviour
{
    public PlayerController player;
    public CommandQueue queue;

    private bool isExecuting;

    public bool IsExecuting => isExecuting;

    public void StartExecution()
    {
        if (isExecuting || queue.Commands.Count == 0) return;

        StartCoroutine(Execute());
    }

    private IEnumerator Execute()
    {
        isExecuting = true;

        foreach (var cmd in queue.Commands)
        {
            switch (cmd)
            {
                case CommandType.MoveUp:
                    yield return player.Move(Vector2Int.up);
                    break;

                case CommandType.MoveDown:
                    yield return player.Move(Vector2Int.down);
                    break;

                case CommandType.MoveLeft:
                    yield return player.Move(Vector2Int.left);
                    break;

                case CommandType.MoveRight:
                    yield return player.Move(Vector2Int.right);
                    break;

                case CommandType.Pause:
                    yield return player.Pause();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }

        isExecuting = false;
    }
}