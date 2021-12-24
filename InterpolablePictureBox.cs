// Decompiled with JetBrains decompiler
// Type: RoomEditor.InterpolablePictureBox
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoomEditor
{
  public class InterpolablePictureBox : PictureBox
  {
    public InterpolationMode interpolationMode { get; set; }

    protected override void OnPaint(PaintEventArgs _paintEventArgs)
    {
      _paintEventArgs.Graphics.InterpolationMode = this.interpolationMode;
      base.OnPaint(_paintEventArgs);
    }
  }
}
