using ClipboardOrganizer.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClipboardOrganizer
{
    public delegate void ClipboardItemUpdatedHandler(MessageEventArgs messageEventArgs);

    public class ClipboardListViewModel : BaseViewModel
    {
        public event ClipboardItemUpdatedHandler ClipboardItemUpdatedVM;

        private FileUtilities fileUtilities = new FileUtilities();

        private ObservableCollection<ClipboardItemViewModel> _ClipboardItems { get; set; }
        public ObservableCollection<ClipboardItemViewModel> ClipboardItems
        {
            get
            {
                return _ClipboardItems;
            }
            set
            {
                _ClipboardItems = value;
                OnPropertyChanged();
            }
        }

        public ClipboardListViewModel()
        {
            ReloadList();
        }

        private ICommand _AddItemCommand;
        public ICommand AddItemCommand
        {
            get
            {
                if (_AddItemCommand == null)
                    _AddItemCommand = new BaseCommand(AddItemFunc, () => true);
                return _AddItemCommand;
            }
        }

        public void ReloadList()
        {
            var items = fileUtilities.GetCurrentClipboardModels();
            foreach (var item in items)
            {
                item.ClipboardItemUpdated += Item_ClipboardItemUpdated;
                item.ClipboardItemCopied += Item_ClipboardItemCopied;
                item.ClipboardItemCleared += Item_ClipboardItemCleared;
                item.ClipboardItemDeleted += (args) => Item_ClipboardItemDeleted(args, item);
            }
            ClipboardItems = new ObservableCollection<ClipboardItemViewModel>(items);
        }

        private void AddItemFunc()
        {
            var newNumber = FileUtilities.GetNextItemNumber();
            var newWindow = new EditWindow(newNumber, "", "", "");
            var result = newWindow.ShowDialog();
            if (result.HasValue && result.Value == true)
            {
                var newItem = new ClipboardItemViewModel(newNumber, newWindow.ClipName, newWindow.ClipboardValue, newWindow.Desc);
                FileUtilities.SaveClipboardItem(newItem);
                newItem.ClipboardItemUpdated += Item_ClipboardItemUpdated;
                newItem.ClipboardItemCopied += Item_ClipboardItemCopied;
                newItem.ClipboardItemCleared += Item_ClipboardItemCleared;
                newItem.ClipboardItemDeleted += (args) => Item_ClipboardItemDeleted(args, newItem);
                ClipboardItems.Add(newItem);
                ClipboardItems = new ObservableCollection<ClipboardItemViewModel>(
                    ClipboardItems.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase));
                ClipboardItemUpdatedVM(new MessageEventArgs(MessageTypeEnum.Good, "Added"));
            }
        }

        private void Item_ClipboardItemDeleted(MessageEventArgs args, ClipboardItemViewModel item)
        {
            FileUtilities.DeleteClipboardItem(item.Number);
            ClipboardItems.Remove(item);
            ClipboardItemUpdatedVM(args);
        }

        private void Item_ClipboardItemCleared(MessageEventArgs args)
        {
            ClipboardItemUpdatedVM(args);
        }

        private void Item_ClipboardItemCopied(MessageEventArgs args)
        {
            ClipboardItemUpdatedVM(args);
        }

        private void Item_ClipboardItemUpdated(MessageEventArgs args)
        {
            ReloadList();
            ClipboardItemUpdatedVM(args);
        }
    }
}
