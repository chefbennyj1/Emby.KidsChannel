using System.Collections.Generic;
using MediaBrowser.Controller.Session;

namespace Emby.WatchParty
{
    public class WatchParty
    {
        public long ItemId { get; set; }
        public List<string> SessionIds { get; set; } = new List<string>();
        public string ControllingDeviceId { get; set; }
    }
}