using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    public GameObject particlePrefab; // Префаб системы частиц
    public Color particleColor = Color.white; // Цвет частиц
    public int particleCount = 20; // Количество частиц
    public float explosionForce = 5f; // Сила разлёта частиц
    public float minSize = 0.1f; // Минимальный размер частиц
    public float maxSize = 1f; // Максимальный размер частиц
    public float minLifetime = 0.5f; // Минимальное время жизни частиц
    public float maxLifetime = 1f; // Максимальное время жизни частиц

    // Метод для запуска эффекта взрыва с привязкой к трансформации нового животного
    public void PlayExplosion(Vector3 position, Transform parent = null)
    {
        if (particlePrefab == null)
        {
            Debug.LogWarning("Particle prefab is not assigned!");
            return;
        }

        // Создаём систему частиц и делаем её дочерним объектом нового животного
        GameObject explosion = Instantiate(particlePrefab, position, Quaternion.identity, parent);

        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogWarning("ParticleSystem component is missing on the prefab!");
            return;
        }

        // Настраиваем частицы
        var mainModule = particleSystem.main;
        mainModule.startColor = particleColor;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
        mainModule.startLifetime = new ParticleSystem.MinMaxCurve(minLifetime, maxLifetime);

        // Запускаем систему частиц
        particleSystem.Play();

        // Уничтожаем систему частиц после завершения
        Destroy(explosion, mainModule.startLifetime.constantMax + mainModule.duration);
    }
}
