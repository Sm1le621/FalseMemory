using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Localization.Tables;

// Native Unity StringTables, loaded synchronously from Resources: no startup
// Addressables wait and no second copy of English strings in source code.
public static class GameText
{
    public const string EnglishLocale="en-US";
    public static string LocaleCode {get;private set;}=EnglishLocale;
    public static event Action LanguageChanged;
    private static StringTable english, selected;
    private static CultureInfo culture=CultureInfo.GetCultureInfo(EnglishLocale);
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset(){english=selected=null;LocaleCode=EnglishLocale;culture=CultureInfo.GetCultureInfo(EnglishLocale);LanguageChanged=null;}
    private static StringTable English=>english!=null?english:english=Resources.Load<StringTable>("Localization/GameText_"+EnglishLocale);
    public static bool TrySetLocale(string code)
    {
        if(string.IsNullOrWhiteSpace(code))return false;
        CultureInfo nextCulture;try {nextCulture=CultureInfo.GetCultureInfo(code);}catch(CultureNotFoundException){return false;}
        var table=Resources.Load<StringTable>("Localization/GameText_"+code);if(table==null)return false;
        selected=table;culture=nextCulture;LocaleCode=code;LanguageChanged?.Invoke();return true;
    }
    public static bool Contains(string key)=>Find(key)!=null;
    private static StringTableEntry Find(string key)
    {
        if(string.IsNullOrEmpty(key))return null;
        return (selected!=null?selected.GetEntry(key):null) ?? (English!=null?English.GetEntry(key):null);
    }
    public static string Get(string key,params object[] args)
    {
        var entry=Find(key);if(entry==null)return "["+(key ?? "missing.key")+"]";
        try {return string.Format(culture,entry.Value,args ?? Array.Empty<object>());}
        catch(FormatException){return "[invalid: "+key+"]";}
    }
    public static string Content(string key,string englishFallback)=>Contains(key)?Get(key):englishFallback ?? "";
}
