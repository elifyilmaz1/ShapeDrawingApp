using System.Drawing;

namespace ShapeDrawingApp.Models;

public abstract class Sekil
{
    // Sekil sinifi, tum sekillerin ortak ozelliklerini ve davranislarini tanimlar.
    protected Sekil()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }

    public Color KenarlikRengi { get; set; } = Color.Black;

    public Color İcRengi { get; set; } = Color.FromArgb(160, Color.LightSkyBlue);

    public float Kalinlik { get; set; } = 2f;

    public abstract Sekiller ShapeType { get; }

    public abstract void Draw(Graphics g);

    public abstract bool HitTest(PointF p);

    public abstract RectangleF GetBounds();

    public abstract void MoveBy(float dx, float dy);
}

