using System;
using System.Text;

namespace Joonaxii.ConsoleBulletHell
{
    public class HealthBarManager
    {
        public const int BOSS_HEALTH_BAR_WIDTH = 102;
        public const int PLAYER_HEALTH_BAR_WIDTH = 42;

        private HealthBar[] _bossBars;
        private StringBuilder _strBoss = new StringBuilder(new string(' ', BOSS_HEALTH_BAR_WIDTH));

        private HealthBar[] _plrBars;
        private StringBuilder _strPlr = new StringBuilder(new string(' ', PLAYER_HEALTH_BAR_WIDTH));

        public HealthBarManager()
        {
            _bossBars = new HealthBar[BOSS_HEALTH_BAR_WIDTH];
            for (int i = 0; i < BOSS_HEALTH_BAR_WIDTH; i++)
            {
                _bossBars[i] = new HealthBar(new char[] { '-', '░', '▒', '▓', '█' });
            }

            _plrBars = new HealthBar[PLAYER_HEALTH_BAR_WIDTH];
            for (int i = 0; i < PLAYER_HEALTH_BAR_WIDTH; i++)
            {
                _plrBars[i] = new HealthBar(new char[] { '-', '░', '▒', '▓', '█' });
            }
        }

        public void DrawScore(long cur, long high, bool isNew)
        {
            Renderer.WriteText(Extensions.GetCentered($"Score: {cur.AddSpaces()}"), 4);
            Renderer.WriteText(Extensions.GetCentered($"{(isNew ? "NEW " : "")}Hi-Score: {high.AddSpaces()}"), 5);
        }

        public void UpdateBossHealthBar(int cur, int max)
        {
            float t = cur / Math.Max(max - 1.0f, 1.0f);
            t = t < 0 ? 0 : t > 1.0f ? 1.0f : t;

            float scaled = t * (BOSS_HEALTH_BAR_WIDTH - 2);
            int whole = Maths.FloorToInt(scaled);
            int deci = (int)((scaled - whole) * (_bossBars[0].CharCount - 1.0f));

            _strBoss[0] = '[';
            _strBoss[BOSS_HEALTH_BAR_WIDTH - 1] = ']';

            for (int i = 1; i < _bossBars.Length - 1; i++)
            {
                int ii = i - 1;
                _strBoss[i] = _bossBars[i].CalcualtePoints(whole < ii  ? 0 : whole > ii ? 4 : deci);
            }

            Renderer.WriteText(Extensions.GetCentered($"Boss Health: {cur.AddSpaces()}/{max.AddSpaces()}"), 7);
            Renderer.WriteText(Extensions.GetCentered(_strBoss.ToString()), 8);
        }

        public void UpdatePlayerHealthBar(int cur, int max, int curS, int maxS, float special)
        {
            float t = cur / Math.Max(max - 1.0f, 1.0f);
            t = t < 0 ? 0 : t > 1.0f ? 1.0f : t;

            float scaled = t * (PLAYER_HEALTH_BAR_WIDTH - 2);
            int whole = Maths.FloorToInt(scaled);
            int deci = (int)((scaled - whole) * (_plrBars[0].CharCount - 1.0f));

            _strPlr[0] = '[';
            _strPlr[PLAYER_HEALTH_BAR_WIDTH - 1] = ']';
            for (int i = 1; i < _plrBars.Length - 1; i++)
            {
                int ii = i - 1;
                _strPlr[i] = _plrBars[i].CalcualtePoints(whole < ii ? 0 : whole > ii ? 4 : deci);
            }

            Renderer.WriteText(Extensions.GetCentered($"Player Health: {cur.AddSpaces()}/{max.AddSpaces()} [Special: {(special >= 0.99f ? "RDY!" :($"{(special * 100.0f).ToString("F1")}%") )}]"), 1);
            Renderer.WriteText(Extensions.GetCentered($"{_strPlr.ToString()} Stock: {curS}/{maxS}"), 2);
        }

        private class HealthBar
        {
            public int CharCount;
            private char[] _chars;
      
            public HealthBar(char[] healthIcons)
            {
                _chars = healthIcons;
                CharCount = _chars.Length;
            }

            public char CalcualtePoints(int currentDeci)
            {
                return _chars[currentDeci];
            }
        }
    }
}