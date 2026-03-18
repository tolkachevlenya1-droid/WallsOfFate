using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public GridManager grid;

    public Vector2Int gridPosition;

    public float moveTime = 0.2f;

    private Vector2 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public IEnumerator Move(Vector2Int direction)
    {
        Vector2Int targetPos = gridPosition + direction;

        GridCell targetCell = grid.GetCell(targetPos);

        if (targetCell == null)
        {
            Fail();
            yield break;
        }

        if (targetCell.CellType == CellType.Wall)
        {
            Fail();
            yield break;
        }

        Vector3 start = transform.position;
        Vector3 end = targetCell.transform.position;

        float t = 0;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / moveTime);
            yield return null;
        }

        transform.position = end;

        gridPosition = targetPos;

        if (targetCell.CellType == CellType.Argument)
        {
            CollectArgument(targetCell);
        }

        if (targetCell.CellType == CellType.Exit)
        {
            Win();
        }
    }

    public IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.3f);
    }

    void CollectArgument(GridCell cell)
    {
        Debug.Log("Argument collected");

        cell.CellType = CellType.Empty;

        cell.gameObject.SetActive(false);
    }

    void Win()
    {
        Debug.Log("Victory");
    }

    void Fail()
    {
        Debug.Log("Fail");
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        gridPosition = Vector2Int.zero;
    }
}