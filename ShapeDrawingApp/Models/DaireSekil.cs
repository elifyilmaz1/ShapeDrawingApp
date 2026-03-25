using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class DaireSekil : Sekil
{
    // Daire sekli, merkez koordinatlari (X, Y) ve capi tutar.
    public float X { get; set; }
    public float Y { get; set; }
    public float Cap { get; set; }

    public override Sekiller ShapeType => Sekiller.Daire;

    public override void Draw(Graphics g)
    {
        // Daire sekli, kenarlik rengi, kalinligi ve ic rengi kullanarak cizilir.
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
        // Daire sekli, noktanin dairenin icinde veya kenarinda olup olmadigini kontrol eder.
        var dikdortgen = GetBounds();
        if (dikdortgen.Width <= 0 || dikdortgen.Height <= 0)
            return false;

        using var yol = new GraphicsPath();
        yol.AddEllipse(dikdortgen);

        if (İcRengi.A > 0 && yol.IsVisible(nokta))
            return true;

        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds() => new RectangleF(X, Y, Cap, Cap);

    public override void MoveBy(float dx, float dy)
    {
        // Daire sekli, merkez koordinatlarini dx ve dy kadar kaydirir.
        X += dx;
        Y += dy;
    }
}

