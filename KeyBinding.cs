// Decompiled with JetBrains decompiler
// Type: RoomEditor.KeyBinding
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

namespace RoomEditor
{
  internal class KeyBinding
  {
    public HotKey hotKey { get; private set; }

    public Action action { get; private set; }

    public KeyBinding(Action _action, HotKey _hotKey)
    {
      this.hotKey = _hotKey;
      this.action = _action;
    }
  }
}
