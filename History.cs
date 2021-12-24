// Decompiled with JetBrains decompiler
// Type: RoomEditor.History
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.Collections.Generic;

namespace RoomEditor
{
  public class History
  {
    private List<List<HistoryEntry>> m_Actions = new List<List<HistoryEntry>>();
    private int m_CurrentHistoryIndex;

    public int currentHistoryIndex => this.m_CurrentHistoryIndex;

    public int actionCount => this.m_Actions.Count;

    public event History.HistoryChangedHandler historyChanged;

    public List<List<HistoryEntry>> actions => this.m_Actions;

    public bool canUndo => this.m_CurrentHistoryIndex > 0;

    public bool canRedo => this.m_CurrentHistoryIndex < this.m_Actions.Count;

    public bool Undo()
    {
      if (this.m_CurrentHistoryIndex <= 0)
        return false;
      List<HistoryEntry> action = this.m_Actions[--this.m_CurrentHistoryIndex];
      for (int index = action.Count - 1; index >= 0; --index)
        action[index].Undo();
      this.callEventHistoryChanged();
      return true;
    }

    public bool Redo()
    {
      if (this.m_CurrentHistoryIndex >= this.m_Actions.Count)
        return false;
      List<HistoryEntry> action = this.m_Actions[this.m_CurrentHistoryIndex++];
      if (action != null && action.Count > 0)
      {
        foreach (HistoryEntry historyEntry in action)
          historyEntry.Redo();
      }
      this.callEventHistoryChanged();
      return true;
    }

    public void AddAction()
    {
      if (this.m_CurrentHistoryIndex < this.m_Actions.Count)
        this.m_Actions.RemoveRange(this.m_CurrentHistoryIndex, this.m_Actions.Count - this.m_CurrentHistoryIndex);
      this.m_Actions.Add(new List<HistoryEntry>());
      ++this.m_CurrentHistoryIndex;
    }

    public void RemoveEmptyAction()
    {
      if (this.m_Actions[this.m_CurrentHistoryIndex - 1].Count != 0)
        return;
      this.m_Actions.RemoveAt(--this.m_CurrentHistoryIndex);
      this.callEventHistoryChanged();
    }

    public void AddEntry(HistoryEntry _entry)
    {
      if (_entry == null)
        return;
      this.m_Actions[this.m_CurrentHistoryIndex - 1].Add(_entry);
      this.callEventHistoryChanged();
    }

    public void AddEntries(List<HistoryEntry> _entries)
    {
      if (_entries == null || _entries.Count == 0)
        return;
      this.m_Actions[this.m_CurrentHistoryIndex - 1].AddRange((IEnumerable<HistoryEntry>) _entries);
      this.callEventHistoryChanged();
    }

    public void Clear()
    {
      this.m_Actions.Clear();
      this.m_CurrentHistoryIndex = 0;
      this.callEventHistoryChanged();
    }

    private void callEventHistoryChanged()
    {
      if (this.historyChanged == null)
        return;
      this.historyChanged(this);
    }

    public delegate void HistoryChangedHandler(History _history);
  }
}
