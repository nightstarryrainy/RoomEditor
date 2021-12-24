// Decompiled with JetBrains decompiler
// Type: RoomEditor.MarkerType
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace RoomEditor
{
  public class MarkerType
  {
    private static List<MarkerType> m_All = new List<MarkerType>();
    private static int currentIndex = 1;

    public string id { get; set; }

    public int color { get; set; }

    public IconInfo icon2 { get; set; }

    public bool keep { get; set; }

    public float defaultXr { get; set; }

    public int index { get; set; }

    public static bool loaded => MarkerType.m_All.Count > 0;

    public static event MarkerType.CreatedHandler created;

    public static event MarkerType.ClearedHandler cleared;

    private MarkerType()
    {
    }

    public static void CreateFromString(string _serializedMarkerType)
    {
      MarkerType _newlyCreated = JsonConvert.DeserializeObject<MarkerType>(_serializedMarkerType);
      _newlyCreated.index = MarkerType.currentIndex++;
      MarkerType.m_All.Add(_newlyCreated);
      if (MarkerType.created == null)
        return;
      MarkerType.created(_newlyCreated);
    }

    public static void Clear()
    {
      MarkerType.m_All.Clear();
      MarkerType.currentIndex = 0;
      if (MarkerType.cleared == null)
        return;
      MarkerType.cleared();
    }

    public static MarkerType GetMarkerTypeByID(string _id)
    {
      for (int index = 0; index < MarkerType.m_All.Count; ++index)
      {
        if (MarkerType.m_All[index].id == _id)
          return MarkerType.m_All[index];
      }
      return (MarkerType) null;
    }

    public static MarkerType GetMarkerTypeByValue(int _value)
    {
      for (int index = 0; index < MarkerType.m_All.Count; ++index)
      {
        if (MarkerType.m_All[index].index == _value)
          return MarkerType.m_All[index];
      }
      return (MarkerType) null;
    }

    public override string ToString() => this.id;

    public delegate void CreatedHandler(MarkerType _newlyCreated);

    public delegate void ClearedHandler();
  }
}
