using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Original grid-authored art. No resizing, sampling or editing of a large image.
public static class PixelBattleAssetBuilder
{
    public const string SkeletonRoot="Assets/Art/Enemies/Skeleton64";
    public const string VfxRoot="Assets/Art/VFX";
    private static readonly Color32[] Palette={
        new Color32(0,0,0,0),new Color32(24,20,29,255),new Color32(60,62,65,255),
        new Color32(99,105,106,255),new Color32(153,150,125,255),new Color32(112,77,50,255),
        new Color32(111,112,80,255),new Color32(170,167,122,255),new Color32(211,204,155,255),
        new Color32(242,226,180,255),new Color32(54,24,37,255),new Color32(102,42,52,255),
        new Color32(142,58,59,255),new Color32(238,158,63,255),new Color32(237,103,39,255),
        new Color32(192,55,35,255),new Color32(255,204,104,255),new Color32(46,77,64,255),
        new Color32(72,146,109,255),new Color32(109,193,140,255),new Color32(170,226,173,255),
        new Color32(237,230,158,255)};

    [MenuItem("FalseMemory/Art/Generate native 64px battle art")]
    public static void Generate()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before generating assets.");
        var outputs=new Dictionary<string,Grid>();
        foreach(SpritePose pose in Enum.GetValues(typeof(SpritePose)))
        {
            int count=pose==SpritePose.Idle || pose==SpritePose.Hurt?2:pose==SpritePose.Attack?3:4;
            for(int i=0;i<count;i++)outputs.Add(SkeletonRoot+"/Skeleton-"+pose+"-"+i.ToString("00")+".png",Skeleton(pose,i));
        }
        for(int i=0;i<8;i++){outputs.Add(VfxRoot+"/Fire64/Fire-"+i.ToString("00")+".png",Fire(i));outputs.Add(VfxRoot+"/Heal64/Heal-"+i.ToString("00")+".png",Heal(i));}
        // Check the whole set before writing; generation never replaces existing art.
        foreach(var path in outputs.Keys)if(File.Exists(path))throw new InvalidOperationException("Asset already exists: "+path);
        foreach(var pair in outputs)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pair.Key));
            var texture=new Texture2D(64,64,TextureFormat.RGBA32,false);texture.SetPixels32(pair.Value.Pixels);texture.Apply();
            File.WriteAllBytes(pair.Key,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(pair.Key,ImportAssetOptions.ForceSynchronousImport);
            var importer=(TextureImporter)AssetImporter.GetAtPath(pair.Key);importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.spritePivot=Vector2.one*0.5f;
            importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.maxTextureSize=64;importer.SaveAndReimport();
        }
    }
    private static Grid Skeleton(SpritePose pose,int frame)
    {
        var g=new Grid();int bob=pose==SpritePose.Idle?frame:pose==SpritePose.Move?frame%2:0;
        int stride=pose==SpritePose.Move?(frame==0?-2:frame==2?2:0):0;
        int lean=pose==SpritePose.Hurt?-2:pose==SpritePose.Attack?frame:0;
        if(pose==SpritePose.Death && frame>=2)
        {
            g.Line(20,55,45,55,1,5);g.Line(21,54,38,49,7,3);g.Line(39,54,49,51,8,3);
            for(int i=0;i<3;i++)g.Line(31+i*3,51,36+i*3,55,9,1);
            Skull(g,25,45,0,false);Shield(g,46,50);Sword(g,20,52,4,57);return g;
        }
        int drop=pose==SpritePose.Death?frame*7+2:0;
        int cx=32+lean, cy=14+bob+drop;
        // Legs: segmented bones with a planted baseline and alternating stride.
        Bone(g,cx-3,41+drop,cx-6+stride,49+drop);Bone(g,cx-6+stride,49+drop,cx-8+stride,56);
        Bone(g,cx+4,41+drop,cx+7-stride,49+drop);Bone(g,cx+7-stride,49+drop,cx+8-stride,56);
        g.Poly(1,cx-10+stride,55,cx-4+stride,55,cx-3+stride,58,cx-12+stride,58);
        g.Line(cx-10+stride,56,cx-4+stride,56,8,2);g.Poly(1,cx+6-stride,55,cx+12-stride,56,cx+14-stride,58,cx+5-stride,58);
        g.Line(cx+7-stride,56,cx+11-stride,57,8,2);
        g.Poly(1,cx-8,23+bob+drop,cx+7,23+bob+drop,cx+7,40+drop,cx-6,42+drop);
        g.Line(cx,24+bob+drop,cx,40+drop,7,3);
        for(int i=0;i<4;i++)
        {
            int y=27+i*3+bob+drop;g.Line(cx-6,y,cx-1,y+1,8,2);g.Line(cx+1,y+1,cx+6,y-1,7,2);
            g.Line(cx-5,y-1,cx-1,y,9,1);g.Px(cx+5,y-2,8);
        }
        // Torn burgundy tabard below a rusted belt; asymmetry is intentional.
        g.Poly(10,cx-6,38+drop,cx+7,38+drop,cx+8,48+drop,cx+3,45+drop,cx,51+drop,cx-4,46+drop,cx-7,49+drop);
        g.Poly(11,cx-4,39+drop,cx+4,39+drop,cx+2,45+drop,cx,48+drop,cx-2,44+drop);
        g.Line(cx-5,39+drop,cx+6,39+drop,5,2);g.Px(cx,38+drop,4);g.Px(cx+1,39+drop,9);
        int wristX=cx-12-(pose==SpritePose.Attack?frame*2:0),wristY=34+bob+drop-(pose==SpritePose.Attack?frame*4:0);
        Bone(g,cx-8,25+bob+drop,cx-12,30+bob+drop);Bone(g,cx-12,30+bob+drop,wristX,wristY);
        Bone(g,cx+9,25+bob+drop,cx+12,32+bob+drop);Bone(g,cx+12,32+bob+drop,cx+14,36+bob+drop);
        int tipX=pose==SpritePose.Attack?(frame==0?7:frame==1?8:41):6;
        int tipY=pose==SpritePose.Attack?(frame==0?40:frame==1?13:9):51;
        Sword(g,wristX,wristY,tipX,tipY);g.Rect(wristX-1,wristY-1,3,3,8);
        Shield(g,cx+16,34+bob+drop);
        g.Poly(1,cx-14,22+bob+drop,cx-8,19+bob+drop,cx-3,23+bob+drop,cx-5,28+bob+drop,cx-13,28+bob+drop);
        g.Poly(2,cx-13,23+bob+drop,cx-8,21+bob+drop,cx-5,24+bob+drop,cx-7,27+bob+drop,cx-12,27+bob+drop);
        g.Line(cx-12,23+bob+drop,cx-8,21+bob+drop,4,1);g.Px(cx-7,24+bob+drop,5);g.Px(cx-10,26+bob+drop,5);
        g.Poly(1,cx+5,21+bob+drop,cx+12,22+bob+drop,cx+14,27+bob+drop,cx+6,28+bob+drop);
        g.Poly(2,cx+6,22+bob+drop,cx+11,23+bob+drop,cx+12,26+bob+drop,cx+6,26+bob+drop);
        g.Px(cx+10,23+bob+drop,4);g.Px(cx+8,25+bob+drop,5);
        g.Poly(10,cx-7,20+bob+drop,cx+6,20+bob+drop,cx+3,27+bob+drop,cx-3,26+bob+drop);
        g.Line(cx-5,21+bob+drop,cx+4,22+bob+drop,11,2);g.Px(cx+2,23+bob+drop,12);
        Skull(g,cx,cy,lean,pose!=SpritePose.Death);return g;
    }
    [MenuItem("FalseMemory/Art/Generate native 64px impact")]
    public static void GenerateImpact()
    {
        if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before generating assets.");
        string path=VfxRoot+"/Hit64/Impact.png";if(File.Exists(path))throw new InvalidOperationException("Asset already exists: "+path);
        var g=new Grid();g.Poly(13,14,43,24,38,43,18,49,15,45,25,25,45,15,49);
        g.Poly(21,19,43,30,34,43,20,44,24,29,40,20,46);
        g.Line(32,12,32,20,21,1);g.Line(44,32,53,32,13,1);g.Line(12,29,20,29,21,1);
        g.Rect(47,23,2,2,21);g.Rect(20,50,2,2,13);
        Directory.CreateDirectory(Path.GetDirectoryName(path));var texture=new Texture2D(64,64,TextureFormat.RGBA32,false);
        texture.SetPixels32(g.Pixels);texture.Apply();File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
        var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.textureType=TextureImporterType.Sprite;
        importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.filterMode=FilterMode.Point;
        importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.maxTextureSize=64;importer.SaveAndReimport();
    }
    private static void Skull(Grid g,int cx,int cy,int tilt,bool lit)
    {
        g.Poly(1,cx-6,cy-7,cx+4,cy-7,cx+7,cy-4,cx+7,cy+2,cx+4,cy+7,cx-3,cy+7,cx-6,cy+3,cx-8,cy-2);
        g.Poly(6,cx-5,cy-6,cx+3,cy-6,cx+6,cy-3,cx+5,cy+2,cx+3,cy+5,cx-3,cy+5,cx-6,cy+1,cx-6,cy-3);
        g.Poly(8,cx-4,cy-5,cx+3,cy-5,cx+5,cy-2,cx+4,cy+1,cx-4,cy+2,cx-6,cy-1);
        g.Line(cx-3,cy-5,cx+2,cy-5,9,2);g.Px(cx-4,cy-3,9);g.Px(cx+3,cy-4,7);
        g.Poly(1,cx-5,cy-1,cx-2,cy-2,cx-1,cy+1,cx-4,cy+2);
        g.Poly(1,cx+2,cy-1,cx+5,cy-2,cx+5,cy+1,cx+2,cy+2);
        if(lit){g.Px(cx-3,cy,13);g.Px(cx+3,cy,13);}
        g.Line(cx,cy+1,cx,cy+2,1,2);g.Line(cx-3,cy+4,cx+3,cy+4,1,1);
        for(int i=-2;i<=2;i+=2)g.Px(cx+i,cy+4,9);g.Line(cx-2,cy+6,cx+2,cy+6,8,1);
        g.Px(cx+1,cy-3,6);g.Px(cx+2,cy-2,6);
    }
    private static void Bone(Grid g,int ax,int ay,int bx,int by){g.Line(ax,ay,bx,by,1,4);g.Line(ax,ay,bx,by,7,2);g.Line(ax,ay,bx,by,9,1);}
    private static void Sword(Grid g,int x,int y,int tx,int ty)
    {
        g.Line(x,y,tx,ty,1,5);g.Line(x,y,tx,ty,3,3);g.Line(x,y,tx,ty,4,1);
        g.Line(x-3,y-1,x+3,y+1,1,3);g.Line(x-3,y-1,x+3,y+1,5,1);g.Px(tx+1,ty,9);
    }
    private static void Shield(Grid g,int cx,int cy)
    {
        g.Poly(1,cx-5,cy-9,cx+5,cy-7,cx+5,cy+4,cx,cy+10,cx-5,cy+5);
        g.Poly(3,cx-4,cy-7,cx+3,cy-6,cx+3,cy+3,cx,cy+7,cx-3,cy+3);
        g.Poly(2,cx-2,cy-5,cx+2,cy-4,cx+2,cy+3,cx,cy+5,cx-2,cy+2);
        g.Line(cx-3,cy-6,cx+2,cy-5,4,1);g.Line(cx,cy-4,cx,cy+4,5,2);
        g.Rect(cx-1,cy-1,3,3,1);g.Px(cx,cy,4);g.Px(cx+2,cy+3,5);g.Px(cx-3,cy+2,5);
    }
    private static Grid Fire(int frame)
    {
        var g=new Grid();float envelope=Mathf.Sin((frame+1)/9f*Mathf.PI);int h=8+Mathf.RoundToInt(envelope*32);
        int wobble=frame%3-1;
        g.Poly(10,18,49,17,41,23,34,22,28,29,37,34,50-h,38,29,44,33,42,40,47,45,43,50,30,54);
        g.Poly(15,21,48,22,41,27,38,26,32,31,39,34+wobble,54-h,38,34,41,40,44,44,40,50,30,51);
        g.Poly(14,25,48,26,41,30,44,34+wobble,61-h,37,40,41,44,37,49,31,50);
        g.Poly(16,29,48,31,43,34+wobble,68-h,36,44,38,47,34,50);
        if(frame<7){g.Rect(18+(frame*7)%27,25-frame*2,2,2,16);g.Px(43-frame,29+frame,14);g.Px(22+frame,16+frame,15);}
        if(frame>=6){for(int y=0;y<64;y++)for(int x=0;x<64;x++)if((x+y+frame)%3==0 || (frame==7 && (x+y)%2==0))g.Px(x,y,0);}
        return g;
    }
    private static Grid Heal(int frame)
    {
        var g=new Grid();int r=12+frame;int y=46-frame;
        g.Ellipse(32,y,r,Math.Max(3,r/3),17);g.Ellipse(32,y-1,r-1,Math.Max(2,r/3-1),19);
        g.Line(32-r+4,y+2,32+r-4,y+2,20,1);
        for(int i=0;i<6;i++)
        {
            int x=15+i*7+(frame%2),cy=39-(frame*3+i*5)%28;
            int color=(i+frame)%3==0?21:19;
            g.Line(x-2,cy,x+2,cy,color,1);g.Line(x,cy-2,x,cy+2,color,1);g.Px(x,cy,20);
        }
        if(frame>1 && frame<6){g.Line(29,29-frame,35,29-frame,18,1);g.Line(32,26-frame,32,32-frame,20,1);}
        if(frame==7)for(int py=0;py<64;py++)for(int px=0;px<64;px++)if((px+py)%3==0)g.Px(px,py,0);
        return g;
    }
    private sealed class Grid
    {
        public readonly Color32[] Pixels=new Color32[64*64];
        public void Px(int x,int y,int c){if(x>=0 && x<64 && y>=0 && y<64)Pixels[(63-y)*64+x]=Palette[c];}
        public void Rect(int x,int y,int w,int h,int c){for(int yy=y;yy<y+h;yy++)for(int xx=x;xx<x+w;xx++)Px(xx,yy,c);}
        public void Line(int x0,int y0,int x1,int y1,int c,int thickness)
        {
            int dx=Math.Abs(x1-x0),sx=x0<x1?1:-1,dy=-Math.Abs(y1-y0),sy=y0<y1?1:-1,err=dx+dy;
            while(true){Rect(x0-thickness/2,y0-thickness/2,thickness,thickness,c);if(x0==x1 && y0==y1)break;int e=2*err;if(e>=dy){err+=dy;x0+=sx;}if(e<=dx){err+=dx;y0+=sy;}}
        }
        public void Poly(int color,params int[] xy)
        {
            int n=xy.Length/2;
            for(int y=0;y<64;y++)for(int x=0;x<64;x++)
            {
                bool inside=false;for(int i=0,j=n-1;i<n;j=i++)
                    if((xy[2*i+1]>y)!=(xy[2*j+1]>y) && x<(float)(xy[2*j]-xy[2*i])*(y-xy[2*i+1])/(xy[2*j+1]-xy[2*i+1])+xy[2*i])inside=!inside;
                if(inside)Px(x,y,color);
            }
        }
        public void Ellipse(int cx,int cy,int rx,int ry,int color)
        {
            // Sample the low-resolution contour onto integer pixels only.
            for(int i=0;i<128;i++){float angle=i*Mathf.PI*2/128;Px(cx+Mathf.RoundToInt(Mathf.Cos(angle)*rx),cy+Mathf.RoundToInt(Mathf.Sin(angle)*ry),color);}
        }
    }
}
