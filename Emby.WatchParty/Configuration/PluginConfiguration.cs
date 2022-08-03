using System.Collections.Generic;
using MediaBrowser.Model.Plugins;

namespace Emby.WatchParty.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public List<WatchParty> Parties { get; set; } = new List<WatchParty>();

    }
}
