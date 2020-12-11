using System.Threading;

namespace Joonaxii.ConsoleBulletHell
{
    public class StoppableThread 
    {
        public bool IsRunning { get; private set; }
        public bool ActuallyRunning { get; set; }
        private Thread _thread;
           
        public StoppableThread(Thread thread)
        {
            _thread = thread;
            _thread.IsBackground = true;
            IsRunning = false;
        }

        public void Start()
        {
            IsRunning = true;
            ActuallyRunning = true;
            _thread.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            _thread = null;
        }
    }
}