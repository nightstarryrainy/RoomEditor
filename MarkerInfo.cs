// Decompiled with JetBrains decompiler
// Type: RoomEditor.MarkerInfo
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Drawing;

namespace RoomEditor
{
  public class MarkerInfo
  {
    public Rectangle rect { get; set; }

    public int value => this.markerType.index + 1024;

    public string tag { get; set; }

    public MarkerType markerType { get; set; }

    public Color color { get; set; }

    public string item { get; set; }

    public string light { get; set; }

    public string mob { get; set; }

    public string level { get; set; }

    public string layer { get; set; }

    public int? dir { get; set; }

    public float? xr { get; set; }

    public float? yr { get; set; }

    public MarkerInfo(Marker _marker, MarkerType _markerType)
    {
      this.markerType = _markerType;
      this.rect = new Rectangle(_marker.x, _marker.y, _marker.width, _marker.height);
      this.color = Color.FromArgb((int) sbyte.MaxValue, _markerType.color >> 16, (_markerType.color & 65280) >> 8, _markerType.color & (int) byte.MaxValue);
      this.tag = _marker.customId;
      this.item = _marker.item;
      this.light = _marker.light;
      this.mob = _marker.mob;
      this.level = _marker.level;
      this.layer = _marker.layer;
      this.dir = _marker.dir;
      this.xr = _marker.xr;
      this.yr = _marker.yr;
    }

    public MarkerInfo(MarkerType _markerType, Rectangle _area)
    {
      this.markerType = _markerType;
      this.rect = _area;
      this.color = Color.FromArgb((int) sbyte.MaxValue, _markerType.color >> 16, (_markerType.color & 65280) >> 8, _markerType.color & (int) byte.MaxValue);
    }

    public Marker ToMarker() => new Marker()
    {
      x = this.rect.X,
      y = this.rect.Y,
      width = this.rect.Width,
      height = this.rect.Height,
      customId = this.tag,
      marker = this.markerType.id,
      item = this.item,
      light = this.light,
      mob = this.mob,
      level = this.level,
      layer = this.layer,
      dir = this.dir,
      xr = this.xr,
      yr = this.yr
    };

    public MarkerInfo Clone() => new MarkerInfo(this.ToMarker(), this.markerType);

    public override string ToString() => this.markerType.id;
  }
}
