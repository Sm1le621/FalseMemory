using UnityEngine;
using UnityEngine.Audio;

// Всі види звуків у бою
public enum BattleSound { MenuOpen, Move, Confirm, Cancel, Attack, Hit, Critical, Miss, Fire, Victory, Defeat, Defend, Flee }

// Файл-налаштування зі звуками бою (створюється через меню Create)
[CreateAssetMenu(menuName = "FalseMemory/Battle Audio Profile")]
public sealed class BattleAudioProfile : ScriptableObject
{
    // Куди йде звук (мікшер)
    public AudioMixerGroup output;

    [Range(0f, 1f)]
    public float volume = 0.8f;

    // Звуки для кожної події
    public AudioClip menuOpen;
    public AudioClip move;
    public AudioClip confirm;
    public AudioClip cancel;
    public AudioClip attack;
    public AudioClip hit;
    public AudioClip critical;
    public AudioClip miss;
    public AudioClip fire;
    public AudioClip victory;
    public AudioClip defeat;
    public AudioClip defend;
    public AudioClip flee;

    // Повертає потрібний звук за типом події
    public AudioClip GetClip(BattleSound sound)
    {
        switch (sound)
        {
            case BattleSound.MenuOpen:
                return menuOpen;
            case BattleSound.Move:
                return move;
            case BattleSound.Confirm:
                return confirm;
            case BattleSound.Cancel:
                return cancel;
            case BattleSound.Attack:
                return attack;
            case BattleSound.Hit:
                return hit;
            case BattleSound.Critical:
                return critical;
            case BattleSound.Miss:
                return miss;
            case BattleSound.Fire:
                return fire;
            case BattleSound.Victory:
                return victory;
            case BattleSound.Defeat:
                return defeat;
            case BattleSound.Defend:
                return defend;
            case BattleSound.Flee:
                return flee;
            default:
                return null;
        }
    }
}
