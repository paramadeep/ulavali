using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace Ulavali
{
    public class LivePropertyDisplay
    {
        private readonly ArrayList _automationObjectProperties = new ArrayList();
        private readonly Form _liveDisplayForm = new Form();
        private readonly DataGridView _dataGrid = new DataGridView();
        private Rectangle _dispalyFormRectangle;


        public LivePropertyDisplay()
        {
            _liveDisplayForm.FormBorderStyle = FormBorderStyle.None;
            _liveDisplayForm.ShowInTaskbar = false;
            _liveDisplayForm.TopMost = true;
            _liveDisplayForm.Visible = false;
            _liveDisplayForm.Opacity = 0.85;
            _liveDisplayForm.MouseEnter += LiveDisplayFormOnMouseEnter;
            _liveDisplayForm.MouseMove += LiveDisplayFormOnMouseEnter;

            _dataGrid.AllowUserToDeleteRows = false;
            _dataGrid.AllowUserToResizeColumns = false;
            _dataGrid.AllowUserToResizeRows = false;
            _dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            _dataGrid.ColumnHeadersVisible = false;
            _dataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            _dataGrid.GridColor = Color.PowderBlue;
            _dataGrid.Location = new Point(0, 0);
            _dataGrid.MultiSelect = false;
            _dataGrid.ReadOnly = true;
            _dataGrid.RowHeadersVisible = false;
            _dataGrid.ScrollBars = ScrollBars.None;
            _dataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Make it a tool window so it doesn't show up with Alt+Tab.
            var style = NativeMethods.GetWindowLong(
                _liveDisplayForm.Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(
                _liveDisplayForm.Handle, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_TOOLWINDOW);
            _dataGrid.Dock = DockStyle.Fill;
            _liveDisplayForm.Controls.Add(_dataGrid);
        }

        private void LiveDisplayFormOnMouseEnter(object sender, EventArgs args)
        {
            Hide();
        }

        public void Hide()
        {
            NativeMethods.ShowWindow(_liveDisplayForm.Handle, NativeMethods.SW_HIDE);
        }

        public void LoadAutomationObjectForDisplay(AutomationElement currentAutomationElement)
        {
            _dataGrid.DataSource = UpdateElementPropertyArray(currentAutomationElement);
            Rect objectBoundingRectangle = currentAutomationElement.Current.BoundingRectangle;

            _dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            _liveDisplayForm.AutoSize = false;
            _liveDisplayForm.AutoSizeMode = AutoSizeMode.GrowOnly;
            _liveDisplayForm.AutoSize = true;

            PositionLiveDiplayDialog(objectBoundingRectangle);
        }

        private void PositionLiveDiplayDialog(Rect objectBoundingRectangle)
        {
            int totalColumnWidth = _dataGrid.Columns.Cast<DataGridViewColumn>()
                .Aggregate(0, (current, column) => current + column.Width);
            int totalRowHeight = _dataGrid.Rows.Cast<DataGridViewRow>()
                .Aggregate(0, (current, row) => current + row.Height);
            _dispalyFormRectangle = new Rectangle((int) (objectBoundingRectangle.X + objectBoundingRectangle.Width),
                (int) (objectBoundingRectangle.Y + objectBoundingRectangle.Height),
                totalColumnWidth,
                totalRowHeight);
            BringDisplayRectangleIntoScreen(objectBoundingRectangle);
            NativeMethods.SetWindowPos(_liveDisplayForm.Handle, NativeMethods.HWND_TOPMOST,
                _dispalyFormRectangle.X,
                _dispalyFormRectangle.Y,
                totalColumnWidth,
                totalRowHeight,
                NativeMethods.SWP_NOACTIVATE);
        }

        private void BringDisplayRectangleIntoScreen(Rect testElementRectangle)
        {
            var screenRectangle = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            if (screenRectangle.Contains(_dispalyFormRectangle)) return;
            if (screenRectangle.Width < _dispalyFormRectangle.X + _dispalyFormRectangle.Width)
            {
                _dispalyFormRectangle.X = screenRectangle.Width - _dispalyFormRectangle.Width;
            }

            if (screenRectangle.Height >= _dispalyFormRectangle.Height + _dispalyFormRectangle.Y) return;
            _dispalyFormRectangle.Y = (int) (testElementRectangle.Y - _dispalyFormRectangle.Height);
            if (screenRectangle.Contains(_dispalyFormRectangle)) return;
            _dispalyFormRectangle.Y =
                (int) (testElementRectangle.Y + testElementRectangle.Height - _dispalyFormRectangle.Height);
        }

        private ArrayList UpdateElementPropertyArray(AutomationElement currentAutomationElement)
        {
            _automationObjectProperties.Clear();
            _automationObjectProperties.Add(new ObjectProperty("Name", currentAutomationElement.Current.Name));
            _automationObjectProperties.Add(new ObjectProperty("Automation Id",
                currentAutomationElement.Current.AutomationId));
            _automationObjectProperties.Add(new ObjectProperty("Process Id",
                currentAutomationElement.Current.ProcessId.ToString(CultureInfo.InvariantCulture)));
            _automationObjectProperties.Add(new ObjectProperty("Control Type",
                currentAutomationElement.Current.LocalizedControlType));
            return _automationObjectProperties;
        }

        public void Show()
        {
            NativeMethods.ShowWindow(_liveDisplayForm.Handle, NativeMethods.SW_SHOWNA);
        }
    }
}