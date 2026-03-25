using System.Drawing;
using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Core;

public static class SekilOlusturma
{
    // SekilOlustur metodu, verilen sekil tipine, baslangic ve bitis noktalarina ve sekil stiline gore uygun bir Sekil nesnesi olusturur.
    public static Sekil SekilOlustur(Sekiller type, PointF start, PointF end, SekilStili style)
    {

        float dx = end.X - start.X;

        float dy = end.Y - start.Y;

        float w = MathF.Abs(dx);

        float h = MathF.Abs(dy);



        float xMin = MathF.Min(start.X, end.X);

        float yMin = MathF.Min(start.Y, end.Y);


        // Sekil tipine gore uygun Sekil nesnesi olusturulur. Dikdortgen, Kare, Daire, Elips, Ucgen, Cizgi ve Paralelkenar gibi sekil tipleri desteklenir.
        Sekil shape = type switch

        {

            Sekiller.Dikdortgen => new DikdortgenSekil { X = xMin, Y = yMin, Genislik = w, Yukseklik = h },

            Sekiller.Kare => new KareSekil { X = xMin, Y = yMin, Boyut = MathF.Min(w, h) },

            Sekiller.Daire => new DaireSekil { X = xMin, Y = yMin, Cap = MathF.Min(w, h) },

            Sekiller.Elips => new ElipsSekil { X = xMin, Y = yMin, Genislik = w, Yukseklik = h },

            Sekiller.Ucgen => CreateTriangle(xMin, yMin, w, h),

            Sekiller.Cizgi => new CizgiSekil { Baslangic = start, Bitis = end },

            Sekiller.Paralelkenar => CreateParallelogram(xMin, yMin, w, h, dx),

            _ => throw new NotSupportedException($"Unsupported shape type: {type}")

        };



        shape.KenarlikRengi = style.KenarlikRengi;

        shape.İcRengi = style.İcRengi;

        shape.Kalinlik = style.Kalinlik;



        return shape;

    }

    // IsMeaningfulShape metodu, verilen sekil tipine, baslangic ve bitis noktalarina gore sekilin anlamli bir sekil olup olmadigini kontrol eder. Cizgi, Kare, Daire gibi sekil tipleri icin belirli bir minimum boyut kontrolu yapilir.
    public static bool IsMeaningfulShape(Sekiller type, PointF start, PointF end)
    {
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float w = MathF.Abs(dx);
        float h = MathF.Abs(dy);

        return type switch
        {
            Sekiller.Cizgi => (w >= 2f || h >= 2f),
            Sekiller.Kare => MathF.Min(w, h) >= 2f,
            Sekiller.Daire => MathF.Min(w, h) >= 2f,
            Sekiller.Dikdortgen => w >= 2f && h >= 2f,
            Sekiller.Elips => w >= 2f && h >= 2f,
            Sekiller.Ucgen => w >= 2f && h >= 2f,
            Sekiller.Paralelkenar => w >= 2f && h >= 2f,
            _ => true
        };
    }
    // CreateTriangle metodu, verilen konum ve boyutlara gore bir UcgenSekil nesnesi olusturur. Ucgen, verilen dikdortgenin icine sigacak sekilde hesaplanir.
    private static UcgenSekil CreateTriangle(float xMin, float yMin, float w, float h)
    {
        var top = new PointF(xMin + w / 2f, yMin);
        var bottomLeft = new PointF(xMin, yMin + h);
        var bottomRight = new PointF(xMin + w, yMin + h);
        return new UcgenSekil { Nokta1 = top, Nokta2 = bottomLeft, Nokta3 = bottomRight };
    }
    // CreateParallelogram metodu, verilen konum, boyut ve yatay kayma miktarina gore bir ParalelkenarSekil nesnesi olusturur. Paralelkenar, verilen dikdortgenin icine sigacak sekilde hesaplanir ve dx degerine gore sola veya saga kaydirilir.
    private static ParalelkenarSekil CreateParallelogram(float xMin, float yMin, float w, float h, float dx)
    {
        float signX = dx >= 0 ? 1f : -1f;
        float offsetX = w * 0.25f * signX;

        return new ParalelkenarSekil
        {
            Nokta1 = new PointF(xMin, yMin),
            Nokta2 = new PointF(xMin + w, yMin),
            Nokta3 = new PointF(xMin + w + offsetX, yMin + h),
            Nokta4 = new PointF(xMin + offsetX, yMin + h)
        };
    }
}

