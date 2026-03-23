using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class DikdortgenSekil : Sekil
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Genislik { get; set; }
    public float Yukseklik { get; set; }

    public override Sekiller ShapeType => Sekiller.Dikdortgen;

    public override void Draw(Graphics g)
    {
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
        {
            return;
        }

        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        if (İcRengi.A > 0)
        {
            using var firca = new SolidBrush(İcRengi);
            g.FillRectangle(firca, dikdortgen);
        }

        g.DrawRectangle(kalem, dikdortgen.X, dikdortgen.Y, dikdortgen.Width, dikdortgen.Height);
    }

    public override bool HitTest(PointF nokta)
    {
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return false;

        using var path = new GraphicsPath();
        path.AddRectangle(dikdortgen);

        if (İcRengi.A > 0 && path.IsVisible(nokta))
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

