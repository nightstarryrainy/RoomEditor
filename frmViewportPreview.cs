// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmViewportPreview
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmViewportPreview : ToolForm
  {
    private Viewport m_Viewport;
    private IContainer components;
    private PictureBox m_PictureBox;

    public Viewport viewport
    {
      get => this.m_Viewport;
      set
      {
        if (value == this.m_Viewport)
          return;
        this.m_Viewport = value;
        if (this.m_Viewport == null)
        {
          this.m_PictureBox.Image = (Image) null;
        }
        else
        {
          this.m_Viewport.displayChanged += new Viewport.ViewportChangedHandler(this.OnDisplayChanged);
          this.OnDisplayChanged();
        }
      }
    }

    public frmViewportPreview()
    {
      this.InitializeComponent();
      this.m_PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
    }

    private void OnDisplayChanged()
    {
      if (this.m_PictureBox.Image != null)
      {
        this.m_PictureBox.Image.Dispose();
        this.m_PictureBox.Image = (Image) null;
      }
      this.m_PictureBox.Image = (Image) this.m_Viewport.target.Clone();
      this.m_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
      Graphics graphics = Graphics.FromImage(this.m_PictureBox.Image);
      Pen pen1 = (Pen) Pens.Black.Clone();
      pen1.Width = (float) Math.Max(3, this.m_Viewport.target.Width / 160);
      pen1.Alignment = PenAlignment.Inset;
      Pen pen2 = pen1;
      double x = (double) this.m_Viewport.viewedArea.X;
      RectangleF viewedArea = this.m_Viewport.viewedArea;
      double y = (double) viewedArea.Y;
      viewedArea = this.m_Viewport.viewedArea;
      double width = (double) viewedArea.Width;
      viewedArea = this.m_Viewport.viewedArea;
      double height = (double) viewedArea.Height;
      graphics.DrawRectangle(pen2, (float) x, (float) y, (float) width, (float) height);
      graphics.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.m_PictureBox = new PictureBox();
      ((ISupportInitialize) this.m_PictureBox).BeginInit();
      this.SuspendLayout();
      this.m_PictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.m_PictureBox.Location = new Point(12, 12);
      this.m_PictureBox.Name = "m_PictureBox";
      this.m_PictureBox.Size = new Size(360, 147);
      this.m_PictureBox.TabIndex = 0;
      this.m_PictureBox.TabStop = false;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(384, 171);
      this.Controls.Add((Control) this.m_PictureBox);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.Name = nameof (frmViewportPreview);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Preview";
      ((ISupportInitialize) this.m_PictureBox).EndInit();
      this.ResumeLayout(false);
    }
  }
}
