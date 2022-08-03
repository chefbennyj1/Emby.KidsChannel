using System.IO;
using System.Linq;
using MediaBrowser.Controller.Library;

namespace Emby.WatchParty
{
    public class StrmFileWriter
    {
        private ILibraryManager LibraryManager { get; set; }
        private ILibraryMonitor LibraryMonitor { get; set; }
        public StrmFileWriter(ILibraryManager libraryManager, ILibraryMonitor libraryMonitor)
        {
            LibraryManager = libraryManager;
            LibraryMonitor = libraryMonitor;
        }
        public string GetStrmFilePath(string name) //name: "Watch Party"
        {
            var virtualFolders =
                LibraryManager.GetVirtualFolders(false).FirstOrDefault(f => f.Name == name);

            //Best check it exists
            var virtualFolderPath = virtualFolders?.Locations?.FirstOrDefault();

            if (virtualFolderPath == null) return null; //This should not happen. But, since it could... check for null

            return Path.Combine(virtualFolderPath, string.Concat(name, ".strm"));

        }

        public void UpdateStrmFile(long internalId, string strmFilePath)
        {
            var item = LibraryManager.GetItemById(internalId);
            LibraryMonitor.ReportFileSystemChangeBeginning(strmFilePath); //Tell Emby we are changing this location
            using (var sw = new StreamWriter(strmFilePath, append: false)) //DO not append. Overwrite with the new value. 
            {
                sw.Write(item.Path);
                sw.Flush();
            }

            //Tell Emby, we have finished changing this location, and it should be refresh.
            //This should update the Watch Party
            LibraryMonitor.ReportFileSystemChangeComplete(strmFilePath, refreshPath: true); 
        }
    }
}
