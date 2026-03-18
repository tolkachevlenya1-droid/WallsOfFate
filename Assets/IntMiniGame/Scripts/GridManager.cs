using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 3;
    public int height = 3;
    public List<GridCell> cells = new();

    [Header("Virtual Grid")]
    [SerializeField] private Transform origin;
    [SerializeField] private RouteBoardPlane boardPlane = RouteBoardPlane.XZ;
    [SerializeField] private Vector2 cellSpacing = Vector2.one;
    [SerializeField] private float surfaceOffset;
    [SerializeField] private bool autoCollectCellsFromChildren = true;
    [SerializeField] private bool autoCollectOccupantsFromScene = true;

    private readonly Dictionary<Vector2Int, GridCell> _cellLookup = new();
    private readonly Dictionary<Vector2Int, List<RouteGridOccupant>> _occupantLookup = new();

    public int RemainingArguments { get; private set; }
    public bool HasExitCell { get; private set; }

    public void RefreshLayout()
    {
        if ((cells == null || cells.Count == 0) && autoCollectCellsFromChildren)
        {
            cells = new List<GridCell>(GetComponentsInChildren<GridCell>(true));
        }

        _cellLookup.Clear();

        if (cells != null)
        {
            for (int index = 0; index < cells.Count; index++)
            {
                GridCell cell = cells[index];
                if (cell == null)
                {
                    continue;
                }

                _cellLookup[cell.GridPosition] = cell;
            }
        }

        BuildOccupantLookup();
        ResetBoardState();
    }

    public void ResetBoardState()
    {
        HasExitCell = false;
        RemainingArguments = 0;

        foreach (KeyValuePair<Vector2Int, GridCell> pair in _cellLookup)
        {
            GridCell cell = pair.Value;
            if (cell == null)
            {
                continue;
            }

            cell.ResetState();

            if (cell.HasAvailableArgument)
            {
                RemainingArguments++;
            }

            if (cell.IsExit())
            {
                HasExitCell = true;
            }
        }

        foreach (KeyValuePair<Vector2Int, List<RouteGridOccupant>> pair in _occupantLookup)
        {
            List<RouteGridOccupant> occupants = pair.Value;
            for (int index = 0; index < occupants.Count; index++)
            {
                RouteGridOccupant occupant = occupants[index];
                if (occupant == null)
                {
                    continue;
                }

                occupant.ResetState();

                if (occupant.HasAvailableArgument)
                {
                    RemainingArguments++;
                }

                if (occupant.IsExit)
                {
                    HasExitCell = true;
                }
            }
        }
    }

    public bool IsInside(Vector2Int position)
    {
        if (_cellLookup.Count > 0)
        {
            return _cellLookup.ContainsKey(position);
        }

        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }

    public bool IsBlocked(Vector2Int position)
    {
        if (_cellLookup.TryGetValue(position, out GridCell cell) && cell != null && cell.IsBlocked())
        {
            return true;
        }

        if (_occupantLookup.TryGetValue(position, out List<RouteGridOccupant> occupants))
        {
            for (int index = 0; index < occupants.Count; index++)
            {
                RouteGridOccupant occupant = occupants[index];
                if (occupant != null && occupant.BlocksMovement)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsForbidden(Vector2Int position)
    {
        if (_cellLookup.TryGetValue(position, out GridCell cell) && cell != null && cell.IsForbidden())
        {
            return true;
        }

        if (_occupantLookup.TryGetValue(position, out List<RouteGridOccupant> occupants))
        {
            for (int index = 0; index < occupants.Count; index++)
            {
                RouteGridOccupant occupant = occupants[index];
                if (occupant != null && occupant.IsForbidden)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsExit(Vector2Int position)
    {
        if (_cellLookup.TryGetValue(position, out GridCell cell) && cell != null && cell.IsExit())
        {
            return true;
        }

        if (_occupantLookup.TryGetValue(position, out List<RouteGridOccupant> occupants))
        {
            for (int index = 0; index < occupants.Count; index++)
            {
                RouteGridOccupant occupant = occupants[index];
                if (occupant != null && occupant.IsExit)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public int CollectArguments(Vector2Int position)
    {
        int collected = 0;

        if (_cellLookup.TryGetValue(position, out GridCell cell) && cell != null && cell.TryCollectArgument())
        {
            collected++;
        }

        if (_occupantLookup.TryGetValue(position, out List<RouteGridOccupant> occupants))
        {
            for (int index = 0; index < occupants.Count; index++)
            {
                RouteGridOccupant occupant = occupants[index];
                if (occupant != null && occupant.TryCollectArgument())
                {
                    collected++;
                }
            }
        }

        if (collected > 0)
        {
            RemainingArguments = Mathf.Max(0, RemainingArguments - collected);
        }

        return collected;
    }

    public Vector3 GetWorldPosition(Vector2Int position)
    {
        if (_cellLookup.TryGetValue(position, out GridCell cell) && cell != null)
        {
            return cell.transform.position + GetSurfaceNormal() * surfaceOffset;
        }

        Transform targetOrigin = origin != null ? origin : transform;
        Vector3 localOffset = boardPlane == RouteBoardPlane.XY
            ? new Vector3(position.x * cellSpacing.x, position.y * cellSpacing.y, 0f)
            : new Vector3(position.x * cellSpacing.x, 0f, position.y * cellSpacing.y);

        return targetOrigin.TransformPoint(localOffset) + GetSurfaceNormal() * surfaceOffset;
    }

    public Vector3 GetSurfaceNormal()
    {
        Transform targetOrigin = origin != null ? origin : transform;
        return boardPlane == RouteBoardPlane.XY ? targetOrigin.forward : targetOrigin.up;
    }

    public Vector3 GetWorldDirection(RouteDirection direction)
    {
        Vector2Int gridOffset = RouteDirectionUtility.ToVector2Int(direction);

        foreach (KeyValuePair<Vector2Int, GridCell> pair in _cellLookup)
        {
            Vector2Int from = pair.Key;
            Vector2Int to = from + gridOffset;
            if (!IsInside(to))
            {
                continue;
            }

            Vector3 vector = GetWorldPosition(to) - GetWorldPosition(from);
            if (vector.sqrMagnitude > 0.0001f)
            {
                return vector.normalized;
            }
        }

        Transform targetOrigin = origin != null ? origin : transform;
        return direction switch
        {
            RouteDirection.Up => boardPlane == RouteBoardPlane.XY ? targetOrigin.up : targetOrigin.forward,
            RouteDirection.Right => targetOrigin.right,
            RouteDirection.Down => boardPlane == RouteBoardPlane.XY ? -targetOrigin.up : -targetOrigin.forward,
            RouteDirection.Left => -targetOrigin.right,
            _ => targetOrigin.forward
        };
    }

    private void Awake()
    {
        RefreshLayout();
    }

    private void OnValidate()
    {
        RefreshLayout();
    }

    private void BuildOccupantLookup()
    {
        _occupantLookup.Clear();

        if (!autoCollectOccupantsFromScene)
        {
            return;
        }

        RouteGridOccupant[] occupants = FindObjectsOfType<RouteGridOccupant>(true);
        for (int index = 0; index < occupants.Length; index++)
        {
            RouteGridOccupant occupant = occupants[index];
            if (occupant == null)
            {
                continue;
            }

            if (!_occupantLookup.TryGetValue(occupant.GridPosition, out List<RouteGridOccupant> list))
            {
                list = new List<RouteGridOccupant>();
                _occupantLookup.Add(occupant.GridPosition, list);
            }

            list.Add(occupant);
        }
    }
}
