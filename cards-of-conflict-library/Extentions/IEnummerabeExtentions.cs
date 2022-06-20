using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

static class IEnummerabeExtentions
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
}