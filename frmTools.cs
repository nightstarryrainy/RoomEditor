// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmTools
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmTools : ToolForm
  {
    private const int marginBetweenGroups = 5;
    private const int deltaHeightGroupPanel = 25;
    private List<RadioButton> m_RadioButtons = new List<RadioButton>();
    private List<int> m_MarkerActionIndexes = new List<int>();
    private Keys[] m_Shortcuts = new Keys[30]
    {
      Keys.D1,
      Keys.D2,
      Keys.D3,
      Keys.D4,
      Keys.D5,
      Keys.D6,
      Keys.D7,
      Keys.D8,
      Keys.D9,
      Keys.D0,
      Keys.A,
      Keys.E,
      Keys.R,
      Keys.T,
      Keys.U,
      Keys.I,
      Keys.O,
      Keys.P,
      Keys.Q,
      Keys.D,
      Keys.F,
      Keys.H,
      Keys.J,
      Keys.K,
      Keys.L,
      Keys.M,
      Keys.W,
      Keys.X,
      Keys.B,
      Keys.N
    };
    private IContainer components;
    private ToolTip toolTip;
    private Panel panel;
    private GroupBox toolsGroup;
    private FlowLayoutPanel toolsPanel;
    private GroupBox markersGroup;
    private FlowLayoutPanel markersPanel;
    private GroupBox linksGroup;
    private FlowLayoutPanel linksPanel;
    private GroupBox collisionsGroup;
    private FlowLayoutPanel collisionPanel;

    public frmTools()
    {
      this.InitializeComponent();
      this.KeyDown += new KeyEventHandler(this.FrmTools_KeyDown);
    }

    private void FrmTools_KeyDown(object _sender, KeyEventArgs _args) => this.OnKeyDown(_args);

    public bool OnKeyDown(KeyEventArgs _args)
    {
      int index = 0;
      while (index < this.m_Shortcuts.Length && this.m_Shortcuts[index] != _args.KeyCode)
        ++index;
      if (index >= this.m_Shortcuts.Length || index >= this.m_RadioButtons.Count)
        return false;
      this.m_RadioButtons[index].Checked = true;
      return true;
    }

    public bool OnKeyUp(KeyEventArgs _args) => false;

    public void OnCDBChanged() => this.OnCDBChangedAsyncImpl();

    public void UpdateCurrentTool()
    {
      foreach (RadioButton radioButton in this.m_RadioButtons)
      {
        if (radioButton.Tag == Tools.currentTool && !radioButton.Checked)
          radioButton.Checked = true;
      }
    }

    protected override void OnFormResize()
    {
      if (this.toolsPanel.Controls.Count > 0)
      {
        Control control = this.toolsPanel.Controls[this.toolsPanel.Controls.Count - 1];
        this.toolsPanel.Height = control.Top + control.Height + this.toolsPanel.Margin.Bottom;
        this.toolsGroup.Height = this.toolsPanel.Height + 25;
      }
      if (this.collisionPanel.Controls.Count > 0)
      {
        Control control = this.collisionPanel.Controls[this.collisionPanel.Controls.Count - 1];
        this.collisionPanel.Height = control.Top + control.Height + this.collisionPanel.Margin.Bottom;
        this.collisionsGroup.Height = this.collisionPanel.Height + 25;
      }
      if (this.linksPanel.Controls.Count > 0)
      {
        Control control = this.linksPanel.Controls[this.linksPanel.Controls.Count - 1];
        this.linksPanel.Height = control.Top + control.Height + this.linksPanel.Margin.Bottom;
        this.linksGroup.Height = this.linksPanel.Height + 25;
      }
      if (this.markersPanel.Controls.Count > 0)
      {
        this.markersGroup.Visible = true;
        this.ResizeMarkersGroup();
      }
      else
        this.markersGroup.Visible = false;
      this.collisionsGroup.Top = this.toolsGroup.Top + this.toolsGroup.Height + 5;
      this.linksGroup.Top = this.collisionsGroup.Top + this.collisionsGroup.Height + 5;
      this.markersGroup.Top = this.linksGroup.Top + this.linksGroup.Height + 5;
    }

    private void ResizeMarkersGroup()
    {
      if (this.markersPanel.Controls.Count <= 0)
        return;
      Control control = this.markersPanel.Controls[this.markersPanel.Controls.Count - 1];
      this.markersPanel.Height = control.Top + control.Height + this.markersPanel.Margin.Bottom;
      this.markersGroup.Height = this.markersPanel.Height + 25;
    }

    private Task OnCDBChangedAsyncImpl()
    {
      int index = 0;
      while (index < this.panel.Controls.Count)
      {
        if (this.panel.Controls[index] is RadioButton control2 && ((Tool) control2.Tag).type == Layer.Type.Marker)
        {
          this.panel.Controls.Remove((Control) control2);
          this.m_RadioButtons.Remove(control2);
          if (control2.Checked && this.toolsPanel.Controls.Count > 0)
            (this.toolsPanel.Controls[0] as RadioButton).Checked = true;
          control2.Dispose();
        }
        else
          ++index;
      }
      foreach (Action action in Action.GetByType(Action.Type.SelectTool).FindAll((Predicate<Action>) (a => this.m_MarkerActionIndexes.Contains(a.actionIndex))))
        action.Dispose();
      int _shortcutIndex = this.toolsPanel.Controls.Count - 1;
      foreach (Tool allTool in Tools.allTools)
      {
        if (allTool.type == Layer.Type.Marker)
        {
          this.CreateRadioButtonForTool(allTool, _shortcutIndex);
          this.m_MarkerActionIndexes.Add(_shortcutIndex);
          ++_shortcutIndex;
        }
      }
      if (Options.instance.toolsSortMarkersAlphabetically)
        this.ReorderMarkerTools();
      this.markersGroup.Visible = this.markersPanel.Controls.Count > 0;
      this.ResizeMarkersGroup();
      return (Task) Task.FromResult<int>(0);
    }

    private void frmTools_Load(object _sender, EventArgs _args)
    {
      Options.instance.sortMarkersAlphabeticallyChanged += new Options.OptionsChangedHandler(this.OnSortMarkerAlphabeticallyChanged);
      if (this.toolsPanel.Controls.Count <= 1)
      {
        int num = 0;
        foreach (Tool allTool in Tools.allTools)
          this.CreateRadioButtonForTool(allTool, num++);
        if (Options.instance.toolsSortMarkersAlphabetically)
          this.ReorderMarkerTools();
        this.ResizeMarkersGroup();
      }
      this.markersGroup.Visible = this.markersPanel.Controls.Count > 0;
    }

    private void CreateRadioButtonForTool(Tool _tool, int _shortcutIndex)
    {
      RadioButton radioButton = (RadioButton) new PixelRadioButton();
      radioButton.BackgroundImage = (Image) _tool.bitmap;
      radioButton.BackgroundImageLayout = ImageLayout.Stretch;
      radioButton.Appearance = Appearance.Button;
      radioButton.Width = 32;
      radioButton.Height = 32;
      radioButton.CheckedChanged += new EventHandler(this.OnToolChanged);
      radioButton.Tag = (object) _tool;
      this.toolTip.SetToolTip((Control) radioButton, _tool.name);
      if (_tool == Tools.currentTool)
        radioButton.Checked = true;
      this.m_RadioButtons.Add(radioButton);
      radioButton.CheckedChanged += new EventHandler(this.HandleCheckingAcrossMultiParents);
      if (_tool.group == Tools.Group.Tools)
        radioButton.Parent = (Control) this.toolsPanel;
      else if (_tool.group == Tools.Group.Collisions)
        radioButton.Parent = (Control) this.collisionPanel;
      else if (_tool.group == Tools.Group.Links)
        radioButton.Parent = (Control) this.linksPanel;
      else if (_tool.group == Tools.Group.Markers)
        radioButton.Parent = (Control) this.markersPanel;
      else
        radioButton.Parent = (Control) this.panel;
      if (Action.DoesAlreadyExists(Action.Type.SelectTool, _shortcutIndex))
        return;
      if (_shortcutIndex < this.m_Shortcuts.Length)
      {
        Action action1 = new Action(Action.Type.SelectTool, "Select " + _tool.name, new HotKey(_key: this.m_Shortcuts[_shortcutIndex]), _shortcutIndex++);
      }
      else
      {
        Action action2 = new Action(Action.Type.SelectTool, "Select " + _tool.name, (HotKey) null, _shortcutIndex++);
      }
    }

    private void HandleCheckingAcrossMultiParents(object _sender, EventArgs _args)
    {
      RadioButton radioButton1 = _sender as RadioButton;
      if (!radioButton1.Checked)
        return;
      foreach (RadioButton radioButton2 in this.m_RadioButtons)
      {
        if (radioButton2 != radioButton1)
          radioButton2.Checked = false;
      }
    }

    private void OnToolChanged(object _sender, EventArgs _args)
    {
      RadioButton radioButton = _sender as RadioButton;
      if (!radioButton.Checked)
        return;
      Tools.currentTool = radioButton.Tag as Tool;
    }

    private void ReorderMarkerTools()
    {
      if (this.markersPanel.Controls.Count <= 0)
        return;
      this.markersPanel.SuspendLayout();
      List<RadioButton> radioButtonList = new List<RadioButton>(this.markersPanel.Controls.Count);
      this.markersPanel.Controls.Clear();
      foreach (RadioButton radioButton in this.m_RadioButtons)
      {
        if (((Tool) radioButton.Tag).type == Layer.Type.Marker)
          radioButtonList.Add(radioButton);
      }
      if (Options.instance.toolsSortMarkersAlphabetically)
        radioButtonList.Sort((Comparison<RadioButton>) ((rb1, rb2) => ((Tool) rb1.Tag).name.CompareTo(((Tool) rb2.Tag).name)));
      this.markersPanel.Controls.AddRange((Control[]) radioButtonList.ToArray());
      this.markersPanel.ResumeLayout();
    }

    private void OnSortMarkerAlphabeticallyChanged() => this.ReorderMarkerTools();

    protected override void Dispose(bool disposing)
    {
      Options.instance.sortMarkersAlphabeticallyChanged -= new Options.OptionsChangedHandler(this.OnSortMarkerAlphabeticallyChanged);
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.toolTip = new ToolTip(this.components);
      this.panel = new Panel();
      this.markersGroup = new GroupBox();
      this.markersPanel = new FlowLayoutPanel();
      this.linksGroup = new GroupBox();
      this.linksPanel = new FlowLayoutPanel();
      this.collisionsGroup = new GroupBox();
      this.collisionPanel = new FlowLayoutPanel();
      this.toolsGroup = new GroupBox();
      this.toolsPanel = new FlowLayoutPanel();
      this.panel.SuspendLayout();
      this.markersGroup.SuspendLayout();
      this.linksGroup.SuspendLayout();
      this.collisionsGroup.SuspendLayout();
      this.toolsGroup.SuspendLayout();
      this.SuspendLayout();
      this.toolTip.AutomaticDelay = 50;
      this.toolTip.AutoPopDelay = 3000;
      this.toolTip.InitialDelay = 50;
      this.toolTip.ReshowDelay = 10;
      this.toolTip.ShowAlways = true;
      this.panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel.AutoScroll = true;
      this.panel.Controls.Add((Control) this.markersGroup);
      this.panel.Controls.Add((Control) this.linksGroup);
      this.panel.Controls.Add((Control) this.collisionsGroup);
      this.panel.Controls.Add((Control) this.toolsGroup);
      this.panel.Location = new Point(3, 3);
      this.panel.Name = "panel";
      this.panel.Size = new Size(299, 556);
      this.panel.TabIndex = 0;
      this.markersGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.markersGroup.Controls.Add((Control) this.markersPanel);
      this.markersGroup.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.markersGroup.Location = new Point(0, 210);
      this.markersGroup.Name = "markersGroup";
      this.markersGroup.Size = new Size(296, 86);
      this.markersGroup.TabIndex = 5;
      this.markersGroup.TabStop = false;
      this.markersGroup.Text = " Markers  ";
      this.markersPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.markersPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.markersPanel.Location = new Point(3, 19);
      this.markersPanel.Name = "markersPanel";
      this.markersPanel.Size = new Size(286, 61);
      this.markersPanel.TabIndex = 2;
      this.linksGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.linksGroup.Controls.Add((Control) this.linksPanel);
      this.linksGroup.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.linksGroup.Location = new Point(0, 141);
      this.linksGroup.Name = "linksGroup";
      this.linksGroup.Size = new Size(296, 63);
      this.linksGroup.TabIndex = 4;
      this.linksGroup.TabStop = false;
      this.linksGroup.Text = " Links  ";
      this.linksPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.linksPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.linksPanel.Location = new Point(3, 19);
      this.linksPanel.Name = "linksPanel";
      this.linksPanel.Size = new Size(286, 38);
      this.linksPanel.TabIndex = 2;
      this.collisionsGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.collisionsGroup.Controls.Add((Control) this.collisionPanel);
      this.collisionsGroup.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.collisionsGroup.Location = new Point(0, 72);
      this.collisionsGroup.Name = "collisionsGroup";
      this.collisionsGroup.Size = new Size(296, 63);
      this.collisionsGroup.TabIndex = 3;
      this.collisionsGroup.TabStop = false;
      this.collisionsGroup.Text = " Collisions  ";
      this.collisionPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.collisionPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.collisionPanel.Location = new Point(3, 19);
      this.collisionPanel.Name = "collisionPanel";
      this.collisionPanel.Size = new Size(286, 38);
      this.collisionPanel.TabIndex = 2;
      this.toolsGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.toolsGroup.Controls.Add((Control) this.toolsPanel);
      this.toolsGroup.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
      this.toolsGroup.Location = new Point(0, 3);
      this.toolsGroup.Name = "toolsGroup";
      this.toolsGroup.Size = new Size(296, 63);
      this.toolsGroup.TabIndex = 2;
      this.toolsGroup.TabStop = false;
      this.toolsGroup.Text = " Tools  ";
      this.toolsPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.toolsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.toolsPanel.Location = new Point(3, 19);
      this.toolsPanel.Name = "toolsPanel";
      this.toolsPanel.Size = new Size(290, 38);
      this.toolsPanel.TabIndex = 2;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(304, 561);
      this.Controls.Add((Control) this.panel);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Name = nameof (frmTools);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Tools";
      this.Load += new EventHandler(this.frmTools_Load);
      this.panel.ResumeLayout(false);
      this.markersGroup.ResumeLayout(false);
      this.linksGroup.ResumeLayout(false);
      this.collisionsGroup.ResumeLayout(false);
      this.toolsGroup.ResumeLayout(false);
      this.ResumeLayout(false);
    }
  }
}
