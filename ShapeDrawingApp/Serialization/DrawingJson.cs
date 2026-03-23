using System.Text.Json;
using System.Text.Json.Serialization;
using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Serialization;

public static class DrawingJson
{
    public static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        options.Converters.Add(new ShapeJsonConverter());
        return options;
    }
}

