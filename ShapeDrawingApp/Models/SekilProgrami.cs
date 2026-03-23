using System.Drawing;

namespace ShapeDrawingApp.Models;

public static class SekilProgrami
{
    public static RectangleF KoselereGoreSinir(params PointF[] koseler)
    {
        if (koseler is null || koseler.Length == 0)
            return RectangleF.Empty;

        float minX = koseler[0].X;
        float minY = koseler[0].Y;
        float maxX = koseler[0].X;
        float maxY = koseler[0].Y;

        for (int i = 1; i < koseler.Length; i++)
        {
            minX = MathF.Min(minX, koseler[i].X);
            minY = MathF.Min(minY, koseler[i].Y);
            maxX = MathF.Max(maxX, koseler[i].X);
            maxY = MathF.Max(maxY, koseler[i].Y);
        }

        return RectangleF.FromLTRB(minX, minY, maxX, maxY);
    }

    public static bool GecerliBoyut(float genislik, float yukseklik, float minBoyut = 2f)
        => MathF.Abs(genislik) >= minBoyut && MathF.Abs(yukseklik) >= minBoyut;
}

