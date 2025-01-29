using UnityEngine;

public class AnimalShooter : MonoBehaviour
{
    public float maxForce = 10f;       // Максимальная сила выстрела
    public float maxDragDistance = 2f; // Максимальное расстояние натяжения
    public LineRenderer lineRenderer;  // Линия прицеливания

    private bool isDragging = false;
    private Vector3 startPosition;
    private Rigidbody rb;
    private Camera mainCamera;

    // При старте вызываем нашу инициализацию
    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Метод инициализации. Вызывается в Start и может быть вызван вручную после Instantiate.
    /// </summary>
    public void Initialize()
    {
        // Берём/обновляем ссылки на компоненты
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;   // По умолчанию кинематический, чтобы не падал
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Ищем основную камеру
        mainCamera = Camera.main;

        // Настраиваем LineRenderer
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

        // Включаем линию, чтобы было видно направление
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
            Vector3 dragVector = new Vector3(
                mouseWorldPosition.x - startPosition.x, 
                mouseWorldPosition.y - startPosition.y,
                0
            );

            // Ограничиваем расстояние натяжения
            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
            }

            // Двигаем объект за мышью
            transform.position = startPosition + dragVector;

            // Линия указывает направление полёта (противоположно натяжению)
            if (lineRenderer != null)
            {
                Vector3 flightDirection = startPosition - transform.position; 
                lineRenderer.SetPosition(0, transform.position); 
                lineRenderer.SetPosition(1, transform.position + flightDirection * 1.5f);
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            // Включаем физику, чтобы объект летел
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero; 
            }

            // Вычисляем силу броска
            Vector3 force = (startPosition - transform.position).normalized * maxForce
                            * Vector3.Distance(startPosition, transform.position);

            // Применяем силу
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }

            // Отключаем линию прицеливания
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
