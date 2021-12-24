// Decompiled with JetBrains decompiler
// Type: RoomEditor.frmMain
// Assembly: RoomEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 998858C2-721B-4132-B16A-2732F6B1A670
// Assembly location: E:\ModTools\RoomEditor.exe

using Microsoft.VisualBasic;
using ModTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace RoomEditor
{
  public class frmMain : ToolForm
  {
    public static Label debugLabel;
    private Layers m_Layers;
    private Viewport m_Viewport;
    private JObject m_JsonRoomObj;
    private JObject m_CDBObj;
    private JArray m_Rooms;
    private JObject m_RoomSheet;
    private JObject m_RoomTypeSheet;
    private bool m_bChanged;
    private History m_History;
    private Dictionary<string, JObject> m_RoomsObj = new Dictionary<string, JObject>();
    private string m_FilePath = "";
    private ClipboardData clipboard = new ClipboardData();
    private IContainer components;
    private InterpolablePictureBox pictureBox;
    private MenuStrip menuStrip;
    public ToolStripMenuItem fileToolStripMenuItem;
    public ToolStripMenuItem exitToolStripMenuItem;
    public ToolStripMenuItem toolsToolStripMenuItem;
    public ToolStripMenuItem toolWindowToolStripMenuItem;
    public ToolStripMenuItem openToolStripMenuItem;
    public ToolStripSeparator toolStripMenuItem1;
    public ToolStripMenuItem saveRoomToolStripMenuItem;
    public ToolStripMenuItem editToolStripMenuItem;
    public ToolStripMenuItem undoToolStripMenuItem;
    public ToolStripMenuItem redoToolStripMenuItem;
    public ToolStripMenuItem roomToolStripMenuItem;
    public ToolStripMenuItem resizeToolStripMenuItem;
    public ToolStripSeparator toolStripMenuItem2;
    public ToolStripMenuItem previewWindowToolStripMenuItem;
    private ToolStripMenuItem openCDBToolStripMenuItem;
    private ToolStripMenuItem newRoomToolStripMenuItem;
    private ToolStripMenuItem layersToolStripMenuItem;
    private ToolStripMenuItem historyToolStripMenuItem;
    private ToolStripMenuItem markerPropertiesToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem3;
    private ToolStripMenuItem resetPositionsToolStripMenuItem;
    private Button cmdResetZoom;
    private TextBox txtZoom;
    private Label lblZoomLevel;
    private ToolStripMenuItem controlsToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem5;
    private ToolStripMenuItem OptionsToolStripMenuItem;
    private ToolStripMenuItem exportRoomToolStripMenuItem;
    private ToolStripMenuItem resetMainWindowPositionToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ToolStripMenuItem pasteToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem4;
    private StatusStrip statusStrip;
    private ToolStripStatusLabel lblCursorPosition;
    private ToolStripStatusLabel lblTilePosition;
    private ToolStripStatusLabel lblToolAreaSelection;
    private ToolStripMenuItem displayToolStripMenuItem;
    private ToolStripMenuItem showGridsToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem6;
    private ToolStripMenuItem selectAllToolStripMenuItem;
    private ToolStripMenuItem editRoomToolStripMenuItem;
    private ToolStripSeparator toolStripMenuItem7;
    private ToolStripMenuItem duplicateRoomToolStripMenuItem;

    public static frmTools toolsForm { get; private set; }

    public static frmControls controlsForm { get; private set; }

    public static frmOptions optionsForm { get; private set; }

    public static frmViewportPreview previewForm { get; private set; }

    public static frmLayers layersForm { get; private set; }

    public static frmHistory historyForm { get; private set; }

    public static frmMarkerProperties markerPropertiesForm { get; private set; }

    public static frmMain activeMain { get; private set; }

    public Viewport viewport => this.m_Viewport;

    public frmMain()
    {
      Options.Load();
      this.InitializeComponent();
      frmMain.activeMain = this;
      this.KeyPress += new KeyPressEventHandler(this.FrmMain_KeyPress);
    }

    public void ResetToolFormsPositions()
    {
      Options.instance.ResetToolsWindowProps();
      if (frmMain.historyForm != null)
        this.CreateHistoryForm();
      if (frmMain.toolsForm != null)
        this.CreateToolsForm();
      if (frmMain.previewForm != null)
        this.CreatePreviewForm();
      if (frmMain.layersForm != null)
        this.CreateLayersForm();
      if (frmMain.markerPropertiesForm != null)
        this.CreateMarkerPropertiesForm();
      if (frmMain.controlsForm == null)
        return;
      this.CreateControlsForm();
    }

    private void FrmMain_KeyPress(object _sender, KeyPressEventArgs _args)
    {
      if (this.m_Viewport == null)
        return;
      if (_args.KeyChar == '+')
      {
        this.m_Viewport.Zoom(1);
      }
      else
      {
        if (_args.KeyChar != '-')
          return;
        this.m_Viewport.Zoom(-1);
      }
    }

    private void TxtZoom_TextChanged(object _sender, EventArgs _args)
    {
    }

    private void TxtZoom_KeyDown(object _sender, KeyEventArgs _args)
    {
      if (_args.KeyCode != Keys.Return)
        return;
      this.lblZoomLevel.Focus();
    }

    private void TxtZoom_Validating(object _sender, CancelEventArgs _args)
    {
      int result = 0;
      if (int.TryParse(this.txtZoom.Text, out result))
        this.txtZoom.Text = result.Clamp<int>(10, 500).ToString() + " %";
      else
        _args.Cancel = true;
    }

    private void TxtZoom_KeyPress(object _sender, KeyPressEventArgs _args)
    {
      if (char.IsDigit(_args.KeyChar) || char.IsControl(_args.KeyChar))
        return;
      _args.Handled = true;
    }

    private void cmdResetZoom_Click(object _sender, EventArgs _args)
    {
      if (this.m_Viewport == null)
        return;
      this.m_Viewport.ResetZoom();
    }

    private void frmMain_Load(object _sender, EventArgs _args)
    {
      if (!Options.instance.LoadWindowProps((ToolForm) this))
        this.ResetPosition();
      this.UpdateTitle();
      Tools.Init();
      this.CreateToolsForm();
      this.KeyDown += new KeyEventHandler(this.OnKeyboardKeyDown);
      this.KeyUp += new KeyEventHandler(this.OnKeyboardkeyUp);
      Tools.currentToolChanged += new Tools.ToolChangedHandler(this.OnToolChanged);
      this.saveRoomToolStripMenuItem.Enabled = false;
      this.exportRoomToolStripMenuItem.Enabled = false;
      this.newRoomToolStripMenuItem.Enabled = false;
      this.openToolStripMenuItem.Enabled = false;
      this.duplicateRoomToolStripMenuItem.Enabled = false;
      this.undoToolStripMenuItem.Enabled = false;
      this.redoToolStripMenuItem.Enabled = false;
      this.copyToolStripMenuItem.Enabled = false;
      this.pasteToolStripMenuItem.Enabled = false;
      this.selectAllToolStripMenuItem.Enabled = false;
      this.resizeToolStripMenuItem.Enabled = false;
      this.editRoomToolStripMenuItem.Enabled = false;
      this.showGridsToolStripMenuItem.Enabled = false;
      this.showGridsToolStripMenuItem.Checked = Options.instance.displayShowGrids;
      this.txtZoom.Enabled = false;
      this.cmdResetZoom.Enabled = false;
      this.lblZoomLevel.Enabled = false;
      Action action1 = new Action(Action.Type.Undo, "Undo", new HotKey(Keys.Control, Keys.Z), _menuItem: this.undoToolStripMenuItem);
      Action action2 = new Action(Action.Type.Redo, "Redo", new HotKey(Keys.Control, Keys.Y), _menuItem: this.redoToolStripMenuItem);
      Action action3 = new Action(Action.Type.NewRoom, "New Room", new HotKey(Keys.Control, Keys.O), _menuItem: this.openToolStripMenuItem);
      Action action4 = new Action(Action.Type.ResizeRoom, "Resize Room", new HotKey(Keys.Control, Keys.R), _menuItem: this.resizeToolStripMenuItem);
      Action action5 = new Action(Action.Type.SaveRoom, "Save Room", new HotKey(Keys.Control, Keys.S), _menuItem: this.saveRoomToolStripMenuItem);
      Action action6 = new Action(Action.Type.ToggleTools, "Toggle Tools", new HotKey(_key: Keys.F5), _menuItem: this.toolsToolStripMenuItem);
      Action action7 = new Action(Action.Type.TogglePreview, "Toggle Preview", new HotKey(_key: Keys.F6), _menuItem: this.previewWindowToolStripMenuItem);
      this.lblCursorPosition.Text = "";
      this.lblToolAreaSelection.Text = "";
    }

    private void OpenRoom(JObject _roomObj)
    {
      int num1 = 0;
      int num2 = 0;
      try
      {
        this.m_Layers = Layers.CreateLayers(_roomObj);
        this.m_Layers.dataChanged += new Layers.LayersEventHandler(this.OnLayersDataChanged);
        this.m_Layers.selection.changed += new Selection.ChangedHandler(this.OnLayersSelectionChanged);
        this.m_Layers.selection.markersChanged += new Selection.MarkerSelectionChangedHandler(this.OnMarkerSelectionChanged);
        this.m_JsonRoomObj = _roomObj;
      }
      catch (Exception ex)
      {
        this.saveRoomToolStripMenuItem.Enabled = false;
        this.exportRoomToolStripMenuItem.Enabled = false;
        throw ex;
      }
      if (this.m_History == null)
      {
        this.m_History = new History();
        this.m_History.historyChanged += new History.HistoryChangedHandler(this.OnHistoryChanged);
      }
      this.m_Viewport = new Viewport(this.pictureBox, this.m_Layers, this);
      this.m_Viewport.InvalidateArea(new Rectangle(0, 0, num1 * 16, num2 * 16));
      this.m_Viewport.UpdatePictureBoxImage();
      this.m_Viewport.history = this.m_History;
      this.m_Viewport.zoomChanged += new Viewport.ZoomChangedHandler(this.OnViewportZoomChanged);
      this.m_Viewport.toolSelectionAreaChanged += new Viewport.ViewportChangedHandler(this.OnToolSelectionAreaChanged);
      if (frmMain.previewForm != null)
        frmMain.previewForm.viewport = this.m_Viewport;
      this.CreateLayersForm();
      this.CreateHistoryForm();
      if (frmMain.markerPropertiesForm != null)
        frmMain.markerPropertiesForm.layers = this.m_Layers;
      this.saveRoomToolStripMenuItem.Enabled = true;
      this.exportRoomToolStripMenuItem.Enabled = true;
      this.undoToolStripMenuItem.Enabled = false;
      this.redoToolStripMenuItem.Enabled = false;
      this.copyToolStripMenuItem.Enabled = false;
      this.pasteToolStripMenuItem.Enabled = false;
      this.resizeToolStripMenuItem.Enabled = true;
      this.editRoomToolStripMenuItem.Enabled = true;
      this.showGridsToolStripMenuItem.Enabled = true;
      this.selectAllToolStripMenuItem.Enabled = true;
      this.cmdResetZoom.Enabled = true;
      this.lblZoomLevel.Enabled = true;
      this.lblCursorPosition.Text = "";
      this.lblTilePosition.Text = "";
      this.pictureBox.MouseMove += new MouseEventHandler(this.OnViewportMouseMove);
      this.m_bChanged = false;
      this.UpdateTitle();
    }

    private void OnToolSelectionAreaChanged()
    {
      if (this.m_Viewport.toolSelectionArea.IsEmpty)
      {
        this.lblToolAreaSelection.Text = "Empty";
      }
      else
      {
        Rectangle _this = Rectangle.Truncate(this.m_Viewport.toolSelectionArea);
        _this.X += (int) ((double) this.m_Viewport.viewedArea.Location.X * (double) this.m_Viewport.zoom);
        _this.Y += (int) ((double) this.m_Viewport.viewedArea.Location.Y * (double) this.m_Viewport.zoom);
        _this = _this.Divided((int) Math.Round(16.0 * (double) this.m_Viewport.zoom));
        this.lblToolAreaSelection.Text = _this.ToString();
      }
    }

    private void OnViewportMouseMove(object _sender, MouseEventArgs _args)
    {
      Point client = ((Control) _sender).PointToClient(Control.MousePosition);
      client.X += (int) ((double) this.m_Viewport.viewedArea.Location.X * (double) this.m_Viewport.zoom);
      client.Y += (int) ((double) this.m_Viewport.viewedArea.Location.Y * (double) this.m_Viewport.zoom);
      this.lblCursorPosition.Text = client.Divided((int) Math.Round(16.0 * (double) this.m_Viewport.zoom)).ToString();
    }

    private void OnViewportZoomChanged(float _oldRatio, float _newRatio)
    {
      this.txtZoom.TextChanged -= new EventHandler(this.TxtZoom_TextChanged);
      this.txtZoom.Text = ((int) ((double) _newRatio * 100.0)).ToString() + " %";
      this.txtZoom.TextChanged += new EventHandler(this.TxtZoom_TextChanged);
    }

    private void OnHistoryChanged(History _history)
    {
      this.undoToolStripMenuItem.Enabled = _history.currentHistoryIndex > 0;
      this.redoToolStripMenuItem.Enabled = _history.currentHistoryIndex < _history.actionCount;
    }

    private void OnSelectedLayerChanged(object _sender, EventArgs _args)
    {
      RadioButton radioButton = _sender as RadioButton;
      this.m_Layers.ResetSelection();
      this.m_Viewport.Invalidate();
      this.m_Viewport.UpdatePictureBoxImage();
      if (!radioButton.Checked)
        return;
      this.m_Layers.currentLayer = radioButton.Tag as Layer;
    }

    private void OnToolChanged(Tool _oldTool, Tool _newTool)
    {
    }

    protected override bool IsInputKey(Keys _keyData) => _keyData == Keys.Up || _keyData == Keys.Down || (_keyData == Keys.Left || _keyData == Keys.Right) || base.IsInputKey(_keyData);

    private void OnKeyboardKeyDown(object _sender, KeyEventArgs _args)
    {
      bool flag = false;
      if (!flag && this.m_Layers != null)
      {
        if (_args.KeyCode == Keys.F1)
          this.m_Layers.currentLayer = (Layer) this.m_Layers.collisionLayer;
        else if (_args.KeyCode == Keys.F2)
          this.m_Layers.currentLayer = (Layer) this.m_Layers.linkLayer;
        else if (_args.KeyCode == Keys.F3)
          this.m_Layers.currentLayer = (Layer) this.m_Layers.markerLayer;
      }
      if (!flag && this.m_Viewport != null)
        flag = this.m_Viewport.OnKeyDown(_args);
      if (!flag && frmMain.toolsForm != null)
        frmMain.toolsForm.OnKeyDown(_args);
      _args.Handled = flag;
    }

    private void OnKeyboardkeyUp(object _sender, KeyEventArgs _args)
    {
      bool flag = false;
      if (this.m_Viewport != null)
        flag = this.m_Viewport.OnKeyUp(_args);
      if (flag || frmMain.toolsForm == null)
        return;
      frmMain.toolsForm.OnKeyUp(_args);
    }

    private void OnToolsFormDisposed(object _sender, EventArgs _args)
    {
      frmMain.toolsForm = (frmTools) null;
      this.toolWindowToolStripMenuItem.Checked = false;
    }

    private void OnControlsFormDisposed(object _sender, EventArgs _args)
    {
      frmMain.controlsForm = (frmControls) null;
      this.controlsToolStripMenuItem.Checked = false;
    }

    private void OnLayersFormDisposed(object _sender, EventArgs _args)
    {
      frmMain.layersForm = (frmLayers) null;
      this.layersToolStripMenuItem.Checked = false;
    }

    private void OnHistoryFormDisposed(object _sender, EventArgs _args)
    {
      frmMain.historyForm = (frmHistory) null;
      this.historyToolStripMenuItem.Checked = false;
    }

    private void OnMarkerPropertiesFormDisposed(object _sender, EventArgs _args) => frmMain.markerPropertiesForm = (frmMarkerProperties) null;

    private void OnPreviewFormDisposed(object _sender, EventArgs _args)
    {
      frmMain.previewForm = (frmViewportPreview) null;
      this.previewWindowToolStripMenuItem.Checked = false;
    }

    private void CreateControlsForm()
    {
      if (frmMain.controlsForm == null)
      {
        frmMain.controlsForm = new frmControls();
        frmMain.controlsForm.Disposed += new EventHandler(this.OnControlsFormDisposed);
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.controlsForm))
      {
        frmMain.controlsForm.Left = this.Left + this.Width / 2 - frmMain.controlsForm.Width / 2;
        frmMain.controlsForm.Top = this.Top + this.Height / 2 - frmMain.controlsForm.Height / 2;
        frmMain.controlsForm.ClampToScreen();
      }
      this.controlsToolStripMenuItem.Checked = true;
      if (frmMain.controlsForm.Visible)
        return;
      frmMain.controlsForm.Show((IWin32Window) this);
    }

    private void CreateToolsForm()
    {
      if (frmMain.toolsForm == null)
      {
        frmMain.toolsForm = new frmTools();
        frmMain.toolsForm.Disposed += new EventHandler(this.OnToolsFormDisposed);
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.toolsForm))
      {
        frmMain.toolsForm.Left = this.Left + this.ClientSize.Width + 5;
        frmMain.toolsForm.Top = this.Top;
        frmMain.toolsForm.ClampToScreen();
      }
      this.toolWindowToolStripMenuItem.Checked = true;
      if (frmMain.toolsForm.Visible)
        return;
      frmMain.toolsForm.Show((IWin32Window) this);
    }

    private void CreateLayersForm()
    {
      if (frmMain.layersForm == null)
      {
        frmMain.layersForm = new frmLayers();
        frmMain.layersForm.Disposed += new EventHandler(this.OnLayersFormDisposed);
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.layersForm))
      {
        frmMain.layersForm.Left = this.Left;
        frmMain.layersForm.Top = this.Bottom;
        frmMain.layersForm.ClampToScreen();
      }
      frmMain.layersForm.viewport = this.m_Viewport;
      frmMain.layersForm.layers = this.m_Layers;
      this.layersToolStripMenuItem.Checked = true;
      if (frmMain.layersForm.Visible)
        return;
      frmMain.layersForm.Show((IWin32Window) this);
    }

    private void CreateHistoryForm()
    {
      if (frmMain.historyForm == null)
      {
        frmMain.historyForm = new frmHistory();
        frmMain.historyForm.Disposed += new EventHandler(this.OnHistoryFormDisposed);
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.historyForm))
      {
        frmMain.historyForm.Left = this.Left - frmMain.historyForm.ClientSize.Width - 5;
        frmMain.historyForm.Top = this.Bottom - frmMain.historyForm.Height;
        frmMain.historyForm.ClampToScreen();
      }
      frmMain.historyForm.viewport = this.m_Viewport;
      this.historyToolStripMenuItem.Checked = true;
      if (frmMain.historyForm.Visible)
        return;
      frmMain.historyForm.Show((IWin32Window) this);
    }

    private void CreateMarkerPropertiesForm()
    {
      if (frmMain.markerPropertiesForm == null)
      {
        frmMain.markerPropertiesForm = new frmMarkerProperties();
        frmMain.markerPropertiesForm.Disposed += new EventHandler(this.OnMarkerPropertiesFormDisposed);
        frmMain.markerPropertiesForm.OnCDBChanged(this.m_CDBObj);
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.markerPropertiesForm))
      {
        frmMain.markerPropertiesForm.Left = this.Left - frmMain.markerPropertiesForm.ClientSize.Width - 5;
        frmMain.markerPropertiesForm.Top = this.Top;
        frmMain.markerPropertiesForm.ClampToScreen();
      }
      frmMain.markerPropertiesForm.layers = this.m_Layers;
      frmMain.markerPropertiesForm.history = this.m_History;
      this.markerPropertiesToolStripMenuItem.Checked = true;
      if (frmMain.markerPropertiesForm.Visible)
        return;
      frmMain.markerPropertiesForm.Show((IWin32Window) this);
    }

    private void OnLayersDataChanged()
    {
      this.m_bChanged = true;
      this.UpdateTitle();
    }

    private void OnLayersSelectionChanged() => this.copyToolStripMenuItem.Enabled = this.m_Layers.hasSelection;

    private void CreatePreviewForm()
    {
      if (frmMain.previewForm == null)
      {
        frmMain.previewForm = new frmViewportPreview();
        frmMain.previewForm.Disposed += new EventHandler(this.OnPreviewFormDisposed);
        this.previewWindowToolStripMenuItem.Checked = true;
        frmMain.previewForm.viewport = this.m_Viewport;
      }
      if (!Options.instance.LoadWindowProps((ToolForm) frmMain.previewForm))
      {
        frmMain.previewForm.Left = this.Right - frmMain.previewForm.Width;
        frmMain.previewForm.Top = this.Bottom;
        frmMain.previewForm.ClampToScreen();
      }
      if (frmMain.previewForm.Visible)
        return;
      frmMain.previewForm.Show((IWin32Window) this);
    }

    private void exitToolStripMenuItem_Click(object _sender, EventArgs _args) => this.Close();

    private void controlsToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.controlsToolStripMenuItem.Checked)
        this.CreateControlsForm();
      frmMain.controlsForm.Visible = this.controlsToolStripMenuItem.Checked;
      this.Focus();
    }

    private void OptionsToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      frmOptions frmOptions = new frmOptions();
      int num = (int) frmOptions.ShowDialog((IWin32Window) this);
      frmOptions.Dispose();
    }

    private void toolWindowToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.toolWindowToolStripMenuItem.Checked)
        this.CreateToolsForm();
      frmMain.toolsForm.Visible = this.toolWindowToolStripMenuItem.Checked;
      this.Focus();
    }

    private void layersToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.layersToolStripMenuItem.Checked)
        this.CreateLayersForm();
      frmMain.layersForm.Visible = this.layersToolStripMenuItem.Checked;
      this.Focus();
    }

    private void historyToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.historyToolStripMenuItem.Checked)
        this.CreateHistoryForm();
      frmMain.historyForm.Visible = this.historyToolStripMenuItem.Checked;
      this.Focus();
    }

    private void previewWindowToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.previewWindowToolStripMenuItem.Checked)
        this.CreatePreviewForm();
      frmMain.previewForm.Visible = this.previewWindowToolStripMenuItem.Checked;
      this.Focus();
    }

    private void markerPropertiesToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.markerPropertiesToolStripMenuItem.Checked)
        this.CreateMarkerPropertiesForm();
      frmMain.markerPropertiesForm.Visible = this.markerPropertiesToolStripMenuItem.Checked;
      this.Focus();
    }

    private void openToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.m_CDBObj == null)
      {
        FileDialog fileDialog = (FileDialog) new OpenFileDialog();
        fileDialog.Filter = "*.json|*.json";
        if (fileDialog.ShowDialog() != DialogResult.OK)
          return;
        if (!this.CloseRoom())
          return;
        try
        {
          this.OpenRoom((JObject) JsonConvert.DeserializeObject(File.ReadAllText(fileDialog.FileName)));
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show("Cannot load the room file " + fileDialog.FileName + ".\nAdditional info : " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
          return;
        }
        this.m_FilePath = fileDialog.FileName;
      }
      else
      {
        frmRoomList frmRoomList = new frmRoomList(frmRoomList.Mode.Open);
        frmRoomList.SetRooms(this.m_Rooms, this.m_RoomSheet);
        if (frmRoomList.IsRoomsDirty)
        {
            SaveFile();
        }
        if (DialogResult.OK != frmRoomList.ShowDialog((IWin32Window) this) || !this.CloseRoom())
          return;
        this.OpenRoom(frmRoomList.selectedRoom);
      }
    }

    private void duplicateRoomToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      frmRoomList frmRoomList = new frmRoomList(frmRoomList.Mode.Duplicate);
      frmRoomList.SetRooms(this.m_Rooms, this.m_RoomSheet);
      if (DialogResult.OK != frmRoomList.ShowDialog((IWin32Window) this) || !this.CloseRoom())
        return;
      JObject selectedRoom = frmRoomList.selectedRoom;
      string str1 = frmRoomList.selectedRoom["id"].ToString();
      int num1 = 1;
      while (frmRoomList.roomNames.IndexOf(str1 + num1.ToString()) != -1)
        ++num1;
      string str2 = str1 + num1.ToString();
      string str3;
      bool flag;
      do
      {
        str3 = Interaction.InputBox("Enter the name for the new room", "Duplicate room", str1 + num1.ToString());
        flag = frmRoomList.roomNames.IndexOf(str3) != -1;
        if (flag)
        {
          int num2 = (int) MessageBox.Show("A room with the name " + str3 + " already exists. Please use a unique name.", "The name already exists", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
      }
      while (flag);
      JObject _roomObj = (JObject) selectedRoom.DeepClone();
      _roomObj["id"] = (JToken) str3;
      Separators separators = new Separators(this.m_RoomSheet);
      SeparatorInfo separatorInfo = separators.GetSeparatorInfo(int.Parse(_roomObj["group"].ToString()));
      separators.InsertLineInGroup(separatorInfo.name,str3);
      ((JArray) this.m_RoomSheet["lines"]).Insert(separatorInfo.lineIndex, (JToken) _roomObj);
      this.OpenRoom(_roomObj);
    }

    private void newRoomToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      frmNewRoom frmNewRoom = new frmNewRoom(this.m_RoomSheet, this.m_RoomTypeSheet);
      if (DialogResult.OK != frmNewRoom.ShowDialog() || !this.CloseRoom())
        return;
      this.OpenRoom(frmNewRoom.roomObject);
      this.m_bChanged = true;
      this.UpdateTitle();
    }

    private void saveRoomFileToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
        SaveFile();
    }

    private void SaveFile()
    {
        this.UpdateRoomData();
        if (this.m_CDBObj != null) {
            StreamWriter text = new FileInfo(this.m_FilePath).CreateText();
            text.Write(CDBTool.CDBTool.GetCDBJObjectAsString(this.m_CDBObj));
            text.Close();
        }
        this.m_bChanged = false;
        this.UpdateTitle();
    }
    private void exportRoomToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
      saveFileDialog.Filter = "*.json|*.json";
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;
      this.UpdateRoomData();
      this.SaveRoomFile(saveFileDialog.FileName, true);
    }

    private void SaveRoomFile(string _fileName, bool _bExport = false)
    {
      StreamWriter text = new FileInfo(_fileName).CreateText();
      text.Write(this.m_JsonRoomObj.ToString());
      text.Close();
      if (_bExport)
        return;
      this.m_FilePath = _fileName;
      this.m_bChanged = false;
    }

    private bool CloseCDB()
    {
      if (!this.CloseRoom())
        return false;
      this.m_RoomsObj.Clear();
      this.m_CDBObj = (JObject) null;
      this.m_FilePath = (string) null;
      MarkerType.Clear();
      if (frmMain.historyForm != null)
      {
        frmMain.historyForm.Clear();
        frmMain.historyForm.viewport = (Viewport) null;
      }
      return true;
    }

    private bool CloseRoom()
    {
      if (this.m_bChanged)
      {
        switch (MessageBox.Show("There are unsaved changes to the current room, would you like to save them ?", "Wait a sec!", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
        {
          case DialogResult.Cancel:
            return false;
          case DialogResult.Yes:
            this.saveRoomFileToolStripMenuItem_Click((object) null, (EventArgs) null);
            if (this.m_bChanged)
              return false;
            break;
        }
      }
      this.m_bChanged = false;
      if (this.m_Layers != null)
      {
        this.m_Layers.ResetSelection();
        frmMain.layersForm.Close();
      }
      this.m_Layers = (Layers) null;
      if (this.m_Viewport != null)
        this.m_Viewport.Dispose();
      this.m_Viewport = (Viewport) null;
      if (this.m_History != null)
        this.m_History.Clear();
      this.m_JsonRoomObj = (JObject) null;
      if (this.m_CDBObj == null)
        this.m_FilePath = "";
      this.saveRoomToolStripMenuItem.Enabled = false;
      this.exportRoomToolStripMenuItem.Enabled = false;
      this.undoToolStripMenuItem.Enabled = false;
      this.redoToolStripMenuItem.Enabled = false;
      this.copyToolStripMenuItem.Enabled = false;
      this.pasteToolStripMenuItem.Enabled = false;
      this.selectAllToolStripMenuItem.Enabled = false;
      this.resizeToolStripMenuItem.Enabled = false;
      this.editRoomToolStripMenuItem.Enabled = false;
      this.showGridsToolStripMenuItem.Enabled = false;
      this.txtZoom.Enabled = false;
      this.cmdResetZoom.Enabled = false;
      this.lblZoomLevel.Enabled = false;
      return true;
    }

    private void UpdateRoomData()
    {
      if (m_JsonRoomObj == null)
          return;
      int num = 0;
      this.m_JsonRoomObj["width"] = (JToken) this.m_Layers.width;
      this.m_JsonRoomObj["height"] = (JToken) this.m_Layers.height;
      foreach (Layer layer in this.m_Layers.layers)
      {
        if (layer.type == Layer.Type.Collision || layer.type == Layer.Type.Link)
          this.m_JsonRoomObj["layers"][(object) num][(object) "data"][(object) "data"] = (JToken) layer.getData();
        else if (layer.type == Layer.Type.Marker)
          this.m_JsonRoomObj["markers"] = (JToken) JsonConvert.DeserializeObject<JArray>(layer.getData());
        ++num;
      }
    }

    private void ResetPosition()
    {
      Rectangle bounds1 = Screen.PrimaryScreen.Bounds;
      int top = bounds1.Top;
      bounds1 = Screen.PrimaryScreen.Bounds;
      int num1 = (bounds1.Height - 850) / 2;
      this.Top = top + num1;
      Rectangle bounds2 = Screen.PrimaryScreen.Bounds;
      int left = bounds2.Left;
      bounds2 = Screen.PrimaryScreen.Bounds;
      int num2 = (bounds2.Width - 850) / 2;
      this.Left = left + num2;
      this.ClampToScreen();
    }

    private void OnMarkerSelectionChanged()
    {
      if (!this.m_Layers.selection.hasMarkers)
        return;
      this.CreateMarkerPropertiesForm();
    }

    private void undoToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (!this.m_History.canUndo)
        return;
      this.m_Viewport.Undo();
      this.m_History.Undo();
      if (frmMain.markerPropertiesForm != null)
        frmMain.markerPropertiesForm.Undo();
      this.m_Viewport.Invalidate();
      this.m_Viewport.UpdatePictureBoxImage();
    }

    private void redoToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (!this.m_History.canRedo)
        return;
      this.m_Viewport.Redo();
      this.m_History.Redo();
      if (frmMain.markerPropertiesForm != null)
        frmMain.markerPropertiesForm.Redo();
      this.m_Viewport.Invalidate();
      this.m_Viewport.UpdatePictureBoxImage();
    }

    private void resizeToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      frmResize frmResize = new frmResize();
      frmResize.width = this.m_Layers.width;
      frmResize.height = this.m_Layers.height;
      if (frmResize.ShowDialog() == DialogResult.OK)
        this.m_Viewport.Resize(frmResize.width, frmResize.height);
      frmResize.Dispose();
    }

    private void editRoomToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      frmNewRoom frmNewRoom = new frmNewRoom(this.m_RoomSheet, this.m_RoomTypeSheet, this.m_JsonRoomObj);
      int width = this.m_Layers.width;
      int height = this.m_Layers.height;
      if (frmNewRoom.ShowDialog() == DialogResult.OK && (width != frmNewRoom.width || height != frmNewRoom.height))
        this.m_Viewport.Resize(frmNewRoom.width, frmNewRoom.height);
      this.m_bChanged = frmNewRoom.changed;
      frmNewRoom.Dispose();
      this.UpdateTitle();
    }

    private void openCDBToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      FileDialog fileDialog = (FileDialog) new OpenFileDialog();
      fileDialog.Filter = "*.cdb|*.cdb";
      if (fileDialog.ShowDialog() == DialogResult.OK)
      {
        if (!this.CloseCDB())
          return;
        try
        {
          this.m_CDBObj = (JObject) JsonConvert.DeserializeObject(File.ReadAllText(fileDialog.FileName));
          foreach (JObject jobject1 in (IEnumerable<JToken>) this.m_CDBObj["sheets"])
          {
            string str = jobject1["name"].ToString();
            if (str == "room" || str == "Room")
            {
              this.m_RoomSheet = jobject1;
              this.m_Rooms = (JArray) jobject1["lines"];
              foreach (JObject room in this.m_Rooms)
                this.m_RoomsObj.Add(room["id"].ToString(), room);
            }
            else if (str == "roomMarker" || str == "RoomMarker")
            {
              foreach (JObject jobject2 in (JArray) jobject1["lines"])
                MarkerType.CreateFromString(jobject2.ToString());
            }
            else if (str == "roomType" || str == "RoomType")
              this.m_RoomTypeSheet = jobject1;
          }
          this.m_FilePath = fileDialog.FileName;
        }
        catch (Exception ex)
        {
          throw new Exception("Cannot open CDB " + fileDialog.FileName + " because : " + ex.Message);
        }
        if (frmMain.toolsForm != null)
          frmMain.toolsForm.OnCDBChanged();
        if (frmMain.markerPropertiesForm != null)
          frmMain.markerPropertiesForm.OnCDBChanged(this.m_CDBObj);
        this.UpdateTitle();
        this.saveRoomToolStripMenuItem.Enabled = false;
        this.exportRoomToolStripMenuItem.Enabled = false;
        this.newRoomToolStripMenuItem.Enabled = true;
        this.openToolStripMenuItem.Enabled = true;
        this.duplicateRoomToolStripMenuItem.Enabled = true;
        this.openToolStripMenuItem_Click((object) null, (EventArgs) null);
      }
      fileDialog.Dispose();
    }

    private void UpdateTitle()
    {
      if (this.m_FilePath != null && this.m_FilePath != "")
        this.Text = new FileInfo(this.m_FilePath).Name;
      if (this.m_CDBObj != null && this.m_JsonRoomObj != null)
        this.Text = this.Text + " : " + (object) this.m_JsonRoomObj["id"];
      if (this.m_bChanged)
        this.Text += "*";
      if (this.Text != "")
        this.Text += " - ";
      this.Text = this.Text + "RoomEditor alpha v" + Versionning.currentVersion;
    }

    private void resetPositionsToolStripMenuItem_Click(object _sender, EventArgs _args) => this.ResetToolFormsPositions();

    private void resetMainWindowPositionToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      Options.instance.ResetWindowProps((ToolForm) this);
      this.ResetPosition();
    }

    private void copyToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.m_Layers.selection.hasMarkers)
      {
        this.clipboard.SetData(this.m_Layers.selection.markers);
      }
      else
      {
        List<TileInfo> tileInfoList = new List<TileInfo>();
        tileInfoList.AddRange((IEnumerable<TileInfo>) this.m_Layers.selection.tiles);
        this.clipboard.SetData((IEnumerable<TileInfo>) tileInfoList);
      }
      this.pasteToolStripMenuItem.Enabled = !this.clipboard.isEmpty;
    }

    private void pasteToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (!this.clipboard.isEmpty)
      {
        this.viewport.Paste(this.clipboard);
        Tools.currentTool = Tools.mover;
        if (frmMain.toolsForm == null)
          return;
        frmMain.toolsForm.UpdateCurrentTool();
      }
      else
        this.pasteToolStripMenuItem.Enabled = false;
    }

    private void showGridsToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      if (this.showGridsToolStripMenuItem.Checked == Options.instance.displayShowGrids)
        return;
      Options.instance.displayShowGrids = !Options.instance.displayShowGrids;
      this.m_Viewport.UpdatePictureBoxImage();
      Options.instance.Save();
    }

    private void selectAllToolStripMenuItem_Click(object _sender, EventArgs _args)
    {
      this.m_Layers.ResetSelection();
      Rectangle empty = Rectangle.Empty;
      this.m_Layers.SelectArea(new Rectangle(0, 0, this.m_Layers.width * 16, this.m_Layers.height * 16), ref empty);
      this.m_Viewport.InvalidateArea(empty);
      this.m_Viewport.UpdatePictureBoxImage();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.cmdResetZoom = new Button();
      this.txtZoom = new TextBox();
      this.lblZoomLevel = new Label();
      this.menuStrip = new MenuStrip();
      this.fileToolStripMenuItem = new ToolStripMenuItem();
      this.openCDBToolStripMenuItem = new ToolStripMenuItem();
      this.openToolStripMenuItem = new ToolStripMenuItem();
      this.newRoomToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem2 = new ToolStripSeparator();
      this.saveRoomToolStripMenuItem = new ToolStripMenuItem();
      this.exportRoomToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem1 = new ToolStripSeparator();
      this.exitToolStripMenuItem = new ToolStripMenuItem();
      this.editToolStripMenuItem = new ToolStripMenuItem();
      this.undoToolStripMenuItem = new ToolStripMenuItem();
      this.redoToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem5 = new ToolStripSeparator();
      this.copyToolStripMenuItem = new ToolStripMenuItem();
      this.pasteToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem6 = new ToolStripSeparator();
      this.selectAllToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem4 = new ToolStripSeparator();
      this.OptionsToolStripMenuItem = new ToolStripMenuItem();
      this.roomToolStripMenuItem = new ToolStripMenuItem();
      this.resizeToolStripMenuItem = new ToolStripMenuItem();
      this.editRoomToolStripMenuItem = new ToolStripMenuItem();
      this.displayToolStripMenuItem = new ToolStripMenuItem();
      this.showGridsToolStripMenuItem = new ToolStripMenuItem();
      this.toolsToolStripMenuItem = new ToolStripMenuItem();
      this.toolWindowToolStripMenuItem = new ToolStripMenuItem();
      this.previewWindowToolStripMenuItem = new ToolStripMenuItem();
      this.layersToolStripMenuItem = new ToolStripMenuItem();
      this.historyToolStripMenuItem = new ToolStripMenuItem();
      this.markerPropertiesToolStripMenuItem = new ToolStripMenuItem();
      this.controlsToolStripMenuItem = new ToolStripMenuItem();
      this.toolStripMenuItem3 = new ToolStripSeparator();
      this.resetPositionsToolStripMenuItem = new ToolStripMenuItem();
      this.resetMainWindowPositionToolStripMenuItem = new ToolStripMenuItem();
      this.pictureBox = new InterpolablePictureBox();
      this.statusStrip = new StatusStrip();
      this.lblCursorPosition = new ToolStripStatusLabel();
      this.lblTilePosition = new ToolStripStatusLabel();
      this.lblToolAreaSelection = new ToolStripStatusLabel();
      this.toolStripMenuItem7 = new ToolStripSeparator();
      this.duplicateRoomToolStripMenuItem = new ToolStripMenuItem();
      this.menuStrip.SuspendLayout();
      ((ISupportInitialize) this.pictureBox).BeginInit();
      this.statusStrip.SuspendLayout();
      this.SuspendLayout();
      this.cmdResetZoom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.cmdResetZoom.Location = new Point(748, 2);
      this.cmdResetZoom.Name = "cmdResetZoom";
      this.cmdResetZoom.Size = new Size(21, 23);
      this.cmdResetZoom.TabIndex = 10;
      this.cmdResetZoom.TabStop = false;
      this.cmdResetZoom.Text = "C";
      this.cmdResetZoom.UseVisualStyleBackColor = true;
      this.cmdResetZoom.Click += new EventHandler(this.cmdResetZoom_Click);
      this.txtZoom.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.txtZoom.Enabled = false;
      this.txtZoom.Location = new Point(696, 3);
      this.txtZoom.Name = "txtZoom";
      this.txtZoom.ReadOnly = true;
      this.txtZoom.Size = new Size(46, 20);
      this.txtZoom.TabIndex = 0;
      this.txtZoom.TabStop = false;
      this.txtZoom.Text = "100 %";
      this.txtZoom.TextAlign = HorizontalAlignment.Right;
      this.lblZoomLevel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
      this.lblZoomLevel.AutoSize = true;
      this.lblZoomLevel.BackColor = Color.Transparent;
      this.lblZoomLevel.Location = new Point(630, 7);
      this.lblZoomLevel.Name = "lblZoomLevel";
      this.lblZoomLevel.Size = new Size(68, 13);
      this.lblZoomLevel.TabIndex = 8;
      this.lblZoomLevel.Text = "Zoom level : ";
      this.menuStrip.Items.AddRange(new ToolStripItem[5]
      {
        (ToolStripItem) this.fileToolStripMenuItem,
        (ToolStripItem) this.editToolStripMenuItem,
        (ToolStripItem) this.roomToolStripMenuItem,
        (ToolStripItem) this.displayToolStripMenuItem,
        (ToolStripItem) this.toolsToolStripMenuItem
      });
      this.menuStrip.Location = new Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new Size(784, 24);
      this.menuStrip.TabIndex = 7;
      this.menuStrip.Text = "menuStrip1";
      this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[10]
      {
        (ToolStripItem) this.openCDBToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem7,
        (ToolStripItem) this.openToolStripMenuItem,
        (ToolStripItem) this.duplicateRoomToolStripMenuItem,
        (ToolStripItem) this.newRoomToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem2,
        (ToolStripItem) this.saveRoomToolStripMenuItem,
        (ToolStripItem) this.exportRoomToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem1,
        (ToolStripItem) this.exitToolStripMenuItem
      });
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      this.openCDBToolStripMenuItem.Name = "openCDBToolStripMenuItem";
      this.openCDBToolStripMenuItem.Size = new Size(178, 22);
      this.openCDBToolStripMenuItem.Text = "Open CDB";
      this.openCDBToolStripMenuItem.Click += new EventHandler(this.openCDBToolStripMenuItem_Click);
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.ShortcutKeys = Keys.O | Keys.Control;
      this.openToolStripMenuItem.Size = new Size(178, 22);
      this.openToolStripMenuItem.Text = "Open room";
      this.openToolStripMenuItem.Click += new EventHandler(this.openToolStripMenuItem_Click);
      this.newRoomToolStripMenuItem.Name = "newRoomToolStripMenuItem";
      this.newRoomToolStripMenuItem.Size = new Size(178, 22);
      this.newRoomToolStripMenuItem.Text = "New room";
      this.newRoomToolStripMenuItem.Click += new EventHandler(this.newRoomToolStripMenuItem_Click);
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new Size(175, 6);
      this.saveRoomToolStripMenuItem.Name = "saveRoomToolStripMenuItem";
      this.saveRoomToolStripMenuItem.ShortcutKeys = Keys.S | Keys.Control;
      this.saveRoomToolStripMenuItem.Size = new Size(178, 22);
      this.saveRoomToolStripMenuItem.Text = "Save room";
      this.saveRoomToolStripMenuItem.Click += new EventHandler(this.saveRoomFileToolStripMenuItem_Click);
      this.exportRoomToolStripMenuItem.Name = "exportRoomToolStripMenuItem";
      this.exportRoomToolStripMenuItem.Size = new Size(178, 22);
      this.exportRoomToolStripMenuItem.Text = "Export room";
      this.exportRoomToolStripMenuItem.Click += new EventHandler(this.exportRoomToolStripMenuItem_Click);
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new Size(175, 6);
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = Keys.F4 | Keys.Alt;
      this.exitToolStripMenuItem.Size = new Size(178, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
      this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.undoToolStripMenuItem,
        (ToolStripItem) this.redoToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem5,
        (ToolStripItem) this.copyToolStripMenuItem,
        (ToolStripItem) this.pasteToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem6,
        (ToolStripItem) this.selectAllToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem4,
        (ToolStripItem) this.OptionsToolStripMenuItem
      });
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new Size(39, 20);
      this.editToolStripMenuItem.Text = "Edit";
      this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
      this.undoToolStripMenuItem.ShortcutKeys = Keys.Z | Keys.Control;
      this.undoToolStripMenuItem.Size = new Size(164, 22);
      this.undoToolStripMenuItem.Text = "Undo";
      this.undoToolStripMenuItem.Click += new EventHandler(this.undoToolStripMenuItem_Click);
      this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
      this.redoToolStripMenuItem.ShortcutKeys = Keys.Y | Keys.Control;
      this.redoToolStripMenuItem.Size = new Size(164, 22);
      this.redoToolStripMenuItem.Text = "Redo";
      this.redoToolStripMenuItem.Click += new EventHandler(this.redoToolStripMenuItem_Click);
      this.toolStripMenuItem5.Name = "toolStripMenuItem5";
      this.toolStripMenuItem5.Size = new Size(161, 6);
      this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      this.copyToolStripMenuItem.ShortcutKeys = Keys.C | Keys.Control;
      this.copyToolStripMenuItem.Size = new Size(164, 22);
      this.copyToolStripMenuItem.Text = "Copy";
      this.copyToolStripMenuItem.Click += new EventHandler(this.copyToolStripMenuItem_Click);
      this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
      this.pasteToolStripMenuItem.ShortcutKeys = Keys.V | Keys.Control;
      this.pasteToolStripMenuItem.Size = new Size(164, 22);
      this.pasteToolStripMenuItem.Text = "Paste";
      this.pasteToolStripMenuItem.Click += new EventHandler(this.pasteToolStripMenuItem_Click);
      this.toolStripMenuItem6.Name = "toolStripMenuItem6";
      this.toolStripMenuItem6.Size = new Size(161, 6);
      this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
      this.selectAllToolStripMenuItem.ShortcutKeys = Keys.A | Keys.Control;
      this.selectAllToolStripMenuItem.Size = new Size(164, 22);
      this.selectAllToolStripMenuItem.Text = "Select All";
      this.selectAllToolStripMenuItem.Click += new EventHandler(this.selectAllToolStripMenuItem_Click);
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new Size(161, 6);
      this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
      this.OptionsToolStripMenuItem.Size = new Size(164, 22);
      this.OptionsToolStripMenuItem.Text = "Options...";
      this.OptionsToolStripMenuItem.Click += new EventHandler(this.OptionsToolStripMenuItem_Click);
      this.roomToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[2]
      {
        (ToolStripItem) this.resizeToolStripMenuItem,
        (ToolStripItem) this.editRoomToolStripMenuItem
      });
      this.roomToolStripMenuItem.Name = "roomToolStripMenuItem";
      this.roomToolStripMenuItem.Size = new Size(51, 20);
      this.roomToolStripMenuItem.Text = "Room";
      this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
      this.resizeToolStripMenuItem.ShortcutKeys = Keys.R | Keys.Control;
      this.resizeToolStripMenuItem.Size = new Size(147, 22);
      this.resizeToolStripMenuItem.Text = "Resize";
      this.resizeToolStripMenuItem.Click += new EventHandler(this.resizeToolStripMenuItem_Click);
      this.editRoomToolStripMenuItem.Name = "editRoomToolStripMenuItem";
      this.editRoomToolStripMenuItem.Size = new Size(147, 22);
      this.editRoomToolStripMenuItem.Text = "Edit";
      this.editRoomToolStripMenuItem.Click += new EventHandler(this.editRoomToolStripMenuItem_Click);
      this.displayToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[1]
      {
        (ToolStripItem) this.showGridsToolStripMenuItem
      });
      this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
      this.displayToolStripMenuItem.Size = new Size(57, 20);
      this.displayToolStripMenuItem.Text = "Display";
      this.showGridsToolStripMenuItem.CheckOnClick = true;
      this.showGridsToolStripMenuItem.Name = "showGridsToolStripMenuItem";
      this.showGridsToolStripMenuItem.ShortcutKeys = Keys.G | Keys.Control;
      this.showGridsToolStripMenuItem.Size = new Size(174, 22);
      this.showGridsToolStripMenuItem.Text = "Show grids";
      this.showGridsToolStripMenuItem.Click += new EventHandler(this.showGridsToolStripMenuItem_Click);
      this.toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[9]
      {
        (ToolStripItem) this.toolWindowToolStripMenuItem,
        (ToolStripItem) this.previewWindowToolStripMenuItem,
        (ToolStripItem) this.layersToolStripMenuItem,
        (ToolStripItem) this.historyToolStripMenuItem,
        (ToolStripItem) this.markerPropertiesToolStripMenuItem,
        (ToolStripItem) this.controlsToolStripMenuItem,
        (ToolStripItem) this.toolStripMenuItem3,
        (ToolStripItem) this.resetPositionsToolStripMenuItem,
        (ToolStripItem) this.resetMainWindowPositionToolStripMenuItem
      });
      this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
      this.toolsToolStripMenuItem.Size = new Size(63, 20);
      this.toolsToolStripMenuItem.Text = "Window";
      this.toolWindowToolStripMenuItem.CheckOnClick = true;
      this.toolWindowToolStripMenuItem.Name = "toolWindowToolStripMenuItem";
      this.toolWindowToolStripMenuItem.ShortcutKeys = Keys.F5;
      this.toolWindowToolStripMenuItem.Size = new Size(231, 22);
      this.toolWindowToolStripMenuItem.Text = "Tools";
      this.toolWindowToolStripMenuItem.Click += new EventHandler(this.toolWindowToolStripMenuItem_Click);
      this.previewWindowToolStripMenuItem.CheckOnClick = true;
      this.previewWindowToolStripMenuItem.Name = "previewWindowToolStripMenuItem";
      this.previewWindowToolStripMenuItem.ShortcutKeys = Keys.F6;
      this.previewWindowToolStripMenuItem.Size = new Size(231, 22);
      this.previewWindowToolStripMenuItem.Text = "Preview";
      this.previewWindowToolStripMenuItem.Click += new EventHandler(this.previewWindowToolStripMenuItem_Click);
      this.layersToolStripMenuItem.CheckOnClick = true;
      this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
      this.layersToolStripMenuItem.ShortcutKeys = Keys.F7;
      this.layersToolStripMenuItem.Size = new Size(231, 22);
      this.layersToolStripMenuItem.Text = "Layers";
      this.layersToolStripMenuItem.Click += new EventHandler(this.layersToolStripMenuItem_Click);
      this.historyToolStripMenuItem.CheckOnClick = true;
      this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
      this.historyToolStripMenuItem.ShortcutKeys = Keys.F8;
      this.historyToolStripMenuItem.Size = new Size(231, 22);
      this.historyToolStripMenuItem.Text = "History";
      this.historyToolStripMenuItem.Click += new EventHandler(this.historyToolStripMenuItem_Click);
      this.markerPropertiesToolStripMenuItem.CheckOnClick = true;
      this.markerPropertiesToolStripMenuItem.Name = "markerPropertiesToolStripMenuItem";
      this.markerPropertiesToolStripMenuItem.ShortcutKeys = Keys.F9;
      this.markerPropertiesToolStripMenuItem.Size = new Size(231, 22);
      this.markerPropertiesToolStripMenuItem.Text = "Marker Properties";
      this.markerPropertiesToolStripMenuItem.Click += new EventHandler(this.markerPropertiesToolStripMenuItem_Click);
      this.controlsToolStripMenuItem.CheckOnClick = true;
      this.controlsToolStripMenuItem.Name = "controlsToolStripMenuItem";
      this.controlsToolStripMenuItem.Size = new Size(231, 22);
      this.controlsToolStripMenuItem.Text = "Controls";
      this.controlsToolStripMenuItem.Click += new EventHandler(this.controlsToolStripMenuItem_Click);
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new Size(228, 6);
      this.resetPositionsToolStripMenuItem.Name = "resetPositionsToolStripMenuItem";
      this.resetPositionsToolStripMenuItem.Size = new Size(231, 22);
      this.resetPositionsToolStripMenuItem.Text = "Reset Tool Windows Positions";
      this.resetPositionsToolStripMenuItem.Click += new EventHandler(this.resetPositionsToolStripMenuItem_Click);
      this.resetMainWindowPositionToolStripMenuItem.Name = "resetMainWindowPositionToolStripMenuItem";
      this.resetMainWindowPositionToolStripMenuItem.Size = new Size(231, 22);
      this.resetMainWindowPositionToolStripMenuItem.Text = "Reset Main Window Position";
      this.resetMainWindowPositionToolStripMenuItem.Click += new EventHandler(this.resetMainWindowPositionToolStripMenuItem_Click);
      this.pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      this.pictureBox.interpolationMode = InterpolationMode.Default;
      this.pictureBox.Location = new Point(13, 27);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new Size(757, 520);
      this.pictureBox.TabIndex = 4;
      this.pictureBox.TabStop = false;
      this.statusStrip.Items.AddRange(new ToolStripItem[3]
      {
        (ToolStripItem) this.lblCursorPosition,
        (ToolStripItem) this.lblTilePosition,
        (ToolStripItem) this.lblToolAreaSelection
      });
      this.statusStrip.Location = new Point(0, 539);
      this.statusStrip.Name = "statusStrip";
      this.statusStrip.Size = new Size(784, 22);
      this.statusStrip.TabIndex = 11;
      this.statusStrip.Text = "statusStrip1";
      this.lblCursorPosition.Name = "lblCursorPosition";
      this.lblCursorPosition.Size = new Size(21, 17);
      this.lblCursorPosition.Text = "x;y";
      this.lblTilePosition.Name = "lblTilePosition";
      this.lblTilePosition.Size = new Size(21, 17);
      this.lblTilePosition.Text = "x;y";
      this.lblToolAreaSelection.Name = "lblToolAreaSelection";
      this.lblToolAreaSelection.Size = new Size(91, 17);
      this.lblToolAreaSelection.Text = "x;y;width;height";
      this.toolStripMenuItem7.Name = "toolStripMenuItem7";
      this.toolStripMenuItem7.Size = new Size(175, 6);
      this.duplicateRoomToolStripMenuItem.Name = "duplicateRoomToolStripMenuItem";
      this.duplicateRoomToolStripMenuItem.Size = new Size(178, 22);
      this.duplicateRoomToolStripMenuItem.Text = "Duplicate room";
      this.duplicateRoomToolStripMenuItem.Click += new EventHandler(this.duplicateRoomToolStripMenuItem_Click);
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.CausesValidation = false;
      this.ClientSize = new Size(784, 561);
      this.Controls.Add((Control) this.statusStrip);
      this.Controls.Add((Control) this.cmdResetZoom);
      this.Controls.Add((Control) this.txtZoom);
      this.Controls.Add((Control) this.lblZoomLevel);
      this.Controls.Add((Control) this.pictureBox);
      this.Controls.Add((Control) this.menuStrip);
      this.KeyPreview = true;
      this.MainMenuStrip = this.menuStrip;
      this.Name = nameof (frmMain);
      this.StartPosition = FormStartPosition.Manual;
      this.Text = "Main";
      this.Load += new EventHandler(this.frmMain_Load);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      ((ISupportInitialize) this.pictureBox).EndInit();
      this.statusStrip.ResumeLayout(false);
      this.statusStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();
    }
  }
}
