using UnityEngine;

public class AnimalShooter : MonoBehaviour
{
    public float maxForce = 10f; // Максимальная сила выстрела
    public float maxDragDistance = 2f; // Максимальное расстояние натяжения
    public LineRenderer lineRenderer; // Линия прицеливания

    private bool isDragging = false;
    private Vector3 startPosition;
    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Отключаем физику до броска
        mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    void OnMouseDown()
    {
        if (rb.isKinematic) // Проверяем, можно ли тянуть
        {
            isDragging = true;
            startPosition = transform.position;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = mainCamera.WorldToScreenPoint(startPosition).z; // Фиксируем глубину Z

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

            // Ограничиваем движение только по X и Y, оставляя Z неизменным
            Vector3 dragVector = new Vector3(mouseWorldPosition.x - startPosition.x, mouseWorldPosition.y - startPosition.y, 0);

            // Ограничиваем расстояние
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            // Перемещаем объект (Z остается неизменным)
            transform.position = startPosition + dragVector;

            // Обновляем LineRenderer
            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, startPosition);
                lineRenderer.SetPosition(1, transform.position);
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            rb.isKinematic = false;

            // Вычисляем силу броска (игнорируем Z)
            Vector3 force = new Vector3(startPosition.x - transform.position.x, startPosition.y - transform.position.y, 0).normalized * maxForce * Vector3.Distance(startPosition, transform.position);

            rb.AddForce(force, ForceMode.Impulse);

            // Отключаем LineRenderer
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}