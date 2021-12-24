// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmNewRoom
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmNewRoom : ToolForm
  {
    private List<string> m_RoomNameList = new List<string>();
    private Separators m_Separators;
    private List<string> m_RoomTypes = new List<string>();
    private IContainer components;
    private ComboBox cboGroup;
    private ComboBox cboType;
    private Label label1;
    private Label label2;
    private CheckBox chkActive;
    private GroupBox grpFlags;
    private FlowLayoutPanel flowLayoutPanel1;
    private CheckBox chkLinkFlip;
    private CheckBox chkOutside;
    private CheckBox chkNoFlip;
    private CheckBox chkHoles;
    private CheckBox chkFatalFall;
    private NumericUpDown numHeight;
    private NumericUpDown numWidth;
    private Label label3;
    private Label label4;
    private Button cmdOK;
    private Button cmdCancel;
    private Label label5;
    private TextBox txtName;

    public JObject roomSheet { get; private set; }

    public int width => (int) this.numWidth.Value;

    public int height => (int) this.numHeight.Value;

    public string name => this.txtName.Text;

    public JObject roomObject { get; private set; }

    public bool changed { get; private set; }

    public frmNewRoom(JObject _roomSheet, JObject _roomTypeSheet, JObject _roomObjectToEdit = null)
    {
      this.roomSheet = _roomSheet;
      this.InitializeComponent();
      this.m_Separators = new Separators(_roomSheet);
      this.PopulateSeparatorCombo();
      foreach (JObject jobject in (IEnumerable<JToken>) _roomTypeSheet["lines"])
        this.m_RoomTypes.Add(jobject["id"].ToString());
      this.m_RoomTypes.Sort();
      this.PopulateTypeCombo();
      foreach (JObject jobject in (IEnumerable<JToken>) this.roomSheet["lines"])
        this.m_RoomNameList.Add(jobject["id"].ToString());
      this.roomObject = _roomObjectToEdit;
      this.changed = this.roomObject == null;
    }

    private void frmNewRoom_Load(object _sender, EventArgs _args)
    {
      this.cboType.TextChanged += new EventHandler(this.CboType_TextChanged);
      this.cboGroup.TextChanged += new EventHandler(this.CboGroup_TextChanged);
      if (this.roomObject != null)
      {
        this.txtName.Text = this.roomObject["id"].ToString();
        this.cboType.SelectedItem = (object) this.roomObject["type"].ToString();
        bool result1;
        bool.TryParse(this.roomObject["active"].ToString(), out result1);
        this.chkActive.Checked = result1;
        int result2;
        int.TryParse(this.roomObject["flags"].ToString(), out result2);
        this.chkLinkFlip.Checked = (uint) (result2 & 1) > 0U;
        this.chkOutside.Checked = (uint) (result2 & 2) > 0U;
        this.chkNoFlip.Checked = (uint) (result2 & 4) > 0U;
        this.chkHoles.Checked = (uint) (result2 & 8) > 0U;
        this.chkFatalFall.Checked = (uint) (result2 & 16) > 0U;
        this.numWidth.Value = (Decimal) int.Parse(this.roomObject["width"].ToString());
        this.numHeight.Value = (Decimal) int.Parse(this.roomObject["height"].ToString());
        this.cboGroup.SelectedIndex = int.Parse(this.roomObject["group"].ToString());
        this.Text = "Edit Room";
      }
      else
        this.Text = "Add a new Room";
    }

    private void CboGroup_TextChanged(object sender, EventArgs e)
    {
      if (this.cboGroup.Text == "" || !this.m_Separators.DoesSeparatorExist(this.cboGroup.Text))
        this.cboGroup.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      else
        this.cboGroup.BackColor = Color.White;
    }

    private void CboType_TextChanged(object _sender, EventArgs _args)
    {
      if (this.cboType.Text == "" || !this.m_RoomTypes.Contains(this.cboType.Text))
        this.cboType.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      else
        this.cboType.BackColor = Color.White;
    }

    private bool IsRoomNameValid()
    {
      if (this.txtName.Text != "" && !this.m_RoomNameList.Contains(this.txtName.Text))
        return true;
      if (this.roomObject == null || !(this.txtName.Text != ""))
        return false;
      return !this.m_RoomNameList.Contains(this.txtName.Text) || this.txtName.Text == this.roomObject["id"].ToString();
    }

    private void PopulateTypeCombo()
    {
      this.cboType.Items.Clear();
      string textUpper = this.cboType.Text.ToUpper();
      foreach (object obj in this.m_RoomTypes.FindAll((Predicate<string>) (s => s.ToUpper().IndexOf(textUpper) != -1)))
        this.cboType.Items.Add(obj);
    }

    private void PopulateSeparatorCombo()
    {
      this.cboGroup.Items.Clear();
      foreach (SeparatorInfo separatorInfo in this.m_Separators.FilterByName(this.cboGroup.Text))
        this.cboGroup.Items.Add((object) separatorInfo);
    }

    private void cmdCancel_Click(object _sender, EventArgs _args)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void cmdOK_Click(object _sender, EventArgs _args)
    {
      if (!this.IsRoomNameValid())
      {
        int num1 = (int) MessageBox.Show("Please set a unique name for the new room", "Wrong name", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else if (this.cboType.Text == "" || !this.m_RoomTypes.Contains(this.cboType.Text))
      {
        int num2 = (int) MessageBox.Show("Please choose an existing type for the new room", "Wrong type", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else if (this.cboGroup.Text == "" || !this.m_Separators.DoesSeparatorExist(this.cboGroup.Text))
      {
        int num3 = (int) MessageBox.Show("Please choose an existing group for the new room", "Wrong group", MessageBoxButtons.OK, MessageBoxIcon.Hand);
      }
      else
      {
        int num4 = 0;
        if (this.chkLinkFlip.Checked)
          num4 |= 1;
        if (this.chkOutside.Checked)
          num4 |= 2;
        if (this.chkNoFlip.Checked)
          num4 |= 4;
        if (this.chkHoles.Checked)
          num4 |= 8;
        if (this.chkFatalFall.Checked)
          num4 |= 16;
        this.m_Separators.InsertLineInGroup(this.cboGroup.Text,this.txtName.Text);
        if (this.roomObject == null)
        {
          this.roomObject = new JObject();
          this.roomObject.Add("id", (JToken) this.txtName.Text);
          this.roomObject.Add("type", (JToken) this.cboType.Text);
          this.roomObject.Add("active", (JToken) this.chkActive.Checked);
          this.roomObject.Add("flags", (JToken) num4);
          this.roomObject.Add("width", (JToken) this.width);
          this.roomObject.Add("height", (JToken) this.height);
          this.roomObject.Add("props", (JToken) JsonConvert.DeserializeObject("{\"tileSize\": " + (object) 16 + ",\"layers\":[{\"l\":\"col\",\"p\":{\"alpha\":1}},{\"l\":\"lnk\",\"p\":{\"alpha\":1}},{\"l\":\"markers\",\"p\":{\"alpha\":1}}]}"));
          this.roomObject.Add("tileProps", (JToken) new JArray());
          this.roomObject.Add("layers", (JToken) new JArray()
          {
            (JToken) new JObject()
            {
              {
                "name",
                (JToken) "col"
              },
              {
                "data",
                (JToken) new JObject()
                {
                  {
                    "file",
                    (JToken) "editor/cols.png"
                  },
                  {
                    "size",
                    (JToken) 16
                  },
                  {
                    "stride",
                    (JToken) 4
                  },
                  {
                    "data",
                    (JToken) Convert.ToBase64String(new byte[this.width * this.height * 2])
                  }
                }
              }
            },
            (JToken) new JObject()
            {
              {
                "name",
                (JToken) "lnk"
              },
              {
                "data",
                (JToken) new JObject()
                {
                  {
                    "file",
                    (JToken) "editor/links.png"
                  },
                  {
                    "size",
                    (JToken) 16
                  },
                  {
                    "stride",
                    (JToken) 4
                  },
                  {
                    "data",
                    (JToken) Convert.ToBase64String(new byte[this.width * this.height * 2])
                  }
                }
              }
            }
          });
          this.roomObject.Add("markers", (JToken) new JArray());
          SeparatorInfo _info;
          this.roomObject.Add("group", (JToken) this.m_Separators.GetIndexForGroupName(this.cboGroup.Text, out _info));
          ((JArray) this.roomSheet["lines"]).Insert(_info.lineIndex, (JToken) this.roomObject);
        }
        else
        {
          if (this.roomObject["id"].ToString() != this.txtName.Text)
          {
            this.roomObject["id"] = (JToken) this.txtName.Text;
            this.changed = true;
          }
          if (this.roomObject["type"].ToString() != this.cboType.Text)
          {
            this.roomObject["type"] = (JToken) this.cboType.Text;
            this.changed = true;
          }
          if (this.roomObject["active"].ToString() != this.chkActive.Checked.ToString())
          {
            this.roomObject["active"] = (JToken) this.chkActive.Checked;
            this.changed = true;
          }
          if (this.roomObject["flags"].ToString() != num4.ToString())
          {
            this.roomObject["flags"] = (JToken) num4;
            this.changed = true;
          }
          string str1 = this.roomObject["width"].ToString();
          int num5 = this.width;
          string str2 = num5.ToString();
          if (str1 != str2)
          {
            this.roomObject["width"] = (JToken) this.width;
            this.changed = true;
          }
          string str3 = this.roomObject["height"].ToString();
          num5 = this.height;
          string str4 = num5.ToString();
          if (str3 != str4)
          {
            this.roomObject["height"] = (JToken) this.height;
            this.changed = true;
          }
          int indexForGroupName = this.m_Separators.GetIndexForGroupName(this.cboGroup.Text, out SeparatorInfo _);
          if (this.roomObject.ContainsKey("group"))
          {
              if (this.roomObject["group"].ToString() != indexForGroupName.ToString()) {
                  this.roomObject["group"] = (JToken)indexForGroupName;
                  this.changed = true;
              }
          }
          else
          {
                  this.roomObject["group"] = (JToken)indexForGroupName;
                  this.changed = true;
          }
    
        }
        this.DialogResult = DialogResult.OK;
        this.Close();
      }
    }

    private void txtName_TextChanged(object _sender, EventArgs _args)
    {
      if (!this.IsRoomNameValid())
        this.txtName.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      else
        this.txtName.BackColor = Color.White;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.txtName = new TextBox();
      this.label5 = new Label();
      this.cmdOK = new Button();
      this.cmdCancel = new Button();
      this.numHeight = new NumericUpDown();
      this.numWidth = new NumericUpDown();
      this.label3 = new Label();
      this.label4 = new Label();
      this.grpFlags = new GroupBox();
      this.flowLayoutPanel1 = new FlowLayoutPanel();
      this.chkLinkFlip = new CheckBox();
      this.chkOutside = new CheckBox();
      this.chkNoFlip = new CheckBox();
      this.chkHoles = new CheckBox();
      this.chkFatalFall = new CheckBox();
      this.chkActive = new CheckBox();
      this.label2 = new Label();
      this.label1 = new Label();
      this.cboType = new ComboBox();
      this.cboGroup = new ComboBox();
      this.numHeight.BeginInit();
      this.numWidth.BeginInit();
      this.grpFlags.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      this.txtName.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      this.txtName.Location = new Point(99, 13);
      this.txtName.Name = "txtName";
      this.txtName.Size = new Size(123, 20);
      this.txtName.TabIndex = 0;
      this.txtName.TextChanged += new EventHandler(this.txtName_TextChanged);
      this.label5.AutoSize = true;
      this.label5.Location = new Point(13, 16);
      this.label5.Name = "label5";
      this.label5.Size = new Size(58, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Name (id) :";
      this.cmdOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      this.cmdOK.Location = new Point(15, 274);
      this.cmdOK.Name = "cmdOK";
      this.cmdOK.Size = new Size(75, 23);
      this.cmdOK.TabIndex = 7;
      this.cmdOK.Text = "&OK";
      this.cmdOK.UseVisualStyleBackColor = true;
      this.cmdOK.Click += new EventHandler(this.cmdOK_Click);
      this.cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
      this.cmdCancel.DialogResult = DialogResult.Cancel;
      this.cmdCancel.Location = new Point(156, 274);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new Size(75, 23);
      this.cmdCancel.TabIndex = 8;
      this.cmdCancel.Text = "&Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      this.cmdCancel.Click += new EventHandler(this.cmdCancel_Click);
      this.numHeight.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.numHeight.Location = new Point(93, 230);
      this.numHeight.Maximum = new Decimal(new int[4]
      {
        256,
        0,
        0,
        0
      });
      this.numHeight.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numHeight.Name = "numHeight";
      this.numHeight.Size = new Size(138, 20);
      this.numHeight.TabIndex = 6;
      this.numHeight.TextAlign = HorizontalAlignment.Right;
      this.numHeight.Value = new Decimal(new int[4]
      {
        15,
        0,
        0,
        0
      });
      this.numWidth.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.numWidth.Location = new Point(93, 204);
      this.numWidth.Maximum = new Decimal(new int[4]
      {
        256,
        0,
        0,
        0
      });
      this.numWidth.Minimum = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numWidth.Name = "numWidth";
      this.numWidth.Size = new Size(138, 20);
      this.numWidth.TabIndex = 5;
      this.numWidth.TextAlign = HorizontalAlignment.Right;
      this.numWidth.Value = new Decimal(new int[4]
      {
        20,
        0,
        0,
        0
      });
      this.label3.Location = new Point(12, 228);
      this.label3.Name = "label3";
      this.label3.Size = new Size(75, 13);
      this.label3.TabIndex = 9;
      this.label3.Text = "Height:";
      this.label4.Location = new Point(12, 206);
      this.label4.Name = "label4";
      this.label4.Size = new Size(75, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Width:";
      this.grpFlags.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.grpFlags.Controls.Add((Control) this.flowLayoutPanel1);
      this.grpFlags.Location = new Point(15, 115);
      this.grpFlags.Name = "grpFlags";
      this.grpFlags.Size = new Size(216, 78);
      this.grpFlags.TabIndex = 4;
      this.grpFlags.TabStop = false;
      this.grpFlags.Text = "  Flags  ";
      this.flowLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.flowLayoutPanel1.Controls.Add((Control) this.chkLinkFlip);
      this.flowLayoutPanel1.Controls.Add((Control) this.chkOutside);
      this.flowLayoutPanel1.Controls.Add((Control) this.chkNoFlip);
      this.flowLayoutPanel1.Controls.Add((Control) this.chkHoles);
      this.flowLayoutPanel1.Controls.Add((Control) this.chkFatalFall);
      this.flowLayoutPanel1.Location = new Point(7, 20);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new Size(203, 52);
      this.flowLayoutPanel1.TabIndex = 0;
      this.chkLinkFlip.AutoSize = true;
      this.chkLinkFlip.Location = new Point(3, 3);
      this.chkLinkFlip.Name = "chkLinkFlip";
      this.chkLinkFlip.Size = new Size(62, 17);
      this.chkLinkFlip.TabIndex = 0;
      this.chkLinkFlip.Text = "LinkFlip";
      this.chkLinkFlip.UseVisualStyleBackColor = true;
      this.chkOutside.AutoSize = true;
      this.chkOutside.Location = new Point(71, 3);
      this.chkOutside.Name = "chkOutside";
      this.chkOutside.Size = new Size(62, 17);
      this.chkOutside.TabIndex = 1;
      this.chkOutside.Text = "Outside";
      this.chkOutside.UseVisualStyleBackColor = true;
      this.chkNoFlip.AutoSize = true;
      this.chkNoFlip.Location = new Point(139, 3);
      this.chkNoFlip.Name = "chkNoFlip";
      this.chkNoFlip.Size = new Size(56, 17);
      this.chkNoFlip.TabIndex = 2;
      this.chkNoFlip.Text = "NoFlip";
      this.chkNoFlip.UseVisualStyleBackColor = true;
      this.chkHoles.AutoSize = true;
      this.chkHoles.Location = new Point(3, 26);
      this.chkHoles.Name = "chkHoles";
      this.chkHoles.Size = new Size(53, 17);
      this.chkHoles.TabIndex = 3;
      this.chkHoles.Text = "Holes";
      this.chkHoles.UseVisualStyleBackColor = true;
      this.chkFatalFall.AutoSize = true;
      this.chkFatalFall.Location = new Point(62, 26);
      this.chkFatalFall.Name = "chkFatalFall";
      this.chkFatalFall.Size = new Size(65, 17);
      this.chkFatalFall.TabIndex = 4;
      this.chkFatalFall.Text = "FatalFall";
      this.chkFatalFall.UseVisualStyleBackColor = true;
      this.chkActive.AutoSize = true;
      this.chkActive.Checked = true;
      this.chkActive.CheckState = CheckState.Checked;
      this.chkActive.Location = new Point(15, 91);
      this.chkActive.Name = "chkActive";
      this.chkActive.Size = new Size(56, 17);
      this.chkActive.TabIndex = 3;
      this.chkActive.Text = "Active";
      this.chkActive.UseVisualStyleBackColor = true;
      this.label2.AutoSize = true;
      this.label2.Location = new Point(12, 67);
      this.label2.Name = "label2";
      this.label2.Size = new Size(64, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Room type :";
      this.label1.AutoSize = true;
      this.label1.Location = new Point(12, 40);
      this.label1.Name = "label1";
      this.label1.Size = new Size(82, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Add into group :";
      this.cboType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboType.AutoCompleteMode = AutoCompleteMode.Suggest;
      this.cboType.AutoCompleteSource = AutoCompleteSource.ListItems;
      this.cboType.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      this.cboType.FormattingEnabled = true;
      this.cboType.Location = new Point(99, 64);
      this.cboType.Name = "cboType";
      this.cboType.Size = new Size(132, 21);
      this.cboType.TabIndex = 2;
      this.cboGroup.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboGroup.AutoCompleteMode = AutoCompleteMode.Suggest;
      this.cboGroup.AutoCompleteSource = AutoCompleteSource.ListItems;
      this.cboGroup.BackColor = Color.FromArgb((int) byte.MaxValue, 192, 192);
      this.cboGroup.FormattingEnabled = true;
      this.cboGroup.Location = new Point(99, 37);
      this.cboGroup.Name = "cboGroup";
      this.cboGroup.Size = new Size(132, 21);
      this.cboGroup.TabIndex = 1;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(243, 309);
      this.Controls.Add((Control) this.txtName);
      this.Controls.Add((Control) this.label5);
      this.Controls.Add((Control) this.cmdOK);
      this.Controls.Add((Control) this.cmdCancel);
      this.Controls.Add((Control) this.numHeight);
      this.Controls.Add((Control) this.numWidth);
      this.Controls.Add((Control) this.label3);
      this.Controls.Add((Control) this.label4);
      this.Controls.Add((Control) this.grpFlags);
      this.Controls.Add((Control) this.chkActive);
      this.Controls.Add((Control) this.label2);
      this.Controls.Add((Control) this.label1);
      this.Controls.Add((Control) this.cboType);
      this.Controls.Add((Control) this.cboGroup);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.MaximizeBox = false;
      this.Name = nameof (frmNewRoom);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "Add a new Room";
      this.Load += new EventHandler(this.frmNewRoom_Load);
      this.numHeight.EndInit();
      this.numWidth.EndInit();
      this.grpFlags.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
