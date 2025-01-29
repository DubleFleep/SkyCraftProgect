using UnityEngine;

public class AnimalMerger : MonoBehaviour
{
    public int tier = 1; // Уровень животного (1 - курица, 2 - корова, 3 - волк и т.д.)
    public GameObject nextTierAnimal; // Префаб животного следующего уровня
    private bool isMerging = false;   // Флаг, чтобы предотвратить дублирование

    void OnCollisionEnter(Collision collision)
    {
        AnimalMerger otherAnimal = collision.gameObject.GetComponent<AnimalMerger>();
        if (otherAnimal != null && otherAnimal.tier == this.tier && !isMerging && !otherAnimal.isMerging)
        {
            isMerging = true;
            otherAnimal.isMerging = true;

            // **Проверяем, есть ли префаб следующего уровня**
            if (nextTierAnimal != null)
            {
                // Создаём новое животное
                Vector3 mergePosition = (transform.position + collision.transform.position) / 2;
                GameObject newAnimal = Instantiate(nextTierAnimal, mergePosition, Quaternion.identity);

                // Настраиваем физику
                Rigidbody rb = newAnimal.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                // Инициализируем скрипт AnimalShooter, если есть
                AnimalShooter shooter = newAnimal.GetComponent<AnimalShooter>();
                if (shooter != null)
                {
                    shooter.enabled = true;
                    shooter.Initialize();
                }
            }
            else
            {
                // Если нет следующего тира,
                // можете оставить пустым (два животных просто уничтожаются)
                // или сделать какую-нибудь другую логику (например, добавить очки, эффект и т.д.)
                Debug.Log("Нет следующего тира. Уничтожаем обоих.");
            }

            // Уничтожаем столкнувшиеся животные (в любом случае)
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
