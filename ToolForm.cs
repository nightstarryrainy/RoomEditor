// Decompiled with JetBrains decompiler
// Type: RoomEditor.ToolForm
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Windows.Forms;

namespace RoomEditor
{
  public class ToolForm : Form
  {
    public ToolForm() => this.Load += new EventHandler(this.OnToolFormLoad);

    public void OnToolFormLoad(object _sender, EventArgs _args)
    {
      this.Move += new EventHandler(this.OnToolFormMoved);
      this.ResizeEnd += new EventHandler(this.OnToolFormResizeEnd);
      this.Resize += new EventHandler(this.OnToolFormResize);
    }

    protected virtual void OnFormResizeEnd()
    {
    }

    protected virtual void OnFormResize()
    {
    }

    private void OnToolFormMoved(object _sender, EventArgs _args) => Options.instance.SaveWindowProps(this);

    private void OnToolFormResize(object _sender, EventArgs _args) => this.OnFormResize();

    private void OnToolFormResizeEnd(object _sender, EventArgs _args)
    {
      Options.instance.SaveWindowProps(this);
      this.OnFormResizeEnd();
    }
  }
}
