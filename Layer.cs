// Decompiled with JetBrains decompiler
// Type: RoomEditor.Layer
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  public abstract class Layer
  {
    public Layer.Type type { get; private set; }

    public string name { get; private set; }

    public bool visible { get; set; } = true;

    public bool locked { get; set; }

    public byte alpha { get; set; } = byte.MaxValue;

    public Layer(Layer.Type _type, string _name, int _width, int _height)
    {
      this.type = _type;
      this.name = _name;
    }

    public int SetValue(int _x, int _y, int _value) => this.locked ? _value : this.SetValueImpl(_x, _y, _value);

    public abstract bool SetRectangle(Rectangle _rectangle, int _value, out List<int> _oldValues);

    public List<int> GetValuesAt(int _x, int _y) => this.visible ? this.GetValuesAtImpl(_x, _y) : new List<int>();

    public override string ToString() => base.ToString();

    protected abstract List<int> GetValuesAtImpl(int _x, int _y);

    protected abstract int SetValueImpl(int _x, int _y, int _value);

    public abstract Layer SetData(JToken _data);

    public abstract string getData();

    public abstract void Reset();

    public abstract void Resize(int _newWidth, int _newHeight);

    public enum Type
    {
      None = 0,
      Collision = 32, // 0x00000020
      Link = 64, // 0x00000040
      Marker = 1024, // 0x00000400
    }
  }
}
