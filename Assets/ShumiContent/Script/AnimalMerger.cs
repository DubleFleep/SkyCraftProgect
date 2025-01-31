using UnityEngine;
using System.Collections; // Нужно для Coroutines

public class AnimalMerger : MonoBehaviour
{
    public int tier = 1;               // Уровень животного (1 - курица, 2 - корова, 3 - волк и т.д.)
    public GameObject nextTierAnimal;  // Префаб животного следующего уровня
    private bool isMerging = false;    // Флаг, чтобы предотвратить дублирование

    // Вложенный класс (если вам нужен), уберите если не используется
    public class Animal : MonoBehaviour
    {
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Отключаем kinematic при создании
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Проверяем, столкнулись ли мы с AnimalMerger того же тира
        AnimalMerger otherAnimal = collision.gameObject.GetComponent<AnimalMerger>();
        if (otherAnimal != null && otherAnimal.tier == this.tier && !isMerging && !otherAnimal.isMerging)
        {
            isMerging = true;
            otherAnimal.isMerging = true;

            // Есть ли префаб следующего уровня?
            if (nextTierAnimal != null)
            {
                // Создаём новое животное
                Vector3 mergePosition = (transform.position + collision.transform.position) / 2;
                GameObject newAnimal = Instantiate(nextTierAnimal, mergePosition, Quaternion.identity);

                // Настройка Rigidbody нового животного
                Rigidbody rb = newAnimal.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Сбрасываем линейную и угловую скорость
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.WakeUp(); 
                    
                    // Ставим isKinematic в false сразу
                    rb.isKinematic = false;
                }

                // Инициализируем AnimalShooter, если есть
                AnimalShooter shooter = newAnimal.GetComponent<AnimalShooter>();
                if (shooter != null)
                {
                    shooter.enabled = true;
                    shooter.Initialize();
                }

                // Запускаем корутину, чтобы в конце кадра/физшаге ещё раз насильно отключить кинематику
                StartCoroutine(ForceDynamicNextFrame(newAnimal));
            }
            else
            {
                // Нет следующего тира — просто уничтожаем оба
                Debug.Log("Нет следующего тира. Уничтожаем обоих.");
            }

            // Уничтожаем старые объекты
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    // Корутина, чтобы гарантированно отключить кинематику на следующем физическом шаге
    private IEnumerator ForceDynamicNextFrame(GameObject spawned)
    {
        // Можно дождаться конца кадра:
        // yield return new WaitForEndOfFrame();
        // Или дождаться следующего физического шага:
        yield return new WaitForFixedUpdate();

        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Насильно отключаем кинематику
            rb.WakeUp();
            Debug.Log($"{spawned.name} → Forced isKinematic=false after next frame.");
        }
    }
}
