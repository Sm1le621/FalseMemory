using UnityEngine;
using UnityEngine.Audio;

public enum BattleSound { MenuOpen, Move, Confirm, Cancel, Attack, Hit, Critical, Miss, Fire, Victory, Defeat, Defend, Flee }

[CreateAssetMenu(menuName="FalseMemory/Battle Audio Profile")]
public sealed class BattleAudioProfile : ScriptableObject
{
    public AudioMixerGroup output;
    [Range(0f,1f)] public float volume=0.8f;
    public AudioClip menuOpen,move,confirm,cancel,attack,hit,critical,miss,fire,victory,defeat,defend,flee;
    public AudioClip GetClip(BattleSound sound)
    {
        switch(sound)
        {
            case BattleSound.MenuOpen:return menuOpen;case BattleSound.Move:return move;
            case BattleSound.Confirm:return confirm;case BattleSound.Cancel:return cancel;
            case BattleSound.Attack:return attack;case BattleSound.Hit:return hit;
            case BattleSound.Critical:return critical;case BattleSound.Miss:return miss;
            case BattleSound.Fire:return fire;
            case BattleSound.Victory:return victory;case BattleSound.Defeat:return defeat;
            case BattleSound.Defend:return defend;
            case BattleSound.Flee:return flee;default:return null;
        }
    }
}
