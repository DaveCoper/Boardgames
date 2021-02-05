using System;
using System.Collections.Generic;
using System.Linq;

namespace Boardgames.Common.Observables
{
    public class ObservableList<T> : ObservableCollection<T>, IList<T>
    {
        private List<T> innerList;

        public ObservableList()
        {
        }

        public ObservableList(List<T> items)
        {
            innerList = new List<T>(items);
        }

        public ObservableList(IEnumerable<T> items)
        {
            innerList = new List<T>(items);
        }

        public override int Count => innerList.Count;

        public T this[int index]
        {
            get => innerList[index];
            set
            {
                var changedItem = innerList[index];
                innerList[index] = value;
                OnItemChanged(changedItem, value);
            }
        }

        public override void Add(T item)
        {
            this.innerList.Add(item);
            OnItemAdded(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            var addedItems = items as List<T> ?? items.ToList();
            this.innerList.AddRange(addedItems);
            OnItemsAdded(addedItems);
        }

        public override bool Remove(T item)
        {
            if (this.innerList.Remove(item))
            {
                OnItemRemoved(item);
                return true;
            }

            return false;
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            var removedItems = new List<T>();
            foreach (var item in items)
            {
                if (this.innerList.Remove(item))
                {
                    removedItems.Add(item);
                }
            }

            OnItemsRemoved(removedItems);
        }

        public void RemoveAll(Predicate<T> filter)
        {
            var removedItems = this.innerList.Where(x => filter(x)).ToList();
            this.innerList.RemoveAll(filter);
            OnItemsRemoved(removedItems);
        }

        public override void Clear()
        {
            var removedItems = this.innerList.ToList();
            this.innerList.Clear();
            OnCollectionCleared(removedItems);
        }

        public override bool Contains(T item)
        {
            return this.innerList.Contains(item);
        }

        public override void CopyTo(T[] array, int arrayIndex)
        {
            array.CopyTo(array, arrayIndex);
        }

        public override IEnumerator<T> GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            innerList.Insert(index, item);
            OnItemAdded(item, index);
        }

        public void RemoveAt(int index)
        {
            var item = this[index];
            innerList.RemoveAt(index);
            OnItemRemoved(item, index);
        }
    }
}