using System.Drawing;
using System.Drawing.Drawing2D;

namespace ShapeDrawingApp.Models;

public class ParalelkenarSekil : Sekil
{
    // Paralelkenar sekli, 4 kose koordinatlarini tutar.
    public PointF Nokta1 { get; set; } 
    public PointF Nokta2 { get; set; } 
    public PointF Nokta3 { get; set; } 
    public PointF Nokta4 { get; set; } 

    public override Sekiller ShapeType => Sekiller.Paralelkenar;

    public PointF P1 { get; private set; }

    public override void Draw(Graphics g)
    {
        // Paralelkenar sekli, kenarlik rengi, kalinligi ve ic rengi kullanarak cizilir.
        var koseler = new[] { Nokta1, Nokta2, Nokta3, Nokta4 };
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
        // Paralelkenar sekli, noktanin paralelkenarin icinde veya kenarinda olup olmadigini kontrol eder.
        var koseler = new[] { Nokta1, Nokta2, Nokta3, Nokta4 };
        using var yol = new GraphicsPath();
        yol.AddPolygon(koseler);

        if (İcRengi .A > 0 && yol.IsVisible(nokta))
            return true;

        using var kalem = new Pen(KenarlikRengi, Kalinlik  + 6);
        return yol.IsOutlineVisible(nokta, kalem);
    }

    public override RectangleF GetBounds() => SekilProgrami.KoselereGoreSinir(Nokta1, Nokta2, Nokta3, Nokta4);

    public override void MoveBy(float dx, float dy)
    {
        // Paralelkenar sekli, 4 kose koordinatlarini dx ve dy kadar kaydirir.
        Nokta1 = new PointF(Nokta1.X + dx, Nokta1.Y + dy);
        Nokta2 = new PointF(Nokta2.X + dx, Nokta2.Y + dy);
        Nokta3 = new PointF(Nokta3.X + dx, Nokta3.Y + dy);
        Nokta4 = new PointF(Nokta4.X + dx, Nokta4.Y + dy);
    }
}

