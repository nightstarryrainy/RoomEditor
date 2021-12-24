// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryPlaceMarkerEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

namespace RoomEditor
{
  internal class HistoryPlaceMarkerEntry : HistoryEntry
  {
    private MarkerInfo m_Marker;
    private MarkerLayer m_Layer;
    private const string m_Name = "Place marker";

    public override string name => "Place marker " + this.m_Marker.ToString();

    public HistoryPlaceMarkerEntry(MarkerInfo _marker, MarkerLayer _layer)
    {
      this.m_Marker = _marker;
      this.m_Layer = _layer;
    }

    public override void Undo() => this.m_Layer.RemoveMarker(this.m_Marker);

    public override void Redo() => this.m_Layer.AddMarker(this.m_Marker);
  }
}
