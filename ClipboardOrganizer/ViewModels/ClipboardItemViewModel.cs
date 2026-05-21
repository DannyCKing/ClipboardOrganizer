using ClipboardOrganizer.Utilities;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ClipboardOrganizer
{
    public class ClipboardItemViewModel : BaseViewModel
    {
        public event ClipboardItemUpdatedHandler ClipboardItemUpdated;
        public event ClipboardItemUpdatedHandler ClipboardItemCopied;
        public event ClipboardItemUpdatedHandler ClipboardItemCleared;
        public event ClipboardItemUpdatedHandler ClipboardItemDeleted;

        private int _Number;
        public int Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                OnPropertyChanged();
            }
        }

        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        private string _ClipboardValue = "";

        public string ClipboardValue
        {
            get
            {
                return _ClipboardValue;
            }
            set
            {
                _ClipboardValue = value;
                OnPropertyChanged();
            }
        }

        private string _Desc = "";

        public string Description
        {
            get
            {
                return _Desc;
            }
            set
            {
                _Desc = value;
                OnPropertyChanged();
            }
        }

        private ClipboardItemType _ItemType = ClipboardItemType.String;

        public ClipboardItemType ItemType
        {
            get { return _ItemType; }
            set
            {
                _ItemType = value;
                OnPropertyChanged();
            }
        }

        public string ItemTypeDisplay
        {
            get
            {
                switch (ItemType)
                {
                    case ClipboardItemType.FolderPath: return "Folder";
                    case ClipboardItemType.URL: return "URL";
                    case ClipboardItemType.Password: return "Password";
                    default: return "Text";
                }
            }
        }

        private ICommand _OpenCommand = null;

        public ICommand OpenCommand
        {
            get
            {
                if (_OpenCommand == null)
                {
                    _OpenCommand = new BaseCommand(OpenFunc, CanOpen);
                }

                return _OpenCommand;
            }
        }

        private bool CanOpen()
        {
            return ItemType == ClipboardItemType.FolderPath || ItemType == ClipboardItemType.URL;
        }

        private void OpenFunc()
        {
            try
            {
                if (ItemType == ClipboardItemType.URL)
                    Process.Start(new ProcessStartInfo(ClipboardValue) { UseShellExecute = true });
                else
                    Process.Start("explorer.exe", ClipboardValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error opening the path", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICommand _EditCommand = null;

        public ICommand EditCommand
        {
            get
            {
                if (_EditCommand == null)
                {
                    _EditCommand = new BaseCommand(EditFunc, CanEdit);
                }

                return _EditCommand;
            }
        }

        private ICommand _CopyCommand;

        public ICommand CopyCommand
        {
            get
            {
                if (_CopyCommand == null)
                {
                    _CopyCommand = new BaseCommand(CopyFunc, CanCopy);
                }

                return _CopyCommand;
            }
        }


        private ICommand _DeleteCommand;

        public ICommand DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new BaseCommand(DeleteFunc, () => true);
                }

                return _DeleteCommand;
            }
        }

        private void DeleteFunc()
        {
            var result = MessageBox.Show("Are you sure you want to delete this entry?", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemDeleted(new MessageEventArgs(MessageTypeEnum.Error, "Deleted"));
            }
        }

        public ClipboardItemViewModel()
        {

        }

        public ClipboardItemViewModel(int number, string name, string clipboardValue, string desc, ClipboardItemType itemType = ClipboardItemType.String)
        {
            Number = number;
            Name = name;
            ClipboardValue = clipboardValue;
            Description = desc;
            ItemType = itemType;
        }

        private bool CanEdit()
        {
            return true;
        }

        private void EditFunc()
        {
            var newWindow = new EditWindow(Number, Name, ClipboardValue, Description, ItemType);
            var result = newWindow.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                var clipboardItem = new ClipboardItemViewModel(newWindow.Id, newWindow.ClipName, newWindow.ClipboardValue, newWindow.Desc, newWindow.ItemType);
                FileUtilities.SaveClipboardItem(clipboardItem);
                ClipboardItemUpdated(new MessageEventArgs(MessageTypeEnum.Info, "Updated"));
            }
        }

        private bool CanCopy()
        {
            return true;
        }

        private void CopyFunc()
        {
            Clipboard.SetText(ClipboardValue);
            ClipboardItemUpdated(new MessageEventArgs(MessageTypeEnum.Good, "Copied"));
        }
    }
}