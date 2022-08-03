using System;
using System.Collections.Generic;
using System.IO;
using Emby.WatchParty.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Emby.WatchParty
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
        public static Plugin Instance { get; set; }
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths,
            xmlSerializer)
        {
            Instance = this;
        }

        public override string Name => "Watch Party";
        public override string Description => "Sync media streams across multiple users to watch library items together.";

        public override Guid Id =>  new Guid("672630FC-6FB8-4207-9C7B-AB414CD85B53");
        
        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".images.thumb.jpg");
        }

        public ImageFormat ThumbImageFormat => ImageFormat.Jpg;

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "WatchPartyConfigurationPage",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.WatchPartyConfigurationPage.html",
                    EnableInMainMenu = true
                },
                new PluginPageInfo
                {
                    Name = "WatchPartyConfigurationPageJS",
                    EmbeddedResourcePath = GetType().Namespace + ".Configuration.WatchPartyConfigurationPage.js"
                }
            };
        }
    }
}
