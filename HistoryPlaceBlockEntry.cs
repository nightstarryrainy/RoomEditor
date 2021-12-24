// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryPlaceBlockEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Drawing;

namespace RoomEditor
{
  internal class HistoryPlaceBlockEntry : HistoryEntry
  {
    private int x;
    private int y;
    private int nextValue;
    private int previousValue;
    private Layer.Type layerType;
    private Layers layers;
    private const string m_Name = "Place block";

    public override string name => "Place block of " + Tools.GetToolForValue(this.nextValue).name;

    public HistoryPlaceBlockEntry(
      int _x,
      int _y,
      int _previousValue,
      int _nextValue,
      Layer.Type _layerType,
      Layers _layers)
    {
      this.x = _x;
      this.y = _y;
      this.nextValue = _nextValue;
      this.previousValue = _previousValue;
      this.layerType = _layerType;
      this.layers = _layers;
    }

    public override void Undo()
    {
      Rectangle empty = Rectangle.Empty;
      this.layers.PlaceBlock(new Point(this.x, this.y), this.previousValue, ref this.layerType, ref empty);
    }

    public override void Redo()
    {
      Rectangle empty = Rectangle.Empty;
      this.layers.PlaceBlock(new Point(this.x, this.y), this.nextValue, ref this.layerType, ref empty);
    }
  }
}
