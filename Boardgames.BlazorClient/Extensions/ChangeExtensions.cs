using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Boardgames.Common.Observables;

namespace Boardgames.BlazorClient.Extensions
{
    public static class ChangeExtensions
    {
        public static void UpdateOnCollectionChanged<T>(this IItemsInCollectionChanged<T> collectionChanged, Action stateChanged)
        {
            WeakReference<Action> reference = new WeakReference<Action>(stateChanged);
            collectionChanged.ItemsInCollectionChanged += (sender, args) =>
            {
                if (reference.TryGetTarget(out var action))
                {
                    action();
                }
            };
        }

        public static void UpdateOnCollectionChanged(this INotifyCollectionChanged collectionChanged, Action stateChanged)
        {
            WeakReference<Action> reference = new WeakReference<Action>(stateChanged);
            collectionChanged.CollectionChanged += (sender, args) =>
            {
                if (reference.TryGetTarget(out var action))
                {
                    action();
                }
            };
        }

        public static void UpdateOnPropertyChanged(this INotifyPropertyChanged propertyChanged, Action stateChanged)
        {
            WeakReference<Action> reference = new WeakReference<Action>(stateChanged);
            propertyChanged.PropertyChanged += (sender, args) =>
            {
                if (reference.TryGetTarget(out var action))
                {
                    action();
                }
            };
        }
    }
}