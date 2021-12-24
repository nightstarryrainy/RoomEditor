// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmMarkerProperties
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmMarkerProperties : ToolForm
  {
    private MarkerInfo m_CurrentMarkerInfo;
    private bool m_bMultiSelection;
    private bool m_bInit;
    private Layers m_Layers;
    private const string m_MultiSelectionText = "Impossible to edit a multi-selection";
    private const string m_NoSelectionText = "No marker selected";
    private const string m_NoRoomOpenedText = "No room opened";
    private List<Tool> markers = new List<Tool>();
    private IContainer components;
    private Panel panel;
    private Label label;
    private Label label5;
    private TextBox txtCustomID;
    private Label label4;
    private Label lblMarkerType;
    private PictureBox pictureBox;
    private Label label2;
    private Label label1;
    private Label label9;
    private Label label8;
    private Label label7;
    private Label label6;
    private ComboBox cboLayer;
    private ComboBox cboLevel;
    private ComboBox cboMob;
    private ComboBox cboLight;
    private ComboBox cboItem;
    private Button cmdResetID;
    private Button cmdResetLayer;
    private Button cmdResetLevel;
    private Button cmdResetMob;
    private Button cmdResetLight;
    private Button cmdResetItem;
    private ListView markerList;
    private ImageList imageList;
    private Label label10;
    private Label label3;
    private NumericUpDown numHeight;
    private NumericUpDown numWidth;
    private NumericUpDown numY;
    private NumericUpDown numX;

    public Layers layers
    {
      get => this.m_Layers;
      set
      {
        Layers layers = this.m_Layers;
        this.m_Layers = value;
        if (this.m_Layers == layers)
          return;
        if (this.m_Layers != null)
          this.m_Layers.selection.markersChanged += new Selection.MarkerSelectionChangedHandler(this.OnMarkerSelectionChanged);
        this.UpdateMarkerInfo();
      }
    }

    public History history { get; set; }

    public frmMarkerProperties()
    {
      this.InitializeComponent();
      this.cboItem.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.cboItem.SelectedIndexChanged += new EventHandler(this.ForceValidateChildren);
      this.cboLayer.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.cboLayer.SelectedIndexChanged += new EventHandler(this.ForceValidateChildren);
      this.cboLevel.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.cboLevel.SelectedIndexChanged += new EventHandler(this.ForceValidateChildren);
      this.cboLight.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.cboLight.SelectedIndexChanged += new EventHandler(this.ForceValidateChildren);
      this.cboMob.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.cboMob.SelectedIndexChanged += new EventHandler(this.ForceValidateChildren);
      this.txtCustomID.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.numX.ValueChanged += new EventHandler(this.ForceValidateChildren);
      this.numX.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.numY.ValueChanged += new EventHandler(this.ForceValidateChildren);
      this.numY.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.numWidth.ValueChanged += new EventHandler(this.ForceValidateChildren);
      this.numWidth.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.numHeight.ValueChanged += new EventHandler(this.ForceValidateChildren);
      this.numHeight.Validated += new EventHandler(this.OnPropertyValueChanged);
      this.Deactivate += new EventHandler(this.ForceValidateChildren);
    }

    public void OnCDBChanged(JObject _cdbObject)
    {
      this.cboItem.Items.Clear();
      this.cboItem.Enabled = false;
      this.cboLayer.Items.Clear();
      this.cboLayer.Enabled = false;
      this.cboLevel.Items.Clear();
      this.cboLevel.Enabled = false;
      this.cboLight.Items.Clear();
      this.cboLight.Enabled = false;
      this.cboMob.Items.Clear();
      this.cboMob.Enabled = false;
      if (_cdbObject == null)
        return;
      this.OnCDBChangedAsyncImpl(_cdbObject);
    }

    private void ForceValidateChildren(object _sender, EventArgs _args) => this.ValidateChildren();

    private void OnPropertyValueChanged(object _sender, EventArgs _args)
    {
      if (this.m_bInit || this.history == null || this.m_CurrentMarkerInfo == null)
        return;
      HistoryEntry _entry = (HistoryEntry) null;
      if (_sender == this.txtCustomID)
      {
        if (this.m_CurrentMarkerInfo.tag != this.txtCustomID.Text && (this.txtCustomID.Text != "" || this.m_CurrentMarkerInfo.tag != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.tag, this.txtCustomID.Text, "tag");
          this.m_CurrentMarkerInfo.tag = this.txtCustomID.Text;
        }
      }
      else if (_sender == this.cboItem)
      {
        if (this.m_CurrentMarkerInfo.item != this.cboItem.Text && (this.cboItem.Text != "" || this.m_CurrentMarkerInfo.item != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.item, this.cboItem.Text, "item");
          this.m_CurrentMarkerInfo.item = this.cboItem.Text;
        }
      }
      else if (_sender == this.cboLayer)
      {
        if (this.m_CurrentMarkerInfo.layer != this.cboLayer.Text && (this.cboLayer.Text != "" || this.m_CurrentMarkerInfo.layer != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.layer, this.cboLayer.Text, "layer");
          this.m_CurrentMarkerInfo.layer = this.cboLayer.Text;
        }
      }
      else if (_sender == this.cboLevel)
      {
        if (this.m_CurrentMarkerInfo.level != this.cboLevel.Text && (this.cboLevel.Text != "" || this.m_CurrentMarkerInfo.level != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.level, this.cboLevel.Text, "level");
          this.m_CurrentMarkerInfo.level = this.cboLevel.Text;
        }
      }
      else if (_sender == this.cboLight)
      {
        if (this.m_CurrentMarkerInfo.light != this.cboLight.Text && (this.cboLight.Text != "" || this.m_CurrentMarkerInfo.light != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.light, this.cboLight.Text, "light");
          this.m_CurrentMarkerInfo.light = this.cboLight.Text;
        }
      }
      else if (_sender == this.cboMob)
      {
        if (this.m_CurrentMarkerInfo.mob != this.cboMob.Text && (this.cboMob.Text != "" || this.m_CurrentMarkerInfo.mob != null))
        {
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<string>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.mob, this.cboMob.Text, "mob");
          this.m_CurrentMarkerInfo.mob = this.cboMob.Text;
        }
      }
      else if (_sender == this.numX)
      {
        this.numX.Value = this.numX.Value.Clamp<Decimal>(0M, (Decimal) (this.layers.width - this.m_CurrentMarkerInfo.rect.Width));
        if ((Decimal) this.m_CurrentMarkerInfo.rect.Left != this.numX.Value)
        {
          Rectangle rect1 = this.m_CurrentMarkerInfo.rect;
          Rectangle rect2 = this.m_CurrentMarkerInfo.rect;
          rect2.X = (int) this.numX.Value;
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<Rectangle>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.rect, rect2, "rect");
          Rectangle _this = Rectangle.Union(this.m_CurrentMarkerInfo.rect, rect2);
          this.m_CurrentMarkerInfo.rect = rect2;
          frmMain.activeMain.viewport.InvalidateArea(_this.Multiplied(16));
          frmMain.activeMain.viewport.UpdatePictureBoxImage();
        }
      }
      else if (_sender == this.numY)
      {
        this.numY.Value = this.numY.Value.Clamp<Decimal>(0M, (Decimal) (this.layers.height - this.m_CurrentMarkerInfo.rect.Height));
        if ((Decimal) this.m_CurrentMarkerInfo.rect.Top != this.numY.Value)
        {
          Rectangle rect1 = this.m_CurrentMarkerInfo.rect;
          Rectangle rect2 = this.m_CurrentMarkerInfo.rect;
          rect2.Y = (int) this.numY.Value;
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<Rectangle>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.rect, rect2, "rect");
          Rectangle _this = Rectangle.Union(this.m_CurrentMarkerInfo.rect, rect2);
          this.m_CurrentMarkerInfo.rect = rect2;
          frmMain.activeMain.viewport.InvalidateArea(_this.Multiplied(16));
          frmMain.activeMain.viewport.UpdatePictureBoxImage();
        }
      }
      else if (_sender == this.numWidth)
      {
        this.numWidth.Value = this.numWidth.Value.Clamp<Decimal>(1M, (Decimal) (this.layers.width - this.m_CurrentMarkerInfo.rect.X));
        if ((Decimal) this.m_CurrentMarkerInfo.rect.Width != this.numWidth.Value)
        {
          Rectangle rect1 = this.m_CurrentMarkerInfo.rect;
          Rectangle rect2 = this.m_CurrentMarkerInfo.rect;
          rect2.Width = (int) this.numWidth.Value;
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<Rectangle>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.rect, rect2, "rect");
          Rectangle _this = Rectangle.Union(this.m_CurrentMarkerInfo.rect, rect2);
          this.m_CurrentMarkerInfo.rect = rect2;
          frmMain.activeMain.viewport.InvalidateArea(_this.Multiplied(16));
          frmMain.activeMain.viewport.UpdatePictureBoxImage();
        }
      }
      else if (_sender == this.numHeight)
      {
        this.numHeight.Value = this.numHeight.Value.Clamp<Decimal>(1M, (Decimal) (this.layers.height - this.m_CurrentMarkerInfo.rect.Y));
        if ((Decimal) this.m_CurrentMarkerInfo.rect.Height != this.numHeight.Value)
        {
          Rectangle rect1 = this.m_CurrentMarkerInfo.rect;
          Rectangle rect2 = this.m_CurrentMarkerInfo.rect;
          rect2.Height = (int) this.numHeight.Value;
          _entry = (HistoryEntry) new HistoryChangeMarkerPropertiesEntry<Rectangle>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.rect, rect2, "rect");
          Rectangle _this = Rectangle.Union(this.m_CurrentMarkerInfo.rect, rect2);
          this.m_CurrentMarkerInfo.rect = rect2;
          frmMain.activeMain.viewport.InvalidateArea(_this.Multiplied(16));
          frmMain.activeMain.viewport.UpdatePictureBoxImage();
        }
      }
      if (_entry == null)
        return;
      this.history.AddAction();
      this.history.AddEntry(_entry);
    }

    private void cmdResetID_Click(object _sender, EventArgs _args)
    {
      this.txtCustomID.Text = (string) null;
      this.ValidateChildren();
    }

    private void cmdResetItem_Click(object _sender, EventArgs _args)
    {
      this.cboItem.SelectedIndex = -1;
      this.cboItem.Text = (string) null;
      this.ValidateChildren();
    }

    private void cmdResetLight_Click(object _sender, EventArgs _args)
    {
      this.cboLight.SelectedIndex = -1;
      this.cboLight.Text = (string) null;
      this.ValidateChildren();
    }

    private void cmdResetMob_Click(object _sender, EventArgs _args)
    {
      this.cboMob.SelectedIndex = -1;
      this.cboMob.Text = (string) null;
      this.ValidateChildren();
    }

    private void cmdResetLevel_Click(object _sender, EventArgs _args)
    {
      this.cboLevel.SelectedIndex = -1;
      this.cboLevel.Text = (string) null;
      this.ValidateChildren();
    }

    private void cmdResetLayer_Click(object _sender, EventArgs _args)
    {
      this.cboLayer.SelectedIndex = -1;
      this.cboLayer.Text = (string) null;
      this.ValidateChildren();
    }

    public void Undo() => this.UpdateMarkerInfo();

    public void Redo() => this.UpdateMarkerInfo();

    private void frmMarkerProperties_Load(object _sender, EventArgs _args)
    {
      this.OnMarkerSelectionChanged();
      foreach (Tool allTool in Tools.allTools)
      {
        if (allTool.isMarker)
          this.markers.Add(allTool);
      }
      if (Options.instance.toolsSortMarkersAlphabetically)
        this.markers.Sort((Comparison<Tool>) ((tool1, tool2) => tool1.name.CompareTo(tool2.name)));
      foreach (Tool marker in this.markers)
        this.imageList.Images.Add((Image) marker.bitmap);
      this.markerList.SmallImageList = this.imageList;
      this.markerList.LargeImageList = this.imageList;
      this.markerList.KeyDown += new KeyEventHandler(this.OnMarkerListKeyDown);
      this.markerList.MouseDown += new MouseEventHandler(this.OnMarkerListMouseDown);
      this.markerList.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(this.OnMarkerListSelectionChanged);
    }

    private void OnMarkerListSelectionChanged(object _sender, EventArgs _args)
    {
      MarkerType markerTypeByValue = MarkerType.GetMarkerTypeByValue(this.markers[this.markerList.SelectedIndices[0]].value - 1024);
      if (this.m_CurrentMarkerInfo.markerType.id != markerTypeByValue.id)
      {
        this.history.AddAction();
        this.history.AddEntry((HistoryEntry) new HistoryChangeMarkerPropertiesEntry<MarkerType>(this.m_CurrentMarkerInfo, this.m_CurrentMarkerInfo.markerType, markerTypeByValue, "markerType"));
        this.m_CurrentMarkerInfo.markerType = markerTypeByValue;
        this.UpdateMarkerInfo();
        frmMain.activeMain.viewport.InvalidateArea(this.m_CurrentMarkerInfo.rect.Multiplied(16));
        frmMain.activeMain.viewport.UpdatePictureBoxImage();
      }
      this.markerList.Visible = false;
    }

    private void OnMarkerListMouseDown(object _sender, MouseEventArgs _args)
    {
      if (_args.Button != MouseButtons.Right)
        return;
      this.markerList.Visible = false;
    }

    private void OnMarkerListKeyDown(object _sender, KeyEventArgs _args)
    {
      if (_args.KeyCode != Keys.Escape)
        return;
      this.markerList.Visible = false;
    }

    private void OnMarkerSelectionChanged()
    {
      if (this.m_Layers != null)
      {
        if (this.m_Layers.selection.markers.Count<MarkerInfo>() > 1)
        {
          this.m_bMultiSelection = true;
          this.m_CurrentMarkerInfo = (MarkerInfo) null;
        }
        else
        {
          this.m_bMultiSelection = false;
          if (this.m_Layers.selection.markers.Count<MarkerInfo>((Func<MarkerInfo, bool>) (m => true)) == 1)
          {
            MarkerInfo markerInfo = this.m_Layers.selection.markers.First<MarkerInfo>();
            if (markerInfo != this.m_CurrentMarkerInfo)
              this.m_CurrentMarkerInfo = markerInfo;
          }
          else
            this.m_CurrentMarkerInfo = (MarkerInfo) null;
        }
      }
      this.UpdateMarkerInfo();
    }

    private Task OnCDBChangedAsyncImpl(JObject _cdbObject)
    {
      foreach (JObject _sheetObj in (IEnumerable<JToken>) _cdbObject["sheets"])
      {
        string str = _sheetObj["name"].ToString();
        if (str == "item")
          this.PopulateCombo(this.cboItem, _sheetObj, "id");
        else if (str == "layer")
          this.PopulateCombo(this.cboLayer, _sheetObj, "id");
        else if (str == "level")
          this.PopulateCombo(this.cboLevel, _sheetObj, "id");
        else if (str == "lightConf")
          this.PopulateCombo(this.cboLight, _sheetObj, "name");
        else if (str == "mob")
          this.PopulateCombo(this.cboMob, _sheetObj, "id");
      }
      return (Task) Task.FromResult<int>(0);
    }

    private void PopulateCombo(ComboBox _comboBox, JObject _sheetObj, string _keyColumnName)
    {
      JArray jarray = (JArray) _sheetObj["lines"];
      _comboBox.Sorted = true;
      foreach (JObject jobject in jarray)
        _comboBox.Items.Add((object) jobject[_keyColumnName].ToString());
      _comboBox.Enabled = true;
    }

    private void UpdateMarkerInfo()
    {
      this.SuspendLayout();
      this.panel.Visible = this.m_Layers != null && !this.m_bMultiSelection && this.m_CurrentMarkerInfo != null;
      if (this.m_CurrentMarkerInfo == null)
      {
        if (this.m_bMultiSelection)
          this.label.Text = "Impossible to edit a multi-selection";
        else if (this.m_Layers == null)
          this.label.Text = "No room opened";
        else
          this.label.Text = "No marker selected";
      }
      else
      {
        this.m_bInit = true;
        this.pictureBox.Image = (Image) Tools.GetToolForValue(this.m_CurrentMarkerInfo.value).bitmap;
        this.lblMarkerType.Text = this.m_CurrentMarkerInfo.markerType.id;
        this.numX.Value = (Decimal) this.m_CurrentMarkerInfo.rect.Left;
        NumericUpDown numY = this.numY;
        Rectangle rect = this.m_CurrentMarkerInfo.rect;
        Decimal top = (Decimal) rect.Top;
        numY.Value = top;
        NumericUpDown numWidth = this.numWidth;
        rect = this.m_CurrentMarkerInfo.rect;
        Decimal width = (Decimal) rect.Width;
        numWidth.Value = width;
        NumericUpDown numHeight = this.numHeight;
        rect = this.m_CurrentMarkerInfo.rect;
        Decimal height = (Decimal) rect.Height;
        numHeight.Value = height;
        this.txtCustomID.Text = this.m_CurrentMarkerInfo.tag;
        this.cboItem.Text = this.m_CurrentMarkerInfo.item;
        this.cboLayer.Text = this.m_CurrentMarkerInfo.layer;
        this.cboLevel.Text = this.m_CurrentMarkerInfo.level;
        this.cboLight.Text = this.m_CurrentMarkerInfo.light;
        this.cboMob.Text = this.m_CurrentMarkerInfo.mob;
        this.m_bInit = false;
      }
      this.ResumeLayout();
    }

    private void pictureBox_Click(object _sender, EventArgs _args)
    {
      this.markerList.Location = new Point(15, 15);
      this.markerList.Items.Clear();
      this.markerList.Columns.Add("", this.markerList.Width - 16);
      this.markerList.Visible = true;
      this.markerList.View = View.Tile;
      int num = 0;
      foreach (Tool marker in this.markers)
        this.markerList.Items.Add(marker.name, num++);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.components = (IContainer) new Container();
      this.panel = new Panel();
      this.label10 = new Label();
      this.label3 = new Label();
      this.numHeight = new NumericUpDown();
      this.numWidth = new NumericUpDown();
      this.numY = new NumericUpDown();
      this.numX = new NumericUpDown();
      this.cmdResetLayer = new Button();
      this.cmdResetLevel = new Button();
      this.cmdResetMob = new Button();
      this.cmdResetLight = new Button();
      this.cmdResetItem = new Button();
      this.cmdResetID = new Button();
      this.cboLayer = new ComboBox();
      this.cboLevel = new ComboBox();
      this.cboMob = new ComboBox();
      this.cboLight = new ComboBox();
      this.cboItem = new ComboBox();
      this.label9 = new Label();
      this.label8 = new Label();
      this.label7 = new Label();
      this.label6 = new Label();
      this.label5 = new Label();
      this.txtCustomID = new TextBox();
      this.label4 = new Label();
      this.lblMarkerType = new Label();
      this.pictureBox = new PictureBox();
      this.label2 = new Label();
      this.label1 = new Label();
      this.markerList = new ListView();
      this.label = new Label();
      this.imageList = new ImageList(this.components);
      this.panel.SuspendLayout();
      this.numHeight.BeginInit();
      this.numWidth.BeginInit();
      this.numY.BeginInit();
      this.numX.BeginInit();
      ((ISupportInitialize) this.pictureBox).BeginInit();
      this.SuspendLayout();
      this.panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.panel.Controls.Add((Control) this.label10);
      this.panel.Controls.Add((Control) this.label3);
      this.panel.Controls.Add((Control) this.numHeight);
      this.panel.Controls.Add((Control) this.numWidth);
      this.panel.Controls.Add((Control) this.numY);
      this.panel.Controls.Add((Control) this.numX);
      this.panel.Controls.Add((Control) this.cmdResetLayer);
      this.panel.Controls.Add((Control) this.cmdResetLevel);
      this.panel.Controls.Add((Control) this.cmdResetMob);
      this.panel.Controls.Add((Control) this.cmdResetLight);
      this.panel.Controls.Add((Control) this.cmdResetItem);
      this.panel.Controls.Add((Control) this.cmdResetID);
      this.panel.Controls.Add((Control) this.cboLayer);
      this.panel.Controls.Add((Control) this.cboLevel);
      this.panel.Controls.Add((Control) this.cboMob);
      this.panel.Controls.Add((Control) this.cboLight);
      this.panel.Controls.Add((Control) this.cboItem);
      this.panel.Controls.Add((Control) this.label9);
      this.panel.Controls.Add((Control) this.label8);
      this.panel.Controls.Add((Control) this.label7);
      this.panel.Controls.Add((Control) this.label6);
      this.panel.Controls.Add((Control) this.label5);
      this.panel.Controls.Add((Control) this.txtCustomID);
      this.panel.Controls.Add((Control) this.label4);
      this.panel.Controls.Add((Control) this.lblMarkerType);
      this.panel.Controls.Add((Control) this.pictureBox);
      this.panel.Controls.Add((Control) this.label2);
      this.panel.Controls.Add((Control) this.label1);
      this.panel.Location = new Point(15, 13);
      this.panel.Name = "panel";
      this.panel.Size = new Size(209, 236);
      this.panel.TabIndex = 0;
      this.label10.AutoSize = true;
      this.label10.Location = new Point(134, 42);
      this.label10.Name = "label10";
      this.label10.Size = new Size(10, 13);
      this.label10.TabIndex = 15;
      this.label10.Text = ";";
      this.label3.AutoSize = true;
      this.label3.Location = new Point(134, 68);
      this.label3.Name = "label3";
      this.label3.Size = new Size(12, 13);
      this.label3.TabIndex = 15;
      this.label3.Text = "x";
      this.numHeight.Location = new Point(152, 66);
      this.numHeight.Maximum = new Decimal(new int[4]
      {
        5000,
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
      this.numHeight.Size = new Size(54, 20);
      this.numHeight.TabIndex = 14;
      this.numHeight.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numWidth.Location = new Point(77, 66);
      this.numWidth.Maximum = new Decimal(new int[4]
      {
        5000,
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
      this.numWidth.Size = new Size(54, 20);
      this.numWidth.TabIndex = 13;
      this.numWidth.Value = new Decimal(new int[4]
      {
        1,
        0,
        0,
        0
      });
      this.numY.Location = new Point(152, 40);
      this.numY.Maximum = new Decimal(new int[4]
      {
        5000,
        0,
        0,
        0
      });
      this.numY.Name = "numY";
      this.numY.Size = new Size(54, 20);
      this.numY.TabIndex = 14;
      this.numX.Location = new Point(77, 40);
      this.numX.Maximum = new Decimal(new int[4]
      {
        5000,
        0,
        0,
        0
      });
      this.numX.Name = "numX";
      this.numX.Size = new Size(54, 20);
      this.numX.TabIndex = 13;
      this.cmdResetLayer.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetLayer.Location = new Point(189, 209);
      this.cmdResetLayer.Name = "cmdResetLayer";
      this.cmdResetLayer.Size = new Size(20, 20);
      this.cmdResetLayer.TabIndex = 11;
      this.cmdResetLayer.Text = "C";
      this.cmdResetLayer.UseVisualStyleBackColor = true;
      this.cmdResetLayer.Click += new EventHandler(this.cmdResetLayer_Click);
      this.cmdResetLevel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetLevel.Location = new Point(189, 186);
      this.cmdResetLevel.Name = "cmdResetLevel";
      this.cmdResetLevel.Size = new Size(20, 20);
      this.cmdResetLevel.TabIndex = 9;
      this.cmdResetLevel.Text = "C";
      this.cmdResetLevel.UseVisualStyleBackColor = true;
      this.cmdResetLevel.Click += new EventHandler(this.cmdResetLevel_Click);
      this.cmdResetMob.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetMob.Location = new Point(189, 161);
      this.cmdResetMob.Name = "cmdResetMob";
      this.cmdResetMob.Size = new Size(20, 20);
      this.cmdResetMob.TabIndex = 7;
      this.cmdResetMob.Text = "C";
      this.cmdResetMob.UseVisualStyleBackColor = true;
      this.cmdResetMob.Click += new EventHandler(this.cmdResetMob_Click);
      this.cmdResetLight.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetLight.Location = new Point(189, 137);
      this.cmdResetLight.Name = "cmdResetLight";
      this.cmdResetLight.Size = new Size(20, 20);
      this.cmdResetLight.TabIndex = 5;
      this.cmdResetLight.Text = "C";
      this.cmdResetLight.UseVisualStyleBackColor = true;
      this.cmdResetLight.Click += new EventHandler(this.cmdResetLight_Click);
      this.cmdResetItem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetItem.Location = new Point(189, 114);
      this.cmdResetItem.Name = "cmdResetItem";
      this.cmdResetItem.Size = new Size(20, 20);
      this.cmdResetItem.TabIndex = 3;
      this.cmdResetItem.Text = "C";
      this.cmdResetItem.UseVisualStyleBackColor = true;
      this.cmdResetItem.Click += new EventHandler(this.cmdResetItem_Click);
      this.cmdResetID.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetID.Location = new Point(189, 90);
      this.cmdResetID.Name = "cmdResetID";
      this.cmdResetID.Size = new Size(20, 20);
      this.cmdResetID.TabIndex = 1;
      this.cmdResetID.Text = "C";
      this.cmdResetID.UseVisualStyleBackColor = true;
      this.cmdResetID.Click += new EventHandler(this.cmdResetID_Click);
      this.cboLayer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboLayer.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cboLayer.FormattingEnabled = true;
      this.cboLayer.Location = new Point(77, 209);
      this.cboLayer.Name = "cboLayer";
      this.cboLayer.Size = new Size(111, 21);
      this.cboLayer.TabIndex = 10;
      this.cboLevel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboLevel.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cboLevel.FormattingEnabled = true;
      this.cboLevel.Location = new Point(77, 186);
      this.cboLevel.Name = "cboLevel";
      this.cboLevel.Size = new Size(111, 21);
      this.cboLevel.TabIndex = 8;
      this.cboMob.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboMob.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cboMob.FormattingEnabled = true;
      this.cboMob.Location = new Point(77, 161);
      this.cboMob.Name = "cboMob";
      this.cboMob.Size = new Size(111, 21);
      this.cboMob.TabIndex = 6;
      this.cboLight.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboLight.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cboLight.FormattingEnabled = true;
      this.cboLight.Location = new Point(77, 137);
      this.cboLight.Name = "cboLight";
      this.cboLight.Size = new Size(111, 21);
      this.cboLight.TabIndex = 4;
      this.cboItem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.cboItem.DropDownStyle = ComboBoxStyle.DropDownList;
      this.cboItem.FormattingEnabled = true;
      this.cboItem.Location = new Point(77, 113);
      this.cboItem.Name = "cboItem";
      this.cboItem.Size = new Size(111, 21);
      this.cboItem.TabIndex = 2;
      this.label9.Location = new Point(3, 212);
      this.label9.Name = "label9";
      this.label9.Size = new Size(65, 16);
      this.label9.TabIndex = 12;
      this.label9.Text = "Layer:";
      this.label8.Location = new Point(3, 188);
      this.label8.Name = "label8";
      this.label8.Size = new Size(65, 16);
      this.label8.TabIndex = 11;
      this.label8.Text = "Level:";
      this.label7.Location = new Point(3, 164);
      this.label7.Name = "label7";
      this.label7.Size = new Size(65, 16);
      this.label7.TabIndex = 10;
      this.label7.Text = "Mob:";
      this.label6.Location = new Point(3, 140);
      this.label6.Name = "label6";
      this.label6.Size = new Size(65, 16);
      this.label6.TabIndex = 9;
      this.label6.Text = "Light:";
      this.label5.Location = new Point(3, 116);
      this.label5.Name = "label5";
      this.label5.Size = new Size(65, 16);
      this.label5.TabIndex = 8;
      this.label5.Text = "Item:";
      this.txtCustomID.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
      this.txtCustomID.Location = new Point(77, 90);
      this.txtCustomID.Name = "txtCustomID";
      this.txtCustomID.Size = new Size(111, 20);
      this.txtCustomID.TabIndex = 0;
      this.label4.Location = new Point(3, 92);
      this.label4.Name = "label4";
      this.label4.Size = new Size(65, 16);
      this.label4.TabIndex = 6;
      this.label4.Text = "Custom id:";
      this.lblMarkerType.Location = new Point(74, 12);
      this.lblMarkerType.Name = "lblMarkerType";
      this.lblMarkerType.Size = new Size(100, 16);
      this.lblMarkerType.TabIndex = 3;
      this.pictureBox.Location = new Point(6, 4);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new Size(32, 32);
      this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
      this.pictureBox.TabIndex = 2;
      this.pictureBox.TabStop = false;
      this.pictureBox.Click += new EventHandler(this.pictureBox_Click);
      this.label2.Location = new Point(3, 68);
      this.label2.Name = "label2";
      this.label2.Size = new Size(65, 16);
      this.label2.TabIndex = 1;
      this.label2.Text = "Size: ";
      this.label1.Location = new Point(3, 44);
      this.label1.Name = "label1";
      this.label1.Size = new Size(65, 16);
      this.label1.TabIndex = 0;
      this.label1.Text = "Position: ";
      this.markerList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.markerList.HeaderStyle = ColumnHeaderStyle.None;
      this.markerList.Location = new Point(228, 23);
      this.markerList.Name = "markerList";
      this.markerList.Size = new Size(194, 220);
      this.markerList.TabIndex = 13;
      this.markerList.UseCompatibleStateImageBehavior = false;
      this.markerList.Visible = false;
      this.label.Anchor = AnchorStyles.Left | AnchorStyles.Right;
      this.label.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, (byte) 0);
      this.label.Location = new Point(12, 104);
      this.label.Name = "label";
      this.label.Size = new Size(210, 48);
      this.label.TabIndex = 2;
      this.label.Text = "Impossible to edit a multi-selection";
      this.label.TextAlign = ContentAlignment.TopCenter;
      this.imageList.ColorDepth = ColorDepth.Depth32Bit;
      this.imageList.ImageSize = new Size(16, 16);
      this.imageList.TransparentColor = Color.Transparent;
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.AutoValidate = AutoValidate.EnableAllowFocusChange;
      this.ClientSize = new Size(234, 261);
      this.Controls.Add((Control) this.markerList);
      this.Controls.Add((Control) this.panel);
      this.Controls.Add((Control) this.label);
      this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
      this.Name = nameof (frmMarkerProperties);
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Marker properties";
      this.Load += new EventHandler(this.frmMarkerProperties_Load);
      this.panel.ResumeLayout(false);
      this.panel.PerformLayout();
      this.numHeight.EndInit();
      this.numWidth.EndInit();
      this.numY.EndInit();
      this.numX.EndInit();
      ((ISupportInitialize) this.pictureBox).EndInit();
      this.ResumeLayout(false);
    }
  }
}
