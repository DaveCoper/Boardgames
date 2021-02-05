using System;

namespace Boardgames.Common.Observables
{
    public interface IItemsInCollectionChanged<T>
    {
        event EventHandler<ItemsInCollectionChangedEventArgs<T>> ItemsInCollectionChanged;
    }
}