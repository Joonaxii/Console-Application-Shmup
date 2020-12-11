using System.Collections;

namespace Joonaxii.ConsoleBulletHell
{
    public class Background
    {
        public float _scrollingSpeed = 1.0f;

        private const int BG_COUNT = 4;
        private const float SIZE_HILL = 64;
        private const float SIZE_HILL_HALF = SIZE_HILL * 0.5f;

        private Sprite _hill;
        private Sprite _starsSprite;
        private Sprite _moonSprite;

        private Entity[] _hills1;
        private Entity[] _hills2;
        private Entity[] _stars;
        private Entity[] _moon;

        private float _yPosHills1 = 5;
        private float _yPosHills2 = 2;
        private float _yPosStars = -20;

        private float _startPos;
        private float _startPos2;

        private float _offsetPosHills1;
        private float _offsetPosHills2;
        private float _offsetPosStars;
        private float _offsetPosMoon;

        private float _offset2 = 7;

        public Background()
        {
            _hill = SpriteBank.GetSprite("Background_Hills");
            _starsSprite = SpriteBank.GetSprite("Background_Stars");
            _moonSprite = SpriteBank.GetSprite("Background_Moon");

            _hills1 = new Entity[BG_COUNT];
            _hills2 = new Entity[BG_COUNT];
            _stars = new Entity[BG_COUNT];
            _moon = new Entity[BG_COUNT];
            for (int i = 0; i < BG_COUNT; i++)
            {
                _hills1[i] = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, -100, _hill, null, Vector2.zero, null);
                _hills2[i] = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, -110, _hill, null, Vector2.zero, null);
                _stars[i] = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, -120, _starsSprite, null, Vector2.zero, null);

                if (i == 0)
                {
                    _moon[i] = new Entity(EntityID.PROP, EntityType.NONE, 0, 0, false, -115, _moonSprite, null, Vector2.zero, null);
                }
            }

             _startPos = SIZE_HILL * BG_COUNT;
             _startPos2 = _startPos;
        }

        public void Start(float scrollingSpeed)
        {
            _offsetPosHills1 = 0;
            _offsetPosHills2 = 0;
            _offsetPosStars = 0;
            _offsetPosMoon = 0;

            _scrollingSpeed = scrollingSpeed;

            for (int i = 0; i < BG_COUNT; i++)
            {
                float val = i * SIZE_HILL;
                float pos1 = _startPos - i * SIZE_HILL;
                _hills1[i].Spawn(new Vector2(pos1, _yPosHills1), Vector2.zero); ;
                _hills2[i].Spawn(new Vector2(_startPos2 - val, _yPosHills2), Vector2.zero);

                Vector2 star = new Vector2(pos1, _yPosStars);
                _stars[i].Spawn(star, Vector2.zero);
                if (i == 0)
                {
                    _moon[i].Spawn(star, Vector2.zero);
                }
            }
        }

        private IEnumerator _fade;

        public void Update(float delta)
        {
            _fade?.MoveNext();

            delta *= _scrollingSpeed;

            _offsetPosHills1 += delta * 25;
            _offsetPosHills2 += delta * 20;
            _offsetPosStars += delta * 10;
            _offsetPosMoon += delta * 9;
   
            float plrPos = Maths.InverseLerp(Player.MIN_Y, Player.MAX_Y, Program.Player.Position.y);

            _yPosHills1 = Maths.Lerp(11, 4, plrPos);
            _yPosHills2 = Maths.Lerp(7, 1, plrPos);
            _yPosStars = Maths.Lerp(-5, -20, plrPos);

            if (_offsetPosHills1 >= SIZE_HILL)
            {
                _offsetPosHills1 -= SIZE_HILL;
            }

            if (_offsetPosHills2 >= SIZE_HILL)
            {
                _offsetPosHills2 -= SIZE_HILL;
            }

            if (_offsetPosStars >= SIZE_HILL)
            {
                _offsetPosStars -= SIZE_HILL;
            }

            if (_offsetPosMoon >= 200)
            {
                _offsetPosMoon -= 200;
            }

            for (int i = 0; i < BG_COUNT; i++)
            {
                float rel = -SIZE_HILL + i * SIZE_HILL;

                _hills1[i].SetPosition(new Vector2(rel - _offsetPosHills1, _yPosHills1));
                _hills2[i].SetPosition(new Vector2(rel - _offsetPosHills2 + _offset2, _yPosHills2));
                _stars[i].SetPosition(new Vector2(rel - _offsetPosStars, _yPosStars));

                if (i == 0)
                {
                    _moon[i].Spawn(new Vector2(90 - _offsetPosMoon, _yPosStars - 12), Vector2.zero);
                }
            }
        }

        public void StartScrollSpeedFade(float to, float duration, FadingType type)
        {
            _fade = ScrollSpeedFader(to, duration, type);
        }

        private IEnumerator ScrollSpeedFader(float to, float duration, FadingType type)
        {
            float start = _scrollingSpeed;
            float t = 0;

            while(t < duration)
            {
                float tn = t / duration;
                switch (type)
                {
                    case FadingType.LINEAR:
                        _scrollingSpeed = Maths.Lerp(start, to, tn);
                        break;
                    case FadingType.SMOOTH_STEP:
                        _scrollingSpeed = Maths.Lerp(start, to, Maths.SmoothStep(0, 1, tn));
                        break;
                    case FadingType.FADE_IN:
                        _scrollingSpeed = Maths.Lerp(start, to, Maths.EaseIn(tn));
                        break;
                    case FadingType.FADE_OUT:
                        _scrollingSpeed = Maths.Lerp(start, to, Maths.EaseOut(tn));
                        break;
                }

                t += Program.UpdateTime.DeltaTime;
                yield return null;
            }

            _scrollingSpeed = to;
            _fade = null;
        }

        public void Stop()
        {
            _fade = null;
            if (_hills1 == null) { return; }
            for (int i = 0; i < BG_COUNT; i++)
            {
                _hills1[i].Despawn(true);
                _hills2[i].Despawn(true);
                _stars[i].Despawn(true);
                if (i == 0)
                {
                    _moon[i].Despawn(true);
                }
            }
        }
    }
}