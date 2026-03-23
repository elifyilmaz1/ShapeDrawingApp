using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class CizgiSekil : Sekil
{
    public PointF Baslangic { get; set; }
    public PointF Bitis { get; set; }

    public override Sekiller ShapeType => Sekiller.Cizgi;

    public override void Draw(Graphics g)
    {
        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        g.DrawLine(kalem, Baslangic, Bitis);
    }

    public override bool HitTest(PointF nokta)
    {
        using var yol = new GraphicsPath();
        yol.AddLine(Baslangic, Bitis);
        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds()
    {
        float minX = MathF.Min(Baslangic.X, Bitis.X);
        float minY = MathF.Min(Baslangic.Y, Bitis.Y);
        float maxX = MathF.Max(Baslangic.X, Bitis.X);
        float maxY = MathF.Max(Baslangic.Y, Bitis.Y);
        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    public override void MoveBy(float dx, float dy)
    {
        Baslangic = new PointF(Baslangic.X + dx, Baslangic.Y + dy);
        Bitis = new PointF(Bitis.X + dx, Bitis.Y + dy);
    }
}

