using UnityEngine;

public class AnimalMerger : MonoBehaviour
{
    public int tier = 1; // Уровень животного
    public GameObject nextTierAnimal; // Префаб следующего уровня
    public float speedMultiplier = 0.8f; // Множитель для уменьшения скорости

    private bool isMerging = false; // Флаг, чтобы предотвратить дублирование

    void OnCollisionEnter(Collision collision)
    {
        AnimalMerger otherAnimal = collision.gameObject.GetComponent<AnimalMerger>();
        if (otherAnimal != null && otherAnimal.tier == this.tier && !isMerging && !otherAnimal.isMerging)
        {
            isMerging = true;
            otherAnimal.isMerging = true;

            // Если есть префаб следующего уровня
            if (nextTierAnimal != null)
            {
                // Создаём новое животное между двумя столкнувшимися
                Vector3 mergePosition = (transform.position + collision.transform.position) / 2;
                GameObject newAnimal = Instantiate(nextTierAnimal, mergePosition, Quaternion.identity);

                // Запускаем эффект взрыва и передаём трансформацию нового животного
                ExplosionEffect explosionEffect = GetComponent<ExplosionEffect>();
                if (explosionEffect != null)
                {
                    explosionEffect.PlayExplosion(mergePosition, newAnimal.transform);
                }

                // Передаём скорость (импульс) от более быстрого животного
                Rigidbody rbThis = GetComponent<Rigidbody>();
                Rigidbody rbOther = collision.rigidbody; // Rigidbody другого животного
                Rigidbody rbNew = newAnimal.GetComponent<Rigidbody>();

                if (rbThis != null && rbOther != null && rbNew != null)
                {
                    rbNew.isKinematic = false; // Делаем новое животное динамическим

                    // Определяем, кто быстрее
                    float speedThis = rbThis.linearVelocity.magnitude;
                    float speedOther = rbOther.linearVelocity.magnitude;

                    if (speedThis >= speedOther)
                    {
                        rbNew.linearVelocity = rbThis.linearVelocity * speedMultiplier; // Уменьшаем скорость
                    }
                    else
                    {
                        rbNew.linearVelocity = rbOther.linearVelocity * speedMultiplier; // Уменьшаем скорость
                    }
                }

                // Инициализация AnimalShooter, если есть
                AnimalShooter shooter = newAnimal.GetComponent<AnimalShooter>();
                if (shooter != null)
                {
                    shooter.Initialize(); // Инициализируем AnimalShooter
                    shooter.enabled = true; // Включаем AnimalShooter
                }

                // Запускаем анимацию масштабирования
                ScaleAnimator scaleAnimator = newAnimal.GetComponent<ScaleAnimator>();
                if (scaleAnimator != null)
                {
                    scaleAnimator.StartAnimation();
                }

                // Проигрываем звук появления нового животного
                AudioManager audioManager = FindObjectOfType<AudioManager>();
                if (audioManager != null)
                {
                    audioManager.PlayNewAnimalSoundEffect();
                }
            }
            else
            {
                Debug.LogWarning("Next tier animal prefab is not assigned!");
            }

            // Уничтожаем старые объекты
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
