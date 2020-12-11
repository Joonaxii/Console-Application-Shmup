using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;

namespace Joonaxii.ConsoleBulletHell
{
    class Program
    {
        public const string TITLE = "~~Joonaxii's Console Bullet Hell~~";
        public static bool DEBUG = false;
        public static bool SYNC_THREADS = true;

        public static Difficulty Difficulty;

        /// <summary>
        /// Sets input suppression to true/false
        /// </summary>
        public static bool INPUTS_SUPPRESSED
        {
            get
            {
                return _suppressInputs;
            }

            set
            {
                _suppressInputs = value;
                if (!_suppressInputs)
                {
                    if (_inputThread != null)
                    {
                        _inputThread.Stop();
                    }
                    return;
                }
                if (_inputThread != null) { return; }

                _inputThread = new StoppableThread(new Thread(SuppressInputsLoop));
                _inputThread.Start();
            }
        }

        #region Managers
        public static PoolManager PoolManager { get; private set; }
        public static EntityManager EntityManager { get; private set; }
        public static CollisionSystem CollisionSystem { get; private set; }
        public static HealthBarManager HealthBarManager { get; private set; }
        public static BulletPatternManager BulletPatternManager { get; private set; }
        public static LevelManager LevelManager { get; private set; }
        #endregion

        #region Public Fields
        public static Time UpdateTime { get; private set; }
        public static Time RenderTime { get; private set; }
        public static Player Player { get => LevelManager?.Player; }
        #endregion

        #region Private Fields
        private static readonly string[] _skipInit = new string[4];

        private static MenuButton[] _mainButtons;
        private static MenuButton[] _difficultyButtons;
        private static MenuButton[] _difficultyConfirmButtons;
        private static Time menuTime;

        private static StoppableThread _updateThread;
        private static StoppableThread _renderThread;
        private static StoppableThread _inputThread;

        private static bool _suppressInputs;
        private static int _exitRequested;

        private static int _currentMain = 0;
        private static int _currentDiff = 0;
        private static long _renderFrame = -1;

        #endregion

        /// <summary>
        /// The "entry point" of the program, intializes everything
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args == null || args.Length < 4)
            {
                menuTime = new Time();
                menuTime.Initialize();

                _mainButtons = new MenuButton[]
                {
                    new MenuButton("Start Game!", DifficultySelection),

                    new MenuButton("Import PNG as a sprite!", SpriteCreator),
                    new MenuButton("Import PNGs as an animation!", AnimationCreator),

                    new MenuButton("Quit Game!", () => _exitRequested = 3),
                };
                Difficulty[] difficulties = Enum.GetValues(typeof(Difficulty)) as Difficulty[];

                _difficultyButtons = new MenuButton[difficulties.Length + 1];
                for (int i = 0; i < difficulties.Length; i++)
                {
                    Difficulty diff = difficulties[i];
                    _difficultyButtons[i] = new MenuButton($"{diff.ToString().Replace("_", " ")}", () => DifficultyConfirm(diff));
                }

                _difficultyButtons[_difficultyButtons.Length - 1] = new MenuButton("Go Back!", () => Main(_skipInit));
                _difficultyConfirmButtons = new MenuButton[]
                {
                    new MenuButton("Yes!", GameInit),
                    new MenuButton("Nah!", DifficultySelection),
                };

                #region Essential Initialization Stuff
                Console.OutputEncoding = Encoding.Unicode;

                INPUTS_SUPPRESSED = true;
                CollisionSystem = new CollisionSystem();

                SpriteBank.Load();

                HealthBarManager = new HealthBarManager();
                EntityManager = new EntityManager();

                PoolManager = new PoolManager();
                PoolManager.Load();

                BulletPatternManager = new BulletPatternManager();

                LevelManager = new LevelManager();
                #endregion

            }

            #region Menu Selection

            Renderer.SetSize(60, 25);
            Thread.Sleep(100);

            INPUTS_SUPPRESSED = true;
            Console.Clear();
            Console.ResetColor();
            _exitRequested = 0;
            _currentMain = 0;

            Console.WriteLine(Extensions.GetCentered(TITLE));

            for (int i = 0; i < _mainButtons.Length; i++)
            {
                _mainButtons[i].Draw(i + 2, i == _currentMain, 0, true);
            }

            Player.LoadHighScore();

            Console.WriteLine("\n");
            Console.WriteLine(Extensions.GetCentered("<<Controls>>"));
            Console.WriteLine(Extensions.GetCentered("Movement => Arrow Keys"));
            Console.WriteLine(Extensions.GetCentered("Shoot/Select => Z"));
            Console.WriteLine(Extensions.GetCentered("Slower Mov. Speed => X"));
            Console.WriteLine(Extensions.GetCentered("Special => C"));
            Console.WriteLine(Extensions.GetCentered("Alt Fire => Z+X"));
            Console.WriteLine("\n");

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(Extensions.GetCentered($"HI-Score ({((Difficulty)i).ToString().Replace("_", " ")}): {Player.HighScore[i].AddSpaces()}"));
            }

            menuTime.Initialize();
            Input.Update();
            int ii;
            while (true)
            {
                Input.Update();
                menuTime.Tick();

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    _mainButtons[_currentMain].Press();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Up))
                {
                    ii = _currentMain;
                    _currentMain--;
                    if (_currentMain < 0)
                    {
                        _currentMain = _mainButtons.Length - 1;
                    }

                    while (!_mainButtons[_currentMain].IsEnabled & ii != _currentMain)
                    {
                        _currentMain--;
                        if (_currentMain < 0)
                        {
                            _currentMain = _mainButtons.Length - 1;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Down))
                {
                    ii = _currentMain;
                    _currentMain++;
                    if (_currentMain >= _mainButtons.Length)
                    {
                        _currentMain = 0;
                    }

                    while (!_mainButtons[_currentMain].IsEnabled & ii != _currentMain)
                    {
                        _currentMain++;
                        if (_currentMain >= _mainButtons.Length)
                        {
                            _currentMain = 0;
                        }
                    }
                }

                for (int i = 0; i < _mainButtons.Length; i++)
                {
                    _mainButtons[i].Draw(i + 2, i == _currentMain, menuTime.DeltaTime, false);
                }

                if (_exitRequested == 3)
                {
                    return;
                }
            }
            #endregion

        }

        private static void AnimationCreator()
        {
        #region Animation Creator Block
  
            string path = "";
            Bitmap bm;

            Console.Clear();
            Console.WriteLine("Welcome to the sprite animator!");
            Console.WriteLine("Press Enter to make an animation or Esc to go back");
            Input.Update();

            while (true)
            {
                Input.Update();
                if (Input.GetKeyDown(KeyCode.Esc))
                {
                    Main(_skipInit);
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Enter))
                {
                    goto animation;
                }
            }

        animation:

            Input.Update();

            Console.Clear();
            INPUTS_SUPPRESSED = false;
            Console.WriteLine("Enter the folder that you want to convert into an animatoin below");
            path = "";
            DirectoryInfo pathName;

            FileInfo[] pngs;

            //ANIMATION FOLDER PATH
            while (true)
            {
                path = Console.ReadLine().Replace("\\", "/").Replace("\"", "");
                if (Directory.Exists(path))
                {
                    pngs = (pathName = new DirectoryInfo(path)).GetFiles("*.png", SearchOption.TopDirectoryOnly);
                    if (pngs.Length < 1)
                    {
                        Console.Clear();
                        Console.WriteLine($"Not enough PNGs in: {path}!");

                        Thread.Sleep(1000);
                        goto animation;
                    }
                    break;
                }

                if (string.IsNullOrWhiteSpace(path)) { goto animation; }

                Console.Clear();
                Console.WriteLine($"Directory: {path} doesn't exists!");

                Thread.Sleep(1000);
                goto animation;
            }

        fpsSelect:

            INPUTS_SUPPRESSED = false;
            Console.Clear();
            Console.WriteLine("Enter the framerate that you want to use below");

            string fpsStr = Console.ReadLine();
            if (!float.TryParse(fpsStr.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float frameRate))
            {
                Console.Clear();
                Console.WriteLine($"Input: {fpsStr} is invalid!");

                Thread.Sleep(1000);
                goto fpsSelect;
            }

            Console.Clear();

            frameRate = Math.Max(frameRate, 0.00001f);
            string pathAnimation = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Console Bullethell/Sprites/{pathName.Name}");
            if (!Directory.Exists(pathAnimation))
            {
                Directory.CreateDirectory(pathAnimation);
            }

            SpriteData[] dats = new SpriteData[pngs.Length];
            for (int i = 0; i < pngs.Length; i++)
            {
                bm = new Bitmap(pngs[i].FullName);
                dats[i] = SpriteData.ToData(bm, pngs[i].FullName);

                bm.Dispose();
                bm = null;
            }

            Animation.ToFile(Path.Combine(pathAnimation, $"{pathName.Name}{Animation.EXTENSION}"), frameRate, dats);
            dats = null;

            GC.Collect();
            Console.WriteLine($" ");
            Console.WriteLine($"Animation sucessfully generated to {pathAnimation}!");
            Console.WriteLine($"Press enter to return to the menu");
            INPUTS_SUPPRESSED = true;

            Input.Update();
            while (true)
            {
                Input.Update();
                if (Input.GetKeyDown(KeyCode.Enter))
                {
                    SpriteBank.Load();
                    Main(_skipInit);
                    return;
                }
            }
            #endregion
        }

        private static void SpriteCreator()
        {
        #region Sprite Creator Block

            string path = "";
            Bitmap bm;

            INPUTS_SUPPRESSED = true;

            Console.Clear();
            Console.WriteLine("Welcome to the sprite creator!");
            Console.WriteLine("Press Enter to make a sprite or Esc to go back");
            Input.Update();

            while (true)
            {
                Input.Update();
                if (Input.GetKeyDown(KeyCode.Esc))
                {
                    Main(_skipInit);
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Enter))
                {
                    goto confirm;
                }
            }

        //SPRITE CREATOR
        confirm:
            Thread.Sleep(100);
            Console.Clear();

            INPUTS_SUPPRESSED = false;

            bool isBatch = false;
            Console.WriteLine("Import a folder of PNGs? (Y/N)");

            isBatch = Console.ReadKey().Key == ConsoleKey.Y;

        png:
            Thread.Sleep(100);
            Console.Clear();

            Console.WriteLine(isBatch ? "Enter the folder withe the PNGs that you want to make a sprites out of below" : "Enter the PNG that you want to make a sprite out of below");

            FileInfo[] paths = null;
            //IMAGE PATH
            while (true)
            {
                path = Console.ReadLine().Replace("\\", "/").Replace("\"", "");

                if (isBatch)
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo dirInf = new DirectoryInfo(path);
                        paths = dirInf.GetFiles("*.png", SearchOption.TopDirectoryOnly);
                        break;
                    }
                    Console.Clear();
                    Console.WriteLine($"File: {path} doesn't exists!");

                    Thread.Sleep(1000);
                    goto png;
                }

                if (File.Exists(path))
                {
                    if (!path.ToLower().EndsWith(".png"))
                    {
                        Console.Clear();
                        Console.WriteLine($"File: {path} is not a PNG!");

                        Thread.Sleep(1000);
                        goto png;
                    }

                    break;
                }
                Console.Clear();
                Console.WriteLine($"File: {path} doesn't exists!");

                Thread.Sleep(1000);
                goto png;
            }

            //COLORS TO CHARS
            string outPath;

            if (isBatch)
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    path = paths[i].FullName;
                    bm = new Bitmap(path);
                    if (SpriteData.SaveToFile(bm, path, out outPath))
                    {
                        bm.Dispose();
                        bm = null;

                        Console.WriteLine($"Sprite sucessfully generated to {outPath}!");
                    }
                }

                GC.Collect();
                INPUTS_SUPPRESSED = true;
                Console.WriteLine($"Press enter to return to the menu");
                Input.Update();
                while (true)
                {
                    Input.Update();
                    if (Input.GetKeyDown(KeyCode.Enter))
                    {
                        SpriteBank.Load();
                        Main(_skipInit);
                        return;
                    }
                }
            }


            bm = new Bitmap(path);
            if (SpriteData.SaveToFile(bm, path, out outPath))
            {
                bm.Dispose();
                bm = null;
                GC.Collect();

                INPUTS_SUPPRESSED = true;
                Console.Clear();
                Console.WriteLine($"Sprite sucessfully generated to {outPath}!");
                Console.WriteLine($"Press enter to return to the menu");
                Input.Update();
                while (true)
                {
                    Input.Update();
                    if (Input.GetKeyDown(KeyCode.Enter))
                    {
                        SpriteBank.Load();
                        Main(_skipInit);
                        return;
                    }
                }
            }

            Console.Clear();
            Console.WriteLine($"Sprite creation unsuccessful!");
            Console.WriteLine($"Press enter to return to the menu");

            while (true)
            {
                Input.Update();

                if (Input.GetKeyDown(KeyCode.Enter))
                {
                    Main(_skipInit);
                    return;
                }
            }

            #endregion
        }

        private static void DifficultySelection()
        {
            _currentDiff = (int)Difficulty;
            INPUTS_SUPPRESSED = true;
            _exitRequested = 0;

            Console.Clear();
            Console.ResetColor();
            Console.WriteLine(Extensions.GetCentered($"Please enter the difficulty you want to play in"));

            for (int i = 0; i < _difficultyButtons.Length; i++)
            {
                _difficultyButtons[i].Draw(i + 2, i == _currentDiff, 0, true);
            }

            menuTime.Initialize();
            Input.Update();
            int ii;
            while (true)
            {
                Input.Update();
                menuTime.Tick();

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    _difficultyButtons[_currentDiff].Press();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Up))
                {
                    ii = _currentDiff;
                    _currentDiff--;
                    if (_currentDiff < 0)
                    {
                        _currentDiff = _difficultyButtons.Length - 1;
                    }

                    while (!_difficultyButtons[_currentDiff].IsEnabled & ii != _currentDiff)
                    {
                        _currentDiff--;
                        if (_currentDiff < 0)
                        {
                            _currentDiff = _difficultyButtons.Length - 1;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Down))
                {
                    ii = _currentDiff;
                    _currentDiff++;
                    if (_currentDiff >= _difficultyButtons.Length)
                    {
                        _currentDiff = 0;
                    }

                    while (!_difficultyButtons[_currentDiff].IsEnabled & ii != _currentDiff)
                    {
                        _currentDiff++;
                        if (_currentDiff >= _difficultyButtons.Length)
                        {
                            _currentDiff = 0;
                        }
                    }
                }

                for (int i = 0; i < _difficultyButtons.Length; i++)
                {
                    _difficultyButtons[i].Draw(i + 2, i == _currentDiff, menuTime.DeltaTime, false);
                }
            }
        }

        private static void DifficultyConfirm(Difficulty diff)
        {
            Difficulty = diff;

            _currentDiff = 0;
            Input.Update();

            INPUTS_SUPPRESSED = true;
            _exitRequested = 0;

            Console.Clear();
            Console.WriteLine(Extensions.GetCentered($"You have chosen the difficulty '{Difficulty.ToString().Replace("_", " ")}'"));
            Console.WriteLine(Extensions.GetCentered($"Are you sure you want to use the selected difficulty?"));

            for (int i = 0; i < _difficultyConfirmButtons.Length; i++)
            {
                _difficultyConfirmButtons[i].Draw(i + 3, i == _currentDiff, 0, true);
            }

            menuTime.Initialize();
            Input.Update();
            int ii;
            while (true)
            {
                Input.Update();
                menuTime.Tick();

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    _difficultyConfirmButtons[_currentDiff].Press();
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Up))
                {
                    ii = _currentDiff;
                    _currentDiff--;
                    if (_currentDiff < 0)
                    {
                        _currentDiff = _difficultyConfirmButtons.Length - 1;
                    }

                    while (!_difficultyConfirmButtons[_currentDiff].IsEnabled & ii != _currentDiff)
                    {
                        _currentDiff--;
                        if (_currentDiff < 0)
                        {
                            _currentDiff = _difficultyConfirmButtons.Length - 1;
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Down))
                {
                    ii = _currentDiff;
                    _currentDiff++;
                    if (_currentDiff >= _difficultyConfirmButtons.Length)
                    {
                        _currentDiff = 0;
                    }

                    while (!_difficultyConfirmButtons[_currentDiff].IsEnabled & ii != _currentDiff)
                    {
                        _currentDiff++;
                        if (_currentDiff >= _difficultyConfirmButtons.Length)
                        {
                            _currentDiff = 0;
                        }
                    }
                }

                for (int i = 0; i < _difficultyConfirmButtons.Length; i++)
                {
                    _difficultyConfirmButtons[i].Draw(i + 3, i == _currentDiff, menuTime.DeltaTime, false);
                }
            }
        }

        private static void GameInit()
        {
            INPUTS_SUPPRESSED = true;
            Console.Clear();
            Renderer.Initialize(_renderThread = new StoppableThread(new Thread(Renderer.Draw)));

            UpdateTime = new Time();
            UpdateTime.Initialize();

            RenderTime = new Time();
            RenderTime.Initialize();

            _renderFrame = -1;

            _updateThread = new StoppableThread(new Thread(Update));
            _updateThread.Start();
            _renderThread.Start();
         
            bool drawnTitle = false;
            while (true)
            {
                if (!drawnTitle & UpdateTime.FrameCount > 60)
                {
                    Renderer.WriteText(Extensions.GetCentered(TITLE), 0);
                    drawnTitle = true;
                }

                int val = _exitRequested;
                switch (_exitRequested)
                {
                    default:
                        break;
                    case 1:
                    case 2:
                        _updateThread.Stop();
                        _renderThread.Stop();
                        Unload();

                        while(_updateThread.ActuallyRunning | _renderThread.ActuallyRunning) { }
                        if (val >= 2)
                        {
                            Main(_skipInit);
                            return;
                        }
                        DifficultySelection();
                        return;
                }
            }
        }

        /// <summary>
        /// Used to suppress inputs to the console while player is using keys to shoot etc etc...
        /// </summary>
        private static void SuppressInputsLoop()
        {
            while (_inputThread.IsRunning)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }
            }
            _inputThread.ActuallyRunning = false;
        }

        /// <summary>
        /// Clears the game/rendering time(s), Despawns all entities and Collects GC(just in case)
        /// </summary>
        private static void Unload()
        {
            UpdateTime.Reset();
            RenderTime.Reset();

            EntityManager.Unload();
            GC.Collect();
        }

        public static void StopRun()
        {
            _exitRequested = 2;
        }

        /// <summary>
        /// The main update loop, has to wait for the renderer to be loaded/intialized, this runs the main game time, collisions, 
        /// inputs, entities and the level
        /// </summary>
        private static void Update()
        {
            while (!Renderer.LOADED) { }

            LevelManager.Load();
            while (_updateThread.IsRunning)
            {
                if (Input.GetKeyDown(KeyCode.One))
                {
                    SYNC_THREADS = !SYNC_THREADS;
                }

                if (SYNC_THREADS)
                {
                    if (_renderFrame == RenderTime.FrameCount) { continue; }
                    _renderFrame = RenderTime.FrameCount;
                }

                Input.Update();
                float delta = UpdateTime.DeltaTime;
                CollisionSystem.Update();

                _exitRequested = _exitRequested == 0 ? LevelManager.Update(delta) : _exitRequested;
                EntityManager.Update(delta);

                UpdateTime.Tick();
                if (UpdateTime.FrameCount % 30 == 0)
                {
                    Renderer.WriteText($"FPS{(SYNC_THREADS ? " (Synch)" : "")}: {UpdateTime.FrameRate}", 3);
                }

                if (Input.GetKeyDown(KeyCode.Zero))
                {
                    DEBUG = !DEBUG;
                }

                if (_exitRequested < 1 && Input.GetKeyDown(KeyCode.Esc))
                {
                    _exitRequested = 2;
                }
            }
            _updateThread.ActuallyRunning = false;
        }
    }
}
