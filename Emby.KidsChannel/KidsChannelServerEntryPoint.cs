
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Tasks;

namespace Emby.KidsChannel
{
    public class KidsChannelServerEntryPoint : IServerEntryPoint
    {
        private ISessionManager SessionManager { get; set; }
        private ITaskManager TaskManager { get; set; }
        private ILibraryManager LibraryManager { get; set; }
        private ILogger Log { get; set; }
        
        public KidsChannelServerEntryPoint(ISessionManager sessionManager, ITaskManager taskManager, ILibraryManager libraryManager, ILogManager logManager)
        {
            SessionManager = sessionManager;
            TaskManager = taskManager;
            LibraryManager = libraryManager;
            Log = logManager.GetLogger(Plugin.Instance.Name);
            
        }

        public void Dispose()
        {
           
        }

        public void Run()
        {
           
        }


        private void SessionManager_SessionActivity(object sender, SessionEventArgs e)
        {
            
        }

        private void SessionManager_PlaybackProgress(object sender, PlaybackProgressEventArgs e)
        {

           
        }

        private  void SessionManager_PlaybackStopped(object sender, PlaybackStopEventArgs e)
        {

        }

        private void SessionManager_PlaybackStart(object sender, PlaybackProgressEventArgs e)
        {
            
        }

        
    }
}
