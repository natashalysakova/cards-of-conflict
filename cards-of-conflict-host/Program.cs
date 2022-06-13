public class CardsOfConflict
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

            Console.WriteLine("Select: ");
            var selected = Console.ReadKey();

            switch (selected.Key)
            {
                case ConsoleKey.D1:
                    new Game().HostNewGame();
                    break;
                case ConsoleKey.D2:
                    new Game().JoinTheGame();
                    break;
                case ConsoleKey.D3:
                    isExit = true;
                    break;
                default:
                    break;
            }

        }

        Console.WriteLine("Thank you for playing Cards of Conflict");
    }
}