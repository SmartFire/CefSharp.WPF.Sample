using CefSharp.WPF.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CefSharp.WPF.Sample.Views
{
    /// <summary>
    /// Interaction logic for CefBrowserView.xaml
    /// </summary>
    public partial class CefBrowserView : UserControl
    {
        public CefBrowserView()
        {
            InitializeComponent();
            this.DataContext = new CefBrowserViewModel(this.cefWebView, this.Dispatcher);
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                (sender as TextBox).SelectAll();
            }, DispatcherPriority.Input);
            e.Handled = true;
        }
    }
}
