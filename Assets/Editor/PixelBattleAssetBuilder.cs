using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// Скрипт ТІЛЬКИ для редактора: малює піксель-арт 64x64 кодом
// (скелет, вогонь, лікування, удар) і зберігає в PNG.
// Запускається з меню FalseMemory -> Art. У грі він не працює.
// Малюнки створені з нуля по сітці пікселів, а не зменшені з великої картинки.
public static class PixelBattleAssetBuilder
{
    // Куди зберігати кадри скелета
    public const string SkeletonRoot = "Assets/Art/Enemies/Skeleton64";
    // Куди зберігати ефекти
    public const string VfxRoot = "Assets/Art/VFX";

    // Палітра кольорів. У функціях малювання колір вказується НОМЕРОМ з цього масиву.
    // 0 - прозорий
    private static readonly Color32[] Palette =
    {
        new Color32(0, 0, 0, 0),           // 0  прозорий
        new Color32(24, 20, 29, 255),      // 1  майже чорний (обведення)
        new Color32(60, 62, 65, 255),      // 2  темно-сірий
        new Color32(99, 105, 106, 255),    // 3  сірий (метал)
        new Color32(153, 150, 125, 255),   // 4  світло-сірий
        new Color32(112, 77, 50, 255),     // 5  коричневий (іржа, дерево)
        new Color32(111, 112, 80, 255),    // 6  темна кістка
        new Color32(170, 167, 122, 255),   // 7  кістка
        new Color32(211, 204, 155, 255),   // 8  світла кістка
        new Color32(242, 226, 180, 255),   // 9  відблиск кістки
        new Color32(54, 24, 37, 255),      // 10 темно-бордовий (тканина)
        new Color32(102, 42, 52, 255),     // 11 бордовий
        new Color32(142, 58, 59, 255),     // 12 червоний
        new Color32(238, 158, 63, 255),    // 13 помаранчевий
        new Color32(237, 103, 39, 255),    // 14 яскраво-помаранчевий
        new Color32(192, 55, 35, 255),     // 15 червоний вогонь
        new Color32(255, 204, 104, 255),   // 16 жовтий вогонь
        new Color32(46, 77, 64, 255),      // 17 темно-зелений
        new Color32(72, 146, 109, 255),    // 18 зелений
        new Color32(109, 193, 140, 255),   // 19 світло-зелений
        new Color32(170, 226, 173, 255),   // 20 дуже світлий зелений
        new Color32(237, 230, 158, 255)    // 21 світло-жовтий
    };

    // Згенерувати всі кадри скелета і ефектів
    [MenuItem("FalseMemory/Art/Generate native 64px battle art")]
    public static void Generate()
    {
        if (EditorApplication.isPlaying)
        {
            throw new InvalidOperationException("Stop Play Mode before generating assets.");
        }

        // шлях до файлу -> малюнок
        Dictionary<string, Grid> outputs = new Dictionary<string, Grid>();

        // Кадри скелета для кожної анімації
        foreach (SpritePose pose in Enum.GetValues(typeof(SpritePose)))
        {
            // Скільки кадрів: біль - 2, атака - 3, інші - 4
            int count;
            if (pose == SpritePose.Hurt)
            {
                count = 2;
            }
            else if (pose == SpritePose.Attack)
            {
                count = 3;
            }
            else
            {
                count = 4;
            }

            for (int i = 0; i < count; i++)
            {
                // Наприклад: Skeleton-Move-00.png
                string path = SkeletonRoot + "/Skeleton-" + pose + "-" + i.ToString("00") + ".png";
                outputs.Add(path, Skeleton(pose, i));
            }
        }

        // По 8 кадрів вогню і лікування
        for (int i = 0; i < 8; i++)
        {
            outputs.Add(VfxRoot + "/Fire64/Fire-" + i.ToString("00") + ".png", Fire(i));
            outputs.Add(VfxRoot + "/Heal64/Heal-" + i.ToString("00") + ".png", Heal(i));
        }

        // Спочатку перевіряємо ВЕСЬ набір: генератор ніколи не перезаписує готові малюнки
        foreach (string path in outputs.Keys)
        {
            if (File.Exists(path))
            {
                throw new InvalidOperationException("Asset already exists: " + path);
            }
        }

        // Зберігаємо кожен малюнок
        foreach (KeyValuePair<string, Grid> pair in outputs)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pair.Key));
            SavePng(pair.Key, pair.Value);

            // Налаштовуємо імпорт як спрайт
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pair.Key);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.spritePivot = Vector2.one * 0.5f;
            ApplyPixelArtSettings(importer);
        }
    }

    // Записати малюнок у PNG і імпортувати в Unity
    private static void SavePng(string path, Grid grid)
    {
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        texture.SetPixels32(grid.Pixels);
        texture.Apply();
        File.WriteAllBytes(path, texture.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(texture);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
    }

    // Налаштування для піксель-арту: без розмиття, без міпмапів, без стиснення
    private static void ApplyPixelArtSettings(TextureImporter importer)
    {
        importer.filterMode = FilterMode.Point;
        importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.maxTextureSize = 64;
        importer.SaveAndReimport();
    }

    // Намалювати один кадр скелета
    private static Grid Skeleton(SpritePose pose, int frame)
    {
        Grid g = new Grid();

        // bob - скелет трохи "підстрибує" при ходьбі
        int bob = 0;
        if (pose == SpritePose.Move)
        {
            bob = frame % 2;
        }

        // stride - крок ногами при ходьбі (кадр 0: -2, кадр 2: +2, інші: 0)
        int stride = 0;
        if (pose == SpritePose.Move)
        {
            if (frame == 0)
            {
                stride = -2;
            }
            else if (frame == 2)
            {
                stride = 2;
            }
        }

        // lean - нахил тіла (біль - назад, атака - вперед)
        int lean = 0;
        if (pose == SpritePose.Hurt)
        {
            lean = -2;
        }
        else if (pose == SpritePose.Attack)
        {
            lean = frame;
        }

        // Останні кадри смерті - купа кісток на землі
        if (pose == SpritePose.Death && frame >= 2)
        {
            g.Line(20, 55, 45, 55, 1, 5);
            g.Line(21, 54, 38, 49, 7, 3);
            g.Line(39, 54, 49, 51, 8, 3);
            for (int i = 0; i < 3; i++)
            {
                g.Line(31 + i * 3, 51, 36 + i * 3, 55, 9, 1);
            }
            Skull(g, 25, 45, 0, false);
            Shield(g, 46, 50);
            Sword(g, 20, 52, 4, 57);
            return g;
        }

        // drop - при смерті скелет "осідає" вниз
        int drop = 0;
        if (pose == SpritePose.Death)
        {
            drop = frame * 7 + 2;
        }

        // Центр голови
        int cx = 32 + lean;
        int cy = 14 + bob + drop;

        // Ноги: кістки з кроком, ступні стоять на землі
        Bone(g, cx - 3, 41 + drop, cx - 6 + stride, 49 + drop);
        Bone(g, cx - 6 + stride, 49 + drop, cx - 8 + stride, 56);
        Bone(g, cx + 4, 41 + drop, cx + 7 - stride, 49 + drop);
        Bone(g, cx + 7 - stride, 49 + drop, cx + 8 - stride, 56);

        // Ступні
        g.Poly(1, cx - 10 + stride, 55, cx - 4 + stride, 55, cx - 3 + stride, 58, cx - 12 + stride, 58);
        g.Line(cx - 10 + stride, 56, cx - 4 + stride, 56, 8, 2);
        g.Poly(1, cx + 6 - stride, 55, cx + 12 - stride, 56, cx + 14 - stride, 58, cx + 5 - stride, 58);
        g.Line(cx + 7 - stride, 56, cx + 11 - stride, 57, 8, 2);

        // Тулуб і хребет
        g.Poly(1, cx - 8, 23 + bob + drop, cx + 7, 23 + bob + drop, cx + 7, 40 + drop, cx - 6, 42 + drop);
        g.Line(cx, 24 + bob + drop, cx, 40 + drop, 7, 3);

        // Ребра (4 штуки)
        for (int i = 0; i < 4; i++)
        {
            int y = 27 + i * 3 + bob + drop;
            g.Line(cx - 6, y, cx - 1, y + 1, 8, 2);
            g.Line(cx + 1, y + 1, cx + 6, y - 1, 7, 2);
            g.Line(cx - 5, y - 1, cx - 1, y, 9, 1);
            g.Px(cx + 5, y - 2, 8);
        }

        // Рвана бордова накидка під іржавим поясом (несиметрична навмисно)
        g.Poly(10, cx - 6, 38 + drop, cx + 7, 38 + drop, cx + 8, 48 + drop, cx + 3, 45 + drop, cx, 51 + drop, cx - 4, 46 + drop, cx - 7, 49 + drop);
        g.Poly(11, cx - 4, 39 + drop, cx + 4, 39 + drop, cx + 2, 45 + drop, cx, 48 + drop, cx - 2, 44 + drop);
        g.Line(cx - 5, 39 + drop, cx + 6, 39 + drop, 5, 2);
        g.Px(cx, 38 + drop, 4);
        g.Px(cx + 1, 39 + drop, 9);

        // Зап'ясток руки з мечем (при атаці рука піднімається)
        int wristX = cx - 12;
        int wristY = 34 + bob + drop;
        if (pose == SpritePose.Attack)
        {
            wristX = wristX - frame * 2;
            wristY = wristY - frame * 4;
        }

        // Ліва рука (з мечем)
        Bone(g, cx - 8, 25 + bob + drop, cx - 12, 30 + bob + drop);
        Bone(g, cx - 12, 30 + bob + drop, wristX, wristY);
        // Права рука (з щитом)
        Bone(g, cx + 9, 25 + bob + drop, cx + 12, 32 + bob + drop);
        Bone(g, cx + 12, 32 + bob + drop, cx + 14, 36 + bob + drop);

        // Кінчик меча. При атаці меч замахується по кадрах
        int tipX = 6;
        int tipY = 51;
        if (pose == SpritePose.Attack)
        {
            if (frame == 0)
            {
                tipX = 7;
                tipY = 40;
            }
            else if (frame == 1)
            {
                tipX = 8;
                tipY = 13;
            }
            else
            {
                tipX = 41;
                tipY = 9;
            }
        }

        Sword(g, wristX, wristY, tipX, tipY);
        // Кулак
        g.Rect(wristX - 1, wristY - 1, 3, 3, 8);

        Shield(g, cx + 16, 34 + bob + drop);

        // Ліве плече (наплічник)
        g.Poly(1, cx - 14, 22 + bob + drop, cx - 8, 19 + bob + drop, cx - 3, 23 + bob + drop, cx - 5, 28 + bob + drop, cx - 13, 28 + bob + drop);
        g.Poly(2, cx - 13, 23 + bob + drop, cx - 8, 21 + bob + drop, cx - 5, 24 + bob + drop, cx - 7, 27 + bob + drop, cx - 12, 27 + bob + drop);
        g.Line(cx - 12, 23 + bob + drop, cx - 8, 21 + bob + drop, 4, 1);
        g.Px(cx - 7, 24 + bob + drop, 5);
        g.Px(cx - 10, 26 + bob + drop, 5);

        // Праве плече
        g.Poly(1, cx + 5, 21 + bob + drop, cx + 12, 22 + bob + drop, cx + 14, 27 + bob + drop, cx + 6, 28 + bob + drop);
        g.Poly(2, cx + 6, 22 + bob + drop, cx + 11, 23 + bob + drop, cx + 12, 26 + bob + drop, cx + 6, 26 + bob + drop);
        g.Px(cx + 10, 23 + bob + drop, 4);
        g.Px(cx + 8, 25 + bob + drop, 5);

        // Комір
        g.Poly(10, cx - 7, 20 + bob + drop, cx + 6, 20 + bob + drop, cx + 3, 27 + bob + drop, cx - 3, 26 + bob + drop);
        g.Line(cx - 5, 21 + bob + drop, cx + 4, 22 + bob + drop, 11, 2);
        g.Px(cx + 2, 23 + bob + drop, 12);

        // Череп (очі світяться, якщо скелет не мертвий)
        Skull(g, cx, cy, lean, pose != SpritePose.Death);
        return g;
    }

    // Згенерувати картинку удару (окремий пункт меню)
    [MenuItem("FalseMemory/Art/Generate native 64px impact")]
    public static void GenerateImpact()
    {
        if (EditorApplication.isPlaying)
        {
            throw new InvalidOperationException("Stop Play Mode before generating assets.");
        }

        string path = VfxRoot + "/Hit64/Impact.png";
        if (File.Exists(path))
        {
            throw new InvalidOperationException("Asset already exists: " + path);
        }

        // Малюємо "зірку" удару
        Grid g = new Grid();
        g.Poly(13, 14, 43, 24, 38, 43, 18, 49, 15, 45, 25, 25, 45, 15, 49);
        g.Poly(21, 19, 43, 30, 34, 43, 20, 44, 24, 29, 40, 20, 46);
        g.Line(32, 12, 32, 20, 21, 1);
        g.Line(44, 32, 53, 32, 13, 1);
        g.Line(12, 29, 20, 29, 21, 1);
        g.Rect(47, 23, 2, 2, 21);
        g.Rect(20, 50, 2, 2, 13);

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        SavePng(path, g);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        ApplyPixelArtSettings(importer);
    }

    // Намалювати череп з центром (cx, cy). lit - чи світяться очі
    private static void Skull(Grid g, int cx, int cy, int tilt, bool lit)
    {
        // Контур і заливка
        g.Poly(1, cx - 6, cy - 7, cx + 4, cy - 7, cx + 7, cy - 4, cx + 7, cy + 2, cx + 4, cy + 7, cx - 3, cy + 7, cx - 6, cy + 3, cx - 8, cy - 2);
        g.Poly(6, cx - 5, cy - 6, cx + 3, cy - 6, cx + 6, cy - 3, cx + 5, cy + 2, cx + 3, cy + 5, cx - 3, cy + 5, cx - 6, cy + 1, cx - 6, cy - 3);
        g.Poly(8, cx - 4, cy - 5, cx + 3, cy - 5, cx + 5, cy - 2, cx + 4, cy + 1, cx - 4, cy + 2, cx - 6, cy - 1);

        // Відблиски зверху
        g.Line(cx - 3, cy - 5, cx + 2, cy - 5, 9, 2);
        g.Px(cx - 4, cy - 3, 9);
        g.Px(cx + 3, cy - 4, 7);

        // Очниці
        g.Poly(1, cx - 5, cy - 1, cx - 2, cy - 2, cx - 1, cy + 1, cx - 4, cy + 2);
        g.Poly(1, cx + 2, cy - 1, cx + 5, cy - 2, cx + 5, cy + 1, cx + 2, cy + 2);

        // Вогники в очах
        if (lit)
        {
            g.Px(cx - 3, cy, 13);
            g.Px(cx + 3, cy, 13);
        }

        // Ніс і рот
        g.Line(cx, cy + 1, cx, cy + 2, 1, 2);
        g.Line(cx - 3, cy + 4, cx + 3, cy + 4, 1, 1);

        // Зуби
        for (int i = -2; i <= 2; i = i + 2)
        {
            g.Px(cx + i, cy + 4, 9);
        }
        g.Line(cx - 2, cy + 6, cx + 2, cy + 6, 8, 1);

        // Тріщина
        g.Px(cx + 1, cy - 3, 6);
        g.Px(cx + 2, cy - 2, 6);
    }

    // Кістка: товста темна лінія + світліша + відблиск
    private static void Bone(Grid g, int ax, int ay, int bx, int by)
    {
        g.Line(ax, ay, bx, by, 1, 4);
        g.Line(ax, ay, bx, by, 7, 2);
        g.Line(ax, ay, bx, by, 9, 1);
    }

    // Меч від руки (x, y) до кінчика (tx, ty)
    private static void Sword(Grid g, int x, int y, int tx, int ty)
    {
        // Лезо
        g.Line(x, y, tx, ty, 1, 5);
        g.Line(x, y, tx, ty, 3, 3);
        g.Line(x, y, tx, ty, 4, 1);
        // Гарда
        g.Line(x - 3, y - 1, x + 3, y + 1, 1, 3);
        g.Line(x - 3, y - 1, x + 3, y + 1, 5, 1);
        // Блиск на кінчику
        g.Px(tx + 1, ty, 9);
    }

    // Щит з центром (cx, cy)
    private static void Shield(Grid g, int cx, int cy)
    {
        g.Poly(1, cx - 5, cy - 9, cx + 5, cy - 7, cx + 5, cy + 4, cx, cy + 10, cx - 5, cy + 5);
        g.Poly(3, cx - 4, cy - 7, cx + 3, cy - 6, cx + 3, cy + 3, cx, cy + 7, cx - 3, cy + 3);
        g.Poly(2, cx - 2, cy - 5, cx + 2, cy - 4, cx + 2, cy + 3, cx, cy + 5, cx - 2, cy + 2);
        g.Line(cx - 3, cy - 6, cx + 2, cy - 5, 4, 1);
        g.Line(cx, cy - 4, cx, cy + 4, 5, 2);
        // Заклепка в центрі
        g.Rect(cx - 1, cy - 1, 3, 3, 1);
        g.Px(cx, cy, 4);
        // Плями іржі
        g.Px(cx + 2, cy + 3, 5);
        g.Px(cx - 3, cy + 2, 5);
    }

    // Один кадр вогню (0..7)
    private static Grid Fire(int frame)
    {
        Grid g = new Grid();

        // Висота полум'я: спочатку росте, потім спадає (по синусоїді)
        float envelope = Mathf.Sin((frame + 1) / 9f * Mathf.PI);
        int h = 8 + Mathf.RoundToInt(envelope * 32);

        // Кінчик полум'я хитається вліво-вправо: -1, 0, 1
        int wobble = frame % 3 - 1;

        // 4 шари полум'я від темного до світлого
        g.Poly(10, 18, 49, 17, 41, 23, 34, 22, 28, 29, 37, 34, 50 - h, 38, 29, 44, 33, 42, 40, 47, 45, 43, 50, 30, 54);
        g.Poly(15, 21, 48, 22, 41, 27, 38, 26, 32, 31, 39, 34 + wobble, 54 - h, 38, 34, 41, 40, 44, 44, 40, 50, 30, 51);
        g.Poly(14, 25, 48, 26, 41, 30, 44, 34 + wobble, 61 - h, 37, 40, 41, 44, 37, 49, 31, 50);
        g.Poly(16, 29, 48, 31, 43, 34 + wobble, 68 - h, 36, 44, 38, 47, 34, 50);

        // Іскри
        if (frame < 7)
        {
            g.Rect(18 + (frame * 7) % 27, 25 - frame * 2, 2, 2, 16);
            g.Px(43 - frame, 29 + frame, 14);
            g.Px(22 + frame, 16 + frame, 15);
        }

        // Останні кадри - вогонь "розсипається": стираємо пікселі сіткою
        if (frame >= 6)
        {
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if ((x + y + frame) % 3 == 0 || (frame == 7 && (x + y) % 2 == 0))
                    {
                        g.Px(x, y, 0);
                    }
                }
            }
        }

        return g;
    }

    // Один кадр лікування (0..7)
    private static Grid Heal(int frame)
    {
        Grid g = new Grid();

        // Зелене кільце, яке росте і піднімається
        int r = 12 + frame;
        int y = 46 - frame;
        g.Ellipse(32, y, r, Math.Max(3, r / 3), 17);
        g.Ellipse(32, y - 1, r - 1, Math.Max(2, r / 3 - 1), 19);
        g.Line(32 - r + 4, y + 2, 32 + r - 4, y + 2, 20, 1);

        // 6 "плюсиків", які летять вгору
        for (int i = 0; i < 6; i++)
        {
            int x = 15 + i * 7 + (frame % 2);
            int cy = 39 - (frame * 3 + i * 5) % 28;

            int color = 19;
            if ((i + frame) % 3 == 0)
            {
                color = 21;
            }

            g.Line(x - 2, cy, x + 2, cy, color, 1);
            g.Line(x, cy - 2, x, cy + 2, color, 1);
            g.Px(x, cy, 20);
        }

        // Великий хрест у середині анімації
        if (frame > 1 && frame < 6)
        {
            g.Line(29, 29 - frame, 35, 29 - frame, 18, 1);
            g.Line(32, 26 - frame, 32, 32 - frame, 20, 1);
        }

        // Останній кадр - ефект зникає
        if (frame == 7)
        {
            for (int py = 0; py < 64; py++)
            {
                for (int px = 0; px < 64; px++)
                {
                    if ((px + py) % 3 == 0)
                    {
                        g.Px(px, py, 0);
                    }
                }
            }
        }

        return g;
    }

    // Полотно 64x64 пікселі з простими функціями малювання
    private sealed class Grid
    {
        public readonly Color32[] Pixels = new Color32[64 * 64];

        // Один піксель. y = 0 - це ВЕРХ картинки (тому перевертаємо: 63 - y)
        public void Px(int x, int y, int c)
        {
            if (x >= 0 && x < 64 && y >= 0 && y < 64)
            {
                Pixels[(63 - y) * 64 + x] = Palette[c];
            }
        }

        // Прямокутник
        public void Rect(int x, int y, int w, int h, int c)
        {
            for (int yy = y; yy < y + h; yy++)
            {
                for (int xx = x; xx < x + w; xx++)
                {
                    Px(xx, yy, c);
                }
            }
        }

        // Лінія заданої товщини (алгоритм Брезенхема)
        public void Line(int x0, int y0, int x1, int y1, int c, int thickness)
        {
            int dx = Math.Abs(x1 - x0);
            int sx;
            if (x0 < x1)
            {
                sx = 1;
            }
            else
            {
                sx = -1;
            }
            int dy = -Math.Abs(y1 - y0);
            int sy;
            if (y0 < y1)
            {
                sy = 1;
            }
            else
            {
                sy = -1;
            }
            int err = dx + dy;

            while (true)
            {
                // У кожній точці лінії малюємо квадратик розміром thickness
                Rect(x0 - thickness / 2, y0 - thickness / 2, thickness, thickness, c);

                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e = 2 * err;
                if (e >= dy)
                {
                    err = err + dy;
                    x0 = x0 + sx;
                }
                if (e <= dx)
                {
                    err = err + dx;
                    y0 = y0 + sy;
                }
            }
        }

        // Залитий многокутник. xy - координати вершин: x1, y1, x2, y2, ...
        public void Poly(int color, params int[] xy)
        {
            int n = xy.Length / 2;

            // Перевіряємо кожен піксель: чи він всередині многокутника
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    // Метод "променя": рахуємо, скільки разів промінь вправо перетинає сторони.
                    // Непарна кількість = точка всередині
                    bool inside = false;
                    int j = n - 1;
                    for (int i = 0; i < n; i++)
                    {
                        int xi = xy[2 * i];
                        int yi = xy[2 * i + 1];
                        int xj = xy[2 * j];
                        int yj = xy[2 * j + 1];

                        if ((yi > y) != (yj > y))
                        {
                            float crossX = (float)(xj - xi) * (y - yi) / (yj - yi) + xi;
                            if (x < crossX)
                            {
                                inside = !inside;
                            }
                        }

                        j = i;
                    }

                    if (inside)
                    {
                        Px(x, y, color);
                    }
                }
            }
        }

        // Контур еліпса (тільки лінія, без заливки)
        public void Ellipse(int cx, int cy, int rx, int ry, int color)
        {
            // Беремо 128 точок по колу і ставимо пікселі тільки в цілі координати
            for (int i = 0; i < 128; i++)
            {
                float angle = i * Mathf.PI * 2 / 128;
                int px = cx + Mathf.RoundToInt(Mathf.Cos(angle) * rx);
                int py = cy + Mathf.RoundToInt(Mathf.Sin(angle) * ry);
                Px(px, py, color);
            }
        }
    }
}
