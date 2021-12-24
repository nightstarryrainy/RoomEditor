// Decompiled with JetBrains decompiler
// Type: RoomEditor.Tools
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Drawing;

namespace RoomEditor
{
  internal class Tools
  {
    private static Tool m_CurrentTool;

    public static Tool selection { get; private set; }

    public static Tool mover { get; private set; }

    public static Tool wall { get; private set; }

    public static Tool eraser { get; private set; }

    public static Tool oneWay { get; private set; }

    public static Tool ladder { get; private set; }

    public static Tool rollZone { get; private set; }

    public static Tool invisibleCollision { get; private set; }

    public static Tool linkToTop { get; private set; }

    public static Tool linkToLeft { get; private set; }

    public static Tool linkToBottom { get; private set; }

    public static Tool linkToRight { get; private set; }

    public static Tool horizontalLink { get; private set; }

    public static Tool verticalLink { get; private set; }

    public static Tool optionalAdd { get; private set; }

    public static Tool optionalDel { get; private set; }

    public static event Tools.ToolChangedHandler currentToolChanged;

    public static Tool currentTool
    {
      get => Tools.m_CurrentTool;
      set
      {
        if (Tools.m_CurrentTool == value)
          return;
        Tool currentTool1 = Tools.m_CurrentTool;
        Tools.m_CurrentTool = value;
        Tool currentTool2 = Tools.m_CurrentTool;
        Tools.OnCurrentToolChanged(currentTool1, currentTool2);
      }
    }

    public static List<Tool> allTools { get; private set; }

    public static void Init()
    {
      Tools.allTools = new List<Tool>();
      Bitmap _bitmap = new Bitmap(16, 16);
      Graphics graphics = Graphics.FromImage((Image) _bitmap);
      Color color = Color.FromArgb(1089534207);
      graphics.Clear(color);
      graphics.Dispose();
      Tools.allTools.Add(Tools.selection = new Tool(1, Layer.Type.None, Tools.Group.Tools, "Selection", _bitmap));
      Tools.selection.areaColor = color;
      Tools.allTools.Add(Tools.mover = new Tool(2, Layer.Type.None, Tools.Group.Tools, "Move selection", ".\\Resources\\RoomEditor\\cols.png", new Point(3, 1)));
      List<Tool> allTools = Tools.allTools;
      Point _coordinatesInTexture = new Point();
      Tool tool;
      Tools.eraser = tool = new Tool(0, Layer.Type.None, Tools.Group.Tools, "Eraser", "", _coordinatesInTexture);
      allTools.Add(tool);
      Tools.allTools.Add(Tools.wall = new Tool(1, Layer.Type.Collision, Tools.Group.Collisions, "Wall", ".\\Resources\\RoomEditor\\cols.png", new Point(0, 0)));
      Tools.allTools.Add(Tools.oneWay = new Tool(2, Layer.Type.Collision, Tools.Group.Collisions, "One Way", ".\\Resources\\RoomEditor\\cols.png", new Point(1, 0)));
      Tools.allTools.Add(Tools.ladder = new Tool(3, Layer.Type.Collision, Tools.Group.Collisions, "Ladder", ".\\Resources\\RoomEditor\\cols.png", new Point(2, 0)));
      Tools.allTools.Add(Tools.rollZone = new Tool(5, Layer.Type.Collision, Tools.Group.Collisions, "Roll Zone", ".\\Resources\\RoomEditor\\cols.png", new Point(0, 1)));
      Tools.allTools.Add(Tools.invisibleCollision = new Tool(4, Layer.Type.Collision, Tools.Group.Collisions, "Invisible Collision", ".\\Resources\\RoomEditor\\cols.png", new Point(3, 0)));
      Tools.allTools.Add(Tools.linkToTop = new Tool(1, Layer.Type.Link, Tools.Group.Links, "Link to top", ".\\Resources\\RoomEditor\\links.png", new Point(0, 0)));
      Tools.allTools.Add(Tools.linkToLeft = new Tool(2, Layer.Type.Link, Tools.Group.Links, "Link to left", ".\\Resources\\RoomEditor\\links.png", new Point(1, 0)));
      Tools.allTools.Add(Tools.linkToBottom = new Tool(3, Layer.Type.Link, Tools.Group.Links, "Link to bottom", ".\\Resources\\RoomEditor\\links.png", new Point(2, 0)));
      Tools.allTools.Add(Tools.linkToRight = new Tool(4, Layer.Type.Link, Tools.Group.Links, "Link to right", ".\\Resources\\RoomEditor\\links.png", new Point(3, 0)));
      Tools.allTools.Add(Tools.horizontalLink = new Tool(5, Layer.Type.Link, Tools.Group.Links, "Horizontal link", ".\\Resources\\RoomEditor\\links.png", new Point(0, 1)));
      Tools.allTools.Add(Tools.verticalLink = new Tool(6, Layer.Type.Link, Tools.Group.Links, "Vertical link", ".\\Resources\\RoomEditor\\links.png", new Point(1, 1)));
      Tools.allTools.Add(Tools.optionalAdd = new Tool(7, Layer.Type.Link, Tools.Group.Links, "Optional add", ".\\Resources\\RoomEditor\\links.png", new Point(2, 1)));
      Tools.allTools.Add(Tools.optionalDel = new Tool(8, Layer.Type.Link, Tools.Group.Links, "Optional delete", ".\\Resources\\RoomEditor\\links.png", new Point(3, 1)));
      Tools.currentTool = Tools.selection;
      MarkerType.created += new MarkerType.CreatedHandler(Tools.OnMarkerTypeCreated);
      MarkerType.cleared += new MarkerType.ClearedHandler(Tools.OnMarkerTypeCleared);
    }

    public static Bitmap GetBitmapForValue(int _value) => Tools.GetToolForValue(_value).bitmap;

    public static Tool GetToolForValue(int _value)
    {
      _value &= 4095;
      for (int index = 0; index < Tools.allTools.Count; ++index)
      {
        if (Tools.allTools[index].value == _value)
          return Tools.allTools[index];
      }
      throw new ArgumentOutOfRangeException("Cannot find tool with value " + (object) _value);
    }

    public static void OnCurrentToolChanged(Tool _oldTool, Tool _newTool)
    {
      if (Tools.currentToolChanged == null)
        return;
      Tools.currentToolChanged(_oldTool, _newTool);
    }

    public static void OnMarkerTypeCreated(MarkerType _markerType) => Tools.allTools.Add(new Tool(_markerType.index, Layer.Type.Marker, Tools.Group.Markers, _markerType.id, ".\\Resources\\RoomEditor\\" + _markerType.icon2.file, new Point(_markerType.icon2.x, _markerType.icon2.y), _markerType.icon2.size)
    {
      areaColor = Color.FromArgb((int) sbyte.MaxValue, _markerType.color >> 16, (_markerType.color & 65280) >> 8, _markerType.color & (int) byte.MaxValue)
    });

    public static void OnMarkerTypeCleared()
    {
      int index = 0;
      while (index < Tools.allTools.Count)
      {
        if (Tools.allTools[index].type == Layer.Type.Marker)
          Tools.allTools.RemoveAt(index);
        else
          ++index;
      }
    }

    public enum Group
    {
      Tools,
      Collisions,
      Links,
      Markers,
    }

    public delegate void ToolChangedHandler(Tool _oldTool, Tool _newTool);
  }
}
