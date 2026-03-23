using System;
using System.Drawing;
using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Models
{
 
    public sealed class SekilStiliAdapter : Sekil
    {
        public SekilStiliAdapter(SekilStili style)
        {
            Id = Guid.NewGuid();
            KenarlikRengi = style.KenarlikRengi;
            İcRengi = style.İcRengi;
            Kalinlik = style.Kalinlik;
        }

        public override Sekiller ShapeType => Sekiller.Dikdortgen;

        public override void Draw(Graphics g)
        {
        }

        public override bool HitTest(PointF p) => false;

        public override RectangleF GetBounds() => RectangleF.Empty;

        public override void MoveBy(float dx, float dy)
        {
        }
    }
}