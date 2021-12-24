// Decompiled with JetBrains decompiler
// Type: RoomEditor.Selection
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  public class Selection
  {
    private HashSet<MarkerInfo> m_SelectedMarkers = new HashSet<MarkerInfo>();
    private Rectangle m_Area = Rectangle.Empty;

    public Layer.Type layerType { get; private set; }

    public IEnumerable<MarkerInfo> markers => (IEnumerable<MarkerInfo>) this.m_SelectedMarkers;

    public bool isEmpty => !this.hasMarkers && !this.hasTiles;

    public bool hasMarkers => (uint) this.m_SelectedMarkers.Count > 0U;

    public bool hasTiles => (uint) this.tiles.Count > 0U;

    public event Selection.MarkerSelectionChangedHandler markersChanged;

    public event Selection.ChangedHandler changed;

    public List<TileInfo> tiles { get; private set; } = new List<TileInfo>();

    public bool moved { get; private set; }

    public Rectangle area => this.m_Area;

    public void Reset()
    {
      if (this.m_SelectedMarkers.Count > 0)
      {
        this.m_SelectedMarkers.Clear();
        this.OnMarkerSelectionChanged();
      }
      if (this.tiles.Count > 0)
      {
        this.tiles.Clear();
        this.OnChanged();
      }
      this.moved = false;
      this.m_Area = Rectangle.Empty;
    }

    public int AddMarker(MarkerInfo _marker)
    {
      int num = 0;
      if (!this.m_SelectedMarkers.Add(_marker))
        return num;
      this.UpdateArea(_marker.rect.Multiplied(16));
      this.OnMarkerSelectionChanged();
      return num;
    }

    public void AddMarkers(List<MarkerInfo> _markers, ref Rectangle _areaUpdated)
    {
      this.m_SelectedMarkers.Add(_markers[0]);
      _areaUpdated = _markers[0].rect.Multiplied(16);
      for (int index = 1; index < _markers.Count; ++index)
      {
        if (this.m_SelectedMarkers.Add(_markers[index]))
          _areaUpdated = Rectangle.Union(_areaUpdated, _markers[index].rect.Multiplied(16));
      }
      this.UpdateArea(_areaUpdated);
      this.OnMarkerSelectionChanged();
    }

    public void SetTile(Point _position, int _value)
    {
      this.Reset();
      if (_value == 0)
        return;
      this.tiles.Add(new TileInfo()
      {
        position = _position,
        value = _value
      });
      this.UpdateArea(new Rectangle(_position, new Size(16, 16)));
    }

    public void SetTiles(List<TileInfo> _tiles)
    {
      this.Reset();
      this.tiles.AddRange((IEnumerable<TileInfo>) _tiles);
      int x = int.MaxValue;
      int num1 = int.MinValue;
      int y = int.MaxValue;
      int num2 = int.MinValue;
      foreach (TileInfo tile in _tiles)
      {
        if (tile.position.X < x)
          x = tile.position.X;
        if (tile.position.X + 16 > num1)
          num1 = tile.position.X + 16;
        if (tile.position.Y < y)
          y = tile.position.Y;
        if (tile.position.Y + 16 > num2)
          num2 = tile.position.Y + 16;
      }
      this.UpdateArea(new Rectangle(x, y, num1 - x, num2 - y));
      this.OnChanged();
    }

    public void ClearMarkers()
    {
      this.m_SelectedMarkers.Clear();
      this.OnMarkerSelectionChanged();
    }

    public List<Point> RetrievePositions(out List<Point> _positions)
    {
      _positions = new List<Point>();
      foreach (TileInfo tile in this.tiles)
        _positions.Add(tile.position);
      return _positions;
    }

    public Point Move(Point _delta, Rectangle _clampingArea)
    {
      this.moved = true;
      if (_delta.X + this.area.Left <= _clampingArea.Left)
        _delta.X = _clampingArea.Left - this.area.Left;
      else if (_delta.X + this.area.Right > _clampingArea.Right)
        _delta.X = _clampingArea.Right - this.area.Right;
      if (_delta.Y + this.area.Top <= _clampingArea.Top)
        _delta.Y = _clampingArea.Top - this.area.Top;
      else if (_delta.Y + this.area.Bottom > _clampingArea.Bottom)
        _delta.Y = _clampingArea.Bottom - this.area.Bottom;
      for (int index = 0; index < this.tiles.Count; ++index)
      {
        TileInfo tile = this.tiles[index];
        tile.position.Offset(_delta);
        this.tiles[index] = tile;
      }
      List<MarkerInfo> markerInfoList = new List<MarkerInfo>();
      markerInfoList.AddRange(this.markers);
      for (int index = 0; index < markerInfoList.Count; ++index)
      {
        Rectangle rect = markerInfoList[index].rect;
        rect.Offset(_delta.Divided(16));
        markerInfoList[index].rect = rect;
      }
      this.m_Area.Offset(_delta);
      return _delta;
    }

    private void UpdateArea(Rectangle _areaAdded)
    {
      if (this.area == Rectangle.Empty)
        this.m_Area = _areaAdded;
      else
        this.m_Area = Rectangle.Union(_areaAdded, this.area);
    }

    private void OnMarkerSelectionChanged()
    {
      if (this.markersChanged != null)
        this.markersChanged();
      this.OnChanged();
    }

    private void OnChanged()
    {
      if (this.changed == null)
        return;
      this.changed();
    }

    public delegate void MarkerSelectionChangedHandler();

    public delegate void ChangedHandler();
  }
}
