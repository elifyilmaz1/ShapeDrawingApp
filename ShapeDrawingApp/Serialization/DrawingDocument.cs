using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Serialization;

public class DrawingDocument
{
    // DrawingDocument sinifi, cizim belgesinin serileştirilmesi ve deseralizasyonu icin kullanilir.
    public int Version { get; set; } = 1;
    public List<Sekil> Sekiller { get; set; } = new();
}

