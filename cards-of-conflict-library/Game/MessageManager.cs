using CardsOfConflict.Library.Enums;
using CardsOfConflict.Library.Model;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardsOfConflict.Library.Game
{
    public class MessageManager : IDisposable
    {
        const int delay = 50;
        readonly Queue<Message> Messages = new();
        readonly Queue<Message> ToSend = new();
        readonly object locker = new();
        readonly CancellationTokenSource cancellationTokenSource = new();

        public MessageManager(TcpClient client)
        {
            Client = client;
            Task.Run(MonitorMessages, cancellationTokenSource.Token);
            Task.Run(SendingAgent, cancellationTokenSource.Token);
        }

        internal void RequestCards(int answersNumber)
        {
            var message = new Message(MessageType.GetCards)
            {
                CardNumber = answersNumber
            };
            SendMessage(message);
        }

        public TcpClient Client { get; }

        public Message GetNextMessage()
        {
            Message? message;
            while (!Messages.TryDequeue(out message))
            {
                // waiting for next message
                Thread.Sleep(delay);
            }
            return message;

        }

        internal void SendWinner(int winnerAnswer)
        {
            var message = new Message(MessageType.Winner)
            {
                Attachment = winnerAnswer
            };
            SendMessage(message);
        }

        internal void GameOver()
        {
            var message = new Message(MessageType.GameOver);
            SendMessage(message);
        }

        internal void NewRound(int round)
        {
            var message = new Message(MessageType.NewRound)
            {
                Attachment = round
            };
            SendMessage(message);
        }

        private void SendingAgent()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Thread.Sleep(delay);
                if (!Client.Connected)
                {
                    //Console.WriteLine("Socket not connected");
                    break;
                }

                lock (locker)
                {
                    if (ToSend.TryDequeue(out var message))
                    {
                        var data = ObjectToByteArray(message);
                        Client.GetStream().Write(data, 0, data.Length);
                    }
                }


            }
        }

        internal void HealthCheck()
        {
            var message = new Message(MessageType.none);
            lock (locker)
            {
                var data = ObjectToByteArray(message);
                Client.GetStream().Write(data, 0, data.Length);
            }
        }

        private void MonitorMessages()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Thread.Sleep(delay);

                if (!Client.Connected)
                {
                    //Console.WriteLine("Socket not connected");
                    break;
                }

                NetworkStream stream = Client.GetStream();
                while (Client.Available < 4)
                {
                    //waiting data
                    Thread.Sleep(50);
                }

                Byte[] bytes = new byte[Client.Available];
                stream.Read(bytes, 0, bytes.Length);

                var message = ByteArrayToObject<Message>(bytes);
                if (message != null)
                {
                    lock (locker)
                    {
                        Messages.Enqueue(message);
                    }
                }
            }
        }

        internal void SendCards(dynamic cards)
        {
            var message = new Message(MessageType.SendCards)
            {
                Attachment = cards
            };

            SendMessage(message);
        }

        internal void SendTextMessage(string text)
        {
            var message = new Message(MessageType.SendMessage)
            {
                Text = text
            };
            SendMessage(message);
        }

        private void SendMessage(Message message)
        {
            lock (locker)
            {
                ToSend.Enqueue(message);
            }
        }

        internal void RequestName()
        {
            var message = new Message(MessageType.RequestName);
            SendMessage(message);
        }

        static byte[] ObjectToByteArray<T>(T obj)
        {
            if (obj == null)
                throw new NullReferenceException("Object obj cannot be null");
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            bf.Serialize(ms, obj);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            return ms.ToArray();

        }
        static T? ByteArrayToObject<T>(byte[] obj)
        {
            if (obj.Length == 0)
                return default;
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream(obj);
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            return (T)bf.Deserialize(ms);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
        }

        internal void SendName(string myPlayerName)
        {
            var message = new Message(MessageType.SendName)
            {
                Text = myPlayerName
            };
            SendMessage(message);
        }

        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }
    }
}