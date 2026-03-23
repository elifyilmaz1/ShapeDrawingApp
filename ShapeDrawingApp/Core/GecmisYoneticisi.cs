using System.Text.Json;
using ShapeDrawingApp.Models;

namespace ShapeDrawingApp.Core;

public class GecmisYoneticisi
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Stack<string> _gerial = new();
    private readonly Stack<string> _ilerial = new();

    public GecmisYoneticisi(JsonSerializerOptions jsonOptions)
    {
        _jsonOptions = jsonOptions;
    }

    public bool geriAlma => _gerial.Count > 1;
    public bool ileriAlma => _ilerial.Count > 0;

    public void Sifirla(List<Sekil> sekiller)
    {
        _gerial.Clear();
        _ilerial.Clear();
        _gerial.Push(SekilleriSerilestir(sekiller));
    }

    public void Kaydet(List<Sekil> sekiller)
    {
        _gerial.Push(SekilleriSerilestir(sekiller));
        _ilerial.Clear();
    }

    public List<Sekil> GeriAl(List<Sekil> mevcut)
    {
        if (!geriAlma)
            return mevcut;

        _ilerial.Push(SekilleriSerilestir(mevcut));
        _gerial.Pop(); 
        return SekilleriCoz(_gerial.Peek());
    }

    public List<Sekil> IleriAl(List<Sekil> mevcut)
    {
        if (!ileriAlma)
            return mevcut;

        _gerial.Push(SekilleriSerilestir(mevcut));
        var nextJson = _ilerial.Pop();
        return SekilleriCoz(nextJson);
    }

    private string SekilleriSerilestir(List<Sekil> sekiller)
        => JsonSerializer.Serialize(sekiller, _jsonOptions);

    private List<Sekil> SekilleriCoz(string json)
        => JsonSerializer.Deserialize<List<Sekil>>(json, _jsonOptions) ?? new List<Sekil        >();
}

