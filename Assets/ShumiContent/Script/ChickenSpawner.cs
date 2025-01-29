using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab; // Префаб курицы
    public Vector3 spawnAreaCenter; // Центр области спавна
    public Vector3 spawnAreaSize; // Размер области спавна
    public int maxChickens = 4; // Максимальное количество куриц на сцене

    void Start()
    {
        StartCoroutine(SpawnChickensWithDelay());
    }

    IEnumerator SpawnChickensWithDelay()
    {
        while (true)
        {
            // Проверяем, нужно ли спавнить курицу
            GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");

            if (chickens.Length < maxChickens)
            {
                SpawnChicken();
            }

            // Ждем случайный интервал перед следующим спавном
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    void SpawnChicken()
    {
        Vector3 randomPosition;
        int attempts = 10; // Количество попыток найти подходящую позицию

        do
        {
            // Генерация случайной позиции в пределах области спавна
            randomPosition = new Vector3(
                Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
                Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
                Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
            );

            attempts--;

            // Проверяем, далеко ли новая позиция от всех куриц
            bool positionIsFar = true;
            foreach (GameObject chicken in GameObject.FindGameObjectsWithTag("Chicken"))
            {
                if (Vector3.Distance(randomPosition, chicken.transform.position) < 1.5f) // Минимальное расстояние между курами
                {
                    positionIsFar = false;
                    break;
                }
            }

            if (positionIsFar)
                break;

        } while (attempts > 0);

        // Генерация случайного поворота
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Создание курицы
        Instantiate(chickenPrefab, randomPosition, randomRotation);
    }
}
