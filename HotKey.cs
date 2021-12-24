// Decompiled with JetBrains decompiler
// Type: RoomEditor.HotKey
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Windows.Forms;

namespace RoomEditor
{
  internal class HotKey
  {
    private static KeysConverter keyConverter = new KeysConverter();

    public Keys modifiers { get; set; }

    public Keys key { get; set; }

    public MouseButtons mouseButtons { get; set; }

    public bool isValid => this.key != Keys.None || (uint) this.mouseButtons > 0U;

    public int complexityScore
    {
      get
      {
        int num = -1;
        if (this.isValid)
        {
          num = 0;
          if ((this.modifiers & Keys.Control) != Keys.None)
            ++num;
          if ((this.modifiers & Keys.Shift) != Keys.None)
            ++num;
          if ((this.modifiers & Keys.Alt) != Keys.None)
            ++num;
        }
        return num;
      }
    }

    public HotKey(Keys _modifiers = Keys.None, Keys _key = Keys.None, MouseButtons _mouseButtons = MouseButtons.None)
    {
      this.modifiers = _modifiers;
      this.key = _key;
      this.mouseButtons = _mouseButtons;
    }

    public override string ToString()
    {
      string str = "";
      if (this.isValid)
      {
        if (this.modifiers != Keys.None)
          str = string.Format("{0} + ", (object) string.Join(" + ", (object) this.modifiers));
        if (this.mouseButtons != MouseButtons.None)
          str += string.Format("Mouse {0}", (object) string.Join(" + Mouse ", (object) this.mouseButtons));
        else
          str += HotKey.keyConverter.ConvertToString((object) this.key);
      }
      return str;
    }

    public override bool Equals(object obj)
    {
      HotKey hotKey = (HotKey) obj;
      return this.modifiers == hotKey.modifiers && this.key == hotKey.key && this.mouseButtons == hotKey.mouseButtons;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
