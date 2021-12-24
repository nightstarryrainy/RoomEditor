// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryMoveMarkerEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  internal class HistoryMoveMarkerEntry : HistoryEntry
  {
    private List<MarkerInfo> m_Markers = new List<MarkerInfo>();
    private const string m_Name = "Move marker";

    public override string name => "Move marker " + string.Join<MarkerInfo>(", ", (IEnumerable<MarkerInfo>) this.m_Markers);

    public Point moveDelta { get; set; }

    public HistoryMoveMarkerEntry(IEnumerable<MarkerInfo> _markers, Point _delta)
    {
      this.m_Markers.AddRange(_markers);
      this.moveDelta = _delta;
    }

    public override void Redo()
    {
      for (int index = 0; index < this.m_Markers.Count; ++index)
      {
        Rectangle rect = this.m_Markers[index].rect;
        rect.Offset(this.moveDelta.Divided(16));
        this.m_Markers[index].rect = rect;
      }
    }

    public override void Undo()
    {
      for (int index = 0; index < this.m_Markers.Count; ++index)
      {
        Rectangle rect = this.m_Markers[index].rect;
        rect.Offset(this.moveDelta.Multiplied(-1).Divided(16));
        this.m_Markers[index].rect = rect;
      }
    }
  }
}
