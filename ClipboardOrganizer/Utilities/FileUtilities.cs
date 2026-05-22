using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardOrganizer
{
    internal class FileUtilities
    {
        public const string APP_NAME = "ClipboardOrganizer";

        public const string SOFTWARE_DEVELOPER_NAME = "KingSoftware";

        public const string CLIPBOARD_ITEMS = "ClipboardItems";

        public const string CLIPBOARD_ITEM_FILE_NAME_PREFIX = "ClipboardItem_";

        public static string DeveloperDirectory
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var developerDirectory = Path.Combine(appData, SOFTWARE_DEVELOPER_NAME);

                if (!Directory.Exists(developerDirectory))
                {
                    Directory.CreateDirectory(developerDirectory);
                }

                return developerDirectory;
            }
        }

        public static string AppDirectory
        {
            get
            {
                var appDirectory = Path.Combine(DeveloperDirectory, APP_NAME);

                if (!Directory.Exists(appDirectory))
                {
                    Directory.CreateDirectory(appDirectory);
                }

                return appDirectory;
            }
        }

        public static string ClipboardDirectory
        {
            get
            {
                var clipboardDirectory = Path.Combine(AppDirectory, CLIPBOARD_ITEMS);

                if (!Directory.Exists(clipboardDirectory))
                {
                    Directory.CreateDirectory(clipboardDirectory);
                }

                return clipboardDirectory;
            }
        }

        public List<ClipboardItemViewModel> GetCurrentClipboardModels()
        {
            var items = new List<ClipboardItemViewModel>();
            var files = Directory.GetFiles(ClipboardDirectory, CLIPBOARD_ITEM_FILE_NAME_PREFIX + "*.txt");

            foreach (var file in files)
            {
                var item = ParseClipboardFile(file);
                if (item != null && !(item.Name == "EMPTY" && string.IsNullOrEmpty(item.ClipboardValue)))
                    items.Add(item);
            }

            return items.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static void DeleteClipboardItem(int number)
        {
            var fullPath = Path.Combine(ClipboardDirectory, CreateFileName(number));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        public static int GetNextItemNumber()
        {
            var files = Directory.GetFiles(ClipboardDirectory, CLIPBOARD_ITEM_FILE_NAME_PREFIX + "*.txt");
            if (files.Length == 0) return 1;
            var numbers = files
                .Select(f => Path.GetFileNameWithoutExtension(f).Replace(CLIPBOARD_ITEM_FILE_NAME_PREFIX, ""))
                .Select(n => int.TryParse(n, out int num) ? num : 0)
                .Where(n => n > 0);
            return numbers.Any() ? numbers.Max() + 1 : 1;
        }

        private static string CreateFileName(int fileNumber)
        {
            var path = CLIPBOARD_ITEM_FILE_NAME_PREFIX + fileNumber.ToString("00") + ".txt";
            return path;
        }

        public static void SaveClipboardItem(ClipboardItemViewModel clipboardModel)
        {
            var clipboardFileName = CreateFileName(clipboardModel.Number);
            var fullFilePath = Path.Combine(ClipboardDirectory, clipboardFileName);
            var clipValueEncoded = Base64Encode(clipboardModel.ClipboardValue.Trim());
            var clipDescEncoded = Base64Encode(clipboardModel.Description.Trim());

            var lines = new List<string>() { clipboardModel.Number.ToString(), clipboardModel.Name, clipValueEncoded, clipDescEncoded, clipboardModel.ItemType.ToString() };
            File.WriteAllLines(fullFilePath, lines);
        }

        private ClipboardItemViewModel ParseClipboardFile(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
            {
                return null;
            }
            else
            {
                var lines = File.ReadAllLines(fullFilePath);
                try
                {
                    var clipValue = Base64Decode(lines[2]);
                    var clipDesc = Base64Decode(lines[3]);
                    var itemType = ClipboardItemType.String;
                    if (lines.Length > 4)
                        Enum.TryParse(lines[4], out itemType);
                    return new ClipboardItemViewModel(int.Parse(lines[0]), lines[1], clipValue, clipDesc, itemType);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
