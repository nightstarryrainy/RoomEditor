// Decompiled with JetBrains decompiler
// Type: RoomEditor.RoomRenderer
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace RoomEditor
{
  internal class RoomRenderer : IDisposable
  {
    private Layers m_Layers;

    public Bitmap target { get; private set; }

    public RoomRenderer(Layers _dataLayers)
    {
      this.m_Layers = _dataLayers;
      this.CreateTarget();
      this.Invalidate();
    }

    public void Invalidate() => this.InvalidateArea(new Rectangle(0, 0, this.target.Width, this.target.Height));

    public void InvalidateArea(RectangleF _areaToInvalidate)
    {
      if (_areaToInvalidate.IsEmpty)
        return;
      this.InvalidateArea(new Rectangle((int) _areaToInvalidate.X, (int) _areaToInvalidate.Y, (int) Math.Ceiling((double) _areaToInvalidate.Width), (int) Math.Ceiling((double) _areaToInvalidate.Height)));
    }

    public void InvalidateArea(Rectangle _areaToInvalidate)
    {
      _areaToInvalidate.X = 0;
      _areaToInvalidate.Y = 0;
      _areaToInvalidate.Width = this.target.Width;
      _areaToInvalidate.Height = this.target.Height;
      if (_areaToInvalidate.IsEmpty)
        return;
      Point point1 = new Point(_areaToInvalidate.Left / 16 * 16, _areaToInvalidate.Top / 16 * 16);
      Point point2 = new Point((_areaToInvalidate.Right / 16 + 1) * 16, (_areaToInvalidate.Bottom / 16 + 1) * 16);
      ColorMatrix newColorMatrix = new ColorMatrix(new float[5][]
      {
        new float[5]{ 1f, 0.0f, 0.0f, 0.0f, 0.0f },
        new float[5]{ 0.0f, 1f, 0.0f, 0.0f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 1f, 0.0f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 1f, 0.0f },
        new float[5]{ 0.0f, 0.0f, 0.0f, 0.0f, 1f }
      });
      ImageAttributes imageAttr1 = new ImageAttributes();
      imageAttr1.SetColorMatrix(newColorMatrix);
      Graphics graphics1 = Graphics.FromImage((Image) this.target);
      graphics1.FillRectangle(Brushes.DimGray, _areaToInvalidate);
      for (int x = point1.X; x < point2.X; x += 16)
      {
        for (int y = point1.Y; y < point2.Y; y += 16)
        {
          Point point3 = new Point(x, y);
          List<int> _valueList;
          List<Layer> _layers;
          this.m_Layers.RetrieveValues(point3, out _valueList, out _layers);
          for (int index = 0; index < _valueList.Count; ++index)
          {
            int num = _valueList[index];
            Tool toolForValue = Tools.GetToolForValue(num);
            if (toolForValue.isAreaTool)
            {
              Brush brush = (Brush) new SolidBrush(Color.FromArgb((int) _layers[index].alpha * (int) toolForValue.areaColor.A / (int) byte.MaxValue, toolForValue.areaColor));
              graphics1.FillRectangle(brush, new Rectangle(point3, new Size(16, 16)));
            }
            if (toolForValue != Tools.eraser && (!toolForValue.isAreaTool || (num & 4096) > 0))
            {
              newColorMatrix.Matrix33 = (float) _layers[index].alpha / (float) byte.MaxValue;
              imageAttr1.SetColorMatrix(newColorMatrix);
              graphics1.DrawImage((Image) toolForValue.bitmap, new Rectangle(point3.X, point3.Y, toolForValue.bitmap.Width, toolForValue.bitmap.Height), 0, 0, toolForValue.bitmap.Width, toolForValue.bitmap.Height, GraphicsUnit.Pixel, imageAttr1);
            }
          }
        }
      }
      foreach (TileInfo tile in this.m_Layers.selection.tiles)
      {
        Tool toolForValue = Tools.GetToolForValue(tile.value);
        Graphics graphics2 = graphics1;
        Bitmap bitmap = toolForValue.bitmap;
        Point position = tile.position;
        int x = position.X;
        position = tile.position;
        int y = position.Y;
        int width1 = toolForValue.bitmap.Width;
        int height1 = toolForValue.bitmap.Height;
        Rectangle destRect = new Rectangle(x, y, width1, height1);
        int width2 = toolForValue.bitmap.Width;
        int height2 = toolForValue.bitmap.Height;
        ImageAttributes imageAttr2 = imageAttr1;
        graphics2.DrawImage((Image) bitmap, destRect, 0, 0, width2, height2, GraphicsUnit.Pixel, imageAttr2);
      }
      Brush brush1 = (Brush) new SolidBrush(Tools.selection.areaColor);
      foreach (MarkerInfo marker in this.m_Layers.selection.markers)
        graphics1.FillRectangle(brush1, marker.rect.Multiplied(16));
      if (this.m_Layers.selection.hasTiles)
      {
        foreach (TileInfo tile in this.m_Layers.selection.tiles)
          graphics1.FillRectangle(brush1, new Rectangle(tile.position.X, tile.position.Y, 16, 16));
      }
      graphics1.Dispose();
    }

    public void Dispose()
    {
      this.target.Dispose();
      this.target = (Bitmap) null;
      this.m_Layers = (Layers) null;
    }

    public void Resize(int _newWidth, int _newHeight) => this.CreateTarget();

    private void CreateTarget()
    {
      if (this.target != null)
        this.target.Dispose();
      this.target = new Bitmap(this.m_Layers.width * 16, this.m_Layers.height * 16);
      Graphics graphics = Graphics.FromImage((Image) this.target);
      graphics.Clear(Color.Gray);
      graphics.Dispose();
    }
  }
}
