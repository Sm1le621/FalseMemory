using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class BattleVFXController : MonoBehaviour
{
    public BattleUI ui;
    public BattleVFXProfile fireProfile, healProfile;
    private readonly System.Collections.Generic.List<Image> particles=new System.Collections.Generic.List<Image>();
    private Coroutine effect;
    private Image frameImage;
    public int CurrentFrame {get;private set;}=-1;
    public bool HasAnimatedFrame => frameImage!=null && frameImage.isActiveAndEnabled && frameImage.sprite!=null;
    public int EffectsPlayed { get; private set; }
    public int ActiveParticles { get; private set; }

    public void Play(bool onPlayer,BattleVFXProfile profile,Color fallback,bool healing)
    {
        if(!isActiveAndEnabled || ui==null) return;
        var portrait=onPlayer?ui.playerPortrait:ui.enemyPortrait;if(portrait==null)return;
        if(effect!=null)StopCoroutine(effect);Clear();
        profile=profile!=null?profile:(healing?healProfile:fireProfile);
        effect=StartCoroutine(Animate(portrait,profile,fallback,healing));EffectsPlayed++;
    }
    private IEnumerator Animate(Image portrait,BattleVFXProfile profile,Color fallback,bool healing)
    {
        int count=profile!=null?Mathf.Clamp(profile.particleCount,4,24):12;
        float duration=profile!=null?Mathf.Clamp(profile.duration,0,2):0.45f;
        float radius=profile!=null?Mathf.Clamp(profile.radius,0,100):42f;
        float size=profile!=null?Mathf.Clamp(profile.particleSize,1,24):8f;
        Color color=profile!=null?profile.color:fallback;
        var shape=profile!=null?profile.shape:(healing?BattleVFXShape.Orbit:BattleVFXShape.Rising);
        EnsurePool(portrait.transform.parent,count);
        bool animated=profile!=null && profile.frames!=null && profile.frames.Length>0;
        if(animated)
        {
            if(frameImage==null)
            {
                var obj=new GameObject("PixelSpellFrame",typeof(RectTransform),typeof(Image));obj.transform.SetParent(portrait.transform.parent,false);
                frameImage=obj.GetComponent<Image>();frameImage.raycastTarget=false;frameImage.preserveAspect=true;
                frameImage.rectTransform.anchorMin=frameImage.rectTransform.anchorMax=Vector2.zero;
            }
            frameImage.color=Color.white;frameImage.rectTransform.sizeDelta=Vector2.one*Mathf.Clamp(profile.frameSize,32,256);
            frameImage.transform.SetAsLastSibling();frameImage.gameObject.SetActive(true);
        }
        Vector2 center=portrait.rectTransform.anchoredPosition+Vector2.Scale(portrait.rectTransform.rect.size,new Vector2(0.5f,0.5f)-portrait.rectTransform.pivot);
        for(int i=0;i<count;i++){particles[i].sprite=profile!=null?profile.particleSprite:null;particles[i].gameObject.SetActive(true);}
        ActiveParticles=count;
        for(float t=0;t<duration;t+=Time.unscaledDeltaTime)
        {
            float progress=t/duration;
            if(animated)
            {
                CurrentFrame=Mathf.Min(profile.frames.Length-1,Mathf.FloorToInt(progress*profile.frames.Length));
                frameImage.sprite=profile.frames[CurrentFrame];
                frameImage.enabled=frameImage.sprite!=null;
                frameImage.rectTransform.anchoredPosition=portrait.rectTransform.anchoredPosition+
                    Vector2.Scale(portrait.rectTransform.rect.size,Vector2.one*0.5f-portrait.rectTransform.pivot);
            }
            for(int i=0;i<count;i++)
            {
                float angle=i*Mathf.PI*2/count+progress*(shape==BattleVFXShape.Orbit?2.5f:0);
                Vector2 offset=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius*(shape==BattleVFXShape.Burst?progress:1f);
                if(shape==BattleVFXShape.Rising)offset=new Vector2((i%5-2)*radius/2.5f, -30f+progress*(75f+i%3*12f));
                var p=particles[i];p.rectTransform.anchoredPosition=center+offset;
                p.rectTransform.sizeDelta=Vector2.one*size*(0.5f+0.5f*Mathf.Sin(progress*Mathf.PI));
                p.rectTransform.localRotation=Quaternion.Euler(0,0,shape==BattleVFXShape.Orbit?45f:0f);
                Color current=color;current.a=Mathf.Sin(progress*Mathf.PI);p.color=current;
            }
            yield return null;
        }
        Clear();effect=null;
    }
    private void EnsurePool(Transform parent,int count)
    {
        while(particles.Count<count)
        {
            var obj=new GameObject("BattleParticle"+particles.Count,typeof(RectTransform),typeof(Image));obj.transform.SetParent(parent,false);
            var image=obj.GetComponent<Image>();image.raycastTarget=false;image.preserveAspect=true;
            image.rectTransform.anchorMin=image.rectTransform.anchorMax=Vector2.zero;image.rectTransform.pivot=new Vector2(0.5f,0.5f);
            particles.Add(image);
        }
    }
    private void Clear(){foreach(var p in particles)if(p!=null)p.gameObject.SetActive(false);if(frameImage!=null)frameImage.gameObject.SetActive(false);ActiveParticles=0;CurrentFrame=-1;}
    private void OnDisable(){if(effect!=null)StopCoroutine(effect);effect=null;Clear();}
    private void OnDestroy(){foreach(var p in particles)if(p!=null)Destroy(p.gameObject);particles.Clear();if(frameImage!=null)Destroy(frameImage.gameObject);}
}
