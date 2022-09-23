using System;
using System.Collections.Generic;
using System.IO;
using Emby.KidsChannel.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Emby.KidsChannel
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasThumbImage
    {
        public static Plugin Instance { get; set; }
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths,
            xmlSerializer)
        {
            Instance = this;
        }

        public override string Name => "Kids Movies";
        public override string Description => "Spotlight movie items for kids in the library.";

        public override Guid Id =>  new Guid("00C58D53-32F4-4F70-A225-510E714539A8");
        
        public Stream GetThumbImage()
        {
            var type = GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".images.thumb.jpg");
        }

        public ImageFormat ThumbImageFormat => ImageFormat.Jpg;

       
    }
}
