// Decompiled with JetBrains decompiler
// Type: RoomEditor.RoomData
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;

namespace RoomEditor
{
  public class RoomData
  {
    private int[,] m_Data;
    private int m_WidthCapacity;
    private int m_HeightCapacity;

    public int height { get; private set; }

    public int width { get; private set; }

    public int this[int _x, int _y]
    {
      get => this.m_Data[_x, _y];
      set => this.m_Data[_x, _y] = value;
    }

    public RoomData(int _width, int _height)
    {
      this.m_Data = new int[_width, _height];
      this.height = _height;
      this.width = _width;
      this.m_WidthCapacity = this.width;
      this.m_HeightCapacity = this.height;
    }

    public void Reset() => Array.Clear((Array) this.m_Data, 0, this.m_Data.Length);

    public void Resize(int _newWidth, int _newHeight)
    {
      if (_newWidth > this.m_WidthCapacity || _newHeight > this.m_HeightCapacity)
      {
        int[,] numArray = new int[_newWidth, _newHeight];
        for (int index1 = 0; index1 < this.m_WidthCapacity; ++index1)
        {
          for (int index2 = 0; index2 < this.m_HeightCapacity; ++index2)
            numArray[index1, index2] = this.m_Data[index1, index2];
        }
        this.m_Data = numArray;
        this.m_WidthCapacity = Math.Max(this.m_WidthCapacity, _newWidth);
        this.m_HeightCapacity = Math.Max(this.m_HeightCapacity, _newHeight);
      }
      this.width = _newWidth;
      this.height = _newHeight;
    }
  }
}
