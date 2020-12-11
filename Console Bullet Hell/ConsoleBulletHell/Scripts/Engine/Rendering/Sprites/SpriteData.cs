using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    public class SpriteData
    {
        public const string EXTENSION = ".sprt";

        public string name;

        public int resX;
        public int resY;

        public float pivotOffsetX;
        public float pivotOffsetY;

        public int order;
        
        public char[] pixels;

        public bool isMasker;
        public char[] mask;

        public static string GetFileName(string path, SpriteData data)
        {
            return Path.Combine(path, $"{data.name}{EXTENSION}");
        }

        public void Flush()
        {
            pixels = null;
            name = null;
        }

        public static SpriteData ToData(Bitmap bm, string file)
        {
            SpriteData data = new SpriteData();

            data.name = Path.GetFileNameWithoutExtension(file);
            data.resX = bm.Width;
            data.resY = bm.Height;

            data.pivotOffsetX = 0;
            data.pivotOffsetY = 0;

            data.order = 0;

            data.pixels = new char[data.resX * data.resY];
            for (int y = 0; y < data.resY; y++)
            {
                for (int x = 0; x < data.resX; x++)
                {
                    Color c = bm.GetPixel(x, y);
                    int i = y * data.resX + x;
                    data.pixels[i] = new FastColor(c.R, c.G, c.B, (byte)(c.A - byte.MaxValue)).ToChar();
                }
            }
            return data;
        }

        public string ToSaveString()
        {
            return $"{resX},{resY},{pivotOffsetX.ToString().Replace(",", ".")},{pivotOffsetY.ToString().Replace(",", ".")},{order}[{new string(pixels)}]";
        }

        public static SpriteData FromSaveString(string data)
        {
            int end;
            string dataPart = data.Substring(0, end = data.IndexOf('['));
            string pixelPart = data.Remove(0, end + 1);

            pixelPart = pixelPart.Substring(0, pixelPart.Length - 1);

            string[] datas = dataPart.Split(',');

            SpriteData finalData = new SpriteData()
            {
                pixels = pixelPart.ToCharArray(),

                pivotOffsetX = datas[2].ToFloat(0.0f),
                pivotOffsetY = datas[3].ToFloat(0.0f),
            };

            int.TryParse(datas[0], out finalData.resX);
            int.TryParse(datas[1], out finalData.resY);
            int.TryParse(datas[4], out finalData.order);  

            return finalData;
        }

        public static bool SaveToFile(Bitmap bm, string file, out string path, string additionPath = "")
        {
            SpriteData data = ToData(bm, file);
            return SaveToFile(data, path = GetFileName(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Console Bullethell/Sprites/{additionPath}"), data));
        }

        public static bool SaveToFile(SpriteData data, string file)
        {
            Extensions.ValidateDirectory(Path.GetDirectoryName(file));

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(data.resX.ToString());
            sb.AppendLine(data.resY.ToString());

            sb.AppendLine(data.pivotOffsetX.ToString());
            sb.AppendLine(data.pivotOffsetY.ToString());

            sb.AppendLine(data.order.ToString());

            int i = 0;
            for (int y = 0; y < data.resY; y++)
            {
                for (int x = 0; x < data.resX; x++)
                {
                    sb.Append(x == data.resX-1 ? $"{data.pixels[i]}\n" : $"{data.pixels[i]}");
                    i++;
                }
            }

            if (data.isMasker)
            {
                sb.AppendLine("1");
                for (int y = 0; y < data.resY; y++)
                {
                    for (int x = 0; x < data.resX; x++)
                    {
                        sb.Append(x == data.resX - 1 ? $"{data.mask[i]}\n" : $"{data.mask[i]}");
                        i++;
                    }
                }
            }

            File.WriteAllText(file, sb.ToString());
            return true;
        }
        public static SpriteData FromFile(string file)
        {
            if (!File.Exists(file)) { return null; }
            return FromData(File.ReadAllLines(file), Path.GetFileNameWithoutExtension(file));
        }
        public static SpriteData FromData(string[] lines, string name)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if(i == 5) { break; }
                lines[i] = lines[i].Trim();
            }

            SpriteData data = new SpriteData();
            data.name = name;

            data.resX = int.TryParse(lines[0], out data.resX) ? data.resX : 1;
            data.resY = int.TryParse(lines[1], out data.resY) ? data.resY : 1;

            data.pivotOffsetX = lines[2].ToFloat();
            data.pivotOffsetY = lines[3].ToFloat();

            int.TryParse(lines[4], out data.order);
 
            int totalLength = data.resX * data.resY;
            data.pixels = new char[totalLength];

            for (int y = 0; y < data.resY; y++)
            {
                for (int x = 0; x < data.resX; x++)
                {
                    data.pixels[y * data.resX + x] = lines[y + 5][x];
                }
            }

            int yy = 5 + data.resY;
            if (lines.Length > yy)
            {
                int.TryParse(lines[yy].Trim(), out int isMask);
                data.isMasker = isMask > 0;

                yy++;
                if (data.isMasker)
                {
                    data.mask = new char[data.pixels.Length];
                    for (int y = 0; y < data.resY; y++)
                    {
                        for (int x = 0; x < data.resX; x++)
                        {
                            data.mask[y * data.resX + x] = lines[y + yy][x];
                        }
                    }
                }
            }

            return data;
        }
    }
}