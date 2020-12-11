using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Joonaxii.ConsoleBulletHell
{
    public static class SpriteBank
    {
        private static Dictionary<string, Sprite> _sprites;
        private static Dictionary<string, Animation> _animations;

        public static void Load()
        {
            DirectoryInfo docs = new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Console Bullethell/Sprites");

            _sprites = new Dictionary<string, Sprite>();
            _animations = new Dictionary<string, Animation>();
 
            LoadSpritesFromResources();
            LoadAnimationsFromResources();
            if (docs.Exists)
            {
                LoadSpritesFromDocuments(docs);
                LoadAnimationsFromDocuments(docs);
            }
            GC.Collect();
        }

        private static void LoadSpritesFromResources()
        {
            SpriteData sprtData;
            List<string> dataS = new List<string>();
            List<string> nameS = new List<string>();

            string[] embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            if (embeddedResources == null || embeddedResources.Length < 1) { return; }

            for (int i = 0; i < embeddedResources.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(embeddedResources[i])) { continue; }
                if (!embeddedResources[i].EndsWith(SpriteData.EXTENSION)) { continue; }

                string n = embeddedResources[i].Replace(SpriteData.EXTENSION, "");
                nameS.Add(n = n.Remove(0, n.LastIndexOf(".") + 1));
          
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResources[i]))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    dataS.Add(System.Text.Encoding.UTF8.GetString(data));
                }
            }

            for (int i = 0; i < dataS.Count; i++)
            {
                if (_sprites.ContainsKey(nameS[i])) { continue; }

                sprtData = SpriteData.FromData(dataS[i].Split('\n'), nameS[i]);
                _sprites.Add(sprtData.name, new Sprite(sprtData));
                sprtData.Flush();
                sprtData = null;
            }
        }

        private static void LoadSpritesFromDocuments(DirectoryInfo docs)
        {
            SpriteData data;
            FileInfo[] infos = docs.GetFiles($"*{SpriteData.EXTENSION}", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < infos.Length; i++)
            {
                if (_sprites.ContainsKey(Path.GetFileNameWithoutExtension(infos[i].FullName))) { continue; }

                data = SpriteData.FromFile(infos[i].FullName);
                _sprites.Add(data.name, new Sprite(data));
                data.Flush();
                data = null;
            }
        }

        private static void LoadAnimationsFromResources()
        {
            Animation animData;
            List<string> dataS = new List<string>();
            List<string> nameS = new List<string>();

            string[] embeddedResources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            if (embeddedResources == null || embeddedResources.Length < 1) { return; }

            for (int i = 0; i < embeddedResources.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(embeddedResources[i])) { continue; }

                if (!embeddedResources[i].EndsWith(Animation.EXTENSION)) { continue; }

                string n = embeddedResources[i].Replace(Animation.EXTENSION, "");
                nameS.Add(n = n.Remove(0, n.LastIndexOf(".") + 1));

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResources[i]))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    dataS.Add(System.Text.Encoding.UTF8.GetString(data));
                }
            }

            for (int i = 0; i < dataS.Count; i++)
            {
                if (_animations.ContainsKey(nameS[i])) { continue; }

                animData = Animation.LoadFromData(dataS[i]);
                _animations.Add(nameS[i], animData);
                animData = null;
            }
        }

        private static void LoadAnimationsFromDocuments(DirectoryInfo docs)
        {
            DirectoryInfo[] animInfos = docs.GetDirectories("*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < animInfos.Length; i++)
            {
                string animationName = animInfos[i].Name;

                if (_animations.ContainsKey(animationName)) { continue; }
                FileInfo[] animData = animInfos[i].GetFiles($"*{Animation.EXTENSION}");
                if (animData.Length < 1) { continue; }

                Animation anim = Animation.LoadFromData(File.ReadAllText(animData[0].FullName));
                _animations.Add(animationName, anim);
            }
        }

        public static Sprite GetSprite(string name)
        {
            return _sprites.TryGetValue(name, out Sprite sprt) ? sprt : null;
        }

        public static Animation GetAnimation(string name)
        {
            return _animations.TryGetValue(name, out Animation sprt) ? sprt : null;
        }
    }
}