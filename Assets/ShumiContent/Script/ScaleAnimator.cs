using UnityEngine;
using System.Collections;

public class ScaleAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float scaleUpDuration = 0.1f; // Время увеличения масштаба
    public float scaleDownDuration = 0.2f; // Время уменьшения масштаба
    public float scaleUpMultiplier = 1.5f; // Множитель увеличения масштаба
    public float scaleDownMultiplier = 0.5f; // Множитель уменьшения масштаба

    private Vector3 originalScale; // Исходный масштаб объекта

    void Start()
    {
        originalScale = transform.localScale; // Сохраняем исходный масштаб
    }

    // Метод для запуска анимации вручную
    public void StartAnimation()
    {
        StartCoroutine(AnimateScale());
    }

    IEnumerator AnimateScale()
    {
        // Увеличиваем масштаб
        yield return ScaleTo(originalScale * scaleUpMultiplier, scaleUpDuration);

        // Уменьшаем масштаб
        yield return ScaleTo(originalScale * scaleDownMultiplier, scaleDownDuration);

        // Возвращаем исходный масштаб
        yield return ScaleTo(originalScale, scaleUpDuration);
    }

    IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // Плавно изменяем масштаб
            transform.localScale = Vector3.Lerp(startScale, targetScale, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Убедимся, что масштаб точно равен целевому
        transform.localScale = targetScale;
    }
}