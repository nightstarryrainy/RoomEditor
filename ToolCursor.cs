// Decompiled with JetBrains decompiler
// Type: RoomEditor.ToolCursor
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoomEditor
{
  internal class ToolCursor
  {
    private InterpolablePictureBox m_Cursor;
    private PictureBox m_ViewportPictureBox;
    private Viewport m_Viewport;
    private float m_fZoomRatio;

    public PictureBox pictureBox => (PictureBox) this.m_Cursor;

    public ToolCursor(Viewport _viewport, PictureBox _viewportPictureBox)
    {
      this.m_Viewport = _viewport;
      this.m_Viewport.zoomChanged += new Viewport.ZoomChangedHandler(this.OnZoomChanged);
      this.m_ViewportPictureBox = _viewportPictureBox;
      this.m_Cursor = new InterpolablePictureBox();
      this.m_Cursor.Visible = false;
      this.m_Cursor.Size = new Size(16, 16);
      this.m_Cursor.SizeMode = PictureBoxSizeMode.StretchImage;
      this.m_Cursor.interpolationMode = InterpolationMode.NearestNeighbor;
      this.m_ViewportPictureBox.Controls.Add((Control) this.m_Cursor);
      this.m_Cursor.BringToFront();
      Tools.currentToolChanged += new Tools.ToolChangedHandler(this.OnCurrentToolChanged);
    }

    private void OnZoomChanged(float _oldRatio, float _newRatio)
    {
      this.m_Cursor.Size = new Size((int) (16.0 * (double) _newRatio), (int) (16.0 * (double) _newRatio));
      this.m_fZoomRatio = _newRatio;
    }

    private void OnCurrentToolChanged(Tool _oldTool, Tool _newTool)
    {
      this.m_Cursor.Visible = _newTool != Tools.selection;
      if (!this.m_Cursor.Visible)
        return;
      this.m_Cursor.Image = (Image) _newTool.bitmap;
    }

    public void OnMouseMove(object _sender, MouseEventArgs _args)
    {
      if (!this.m_Cursor.Visible)
        return;
      int _gridSize = (int) (16.0 * (double) this.m_Viewport.zoom);
      Point point = Point.Round(this.m_Viewport.viewedArea.Location.Multiplied(-this.m_Viewport.zoom));
      this.m_Cursor.Location = this.m_ViewportPictureBox.PointToClient(Control.MousePosition - (Size) point).Snapped(_gridSize) + (Size) point;
    }
  }
}
