using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DraggableTileGroup : MonoBehaviour
{
    public float liftHeight = 0.5f;
    public float moveSpeed = 30f;

    private Rigidbody rb;
    private Vector3 anchorOffset;
    private bool isDragging = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
            anchorOffset = transform.position - hit.point + Vector3.up * liftHeight;
        else
            anchorOffset = Vector3.up * liftHeight;

        transform.position += Vector3.up * liftHeight;

        rb.isKinematic = false;
        rb.drag = 5f;
        rb.angularDrag = 5f;
        isDragging = true;
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPos() + anchorOffset;
            Vector3 toTarget = (targetPos - rb.position);
            rb.velocity = toTarget * moveSpeed;
        }
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.isKinematic = true;
    }

    Vector3 GetMouseWorldPos()
    {
        var plane = new Plane(Vector3.up, 0);
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float enter);
        return ray.GetPoint(enter);
    }
}
