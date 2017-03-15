using System.Windows.Media;
using System.Windows.Shapes;

namespace Backgammon.Graphics
{
    class CheckerGoalGraphics
    {
        public Rectangle checker {get;}

        public CheckerGoalGraphics(int size, Color color)
        {
            // Create checker
            checker = new Rectangle();
            checker.Height = size/3;
            checker.Width = size;

            // Border
            SolidColorBrush borderBrush = new SolidColorBrush();
            borderBrush.Color = Colors.DarkGray;
            checker.StrokeThickness = 1;
            checker.Stroke = borderBrush;

            // Fill
            Color color2 = (Color)ColorConverter.ConvertFromString("Gray");
            LinearGradientBrush brush = new LinearGradientBrush(color, color2, 0);
            checker.Fill = brush;
        }
    }
}
