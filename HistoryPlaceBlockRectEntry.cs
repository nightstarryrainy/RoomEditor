// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryPlaceBlockRectEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  internal class HistoryPlaceBlockRectEntry : HistoryEntry
  {
    private Rectangle m_Rectangle;
    private List<int> m_OldValues = new List<int>();
    private int m_NewValue;
    private Layer.Type m_LayerType;
    private Layers m_Layers;
    private const string m_Name = "Place block rectangle";

    public override string name => "Place block rectangle of " + Tools.GetToolForValue(this.m_NewValue).name;

    public HistoryPlaceBlockRectEntry(
      Rectangle _rect,
      List<int> _oldValues,
      int _newValue,
      Layer.Type _layerType,
      Layers _layers)
    {
      this.m_Rectangle = _rect;
      this.m_OldValues.AddRange((IEnumerable<int>) _oldValues);
      this.m_NewValue = _newValue;
      this.m_LayerType = _layerType;
      this.m_Layers = _layers;
    }

    public override void Undo()
    {
      Rectangle empty = Rectangle.Empty;
      int right = this.m_Rectangle.Right;
      int bottom = this.m_Rectangle.Bottom;
      int num = 0;
      List<Point> _positions = new List<Point>();
      for (int x = this.m_Rectangle.X; x < right; x += 16)
      {
        int y = this.m_Rectangle.Y;
        while (y < bottom)
        {
          _positions.Add(new Point(x, y));
          y += 16;
          ++num;
        }
      }
      this.m_Layers.PlaceBlocks(_positions, this.m_OldValues, ref this.m_LayerType, ref empty);
    }

    public override void Redo()
    {
      Rectangle empty = Rectangle.Empty;
      this.m_Layers.PlaceBlockRect(this.m_Rectangle, this.m_NewValue, out List<int> _, ref this.m_LayerType, ref empty);
    }
  }
}
