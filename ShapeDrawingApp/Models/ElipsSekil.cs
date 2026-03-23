using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class ElipsSekil : Sekil
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Genislik { get; set; }
    public float Yukseklik { get; set; }

    public override Sekiller ShapeType => Sekiller.Elips;

    public override void Draw(Graphics g)
    {
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return;

        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        if (İcRengi.A > 0)
        {
            using var firca = new SolidBrush(İcRengi);
            g.FillEllipse(firca, dikdortgen);
        }

        g.DrawEllipse(kalem, dikdortgen);
    }

    public override bool HitTest(PointF nokta)
    {
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return false;

        using var path = new GraphicsPath();
        path.AddEllipse(dikdortgen);

        if (İcRengi .A > 0 && path.IsVisible(nokta))
            return true;

        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return path.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds() => new RectangleF(X, Y, Genislik, Yukseklik);

    public override void MoveBy(float dx, float dy)
    {
        X += dx;
        Y += dy;
    }
}

