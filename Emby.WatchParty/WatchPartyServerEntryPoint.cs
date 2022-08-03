using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Model.Querying;
using MediaBrowser.Model.Session;

namespace Emby.WatchParty
{
    public class WatchPartyServerEntryPoint : IServerEntryPoint
    {
        private ISessionManager SessionManager { get; set; }
        private IUserManager UserManager { get; set; }
        private ILibraryManager LibraryManager { get; set; }

        public WatchPartyServerEntryPoint(ISessionManager sessionManager, IUserManager userManager, ILibraryManager libraryManager)
        {
            SessionManager = sessionManager;
            UserManager = userManager;
            LibraryManager = libraryManager;
        }

        public void Dispose()
        {
            SessionManager.PlaybackStart -= SessionManager_PlaybackStart;
            SessionManager.PlaybackStopped -= SessionManager_PlaybackStopped; 
        }

        public void Run()
        {
            Plugin.Instance.UpdateConfiguration(Plugin.Instance.Configuration);
           
            //var watchPartyLibrary = LibraryManager.GetVirtualFolders(true).FirstOrDefault(f => f.Name == "Watch Party");

            //if (watchPartyLibrary != null)
            //{
            //    LibraryManager.AddVirtualFolder("Watch Party", new LibraryOptions()
            //    {
            //        AutomaticRefreshIntervalDays = 1,
            //        CollapseSingleItemFolders = true, //Not sure what this does.
            //        ContentType = "Movies", //Hmmm... will this be a problem if watch parties wanted to watch tv shows? Someones gonna request eventually.
            //        DownloadImagesInAdvance = false,
            //        EnableAdultMetadata = false,
            //        EnableArchiveMediaFiles = false,
            //        Name = "Watch Party",
            //        EnableMarkerDetection = false, //No do not detect intros
            //        EnableRealtimeMonitor = false, //No do not monitor this library
            //    }, false);
            //}

            SessionManager.PlaybackStart += SessionManager_PlaybackStart;
            SessionManager.PlaybackStopped += SessionManager_PlaybackStopped;
        }

        private void SessionManager_PlaybackStopped(object sender, PlaybackStopEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void SessionManager_PlaybackStart(object sender, PlaybackProgressEventArgs e)
        {

            var config = Plugin.Instance.Configuration;

            //Our Admin user. We may need to control sessions based on them.
            var admin = UserManager.GetUsers(new UserQuery()).Items.FirstOrDefault(user => user.Policy.IsAdministrator);

            if (e.Item.Parent.Name != "Watch Party") return;//We don't care about anything else, just Watch Party.

            //A Party! I love parties!

            var party = config.Parties.FirstOrDefault(p => p.ItemId == e.Item.InternalId); //<== This party exists. The User just selected it.
           
            //Add the session to the party.
            party?.SessionIds.Add(e.Session.Id);

            //We need to stop Emby from actually playing anything after the item is selected - Pause the session.
            //What happens if the user has Custom Intros?? Needs testing. That could be a problem.
            await SessionManager.SendPlaystateCommand(null, e.Session.Id,
                new PlaystateRequest()
                {
                    Command = PlaystateCommand.Pause, 
                    ControllingUserId = admin?.Id.ToString()

                }, CancellationToken.None); 

           
        }

        private async Task BeginWatchPartyPlayback(WatchParty party)
        {
            foreach (var sessionId in party.SessionIds)
            {
                var currentMemberSession =
                    SessionManager.Sessions.FirstOrDefault(session => session.Id == sessionId);

                if(currentMemberSession is null) continue; //<== Maybe they logged off - move on!

                await SessionManager.SendPlayCommand(null, currentMemberSession.Id,
                    new PlayRequest() { ItemIds = new[] { party.ItemId, } }, CancellationToken.None); // Start the playback on the users device.
            }
        }
    }
}
