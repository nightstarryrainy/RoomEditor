// Decompiled with JetBrains decompiler
// Type: RoomEditor.ClipboardData
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;

namespace RoomEditor
{
  public class ClipboardData
  {
    public List<MarkerInfo> markerList { get; private set; } = new List<MarkerInfo>();

    public List<TileInfo> tileList { get; private set; } = new List<TileInfo>();

    public bool isEmpty => this.markerList.Count == 0 && this.tileList.Count == 0;

    public bool hasMarkerData => (uint) this.markerList.Count > 0U;

    public bool hasTileData => (uint) this.tileList.Count > 0U;

    public void SetData(IEnumerable<MarkerInfo> _markerListToCopy)
    {
      this.Clear();
      foreach (MarkerInfo markerInfo in _markerListToCopy)
        this.markerList.Add(markerInfo.Clone());
    }

    public void SetData(IEnumerable<TileInfo> _tileListToCopy)
    {
      this.Clear();
      this.tileList.AddRange(_tileListToCopy);
    }

    public void Clear()
    {
      this.markerList.Clear();
      this.tileList.Clear();
    }
  }
}
