// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmKeyBindings
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmKeyBindings : ToolForm
  {
    private HotKey test = new HotKey();
    private IContainer components;
    private TextBox txtHotKey;
    private ToolTip toolTip;

    public frmKeyBindings()
    {
      this.InitializeComponent();
      this.txtHotKey.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OnPreview);
      this.txtHotKey.KeyDown += new KeyEventHandler(this.txtHotKey_KeyDown);
      this.txtHotKey.MouseDown += new MouseEventHandler(this.txtHotKey_MouseDown);
      this.toolTip.SetToolTip((Control) this.txtHotKey, this.txtHotKey.Text);
    }

    private void OnPreview(object _sender, PreviewKeyDownEventArgs _args)
    {
      if (!_args.Control)
        return;
      _args.IsInputKey = true;
    }

    private void txtHotKey_KeyDown(object _sender, KeyEventArgs _args)
    {
      Keys[] keysArray = new Keys[6]
      {
        Keys.Alt,
        Keys.ControlKey,
        Keys.ShiftKey,
        Keys.Menu,
        Keys.LWin,
        Keys.RWin
      };
      Keys modifiers = this.test.modifiers;
      this.test.modifiers = Control.ModifierKeys;
      if (this.test.modifiers != modifiers)
      {
        this.test.mouseButtons = Control.MouseButtons;
        this.test.key = Keys.None;
      }
      if (!((IEnumerable<Keys>) keysArray).Contains<Keys>(_args.KeyCode))
      {
        this.test.key = _args.KeyCode;
        this.test.mouseButtons = MouseButtons.None;
      }
      this.UpdateTxt();
    }

    private void UpdateTxt()
    {
      this.txtHotKey.Text = this.test.ToString();
      Size size = TextRenderer.MeasureText(this.txtHotKey.Text, this.txtHotKey.Font);
      if (size.Width <= this.txtHotKey.Width && size.Height <= this.txtHotKey.Height)
        return;
      this.toolTip.SetToolTip((Control) this.txtHotKey, this.txtHotKey.Text);
    }

    private void txtHotKey_MouseDown(object _sender, MouseEventArgs _args)
    {
      this.test.mouseButtons = Control.MouseButtons;
      if (this.test.mouseButtons != MouseButtons.None)
        this.test.key = Keys.None;
      this.UpdateTxt();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.txtHotKey = new TextBox();
      this.toolTip = new ToolTip(this.components);
      this.SuspendLayout();
      this.txtHotKey.Location = new Point(89, 35);
      this.txtHotKey.Name = "txtHotKey";
      this.txtHotKey.ReadOnly = true;
      this.txtHotKey.ShortcutsEnabled = false;
      this.txtHotKey.Size = new Size(183, 20);
      this.txtHotKey.TabIndex = 0;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(284, 261);
      this.Controls.Add((Control) this.txtHotKey);
      this.Name = nameof (frmKeyBindings);
      this.Text = nameof (frmKeyBindings);
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
