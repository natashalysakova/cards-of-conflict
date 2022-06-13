using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

public class MessageManager
{
    const string SEPARATOR = "&*";
    Queue<Message> Messages;
    object locker = new object();

    public MessageManager(TcpClient client)
    {
        Client = client;
        Messages = new Queue<Message>();
        Task.Run(MonitorMessages);
    }

    public TcpClient Client { get; }

    public Message GetNextMessage()
    {
        while (Messages.Count == 0)
        {
            // waiting for next message
            Thread.Sleep(100);
        }

        lock (locker)
        {
            return Messages.Dequeue();
        }
    }

    private void MonitorMessages()
    {
        while (true)
        {
            Thread.Sleep(200);

            if (!Client.Connected)
                throw new Exception("Socket not connected");

            NetworkStream stream = Client.GetStream();
            while (Client.Available < 4)
            {
                //waiting data
                Thread.Sleep(100);
            }

            Byte[] bytes = new byte[Client.Available];
            stream.Read(bytes, 0, bytes.Length);

            var messages = ByteArrayToObject<IEnumerable<Message>>(bytes);
            lock (locker)
            {
                foreach (var message in messages)
                {
                    Messages.Enqueue(message);
                }

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
        SendMessages(new List<Message>() { message });
    }
    private void SendMessages(IEnumerable<Message> messages)
    {
        var data = ObjectToByteArray(messages);
        Client.GetStream().Write(data, 0, data.Length);
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

}