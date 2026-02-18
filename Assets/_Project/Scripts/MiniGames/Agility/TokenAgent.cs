using System.Collections;
using UnityEngine;
public class TokenAgent : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float arriveDistance = 0.05f;
    public bool lockY = true;

    private float _y;

    private void Awake()
    {
        _y = transform.position.y;
    }

    public IEnumerator MoveTo(Vector3 target)
    {
        if (lockY) target.y = _y;

        while ((transform.position - target).sqrMagnitude > arriveDistance * arriveDistance)
        {
            Vector3 next = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            if (lockY) next.y = _y;
            transform.position = next;
            yield return null;
        }
    }
}
