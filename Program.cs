using System;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Size=System.Drawing.Size;

namespace White_Spy
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            new WhiteSpy().Run();
        }
    }

    internal class WhiteSpy
    {
        private const int TimeInterval = 2;
        private readonly CatchObjectAtMousePoint CatchObjcetAtMouse = new CatchObjectAtMousePoint();

        private readonly ToolStripMenuItem ExitToolStripMenuItem = new ToolStripMenuItem();
        private readonly HighlightRectangle Highlight = new HighlightRectangle();
        private readonly KeyboardHook KeyboardHook = new KeyboardHook();
        private readonly LivePropertyDisplay LivePropertyDisplay = new LivePropertyDisplay();
        private readonly NotifyIcon SystemTrayControl = new NotifyIcon();
        private readonly ContextMenuStrip SystemTrayMenu = new ContextMenuStrip();
        private readonly Timer Timer = new Timer();
        private static AutomationElement _currentAutomationElement = AutomationElement.RootElement;
        private static Rect _focusedRect;
        private static AutomationElement _previousFocusedAutomationElement;
        private static SpyState spyState;
        
        private enum SpyState
        {
            Spying, ShowProperties, Idel
        } ;

        public void Run()
        {
            Application.EnableVisualStyles();
            InitilizeSystemTrayControl();
            StartListeningToMouseMoves();
            Application.Run();
        }

        private void InitilizeSystemTrayControl()
        {
            SystemTrayControl.Visible = true;
            SystemTrayControl.Icon = new Icon("image\\User.ico");

            // 
            // SystemTrayControl
            // 
            SystemTrayControl.BalloonTipText = "Start Spying";
            SystemTrayControl.ContextMenuStrip = SystemTrayMenu;
            SystemTrayControl.Text = "Start Spying";
            SystemTrayControl.Visible = true;
            SystemTrayControl.Click += OnClickOnSystemTrayControl;
            // 
            // SystemTrayMenu
            // 
            SystemTrayMenu.Items.AddRange(new ToolStripItem[]
                                              {
                                                  ExitToolStripMenuItem
                                              });
            SystemTrayMenu.Name = "SystemTrayMenu";
            SystemTrayMenu.Size = new Size(104, 48);
            // 
            // ExitToolStripMenuItem
            // 
            ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            ExitToolStripMenuItem.Size = new Size(103, 22);
            ExitToolStripMenuItem.Text = "exit";
            ExitToolStripMenuItem.Click += ExitApplication;
            // 
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            SystemTrayControl.Visible = false;
            Application.Exit();
        }

        private void OnClickOnSystemTrayControl(object sender, EventArgs e1)
        {
            if (SystemTrayControl.Text == "Start Spying")
            {
                StartListeningToMouseMoves();
            }
            else
            {
                StopListening();
            }
        }

        private void StartListeningToMouseMoves()
        {
            spyState = SpyState.Spying;
            Timer.Interval = TimeInterval;
            Timer.Tick += HandelMouseMoveToUpdateHilight;
            Timer.Start();
            KeyboardHook.SetHook();
            KeyboardHook.OnKeyPress += ListenKeyPress;
            SystemTrayControl.Text = "Stop Spying";
        }

        private void StopListening()
        {
            spyState = SpyState.Idel;
            Timer.Stop();
            KeyboardHook.OnKeyPress -= ListenKeyPress;
            Highlight.Visible = false;
            LivePropertyDisplay.Hide();
            SystemTrayControl.Text = "Start Spying";
            KeyboardHook.UnHook();
        }

        private void DisplayCurrentUiObjectProperties()
        {
            AutomationElement automationElementAtStartOfThread = _currentAutomationElement;
            LivePropertyDisplay.Hide();
            LivePropertyDisplay.LoadAutomationObjectForDisplay(automationElementAtStartOfThread);
            //            if (automationElementAtStartOfThread != _currentAutomationElement) return;
            LivePropertyDisplay.Show();
        }


        private void HandelMouseMoveToUpdateHilight(object sender, EventArgs e1)
        {
            _previousFocusedAutomationElement = _currentAutomationElement;
            _currentAutomationElement = CatchObjcetAtMouse.ObjectAtCurrentMousePosition;
            if (_previousFocusedAutomationElement.Equals(_currentAutomationElement)) return;
            _focusedRect = _currentAutomationElement.Current.BoundingRectangle;

            // Show rectangle
            Highlight.Location = new Rectangle(
                (int)_focusedRect.Left, (int)_focusedRect.Top,
                (int)_focusedRect.Width, (int)_focusedRect.Height);
            Highlight.Visible = true;

            DisplayCurrentUiObjectProperties();
        }


        private void ListenKeyPress(int keyChar)
        {
            if (keyChar == 27)
            {
                switch (spyState)
                {
                    case SpyState.Spying:
                        Timer.Stop();
                        spyState = SpyState.ShowProperties;
                        break;
                    case SpyState.ShowProperties:
                        StopListening();
                        break;
                }
            }
        }
    }
}