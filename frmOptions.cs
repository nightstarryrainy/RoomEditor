// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmOptions
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmOptions : ToolForm
  {
    private IContainer components;
    private GroupBox groupBox1;
    private CheckBox chkSortMarkers;
    private GroupBox groupBox2;
    private Button cmdResetFormsPosition;

    public frmOptions() => this.InitializeComponent();

    private void frmOptions_Load(object _sender, EventArgs _args)
    {
      this.chkSortMarkers.Checked = Options.instance.toolsSortMarkersAlphabetically;
      this.chkSortMarkers.CheckedChanged += new EventHandler(this.ChkSortMarkers_CheckedChanged);
    }

    private void ChkSortMarkers_CheckedChanged(object _sender, EventArgs _args)
    {
      Options.instance.toolsSortMarkersAlphabetically = this.chkSortMarkers.Checked;
      Options.instance.Save();
    }

    private void cmdResetFormsPosition_Click(object _sender, EventArgs _args)
    {
      ((frmMain) this.Owner).ResetToolFormsPositions();
      Options.instance.Save();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.groupBox1 = new GroupBox();
      this.chkSortMarkers = new CheckBox();
      this.groupBox2 = new GroupBox();
      this.cmdResetFormsPosition = new Button();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      this.groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.groupBox1.Controls.Add((Control) this.chkSortMarkers);
      this.groupBox1.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.groupBox1.Location = new Point(13, 13);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new Size(259, 51);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Tools";
      this.chkSortMarkers.AutoSize = true;
      this.chkSortMarkers.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.chkSortMarkers.Location = new Point(7, 22);
      this.chkSortMarkers.Name = "chkSortMarkers";
      this.chkSortMarkers.Size = new Size(152, 17);
      this.chkSortMarkers.TabIndex = 0;
      this.chkSortMarkers.Text = "Sort markers alphabetically";
      this.chkSortMarkers.UseVisualStyleBackColor = true;
      this.groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.groupBox2.Controls.Add((Control) this.cmdResetFormsPosition);
      this.groupBox2.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.groupBox2.Location = new Point(13, 70);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new Size(259, 51);
      this.groupBox2.TabIndex = 1;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Windows";
      this.cmdResetFormsPosition.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
      this.cmdResetFormsPosition.Location = new Point(7, 22);
      this.cmdResetFormsPosition.Name = "cmdResetFormsPosition";
      this.cmdResetFormsPosition.Size = new Size(152, 23);
      this.cmdResetFormsPosition.TabIndex = 0;
      this.cmdResetFormsPosition.Text = "Reset tool windows positions";
      this.cmdResetFormsPosition.UseVisualStyleBackColor = true;
      this.cmdResetFormsPosition.Click += new EventHandler(this.cmdResetFormsPosition_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(284, 134);
      this.Controls.Add((Control) this.groupBox2);
      this.Controls.Add((Control) this.groupBox1);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Name = nameof (frmOptions);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Options";
      this.Load += new EventHandler(this.frmOptions_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
