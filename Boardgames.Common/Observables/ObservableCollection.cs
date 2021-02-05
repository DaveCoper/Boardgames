using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Boardgames.Common.Observables
{
    public abstract class ObservableCollection<T> :
        ObservableObject,
        ICollection<T>,
        IEnumerable<T>,
        IEnumerable,
        INotifyCollectionChanged,
        IItemsInCollectionChanged<T>
    {
        public event EventHandler<ItemsInCollectionChangedEventArgs<T>> ItemsInCollectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public abstract int Count { get; }

        public virtual bool IsReadOnly => false;

        public abstract IEnumerator<T> GetEnumerator();

        public abstract void Add(T item);

        public abstract void Clear();

        public abstract bool Contains(T item);

        public abstract void CopyTo(T[] array, int arrayIndex);

        public abstract bool Remove(T item);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected void OnItemAdded(T item)
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    item));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    new List<T> { item },
                    null));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemAdded(T item, int index)
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    item,
                    index));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    new List<T> { item },
                    null));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemsAdded(IEnumerable<T> items)
        {
            var itemList = items as List<T> ?? items.ToList();

            // Hack for WPF. Adding or removing multiple elements will crash UI.
            foreach (var itm in itemList)
            {
                this.CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add,
                        itm));
            }

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    itemList,
                    null));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnCollectionCleared(IEnumerable<T> removedItems)
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    null,
                    removedItems.ToList()));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemRemoved(T item)
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    new List<T> { item },
                    null));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemRemoved(T item, int index)
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item,
                    index));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    null,
                    new List<T> { item }));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemsRemoved(IEnumerable<T> items)
        {
            var itemList = items as List<T> ?? items.ToList();

            // Hack for WPF. Adding or removing multiple elements will crash UI.
            foreach (var itm in itemList)
            {
                this.CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        itm));
            }

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    null,
                    itemList));

            RaisePropertyChanged(nameof(this.Count));
        }

        protected void OnItemChanged(T oldItem, T newItem)
        {
            this.CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        oldItem,
                        newItem));

            this.ItemsInCollectionChanged?.Invoke(
                this,
                new ItemsInCollectionChangedEventArgs<T>(
                    new List<T> { newItem },
                    new List<T> { oldItem }));
        }
    }
}