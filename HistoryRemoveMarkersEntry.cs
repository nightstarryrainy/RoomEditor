// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryRemoveMarkersEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;

namespace RoomEditor
{
  internal class HistoryRemoveMarkersEntry : HistoryEntry
  {
    private List<MarkerInfo> m_Markers;
    private MarkerLayer m_Layer;
    private const string m_Name = "Remove marker";

    public override string name => "Remove marker " + string.Join<MarkerInfo>(", ", (IEnumerable<MarkerInfo>) this.m_Markers);

    public HistoryRemoveMarkersEntry(List<MarkerInfo> _markers, MarkerLayer _layer)
    {
      this.m_Markers = _markers;
      this.m_Layer = _layer;
    }

    public override void Undo()
    {
      foreach (MarkerInfo marker in this.m_Markers)
        this.m_Layer.AddMarker(marker);
    }

    public override void Redo()
    {
      foreach (MarkerInfo marker in this.m_Markers)
        this.m_Layer.RemoveMarker(marker);
    }
  }
}
