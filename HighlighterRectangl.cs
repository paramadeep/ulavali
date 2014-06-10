using System.Drawing;
using System.Windows.Forms;

namespace Ulavali
{
    internal class HighlightRectangle 
    {
        #region Private Fields

        private bool _highlightShown;
        private readonly int _highlightLineWidth;
        private Rectangle _highlightLocation;

        private readonly Form _leftForm;
        private readonly Form _topForm;
        private readonly Form _rightForm;
        private readonly Form _bottomForm;
        private readonly Form _infoForm;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// Creates each side of the highlight rectangle as a form, so that
        /// drawing and erasing are handled automatically.
        /// </remarks>
        public HighlightRectangle()
        {
            // Construct the rectangle and set some values.
            _highlightShown = false;
            _highlightLineWidth = 3;
            _leftForm = new Form();
            _topForm = new Form();
            _rightForm = new Form();
            _bottomForm = new Form();
            _infoForm = new Form();
            Form[] forms = { _leftForm, _topForm, _rightForm, _bottomForm };
            foreach (Form form in forms)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.ShowInTaskbar = false;
                form.TopMost = true;
                form.Visible = false;
                form.Left = 0;
                form.Top = 0;
                form.Width = 1;
                form.Height = 1;
                form.BackColor = Color.Red;

                // Make it a tool window so it doesn't show up with Alt+Tab.
                int style = NativeMethods.GetWindowLong(
                    form.Handle, NativeMethods.GWL_EXSTYLE);
                NativeMethods.SetWindowLong(
                    form.Handle, NativeMethods.GWL_EXSTYLE,
                    style | NativeMethods.WS_EX_TOOLWINDOW);
            }

            
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Sets the visible state of the rectangle.
        /// </summary>
        /// <remarks>
        /// The Layout method is called by using BeginInvoke, to prevent
        /// cross-thread updates to the UI. This method can be called on
        /// any form that belongs to the UI thread.
        /// </remarks>
        public bool Visible
        {
            set
            {
                if (_highlightShown == value) return;
                _highlightShown = value;
                if (_highlightShown)
                {
                    var mi = new MethodInvoker(Layout);
                    _leftForm.BeginInvoke(mi);
                    mi = ShowRectangle;
                    _leftForm.BeginInvoke(mi);
                }
                else
                {
                    var mi = new MethodInvoker(HideRectangle);
                    _leftForm.BeginInvoke(mi);
                }
            }
        }

        /// <summary>
        /// Sets the location of the highlight.
        /// </summary>
        public Rectangle Location
        {
            set
            {
                _highlightLocation = value;
                var mi = new MethodInvoker(Layout);
                _leftForm.BeginInvoke(mi);
            }
            get { return _highlightLocation; }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Shows or hides the rectangle.
        /// </summary>
        /// <param name="show">true to show, false to hide.</param>
        private void Show(bool show)
        {
            if (show)
            {
                NativeMethods.ShowWindow(_leftForm.Handle, NativeMethods.SW_SHOWNA);
                NativeMethods.ShowWindow(_topForm.Handle, NativeMethods.SW_SHOWNA);
                NativeMethods.ShowWindow(_rightForm.Handle, NativeMethods.SW_SHOWNA);
                NativeMethods.ShowWindow(_bottomForm.Handle, NativeMethods.SW_SHOWNA);
            }
            else
            {
                _leftForm.Hide();
                _topForm.Hide();
                _rightForm.Hide();
                _bottomForm.Hide();
            }
        }

        /// <summary>
        /// Shows the highlight.
        /// </summary>
        /// <remarks> Parameterless method for MethodInvoker.</remarks>
        void ShowRectangle()
        {
            Show(true);
        }

        /// <summary>
        /// Hides the highlight.
        /// </summary>
        /// <remarks> Parameterless method for MethodInvoker.</remarks>
        void HideRectangle()
        {
            Show(false);
        }

        /// <summary>
        /// Sets the position and size of the four forms that make up the rectangle.
        /// </summary>
        /// <remarks>
        /// Use the Win32 SetWindowPosfunction so that SWP_NOACTIVATE can be set. 
        /// This ensures that the windows are shown without receiving the focus.
        /// </remarks>
        private void Layout()
        {
            // Use SetWindowPos instead of changing the location via form properties: 
            // this allows us to also specify HWND_TOPMOST. 
            // Using Form.TopMost = true to do this has the side-effect
            // of activating the rectangle windows, causing them to gain the focus.
            NativeMethods.SetWindowPos(_leftForm.Handle, NativeMethods.HWND_TOPMOST,
                        _highlightLocation.Left - _highlightLineWidth, 
                        _highlightLocation.Top, 
                        _highlightLineWidth, _highlightLocation.Height, 
                        NativeMethods.SWP_NOACTIVATE);
            NativeMethods.SetWindowPos(_topForm.Handle, NativeMethods.HWND_TOPMOST,
                        _highlightLocation.Left - _highlightLineWidth, 
                        _highlightLocation.Top - _highlightLineWidth, 
                        _highlightLocation.Width + 2 * _highlightLineWidth, 
                        _highlightLineWidth, 
                        NativeMethods.SWP_NOACTIVATE);
            NativeMethods.SetWindowPos(_rightForm.Handle, NativeMethods.HWND_TOPMOST,
                        _highlightLocation.Left + _highlightLocation.Width, 
                        _highlightLocation.Top, _highlightLineWidth, 
                        _highlightLocation.Height, 
                        NativeMethods.SWP_NOACTIVATE);
            NativeMethods.SetWindowPos(_bottomForm.Handle, NativeMethods.HWND_TOPMOST,
                        _highlightLocation.Left - _highlightLineWidth, 
                        _highlightLocation.Top + _highlightLocation.Height, 
                        _highlightLocation.Width + 2 * _highlightLineWidth, 
                        _highlightLineWidth, 
                        NativeMethods.SWP_NOACTIVATE);
            NativeMethods.SetWindowPos(_infoForm.Handle, NativeMethods.HWND_TOPMOST,
                        _highlightLocation.Left - _highlightLineWidth, 
                        _highlightLocation.Top + _highlightLocation.Height, 
                        5, 
                        5, 
                        NativeMethods.SWP_NOACTIVATE);
        }


        #endregion

    }  // class
}  // namespace

