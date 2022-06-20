using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

public class MessageManager : IDisposable
{
    const string SEPARATOR = "&*";
    Queue<Message> Messages;
    Queue<Message> ToSend;
    object locker = new object();
    CancellationTokenSource cancellationTokenSource;

    public MessageManager(TcpClient client)
    {
        Client = client;
        Messages = new Queue<Message>();
        ToSend = new Queue<Message>();
        cancellationTokenSource = new CancellationTokenSource();
        Task.Run(MonitorMessages, cancellationTokenSource.Token);
        Task.Run(SendingAgent, cancellationTokenSource.Token);
    }

    public TcpClient Client { get; }

    public Message GetNextMessage()
    {
        Message message;
        while (!Messages.TryDequeue(out message))
        {
            // waiting for next message
            Thread.Sleep(200);

        }
        return message;

    }

    private void SendingAgent()
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            Thread.Sleep(200);
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
            Thread.Sleep(200);

            if (!Client.Connected)
            {
                //Console.WriteLine("Socket not connected");
                break;
            }

            NetworkStream stream = Client.GetStream();
            while (Client.Available < 4)
            {
                //waiting data
                Thread.Sleep(100);
            }

            Byte[] bytes = new byte[Client.Available];
            stream.Read(bytes, 0, bytes.Length);

            var message = ByteArrayToObject<Message>(bytes);
            lock (locker)
            {
                //Console.WriteLine("message added to the queue:" + message.Type);
                Messages.Enqueue(message);
            }
        }
    }

    internal void SendTextMessage(string text)
    {
        var message = new Message(MessageType.SendMessage);
        message.Text = text;
        SendMessage(message);
    }

    internal void SendMessage(Message message)
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
            return null;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
    static T ByteArrayToObject<T>(byte[] obj)
    {
        if (obj.Length == 0)
            return default;
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream(obj))
        {
            return (T)bf.Deserialize(ms);
        }
    }

    internal void SendName(string myPlayerName)
    {
        var message = new Message(MessageType.SendName);
        message.Text = myPlayerName;
        SendMessage(message);
    }

    public void Dispose()
    {
        cancellationTokenSource.Cancel();
    }
}

[Serializable]
public class Message
{
    public Message(MessageType type)
    {
        Type = type;
    }
    public MessageType Type { get; set; }
    public string Text { get; set; }
    public dynamic Attachment { get; set; }
    public int CardNumber { get; set; }

}