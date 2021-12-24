// Decompiled with JetBrains decompiler
// Type: RoomEditor.Options
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RoomEditor
{
  internal class Options
  {
    private static Options m_Instance;
    private const int currentVersion = 1;
    private bool m_bSortMarkersAlphabetically;
    [JsonProperty]
    private int version = -1;
    [JsonProperty]
    public bool displayShowGrids;
    [JsonProperty]
    public int historyMaxRelevantEntries = 15;
    [JsonProperty(PropertyName = "windows")]
    private List<Options.WindowProps> forms = new List<Options.WindowProps>();

    public static Options instance
    {
      get
      {
        if (Options.m_Instance == null)
          Options.m_Instance = new Options();
        return Options.m_Instance;
      }
    }

    public event Options.OptionsChangedHandler sortMarkersAlphabeticallyChanged;

    public static void Load()
    {
      try
      {
        Options options = JsonConvert.DeserializeObject<Options>(File.ReadAllText(".\\Resources\\RoomEditor\\Options.json"));
        bool flag = false;
        if (options.version != 1)
        {
          options.version = 1;
          flag = true;
        }
        Options.m_Instance = options;
        if (!flag)
          return;
        Options.instance.Save();
      }
      catch (FileNotFoundException ex)
      {
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show("Error loading options : " + ex.Message + "\nOptions have not been loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    public void SaveWindowProps(ToolForm _form)
    {
      int index = 0;
      while (index < this.forms.Count && this.forms[index].name != _form.Name)
        ++index;
      Options.WindowProps windowProps;
      if (index < this.forms.Count)
      {
        windowProps = this.forms[index];
        this.forms.RemoveAt(index);
      }
      else
      {
        windowProps = new Options.WindowProps();
        windowProps.name = _form.Name;
      }
      windowProps.location = _form.Location;
      windowProps.size = _form.Size;
      this.forms.Add(windowProps);
      this.Save();
    }

    public bool LoadWindowProps(ToolForm _form)
    {
      Options.WindowProps windowProps = this.forms.Find((Predicate<Options.WindowProps>) (prop => prop.name == _form.Name));
      if (!(windowProps.name == _form.Name))
        return false;
      _form.Location = windowProps.location;
      _form.Size = windowProps.size;
      _form.ClampToScreen();
      return true;
    }

    public void ResetAllWindowProps() => this.forms.Clear();

    public void ResetToolsWindowProps()
    {
      int index = 0;
      while (index < this.forms.Count)
      {
        if (this.forms[index].name != frmMain.activeMain.Name)
          this.forms.RemoveAt(index);
        else
          ++index;
      }
    }

    public void ResetWindowProps(ToolForm _form)
    {
      int index = 0;
      while (index < this.forms.Count && _form.Name != this.forms[index].name)
        ++index;
      if (index >= this.forms.Count)
        return;
      this.forms.RemoveAt(index);
    }

    public void Save()
    {
      JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
      string contents = JsonConvert.SerializeObject((object) this, Formatting.Indented);
      if (!File.Exists(".\\Resources\\RoomEditor\\Options.json"))
        File.CreateText(".\\Resources\\RoomEditor\\Options.json").Close();
      File.WriteAllText(".\\Resources\\RoomEditor\\Options.json", contents);
    }

    private Options() => this.version = 1;

    private void SetSortMarkerAlphabetically(bool _bSetting)
    {
      if (_bSetting == this.m_bSortMarkersAlphabetically)
        return;
      this.m_bSortMarkersAlphabetically = _bSetting;
      if (this.sortMarkersAlphabeticallyChanged == null)
        return;
      this.sortMarkersAlphabeticallyChanged();
    }

    [JsonProperty]
    public bool toolsSortMarkersAlphabetically
    {
      get => this.m_bSortMarkersAlphabetically;
      set => this.SetSortMarkerAlphabetically(value);
    }

    [Flags]
    public enum Section
    {
      Tools = 0,
      Windows = 1,
      All = Windows, // 0x00000001
    }

    public delegate void OptionsChangedHandler();

    private struct WindowProps
    {
      public string name;
      public Point location;
      public Size size;
    }
  }
}
