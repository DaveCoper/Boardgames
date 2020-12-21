using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Boardgames.WpfClient.Panels
{
    public class CardStackPanel : Panel
    {
        public static readonly DependencyProperty CardOffsetProperty = DependencyProperty.Register(
            nameof(CardOffset),
            typeof(double),
            typeof(CardStackPanel),
            new PropertyMetadata(10.0));

        public double CardOffset
        {
            get => (double)GetValue(CardOffsetProperty);
            set => SetValue(CardOffsetProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = Size.Empty;
            double cardOffset = CardOffset;
            double offset = 0.0;

            foreach (var itm in this.Children.OfType<FrameworkElement>())
            {
                itm.Measure(availableSize);
                desiredSize = new Size(
                    Math.Max(desiredSize.Width, itm.DesiredSize.Width + offset),
                    Math.Max(desiredSize.Height, itm.DesiredSize.Height));

                offset += cardOffset;
            }

            return desiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size desiredSize = Size.Empty;
            double cardOffset = CardOffset;
            double offset = 0.0;

            foreach (var itm in this.Children.OfType<FrameworkElement>())
            {
                var itmDesiredSize = itm.DesiredSize;
                itm.Arrange(new Rect(offset, 0, itmDesiredSize.Width, itmDesiredSize.Height));
                desiredSize = new Size(
                    Math.Max(desiredSize.Width, itmDesiredSize.Width + offset),
                    Math.Max(desiredSize.Height, itmDesiredSize.Height));

                offset += cardOffset;
            }

            return new Size(Math.Max(1, desiredSize.Width), Math.Max(1, desiredSize.Height));
        }
    }
}