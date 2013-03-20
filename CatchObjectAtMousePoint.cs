using System;
using System.Drawing;
using System.Windows.Automation;

namespace White_Spy
{
    public class CatchObjectAtMousePoint
    {
        private AutomationElement objectAtCurrentMousePosition = AutomationElement.RootElement;
        private AutomationElement objectAtPreviousMousePosition;

        public AutomationElement ObjectAtCurrentMousePosition
        {
            get
            {
                objectAtPreviousMousePosition = objectAtCurrentMousePosition;
                objectAtCurrentMousePosition = getAutomationElementAtCurrentCursorPosition();
                return objectAtCurrentMousePosition;
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
//        private void OnTimerTick()
//        {
//            try
//            {
//                if (!objectAtPreviousMousePosition.Equals(ObjectAtCurrentMousePosition))
//                {
//                    if (!objectAtCurrentMousePosition.Current.ProcessId.Equals(Process.GetCurrentProcess().Id))
//                    {
//                        OnMouseMove(null, ObjectAtCurrentMousePosition);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//            }
//        }

//        public event OnMouseMoveHandler OnMouseMove;
//
//
//
//
//        public void StopListening()
//        {
//            mouseHooker.RemoveHook();
//            MouseHooker.OnMouseMove -= OnTimerTick;
//        }
//
//        public void StartListening()
//        {
//            MouseHooker.OnMouseMove += OnTimerTick;
//            mouseHooker.StartMouseHook();
//
//        }
//    }
//
//    public delegate void OnMouseMoveHandler(object sender, AutomationElement currentCursorPosition);
    