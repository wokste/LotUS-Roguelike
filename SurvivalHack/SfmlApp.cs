using System;
using SFML.Graphics;
using SFML.Window;

namespace SurvivalHack
{
    class SfmlApp
    {
        private readonly Game _game;
        private readonly SfmlGridRenderer _gridRenderer;
        private readonly RenderWindow _window;

        static void Main(string[] args)
        {
            Console.WriteLine("STUFF");
            var app = new SfmlApp();
            app.Run();
        }

        public SfmlApp()
        {
            var contextSettings = new ContextSettings
            {
                DepthBits = 24
            };
            
            _window = new RenderWindow(new VideoMode(640, 480), "SFML SurvivalHack", Styles.Default, contextSettings);
            _window.SetActive();

            _window.SetVisible(true);
            _window.SetVerticalSyncEnabled(true);

            _window.Closed += Window_Closed;

            _window.SetFramerateLimit(60);
            
            _game = new Game();
            _game.Init();

            _gridRenderer = new SfmlGridRenderer(_game.Grid);
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        public void Run()
        {
            while (_window.IsOpen())
            {
                // Dispatch events to work with native event loop
                _window.DispatchEvents();

                Update();
                Render();
            }
        }

        private void Update()
        {

        }

        private void Render()
        {
            _window.Clear();

            _gridRenderer.Draw(_window, new RenderStates());

            _window.Display();
        }
    }
}
