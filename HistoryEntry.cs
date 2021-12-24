// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

namespace RoomEditor
{
  public abstract class HistoryEntry
  {
    public abstract string name { get; }

    public abstract void Undo();

    public abstract void Redo();
  }
}
