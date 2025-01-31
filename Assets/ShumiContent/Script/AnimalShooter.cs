using UnityEngine;

public class AnimalShooter : MonoBehaviour
{
    [Header("Throw Settings")]
    public float maxForce = 10f;          // Максимальная сила выстрела
    public float maxDragDistance = 2f;    // Максимальное расстояние натяжения
    public LineRenderer lineRenderer;     // Линия прицеливания

    [Header("Shake Settings")]
    public float shakeFrequency = 5f;     // Частота дрожания
    public float shakeMaxAmplitude = 0.1f;// Максимальная амплитуда дрожания (при полной натяжке)

    [Header("Spin Settings")]
    public float spinImpulse = 0.2f;      // Сила вращения при отпускании

    private bool isDragging = false;
    private Vector3 startPosition;
    private Rigidbody rb;
    private Camera mainCamera;
    
    

    // Сохраним dragVector, чтобы другие скрипты могли узнать направление броска
    public Vector3 currentDragVector { get; private set; } = Vector3.zero;

    // Инициализация (метод, если будем вызывать после Instantiate)
    public void Initialize()
{
    rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        // Не сбрасывайте velocity, если хотите сохранить "импульс" от мерджа
        rb.isKinematic = false; 
        // Уберите/закомментируйте:
        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
    }

    mainCamera = Camera.main;
    if (lineRenderer != null)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }
}


    void Start()
    {
        Initialize();
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

            currentDragVector = dragVector; // Запоминаем, чтобы знать направление броска

            // --- Дрожание (shake) ---
            // Амплитуда зависит от длины dragVector: при maxDragDistance = shakeMaxAmplitude
            float dragPercent = dragVector.magnitude / maxDragDistance;
            float shakeAmplitude = dragPercent * shakeMaxAmplitude;

            // Вычислим небольшое смещение (синус + косинус, чтобы колебания шли по разным осям)
            float shakeOffsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
            float shakeOffsetY = Mathf.Cos(Time.time * shakeFrequency * 1.2f) * shakeAmplitude;

            // Применяем смещение к позиции
            Vector3 shakeOffset = new Vector3(shakeOffsetX, shakeOffsetY, 0);

            // Итоговая позиция
            transform.position = startPosition + dragVector + shakeOffset;

            // Линия указывает направление полета (противоположное натяжению)
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

            // Включаем физику
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Вычисляем силу броска
            // (startPosition - transform.position) — вектор к старту, пропорциональный длине натяжения
            Vector3 forceDir = (startPosition - transform.position).normalized;
            float distance = Vector3.Distance(startPosition, transform.position);
            Vector3 force = forceDir * maxForce * distance;

            // Применяем силу
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);

                // Добавляем лёгкий момент вращения
                // Направление торка возьмём перпендикулярно вектору броска (Z = 0, так что можно "выпятить" ось)
                Vector3 torqueAxis = new Vector3(0, 0, 1);
                // Можно случайно чуть изменить направление, чтобы была рандомная крутка
                torqueAxis = Quaternion.Euler(0, 0, Random.Range(-45f, 45f)) * torqueAxis;

                rb.AddTorque(torqueAxis * spinImpulse, ForceMode.Impulse);
            }

            // Отключаем линию
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
