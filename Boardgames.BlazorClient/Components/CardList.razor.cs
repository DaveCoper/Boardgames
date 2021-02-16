using System;
using System.Collections.Generic;
using Boardgames.BlazorClient.Services;
using Boardgames.Common.Observables;
using Microsoft.AspNetCore.Components;

namespace Boardgames.BlazorClient.Components
{
    public partial class CardList<TCardType> : ComponentBase, IDisposable
    {
        private IList<TCardType> cards;

        private TCardType selectedCard;

        [Parameter]
        public double CardSpace { get; set; }

        [Parameter]
        public double CardWidth { get; set; }

        [Parameter]
        public double CardHeight { get; set; }

        [Parameter]
        public bool CardsCanBeDragged { get; set; } = false;

        [Parameter]
        public IList<TCardType> Cards
        {
            get => cards;
            set
            {
                if (cards != value)
                {
                    if (cards is IItemsInCollectionChanged<TCardType> oldCollection)
                    {
                        oldCollection.ItemsInCollectionChanged -= OnCardsChanged;
                    }

                    cards = value;

                    if (value is IItemsInCollectionChanged<TCardType> newCollection)
                    {
                        newCollection.ItemsInCollectionChanged += OnCardsChanged;
                    }
                }
            }
        }

        [Parameter]
        public TCardType SelectedCard
        {
            get => selectedCard;
            set => selectedCard = value;
        }

        [Parameter]
        public RenderFragment<TCardType> ChildContent { get; set; }

        [Inject]
        public DragDropDataStore DragDropDataStore { get; set; }

        void IDisposable.Dispose()
        {
            this.Cards = null;
        }

        protected void OnDragStart(TCardType card)
        {
            DragDropDataStore.DragDropData = card;
        }

        protected void CardCliecked(TCardType card)
        {
            this.SelectedCard = card;
        }

        protected string ToPx(double value)
        {
            return $"{value}px";
        }

        private void OnCardsChanged(object sender, ItemsInCollectionChangedEventArgs<TCardType> e)
        {
            this.StateHasChanged();
        }
    }
}