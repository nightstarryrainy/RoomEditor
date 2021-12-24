// Decompiled with JetBrains decompiler
// Type: RoomEditor.MarkerLayer
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using ModTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace RoomEditor
{
  public class MarkerLayer : Layer
  {
    private List<MarkerInfo> m_Markers = new List<MarkerInfo>();

    public MarkerInfo lastAddedMarker => this.m_Markers.Count <= 0 ? (MarkerInfo) null : this.m_Markers[this.m_Markers.Count - 1];

    public MarkerLayer(string _name, int _width, int _height)
      : base(Layer.Type.Marker, _name, _width, _height)
    {
      this.width = _width;
      this.height = _height;
    }

    public override Layer SetData(JToken _data)
    {
      if (MarkerType.loaded)
      {
        foreach (JObject jobject in (JArray) _data)
        {
          Marker _marker = JsonConvert.DeserializeObject<Marker>(jobject.ToString());
          MarkerType markerTypeById = MarkerType.GetMarkerTypeByID(_marker.marker);
          this.m_Markers.Add(new MarkerInfo(_marker, markerTypeById));
        }
      }
      return (Layer) this;
    }

    public override string getData()
    {
      List<string> stringList = new List<string>();
      StringWriter stringWriter = new StringWriter();
      CastleJsonTextWriter castleJsonTextWriter1 = new CastleJsonTextWriter((TextWriter) stringWriter);
      castleJsonTextWriter1.Formatting = Formatting.Indented;
      castleJsonTextWriter1.Indentation = 1;
      castleJsonTextWriter1.IndentChar = '\t';
      CastleJsonTextWriter castleJsonTextWriter2 = castleJsonTextWriter1;
      JsonSerializer jsonSerializer = new JsonSerializer();
      jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
      foreach (MarkerInfo marker1 in this.m_Markers)
      {
        Marker marker2 = marker1.ToMarker();
        if (marker2.x < this.width && marker2.y < this.height)
        {
          if (marker2.x + marker2.width > this.width)
            marker2.width = this.width - marker2.x;
          if (marker2.y + marker2.height > this.height)
            marker2.height = this.height - marker2.y;
          jsonSerializer.Serialize((JsonWriter) castleJsonTextWriter2, (object) marker2);
          string str = stringWriter.ToString().Replace("\r", "");
          StringBuilder stringBuilder = stringWriter.GetStringBuilder();
          stringBuilder.Remove(0, stringBuilder.Length);
          stringList.Add(str);
        }
      }
      castleJsonTextWriter2.Close();
      stringWriter.Close();
      return string.Format("[{0}]", (object) string.Join(",\r", (IEnumerable<string>) stringList));
    }

    public override void Reset() => this.m_Markers.Clear();

    public MarkerInfo SetRectangle(Rectangle _rectangle, int _value)
    {
      MarkerInfo _marker = new MarkerInfo(MarkerType.GetMarkerTypeByValue((int) (_value - this.type)), _rectangle);
      this.AddMarker(_marker);
      return _marker;
    }

    public override bool SetRectangle(Rectangle _rectangle, int _value, out List<int> _oldValues)
    {
      _oldValues = new List<int>();
      if (_value == 0)
        return false;
      _oldValues.AddRange(Enumerable.Repeat<int>(0, _rectangle.Width * _rectangle.Height));
      this.SetRectangle(_rectangle, _value);
      return true;
    }

    public List<MarkerInfo> GetMarkersAt(int _x, int _y)
    {
      List<MarkerInfo> markerInfoList = new List<MarkerInfo>();
      if (this.visible)
      {
        foreach (MarkerInfo marker in this.m_Markers)
        {
          if (marker.rect.Contains(_x, _y))
            markerInfoList.Add(marker);
        }
      }
      return markerInfoList;
    }

    public void AddMarker(MarkerInfo _marker)
    {
      if (this.locked)
        return;
      this.m_Markers.Add(_marker);
    }

    public void RemoveMarker(MarkerInfo _marker)
    {
      if (this.locked)
        return;
      this.m_Markers.Remove(_marker);
    }

    public override void Resize(int _newWidth, int _newHeight)
    {
      this.width = _newWidth;
      this.height = _newHeight;
    }

    protected override List<int> GetValuesAtImpl(int _x, int _y)
    {
      List<int> intList = new List<int>();
      foreach (MarkerInfo marker in this.m_Markers)
      {
        Rectangle rect = marker.rect;
        if (rect.Contains(_x, _y))
        {
          int num1 = marker.value;
          rect = marker.rect;
          int x = rect.X;
          rect = marker.rect;
          int num2 = rect.Width / 2;
          if (x + num2 == _x)
          {
            rect = marker.rect;
            int y = rect.Y;
            rect = marker.rect;
            int num3 = rect.Height / 2;
            if (y + num3 == _y)
              num1 |= 4096;
          }
          intList.Add(num1);
        }
      }
      return intList;
    }

    protected override int SetValueImpl(int _x, int _y, int _value)
    {
      if (_value != 0)
        this.AddMarker(new MarkerInfo(MarkerType.GetMarkerTypeByValue((int) (_value - this.type)), new Rectangle(_x, _y, 1, 1)));
      return 0;
    }

    private int width { get; set; }

    private int height { get; set; }
  }
}
