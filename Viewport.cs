// Decompiled with JetBrains decompiler
// Type: RoomEditor.Viewport
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoomEditor
{
  public class Viewport : IDisposable
  {
    public const MouseButtons moveButton = MouseButtons.Middle;
    public const MouseButtons placeButton = MouseButtons.Left;
    public const MouseButtons undoMouseButton = MouseButtons.XButton1;
    public const MouseButtons redoMousebutton = MouseButtons.XButton2;
    public const MouseButtons cancelButton = MouseButtons.Right;
    public const Keys pickingButton = Keys.ControlKey;
    public const Keys bucketingButton = Keys.ShiftKey;
    public const Keys allLayerActionButton = Keys.Menu;
    public const Keys keyUp = Keys.Up;
    public const Keys keyDown = Keys.Down;
    public const Keys keyLeft = Keys.Left;
    public const Keys keyRight = Keys.Right;
    public const float zoomPower = 0.1f;
    private const int minSize = 16;
    private const int maxSize = 32000;
    private InterpolablePictureBox m_PictureBox;
    private RectangleF m_ViewedArea;
    private Layers m_Layers;
    private RoomRenderer m_Renderer;
    private bool m_bMoving;
    private bool m_bZooming;
    private bool m_bPlacingBlocks;
    private bool m_bMovingSelection;
    private bool m_bUpdatingPictureBoxImage;
    private bool m_bPicking;
    private bool m_bBucketing;
    private bool m_bActingOnAllLayers;
    private Point m_LastMousePosition;
    private Size m_LastPictureBoxSize;
    private RectangleF m_AreaToolSelection;
    private RectangleF m_LastSelectedArea = RectangleF.Empty;
    private int m_LastValueIndex;
    private Point m_LastPickPosition;
    private int m_LastMarkerIndex;
    private Point m_LastMarkerPosition;
    private frmMain m_MainForm;
    private Bitmap m_PickerBitmap;
    private Bitmap m_BucketBitmap;
    private ToolCursor m_Cursor;
    private float m_fCurrentZoom = 1f;
    private TextureBrush m_AreaToolBrush;
    private Brush m_PlacingBlocksBrush;

    public Bitmap target => this.m_Renderer.target;

    public RectangleF viewedArea => this.m_ViewedArea;

    public RectangleF toolSelectionArea => this.m_AreaToolSelection;

    public float zoomRatio => 1f / this.m_fCurrentZoom;

    public float zoom => this.m_fCurrentZoom;

    public History history { get; set; }

    public event Viewport.ViewportChangedHandler displayChanged;

    public event Viewport.ViewportChangedHandler toolSelectionAreaChanged;

    public event Viewport.ZoomChangedHandler zoomChanged;

    public Viewport(InterpolablePictureBox _pictureBox, Layers _layers, frmMain _mainForm)
    {
      this.m_PictureBox = _pictureBox;
      this.m_bMoving = false;
      this.m_bPlacingBlocks = false;
      this.m_bZooming = false;
      this.m_bUpdatingPictureBoxImage = false;
      this.m_bPicking = false;
      this.m_bActingOnAllLayers = false;
      this.m_bBucketing = false;
      this.m_Layers = _layers;
      this.m_LastPictureBoxSize = this.m_PictureBox.Size;
      this.m_Renderer = new RoomRenderer(this.m_Layers);
      this.m_Layers.beforeResetSelection += new Layers.BeforeResetSelectionHandler(this.BeforeResetSelection);
      this.m_PictureBox.MouseDown += new MouseEventHandler(this.OnMouseDown);
      this.m_PictureBox.MouseMove += new MouseEventHandler(this.OnMouseMove);
      this.m_PictureBox.MouseEnter += new EventHandler(this.OnMouseEnter);
      this.m_PictureBox.MouseUp += new MouseEventHandler(this.OnMouseUp);
      this.m_PictureBox.MouseWheel += new MouseEventHandler(this.OnMouseWheel);
      this.m_PictureBox.Resize += new EventHandler(this.OnResize);
      this.m_PictureBox.SizeMode = PictureBoxSizeMode.Normal;
      this.m_PictureBox.interpolationMode = InterpolationMode.NearestNeighbor;
      Size size = _pictureBox.Size;
      int width = size.Width;
      size = _pictureBox.Size;
      int height = size.Height;
      this.m_ViewedArea = (RectangleF) new Rectangle(0, 0, width, height);
      this.UpdatePictureBoxImage();
      this.m_LastPickPosition = new Point(-1, -1);
      this.m_MainForm = _mainForm;
      this.m_Cursor = new ToolCursor(this, (PictureBox) this.m_PictureBox);
      this.m_Cursor.pictureBox.MouseDown += new MouseEventHandler(this.OnMouseDown);
      this.m_Cursor.pictureBox.MouseMove += new MouseEventHandler(this.OnMouseMove);
      this.m_Cursor.pictureBox.MouseUp += new MouseEventHandler(this.OnMouseUp);
      this.m_Cursor.pictureBox.MouseWheel += new MouseEventHandler(this.OnMouseWheel);
      Bitmap bitmap = (Bitmap) Image.FromFile(".\\Resources\\RoomEditor\\cols.png");
      this.m_PickerBitmap = new Bitmap(16, 16);
      Graphics graphics1 = Graphics.FromImage((Image) this.m_PickerBitmap);
      graphics1.DrawImage((Image) bitmap, new Rectangle(0, 0, 16, 16), new Rectangle(16, 16, 16, 16), GraphicsUnit.Pixel);
      graphics1.Dispose();
      this.m_BucketBitmap = new Bitmap(16, 16);
      Graphics graphics2 = Graphics.FromImage((Image) this.m_BucketBitmap);
      graphics2.DrawImage((Image) bitmap, new Rectangle(0, 0, 16, 16), new Rectangle(32, 16, 16, 16), GraphicsUnit.Pixel);
      graphics2.Dispose();
      this.m_PlacingBlocksBrush = (Brush) new SolidBrush(Tools.selection.areaColor);
      bitmap.Dispose();
    }

    public void InvalidateArea(RectangleF _areaToInvalidate)
    {
      this.m_Renderer.InvalidateArea(_areaToInvalidate);
      if (this.displayChanged == null)
        return;
      this.displayChanged();
    }

    public void Invalidate()
    {
      this.m_Renderer.Invalidate();
      if (this.displayChanged == null)
        return;
      this.displayChanged();
    }

    public void InvalidateArea(Rectangle _areaToInvalidate)
    {
      this.m_Renderer.InvalidateArea(_areaToInvalidate);
      if (this.displayChanged == null)
        return;
      this.displayChanged();
    }

    public void UpdatePictureBoxImage()
    {
      if (this.m_bUpdatingPictureBoxImage)
        return;
      this.m_bUpdatingPictureBoxImage = true;
      if (this.m_PictureBox.Image == null)
        this.m_PictureBox.Image = (Image) new Bitmap(this.m_PictureBox.Width, this.m_PictureBox.Height);
      Bitmap image = (Bitmap) this.m_PictureBox.Image;
      Graphics graphics = Graphics.FromImage((Image) image);
      graphics.Clear(frmMain.activeMain.BackColor);
      graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      graphics.DrawImage((Image) this.m_Renderer.target, (RectangleF) new Rectangle(0, 0, image.Width, image.Height), this.m_ViewedArea, GraphicsUnit.Pixel);
      RectangleF viewedArea = this.viewedArea;
      Point _this = Point.Truncate(viewedArea.Location);
      Point point = _this - (Size) _this.Divided(16).Multiplied(16);
      if ((double) this.m_AreaToolSelection.Height > 0.0 || (double) this.m_AreaToolSelection.Width > 0.0)
      {
        graphics.ScaleTransform(this.zoom, this.zoom);
        Rectangle rect = this.SnappedRectangle(this.m_AreaToolSelection);
        this.m_AreaToolBrush.ResetTransform();
        this.m_AreaToolBrush.TranslateTransform((float) -point.X, (float) -point.Y);
        rect.Offset(Point.Round(this.m_ViewedArea.Location).Multiplied(-1));
        graphics.FillRectangle((Brush) this.m_AreaToolBrush, rect);
        graphics.FillRectangle(this.m_PlacingBlocksBrush, rect);
        graphics.ScaleTransform(1f / this.zoom, 1f / this.zoom);
      }
      if (Options.instance.displayShowGrids)
      {
        Point empty1 = Point.Empty;
        ref Point local1 = ref empty1;
        viewedArea = this.viewedArea;
        PointF location = viewedArea.Location;
        int num1 = (int) (-(double) location.X * (double) this.zoom - 0.5 * (double) this.zoom);
        local1.X = num1;
        ref Point local2 = ref empty1;
        viewedArea = this.viewedArea;
        location = viewedArea.Location;
        int num2 = (int) (-(double) location.Y * (double) this.zoom - 0.5 * (double) this.zoom);
        local2.Y = num2;
        Point empty2 = Point.Empty;
        empty2.X = (int) ((double) empty1.X + (double) Math.Min((float) (image.Width - empty1.X), (float) (this.m_Layers.width * 16) * this.zoom));
        empty2.Y = (int) ((double) empty1.Y + (double) Math.Min((float) (image.Height - empty1.Y), (float) (this.m_Layers.height * 16) * this.zoom));
        for (int x = empty1.X; x <= empty2.X; x += (int) (16.0 * (double) this.zoom))
          graphics.DrawLine(Pens.Black, new Point(x, empty1.Y), new Point(x, empty2.Y));
        for (int y = empty1.Y; y <= empty2.Y; y += (int) (16.0 * (double) this.zoom))
          graphics.DrawLine(Pens.Black, new Point(empty1.X, y), new Point(empty2.X, y));
      }
      graphics.Dispose();
      this.m_PictureBox.Image = (Image) image;
      this.m_PictureBox.Size = image.Size;
      this.m_PictureBox.Invalidate();
      this.m_PictureBox.Refresh();
      this.m_bUpdatingPictureBoxImage = false;
      if (this.displayChanged == null)
        return;
      this.displayChanged();
    }

    public void Dispose()
    {
      if (this.m_PictureBox.Image != null)
        this.m_PictureBox.Image.Dispose();
      this.m_PictureBox.Image = (Image) null;
      this.m_Renderer.Dispose();
      this.m_Renderer = (RoomRenderer) null;
      this.m_PictureBox.MouseDown -= new MouseEventHandler(this.OnMouseDown);
      this.m_PictureBox.MouseMove -= new MouseEventHandler(this.OnMouseMove);
      this.m_PictureBox.MouseEnter -= new EventHandler(this.OnMouseEnter);
      this.m_PictureBox.MouseUp -= new MouseEventHandler(this.OnMouseUp);
      this.m_PictureBox.MouseWheel -= new MouseEventHandler(this.OnMouseWheel);
      this.m_PictureBox.Resize -= new EventHandler(this.OnResize);
    }

    public bool OnKeyDown(KeyEventArgs _args)
    {
      Rectangle rectangle = Rectangle.Empty;
      if (_args.KeyCode == Keys.Delete || _args.KeyCode == Keys.Back)
      {
        List<Layer> layerList = new List<Layer>();
        Layer currentLayer = this.m_Layers.currentLayer;
        if (this.m_bActingOnAllLayers)
        {
          foreach (Layer layer in this.m_Layers.layers)
            layerList.Add(layer);
        }
        else
          layerList.Add(this.m_Layers.currentLayer);
        foreach (Layer layer in layerList)
        {
          this.m_Layers.currentLayer = layer;
          Rectangle _areaUpdated = Rectangle.Empty;
          if (this.m_Layers.currentLayer.type == Layer.Type.Marker)
          {
            this.history.AddAction();
            List<MarkerInfo> _markers = new List<MarkerInfo>();
            _markers.AddRange(this.m_Layers.selection.markers);
            _areaUpdated = this.m_Layers.RemoveSelectedMarkers();
            this.history.AddEntry((HistoryEntry) new HistoryRemoveMarkersEntry(_markers, this.m_Layers.markerLayer));
          }
          else if (this.m_Layers.hasSelection)
          {
            this.history.AddAction();
            List<Point> _positions;
            this.m_Layers.selection.RetrievePositions(out _positions);
            this.m_Layers.ResetSelection();
            this.PlaceBlockList(_positions, 0, ref _areaUpdated);
            this.history.RemoveEmptyAction();
          }
          else
            this.PlaceBlock(this.m_PictureBox.PointToClient(Control.MousePosition), Tools.eraser.value, ref _areaUpdated);
          if (!_areaUpdated.IsEmpty)
            rectangle = !rectangle.IsEmpty ? Rectangle.Union(rectangle, _areaUpdated) : _areaUpdated;
        }
        if ((double) this.m_LastSelectedArea.GetArea() + (double) rectangle.GetArea() < (double) (this.m_Renderer.target.Width * this.m_Renderer.target.Height))
        {
          this.InvalidateArea(this.m_LastSelectedArea);
          this.InvalidateArea(rectangle);
        }
        else
          this.Invalidate();
        this.InvalidateArea(rectangle);
        this.UpdatePictureBoxImage();
        return true;
      }
      if ((_args.KeyCode == Keys.Up || _args.KeyCode == Keys.Down || (_args.KeyCode == Keys.Left || _args.KeyCode == Keys.Right)) && (!this.m_bMoving && !this.m_bPlacingBlocks && !this.m_Layers.selection.isEmpty))
      {
        Point empty = Point.Empty;
        if (_args.KeyCode == Keys.Up)
          --empty.Y;
        if (_args.KeyCode == Keys.Down)
          ++empty.Y;
        if (_args.KeyCode == Keys.Left)
          --empty.X;
        if (_args.KeyCode == Keys.Right)
          ++empty.X;
        if (empty != Point.Empty)
          this.MoveSelection(empty.Multiplied(16));
        this.Invalidate();
        this.UpdatePictureBoxImage();
        return true;
      }
      if (_args.KeyCode == Keys.ControlKey && !this.m_bPicking && (!this.m_bBucketing && !this.m_bActingOnAllLayers))
      {
        this.m_bPicking = true;
        this.m_MainForm.Cursor = new Cursor(this.m_PickerBitmap.GetHicon());
        return true;
      }
      if (_args.KeyCode == Keys.ShiftKey && Tools.currentTool != null && (Tools.currentTool != Tools.selection && Tools.currentTool != Tools.eraser) && (!this.m_bPicking && !this.m_bBucketing && !this.m_bActingOnAllLayers))
      {
        this.m_bBucketing = true;
        this.m_MainForm.Cursor = new Cursor(this.m_BucketBitmap.GetHicon());
        return true;
      }
      if (_args.KeyCode == Keys.Menu && !this.m_bPicking && !this.m_bBucketing)
      {
        int num = this.m_bActingOnAllLayers ? 1 : 0;
      }
      return false;
    }

    public bool OnKeyUp(KeyEventArgs _args)
    {
      if (_args.KeyCode == Keys.ControlKey)
        this.m_bPicking = false;
      else if (_args.KeyCode == Keys.ShiftKey)
      {
        this.m_bBucketing = false;
      }
      else
      {
        int keyCode = (int) _args.KeyCode;
      }
      this.m_MainForm.Cursor = Cursors.Default;
      return false;
    }

    public void Undo() => this.m_Layers.ResetSelection(!this.m_Layers.selection.moved);

    public void Redo() => this.m_Layers.ResetSelection(false);

    public void Resize(int _newWidth, int _newHeight, bool _isHistoryReplay = false)
    {
      if (_newWidth == this.m_Layers.width && _newHeight == this.m_Layers.height)
        return;
      if (!_isHistoryReplay)
      {
        this.history.AddAction();
        this.history.AddEntry((HistoryEntry) new HistoryResizeEntry(this.m_Layers.width, this.m_Layers.height, _newWidth, _newHeight, this));
      }
      this.m_Layers.Resize(_newWidth, _newHeight);
      this.m_Renderer.Resize(_newWidth, _newHeight);
      this.Invalidate();
      this.UpdatePictureBoxImage();
    }

    public void Paste(ClipboardData _data)
    {
      if (_data.hasMarkerData)
      {
        this.m_Layers.ResetSelection();
        Rectangle rectangle = Rectangle.Empty;
        this.history.AddAction();
        foreach (MarkerInfo marker in _data.markerList)
        {
          MarkerInfo markerInfo = marker.Clone();
          this.m_Layers.PlaceMarker(markerInfo);
          this.history.AddEntry((HistoryEntry) new HistoryPlaceMarkerEntry(markerInfo, this.m_Layers.markerLayer));
          this.m_Layers.selection.AddMarker(markerInfo);
          this.m_Layers.selection.Move(new Point(-16, -16), new Rectangle(0, 0, this.target.Width, this.target.Height));
          rectangle = !(rectangle == Rectangle.Empty) ? Rectangle.Union(rectangle, markerInfo.rect).Multiplied(16) : markerInfo.rect.Multiplied(16);
        }
        this.InvalidateArea(rectangle);
        this.UpdatePictureBoxImage();
      }
      else
      {
        if (!_data.hasTileData)
          return;
        this.m_Layers.ResetSelection();
        Rectangle empty = Rectangle.Empty;
        this.m_Layers.selection.SetTiles(_data.tileList);
        this.m_Layers.selection.Move(new Point(-16, -16), new Rectangle(0, 0, this.target.Width, this.target.Height));
        this.history.AddAction();
        this.InvalidateArea(this.m_Layers.selection.area);
        this.UpdatePictureBoxImage();
      }
    }

    private void MoveSelection(Point _delta)
    {
      if (!this.m_Layers.selection.moved)
      {
        if (this.m_Layers.selection.hasTiles)
        {
          List<Point> _positions = new List<Point>();
          List<int> _previousValues = new List<int>();
          foreach (TileInfo tile in this.m_Layers.selection.tiles)
          {
            _positions.Add(tile.position);
            _previousValues.Add(tile.value);
          }
          this.history.AddAction();
          this.history.AddEntry((HistoryEntry) new HistoryPlaceBlocksEntry(_positions, _previousValues, 0, this.m_Layers.currentLayer.type, this.m_Layers, "Move multiple blocks"));
        }
        else if (this.m_Layers.selection.hasMarkers)
        {
          this.history.AddAction();
          this.history.AddEntry((HistoryEntry) new HistoryMoveMarkerEntry(this.m_Layers.selection.markers, Point.Empty));
        }
      }
      Point _delta1 = this.m_Layers.selection.Move(_delta, new Rectangle(0, 0, this.target.Width, this.target.Height));
      if (!this.m_Layers.selection.hasMarkers)
        return;
      List<HistoryEntry> action = this.history.actions[this.history.actions.Count - 1];
      if (!(action[action.Count - 1] is HistoryMoveMarkerEntry historyMoveMarkerEntry))
        this.history.AddEntry((HistoryEntry) new HistoryMoveMarkerEntry(this.m_Layers.selection.markers, _delta1));
      else
        historyMoveMarkerEntry.moveDelta += (Size) _delta1;
    }

    private void BeforeResetSelection(bool _bRestoreSelectedTiles)
    {
      if (!this.m_Layers.selection.hasTiles || !this.m_Layers.selection.moved)
        return;
      List<Point> _positions = new List<Point>();
      List<int> _previousValues = new List<int>();
      List<int> _nextValues = new List<int>();
      foreach (TileInfo tile in this.m_Layers.selection.tiles)
      {
        _positions.Add(tile.position);
        _nextValues.Add(tile.value);
        List<int> intList = _previousValues;
        Layer currentLayer = this.m_Layers.currentLayer;
        Point position = tile.position;
        int _x = position.X / 16;
        position = tile.position;
        int _y = position.Y / 16;
        int num = currentLayer.GetValuesAt(_x, _y)[0];
        intList.Add(num);
      }
      this.history.AddEntry((HistoryEntry) new HistoryPlaceBlocksEntry(_positions, _previousValues, _nextValues, this.m_Layers.currentLayer.type, this.m_Layers));
    }

    private void SelectMarkers(Rectangle _snappedArea, ref Rectangle _areaUpdated)
    {
      if (_snappedArea.Width == 16 && _snappedArea.Height == 16)
      {
        List<MarkerInfo> markersAt = this.m_Layers.markerLayer.GetMarkersAt(_snappedArea.X / 16, _snappedArea.Y / 16);
        if (markersAt.Count == 0)
        {
          this.m_Layers.SelectBlock(_snappedArea.Location, ref _areaUpdated);
        }
        else
        {
          Point location = _snappedArea.Location;
          int index = markersAt.Count - 1;
          if (location == this.m_LastMarkerPosition)
          {
            index = this.m_LastMarkerIndex - 1;
            if (index < 0)
              index = markersAt.Count - 1;
          }
          MarkerInfo _marker = markersAt[index];
          this.m_Layers.selection.AddMarker(_marker);
          _areaUpdated = _marker.rect.Multiplied(16);
          this.m_LastMarkerPosition = location;
          this.m_LastMarkerIndex = index;
        }
      }
      else
        this.m_Layers.SelectMarkersInArea(_snappedArea, ref _areaUpdated);
    }

    private void SelectTiles(Rectangle _snappedArea, ref Rectangle _areaUpdated) => this.m_Layers.SelectArea(_snappedArea, ref _areaUpdated);

    private void PlaceBlock(Point _loosePosition, int _value, ref Rectangle _areaUpdated)
    {
      Point _position = new Point(_loosePosition.X / 16 * 16, _loosePosition.Y / 16 * 16);
      if (_position.X >= this.m_Renderer.target.Width || _position.Y >= this.m_Renderer.target.Height)
        return;
      List<Layer> layerList = new List<Layer>();
      Layer currentLayer = this.m_Layers.currentLayer;
      Layer.Type _forcedLayer = Layer.Type.None;
      int _previousValue = this.m_Layers.PlaceBlock(_position, _value, ref _forcedLayer, ref _areaUpdated);
      if (_previousValue == _value)
        return;
      this.history.AddEntry((HistoryEntry) new HistoryPlaceBlockEntry(_position.X, _position.Y, _previousValue, _value, _forcedLayer, this.m_Layers));
    }

    private void PlaceMarker(Rectangle _snappedArea, int _value, ref Rectangle _areaUpdated)
    {
      MarkerInfo _marker = this.m_Layers.PlaceMarker(_snappedArea, _value, ref _areaUpdated);
      this.history.AddEntry((HistoryEntry) new HistoryPlaceMarkerEntry(_marker, this.m_Layers.markerLayer));
      this.m_Layers.selection.AddMarker(_marker);
    }

    private void PlaceBlockList(
      List<Point> _snappedPositions,
      int _value,
      ref Rectangle _areaUpdated)
    {
      if (_snappedPositions.Count == 0)
        return;
      Layer.Type _forcedLayer = Layer.Type.None;
      List<int> _previousValues = this.m_Layers.PlaceBlocks(_snappedPositions, _value, ref _forcedLayer, ref _areaUpdated);
      if (_previousValues.FindIndex((Predicate<int>) (elem => elem != _value)) == -1)
        return;
      this.history.AddEntry((HistoryEntry) new HistoryPlaceBlocksEntry(_snappedPositions, _previousValues, _value, _forcedLayer, this.m_Layers));
    }

    private void PlaceRectangle(Rectangle _area, int _value, out Rectangle _updatedArea)
    {
      _updatedArea = new Rectangle();
      Layer.Type _forcedLayer = Layer.Type.None;
      List<Layer> layerList = new List<Layer>();
      List<int> _oldValues;
      if (!this.m_Layers.PlaceBlockRect(_area, _value, out _oldValues, ref _forcedLayer, ref _updatedArea))
        return;
      this.history.AddEntry((HistoryEntry) new HistoryPlaceBlockRectEntry(_area, _oldValues, _value, _forcedLayer, this.m_Layers));
    }

    public void Zoom(int _delta)
    {
      if (_delta == 0)
        return;
      int num1 = (int) (16.0 * (double) this.m_fCurrentZoom);
      float val1 = (float) _delta * 0.1f * (float) num1;
      float num2 = (double) val1 > 0.0 ? Math.Max(val1, 1f) : Math.Min(val1, -1f);
      if ((double) num1 + (double) num2 <= 0.0)
        return;
      this.m_fCurrentZoom = (float) Math.Round((double) num1 + (double) num2) / 16f;
      this.SetViewedAreaSize((float) this.m_PictureBox.Width / this.m_fCurrentZoom, (float) this.m_PictureBox.Height / this.m_fCurrentZoom);
    }

    private void SetViewedAreaSize(float _fWidth, float _fHeight)
    {
      if ((double) _fWidth.Clamp<float>(16f, 32000f) != (double) _fWidth || (double) _fHeight.Clamp<float>(16f, 32000f) != (double) _fHeight)
        return;
      float _oldRatio = 16f;
      this.m_ViewedArea.Width = _fWidth;
      this.m_ViewedArea.Height = _fHeight;
      this.ClampViewedArea();
      this.UpdatePictureBoxImage();
      if (this.zoomChanged == null)
        return;
      this.zoomChanged(_oldRatio, (float) this.m_PictureBox.Width / this.m_ViewedArea.Width);
    }

    public void SetZoom(float _fRatio)
    {
      if ((double) _fRatio <= 0.0 || (double) this.m_fCurrentZoom == (double) _fRatio)
        return;
      this.m_fCurrentZoom = _fRatio;
      this.SetViewedAreaSize((float) this.m_PictureBox.Width / this.m_fCurrentZoom, (float) this.m_PictureBox.Height / this.m_fCurrentZoom);
    }

    public void ResetZoom() => this.SetZoom(1f);

    private void OnMouseEnter(object _sender, EventArgs _args) => this.m_LastMousePosition = this.m_PictureBox.PointToClient(Control.MousePosition);

    private void OnMouseDown(object _sender, MouseEventArgs _args)
    {
      this.m_LastMousePosition = this.m_PictureBox.PointToClient(Control.MousePosition);
      if (_args.Button == MouseButtons.Middle)
        this.m_bMoving = !this.m_bMoving;
      else if (_args.Button == MouseButtons.Right && this.m_bPlacingBlocks)
      {
        this.m_bPlacingBlocks = false;
        this.m_bPicking = false;
        this.m_bBucketing = false;
        this.UpdateToolArea();
      }
      else if (_args.Button == MouseButtons.Right)
      {
        this.m_bPicking = false;
        this.m_bBucketing = false;
        this.m_Layers.ResetSelection();
        this.InvalidateArea(this.m_LastSelectedArea);
        this.UpdatePictureBoxImage();
        this.m_LastSelectedArea = (RectangleF) Rectangle.Empty;
      }
      else if (_args.Button == MouseButtons.Left && this.m_bPicking && (!this.m_bBucketing && !this.m_bActingOnAllLayers))
      {
        this.m_Layers.ResetSelection();
        this.Pick();
      }
      else if (_args.Button == MouseButtons.Left && this.m_bBucketing && (!this.m_bPicking && !this.m_bActingOnAllLayers))
      {
        this.m_Layers.ResetSelection();
        this.Bucket();
      }
      else if (_args.Button == MouseButtons.Left && Tools.currentTool == Tools.mover)
        this.m_bMovingSelection = !this.m_Layers.selection.isEmpty;
      else if (_args.Button == MouseButtons.Left)
      {
        this.m_bPlacingBlocks = true;
        this.m_AreaToolSelection.Offset((PointF) this.m_LastMousePosition);
        if (this.toolSelectionAreaChanged != null)
          this.toolSelectionAreaChanged();
        this.m_AreaToolBrush = new TextureBrush((Image) Tools.currentTool.bitmap);
      }
      else if (_args.Button == MouseButtons.XButton1)
      {
        this.Undo();
      }
      else
      {
        if (_args.Button != MouseButtons.XButton2)
          return;
        this.Redo();
      }
    }

    private void OnMouseUp(object _sender, MouseEventArgs _args)
    {
      if (_args.Button == MouseButtons.Middle)
      {
        this.m_bMoving = false;
      }
      else
      {
        if (_args.Button != MouseButtons.Left)
          return;
        if (this.m_bMovingSelection)
        {
          this.m_bMovingSelection = false;
        }
        else
        {
          this.UpdateToolArea();
          this.m_bPlacingBlocks = false;
        }
      }
    }

    private void UpdateToolArea()
    {
      if (this.m_AreaToolSelection == RectangleF.Empty)
        return;
      this.m_LastSelectedArea = (RectangleF) this.m_Layers.selection.area;
      this.m_Layers.ResetSelection();
      Rectangle rectangle = this.SnappedRectangle(this.m_AreaToolSelection);
      Rectangle _updatedArea = rectangle;
      if (this.m_bPlacingBlocks)
      {
        this.history.AddAction();
        if (Tools.currentTool == Tools.selection)
        {
          if (this.m_Layers.currentLayer.type == Layer.Type.Marker)
            this.SelectMarkers(rectangle, ref _updatedArea);
          else
            this.SelectTiles(rectangle, ref _updatedArea);
        }
        else if (Tools.currentTool.isMarker)
          this.PlaceMarker(rectangle, Tools.currentTool.value, ref _updatedArea);
        else if (rectangle.Width > 16 || rectangle.Height > 16)
          this.PlaceRectangle(rectangle, Tools.currentTool.value, out _updatedArea);
        else
          this.PlaceBlock(new Point(rectangle.X, rectangle.Y), Tools.currentTool.value, ref _updatedArea);
        this.history.RemoveEmptyAction();
      }
      if ((double) this.m_LastSelectedArea.GetArea() + (double) _updatedArea.GetArea() < (double) (this.m_Renderer.target.Width * this.m_Renderer.target.Height))
      {
        this.InvalidateArea(this.m_LastSelectedArea);
        this.InvalidateArea(_updatedArea);
      }
      else
        this.Invalidate();
      this.m_LastSelectedArea = (RectangleF) _updatedArea;
      this.m_AreaToolSelection = (RectangleF) Rectangle.Empty;
      if (this.toolSelectionAreaChanged != null)
        this.toolSelectionAreaChanged();
      this.UpdatePictureBoxImage();
    }

    private Rectangle SnappedRectangle(RectangleF _rectangle)
    {
      int x = (int) ((double) this.m_ViewedArea.X + (double) _rectangle.X * (double) this.zoomRatio) / 16 * 16;
      int num1 = ((int) ((double) this.m_ViewedArea.X + ((double) _rectangle.X + (double) _rectangle.Width) * (double) this.zoomRatio) / 16 + 1) * 16;
      int y = (int) ((double) this.m_ViewedArea.Y + (double) _rectangle.Y * (double) this.zoomRatio) / 16 * 16;
      int num2 = ((int) ((double) this.m_ViewedArea.Y + ((double) _rectangle.Y + (double) _rectangle.Height) * (double) this.zoomRatio) / 16 + 1) * 16;
      int width = num1 - x;
      int num3 = y;
      int height = num2 - num3;
      return new Rectangle(x, y, width, height);
    }

    private Point SnappedPoint(Point _point) => new Point((int) ((double) this.m_ViewedArea.X + (double) _point.X * (double) this.zoomRatio) / 16 * 16, (int) ((double) this.m_ViewedArea.Y + (double) _point.Y * (double) this.zoomRatio) / 16 * 16);

    private void Pick()
    {
      Point _position = this.SnappedPoint(this.m_LastMousePosition);
      List<int> _valueList;
      List<Layer> _layers;
      this.m_Layers.RetrieveValues(_position, out _valueList, out _layers);
      if (_valueList.Count == 0)
        return;
      int index = 0;
      if (_valueList.Count == 1)
        Tools.currentTool = Tools.GetToolForValue(_valueList[0]);
      else if (_position == this.m_LastPickPosition)
      {
        while (index < _valueList.Count && index <= this.m_LastValueIndex)
          ++index;
        if (index >= _layers.Count)
          index = 0;
      }
      else
      {
        Layer.Type type = this.m_Layers.currentLayer.type;
        index = 0;
        while (index < _layers.Count && _layers[index].type != type)
          ++index;
        if (index >= _layers.Count)
          index = 0;
      }
      this.m_LastPickPosition = _position;
      this.m_LastValueIndex = index;
      Tools.currentTool = Tools.GetToolForValue(_valueList[index]);
      frmMain.toolsForm.UpdateCurrentTool();
    }

    private void Bucket()
    {
      if (Tools.currentTool == null || Tools.currentTool == Tools.selection || Tools.currentTool.type != Layer.Type.Collision && Tools.currentTool.type != Layer.Type.Link)
        return;
      Point point1 = this.SnappedPoint(this.m_LastMousePosition);
      List<Point> pointList = new List<Point>();
      pointList.Add(point1);
      int index = 0;
      while (index < pointList.Count)
      {
        Point point2 = pointList[index];
        List<int> _valueList;
        this.m_Layers.RetrieveValues(pointList[index], out _valueList, out List<Layer> _, Tools.currentTool.type);
        if (_valueList.Count == 0)
        {
          if (point2.X - 16 >= 0)
          {
            Point point3 = new Point(point2.X - 16, point2.Y);
            if (!pointList.ContainsPoint(point3))
              pointList.Add(point3);
          }
          if (point2.X + 16 < this.m_Layers.width * 16)
          {
            Point point3 = new Point(point2.X + 16, point2.Y);
            if (!pointList.ContainsPoint(point3))
              pointList.Add(point3);
          }
          if (point2.Y - 16 >= 0)
          {
            Point point3 = new Point(point2.X, point2.Y - 16);
            if (!pointList.ContainsPoint(point3))
              pointList.Add(point3);
          }
          if (point2.Y + 16 < this.m_Layers.height * 16)
          {
            Point point3 = new Point(point2.X, point2.Y + 16);
            if (!pointList.ContainsPoint(point3))
              pointList.Add(point3);
          }
          ++index;
        }
        else
          pointList.RemoveAt(index);
      }
      Rectangle _areaUpdated = new Rectangle();
      this.history.AddAction();
      this.PlaceBlockList(pointList, Tools.currentTool.value, ref _areaUpdated);
      this.history.RemoveEmptyAction();
      this.InvalidateArea(_areaUpdated);
      this.UpdatePictureBoxImage();
    }

    private void OnMouseMove(object _sender, MouseEventArgs _args)
    {
    
      Point client = this.m_PictureBox.PointToClient(Control.MousePosition);
      int x1 = client.X - this.m_LastMousePosition.X;
      client = this.m_PictureBox.PointToClient(Control.MousePosition);
      int y1 = client.Y - this.m_LastMousePosition.Y;
      Point _this = new Point(x1, y1);
      if (this.m_bMoving)
      {
        float num1 = (float) _this.X * this.zoomRatio;
        float num2 = (float) _this.Y * this.zoomRatio;
        this.m_ViewedArea.X -= num1;
        this.m_ViewedArea.Y -= num2;
        this.ClampViewedArea();
        this.m_LastMousePosition = this.m_PictureBox.PointToClient(Control.MousePosition);
        this.UpdatePictureBoxImage();
      }
      else if (this.m_bMovingSelection)
      {
        Point point = _this.Snapped((int) (16.0 * (double) this.zoom));
        if (!point.IsEmpty)
        {
          this.m_LastMousePosition += (Size) point;
          this.MoveSelection(new Point(point.X, point.Y));
          this.Invalidate();
          this.UpdatePictureBoxImage();
        }
      }
      else if (this.m_bPlacingBlocks)
      {
        double width = (double) this.m_AreaToolSelection.Width;
        int x2 = _this.X;
        double height = (double) this.m_AreaToolSelection.Height;
        int y2 = _this.Y;
        this.m_AreaToolSelection.Width = (float) Math.Abs(_this.X);
        this.m_AreaToolSelection.Height = (float) Math.Abs(_this.Y);
        this.m_AreaToolSelection.X = _this.X >= 0 ? (float) this.m_LastMousePosition.X : (float) this.m_LastMousePosition.X - this.m_AreaToolSelection.Width;
        this.m_AreaToolSelection.Y = _this.Y >= 0 ? (float) this.m_LastMousePosition.Y : (float) this.m_LastMousePosition.Y - this.m_AreaToolSelection.Height;
        if (this.toolSelectionAreaChanged != null)
          this.toolSelectionAreaChanged();
        this.UpdatePictureBoxImage();
      }
      else if (this.m_bZooming)
        this.Zoom((int) ((double) -_this.Y * 1.25));
      if (this.m_Cursor == null)
        return;
      this.m_Cursor.OnMouseMove(_sender, _args);
    }

    private void OnMouseWheel(object _sender, MouseEventArgs _args) => this.Zoom(_args.Delta / 120);

    private void OnResize(object _sender, EventArgs _args)
    {
      if (this.m_PictureBox.Width <= 0 || this.m_PictureBox.Height <= 0)
        return;
      float num1 = (float) this.m_PictureBox.Width / (float) this.m_LastPictureBoxSize.Width;
      float num2 = (float) this.m_PictureBox.Height / (float) this.m_LastPictureBoxSize.Height;
      this.m_ViewedArea.Width *= num1;
      this.m_ViewedArea.Height *= num2;
      this.m_LastPictureBoxSize = this.m_PictureBox.Size;
      if (this.m_PictureBox.Image.Width < this.m_PictureBox.Width || this.m_PictureBox.Image.Height < this.m_PictureBox.Height)
      {
        if (this.m_PictureBox.Image != null)
          this.m_PictureBox.Image.Dispose();
        this.m_PictureBox.Image = (Image) new Bitmap(this.m_PictureBox.Width, this.m_PictureBox.Height);
      }
      this.UpdatePictureBoxImage();
    }

    private void ClampViewedArea()
    {
      this.m_ViewedArea.Width = this.m_ViewedArea.Width.Clamp<float>(16f, 32000f);
      this.m_ViewedArea.Height = this.m_ViewedArea.Height.Clamp<float>(16f, 32000f);
    }

    public delegate void ViewportChangedHandler();

    public delegate void ZoomChangedHandler(float _oldRatio, float _newRatio);
  }
}
