// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmResize
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmResize : ToolForm
  {
    private IContainer components;
    private Button cmdCancel;
    private Button cmdOK;
    private Label label1;
    private Label label2;
    private NumericUpDown numWidth;
    private NumericUpDown numHeight;

    public int width
    {
      get => (int) this.numWidth.Value;
      set => this.numWidth.Value = (Decimal) value;
    }

    public int height
    {
      get => (int) this.numHeight.Value;
      set => this.numHeight.Value = (Decimal) value;
    }

    public frmResize() => this.InitializeComponent();

    private void cmdCancel_Click(object _sender, EventArgs _args)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void cmdOK_Click(object _sender, EventArgs _args)
    {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void frmResize_Load(object _sender, EventArgs _args)
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.cmdCancel = new Button();
      this.cmdOK = new Button();
      this.label1 = new Label();
      this.label2 = new Label();
      this.numWidth = new NumericUpDown();
      this.numHeight = new NumericUpDown();
      this.numWidth.BeginInit();
      this.numHeight.BeginInit();
      this.SuspendLayout();
      this.cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.cmdCancel.DialogResult = DialogResult.Cancel;
      this.cmdCancel.Location = new Point(94, 63);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new Size(75, 23);
      this.cmdCancel.TabIndex = 4;
      this.cmdCancel.Text = "&Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      this.cmdCancel.Click += new EventHandler(this.cmdCancel_Click);
      this.cmdOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.cmdOK.Location = new Point(12, 63);
      this.cmdOK.Name = "cmdOK";
      this.cmdOK.Size = new Size(75, 23);
      this.cmdOK.TabIndex = 3;
      this.cmdOK.Text = "&OK";
      this.cmdOK.UseVisualStyleBackColor = true;
      this.cmdOK.Click += new EventHandler(this.cmdOK_Click);
      this.label1.Location = new Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new Size(75, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Width:";
      this.label2.Location = new Point(13, 35);
      this.label2.Name = "label2";
      this.label2.Size = new Size(75, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Height:";
      this.numWidth.Location = new Point(94, 11);
      this.numWidth.Maximum = new Decimal(new int[4]
      {
        256,
        0,
        0,
        0
      });
      this.numWidth.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numWidth.Name = "numWidth";
      this.numWidth.Size = new Size(75, 20);
      this.numWidth.TabIndex = 1;
      this.numWidth.TextAlign = HorizontalAlignment.Right;
      this.numWidth.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numHeight.Location = new Point(94, 37);
      this.numHeight.Maximum = new Decimal(new int[4]
      {
        256,
        0,
        0,
        0
      });
      this.numHeight.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numHeight.Name = "numHeight";
      this.numHeight.Size = new Size(75, 20);
      this.numHeight.TabIndex = 2;
      this.numHeight.TextAlign = HorizontalAlignment.Right;
      this.numHeight.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.AcceptButton = (IButtonControl) this.cmdOK;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CancelButton = (IButtonControl) this.cmdCancel;
      this.ClientSize = new Size(181, 98);
      this.Controls.Add((Control) this.numHeight);
      this.Controls.Add((Control) this.numWidth);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cmdOK);
      this.Controls.Add((Control) this.cmdCancel);
      this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
      this.KeyPreview = true;
      this.Name = nameof (frmResize);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Text = "Resize";
      this.Load += new EventHandler(this.frmResize_Load);
      this.numWidth.EndInit();
      this.numHeight.EndInit();
      this.ResumeLayout(false);
    }
  }
}
