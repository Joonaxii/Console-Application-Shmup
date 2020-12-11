using System;
using System.Diagnostics;

namespace Joonaxii.ConsoleBulletHell
{
    public class Time
    {
        public float TimeScale = 1.0f;

        public float RunTime { get; private set; }
        public float DeltaTime { get; private set; }

        public float FrameRate { get; private set; }
        public long FrameCount { get; private set; }

        private Stopwatch _sw;
 
        private double _curTime;
        private double _prevTime;

        public void Initialize()
        {
            _sw = new Stopwatch();

            FrameRate = 0;

            RunTime = 0;
            DeltaTime = 0;
            FrameCount = 0;
            _sw = Stopwatch.StartNew();
        }

        public void Reset()
        {
            FrameRate = 0;

            RunTime = 0;
            DeltaTime = 0;
            FrameCount = 0;
            _sw = Stopwatch.StartNew();
        }

        public WaitForSeconds WaitForSeconds(float dur)
        {
            return new WaitForSeconds(this, dur);
        }

        public void Tick()
        {
            _prevTime = _curTime;
            _curTime = _sw.Elapsed.TotalSeconds;
        
            DeltaTime = (float)(_curTime - _prevTime) * TimeScale;
            RunTime += DeltaTime;
           
            FrameCount++;
            FrameRate = FrameCount / RunTime;
        }
    }
}