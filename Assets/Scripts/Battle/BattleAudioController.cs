using UnityEngine;

[DisallowMultipleComponent,RequireComponent(typeof(AudioSource))]
public sealed class BattleAudioController : MonoBehaviour
{
    public BattleAudioProfile profile;
    private AudioSource source;
    public BattleSound LastEvent {get;private set;}
    public int EventsRequested {get;private set;}
    private void Awake()
    {
        source=GetComponent<AudioSource>();source.playOnAwake=false;source.loop=false;
        source.spatialBlend=0f;source.ignoreListenerVolume=false;
    }
    public bool Play(BattleSound sound)
    {
        LastEvent=sound;EventsRequested++;
        if(!isActiveAndEnabled || profile==null || source==null)return false;
        var clip=profile.GetClip(sound);if(clip==null)return false;
        source.outputAudioMixerGroup=profile.output;
        source.PlayOneShot(clip,Mathf.Clamp01(profile.volume));return true;
    }
}
