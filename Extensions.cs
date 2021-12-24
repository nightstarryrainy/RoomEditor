// Decompiled with JetBrains decompiler
// Type: RoomEditor.Extensions
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  internal static class Extensions
  {
    public static T Clamp<T>(this T _value, T _min, T _max) where T : IComparable
    {
      if (_value.CompareTo((object) _min) <= 0)
        return _min;
      return _value.CompareTo((object) _max) <= 0 || _max.CompareTo((object) _min) <= 0 ? _value : _max;
    }

    public static bool ContainsPoint(this List<Point> _this, Point _value)
    {
      int index = 0;
      while (index < _this.Count && (_this[index].X != _value.X || _this[index].Y != _value.Y))
        ++index;
      return index < _this.Count;
    }

    public static Rectangle GetBoundingRectangle(this List<Point> _this)
    {
      int x = int.MaxValue;
      int num1 = int.MinValue;
      int y = int.MaxValue;
      int num2 = int.MinValue;
      foreach (Point thi in _this)
      {
        if (thi.X < x)
          x = thi.X;
        if (thi.X > num1)
          num1 = thi.X;
        if (thi.Y < y)
          y = thi.Y;
        if (thi.Y > num2)
          num2 = thi.Y;
      }
      return new Rectangle(x, y, num1 - x, num2 - y);
    }

    public static float GetArea(this RectangleF _this) => _this.Width * _this.Height;

    public static int GetArea(this Rectangle _this) => _this.Width * _this.Height;

    public static Rectangle Multiplied(this Rectangle _this, int _factor)
    {
      Rectangle rectangle = _this;
      rectangle.X *= _factor;
      rectangle.Y *= _factor;
      rectangle.Width *= _factor;
      rectangle.Height *= _factor;
      return rectangle;
    }

    public static Point Snapped(this Point _this, int _gridSize) => _this.Divided(_gridSize).Multiplied(_gridSize);

    public static Point Multiplied(this Point _this, int _factor)
    {
      Point point = _this;
      point.X *= _factor;
      point.Y *= _factor;
      return point;
    }

    public static PointF Multiplied(this PointF _this, float _factor)
    {
      PointF pointF = _this;
      pointF.X *= _factor;
      pointF.Y *= _factor;
      return pointF;
    }

    public static Rectangle Divided(this Rectangle _this, int _factor)
    {
      Rectangle rectangle = _this;
      rectangle.X /= _factor;
      rectangle.Y /= _factor;
      rectangle.Width /= _factor;
      rectangle.Height /= _factor;
      return rectangle;
    }

    public static Point Divided(this Point _this, int _factor)
    {
      Point point = _this;
      point.X /= _factor;
      point.Y /= _factor;
      return point;
    }

    public static RectangleF Multiplied(this RectangleF _this, float _factor)
    {
      RectangleF rectangleF = _this;
      rectangleF.X *= _factor;
      rectangleF.Y *= _factor;
      rectangleF.Width *= _factor;
      rectangleF.Height *= _factor;
      return rectangleF;
    }

    public static Rectangle Scaled(this Rectangle _this, int _factor)
    {
      Rectangle rectangle = _this;
      rectangle.Width *= _factor;
      rectangle.Height *= _factor;
      return rectangle;
    }

    public static RectangleF Scaled(this RectangleF _this, float _factor)
    {
      RectangleF rectangleF = _this;
      rectangleF.Width *= _factor;
      rectangleF.Height *= _factor;
      return rectangleF;
    }

    public static void ClampToScreen(this Form _this)
    {
      Screen screen = Screen.FromControl((Control) _this);
      if (screen.Bounds.Width < _this.Width)
        _this.Width = screen.Bounds.Width;
      if (screen.Bounds.Height < _this.Height)
        _this.Height = screen.Bounds.Height;
      int top1 = _this.Top;
      Rectangle bounds = screen.Bounds;
      int top2 = bounds.Top;
      if (top1 < top2)
      {
        Form form = _this;
        bounds = screen.Bounds;
        int top3 = bounds.Top;
        form.Top = top3;
      }
      int left1 = _this.Left;
      bounds = screen.Bounds;
      int left2 = bounds.Left;
      if (left1 < left2)
      {
        Form form = _this;
        bounds = screen.Bounds;
        int left3 = bounds.Left;
        form.Left = left3;
      }
      int num1 = _this.Top + _this.Height;
      bounds = screen.Bounds;
      int bottom = bounds.Bottom;
      if (num1 > bottom)
      {
        Form form = _this;
        bounds = screen.Bounds;
        int num2 = bounds.Bottom - _this.Height;
        form.Top = num2;
      }
      int num3 = _this.Left + _this.Width;
      bounds = screen.Bounds;
      int right = bounds.Right;
      if (num3 <= right)
        return;
      Form form1 = _this;
      bounds = screen.Bounds;
      int num4 = bounds.Right - _this.Width;
      form1.Left = num4;
    }
  }
}
