using System.Collections.Generic;
using UnityEngine;

public static class AgilitySceneUtility
{
    public static T FindInLoadedScene<T>(string objectName = null) where T : Component
    {
        foreach (var candidate in Resources.FindObjectsOfTypeAll<T>())
        {
            if (candidate == null || !candidate.gameObject.scene.IsValid())
                continue;

            if (!string.IsNullOrEmpty(objectName) && candidate.gameObject.name != objectName)
                continue;

            return candidate;
        }

        return null;
    }

    public static Transform FindTransform(string objectName)
    {
        foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (transform == null || !transform.gameObject.scene.IsValid())
                continue;

            if (transform.name == objectName)
                return transform;
        }

        return null;
    }

    public static Vector3 ResolveArenaCenter(Transform explicitCenter = null)
    {
        if (explicitCenter != null)
            return explicitCenter.position;

        var board = FindTransform("Board");
        if (board != null)
            return board.position;

        return Vector3.zero;
    }

    public static float ResolveArenaRadius(Transform explicitCenter = null, float fallbackRadius = 4.5f)
    {
        if (explicitCenter != null)
        {
            var explicitRadius = ResolveRendererRadius(explicitCenter);
            if (explicitRadius > 0f)
                return explicitRadius;
        }

        var board = FindTransform("Board");
        if (board != null)
        {
            var boardRadius = ResolveRendererRadius(board);
            if (boardRadius > 0f)
                return boardRadius;
        }

        return fallbackRadius;
    }

    public static float ResolveTopY(Transform root, float fallbackY = 0f)
    {
        if (root != null && TryGetWorldBounds(root, out Bounds bounds))
            return bounds.max.y;

        return fallbackY;
    }

    public static float ResolveBottomOffset(Transform root, float fallbackOffset = 0f)
    {
        if (root != null && TryGetWorldBounds(root, out Bounds bounds))
            return bounds.min.y - root.position.y;

        return fallbackOffset;
    }

    public static bool TryGetWorldBounds(Transform root, out Bounds bounds)
    {
        bounds = default;
        if (root == null)
            return false;

        bool hasBounds = false;

        foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        foreach (var collider in root.GetComponentsInChildren<Collider>(true))
        {
            if (!collider.enabled)
                continue;

            if (!hasBounds)
            {
                bounds = collider.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        return hasBounds;
    }
    public static Vector3 CardinalPoint(Vector3 center, float radius, ArenaCardinal direction, float y)
    {
        Vector3 offset = direction switch
        {
            ArenaCardinal.North => Vector3.forward,
            ArenaCardinal.South => Vector3.back,
            ArenaCardinal.East => Vector3.right,
            ArenaCardinal.West => Vector3.left,
            _ => Vector3.forward
        };

        Vector3 point = center + offset * radius;
        point.y = y;
        return point;
    }

    public static Vector3 ClampToArena(Vector3 position, Vector3 center, float radius, float padding = 0f)
    {
        Vector3 planar = position - center;
        planar.y = 0f;

        float maxRadius = Mathf.Max(0f, radius - padding);
        if (planar.sqrMagnitude <= maxRadius * maxRadius)
            return position;

        Vector3 clamped = center + planar.normalized * maxRadius;
        clamped.y = position.y;
        return clamped;
    }

    public static ArenaCardinal[] ShuffleCardinals(System.Random rng = null)
    {
        var list = new List<ArenaCardinal>
        {
            ArenaCardinal.North,
            ArenaCardinal.South,
            ArenaCardinal.East,
            ArenaCardinal.West
        };

        rng ??= new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }

        return list.ToArray();
    }

    private static float ResolveRendererRadius(Transform root)
    {
        if (!TryGetWorldBounds(root, out Bounds bounds))
            return 0f;

        float radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
        return radius > 0.01f ? radius * 0.82f : 0f;
    }
}

public enum ArenaCardinal
{
    North,
    South,
    East,
    West
}
