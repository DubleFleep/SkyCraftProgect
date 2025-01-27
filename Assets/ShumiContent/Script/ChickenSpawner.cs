using UnityEngine;

public class ChickenSpawner : MonoBehaviour
{
    public GameObject chickenPrefab; // Префаб курицы
    public Vector3 spawnAreaCenter; // Центр области спавна
    public Vector3 spawnAreaSize; // Размер области спавна
    public int maxChickens = 4; // Максимальное количество куриц на сцене

    void Update()
    {
        // Находим все объекты с тегом "Chicken"
        GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");

        // Если куриц меньше, чем нужно, спавним новую
        if (chickens.Length < maxChickens)
        {
            SpawnChicken();
        }
    }

    void SpawnChicken()
{
    // Генерация случайной позиции в пределах области спавна
    Vector3 randomPosition = new Vector3(
        Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
        Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
        Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
    );

    // Генерация случайного поворота
    Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 360);

    // Создание курицы в случайной позиции и с случайным поворотом
    Instantiate(chickenPrefab, randomPosition, randomRotation);
}
}