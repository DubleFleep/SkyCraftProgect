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
        rb.isKinematic = false; // Включаем физику
        mainCamera = Camera.main;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        startPosition = transform.position; // Запоминаем стартовую позицию

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = mainCamera.WorldToScreenPoint(startPosition).z; // Фиксируем глубину Z

            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

            // Ограничиваем движение только по X и Y, оставляя Z неизменным
            Vector3 dragVector = new Vector3(mouseWorldPosition.x - startPosition.x, mouseWorldPosition.y - startPosition.y, 0);

            // Ограничиваем расстояние натяжения
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            transform.position = startPosition + dragVector;

            // Обновляем LineRenderer - ТЕПЕРЬ ЛИНИЯ ПОКАЗЫВАЕТ НАПРАВЛЕНИЕ ПОЛЕТА
            if (lineRenderer != null)
            {
                Vector3 flightDirection = startPosition - transform.position; // Вектор в сторону полета
                lineRenderer.SetPosition(0, transform.position); // Начало линии - там, где объект
                lineRenderer.SetPosition(1, transform.position + flightDirection * 1.5f); // Конец линии дальше в сторону полета
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            // Включаем физику
            rb.isKinematic = false;

            // Вычисляем силу броска (игнорируем Z)
            Vector3 force = (startPosition - transform.position).normalized * maxForce * Vector3.Distance(startPosition, transform.position);

            rb.linearVelocity = Vector3.zero; // Сбрасываем скорость перед броском
            rb.AddForce(force, ForceMode.Impulse);

            // Отключаем LineRenderer
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
