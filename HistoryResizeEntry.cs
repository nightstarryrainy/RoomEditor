// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryResizeEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

namespace RoomEditor
{
  internal class HistoryResizeEntry : HistoryEntry
  {
    private int previousWidth;
    private int previousHeight;
    private int nextWidth;
    private int nextHeight;
    private Viewport viewport;
    private const string m_Name = "Resize room";

    public override string name => "Resize room to " + (object) this.nextWidth + " x " + (object) this.nextHeight;

    public HistoryResizeEntry(
      int _previousWidth,
      int _previousHeight,
      int _nextWidth,
      int _nextHeight,
      Viewport _viewport)
    {
      this.previousWidth = _previousWidth;
      this.previousHeight = _previousHeight;
      this.nextWidth = _nextWidth;
      this.nextHeight = _nextHeight;
      this.viewport = _viewport;
    }

    public override void Undo() => this.viewport.Resize(this.previousWidth, this.previousHeight, true);

    public override void Redo() => this.viewport.Resize(this.nextWidth, this.nextHeight, true);
  }
}
