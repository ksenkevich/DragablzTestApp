using Dragablz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragablzTestApp
{
    public class DockableTabInterTabClient : IInterTabClient
    {
        private TabablzControl _SourceTabablzControl;

        public TabablzControl SourceTabablzControl
        {
            get { return _SourceTabablzControl; }
            set { _SourceTabablzControl = value; }
        }

        public List<NewTabHost<Window>> NewTabHosts = new List<NewTabHost<Window>>();

        public INewTabHost<Window> GetNewHost(IInterTabClient interTabClient, object partition, TabablzControl source)
        {
            if (SourceTabablzControl == null)
            {
                SourceTabablzControl = source;
                SourceTabablzControl.IsVisibleChanged += SourceTabablzControl_IsVisibleChanged;
            }

            var window = new DragablzWindow {WindowState = WindowState.Normal};
            window.SizeChanged += Window_SizeChanged;
            window.StateChanged += Window_StateChanged;

            var tabControl = new TabablzControl
            {
                InterTabController = new InterTabController {InterTabClient = new DockableTabInterTabClient()}
            };

            window.Content = tabControl;
            var newTabHost = new NewTabHost<Window>(window, tabControl);
            NewTabHosts.Add(newTabHost);
            window.Closed += Window_Closed;
            return newTabHost;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            var window = sender as DragablzWindow;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as DragablzWindow;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var tabHost = NewTabHosts.FirstOrDefault(x => x.Container == (Window)sender);
            if (tabHost != null)
            {
                NewTabHosts.Remove(tabHost);
            }
        }

        private void SourceTabablzControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tabControl = (TabablzControl)sender;
            foreach (var host in NewTabHosts)
            {
                var window = host.Container;

                if (tabControl.IsVisible)
                {
                    window.Show();
                }
                else
                {
                    window.Hide();
                }
            }
        }

        public TabEmptiedResponse TabEmptiedHandler(TabablzControl tabControl, Window window)
        {
            return TabEmptiedResponse.CloseWindowOrLayoutBranch;
        }
    }
}
