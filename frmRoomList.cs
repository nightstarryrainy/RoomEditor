// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmRoomList
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmRoomList : ToolForm
  {
    private List<frmRoomList.RoomInfo> m_Rooms = new List<frmRoomList.RoomInfo>();
    private RoomRenderer m_Renderer;
    private Separators m_Separators;
    private int m_SelectedGroupIndex = -1;
    private bool m_bApplyFilterChanges = true;
    private Color defaultColor = Color.Gray;
    private frmRoomList.Mode m_CurrentMode;
    private IContainer components;
    private TextBox txtFilterRoom;
    private ListBox roomNamesList;
    private PictureBox pictureBox;
    private Button cmdOpen;
    private Button cmdCancel;
    private ListBox roomGroupList;
    private TextBox txtFilterGroup;
    public bool IsRoomsDirty { get; private set; } = false;

    public JObject selectedRoom { get; private set; }

    public List<string> roomNames { get; private set; } = new List<string>();

    public frmRoomList(frmRoomList.Mode _mode)
    {
      this.InitializeComponent();
      this.roomNamesList.DoubleClick += new EventHandler(this.RoomNamesList_DoubleClick);
      this.roomGroupList.MouseDown += new MouseEventHandler(this.RoomGroupList_MouseDown);
      this.roomGroupList.KeyDown += new KeyEventHandler(this.RoomGroupList_KeyDown);
      this.txtFilterGroup.Enter += new EventHandler(this.OnTextBoxEnter);
      this.txtFilterRoom.Enter += new EventHandler(this.OnTextBoxEnter);
      this.txtFilterGroup.Leave += new EventHandler(this.OnTextBoxLeave);
      this.txtFilterRoom.Leave += new EventHandler(this.OnTextBoxLeave);
      this.OnTextBoxLeave((object) this.txtFilterRoom, (EventArgs) null);
      this.m_CurrentMode = _mode;
      if (_mode == frmRoomList.Mode.Open)
      {
        this.cmdOpen.Text = "Open";
        this.Text = "Open room";
      }
      else
      {
        if (_mode != frmRoomList.Mode.Duplicate)
          return;
        this.cmdOpen.Text = "Duplicate";
        this.Text = "Duplicate room";
      }
    }

    private void OnTextBoxEnter(object _sender, EventArgs _args)
    {
      TextBox textBox = _sender as TextBox;
      if (!(textBox.Text == textBox.Tag.ToString()))
        return;
      this.m_bApplyFilterChanges = false;
      textBox.Text = "";
      textBox.ForeColor = Color.Black;
      textBox.Font = new Font(textBox.Font, FontStyle.Regular);
      this.m_bApplyFilterChanges = true;
    }

    private void OnTextBoxLeave(object _sender, EventArgs _args)
    {
      TextBox textBox = _sender as TextBox;
      if (!(textBox.Text == ""))
        return;
      this.m_bApplyFilterChanges = false;
      textBox.Text = textBox.Tag.ToString();
      textBox.ForeColor = Color.Gray;
      textBox.Font = new Font(textBox.Font, FontStyle.Italic);
      this.m_bApplyFilterChanges = true;
    }

    private void RoomGroupList_KeyDown(object _sender, KeyEventArgs _args)
    {
      if (_args.KeyCode != Keys.Escape)
        return;
      this.roomGroupList.ClearSelected();
    }

    private void RoomGroupList_MouseDown(object _sender, MouseEventArgs _args)
    {
      if (_args.Button != MouseButtons.Right)
        return;
      this.roomGroupList.ClearSelected();
    }

    public void SetRooms(JArray _rooms, JObject _roomSheet)
    {
      this.m_Rooms.Clear();
      this.roomNamesList.ClearSelected();
      this.roomNamesList.Items.Clear();
      this.roomNames.Clear();
      this.m_Separators = new Separators(_roomSheet);
      int _lineIndex = 0;
      foreach (JObject room in _rooms)
      {
        frmRoomList.RoomInfo roomInfo = new frmRoomList.RoomInfo()
        {
          name = room["id"].ToString(),
          data = room,
          groupIndex = this.m_Separators.GetIndexForLineIndex(_lineIndex)
        };
        if (room.ContainsKey("group"))
        {
            var groupIndex = room["group"].Value<int>();
            if (roomInfo.groupIndex != groupIndex)
            {
                room["group"] = groupIndex;
                IsRoomsDirty = true;
            }
        }
        else
        {
            room.Add("group",roomInfo.groupIndex);
            IsRoomsDirty = true;
        }
        this.m_Rooms.Add(roomInfo);
        this.roomNamesList.Items.Add((object) roomInfo);
        this.roomNames.Add(roomInfo.name);
        ++_lineIndex;
      }
      this.m_SelectedGroupIndex = -1;
      this.PopulateGroupList();
      this.PopulateRoomList();
    }

    private void PopulateRoomList()
    {
      List<frmRoomList.RoomInfo> all = this.m_Rooms.FindAll((Predicate<frmRoomList.RoomInfo>) (room => this.txtFilterRoom.ForeColor == this.defaultColor || room.name.ToUpper().IndexOf(this.txtFilterRoom.Text.ToUpper()) != -1));
      this.roomNamesList.ClearSelected();
      this.roomNamesList.Items.Clear();
      List<int> intList = (List<int>) null;
      if (this.txtFilterGroup.Text != "")
      {
        List<string> groupNameList = this.GetGroupNameList();
        intList = new List<int>();
        foreach (string _groupName in groupNameList)
          intList.Add(this.m_Separators.GetIndexForGroupName(_groupName));
      }
      foreach (frmRoomList.RoomInfo roomInfo in all)
      {
        if ((this.m_SelectedGroupIndex == -1 || roomInfo.groupIndex == this.m_SelectedGroupIndex) && (intList == null || intList.Contains(roomInfo.groupIndex)))
          this.roomNamesList.Items.Add((object) roomInfo);
      }
    }

    private List<string> GetGroupNameList()
    {
      List<string> _list;
      this.m_Separators.RetrieveSeparatorNames(out _list);
      return _list.FindAll((Predicate<string>) (name => this.txtFilterGroup.ForeColor == this.defaultColor || name.ToUpper().IndexOf(this.txtFilterGroup.Text.ToUpper()) != -1));
    }

    private void PopulateGroupList()
    {
      this.roomGroupList.ClearSelected();
      this.roomGroupList.Items.Clear();
      foreach (object groupName in this.GetGroupNameList())
        this.roomGroupList.Items.Add(groupName);
    }

    private void txtFilter_TextChanged(object _sender, EventArgs _args)
    {
      if (!this.m_bApplyFilterChanges)
        return;
      this.PopulateRoomList();
    }

    private void txtFilterGroup_TextChanged(object _sender, EventArgs _args)
    {
      if (!this.m_bApplyFilterChanges)
        return;
      this.PopulateGroupList();
      this.PopulateRoomList();
    }

    private void cmdCancel_Click(object _sender, EventArgs _args)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void cmdOpen_Click(object _sender, EventArgs _args)
    {
      if (this.selectedRoom == null)
      {
        string str = "open";
        if (this.m_CurrentMode == frmRoomList.Mode.Duplicate)
          str = "duplicate";
        int num = (int) MessageBox.Show("Please select a room to " + str + ".", "error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        this.Close();
        this.DialogResult = DialogResult.OK;
      }
    }

    public new void Close()
    {
      if (this.m_Renderer != null)
      {
        this.m_Renderer.Dispose();
        this.m_Renderer = (RoomRenderer) null;
        this.pictureBox.Image = (Image) null;
      }
      base.Close();
    }

    private void roomNamesList_SelectedIndexChanged(object _sender, EventArgs _args)
    {
      if (!(this.roomNamesList.SelectedItem is frmRoomList.RoomInfo selectedItem))
      {
        this.selectedRoom = (JObject) null;
        if (this.m_Renderer == null)
          return;
        this.m_Renderer.Dispose();
        this.m_Renderer = (RoomRenderer) null;
        this.pictureBox.Image = (Image) null;
      }
      else
      {
        this.selectedRoom = selectedItem.data;
        try
        {
          if (this.m_Renderer != null)
            this.m_Renderer.Dispose();
          this.m_Renderer = new RoomRenderer(Layers.CreateLayers(this.selectedRoom));
          this.pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
          this.pictureBox.Image = (Image) this.m_Renderer.target;
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          this.pictureBox.Image = (Image) null;
        }
      }
    }

    private void RoomNamesList_DoubleClick(object _sender, EventArgs _args)
    {
      if (!(this.roomNamesList.SelectedItem is frmRoomList.RoomInfo))
        return;
      this.Close();
      this.DialogResult = DialogResult.OK;
    }

    private void roomGroup_SelectedIndexChanged(object _sender, EventArgs _args)
    {
      this.m_SelectedGroupIndex = this.roomGroupList.SelectedItem == null ? -1 : this.m_Separators.GetIndexForGroupName(this.roomGroupList.SelectedItem.ToString());
      this.PopulateRoomList();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.txtFilterRoom = new TextBox();
      this.roomNamesList = new ListBox();
      this.pictureBox = new PictureBox();
      this.cmdOpen = new Button();
      this.cmdCancel = new Button();
      this.roomGroupList = new ListBox();
      this.txtFilterGroup = new TextBox();
      ((ISupportInitialize) this.pictureBox).BeginInit();
      this.SuspendLayout();
      this.txtFilterRoom.Location = new Point(13, 209);
      this.txtFilterRoom.Name = "txtFilterRoom";
      this.txtFilterRoom.Size = new Size(190, 20);
      this.txtFilterRoom.TabIndex = 2;
      this.txtFilterRoom.Tag = (object) "Filter room list";
      this.txtFilterRoom.TextChanged += new EventHandler(this.txtFilter_TextChanged);
      this.roomNamesList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
      this.roomNamesList.FormattingEnabled = true;
      this.roomNamesList.Location = new Point(13, 235);
      this.roomNamesList.Name = "roomNamesList";
      this.roomNamesList.Size = new Size(190, 186);
      this.roomNamesList.TabIndex = 3;
      this.roomNamesList.SelectedIndexChanged += new EventHandler(this.roomNamesList_SelectedIndexChanged);
      this.pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pictureBox.Location = new Point(209, 13);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new Size(397, 436);
      this.pictureBox.TabIndex = 2;
      this.pictureBox.TabStop = false;
      this.cmdOpen.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.cmdOpen.Location = new Point(13, 427);
      this.cmdOpen.Name = "cmdOpen";
      this.cmdOpen.Size = new Size(75, 23);
      this.cmdOpen.TabIndex = 4;
      this.cmdOpen.Text = "Open";
      this.cmdOpen.UseVisualStyleBackColor = true;
      this.cmdOpen.Click += new EventHandler(this.cmdOpen_Click);
      this.cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.cmdCancel.Location = new Point(128, 427);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new Size(75, 23);
      this.cmdCancel.TabIndex = 5;
      this.cmdCancel.Text = "Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      this.cmdCancel.Click += new EventHandler(this.cmdCancel_Click);
      this.roomGroupList.FormattingEnabled = true;
      this.roomGroupList.Location = new Point(13, 39);
      this.roomGroupList.Name = "roomGroupList";
      this.roomGroupList.Size = new Size(190, 160);
      this.roomGroupList.TabIndex = 1;
      this.roomGroupList.SelectedIndexChanged += new EventHandler(this.roomGroup_SelectedIndexChanged);
      this.txtFilterGroup.Location = new Point(12, 13);
      this.txtFilterGroup.Name = "txtFilterGroup";
      this.txtFilterGroup.Size = new Size(190, 20);
      this.txtFilterGroup.TabIndex = 0;
      this.txtFilterGroup.Tag = (object) "Filter group list";
      this.txtFilterGroup.TextChanged += new EventHandler(this.txtFilterGroup_TextChanged);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(618, 461);
      this.Controls.Add((Control) this.txtFilterGroup);
      this.Controls.Add((Control) this.roomGroupList);
      this.Controls.Add((Control) this.cmdCancel);
      this.Controls.Add((Control) this.cmdOpen);
      this.Controls.Add((Control) this.pictureBox);
      this.Controls.Add((Control) this.roomNamesList);
      this.Controls.Add((Control) this.txtFilterRoom);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Name = nameof (frmRoomList);
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Open room";
      ((ISupportInitialize) this.pictureBox).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    public enum Mode
    {
      Open,
      Duplicate,
    }

    private class RoomInfo
    {
      public string name;
      public int groupIndex;
      public JObject data;

      public override string ToString() => this.name;
    }
  }
}
