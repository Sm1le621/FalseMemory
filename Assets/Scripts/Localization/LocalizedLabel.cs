using TMPro;
using UnityEngine;

[DisallowMultipleComponent,RequireComponent(typeof(TMP_Text))]
public sealed class LocalizedLabel : MonoBehaviour
{
    public string key;
    private TMP_Text label;
    private void Awake(){label=GetComponent<TMP_Text>();}
    private void OnEnable(){GameText.LanguageChanged+=Refresh;Refresh();}
    private void OnDisable(){GameText.LanguageChanged-=Refresh;}
    public void Refresh(){if(label==null)label=GetComponent<TMP_Text>();if(!string.IsNullOrEmpty(key))label.text=GameText.Get(key);}
}
