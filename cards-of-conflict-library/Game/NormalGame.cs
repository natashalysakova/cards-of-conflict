using System.Net.Sockets;
using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Model;

namespace CardsOfConflict.Library.Game
{
    public class NormalGame : IDisposable
    {
        MessageManager messageManager;

        public NormalGame()
        {
            messageManager = new MessageManager(new TcpClient());
        }

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

        int GameLoop()
        {

            var isGameActive = true;
            var cards = new List<WhiteCard>();

            while (isGameActive)
            {
                Thread.Sleep(100);

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
                                    break;
                            }


                            var response = new Message(MessageType.SendCards);
                            response.Attachment = cards[id - 1];
                            cards.RemoveAt(id - 1);
                            messageManager.SendMessage(response);
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


                        var responseWinner = new Message(MessageType.Winner);
                        responseWinner.Attachment = winner;
                        messageManager.SendMessage(responseWinner);

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