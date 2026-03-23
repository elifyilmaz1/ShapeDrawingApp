using System.Windows.Forms;

namespace ShapeDrawingApp.UI;

public class DoubleBufferedPanel : Panel
{
    public DoubleBufferedPanel()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }
}

