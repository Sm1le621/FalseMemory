using UnityEngine;

// Програє звуки бою (удар, промах, перемога...).
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public sealed class BattleAudioController : MonoBehaviour
{
    // Набір звуків (якщо не призначено - звуків не буде)
    public BattleAudioProfile profile;

    private AudioSource source;

    // Останній звук, який просили програти (потрібно для тестів)
    public BattleSound LastEvent { get; private set; }

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        // 0 = звук 2D (не залежить від позиції в просторі)
        source.spatialBlend = 0f;
        // Враховуємо загальну гучність гри
        source.ignoreListenerVolume = false;
    }

    // Програти звук. Повертає true, якщо звук справді заграв
    public bool Play(BattleSound sound)
    {
        LastEvent = sound;

        if (!isActiveAndEnabled || profile == null || source == null)
        {
            return false;
        }

        AudioClip clip = profile.GetClip(sound);
        if (clip == null)
        {
            return false;
        }

        source.outputAudioMixerGroup = profile.output;
        source.PlayOneShot(clip, Mathf.Clamp01(profile.volume));
        return true;
    }
}
