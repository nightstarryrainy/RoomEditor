// Decompiled with JetBrains decompiler
// Type: RoomEditor.HistoryPlaceBlocksEntry
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RoomEditor
{
  internal class HistoryPlaceBlocksEntry : HistoryEntry
  {
    private List<Point> m_Positions;
    private List<int> m_NextValues;
    private int m_NextValue;
    private List<int> m_PreviousValues;
    private int m_PreviousValue;
    private Layer.Type m_LayerType;
    private Layers m_Layers;
    private string m_Name = "Place multiple blocks";

    public override string name => this.m_Name;

    public HistoryPlaceBlocksEntry(
      List<Point> _positions,
      List<int> _previousValues,
      int _nextValue,
      Layer.Type _layerType,
      Layers _layers,
      string _nameOverride = "")
      : this(_positions, _previousValues, new List<int>(Enumerable.Repeat<int>(_nextValue, _positions.Count)), _layerType, _layers, _nameOverride)
    {
    }

    public HistoryPlaceBlocksEntry(
      List<Point> _positions,
      List<int> _previousValues,
      List<int> _nextValues,
      Layer.Type _layerType,
      Layers _layers,
      string _nameOverride = "")
    {
      this.m_Positions = new List<Point>();
      this.m_Positions.AddRange((IEnumerable<Point>) _positions);
      if (_nextValues.Count == 1 || _nextValues.FindIndex((Predicate<int>) (elem => elem != _nextValues[0])) == -1)
      {
        this.m_NextValue = _nextValues[0];
      }
      else
      {
        this.m_NextValues = new List<int>();
        this.m_NextValues.AddRange((IEnumerable<int>) _nextValues);
      }
      if (_previousValues.Count == 1 || _previousValues.FindIndex((Predicate<int>) (elem => elem != _previousValues[0])) == -1)
      {
        this.m_PreviousValue = _previousValues[0];
      }
      else
      {
        this.m_PreviousValues = new List<int>();
        this.m_PreviousValues = _previousValues;
      }
      this.m_LayerType = _layerType;
      this.m_Layers = _layers;
      if (_nameOverride != "")
      {
        this.m_Name = _nameOverride;
      }
      else
      {
        this.m_Name = "Place multiple blocks";
        if (this.m_NextValues == null)
          this.m_Name = this.m_Name + " of " + Tools.GetToolForValue(this.m_NextValue).name;
      }
      this.m_Name = this.m_Name + " (" + (object) this.m_Positions.Count + ")";
    }

    public override void Undo()
    {
      Rectangle empty = Rectangle.Empty;
      if (this.m_PreviousValues != null)
        this.m_Layers.PlaceBlocks(this.m_Positions, this.m_PreviousValues, ref this.m_LayerType, ref empty);
      else
        this.m_Layers.PlaceBlocks(this.m_Positions, this.m_PreviousValue, ref this.m_LayerType, ref empty);
    }

    public override void Redo()
    {
      Rectangle empty = Rectangle.Empty;
      if (this.m_NextValues != null)
        this.m_Layers.PlaceBlocks(this.m_Positions, this.m_NextValues, ref this.m_LayerType, ref empty);
      else
        this.m_Layers.PlaceBlocks(this.m_Positions, this.m_NextValue, ref this.m_LayerType, ref empty);
    }
  }
}
