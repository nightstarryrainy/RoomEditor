// Decompiled with JetBrains decompiler
// Type: RoomEditor.Tool
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RoomEditor
{
  internal class Tool
  {
    private int m_Value;
    private static Dictionary<string, Bitmap> m_TextureCache = new Dictionary<string, Bitmap>();

    public int value => this.m_Value;

    public Layer.Type type { get; private set; }

    public Tools.Group group { get; private set; }

    public Bitmap bitmap { get; private set; }

    public string name { get; private set; }

    public bool isAreaTool => this.type == Layer.Type.Marker;

    public bool isMarker => this.type == Layer.Type.Marker;

    public Color areaColor { get; set; }

    public virtual bool doesPlaceBlocks => true;

    public Tool(
      int _value,
      Layer.Type _type,
      Tools.Group _group,
      string _name,
      string _texturePath,
      Point _coordinatesInTexture,
      int _size = 16)
      : this(_value, _type, _name, _group)
    {
      this.bitmap = new Bitmap(16, 16);
      Graphics graphics = Graphics.FromImage((Image) this.bitmap);
      if (File.Exists(_texturePath))
      {
        Bitmap bitmap;
        if (!Tool.m_TextureCache.TryGetValue(_texturePath, out bitmap))
        {
          bitmap = (Bitmap) Image.FromFile(_texturePath);
          Tool.m_TextureCache.Add(_texturePath, bitmap);
        }
        graphics.DrawImage((Image) bitmap, new Rectangle(0, 0, 16, 16), new Rectangle(_coordinatesInTexture.X * _size, _coordinatesInTexture.Y * _size, _size, _size), GraphicsUnit.Pixel);
      }
      else
        graphics.Clear(Color.Gray);
      graphics.Dispose();
      this.areaColor = this.bitmap.GetPixel(this.bitmap.Width / 2, this.bitmap.Height / 2);
    }

    public Tool(int _value, Layer.Type _type, Tools.Group _group, string _name, Bitmap _bitmap)
      : this(_value, _type, _name, _group)
    {
      this.bitmap = _bitmap.Clone(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), _bitmap.PixelFormat);
    }

    public override string ToString() => this.name;

    private Tool(int _value, Layer.Type _type, string _name, Tools.Group _group)
    {
      this.m_Value = (int) (_value + _type);
      this.type = _type;
      this.name = _name;
      this.group = _group;
    }
  }
}
