using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Small original native-pixel masks; temporary silhouettes, not final item illustrations.
public static class EquipmentPlaceholderBuilder
{
    public const string Root="Assets/Art/UI/Equipment64";
    private static readonly string[] Names={"Sword","Shield","Helmet","Chest","Gloves","Legs","Boots","Ring"};
    [MenuItem("FalseMemory/Art/Generate equipment slot silhouettes")]
    public static void Generate()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before authoring assets.");
        foreach(var name in Names)if(File.Exists(Root+"/"+name+".png"))throw new InvalidOperationException("Existing art: "+name);
        Directory.CreateDirectory(Root);
        foreach(var name in Names)
        {
            var g=new Grid();Draw(g,name);var texture=new Texture2D(64,64,TextureFormat.RGBA32,false);
            texture.SetPixels32(g.pixels);texture.Apply();string path=Root+"/"+name+".png";
            File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.filterMode=FilterMode.Point;
            importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.spritePivot=Vector2.one*.5f;importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;
            importer.maxTextureSize=64;importer.SaveAndReimport();
        }
    }
    private static void Draw(Grid g,string name)
    {
        switch(name)
        {
            case "Sword":g.Line(18,14,48,48,5);g.Line(16,24,30,12,4);g.Line(16,16,10,9,4);g.Box(8,6,13,11);break;
            case "Shield":g.Line(13,48,51,48,4);g.Line(13,48,16,26,4);g.Line(51,48,48,26,4);g.Line(16,26,32,12,4);g.Line(48,26,32,12,4);g.Line(32,17,32,46,3);g.Line(19,35,45,35,3);break;
            case "Helmet":g.Line(15,27,16,43,4);g.Line(16,43,24,51,4);g.Line(24,51,40,51,4);g.Line(40,51,48,43,4);g.Line(48,43,49,27,4);g.Box(16,24,48,29);g.Box(20,19,26,26);g.Box(38,19,44,26);g.Line(31,25,31,48,3);g.Box(18,35,25,38);g.Box(38,35,45,38);break;
            case "Chest":g.Line(19,49,12,40,5);g.Line(12,40,19,32,5);g.Line(45,49,52,40,5);g.Line(52,40,45,32,5);g.Line(19,49,27,43,4);g.Line(45,49,37,43,4);g.Line(27,43,37,43,4);g.Line(20,32,18,14,5);g.Line(44,32,46,14,5);g.Line(18,14,46,14,4);g.Line(32,17,32,40,3);g.Line(21,30,43,30,3);break;
            case "Gloves":g.Box(21,13,40,31);g.Box(21,31,25,49);g.Box(27,31,31,53);g.Box(33,31,37,51);g.Box(39,29,43,45);g.Line(20,30,13,38,6);g.Box(21,11,40,15);break;
            case "Legs":g.Box(19,43,44,49);g.Line(20,43,16,15,7);g.Line(43,43,48,15,7);g.Line(31,41,26,15,5);g.Line(33,41,38,15,5);g.Line(16,14,26,14,4);g.Line(38,14,48,14,4);break;
            case "Boots":g.Box(16,26,27,49);g.Box(36,26,47,49);g.Line(22,28,17,15,8);g.Line(42,28,47,15,8);g.Box(9,12,27,18);g.Box(36,12,54,18);g.Box(14,44,29,49);g.Box(34,44,49,49);break;
            case "Ring":for(int y=10;y<=50;y++)for(int x=10;x<=50;x++){int d=(x-31)*(x-31)+(y-29)*(y-29);if(d>=13*13 && d<=19*19)g.Set(x,y);}g.Box(23,45,39,51);g.Line(23,45,31,54,3);g.Line(39,45,31,54,3);break;
        }
    }
    private sealed class Grid
    {
        public readonly Color32[] pixels=new Color32[64*64];
        public void Set(int x,int y){if(x>=0 && x<64 && y>=0 && y<64)pixels[y*64+x]=new Color32(182,182,182,255);}
        public void Box(int x0,int y0,int x1,int y1){for(int y=y0;y<=y1;y++)for(int x=x0;x<=x1;x++)Set(x,y);}
        public void Line(int x0,int y0,int x1,int y1,int thickness)
        {
            int dx=Math.Abs(x1-x0),sx=x0<x1?1:-1,dy=-Math.Abs(y1-y0),sy=y0<y1?1:-1,err=dx+dy;
            while(true){Box(x0-thickness/2,y0-thickness/2,x0+thickness/2,y0+thickness/2);if(x0==x1 && y0==y1)break;int e=2*err;if(e>=dy){err+=dy;x0+=sx;}if(e<=dx){err+=dx;y0+=sy;}}
        }
    }
}
