// Decompiled with JetBrains decompiler
// Type: RoomEditor.Program
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using ModTools;
using System;
using System.Windows.Forms;

namespace RoomEditor
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      try
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        frmMain frmMain = new frmMain();
        frmMain.Text = "";
        Application.Run((Form) frmMain);
      }
      catch (Exception ex)
      {
        Error.Show(ex, true);
      }
    }
  }
}
