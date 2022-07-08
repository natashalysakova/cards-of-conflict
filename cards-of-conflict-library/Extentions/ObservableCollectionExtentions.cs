using CardsOfConflict.Library.Interfaces;
using CardsOfConflict.Library.Model;
using System.Collections.ObjectModel;

namespace CardsOfConflict.Library.Extentions;

internal static class ObservableCollectionExtentions
{
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values) where T : ICard
    {
        foreach (var item in values)
        {
            collection.Add(item);
        }
    }
    public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> func) where T : ICard
    {
        foreach (var item in collection)
        {
            if (func.Invoke(item))
            {
                collection.Remove(item);
            }
        }
    }
}
