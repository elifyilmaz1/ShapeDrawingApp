using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Serialization;

public class DrawingDocument
{
    public int Version { get; set; } = 1;
    public List<Sekil> Sekiller { get; set; } = new();
}

