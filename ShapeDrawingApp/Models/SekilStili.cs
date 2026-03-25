using System.Drawing;

namespace ShapeDrawingApp.Models;

public struct SekilStili
{
    // SekilStili yapisi, sekil ciziminde kullanilan kenarlik rengi, ic rengi ve kalinlik gibi stil ozelliklerini tutar.
    public Color KenarlikRengi { get; set; }
    public Color İcRengi { get; set; }
    public float Kalinlik { get; set; }
}

