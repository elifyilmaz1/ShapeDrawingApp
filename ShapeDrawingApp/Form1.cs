using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.Json;
using ShapeDrawingApp.Core;
using ShapeDrawingApp.Models;
using ShapeDrawingApp.Serialization;
using ShapeDrawingApp.UI;

namespace ShapeDrawingApp;

public partial class Form1 : Form
{
    private enum InteractionMode
    {
        //etkileşim modu
        Cizim,
        Secili
    }
    //uygulamadaki şekillerin listesi
    private List<Sekil> _sekiller = new();
    private Sekil? _previewShape;
    private Sekil? _selectedShape;
    private Guid? _selectedId;

    private InteractionMode _mod = InteractionMode.Cizim;
    private Sekiller _guncelSekil = Sekiller.Dikdortgen;
    private bool _isDrawing;
    private bool _isDragging;
    private PointF _drawStart;
    private Point _lastMouse;

    private SekilStili _currentStyle;

    private readonly JsonSerializerOptions _jsonOptions;
    private readonly GecmisYoneticisi _history;

    private DoubleBufferedPanel _canvasPanel = null!;
    private ComboBox _shapeComboBox = null!;
    private RadioButton _drawModeRadio = null!;
    private RadioButton _selectModeRadio = null!;
    private NumericUpDown _thicknessUpDown = null!;
    private Button _borderColorButton = null!;
    private Button _fillColorButton = null!;
    private Button _applyPropsButton = null!;
    private Button _deleteButton = null!;
    private Button _undoButton = null!;
    private Button _redoButton = null!;
    private Button _saveButton = null!;
    private Button _loadButton = null!;
    private Label _statusLabel = null!;

    public Form1()
    {
        InitializeComponent();

        _jsonOptions = DrawingJson.CreateOptions();
        _history = new GecmisYoneticisi(_jsonOptions);

        InitDefaultStyle();
        InitUi();

        _history.Sifirla(_sekiller);

        SetSelectedShape(null, syncUi: false);
        UpdateUndoRedoButtons();
        UpdateStatus();
    }

    private void InitDefaultStyle()
    {
        _currentStyle = new SekilStili
        {
            KenarlikRengi = Color.Black,
            İcRengi = Color.FromArgb(160, Color.LightSkyBlue),
            Kalinlik = 2f
        };
        _guncelSekil = Sekiller.Dikdortgen;
    }

    private void InitUi()
    {
        Text = "Sekil Olusturma Uygulaması";
        KeyPreview = true;
        Width = 1100;
        Height = 700;

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 90,
            AutoSize = false,
            WrapContents = true
        };

        _shapeComboBox = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
        _shapeComboBox.Items.AddRange(new object[]
        {
            //şekil seçim combo box'ı
            "Dikdörtgen",
            "Kare",
            "Daire",
            "Elips",
            "Üçgen",
            "Çizgi",
            "Paralelkenar"
        });
        _shapeComboBox.SelectedIndex = 0;
        _shapeComboBox.SelectedIndexChanged += (_, _) => UpdateActiveShapeFromUi();
        toolbar.Controls.Add(new Label { Text = "Şekil:", AutoSize = true, Padding = new Padding(6, 10, 0, 0) });
        toolbar.Controls.Add(_shapeComboBox);
        //mod seçim radio button'ları
        _drawModeRadio = new RadioButton { Text = "Çiz", Checked = true, AutoSize = true, Padding = new Padding(15, 10, 0, 0) };
        _selectModeRadio = new RadioButton { Text = "Seç/Taşı", AutoSize = true, Padding = new Padding(15, 10, 0, 0) };
        _drawModeRadio.CheckedChanged += (_, _) => _mod = _drawModeRadio.Checked ? InteractionMode.Cizim : _mod;
        _selectModeRadio.CheckedChanged += (_, _) => _mod = _selectModeRadio.Checked ? InteractionMode.Secili : _mod;
        toolbar.Controls.Add(_drawModeRadio);
        toolbar.Controls.Add(_selectModeRadio);
        //kalınlık seçimi 
        toolbar.Controls.Add(new Label { Text = "Kalınlık:", AutoSize = true, Padding = new Padding(15, 10, 0, 0) });
        _thicknessUpDown = new NumericUpDown { Minimum = 1, Maximum = 30, DecimalPlaces = 0, Width = 70, Value = (decimal)_currentStyle.Kalinlik };
        _thicknessUpDown.ValueChanged += (_, _) => _currentStyle.Kalinlik = (float)_thicknessUpDown.Value;
        toolbar.Controls.Add(_thicknessUpDown);
        //renk seçim butonları
        _borderColorButton = new Button { Text = "Kenarlık Rengi", Width = 120, Height = 30, BackColor = _currentStyle.KenarlikRengi };
        _borderColorButton.Click += (_, _) => PickColor(isBorder: true);
        toolbar.Controls.Add(_borderColorButton);
        //dolgu rengi seçim butonu
        _fillColorButton = new Button { Text = "İç Rengi", Width = 120, Height = 30, BackColor = _currentStyle.İcRengi };
        _fillColorButton.Click += (_, _) => PickColor(isBorder: false);
        toolbar.Controls.Add(_fillColorButton);
        //seçilen şekle özellikleri uygulama butonu
        _applyPropsButton = new Button { Text = "Seçilenleri Uygula", Width = 220, Height = 35 };
        _applyPropsButton.Click += (_, _) => ApplyPropertiesToSelected();
        toolbar.Controls.Add(_applyPropsButton);
        //seçilen şekli silme butonu
        _deleteButton = new Button { Text = "Seçileni Sil", Width = 140, Height = 35 };
        _deleteButton.Click += (_, _) => DeleteSelectedShape();
        toolbar.Controls.Add(_deleteButton);
        //geri alma ve ileri alma butonları
        _undoButton = new Button { Text = "Geri al (Ctrl+Z)", Width = 140, Height = 35 };
        _undoButton.Click += (_, _) => Undo();
        toolbar.Controls.Add(_undoButton);

        _redoButton = new Button { Text = "İleri al (Ctrl+Y)", Width = 140, Height = 35 };
        _redoButton.Click += (_, _) => Redo();
        toolbar.Controls.Add(_redoButton);
        //dosya kaydetme ve yükleme butonları
        _saveButton = new Button { Text = "Kaydet", Width = 120, Height = 35 };
        _saveButton.Click += (_, _) => SaveToFile();
        toolbar.Controls.Add(_saveButton);

        _loadButton = new Button { Text = "Yükle", Width = 120, Height = 35 };
        _loadButton.Click += (_, _) => LoadFromFile();
        toolbar.Controls.Add(_loadButton);
        //durum göstergesi label'ı
        _statusLabel = new Label { Text = "", AutoSize = true, Padding = new Padding(10, 10, 0, 0) };
        toolbar.Controls.Add(_statusLabel);
        //çizim yapılan panel
        _canvasPanel = new DoubleBufferedPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };
        //panel olayları
        _canvasPanel.Paint += CanvasPanel_Paint;
        _canvasPanel.MouseDown += CanvasPanel_MouseDown;
        _canvasPanel.MouseMove += CanvasPanel_MouseMove;
        _canvasPanel.MouseUp += CanvasPanel_MouseUp;

        Controls.Add(_canvasPanel);
        Controls.Add(toolbar);

        _thicknessUpDown.Value = (decimal)_currentStyle.Kalinlik;
        _borderColorButton.BackColor = _currentStyle.KenarlikRengi;
        _fillColorButton.BackColor = _currentStyle.İcRengi;

        UpdateActiveShapeFromUi();
    }

    private void UpdateActiveShapeFromUi()
    {
        var idx = _shapeComboBox.SelectedIndex;
        _guncelSekil = idx switch
        {
            0 => Sekiller.Dikdortgen,
            1 => Sekiller.Kare,
            2 => Sekiller.Daire,
            3 => Sekiller.Elips,
            4 => Sekiller.Ucgen,
            5 => Sekiller.Cizgi,
            6 => Sekiller.Paralelkenar,
            _ => Sekiller.Dikdortgen
        };
    }

    private void PickColor(bool isBorder)
    {
        using var dlg = new ColorDialog();
        dlg.Color = isBorder ? _currentStyle.KenarlikRengi : _currentStyle.İcRengi;
        dlg.FullOpen = true;

        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            if (isBorder)
            {
                _currentStyle.KenarlikRengi = dlg.Color;
                _borderColorButton.BackColor = dlg.Color;
            }
            else
            {
                _currentStyle.İcRengi = dlg.Color;
                _fillColorButton.BackColor = dlg.Color;
            }

        }
    }

    private void ApplyPropertiesToSelected()
    {
        if (_selectedShape == null)
        {
            MessageBox.Show("Önce bir şekil seç (Seç/oynat modu).", "Seçim yok", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _selectedShape.KenarlikRengi = _currentStyle.KenarlikRengi;
        _selectedShape.İcRengi = _currentStyle.İcRengi;
        _selectedShape.Kalinlik = _currentStyle.Kalinlik;

        _history.Kaydet(_sekiller);
        UpdateUndoRedoButtons();
        canvasPanelInvalidate();
    }

    private void DeleteSelectedShape()
    {
        if (_selectedShape == null || _selectedId == null)
            return;

        _sekiller.RemoveAll(s => s.Id == _selectedId.Value);
        SetSelectedShape(null, syncUi: false);

        _history.Kaydet(_sekiller);
        UpdateUndoRedoButtons();
        canvasPanelInvalidate();
    }

    private void Undo()
    {
        var next = _history.GeriAl(_sekiller);
        _sekiller = next;
        RemapSelectionAfterHistory();
        UpdateUndoRedoButtons();
        canvasPanelInvalidate();
    }

    private void Redo()
    {
        var next = _history.IleriAl(_sekiller);
        _sekiller = next;
        RemapSelectionAfterHistory();
        UpdateUndoRedoButtons();
        canvasPanelInvalidate();
    }

    private void RemapSelectionAfterHistory()
    {
        if (_selectedId == null)
        {
            _selectedShape = null;
            return;
        }

        _selectedShape = _sekiller.FirstOrDefault(s => s.Id == _selectedId.Value);
        if (_selectedShape == null)
        {
            _selectedId = null;
        }
        else
        {
            SyncUiToSelectedShape(_selectedShape);
        }

        UpdateStatus();
    }

    private void SaveToFile()
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json",
            AddExtension = true
        };

        if (sfd.ShowDialog(this) != DialogResult.OK)
            return;

        var doc = new DrawingDocument
        {
            Version = 1,
            Sekiller = _sekiller
        };

        var json = JsonSerializer.Serialize(doc, _jsonOptions);
        File.WriteAllText(sfd.FileName, json);
    }

    private void LoadFromFile()
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = "json"
        };

        if (ofd.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            var json = File.ReadAllText(ofd.FileName);
            var doc = JsonSerializer.Deserialize<DrawingDocument>(json, _jsonOptions);
            _sekiller = doc?.Sekiller ?? new List<Sekil>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load JSON: {ex.Message}", "Load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetSelectedShape(null, syncUi: false);
        _previewShape = null;

        _history.Sifirla(_sekiller);
        UpdateUndoRedoButtons();
        UpdateStatus();
        canvasPanelInvalidate();
    }

    private void SetSelectedShape(Sekil? shape, bool syncUi)
    {
        _selectedShape = shape;
        _selectedId = shape?.Id;

        if (shape != null && syncUi)
            SyncUiToSelectedShape(shape);

        UpdateStatus();
    }

    private void SyncUiToSelectedShape(Sekil shape)
    {
        var thickness = (decimal)shape.Kalinlik;
        var clamped = Math.Max(_thicknessUpDown.Minimum, Math.Min(_thicknessUpDown.Maximum, thickness));
        _thicknessUpDown.Value = clamped;
        _borderColorButton.BackColor = shape.KenarlikRengi;
        _fillColorButton.BackColor = shape.İcRengi;

        _currentStyle = new SekilStili
        {
            KenarlikRengi = shape.KenarlikRengi,
            İcRengi = shape.İcRengi,
            Kalinlik = shape.Kalinlik
        };
    }

    private void UpdateUndoRedoButtons()
    {
        _undoButton.Enabled = _history.geriAlma;
        _redoButton.Enabled = _history.ileriAlma;
    }

    private void UpdateStatus()
    {
        var selectedText = _selectedShape == null ? "yok" : _selectedShape.ShapeType.ToString();
        _statusLabel.Text = $"Sekiller: {_sekiller.Count} tane | Secili: {selectedText} | Mod: {_mod}";
    }

    private void canvasPanelInvalidate()
    {
        _canvasPanel.Invalidate();
    }

    private void CanvasPanel_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        foreach (var shape in _sekiller)
            shape.Draw(g);

        _previewShape?.Draw(g);

        if (_selectedShape != null)
        {
            var b = _selectedShape.GetBounds();
            using var kalem = new Pen(Color.Orange, 2) { DashStyle = DashStyle.Dash };
            g.DrawRectangle(kalem, b.X - 3, b.Y - 3, b.Width + 6, b.Height + 6);
        }
    }

    private void CanvasPanel_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        _canvasPanel.Focus();

        if (_mod == InteractionMode.Cizim)
        {
            SetSelectedShape(null, syncUi: false);
            _previewShape = null;
            _drawStart = e.Location;
            _isDrawing = true;
            _canvasPanel.Capture = true;
            canvasPanelInvalidate();
            return;
        }

        _isDragging = false;
        _canvasPanel.Capture = true;

        var hit = HitTest(e.Location);
        if (hit != null)
        {
            SetSelectedShape(hit, syncUi: true);
            _isDragging = true;
            _lastMouse = e.Location;
        }
        else
        {
            SetSelectedShape(null, syncUi: false);
        }

        canvasPanelInvalidate();
    }

    private void CanvasPanel_MouseMove(object? sender, MouseEventArgs e)
    {
        if (_mod == InteractionMode.Cizim)
        {
            if (!_isDrawing)
                return;

            var end = e.Location;
            _previewShape = SekilOlusturma.SekilOlustur(_guncelSekil, _drawStart, end, _currentStyle);
            canvasPanelInvalidate();
            return;
        }

        if (!_isDragging || _selectedShape == null)
            return;

        var dx = e.Location.X - _lastMouse.X;
        var dy = e.Location.Y - _lastMouse.Y;
        if (dx == 0 && dy == 0)
            return;

        _selectedShape.MoveBy(dx, dy);
        _lastMouse = e.Location;
        canvasPanelInvalidate();
    }

    private void CanvasPanel_MouseUp(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
            return;

        _canvasPanel.Capture = false;

        if (_mod == InteractionMode.Cizim)
        {
            if (!_isDrawing)
                return;

            _isDrawing = false;

            var end = e.Location;
            if (_previewShape != null && SekilOlusturma.IsMeaningfulShape(_guncelSekil, _drawStart, end))
            {
                _sekiller.Add(_previewShape);
                SetSelectedShape(_previewShape, syncUi: true);
                _previewShape = null;

                _history.Kaydet(_sekiller);
                UpdateUndoRedoButtons();
            }
            else
            {
                _previewShape = null;
            }

            canvasPanelInvalidate();
            return;
        }

        if (_isDragging)
        {
            _isDragging = false;
            _history.Kaydet(_sekiller);
            UpdateUndoRedoButtons();
        }

        canvasPanelInvalidate();
    }

    private Sekil? HitTest(PointF p)
    {
        for (int i = _sekiller.Count - 1; i >= 0; i--)
        {
            if (_sekiller[i].HitTest(p))
                return _sekiller[i];
        }

        return null;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Control && e.KeyCode == Keys.Z)
        {
            Undo();
            e.Handled = true;
            return;
        }

        if (e.Control && e.KeyCode == Keys.Y)
        {
            Redo();
            e.Handled = true;
            return;
        }

        if (e.KeyCode == Keys.Delete)
        {
            DeleteSelectedShape();
            e.Handled = true;
        }
    }
}
