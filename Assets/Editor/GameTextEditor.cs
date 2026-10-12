using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;

public sealed class GameTextEditor : EditorWindow
{
    private const string TablePath="Assets/Resources/Localization/GameText_en-US.asset";
    private Vector2 scroll;
    private string search="", newKey="", newValue="";
    [MenuItem("FalseMemory/Localization/English Text")]
    public static void Open(){GetWindow<GameTextEditor>("Game Text (en-US)");}
    private void OnGUI()
    {
        var table=AssetDatabase.LoadAssetAtPath<StringTable>(TablePath);
        if(table==null){EditorGUILayout.HelpBox("English StringTable is missing.",MessageType.Error);return;}
        EditorGUILayout.LabelField("English (en-US)",EditorStyles.boldLabel);
        search=EditorGUILayout.TextField("Search",search);
        scroll=EditorGUILayout.BeginScrollView(scroll);
        foreach(var pair in table.OrderBy(p=>table.SharedData.GetEntry(p.Key).Key))
        {
            string key=table.SharedData.GetEntry(pair.Key).Key;
            if(!string.IsNullOrEmpty(search) && key.IndexOf(search,System.StringComparison.OrdinalIgnoreCase)<0 &&
                pair.Value.Value.IndexOf(search,System.StringComparison.OrdinalIgnoreCase)<0)continue;
            EditorGUILayout.LabelField(key,EditorStyles.boldLabel);
            string value=EditorGUILayout.TextArea(pair.Value.Value,GUILayout.MinHeight(38));
            if(value!=pair.Value.Value)
            {
                if(Regex.IsMatch(value,"[А-Яа-яЁё]"))EditorGUILayout.HelpBox("Game text must be English.",MessageType.Warning);
                else {Undo.RecordObject(table,"Edit English text");pair.Value.Value=value;EditorUtility.SetDirty(table);}
            }
        }
        EditorGUILayout.EndScrollView();
        newKey=EditorGUILayout.TextField("New stable key",newKey);newValue=EditorGUILayout.TextField("English value",newValue);
        bool valid=Regex.IsMatch(newKey,"^[a-z0-9][a-z0-9_.-]*$") && !string.IsNullOrWhiteSpace(newValue) &&
            !Regex.IsMatch(newValue,"[А-Яа-яЁё]") && table.GetEntry(newKey)==null;
        using(new EditorGUI.DisabledScope(!valid))if(GUILayout.Button("Add entry"))
        {
            Undo.RecordObjects(new Object[]{table,table.SharedData},"Add English text");table.AddEntry(newKey,newValue);
            EditorUtility.SetDirty(table);EditorUtility.SetDirty(table.SharedData);newKey=newValue="";
        }
        if(GUILayout.Button("Save table")){AssetDatabase.SaveAssetIfDirty(table);AssetDatabase.SaveAssetIfDirty(table.SharedData);}
    }
}
