using System.Windows.Media;
using System.Windows.Shapes;

namespace Backgammon.Graphics
{
    class CheckerGraphics
    {
        public Ellipse checker {get;}

        public CheckerGraphics(int size, Color color)
        {
            // Create checker
            checker = new Ellipse();
            checker.Height = size;
            checker.MaxWidth = size;

            // Border
            SolidColorBrush borderBrush = new SolidColorBrush();
            borderBrush.Color = Colors.DarkGray;
            checker.StrokeThickness = 1;
            checker.Stroke = borderBrush;
            
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = color;
            checker.Fill = brush;
        }
    }
}
