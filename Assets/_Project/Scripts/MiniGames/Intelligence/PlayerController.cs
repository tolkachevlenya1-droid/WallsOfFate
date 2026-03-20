using System.Collections;
using UnityEngine;

namespace Game
{

    public class PlayerController : MonoBehaviour
    {
        public GridManager grid;
        public Vector2Int gridPosition;
        public float moveTime = 0.2f;

        [Header("Route Settings")]
        [SerializeField] private RouteDirection startingDirection = RouteDirection.Up;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float turnTime = 0.15f;
        [SerializeField] private float heightOffset = 0.35f;
        [SerializeField] private Vector3 visualRotationOffset;
        [SerializeField] private bool deriveGridPositionFromTransform = true;

        private Vector2Int _startGridPosition;
        private RouteDirection _startDirection;
        private bool _startCaptured;

        public RouteDirection FacingDirection { get; private set; }
        public Vector2Int StartGridPosition => _startGridPosition;
        public RouteDirection StartDirection => _startDirection;

        public void Initialize(GridManager targetGrid)
        {
            grid = targetGrid != null ? targetGrid : grid;

            if (grid == null)
            {
                grid = FindObjectOfType<GridManager>();
            }

            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            SyncGridPositionFromTransform();

            if (!_startCaptured)
            {
                _startGridPosition = gridPosition;
                _startDirection = startingDirection;
                _startCaptured = true;
            }

            FacingDirection = startingDirection;
            SnapToGrid();
        }

        public void SetStartingDirection(RouteDirection direction, bool snapImmediately)
        {
            startingDirection = direction;
            _startDirection = direction;

            if (snapImmediately)
            {
                FacingDirection = direction;
                SnapToGrid();
            }
        }

        public void ResetToStart()
        {
            StopAllCoroutines();
            gridPosition = _startGridPosition;
            FacingDirection = _startDirection;
            SnapToGrid();
        }

        public IEnumerator AnimateTurn(RouteDirection newDirection)
        {
            FacingDirection = newDirection;

            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            Quaternion startRotation = visualRoot.rotation;
            Quaternion targetRotation = GetFacingRotation();

            if (turnTime <= 0.01f)
            {
                visualRoot.rotation = targetRotation;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < turnTime)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / turnTime);
                visualRoot.rotation = Quaternion.Slerp(startRotation, targetRotation, progress);
                yield return null;
            }

            visualRoot.rotation = targetRotation;
        }

        public IEnumerator AnimateMoveTo(Vector2Int targetPosition)
        {
            if (grid == null)
            {
                yield break;
            }

            Vector3 startPosition = transform.position;
            gridPosition = targetPosition;
            Vector3 targetWorldPosition = GetCurrentWorldPosition();

            if (moveTime <= 0.01f)
            {
                transform.position = targetWorldPosition;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / moveTime);
                transform.position = Vector3.Lerp(startPosition, targetWorldPosition, progress);
                yield return null;
            }

            transform.position = targetWorldPosition;
        }

        public Vector2Int PeekPosition(RouteDirection direction)
        {
            return gridPosition + RouteDirectionUtility.ToVector2Int(direction);
        }

        public Vector2Int PeekForwardPosition()
        {
            return PeekPosition(FacingDirection);
        }

        private void Awake()
        {
            Initialize(grid);
        }

        private void OnValidate()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }

            if (!Application.isPlaying)
            {
                SyncGridPositionFromTransform();
            }
        }

        private void SnapToGrid()
        {
            if (grid == null)
            {
                return;
            }

            transform.position = GetCurrentWorldPosition();

            if (visualRoot != null)
            {
                visualRoot.rotation = GetFacingRotation();
            }
        }

        private Vector3 GetCurrentWorldPosition()
        {
            return grid.GetWorldPosition(gridPosition) + grid.GetSurfaceNormal() * heightOffset;
        }

        private void SyncGridPositionFromTransform()
        {
            if (!deriveGridPositionFromTransform)
            {
                return;
            }

            if (grid == null)
            {
                grid = FindObjectOfType<GridManager>();
            }

            if (grid != null && grid.TryGetGridPositionFromWorld(transform.position, out Vector2Int resolvedPosition))
            {
                gridPosition = resolvedPosition;
            }
        }

        private Quaternion GetFacingRotation()
        {
            if (grid == null)
            {
                return transform.rotation;
            }

            Vector3 worldDirection = grid.GetWorldDirection(FacingDirection);
            if (worldDirection.sqrMagnitude < 0.001f)
            {
                return transform.rotation;
            }

            Quaternion baseRotation = Quaternion.LookRotation(worldDirection, grid.GetSurfaceNormal());
            return baseRotation * Quaternion.Euler(visualRotationOffset);
        }
    }
}