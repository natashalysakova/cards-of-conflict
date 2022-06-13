using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

static class Extentions
{
    public static Stack<T> ShuffleIntoStack<T>(this IEnumerable<T> collection)
    {
        var r = new Random();
        var stack = new Stack<T>();
        foreach (var item in collection.OrderBy(x => r.Next()))
        {
            stack.Push(item);
        }
        return stack;
    }
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
    {
        var r = new Random();
        foreach (var item in collection.OrderBy(x => r.Next()))
        {
            yield return item;
        }

    }

    //public static void SendMessage(this TcpClient client, string message)
    //{
    //    var data = Encoding.UTF8.GetBytes(message);
    //    client.GetStream().Write(data, 0, data.Length);
    //}
    //public static void SendMessageType(this TcpClient client, MessageType type)
    //{
    //    var str = ((int)type);
    //    var data = BitConverter.GetBytes(str);
    //    client.GetStream().Write(data, 0, data.Length);
    //}
    //public static MessageType GetMessageType(this TcpClient client)
    //{
    //    const int messageLength = 4;
    //    try
    //    {
    //        NetworkStream stream = client.GetStream();

    //        while (client.Available < messageLength)
    //        {
    //            // wait for enough bytes to be available
    //        }

    //        Byte[] bytes = new Byte[messageLength];

    //        stream.Read(bytes, 0, messageLength);

    //        //translate bytes of request to string
    //        return (MessageType)Enum.Parse(typeof(MessageType), Encoding.UTF8.GetString(bytes));
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine(ex.Message);
    //        return MessageType.none;
    //    }
        
    //}

    //public static void SendCards(this TcpClient client, IEnumerable<ICard> cards)
    //{
    //    var data = ObjectToByteArray(cards);
    //    client.GetStream().Write(data, 0, data.Length);
    //}

    //public static IEnumerable<WhiteCard> GetCards(this TcpClient client)
    //{
    //    NetworkStream stream = client.GetStream();

    //    while (client.Available < 3)
    //    {
    //        // wait for enough bytes to be available
    //    }

    //    Byte[] bytes = new Byte[client.Available];

    //    stream.Read(bytes, 0, bytes.Length);

    //    //translate bytes of request to string
    //    return ByteArrayToObject<IEnumerable<WhiteCard>>(bytes);
    //}

    //public static string GetMessage(this TcpClient client)
    //{
    //    NetworkStream stream = client.GetStream();

    //    while (client.Available < 3)
    //    {
    //        // wait for enough bytes to be available
    //    }

    //    Byte[] bytes = new Byte[client.Available];

    //    stream.Read(bytes, 0, bytes.Length);

    //    //translate bytes of request to string
    //    return Encoding.UTF8.GetString(bytes);
    //}

    
}