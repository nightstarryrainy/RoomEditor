// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmHistory
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmHistory : ToolForm
  {
    private Viewport m_Viewport;
    private IContainer components;
    private ListView historyList;

    public Viewport viewport
    {
      get => this.m_Viewport;
      set
      {
        this.m_Viewport = value;
        if (this.m_Viewport == null)
          return;
        this.m_Viewport.history.historyChanged += new History.HistoryChangedHandler(this.OnHistoryChanged);
        this.OnHistoryChanged(this.m_Viewport.history);
      }
    }

    public frmHistory()
    {
      this.InitializeComponent();
      this.historyList.DoubleClick += new EventHandler(this.OnListDoubleClick);
    }

    public void Clear() => this.historyList.Items.Clear();

    private void OnListDoubleClick(object _sender, EventArgs _args)
    {
      if (this.historyList.SelectedIndices.Count <= 0)
        return;
      int num = this.history.actionCount - 1 - this.historyList.SelectedIndices[0];
      if (num > this.history.currentHistoryIndex)
      {
        for (int currentHistoryIndex = this.history.currentHistoryIndex; currentHistoryIndex < num; ++currentHistoryIndex)
          this.history.Redo();
      }
      else if (num < this.history.currentHistoryIndex)
      {
        for (int currentHistoryIndex = this.history.currentHistoryIndex; currentHistoryIndex > num; --currentHistoryIndex)
          this.history.Undo();
      }
      this.viewport.Invalidate();
      this.viewport.UpdatePictureBoxImage();
    }

    private void OnHistoryChanged(History _history)
    {
      this.historyList.SuspendLayout();
      this.historyList.Columns.Clear();
      this.historyList.Items.Clear();
      this.historyList.Columns.Add("", -2);
      int actionCount = _history.actionCount;
      Font font = new Font(this.historyList.Font, FontStyle.Italic);
      int maxRelevantEntries = Options.instance.historyMaxRelevantEntries;
      int num1 = 0;
      for (int index = actionCount - 1; index >= 0 && maxRelevantEntries > 0; --index)
      {
        List<HistoryEntry> action = _history.actions[index];
        string str = "";
        int num2 = 0;
        foreach (HistoryEntry historyEntry in action)
        {
          if (str == "")
            str = historyEntry.name;
          ++num2;
        }
        this.historyList.Items.Add(str + (num2 > 1 ? " (x" + (object) num2 + ")" : ""));
        ListViewItem listViewItem = this.historyList.Items[this.historyList.Items.Count - 1];
        if (index >= this.history.currentHistoryIndex)
        {
          listViewItem.Font = font;
          listViewItem.ForeColor = Color.Gray;
          ++num1;
        }
        else
        {
          if (maxRelevantEntries == Options.instance.historyMaxRelevantEntries)
            listViewItem.EnsureVisible();
          --maxRelevantEntries;
        }
      }
      if (actionCount > Options.instance.historyMaxRelevantEntries && maxRelevantEntries == 0)
        this.historyList.Items.Add("...");
      else
        this.historyList.Items.Add("Start");
      this.historyList.ResumeLayout();
    }

    private History history => this.viewport.history;

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.historyList = new ListView();
      this.SuspendLayout();
      this.historyList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.historyList.FullRowSelect = true;
      this.historyList.HeaderStyle = ColumnHeaderStyle.None;
      this.historyList.Location = new Point(13, 13);
      this.historyList.MultiSelect = false;
      this.historyList.Name = "historyList";
      this.historyList.Size = new Size(209, 236);
      this.historyList.TabIndex = 0;
      this.historyList.UseCompatibleStateImageBehavior = false;
      this.historyList.View = View.Details;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(234, 261);
      this.Controls.Add((Control) this.historyList);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Name = nameof (frmHistory);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "HistoryForm";
      this.ResumeLayout(false);
    }
  }
}
