using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Serialization;

public class ShapeJsonConverter : JsonConverter<Sekil>
{
    // ShapeJsonConverter sinifi, Sekil nesnelerinin JSON formatina serileştirilmesi ve deseralizasyonu icin kullanilir.
    public override Sekil? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var typeStr = root.GetProperty("Sekil").GetString();
        if (string.IsNullOrWhiteSpace(typeStr))
            throw new JsonException("EksikSekil.");

        var shapeType = Enum.Parse<Sekiller>(typeStr, ignoreCase: true);

        Guid id = root.TryGetProperty("Id", out var idProp) ? idProp.GetGuid() : Guid.NewGuid();
        int borderArgb = root.GetProperty("Kenarlik Rengi").GetInt32();
        int fillArgb = root.GetProperty("Ic Renk").GetInt32();
        float thickness = (float)root.GetProperty("Kalinlik").GetDouble();

        Sekil shape = shapeType switch
        {
            // Sekil tipine gore uygun Sekil nesnesi olustur
            Sekiller.Dikdortgen => new DikdortgenSekil(),
            Sekiller.Kare => new KareSekil(),
            Sekiller.Daire => new DaireSekil(),
            Sekiller.Elips => new ElipsSekil(),
            Sekiller.Ucgen => new UcgenSekil(),
            Sekiller.Cizgi => new CizgiSekil(),
            Sekiller.Paralelkenar => new ParalelkenarSekil(),
            _ => throw new JsonException($"Desteklenmeyen Sekil: {shapeType}")
        };

        shape.Id = id;
        shape.KenarlikRengi = Color.FromArgb(borderArgb);
        shape.İcRengi = Color.FromArgb(fillArgb);
        shape.Kalinlik = thickness;

        // Sekil tipine gore ek ozellikleri oku
        switch (shape)
        {
            case DikdortgenSekil dikdortgen:
                dikdortgen.X = (float)root.GetProperty("X").GetDouble();
                dikdortgen.Y = (float)root.GetProperty("Y").GetDouble();
                dikdortgen.Genislik = (float)root.GetProperty("Genislik").GetDouble();
                dikdortgen.Yukseklik = (float)root.GetProperty("Yukseklik").GetDouble();
                break;

            case KareSekil kare:
                kare.X = (float)root.GetProperty("X").GetDouble();
                kare.Y = (float)root.GetProperty("Y").GetDouble();
                kare.Boyut = (float)root.GetProperty("Boyut").GetDouble();
                break;

            case DaireSekil daire:
                daire.X = (float)root.GetProperty("X").GetDouble();
                daire.Y = (float)root.GetProperty("Y").GetDouble();
                daire.Cap = (float)root.GetProperty("Cap").GetDouble();
                break;

            case ElipsSekil elips:
                elips.X = (float)root.GetProperty("X").GetDouble();
                elips.Y = (float)root.GetProperty("Y").GetDouble();
                elips.Genislik = (float)root.GetProperty("Genislik").GetDouble();
                elips.Yukseklik = (float)root.GetProperty("Yukseklik").GetDouble();
                break;

            case CizgiSekil cizgi:
                cizgi.Baslangic = new PointF(
                    ReadFloatProperty(root, "BaslangicX", "BaslangıçX"),
                    ReadFloatProperty(root, "BaslangicY", "BaslangıçY"));
                cizgi.Bitis = new PointF(
                    ReadFloatProperty(root, "BitisX", "BitişX"),
                    ReadFloatProperty(root, "BitisY", "BitişY"));
                break;

            case UcgenSekil ucgen:
                ReadPointsArray(root, "Koseler", 3, out var t1, out var t2, out var t3);
                ucgen.Nokta1 = t1;
                ucgen.Nokta2 = t2;
                ucgen.Nokta3 = t3;
                break;

            case ParalelkenarSekil paralelkenar:
                ReadPointsArray(root, "Koseler", 4, out var p1, out var p2, out var p3, out var p4);
                paralelkenar.Nokta1 = p1;
                paralelkenar.Nokta2 = p2;
                paralelkenar.Nokta3 = p3;
                paralelkenar.Nokta4 = p4;
                break;
        }

        return shape;
    }

    public override void Write(Utf8JsonWriter writer, Sekil value, JsonSerializerOptions options)
    {
        // Sekil nesnesini JSON formatina serileştir
        writer.WriteStartObject();

        writer.WriteString("Sekil", value.ShapeType.ToString());
        writer.WriteString("Id", value.Id.ToString());
        writer.WriteNumber("Kenarlik Rengi", value.KenarlikRengi.ToArgb());
        writer.WriteNumber("Ic Renk", value.İcRengi.ToArgb());
        writer.WriteNumber("Kalinlik", value.Kalinlik);

        switch (value)
        {
            // Sekil tipine gore ek ozellikleri yaz
            case DikdortgenSekil dikdortgen:
                writer.WriteNumber("X", dikdortgen.X);
                writer.WriteNumber("Y", dikdortgen.Y);
                writer.WriteNumber("Genislik", dikdortgen.Genislik);
                writer.WriteNumber("Yukseklik", dikdortgen.Yukseklik);
                break;

            case KareSekil kare:
                writer.WriteNumber("X", kare.X);
                writer.WriteNumber("Y", kare.Y);
                writer.WriteNumber("Boyut", kare.Boyut);
                break;

            case DaireSekil daire:
                writer.WriteNumber("X", daire.X);
                writer.WriteNumber("Y", daire.Y);
                writer.WriteNumber("Cap", daire.Cap);
                break;

            case ElipsSekil elips:
                writer.WriteNumber("X", elips.X);
                writer.WriteNumber("Y", elips.Y);
                writer.WriteNumber("Genislik", elips.Genislik);
                writer.WriteNumber("Yukseklik", elips.Yukseklik);
                break;

            case UcgenSekil ucgen:
                WritePointsArray(writer, "Koseler:", new[] { ucgen.Nokta1, ucgen.Nokta2, ucgen.Nokta3 });
                break;

            case CizgiSekil cizgi:
                writer.WriteNumber("BaslangicX", cizgi.Baslangic.X);
                writer.WriteNumber("BaslangicY", cizgi.Baslangic.Y);
                writer.WriteNumber("BitisX", cizgi.Bitis.X);
                writer.WriteNumber("BitisY", cizgi.Bitis.Y);
                break;

            case ParalelkenarSekil paralelkenar:
                WritePointsArray(writer, "Koseler:", new[] { paralelkenar.Nokta1, paralelkenar.Nokta2, paralelkenar.Nokta3, paralelkenar.Nokta4 });
                break;

            default:
                throw new JsonException($"Desteklenmeyen Sekil {value.GetType().Name}");
        }

        writer.WriteEndObject();
    }

    private static void WritePointsArray(Utf8JsonWriter writer, string name, PointF[] points)
    {
        // Bir dizi nokta koordinatlarini JSON formatinda yaz
        writer.WritePropertyName(name);
        writer.WriteStartArray();
        foreach (var pt in points)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(pt.X);
            writer.WriteNumberValue(pt.Y);
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }

    // Bir dizi nokta koordinatlarini JSON formatindan oku
    private static void ReadPointsArray(JsonElement root, string name, int expectedCount, out PointF a, out PointF b, out PointF c)
    {
        var pointsEl = ReadRequiredProperty(root, name, $"{name}:");
        if (pointsEl.GetArrayLength() != expectedCount)
            throw new JsonException($"Expected {expectedCount} points in '{name}'.");

        a = ReadPoint(pointsEl[0]);
        b = ReadPoint(pointsEl[1]);
        c = ReadPoint(pointsEl[2]);
    }

    private static void ReadPointsArray(JsonElement root, string name, int expectedCount, out PointF a, out PointF b, out PointF c, out PointF d)
    {
        var pointsEl = ReadRequiredProperty(root, name, $"{name}:");
        if (pointsEl.GetArrayLength() != expectedCount)
            throw new JsonException($"Expected {expectedCount} points in '{name}'.");

        a = ReadPoint(pointsEl[0]);
        b = ReadPoint(pointsEl[1]);
        c = ReadPoint(pointsEl[2]);
        d = ReadPoint(pointsEl[3]);
    }

    private static PointF ReadPoint(JsonElement pointEl)
    {
        float x = (float)pointEl[0].GetDouble();
        float y = (float)pointEl[1].GetDouble();
        return new PointF(x, y);
    }

    private static JsonElement ReadRequiredProperty(JsonElement root, params string[] names)
    {
        foreach (var name in names)
        {
            if (root.TryGetProperty(name, out var value))
                return value;
        }

        throw new JsonException($"Missing required property. Expected one of: {string.Join(", ", names)}");
    }

    // Bir float ozelligi JSON formatindan oku, alternatif isimler desteklenir
    private static float ReadFloatProperty(JsonElement root, params string[] names)
    {
        return (float)ReadRequiredProperty(root, names).GetDouble();
    }
}

