using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AndreyTalanin0x00.Integrations.Blobs.Helpers;

internal static class ReadOnlyCollectionHelpers
{
    public static ReadOnlyCollection<T> CreateReadOnlyCollection<T>(IEnumerable<T> items)
    {
        IEnumerable<T> itemEnumerable = items;
        if (itemEnumerable is not IList<T> itemList)
            itemList = itemEnumerable.ToArray();

        return new ReadOnlyCollection<T>(itemList);
    }
}
