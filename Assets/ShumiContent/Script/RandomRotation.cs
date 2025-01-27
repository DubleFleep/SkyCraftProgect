using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float minRotationSpeed = 10f; // Минимальная скорость вращения
    public float maxRotationSpeed = 50f; // Максимальная скорость вращения

    private Vector3 rotationAxis; // Ось вращения
    private float rotationSpeed;  // Скорость вращения

    void Start()
    {
        // Генерация случайной оси вращения
        rotationAxis = new Vector3(
            Random.Range(-1f, 1f), // Случайное значение для оси X
            Random.Range(-1f, 1f), // Случайное значение для оси Y
            Random.Range(-1f, 1f)  // Случайное значение для оси Z
        ).normalized; // Нормализуем, чтобы получить единичный вектор

        // Генерация случайной скорости вращения
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void Update()
    {
        // Плавное изменение оси вращения
        rotationAxis = Vector3.Slerp(rotationAxis, new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized, 0.01f);

        // Вращение объекта вокруг случайной оси
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }
}