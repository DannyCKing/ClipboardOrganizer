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
using System.Windows.Shapes;

namespace ClipboardOrganizer
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public int Id { get; private set; }

        public string ClipName { get; private set; }

        public string ClipboardValue { get; private set; }

        public string Desc { get; private set; }

        public ClipboardItemType ItemType { get; private set; }

        public EditWindow(int id, string name, string clipValue, string desc, ClipboardItemType itemType = ClipboardItemType.String)
        {
            Id = id;
            ClipName = name;
            ClipboardValue = clipValue;
            Desc = desc;
            ItemType = itemType;
            InitializeComponent();

            this.Loaded += EditWindow_Loaded;
        }

        private void EditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = ClipName;
            DescriptionTextBox.Text = Desc;
            ValueTextBox.Text = ClipboardValue;
            TypeComboBox.ItemsSource = Enum.GetValues(typeof(ClipboardItemType));
            TypeComboBox.SelectedItem = ItemType;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ClipName = NameTextBox.Text;
            ClipboardValue = ValueTextBox.Text;
            Desc = DescriptionTextBox.Text;
            ItemType = (ClipboardItemType)TypeComboBox.SelectedItem;

            this.DialogResult = true;

            this.Close();
        }
    }
}
