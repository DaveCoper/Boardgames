using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Boardgames.NinthPlanet.Models;

namespace Boardgames.WpfClient.Converters
{
    public class NinthPlanetCardPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Card card)
            {
                int cardIndex = 0;
                switch (card.Color)
                {
                    case CardColor.Rocket:
                        break;

                    case CardColor.Blue:
                        cardIndex = 4;
                        break;

                    case CardColor.Green:
                        cardIndex = 4 + 9;
                        break;

                    case CardColor.Pink:
                        cardIndex = 4 + 9 * 2;
                        break;

                    case CardColor.Yellow:
                        cardIndex = 4 + 9 * 3;
                        break;
                }

                cardIndex += card.Value - 1;
                var row = cardIndex / 10;
                var column = cardIndex % 10;

                return new Int32Rect(column * 90, Math.Max(0, (row * 140) - 1), 89, 139);
            }

            return new Int32Rect();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}