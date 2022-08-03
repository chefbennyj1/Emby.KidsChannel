using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Services;
using MediaBrowser.Model.Tasks;

namespace Emby.WatchParty.Api
{
    public class WatchPartyService : IService
    {
        [Route("/CreateWatchParty", "POST", Summary = "Create a watch party element in the Watch Party Channel")]

        public class CreateWatchPartyRequest : IReturnVoid
        {
            [ApiMember(Name = "InternalIds", Description = "Internal Id of the base item to add to the watch party", IsRequired = true, DataType = "long", ParameterType = "query", Verb = "POST")]
            public long InternalId { get; set; }
        }

        private ITaskManager TaskManager { get; set; }
        public WatchPartyService(ITaskManager taskManager)
        {
            TaskManager = taskManager;
        }

        public async void Post(CreateWatchPartyRequest request)
        {
            var watchParty = new WatchParty
            {
                ItemId = request.InternalId
            };

            var config = Plugin.Instance.Configuration;
            var parties = new List<WatchParty> { watchParty };




            Plugin.Instance.Configuration.Parties = parties;

            Plugin.Instance.UpdateConfiguration(config);

            var channelTask = TaskManager.ScheduledTasks.FirstOrDefault(t => t.Name == "Refresh Internet Channels");
            if (channelTask != null)
            {
                await TaskManager.Execute(channelTask, new TaskOptions());
            }
           
        }
    }
}
