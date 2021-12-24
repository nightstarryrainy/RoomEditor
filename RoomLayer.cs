// Decompiled with JetBrains decompiler
// Type: RoomEditor.RoomLayer
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  public class RoomLayer : Layer
  {
    public event RoomLayer.DataChangedHandler dataChanged;

    private RoomData roomData { get; set; }

    public RoomLayer(Layer.Type _type, string _name, int _width, int _height)
      : base(_type, _name, _width, _height)
    {
      this.roomData = new RoomData(_width, _height);
    }

    public override Layer SetData(JToken _data)
    {
      byte[] numArray = Convert.FromBase64String(_data.ToString());
      if (numArray.Length != this.roomData.height * this.roomData.width * 2)
        throw new Exception("Error in data size");
      int num1 = 0;
      for (int _y = 0; _y < this.roomData.height; ++_y)
      {
        int _x = 0;
        while (_x < this.roomData.width)
        {
          int num2 = (int) numArray[num1 * 2] | (int) numArray[num1 * 2 + 1] << 8;
          if (num2 != 0)
            num2 += (int) this.type;
          this.roomData[_x, _y] = num2;
          ++_x;
          ++num1;
        }
      }
      return (Layer) this;
    }

    public override string getData()
    {
      byte[] inArray = new byte[this.roomData.height * this.roomData.width * 2];
      int num1 = 0;
      for (int _y = 0; _y < this.roomData.height; ++_y)
      {
        int _x = 0;
        while (_x < this.roomData.width)
        {
          int num2 = this.roomData[_x, _y];
          if (num2 != 0)
            num2 -= (int) this.type;
          inArray[num1 * 2] = (byte) (num2 & (int) byte.MaxValue);
          inArray[num1 * 2 + 1] = (byte) ((num2 & 65280) >> 8);
          ++_x;
          ++num1;
        }
      }
      return Convert.ToBase64String(inArray);
    }

    public override bool SetRectangle(Rectangle _rectangle, int _value, out List<int> _oldValues)
    {
      int right = _rectangle.Right;
      int bottom = _rectangle.Bottom;
      bool flag = false;
      _oldValues = new List<int>();
      for (int x = _rectangle.X; x < right; ++x)
      {
        for (int y = _rectangle.Y; y < bottom; ++y)
        {
          int num = this.SetValue(x, y, _value);
          flag = flag || num != _value;
          _oldValues.Add(num);
        }
      }
      if (flag && this.dataChanged != null)
        this.dataChanged();
      return flag;
    }

    public List<Point> GetPositionsWithValue()
    {
      List<Point> pointList = new List<Point>();
      for (int index1 = 0; index1 < this.roomData.width; ++index1)
      {
        for (int index2 = 0; index2 < this.roomData.height; ++index2)
        {
          if (this.roomData[index1, index2] != 0)
            pointList.Add(new Point(index1, index2));
        }
      }
      return pointList;
    }

    public override void Resize(int _newWidth, int _newHeight) => this.roomData.Resize(_newWidth, _newHeight);

    public override void Reset() => this.roomData.Reset();

    protected override List<int> GetValuesAtImpl(int _x, int _y)
    {
      List<int> intList = new List<int>();
      if (_x < this.roomData.width && _y < this.roomData.height)
        intList.Add(this.roomData[_x, _y]);
      return intList;
    }

    protected override int SetValueImpl(int _x, int _y, int _value)
    {
      if (_x < 0 || _y < 0 || (_x >= this.roomData.width || _y >= this.roomData.height))
        return _value;
      int num = this.roomData[_x, _y];
      if (_value != num)
      {
        this.roomData[_x, _y] = _value;
        if (this.dataChanged != null)
          this.dataChanged();
      }
      return num;
    }

    public delegate void DataChangedHandler();
  }
}
