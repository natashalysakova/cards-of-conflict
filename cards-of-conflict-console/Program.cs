using CardsOfConflict.Library.Game;

namespace CardsOfConflictGame
{
    public class CardsOfConflictGame
    {
        static bool isExit = false;

        public static void Main(string[] args)
        {
            Console.WriteLine("HelloWorld");
            while (!isExit)
            {
                Console.WriteLine("Game Menu :");
                Console.WriteLine("1. Host new game");
                Console.WriteLine("2. Join game");
                Console.WriteLine("3. Exit");

                //Console.WriteLine("4. Export");

                Console.WriteLine("Select: ");
                var selected = Console.ReadKey();

                switch (selected.Key)
                {
                    case ConsoleKey.D1:
                        new Game().HostNewGame();
                        break;
                    case ConsoleKey.D2:
                        var game = new Game();
                        _ = game.JoinTheGame();
                        game.Dispose();
                        break;
                    case ConsoleKey.D3:
                        isExit = true;
                        break;
                    //case ConsoleKey.D4:
                    //new Game().ReadFromFile();
                    //break;
                    default:
                        break;
                }

            }

            Console.WriteLine("Thank you for playing Cards of Conflict");
        }
    }
}