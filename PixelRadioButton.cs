// Decompiled with JetBrains decompiler
// Type: RoomEditor.PixelRadioButton
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoomEditor
{
  internal class PixelRadioButton : RadioButton
  {
    protected override void OnPaint(PaintEventArgs _paintEventArgs)
    {
      _paintEventArgs.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
      base.OnPaint(_paintEventArgs);
    }
  }
}
