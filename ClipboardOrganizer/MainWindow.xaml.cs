using ClipboardOrganizer.Utilities;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClipboardOrganizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClipboardListViewModel ListVM = new ClipboardListViewModel();
        private Storyboard myStoryboard;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myStoryboard = (Storyboard)this.Resources["FadeOutStoryboard"];
            myStoryboard.Completed += FadeOut_Completed;
            var margin = 25;
            var rect = GetScreen();

            var maxWidth = 300;

            var minWidth = 200;

            var currentWidth = rect.Width * 0.20;

            if(currentWidth > maxWidth)
            {
                currentWidth = maxWidth;
            }
            else if(currentWidth < minWidth)
            {
                currentWidth = minWidth;
            }

            this.Width = currentWidth;
            Left = rect.Width - currentWidth - margin;

            Height = rect.Height * 0.9;

            Top = rect.Height * 0.05;

            this.MainGrid.DataContext = ListVM;

            ListVM.ClipboardItemUpdatedVM += VM_ClipboardItemCopiedVM;
        }

        private void VM_ClipboardItemCopiedVM(MessageEventArgs args)
        {
            switch (args.MessageType)
            {
                case MessageTypeEnum.Good:
                    ToastAreaLabel.Background = new SolidColorBrush(Colors.Green);
                    break;
                case MessageTypeEnum.Info:
                    ToastAreaLabel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 87, 51));
                    break;
                case MessageTypeEnum.Warning:
                    ToastAreaLabel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 87, 51));
                    break;
                case MessageTypeEnum.Error:
                    ToastAreaLabel.Background = new SolidColorBrush(Colors.Red);
                    break;
            }

            ToastAreaLabel.Content = args.Message;

            ToastArea.Visibility = Visibility.Visible;

            myStoryboard.Begin();
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            ToastArea.Visibility = Visibility.Collapsed;
        }

        public Rect GetScreen()
        {
            return System.Windows.SystemParameters.WorkArea;
        }
    }
}
