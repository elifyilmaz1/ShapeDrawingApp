using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class UcgenSekil : Sekil
{
    public PointF Nokta1 { get; set; }
    public PointF Nokta2 { get; set; }
    public PointF Nokta3 { get; set; }

    public override Sekiller ShapeType => Sekiller.Ucgen;

    public override void Draw(Graphics g)
    {
        var koseler = new[] { Nokta1, Nokta2, Nokta3};

        using var kalem = new Pen(KenarlikRengi, Kalinlik);
        if (İcRengi.A > 0)
        {
            using var firca = new SolidBrush(İcRengi);
            g.FillPolygon(firca, koseler);
        }

        g.DrawPolygon(kalem, koseler);
    }

    public override bool HitTest(PointF nokta)
    {
        using var yol = new GraphicsPath();
        yol.AddPolygon(new[] { Nokta1, Nokta2, Nokta3 });

        if (İcRengi.A > 0 && yol.IsVisible(nokta))
            return true;

        using var kalem = new Pen(KenarlikRengi, Kalinlik + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds() => SekilProgrami.KoselereGoreSinir(Nokta1, Nokta2, Nokta3);

    public override void MoveBy(float dx, float dy)
    {
        Nokta1 = new PointF(Nokta1.X + dx, Nokta1.Y + dy);
        Nokta2 = new PointF(Nokta2.X + dx, Nokta2.Y + dy);
        Nokta3 = new PointF(Nokta3.X + dx, Nokta3.Y + dy);
    }
}

