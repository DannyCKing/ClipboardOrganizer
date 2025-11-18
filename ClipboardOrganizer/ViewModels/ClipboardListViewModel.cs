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

        public void ReloadList()
        {
            ClipboardItems = new ObservableCollection<ClipboardItemViewModel>();

            var items = fileUtilities.GetCurrentClipboardModels();
            foreach(var item in items)
            {
                item.ClipboardItemUpdated += Item_ClipboardItemUpdated;
                item.ClipboardItemCopied += Item_ClipboardItemCopied;
                item.ClipboardItemCleared += Item_ClipboardItemCleared; ;

            }
            ClipboardItems = new ObservableCollection<ClipboardItemViewModel>(items);
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
