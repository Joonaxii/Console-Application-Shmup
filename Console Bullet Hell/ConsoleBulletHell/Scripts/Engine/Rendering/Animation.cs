using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    public class Animation
    {
        public const string EXTENSION = ".anim";

        public Sprite[] Frames;
        private float _frameTime;

        public Animation(float fps, Sprite[] sprites)
        {
            Frames = sprites;
            _frameTime = 1.0f / fps;
        }

        public bool Animate(Entity entity, ref float time, ref int frame, float deltaTime,bool reversed = false)
        {
            time += deltaTime;

            if (time < _frameTime) { return false; }
            time = time - _frameTime;

            entity.Sprite = Frames[reversed ? frame-- : frame++];
            frame = reversed ? (frame < 0 ?  Frames.Length-1 : frame) : frame % Frames.Length;

            return frame == (reversed ? Frames.Length - 1 : 0);
        }

        public static void ToFile(string path, float fps, SpriteData[] sprites)
        {
            Extensions.ValidateDirectory(Path.GetDirectoryName(path));

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(fps.ToString().Replace(",", "."));
            for (int i = 0; i < sprites.Length; i++)
            {
                sb.AppendLine(sprites[i].ToSaveString());
            }

            File.WriteAllText(path, sb.ToString());
        }

        public static Animation LoadFromData(string data)
        {
            string[] lines = data.Split('\n');

            float fps = 24.0f;
            List<Sprite> sprites = new List<Sprite>();
            Sprite temp;

            for (int i = 0; i < lines.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        fps = lines[i].ToFloat(24.0f);
                        break;
                    default:
                        if (string.IsNullOrWhiteSpace(lines[i])) { continue; }

                        temp = new Sprite(SpriteData.FromSaveString(lines[i].Trim()));
                        if(temp != null)
                        {
                            sprites.Add(temp);
                        }
                        break;
                }
            }
            return new Animation(fps, sprites.ToArray());
        }
    }
}