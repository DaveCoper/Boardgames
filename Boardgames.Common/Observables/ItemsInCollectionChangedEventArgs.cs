using System;
using System.Collections.Generic;

namespace Boardgames.Common.Observables
{
    public class ItemsInCollectionChangedEventArgs<T> : EventArgs
    {
        public ItemsInCollectionChangedEventArgs(List<T> addedItems, List<T> removedItems)
        {
            AddedItems = addedItems ?? new List<T>();
            RemovedItems = removedItems ?? new List<T>();
        }

        public List<T> AddedItems { get; }

        public List<T> RemovedItems { get; }
    }
}