// Decompiled with JetBrains decompiler
// Type: RoomEditor.Properties.Resources
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace RoomEditor.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (RoomEditor.Properties.Resources.resourceMan == null)
          RoomEditor.Properties.Resources.resourceMan = new ResourceManager("RoomEditor.Properties.Resources", typeof (RoomEditor.Properties.Resources).Assembly);
        return RoomEditor.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => RoomEditor.Properties.Resources.resourceCulture;
      set => RoomEditor.Properties.Resources.resourceCulture = value;
    }
  }
}
