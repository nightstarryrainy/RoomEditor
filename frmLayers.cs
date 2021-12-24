// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmLayers
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RoomEditor
{
  public class frmLayers : ToolForm
  {
    private Layers m_Layers;
    private bool bAutomaticallyChangingLayer;
    private IContainer components;
    private Panel panel;
    private TrackBar trackBarMarker;
    private TrackBar trackBarLnk;
    private CheckBox chkLockedCol;
    private TrackBar trackBarCol;
    private CheckBox chkVisibleCol;
    private CheckBox chkLockedMarker;
    private CheckBox chkVisibleMarker;
    private CheckBox chkLockedLnk;
    private CheckBox chkVisibleLnk;
    private RadioButton rdioCollision;
    private RadioButton rdioLink;
    private RadioButton rdioMarker;
    private Label label;

    public Layers layers
    {
      get => this.m_Layers;
      set
      {
        if (this.m_Layers == value)
          return;
        if (this.m_Layers != null)
          this.m_Layers.currentLayerChanged -= new Layers.LayersEventHandler(this.OnCurrentLayerChanged);
        this.m_Layers = value;
        this.m_Layers.currentLayerChanged += new Layers.LayersEventHandler(this.OnCurrentLayerChanged);
        this.UpdateDisplay();
      }
    }

    public Viewport viewport { get; set; }

    public frmLayers() => this.InitializeComponent();

    private void frmLayers_Load(object _sender, EventArgs _args) => this.UpdateDisplay();

    private void OnCurrentLayerChanged()
    {
      foreach (Control control in (ArrangedElementCollection) this.panel.Controls)
      {
        if (control is RadioButton radioButton1 && radioButton1.Tag == this.m_Layers.currentLayer)
        {
          this.bAutomaticallyChangingLayer = true;
          radioButton1.Checked = true;
          this.bAutomaticallyChangingLayer = false;
          break;
        }
      }
    }

    private void UpdateDisplay()
    {
      if (this.m_Layers != null)
      {
        Layer collisionLayer = (Layer) this.m_Layers.collisionLayer;
        Layer linkLayer = (Layer) this.m_Layers.linkLayer;
        Layer markerLayer = (Layer) this.m_Layers.markerLayer;
        this.rdioCollision.Checked = this.m_Layers.currentLayer == collisionLayer;
        this.rdioLink.Checked = this.m_Layers.currentLayer == linkLayer;
        this.rdioMarker.Checked = this.m_Layers.currentLayer == markerLayer;
        this.rdioCollision.CheckedChanged -= new EventHandler(this.OnSelectedLayerChanged);
        this.rdioLink.CheckedChanged -= new EventHandler(this.OnSelectedLayerChanged);
        this.rdioMarker.CheckedChanged -= new EventHandler(this.OnSelectedLayerChanged);
        this.rdioCollision.CheckedChanged += new EventHandler(this.OnSelectedLayerChanged);
        this.rdioLink.CheckedChanged += new EventHandler(this.OnSelectedLayerChanged);
        this.rdioMarker.CheckedChanged += new EventHandler(this.OnSelectedLayerChanged);
        this.rdioCollision.Tag = (object) collisionLayer;
        this.chkLockedCol.Tag = (object) collisionLayer;
        this.chkVisibleCol.Tag = (object) collisionLayer;
        this.trackBarCol.Tag = (object) collisionLayer;
        this.rdioLink.Tag = (object) linkLayer;
        this.chkLockedLnk.Tag = (object) linkLayer;
        this.chkVisibleLnk.Tag = (object) linkLayer;
        this.trackBarLnk.Tag = (object) linkLayer;
        this.rdioMarker.Tag = (object) markerLayer;
        this.chkLockedMarker.Tag = (object) markerLayer;
        this.chkVisibleMarker.Tag = (object) markerLayer;
        this.trackBarMarker.Tag = (object) markerLayer;
        this.chkVisibleCol.CheckedChanged += new EventHandler(this.OnVisibleCheckedChanged);
        this.chkVisibleLnk.CheckedChanged += new EventHandler(this.OnVisibleCheckedChanged);
        this.chkVisibleMarker.CheckedChanged += new EventHandler(this.OnVisibleCheckedChanged);
        this.chkLockedCol.CheckedChanged += new EventHandler(this.OnLockedCheckedChanged);
        this.chkLockedLnk.CheckedChanged += new EventHandler(this.OnLockedCheckedChanged);
        this.chkLockedMarker.CheckedChanged += new EventHandler(this.OnLockedCheckedChanged);
        this.trackBarCol.ValueChanged += new EventHandler(this.OnAlphaValueChanged);
        this.trackBarLnk.ValueChanged += new EventHandler(this.OnAlphaValueChanged);
        this.trackBarMarker.ValueChanged += new EventHandler(this.OnAlphaValueChanged);
        Tools.currentToolChanged += new Tools.ToolChangedHandler(this.OnToolChanged);
        this.panel.Visible = true;
      }
      else
      {
        this.chkVisibleCol.CheckedChanged -= new EventHandler(this.OnVisibleCheckedChanged);
        this.chkVisibleLnk.CheckedChanged -= new EventHandler(this.OnVisibleCheckedChanged);
        this.chkVisibleMarker.CheckedChanged -= new EventHandler(this.OnVisibleCheckedChanged);
        this.chkLockedCol.CheckedChanged -= new EventHandler(this.OnLockedCheckedChanged);
        this.chkLockedLnk.CheckedChanged -= new EventHandler(this.OnLockedCheckedChanged);
        this.chkLockedMarker.CheckedChanged -= new EventHandler(this.OnLockedCheckedChanged);
        this.trackBarCol.ValueChanged -= new EventHandler(this.OnAlphaValueChanged);
        this.trackBarLnk.ValueChanged -= new EventHandler(this.OnAlphaValueChanged);
        this.trackBarMarker.ValueChanged -= new EventHandler(this.OnAlphaValueChanged);
        Tools.currentToolChanged -= new Tools.ToolChangedHandler(this.OnToolChanged);
        this.panel.Visible = false;
      }
    }

    private void OnAlphaValueChanged(object _sender, EventArgs _args)
    {
      TrackBar trackBar = _sender as TrackBar;
      Layer tag = trackBar.Tag as Layer;
      tag.alpha = (byte) trackBar.Value;
      if (!tag.visible)
        return;
      tag.visible = false;
      this.viewport.Invalidate();
      tag.visible = true;
      this.viewport.Invalidate();
      this.viewport.UpdatePictureBoxImage();
    }

    private void OnVisibleCheckedChanged(object _sender, EventArgs _args)
    {
      CheckBox checkBox = _sender as CheckBox;
      (checkBox.Tag as Layer).visible = checkBox.Checked;
      this.viewport.Invalidate();
      this.viewport.UpdatePictureBoxImage();
    }

    private void OnLockedCheckedChanged(object _sender, EventArgs _args)
    {
      CheckBox checkBox = _sender as CheckBox;
      (checkBox.Tag as Layer).locked = checkBox.Checked;
    }

    private void OnSelectedLayerChanged(object _sender, EventArgs _args)
    {
      if (this.bAutomaticallyChangingLayer)
        return;
      RadioButton radioButton = _sender as RadioButton;
      this.m_Layers.ResetSelection();
      this.viewport.Invalidate();
      this.viewport.UpdatePictureBoxImage();
      if (!radioButton.Checked)
        return;
      this.m_Layers.currentLayer = radioButton.Tag as Layer;
    }

    private void OnToolChanged(Tool _oldTool, Tool _newTool)
    {
      foreach (Control control in (ArrangedElementCollection) this.panel.Controls)
      {
        if (control is RadioButton radioButton1 && (radioButton1.Tag as Layer).type == _newTool.type && !radioButton1.Checked)
        {
          this.bAutomaticallyChangingLayer = true;
          radioButton1.Checked = true;
          this.bAutomaticallyChangingLayer = false;
        }
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.panel = new Panel();
      this.trackBarMarker = new TrackBar();
      this.trackBarLnk = new TrackBar();
      this.chkLockedCol = new CheckBox();
      this.trackBarCol = new TrackBar();
      this.chkVisibleCol = new CheckBox();
      this.chkLockedMarker = new CheckBox();
      this.chkVisibleMarker = new CheckBox();
      this.chkLockedLnk = new CheckBox();
      this.chkVisibleLnk = new CheckBox();
      this.rdioCollision = new RadioButton();
      this.rdioLink = new RadioButton();
      this.rdioMarker = new RadioButton();
      this.label = new Label();
      this.panel.SuspendLayout();
      this.trackBarMarker.BeginInit();
      this.trackBarLnk.BeginInit();
      this.trackBarCol.BeginInit();
      this.SuspendLayout();
      this.panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel.Controls.Add((Control) this.trackBarMarker);
      this.panel.Controls.Add((Control) this.trackBarLnk);
      this.panel.Controls.Add((Control) this.chkLockedCol);
      this.panel.Controls.Add((Control) this.trackBarCol);
      this.panel.Controls.Add((Control) this.chkVisibleCol);
      this.panel.Controls.Add((Control) this.chkLockedMarker);
      this.panel.Controls.Add((Control) this.chkVisibleMarker);
      this.panel.Controls.Add((Control) this.chkLockedLnk);
      this.panel.Controls.Add((Control) this.chkVisibleLnk);
      this.panel.Controls.Add((Control) this.rdioCollision);
      this.panel.Controls.Add((Control) this.rdioLink);
      this.panel.Controls.Add((Control) this.rdioMarker);
      this.panel.Location = new Point(4, 11);
      this.panel.Name = "panel";
      this.panel.Size = new Size(372, 151);
      this.panel.TabIndex = 24;
      this.trackBarMarker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.trackBarMarker.AutoSize = false;
      this.trackBarMarker.LargeChange = 25;
      this.trackBarMarker.Location = new Point(72, 112);
      this.trackBarMarker.Maximum = (int) byte.MaxValue;
      this.trackBarMarker.Name = "trackBarMarker";
      this.trackBarMarker.Size = new Size(157, 24);
      this.trackBarMarker.TabIndex = 32;
      this.trackBarMarker.TickFrequency = 0;
      this.trackBarMarker.TickStyle = TickStyle.None;
      this.trackBarMarker.Value = (int) byte.MaxValue;
      this.trackBarLnk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.trackBarLnk.AutoSize = false;
      this.trackBarLnk.LargeChange = 25;
      this.trackBarLnk.Location = new Point(72, 63);
      this.trackBarLnk.Maximum = (int) byte.MaxValue;
      this.trackBarLnk.Name = "trackBarLnk";
      this.trackBarLnk.Size = new Size(157, 24);
      this.trackBarLnk.TabIndex = 31;
      this.trackBarLnk.TickFrequency = 0;
      this.trackBarLnk.TickStyle = TickStyle.None;
      this.trackBarLnk.Value = (int) byte.MaxValue;
      this.chkLockedCol.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkLockedCol.AutoSize = true;
      this.chkLockedCol.BackColor = SystemColors.Control;
      this.chkLockedCol.Location = new Point(235, 10);
      this.chkLockedCol.Name = "chkLockedCol";
      this.chkLockedCol.Size = new Size(62, 17);
      this.chkLockedCol.TabIndex = 30;
      this.chkLockedCol.Text = "Locked";
      this.chkLockedCol.UseVisualStyleBackColor = false;
      this.trackBarCol.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.trackBarCol.AutoSize = false;
      this.trackBarCol.LargeChange = 25;
      this.trackBarCol.Location = new Point(72, 10);
      this.trackBarCol.Maximum = (int) byte.MaxValue;
      this.trackBarCol.Name = "trackBarCol";
      this.trackBarCol.Size = new Size(157, 24);
      this.trackBarCol.TabIndex = 29;
      this.trackBarCol.TickFrequency = 0;
      this.trackBarCol.TickStyle = TickStyle.None;
      this.trackBarCol.Value = (int) byte.MaxValue;
      this.chkVisibleCol.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkVisibleCol.AutoSize = true;
      this.chkVisibleCol.BackColor = Color.Transparent;
      this.chkVisibleCol.Checked = true;
      this.chkVisibleCol.CheckState = CheckState.Checked;
      this.chkVisibleCol.Location = new Point(303, 10);
      this.chkVisibleCol.Name = "chkVisibleCol";
      this.chkVisibleCol.Size = new Size(56, 17);
      this.chkVisibleCol.TabIndex = 28;
      this.chkVisibleCol.Text = "Visible";
      this.chkVisibleCol.UseVisualStyleBackColor = false;
      this.chkLockedMarker.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkLockedMarker.AutoSize = true;
      this.chkLockedMarker.Location = new Point(235, 112);
      this.chkLockedMarker.Name = "chkLockedMarker";
      this.chkLockedMarker.Size = new Size(62, 17);
      this.chkLockedMarker.TabIndex = 27;
      this.chkLockedMarker.Text = "Locked";
      this.chkLockedMarker.UseVisualStyleBackColor = true;
      this.chkVisibleMarker.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkVisibleMarker.AutoSize = true;
      this.chkVisibleMarker.Checked = true;
      this.chkVisibleMarker.CheckState = CheckState.Checked;
      this.chkVisibleMarker.Location = new Point(303, 112);
      this.chkVisibleMarker.Name = "chkVisibleMarker";
      this.chkVisibleMarker.Size = new Size(56, 17);
      this.chkVisibleMarker.TabIndex = 26;
      this.chkVisibleMarker.Text = "Visible";
      this.chkVisibleMarker.UseVisualStyleBackColor = true;
      this.chkLockedLnk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkLockedLnk.AutoSize = true;
      this.chkLockedLnk.Location = new Point(235, 63);
      this.chkLockedLnk.Name = "chkLockedLnk";
      this.chkLockedLnk.Size = new Size(62, 17);
      this.chkLockedLnk.TabIndex = 25;
      this.chkLockedLnk.Text = "Locked";
      this.chkLockedLnk.UseVisualStyleBackColor = true;
      this.chkVisibleLnk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.chkVisibleLnk.AutoSize = true;
      this.chkVisibleLnk.Checked = true;
      this.chkVisibleLnk.CheckState = CheckState.Checked;
      this.chkVisibleLnk.Location = new Point(303, 63);
      this.chkVisibleLnk.Name = "chkVisibleLnk";
      this.chkVisibleLnk.Size = new Size(56, 17);
      this.chkVisibleLnk.TabIndex = 24;
      this.chkVisibleLnk.Text = "Visible";
      this.chkVisibleLnk.UseVisualStyleBackColor = true;
      this.rdioCollision.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.rdioCollision.Appearance = Appearance.Button;
      this.rdioCollision.Location = new Point(1, 1);
      this.rdioCollision.Name = "rdioCollision";
      this.rdioCollision.Size = new Size(372, 45);
      this.rdioCollision.TabIndex = 33;
      this.rdioCollision.TabStop = true;
      this.rdioCollision.Text = "Collisions";
      this.rdioCollision.UseVisualStyleBackColor = true;
      this.rdioLink.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.rdioLink.Appearance = Appearance.Button;
      this.rdioLink.Location = new Point(1, 52);
      this.rdioLink.Name = "rdioLink";
      this.rdioLink.Size = new Size(372, 45);
      this.rdioLink.TabIndex = 34;
      this.rdioLink.TabStop = true;
      this.rdioLink.Text = "Links";
      this.rdioLink.UseVisualStyleBackColor = true;
      this.rdioMarker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.rdioMarker.Appearance = Appearance.Button;
      this.rdioMarker.Location = new Point(1, 103);
      this.rdioMarker.Name = "rdioMarker";
      this.rdioMarker.Size = new Size(372, 45);
      this.rdioMarker.TabIndex = 35;
      this.rdioMarker.TabStop = true;
      this.rdioMarker.Text = "Markers";
      this.rdioMarker.UseVisualStyleBackColor = true;
      this.label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.label.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.label.Location = new Point(27, 73);
      this.label.Name = "label";
      this.label.Size = new Size(330, 24);
      this.label.TabIndex = 25;
      this.label.Text = "No room opened";
      this.label.TextAlign = ContentAlignment.TopCenter;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(384, 171);
      this.Controls.Add((Control) this.panel);
      this.Controls.Add((Control) this.label);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.MaximizeBox = false;
      this.Name = nameof (frmLayers);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Layers";
      this.Load += new EventHandler(this.frmLayers_Load);
      this.panel.ResumeLayout(false);
      this.panel.PerformLayout();
      this.trackBarMarker.EndInit();
      this.trackBarLnk.EndInit();
      this.trackBarCol.EndInit();
      this.ResumeLayout(false);
    }
  }
}
