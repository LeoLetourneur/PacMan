using System;

namespace PacManXNAGames
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GamePacMan game = new GamePacMan())
            {
                game.Run();
            }
        }
    }
#endif
}

