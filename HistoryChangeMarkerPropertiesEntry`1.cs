// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryChangeMarkerPropertiesEntry`1
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

namespace RoomEditor
{
  internal class HistoryChangeMarkerPropertiesEntry<T> : HistoryEntry
  {
    private T m_OldValue;
    private T m_NewValue;
    private string m_FieldName;
    private MarkerInfo m_MarkerInfo;
    private const string m_Name = "Changed marker ";

    public override string name => "Changed marker " + this.m_FieldName + " to " + (object) this.m_NewValue;

    public HistoryChangeMarkerPropertiesEntry(
      MarkerInfo _markerInfo,
      T _oldValue,
      T _newValue,
      string _propertyNameChanged)
    {
      this.m_OldValue = _oldValue;
      this.m_NewValue = _newValue;
      this.m_FieldName = _propertyNameChanged;
      this.m_MarkerInfo = _markerInfo;
    }

    public override void Redo() => typeof (MarkerInfo).GetProperty(this.m_FieldName).SetValue((object) this.m_MarkerInfo, (object) this.m_NewValue);

    public override void Undo() => typeof (MarkerInfo).GetProperty(this.m_FieldName).SetValue((object) this.m_MarkerInfo, (object) this.m_OldValue);
  }
}
