using UnityEngine;

public class AnimalShooter : MonoBehaviour
{
    public float maxForce = 10f; // Максимальная сила выстрела
    public float maxDragDistance = 2f; // Максимальное расстояние, на которое можно тянуть животное

    private bool isDragging = false;
    private Vector3 startPosition;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Отключаем физику, пока животное не выстрелено
    }

    void OnMouseDown()
    {
        if (rb.isKinematic) // Если животное ещё не выстрелено
        {
            isDragging = true;
            startPosition = transform.position;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Получаем позицию мыши в мировых координатах
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ограничиваем движение по оси Z

            // Вычисляем направление и расстояние
            Vector3 direction = (startPosition - mousePosition).normalized;
            float distance = Vector3.Distance(startPosition, mousePosition);

            // Ограничиваем расстояние
            distance = Mathf.Min(distance, maxDragDistance);

            // Перемещаем животное
            transform.position = startPosition - direction * distance;

            // Отрисовка линии
            Debug.DrawLine(startPosition, transform.position, Color.red);
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;

            // Включаем физику
            rb.isKinematic = false;

            // Вычисляем силу выстрела
            Vector3 force = (startPosition - transform.position) * maxForce;
            rb.AddForce(force, ForceMode.Impulse); // Применяем силу
        }
    }

    
}