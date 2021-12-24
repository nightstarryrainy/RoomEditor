// Decompiled with JetBrains decompiler
// Type: RoomEditor.Action
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RoomEditor
{
  internal class Action : IComparable, IDisposable
  {
    private ToolStripMenuItem m_MenuItem;
    private static Dictionary<Action.Type, List<Action>> m_All = new Dictionary<Action.Type, List<Action>>();

    public Action.Type type { get; private set; }

    public string name { get; private set; }

    public HotKey hotKey { get; private set; }

    public int actionIndex { get; private set; }

    public Action(
      Action.Type _type,
      string _name,
      HotKey _defaultHotKey,
      int _actionIndex = -1,
      ToolStripMenuItem _menuItem = null)
    {
      this.type = !Action.DoesAlreadyExists(_type, _actionIndex) ? _type : throw new Exception("Trying to create an action that already exist");
      this.name = _name;
      this.m_MenuItem = _menuItem;
      this.SetHotKey(_defaultHotKey);
      if (Action.m_All.TryGetValue(this.type, out List<Action> _))
        Action.m_All[this.type].Add(this);
      else
        Action.m_All.Add(this.type, new List<Action>()
        {
          this
        });
      this.actionIndex = _actionIndex;
    }

    public void Dispose()
    {
      List<Action> actionList;
      Action.m_All.TryGetValue(this.type, out actionList);
      actionList.Remove(this);
      if (actionList.Count > 0)
        Action.m_All[this.type] = actionList;
      else
        Action.m_All.Remove(this.type);
    }

    public static List<Action> GetByType(Action.Type _type)
    {
      List<Action> actionList;
      return !Action.m_All.TryGetValue(_type, out actionList) ? (List<Action>) null : actionList;
    }

    public bool SetHotKey(HotKey _hotKey)
    {
      if (this.IsAlreadyAssigned(_hotKey))
        return false;
      this.hotKey = _hotKey;
      this.UpdateMenuItem(this.m_MenuItem);
      return true;
    }

    public void UpdateMenuItem(ToolStripMenuItem _menuItem)
    {
      this.m_MenuItem = _menuItem;
      if (this.m_MenuItem == null)
        return;
      if (this.hotKey.isValid && this.hotKey.mouseButtons == MouseButtons.None)
        this.m_MenuItem.ShortcutKeys = this.hotKey.modifiers | this.hotKey.key;
      else
        this.m_MenuItem.ShortcutKeys = Keys.None;
    }

    public int CompareTo(object _object)
    {
        var action = _object as Action;
      if (action == null)
      {
        InvalidCastException invalidCastException = new InvalidCastException("Cannot cast object passed as parameter into Action");
      }
      if (this.hotKey != null && action.hotKey != null)
        return this.hotKey.complexityScore.CompareTo(action.hotKey.complexityScore);
      if (this.hotKey != null)
        return 1;
      return this.hotKey == action.hotKey ? 0 : -1;
    }

    public static bool DoesAlreadyExists(Action.Type _type, int _index)
    {
      List<Action> byType = Action.GetByType(_type);
      if (byType == null)
        return false;
      int index = 0;
      while (index < byType.Count && byType[index].actionIndex != _index)
        ++index;
      return index < byType.Count;
    }

    private bool IsAlreadyAssigned(HotKey _hotKey)
    {
      foreach (KeyValuePair<Action.Type, List<Action>> keyValuePair in Action.m_All)
      {
        foreach (Action action in keyValuePair.Value)
        {
          if (action.hotKey == _hotKey)
            return true;
        }
      }
      return false;
    }

    public enum Type
    {
      Undo,
      Redo,
      NewRoom,
      ResizeRoom,
      SaveRoom,
      SaveRoomAs,
      ToggleTools,
      TogglePreview,
      SelectTool,
    }
  }
}
