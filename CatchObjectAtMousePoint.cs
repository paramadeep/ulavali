using System;
using System.Drawing;
using System.Windows.Automation;

namespace Ulavali
{
    public class CatchObjectAtMousePoint
    {
        private AutomationElement _objectAtCurrentMousePosition = AutomationElement.RootElement;

        public AutomationElement ObjectAtCurrentMousePosition
        {
            get
            {
                _objectAtCurrentMousePosition = getAutomationElementAtCurrentCursorPosition();
                return _objectAtCurrentMousePosition;
            }
        }

        private AutomationElement getAutomationElementAtCurrentCursorPosition()
        {
            try
            {
                var mousePosition = new Point();
                NativeMethods.GetCursorPos(ref mousePosition);
                //todo: handle if exception is thrown
                return AutomationElement.FromPoint(new System.Windows.Point(mousePosition.X, mousePosition.Y));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}    