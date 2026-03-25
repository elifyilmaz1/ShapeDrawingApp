using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class KareSekil : Sekil
{
    // Kare sekli, sol ust kose koordinatlari (X, Y) ve boyutu tutar.
    public float X { get; set; }
    public float Y { get; set; }
    public float Boyut { get; set; }

    public override Sekiller ShapeType => Sekiller.Kare;

    public override void Draw(Graphics g)
    {
        //  Kare sekli, kenarlik rengi, kalinligi ve ic rengi kullanarak cizilir.
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return;

        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        if (İcRengi .A > 0)
        {
            using var firca = new SolidBrush(İcRengi);
            g.FillRectangle(firca, dikdortgen);
        }

        g.DrawRectangle(kalem, dikdortgen.X, dikdortgen.Y, dikdortgen.Width, dikdortgen.Height);
    }

    public override bool HitTest(PointF nokta)
    {
        //  Kare sekli, noktanin karenin icinde veya kenarinda olup olmadigini kontrol eder.
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return false;

        using var yol = new GraphicsPath();
        yol.AddRectangle(dikdortgen);

        if (İcRengi.A > 0 && yol.IsVisible(nokta))
            return true;

        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds() => new RectangleF(X, Y, Boyut, Boyut);

    public override void MoveBy(float dx, float dy)
    {
        // Kare sekli, sol ust kose koordinatlarini dx ve dy kadar kaydirir.
        X += dx;
        Y += dy;
    }
}

