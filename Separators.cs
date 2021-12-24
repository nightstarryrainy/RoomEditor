// Decompiled with JetBrains decompiler
// Type: RoomEditor.Separators
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RoomEditor
{
  internal class Separators
  {
    private List<SeparatorInfo> m_Separators = new List<SeparatorInfo>();
    private JObject m_RoomSheet;

    public Separators(JObject _roomSheet)
    {
        //修改可以打開CDB源文件
      List<int> separators = new List<int>();
      List<string> stringList = new List<string>();
      JArray jSeparators = null;
      if(_roomSheet.ContainsKey("separators"))
      {
          jSeparators = _roomSheet["separators"].Value<JArray>();
      }

      if (jSeparators != null && jSeparators.Count() > 0)
      {
          foreach (JValue jvalue in jSeparators)
              separators.Add(jvalue.Value<int>());
      }
      else
      {
          Dictionary<string, int> rooms = new Dictionary<string, int>();
          List<string> roomNames = new List<string>();
          int index = 0;
          foreach (JObject room in (IEnumerable<JToken>)_roomSheet["lines"]) {
              var name = room["id"].ToString();
              roomNames.Add(name);
              rooms[name] = index++;
          }

          foreach (JValue jvalue in (IEnumerable<JToken>) _roomSheet["separatorIds"])
          {
              if (jvalue.Type == JTokenType.Integer)
              {
                  var idx = jvalue.Value<int>();
                  separators.Add(idx);
              }
              else
              {
                  var id = jvalue.Value<string>();
                  var idx = rooms[id];
                  separators.Add(idx);
              }
          }
      }


      foreach (JValue jvalue in (IEnumerable<JToken>) _roomSheet["props"][(object) "separatorTitles"])
        stringList.Add(jvalue.ToString());
      for (int index = 0; index < separators.Count; ++index)
        this.m_Separators.Add(new SeparatorInfo()
        {
          name = stringList[index],
          lineIndex = separators[index]
        });
      this.m_Separators.Sort((Comparison<SeparatorInfo>) ((a, b) => a.lineIndex.CompareTo(b.lineIndex)));
      this.m_RoomSheet = _roomSheet;
    }

 

        public List<SeparatorInfo> FilterByName(string _pattern) => this.m_Separators.FindAll((Predicate<SeparatorInfo>) (s => s.name.ToUpper().IndexOf(_pattern.ToUpper()) != -1));

    public void RetrieveSeparatorNames(out List<string> _list, bool _bSort = true)
    {
      _list = new List<string>();
      foreach (SeparatorInfo separator in this.m_Separators)
        _list.Add(separator.name);
      if (!_bSort)
        return;
      _list.Sort();
    }

    public bool DoesSeparatorExist(string _separatorName) => this.m_Separators.Find((Predicate<SeparatorInfo>) (sep => sep.name == _separatorName)).name != null;

    public int GetIndexForGroupName(string _groupName, out SeparatorInfo _info)
    {
      int index = this.m_Separators.FindIndex((Predicate<SeparatorInfo>) (sep => sep.name == _groupName));
      _info = this.m_Separators[index];
      return index;
    }

    public SeparatorInfo GetSeparatorInfo(int _groupIndex) => this.m_Separators[_groupIndex];

    public int GetIndexForGroupName(string _groupName) => this.GetIndexForGroupName(_groupName, out SeparatorInfo _);

    public int GetIndexForLineIndex(int _lineIndex)
    {
      int index = 1;
      while (index < this.m_Separators.Count && this.m_Separators[index].lineIndex <= _lineIndex)
        ++index;
      return Math.Min(index - 1, this.m_Separators.Count - 1);
    }

    public void InsertLineInGroup(string _groupName,string roomName)
    {
      this.m_Separators.Sort((Comparison<SeparatorInfo>) ((a, b) => a.lineIndex.CompareTo(b.lineIndex)));
      int index1 = 0;
      while (index1 < this.m_Separators.Count && this.m_Separators[index1].name != _groupName)
        ++index1;
      //如果存在separatorIds 更新Id
      bool isContainIds = this.m_RoomSheet.ContainsKey("separatorIds");
      bool isContainSeparators = this.m_RoomSheet.ContainsKey("separators");

      if (isContainIds)
      {
          var id= this.m_RoomSheet["separatorIds"][index1];
          if (id.Type == JTokenType.String)
              this.m_RoomSheet["separatorIds"][index1] = (JToken)roomName;
      }
      ;
      for (int index2 = index1 + 1; index2 < this.m_Separators.Count; ++index2)
      {
        SeparatorInfo separator = this.m_Separators[index2];
        ++separator.lineIndex;
        this.m_Separators[index2] = separator;
        if (isContainSeparators)
        {
            this.m_RoomSheet["separators"][(object)index2] = (JToken)separator.lineIndex;
        }

        if (isContainIds)
        {
           //to nothing
            var id =   this.m_RoomSheet["separatorIds"][(object) index2];
            if(id.Type == JTokenType.Integer)
                this.m_RoomSheet["separatorIds"][(object)index2] = (JToken)separator.lineIndex;
        }
      }
    }
  }

  static class Helper
  {
      public static List<List<T>> Split<T>(this List<T> list, List<string> indexes, Func<T, string> checkAction) {
          List<List<T>> ret = new List<List<T>>();
          int last = 0;
          foreach (var str in indexes) {

              var index = list.FindIndex(last, (v) =>
              {
                  return checkAction(v) == str;
              });

              if (index >= 0 && index <= list.Count) {
                  var count = index - last;
                  if (count > 0) {
                      ret.Add(list.Take(last, count));
                  }
                  last = index;
              }
          }

          {
              ret.Add(list.Take(last));
          }
          return ret;
      }
      public static List<T> Take<T>(this List<T> list, int start, int count = -1) {
          var ret = new List<T>();
          if (count == -1) {
              var ac = list.Count;
              count = ac - start;
          }
          if (count > 0) {
              for (var index = start; index < list.Count; ++index) {
                  ret.Add(list[index]);
                  if (--count == 0)
                      break;
              }
          }
          return ret;
      }
    }
}
