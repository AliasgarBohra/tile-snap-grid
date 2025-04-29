using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class DraggableTile : MonoBehaviour
{
    [Header("Grid & Drag Settings")]
    [SerializeField] private float liftHeight = 0.5f;

    private Rigidbody rb;
    private Vector3 anchorOffset;
    private Vector2Int startGridPos;
    private bool isDragging = false;

    private Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.isKinematic = true;
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        startGridPos = GridManager.Instance.GetGridPosition(transform.position);
        GridManager.Instance.Vacate(startGridPos);

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit) && hit.collider == GetComponent<Collider>())
            anchorOffset = transform.position - hit.point + Vector3.up * liftHeight;
        else
            anchorOffset = Vector3.up * liftHeight;

        transform.position += Vector3.up * liftHeight;

        rb.isKinematic = false;
        rb.drag = 5f;
        rb.angularDrag = 5f;

        isDragging = true;
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 targetPos = GetMouseWorldPos() + anchorOffset;
            Vector3 toTarget = targetPos - rb.position;
            float moveSpeed = 30f;
            rb.velocity = toTarget * moveSpeed;
        }
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.drag = 0f;
        rb.angularDrag = 0.05f;
        rb.isKinematic = true;

        Vector2Int gridPos = GridManager.Instance.GetGridPosition(transform.position);
        bool bad = !GridManager.Instance.IsWithinBounds(gridPos) || GridManager.Instance.IsOccupied(gridPos);

        if (bad)
        {
            transform.position = GridManager.Instance.GetWorldPosition(startGridPos);
            GridManager.Instance.Occupy(startGridPos, gameObject);
        }
        else
        {
            transform.position = GridManager.Instance.GetWorldPosition(gridPos);
            GridManager.Instance.Occupy(gridPos, gameObject);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        var plane = new Plane(Vector3.up, 0);
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float enter);
        return ray.GetPoint(enter);
    }
}