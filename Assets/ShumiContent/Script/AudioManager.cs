using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource; // Источник фоновой музыки
    public AudioSource newAnimalSoundEffectSource; // Источник звука появления нового животного

    public AudioClip backgroundMusic;         // Музыка для воспроизведения
    public AudioClip newAnimalSoundEffect;  // Звуковое сопровождение при появлении нового животного

    void Start()
    {
        if (backgroundMusic != null && backgroundMusicSource != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
    }

    public void PlayNewAnimalSoundEffect()
    {
        if (newAnimalSoundEffect != null && newAnimalSoundEffectSource != null)
        {
            newAnimalSoundEffectSource.Stop(); // Останавливаем предыдущий звуковой эффект, если он воспроизводится
            newAnimalSoundEffectSource.PlayOneShot(newAnimalSoundEffect);
        }
    }
}
