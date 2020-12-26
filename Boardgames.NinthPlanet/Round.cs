﻿using System;
using System.Collections.Generic;
using System.Linq;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.NinthPlanet
{
    internal class Round
    {
        public Dictionary<int, PlayerPrivateState> StateOfPlayersAtTable { get; set; }

        public List<TaskCard> AvailableGoals { get; set; }

        public List<int> PlayerOrder { get; set; }

        public int CurrentPlayerId { get; set; }

        public CardColor? ColorOfCurrentTrick { get; set; }

        public int CurrentCaptainPlayerId { get; set; }

        public Dictionary<int, Card> CurrentTrick { get; internal set; }

        public bool CanPlayCard(int playerId, Card card)
        {
            if (this.CurrentPlayerId != playerId || !this.StateOfPlayersAtTable.TryGetValue(playerId, out var state))
            {
                return false;
            }

            if (!state.CardsInHand.Contains(card))
            {
                return false;
            }

            if (ColorOfCurrentTrick.HasValue)
            {
                var trickColor = ColorOfCurrentTrick.Value;
                return trickColor == card.Color || state.CardsInHand.TrueForAll(x => x.Color != trickColor);
            }

            return true;
        }

        public void PlayCard(int playerId, Card card)
        {
            if(ColorOfCurrentTrick == null)
            {
                ColorOfCurrentTrick = card.Color;
            }

            var state = this.StateOfPlayersAtTable[playerId];
            state.CardsInHand.Remove(card);
            this.CurrentTrick.Add(playerId, card);
        }

        public bool TrickIsComplete()
        {
            return this.PlayerOrder.TrueForAll(x => this.CurrentTrick.ContainsKey(x));
        }

        public void MoveToNextPlayer()
        {
            var playerIndex = this.PlayerOrder.IndexOf(CurrentPlayerId);
            playerIndex = (playerIndex + 1) % this.PlayerOrder.Count;
            this.CurrentPlayerId = this.PlayerOrder[playerIndex];
        }

        public int GetTrickWinner()
        {
            var rockets = this.CurrentTrick.Values.Where(x => x.Color == CardColor.Rocket).ToList();
            if (rockets.Count > 0)
            {
                var highestRocketValue = rockets.Max(x => x.Value);
                return this.CurrentTrick.First(x => x.Value.Color == CardColor.Rocket && x.Value.Value == highestRocketValue).Key;
            }

            var coloredCards = this.CurrentTrick.Values.Where(x => x.Color == ColorOfCurrentTrick).ToList();
            if (coloredCards.Count > 0)
            {
                var highestColorValue = coloredCards.Max(x => x.Value);
                return this.CurrentTrick.First(x => x.Value.Color == ColorOfCurrentTrick && x.Value.Value == highestColorValue).Key;
            }

            throw new Exception();
        }

        public void GoToNextTrick(int trickWinnerId)
        {
            this.CurrentTrick.Clear();
            this.CurrentPlayerId = trickWinnerId;
            this.ColorOfCurrentTrick = null;
        }

        public List<TaskCard> GetFinishedTasks(int trickWinnerId)
        {
            throw new NotImplementedException();

            /*
            this.CurrentTrick.Values.Join(
                this.StateOfPlayersAtTable[trickWinnerId].TasksCards, 
                trick => trick.Value, 
                task => task.Card, 
                (trick, Card) => { });
            */
        }

        public List<TaskCard> GetFailedTasks(int trickWinnerId)
        {
            throw new NotImplementedException();
        }
    }
}