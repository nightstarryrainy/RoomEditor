// Decompiled with JetBrains decompiler
// Type: RoomEditor.Layers
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RoomEditor
{
  public class Layers
  {
    private List<Layer> m_Layers = new List<Layer>();
    private Layer m_CurrentLayer;

    public Layer currentLayer
    {
      get => this.m_CurrentLayer;
      set
      {
        if (this.m_CurrentLayer == value)
          return;
        this.m_CurrentLayer = value;
        if (this.currentLayerChanged == null)
          return;
        this.currentLayerChanged();
      }
    }

    public List<Layer> layers => this.m_Layers;

    public int width { get; private set; }

    public int height { get; private set; }

    public bool allowMarkerMultiSelection { get; set; }

    public bool hasSelection => !this.selection.isEmpty;

    public MarkerLayer markerLayer { get; private set; }

    public RoomLayer collisionLayer { get; private set; }

    public RoomLayer linkLayer { get; private set; }

    public event Layers.LayerCreatedHandler layerCreated;

    public event Layers.LayersEventHandler dataChanged;

    public event Layers.LayersEventHandler currentLayerChanged;

    public event Layers.BeforeResetSelectionHandler beforeResetSelection;

    public Selection selection { get; private set; } = new Selection();

    public static Layers CreateLayers(JObject _roomObj)
    {
      Layers layers = new Layers(int.Parse(_roomObj["width"].ToString()), int.Parse(_roomObj["height"].ToString()));
      layers.collisionLayer = layers.CreateLayer(Layer.Type.Collision, "Collisions").SetData(_roomObj["layers"][(object) 0][(object) "data"][(object) "data"]) as RoomLayer;
      layers.linkLayer = layers.CreateLayer(Layer.Type.Link, "Links").SetData(_roomObj["layers"][(object) 1][(object) "data"][(object) "data"]) as RoomLayer;
      layers.markerLayer = layers.CreateLayer(Layer.Type.Marker, "Markers").SetData(_roomObj["markers"]) as MarkerLayer;
      layers.currentLayer = layers.GetLayerForType(Layer.Type.Collision);
      return layers;
    }

    public Layer CreateLayer(Layer.Type _type, string _name)
    {
      Layer _createdLayer = _type != Layer.Type.Marker ? (Layer) new RoomLayer(_type, _name, this.width, this.height) : (Layer) new MarkerLayer(_name, this.width, this.height);
      this.m_Layers.Add(_createdLayer);
      if (this.currentLayer == null && Tools.currentTool != null && Tools.currentTool.type == _type)
        this.currentLayer = _createdLayer;
      if (this.layerCreated != null)
        this.layerCreated(_createdLayer);
      return _createdLayer;
    }

    public void ResetSelection(bool _bRestoreSelectedTiles = true)
    {
      if (this.beforeResetSelection != null)
        this.beforeResetSelection(_bRestoreSelectedTiles);
      if (_bRestoreSelectedTiles)
      {
        foreach (TileInfo tile in this.selection.tiles)
        {
          Layer.Type _forcedLayer = Layer.Type.None;
          Rectangle empty = Rectangle.Empty;
          this.PlaceBlock(tile.position, tile.value, ref _forcedLayer, ref empty);
        }
      }
      this.selection.Reset();
    }

    public Rectangle RemoveSelectedMarkers()
    {
      Rectangle a = Rectangle.Empty;
      if (this.selection.hasMarkers)
      {
        foreach (MarkerInfo marker in this.selection.markers)
        {
          a = !a.IsEmpty ? Rectangle.Union(a, marker.rect) : marker.rect;
          this.markerLayer.RemoveMarker(marker);
        }
        this.selection.ClearMarkers();
      }
      return a;
    }

    public List<int> PlaceBlocks(
      List<Point> _positions,
      List<int> _values,
      ref Layer.Type _forcedLayer,
      ref Rectangle _areaUpdated)
    {
      bool flag = false;
      List<int> intList = new List<int>();
      for (int index = 0; index < _positions.Count; ++index)
      {
        Rectangle empty = Rectangle.Empty;
        int num = this.PlaceBlockImpl(_positions[index], _values[index], ref _forcedLayer, ref empty, true);
        if (!empty.IsEmpty)
          _areaUpdated = !_areaUpdated.IsEmpty ? Rectangle.Union(empty, _areaUpdated) : empty;
        intList.Add(num);
        flag = flag || num != _values[index];
      }
      if (flag)
        this.DataChanged();
      return intList;
    }

    public List<int> PlaceBlocks(
      List<Point> _positions,
      int _value,
      ref Layer.Type _forcedLayer,
      ref Rectangle _areaUpdated)
    {
      List<int> _values = new List<int>(Enumerable.Repeat<int>(_value, _positions.Count));
      return this.PlaceBlocks(_positions, _values, ref _forcedLayer, ref _areaUpdated);
    }

    public int PlaceBlock(
      Point _position,
      int _value,
      ref Layer.Type _forcedLayer,
      ref Rectangle _areaUpdated)
    {
      return this.PlaceBlockImpl(_position, _value, ref _forcedLayer, ref _areaUpdated, false);
    }

    public MarkerInfo PlaceMarker(
      Rectangle _rectangle,
      int _value,
      ref Rectangle _areaUpdated)
    {
      MarkerInfo markerInfo = this.markerLayer.SetRectangle(_rectangle.Divided(16), _value);
      _areaUpdated = _rectangle;
      this.DataChanged();
      return markerInfo;
    }

    public MarkerInfo PlaceMarker(MarkerInfo _markerInfo)
    {
      this.markerLayer.AddMarker(_markerInfo);
      this.DataChanged();
      return _markerInfo;
    }

    public void SelectMarkersInArea(Rectangle _area, ref Rectangle _areaUpdated)
    {
      Rectangle rectangle = _area.Divided(16);
      HashSet<MarkerInfo> markerInfoSet = new HashSet<MarkerInfo>();
      for (int x = rectangle.X; x < rectangle.Right; ++x)
      {
        for (int y = rectangle.Y; y < rectangle.Bottom; ++y)
        {
          List<MarkerInfo> markersAt = this.markerLayer.GetMarkersAt(x, y);
          if (markersAt.Count > 0)
            markerInfoSet.Add(markersAt[markersAt.Count - 1]);
        }
      }
      if (markerInfoSet.Count <= 0)
        return;
      List<MarkerInfo> _markers = new List<MarkerInfo>();
      _markers.AddRange((IEnumerable<MarkerInfo>) markerInfoSet);
      _areaUpdated = _markers[0].rect;
      this.selection.AddMarkers(_markers, ref _areaUpdated);
      _areaUpdated = _areaUpdated.Multiplied(16);
    }

    public void SelectBlock(Point _position, ref Rectangle _areaUpdated)
    {
      _position.X /= 16;
      _position.Y /= 16;
      this.SelectBlockImpl(_position, ref _areaUpdated);
    }

    private void SelectBlockImpl(Point _position, ref Rectangle _areaUpdated)
    {
      Layer layer = this.currentLayer;
      List<int> valuesAt = layer.GetValuesAt(_position.X, _position.Y);
      if (valuesAt.Count == 0 || valuesAt[0] == 0)
      {
        int num = this.m_Layers.Count - 1;
        int index1 = 0;
        int index2 = 0;
        while (index2 < this.m_Layers.Count && this.m_Layers[index2] != this.currentLayer)
          ++index2;
        do
        {
          if (index1 == index2)
            ++index1;
          if (index1 == -1)
            index1 = 0;
          layer = this.m_Layers[index1];
          valuesAt = layer.GetValuesAt(_position.X, _position.Y);
          --num;
          ++index1;
        }
        while ((valuesAt.Count == 0 || valuesAt[0] == 0) && num > 0);
      }
      if (valuesAt.Count == 0 || valuesAt[0] == 0)
        return;
      this.currentLayer = layer;
      if (this.currentLayer.type != Layer.Type.Marker)
      {
        this.selection.SetTile(_position.Multiplied(16), valuesAt[0]);
        _areaUpdated = new Rectangle(_position.X, _position.Y, 1, 1);
      }
      else
      {
        List<MarkerInfo> markersAt = this.markerLayer.GetMarkersAt(_position.X, _position.Y);
        MarkerInfo _marker = markersAt[markersAt.Count - 1];
        this.selection.AddMarker(_marker);
        _areaUpdated = _marker.rect;
      }
      _areaUpdated = _areaUpdated.Multiplied(16);
    }

    public void SelectArea(Rectangle _area, ref Rectangle _areaUpdated)
    {
      Rectangle rectangle = Rectangle.Intersect(_area.Divided(16), new Rectangle(0, 0, this.width, this.height));
      if (rectangle.Width == 1 && rectangle.Height == 1)
      {
        this.SelectBlockImpl(rectangle.Location, ref _areaUpdated);
      }
      else
      {
        List<TileInfo> _tiles = new List<TileInfo>();
        for (int left = rectangle.Left; left < rectangle.Right; ++left)
        {
          for (int top = rectangle.Top; top < rectangle.Bottom; ++top)
          {
            int num = this.currentLayer.GetValuesAt(left, top)[0];
            if (num != 0)
              _tiles.Add(new TileInfo()
              {
                position = new Point(left, top).Multiplied(16),
                value = num
              });
          }
        }
        this.selection.SetTiles(_tiles);
      }
      _areaUpdated = _area;
      Rectangle empty = Rectangle.Empty;
      Layer.Type _forcedLayer = Layer.Type.None;
      this.PlaceBlockRect(_area, 0, out List<int> _, ref _forcedLayer, ref empty);
    }

    public bool PlaceBlockRect(
      Rectangle _rectangle,
      int _value,
      out List<int> _oldValues,
      ref Layer.Type _forcedLayer,
      ref Rectangle _areaUpdated)
    {
      Layer layerForValueOrType = this.GetLayerForValueOrType(_value, ref _forcedLayer);
      _rectangle = _rectangle.Divided(16);
      _oldValues = new List<int>();
      Rectangle _rectangle1 = _rectangle;
      int num = _value;
      ref List<int> local = ref _oldValues;
      if (!layerForValueOrType.SetRectangle(_rectangle1, num, out local))
        return false;
      this.DataChanged();
      return true;
    }

    public List<MarkerInfo> GetMarkersAt(Point _position)
    {
      _position.X /= 16;
      _position.Y /= 16;
      return this.markerLayer.GetMarkersAt(_position.X, _position.Y);
    }

    public void RetrieveValues(
      Point _position,
      out List<int> _valueList,
      out List<Layer> _layers,
      Layer.Type _layerType = Layer.Type.None)
    {
      _valueList = new List<int>();
      _layers = new List<Layer>();
      _position.X /= 16;
      _position.Y /= 16;
      foreach (Layer layer in this.m_Layers)
      {
        if ((layer.type == _layerType || _layerType == Layer.Type.None) && (this.height > _position.Y && this.width > _position.X))
        {
          List<int> valuesAt = layer.GetValuesAt(_position.X, _position.Y);
          if (valuesAt.Count != 0)
          {
            foreach (int num in valuesAt)
            {
              if (num != 0)
              {
                _valueList.Add(num);
                _layers.Add(layer);
              }
            }
          }
        }
      }
    }

    private Layer GetLayerForType(Layer.Type _type)
    {
      int index = 0;
      while (index < this.m_Layers.Count && this.m_Layers[index].type != _type)
        ++index;
      return index < this.m_Layers.Count ? this.m_Layers[index] : (Layer) null;
    }

    public void Resize(int _newWidth, int _newHeight)
    {
      this.width = _newWidth;
      this.height = _newHeight;
      foreach (Layer layer in this.m_Layers)
        layer.Resize(_newWidth, _newHeight);
      this.DataChanged();
    }

    public Layer GetLayerForValue(int _value)
    {
      Layer.Type _type;
      if (_value >= 1024)
        _type = Layer.Type.Marker;
      else if (_value >= 64)
      {
        _type = Layer.Type.Link;
      }
      else
      {
        if (_value < 32)
          return this.currentLayer;
        _type = Layer.Type.Collision;
      }
      return this.GetLayerForType(_type);
    }

    private int PlaceBlockImpl(
      Point _position,
      int _value,
      ref Layer.Type _forcedLayer,
      ref Rectangle _areaUpdated,
      bool _bulk)
    {
      Layer layerForValueOrType = this.GetLayerForValueOrType(_value, ref _forcedLayer);
      _position.X /= 16;
      _position.Y /= 16;
      int x = _position.X;
      int y = _position.Y;
      int num1 = _value;
      int num2 = layerForValueOrType.SetValue(x, y, num1);
      if (_value != num2)
      {
        _areaUpdated.X = _position.X * 16;
        _areaUpdated.Y = _position.Y * 16;
        _areaUpdated.Width = 16;
        _areaUpdated.Height = 16;
        if (!_bulk)
          this.DataChanged();
      }
      return num2;
    }

    private void DataChanged()
    {
      if (this.dataChanged == null)
        return;
      this.dataChanged();
    }

    private Layer GetLayerForValueOrType(int _value, ref Layer.Type _forcedLayer)
    {
      Layer layer = _forcedLayer != Layer.Type.None ? this.GetLayerForType(_forcedLayer) : this.GetLayerForValue(_value);
      _forcedLayer = layer.type;
      return layer;
    }

    private Layers(int _width, int _height)
    {
      this.width = _width;
      this.height = _height;
      Tools.currentToolChanged += new Tools.ToolChangedHandler(this.OnToolChanged);
      if (Tools.currentTool == null)
        return;
      this.OnToolChanged((Tool) null, Tools.currentTool);
    }

    private void OnToolChanged(Tool _oldTool, Tool _newTool)
    {
      if (this.currentLayer != null && (this.currentLayer.type == _newTool.type || _newTool.type == Layer.Type.None))
        return;
      this.currentLayer = this.GetLayerForType(_newTool.type);
    }

    public delegate void LayerCreatedHandler(Layer _createdLayer);

    public delegate void LayersEventHandler();

    public delegate void BeforeResetSelectionHandler(bool _bRestoreSelectedTile);
  }
}
