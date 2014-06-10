using System;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Ulavali.Properties;
using Size=System.Drawing.Size;

namespace Ulavali
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
        private readonly CatchObjectAtMousePoint _catchObjcetAtMouse = new CatchObjectAtMousePoint();

        private readonly ToolStripMenuItem _exitToolStripMenuItem = new ToolStripMenuItem();
        private readonly HighlightRectangle _highlight = new HighlightRectangle();
        private readonly KeyboardHook _keyboardHook = new KeyboardHook();
        private readonly LivePropertyDisplay _livePropertyDisplay = new LivePropertyDisplay();
        private readonly NotifyIcon _systemTrayControl = new NotifyIcon();
        private readonly ContextMenuStrip _systemTrayMenu = new ContextMenuStrip();
        private readonly Timer _timer = new Timer();
        private static AutomationElement _currentAutomationElement = AutomationElement.RootElement;
        private static Rect _focusedRect;
        private static AutomationElement _previousFocusedAutomationElement;
        private static SpyState _spyState;
        
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
            _systemTrayControl.Visible = true;
            _systemTrayControl.Icon = new Icon("image\\User.ico");

            // 
            // SystemTrayControl
            // 
            _systemTrayControl.BalloonTipText = Resources.Start_Spying;
            _systemTrayControl.ContextMenuStrip = _systemTrayMenu;
            _systemTrayControl.Text = Resources.Start_Spying;
            _systemTrayControl.Visible = true;
            _systemTrayControl.Click += OnClickOnSystemTrayControl;
            // 
            // SystemTrayMenu
            // 
            _systemTrayMenu.Items.AddRange(new ToolStripItem[]
                                              {
                                                  _exitToolStripMenuItem
                                              });
            _systemTrayMenu.Name = "SystemTrayMenu";
            _systemTrayMenu.Size = new Size(104, 48);
            // 
            // ExitToolStripMenuItem
            // 
            _exitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            _exitToolStripMenuItem.Size = new Size(103, 22);
            _exitToolStripMenuItem.Text = Resources.SystemTrayControl_exit;
            _exitToolStripMenuItem.Click += ExitApplication;
            // 
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            _systemTrayControl.Visible = false;
            Application.Exit();
        }

        private void OnClickOnSystemTrayControl(object sender, EventArgs e1)
        {
            if (_systemTrayControl.Text == Resources.Start_Spying)
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
            _spyState = SpyState.Spying;
            _timer.Interval = TimeInterval;
            _timer.Tick += HandelMouseMoveToUpdateHilight;
            _timer.Start();
            _keyboardHook.SetHook();
            KeyboardHook.OnKeyPress += ListenKeyPress;
            _systemTrayControl.Text = Resources.Stop_Spying;
        }

        private void StopListening()
        {
            _spyState = SpyState.Idel;
            _timer.Stop();
            KeyboardHook.OnKeyPress -= ListenKeyPress;
            _highlight.Visible = false;
            _livePropertyDisplay.Hide();
            _systemTrayControl.Text = Resources.Start_Spying;
            _keyboardHook.UnHook();
        }

        private void DisplayCurrentUiObjectProperties()
        {
            var automationElementAtStartOfThread = _currentAutomationElement;
            _livePropertyDisplay.Hide();
            _livePropertyDisplay.LoadAutomationObjectForDisplay(automationElementAtStartOfThread);
            _livePropertyDisplay.Show();
        }


        private void HandelMouseMoveToUpdateHilight(object sender, EventArgs e1)
        {
            _previousFocusedAutomationElement = _currentAutomationElement;
            _currentAutomationElement = _catchObjcetAtMouse.ObjectAtCurrentMousePosition;
            if (_previousFocusedAutomationElement.Equals(_currentAutomationElement)) return;
            _focusedRect = _currentAutomationElement.Current.BoundingRectangle;

            _highlight.Location = new Rectangle(
                (int)_focusedRect.Left, (int)_focusedRect.Top,
                (int)_focusedRect.Width, (int)_focusedRect.Height);
            _highlight.Visible = true;

            DisplayCurrentUiObjectProperties();
        }


        private void ListenKeyPress(int keyChar)
        {
            if (keyChar != 27) return;
            switch (_spyState)
            {
                case SpyState.Spying:
                    _timer.Stop();
                    _spyState = SpyState.ShowProperties;
                    break;
                case SpyState.ShowProperties:
                    StopListening();
                    break;
            }
        }
    }
}