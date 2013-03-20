using System;
using System.Collections;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Point=System.Drawing.Point;

namespace White_Spy
{
    public class LivePropertyDisplay
    {
        private ArrayList AutomationObjectProperties = new ArrayList();
        private DataGridView dataGrid = new DataGridView();
        private Form LiveDisplayForm = new Form();
        private Rectangle dispalyFormRectangle;


        public LivePropertyDisplay()
        {
            LiveDisplayForm.FormBorderStyle = FormBorderStyle.None;
            LiveDisplayForm.ShowInTaskbar = false;
            LiveDisplayForm.TopMost = true;
            LiveDisplayForm.Visible = false;
            LiveDisplayForm.Opacity = 0.85;
            LiveDisplayForm.MouseEnter += LiveDisplayFormOnMouseEnter;
            LiveDisplayForm.MouseMove += LiveDisplayFormOnMouseEnter;

            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.AllowUserToResizeColumns = false;
            dataGrid.AllowUserToResizeRows = false;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGrid.ColumnHeadersVisible = false;
            dataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGrid.GridColor = Color.PowderBlue;
            dataGrid.Location = new Point(0, 0);
            dataGrid.MultiSelect = false;
            dataGrid.ReadOnly = true;
            dataGrid.RowHeadersVisible = false;
            dataGrid.ScrollBars = ScrollBars.None;
            dataGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            
            // Make it a tool window so it doesn't show up with Alt+Tab.
            int style = NativeMethods.GetWindowLong(
                LiveDisplayForm.Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(
                LiveDisplayForm.Handle, NativeMethods.GWL_EXSTYLE,
                style | NativeMethods.WS_EX_TOOLWINDOW);
            dataGrid.Dock = DockStyle.Fill;
            LiveDisplayForm.Controls.Add(dataGrid);
        }

        private void LiveDisplayFormOnMouseEnter(object sender, EventArgs args)
        {
            Hide();
        }

        public void Hide()
        {
            NativeMethods.ShowWindow(LiveDisplayForm.Handle, NativeMethods.SW_HIDE);
        }

        public void LoadAutomationObjectForDisplay(AutomationElement currentAutomationElement)
        {
            dataGrid.DataSource = UpdateElementPropertyArray(currentAutomationElement);
            Rect objectBoundingRectangle = currentAutomationElement.Current.BoundingRectangle;

            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            LiveDisplayForm.AutoSize = false;
            LiveDisplayForm.AutoSizeMode = AutoSizeMode.GrowOnly;
            LiveDisplayForm.AutoSize = true;

            PositionLiveDiplayDialog(objectBoundingRectangle);
        }

        private void PositionLiveDiplayDialog(Rect objectBoundingRectangle)
        {
            int totalColumnWidth= 0;
            foreach (DataGridViewColumn column in dataGrid.Columns)
            {
                totalColumnWidth = totalColumnWidth + column.Width;
            }
             
            int totalRowHeight= 0;
            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                totalRowHeight = totalRowHeight + row.Height;
            }

            dispalyFormRectangle = new Rectangle((int) (objectBoundingRectangle.X + objectBoundingRectangle.Width),
                                                 (int) (objectBoundingRectangle.Y + objectBoundingRectangle.Height),
                                                 totalColumnWidth,
                                                 totalRowHeight);
            BringDisplayRectangleIntoScreen(objectBoundingRectangle);

            NativeMethods.SetWindowPos(LiveDisplayForm.Handle, NativeMethods.HWND_TOPMOST,
                                       (int) (dispalyFormRectangle.X),
                                       (int) (dispalyFormRectangle.Y),
                                       totalColumnWidth,
                                       totalRowHeight,
                                       NativeMethods.SWP_NOACTIVATE);
        }

        private void BringDisplayRectangleIntoScreen(Rect testElementRectangle)
        {
            var screenRectangle = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            if (screenRectangle.Contains(dispalyFormRectangle))return;
            if (screenRectangle.Width<dispalyFormRectangle.X+dispalyFormRectangle.Width)
            {
                dispalyFormRectangle.X = screenRectangle.Width - dispalyFormRectangle.Width;
            }

            if (screenRectangle.Height < dispalyFormRectangle.Height + dispalyFormRectangle.Y)
            {
                dispalyFormRectangle.Y = (int)(testElementRectangle.Y - dispalyFormRectangle.Height);
                if (screenRectangle.Contains(dispalyFormRectangle)) return;
                dispalyFormRectangle.Y = (int) (testElementRectangle.Y + testElementRectangle.Height - dispalyFormRectangle.Height);
            }
                
            
        }

        private ArrayList UpdateElementPropertyArray(AutomationElement currentAutomationElement)
        {
            AutomationObjectProperties.Clear();
            AutomationObjectProperties.Add(new ObjectProperty("Name", currentAutomationElement.Current.Name));
            AutomationObjectProperties.Add(new ObjectProperty("Automation Id",
                                                              currentAutomationElement.Current.AutomationId));
            AutomationObjectProperties.Add(new ObjectProperty("Process Id",
                                                              currentAutomationElement.Current.ProcessId.ToString()));
            AutomationObjectProperties.Add(new ObjectProperty("Control Type",
                                                              currentAutomationElement.Current.LocalizedControlType));
//            AutomationObjectProperties.Add(new ObjectProperty("White Type",
//                                                              GetWhiteType(currentAutomationElement)));

            return AutomationObjectProperties;
        }

        public void Show()
        {
            NativeMethods.ShowWindow(LiveDisplayForm.Handle, NativeMethods.SW_SHOWNA);
        }
    }

    public class ObjectProperty
    {
        private String property;
        private String propertyValue;

        public ObjectProperty(String propertyName, String propertyValue)
        {
            property = propertyName;
            this.propertyValue = propertyValue;
        }

        public String Property
        {
            get { return property; }
            set { property = value; }
        }

        public String Value
        {
            get { return propertyValue; }
            set { propertyValue = value; }
        }
    }
}