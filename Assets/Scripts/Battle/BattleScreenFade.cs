using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Only the black overlay survives a Single restart/menu load, never game state.
public sealed class BattleScreenFade : MonoBehaviour
{
    public Image panel;
    public void LoadAndReveal(string scene,float duration)
    {
        transform.SetParent(null,false);DontDestroyOnLoad(gameObject);
        StartCoroutine(Load(scene,duration));
    }
    private IEnumerator Load(string scene,float duration)
    {
        AsyncOperation operation=null;
        try{operation=SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);}
        catch(System.Exception ex){Debug.LogException(ex,this);}
        if(operation!=null)yield return operation;
        yield return Fade(panel,0f,duration);
        Destroy(gameObject);
    }
    public static IEnumerator Fade(Image image,float alpha,float duration)
    {
        if(image==null)yield break;
        alpha=Mathf.Clamp01(alpha);duration=Mathf.Clamp(duration,0f,2f);
        Color color=image.color;float start=color.a;
        for(float t=0;t<duration;t+=Time.unscaledDeltaTime)
        {
            if(image==null)yield break;
            color.a=Mathf.Lerp(start,alpha,Mathf.SmoothStep(0f,1f,t/duration));image.color=color;yield return null;
        }
        if(image!=null){color.a=alpha;image.color=color;}
    }
}
