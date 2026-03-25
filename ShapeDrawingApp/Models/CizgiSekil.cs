using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class CizgiSekil : Sekil
{
    // Cizgi sekli, baslangic ve bitis noktalarini tutar.
    public PointF Baslangic { get; set; }
    public PointF Bitis { get; set; }

    public override Sekiller ShapeType => Sekiller.Cizgi;

    public override void Draw(Graphics g)
    {
        // Cizgi sekli, sadece kenarlik rengi ve kalinligi kullanarak cizilir.
        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        g.DrawLine(kalem, Baslangic, Bitis);
    }

    public override bool HitTest(PointF nokta)
    {
        // Cizgi sekli, noktanin cizgiye yakin olup olmadigini kontrol eder.
        using var yol = new GraphicsPath();
        yol.AddLine(Baslangic, Bitis);
        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds()
    {
        // Cizgi sekli, baslangic ve bitis noktalarinin kapsayan bir dikdortgen cizer.
        float minX = MathF.Min(Baslangic.X, Bitis.X);
        float minY = MathF.Min(Baslangic.Y, Bitis.Y);
        float maxX = MathF.Max(Baslangic.X, Bitis.X);
        float maxY = MathF.Max(Baslangic.Y, Bitis.Y);
        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    public override void MoveBy(float dx, float dy)
    {
        // Cizgi sekli, baslangic ve bitis noktalarini dx ve dy kadar kaydirir.
        Baslangic = new PointF(Baslangic.X + dx, Baslangic.Y + dy);
        Bitis = new PointF(Bitis.X + dx, Bitis.Y + dy);
    }
}

