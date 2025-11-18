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

        public const int MAX_ITEMS = 20;
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
            List<ClipboardItemViewModel> items = new List<ClipboardItemViewModel>();
            var files = Directory.GetFiles(ClipboardDirectory);

            for (int i = 1; i <= MAX_ITEMS; i++)
            {
                var fileName = CreateFileName(i);
                var fullPath = Path.Combine(ClipboardDirectory, fileName);
                var clipboardModel = ParseClipboardFile(fullPath);

                // no record exist yet in this slot, insert an empty record
                if (clipboardModel == null)
                {
                    clipboardModel = new ClipboardItemViewModel(i, "EMPTY", "", "");
                    SaveClipboardItem(clipboardModel);
                }
                items.Add(clipboardModel);
            }

            return items;
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

            var lines = new List<string>() { clipboardModel.Number.ToString(), clipboardModel.Name, clipValueEncoded, clipDescEncoded };
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
                    return new ClipboardItemViewModel(int.Parse(lines[0]), lines[1], clipValue, clipDesc);
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
