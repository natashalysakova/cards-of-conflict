using System.Net.Sockets;
using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{
    public class NormalGame : IDisposable
    {
        private readonly MessageManager messageManager = new(new TcpClient());

        public void Dispose()
        {
            messageManager.Dispose();
        }

        public int JoinTheGame(string ipAddress, int port)
        {
            while (true)
            {
                try
                {
                    messageManager.Client.Connect(ipAddress, port);
                    Console.WriteLine("Connected");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot connect to {ipAddress}");
                    Console.WriteLine(ex.Message);
                    return -1;
                }

                return GameLoop();
            }



        }

        private int GameLoop()
        {

            var isGameActive = true;
            var cards = new List<WhiteCard>();

            while (isGameActive)
            {
                Thread.Sleep(200);

                var nexMessage = messageManager.GetNextMessage();
                switch (nexMessage.Type)
                {
                    case MessageType.SendCards:
                        cards.AddRange(nexMessage.Attachment);

                        break;
                    case MessageType.GetCards:

                        var number = nexMessage.CardNumber;
                        Console.WriteLine($"Choose {number} answers");
                        for (int i = 0; i < number; i++)
                        {
                            int id;
                            while (true)
                            {
                                Console.WriteLine($"Select answer #{i + 1}:");
                                if (int.TryParse(Console.ReadLine(), out id))
                                {
                                    break;
                                }
                            }


                            
                            cards.RemoveAt(id - 1);
                            messageManager.SendCards(cards[id - 1]);
                        }

                        break;
                    case MessageType.SendMessage:
                        var message = nexMessage.Text;
                        Console.WriteLine(message);
                        break;
                    case MessageType.GetMessage:
                        messageManager.SendTextMessage("??");
                        break;
                    case MessageType.Winner:
                        int winner;
                        while (true)
                        {
                            var answersNumber = nexMessage.Attachment;
                            Console.WriteLine("Select a winner:");
                            if (int.TryParse(Console.ReadLine(), out winner))
                            {
                                if (winner > answersNumber || winner < 1)
                                {
                                    Console.WriteLine("Wrong answer");
                                }
                                else
                                {
                                    break;
                                }
                            }

                        }

                        messageManager.SendWinner(winner);

                        break;
                    case MessageType.RequestName:
#if DEBUG
                        var myPlayerName = "player";
#else
                    Console.WriteLine("Enter your name: ");
                    var myPlayerName = Console.ReadLine();
#endif
                        messageManager.SendName(myPlayerName);
                        break;
                    case MessageType.SendName:
                        break;
                    case MessageType.none:
                        //Console.WriteLine("Health Check done");
                        break;
                    case MessageType.NewRound:
                        Console.Clear();
                        Console.WriteLine($"====== Round {nexMessage.Attachment} ======");
                        Console.WriteLine("My Cards");
                        for (int i = 0; i < cards.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {cards[i]}");
                        }
                        break;
                    case MessageType.GameOver:
                        isGameActive = false;
                        break;
                    default:
                        break;
                }
            }
            return 0;
        }
    }
}