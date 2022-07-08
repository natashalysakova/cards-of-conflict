using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Extentions;
using CardsOfConflict.Library.Interfaces;
using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using static CardsOfConflict.Library.Game.Game;

namespace CardsOfConflict.Library.Game
{
    public class NormalGame : IDisposable
    {
        private readonly MessageManager messageManager = new(new TcpClient());

        public delegate IEnumerable<int> RequestAnswersDelegate(int numberOfAnswers);
        public delegate void MessageRecivedDelegate(string message);
        public delegate int SelectWinnerDelegate(int numberOfPlayers);
        public delegate void NextRoundDelegate(int round, IEnumerable<WhiteCard> cards);
        public delegate void GameOverDelegate();

        public event RequestAnswersDelegate RequestAnswers;
        public event MessageRecivedDelegate MessageRecived;
        public event SelectWinnerDelegate SelectWinner;
        public event NextRoundDelegate NextRound;
        public event GameOverDelegate GameOver;
        public event GameStartedDelegate GameStarted;
        public void Dispose()
        {
            messageManager.Dispose();
        }

        public int JoinTheGame(string ipAddress, int port)
        {
#if DEBUG
            var myPlayerName = "player";
#else
                    Console.WriteLine("Enter your name: ");
                    var myPlayerName = Console.ReadLine();
#endif

            return JoinTheGame(ipAddress, port, new HostPlayer(myPlayerName) , new CancellationTokenSource()); ;
        }
        public int JoinTheGame(string ipAddress, int port, IPlayer player,  CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    messageManager.Client.Connect(ipAddress, port);
                    Console.WriteLine("Connected");
                    return GameLoop(player, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot connect to {ipAddress}");
                    Console.WriteLine(ex.Message);
                    return -1;
                }
            }
            return -1;
        }

        private int GameLoop(IPlayer player, CancellationTokenSource token)
        {

            //var isGameActive = true;
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(200);

                var nexMessage = messageManager.GetNextMessage();
                switch (nexMessage.Type)
                {
                    case MessageType.SendCards:
                        player.Cards.AddRange(nexMessage.Attachment as IEnumerable<WhiteCard>);

                        break;
                    case MessageType.GetCards:

                        var number = nexMessage.CardNumber;

                        var ids = RequestAnswers?.Invoke(number);

                        foreach (var id in ids)
                        {
                            player.Cards.RemoveAt(id);
                            messageManager.SendCards(player.Cards[id]);
                        }

                        break;
                    case MessageType.SendMessage:
                        var message = nexMessage.Text;
                        MessageRecived?.Invoke(message);
                        break;
                    case MessageType.GetMessage:
                        messageManager.SendTextMessage("??");
                        break;
                    case MessageType.Winner:                      
                        var winner = SelectWinner?.Invoke(nexMessage.Attachment);
                        messageManager.SendWinner(winner);

                        break;
                    case MessageType.RequestName:
                        messageManager.SendName(player.Name);
                        break;
                    case MessageType.SendName:
                        break;
                    case MessageType.none:
                        //Console.WriteLine("Health Check done");
                        break;
                    case MessageType.NewRound:
                        NextRound?.Invoke(nexMessage.Attachment, player.Cards);
                        break;
                    case MessageType.GameOver:
                        GameOver?.Invoke();
                        token.Cancel();
                        break;
                    case MessageType.GameStarted:
                        var players = nexMessage.Attachment;
                        GameStarted?.Invoke(players);
                        break;
                    default:
                        break;
                }
            }
            return 0;
        }
    }
}