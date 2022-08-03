using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;

namespace Emby.WatchParty
{
    public class WatchPartyChannel : IChannel, IHasCacheKey, IRequiresMediaInfoCallback
    {
        private ILibraryManager LibraryManager { get; set; }
        public WatchPartyChannel(ILibraryManager libraryManager)
        {
            LibraryManager = libraryManager;
        }
        public async Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            var config = Plugin.Instance.Configuration;
            var party = config.Parties.FirstOrDefault();
            if (party == null) return null;
            var item = LibraryManager.GetItemById(party.ItemId);
            

            var items = new List<ChannelItemInfo>
            {
                new ChannelItemInfo()
                {
                    Name          = item.Name,
                    ImageUrl      = item.PrimaryImagePath,
                    Type          = ChannelItemType.Media,
                    ContentType   = GetChannelMediaContentType(item.GetType().Name),
                    MediaType     = ChannelMediaType.Video,
                    IsLiveStream  = false,
                    MediaSources  = new List<MediaSourceInfo> { new ChannelMediaInfo { Path = item.Path, Protocol = MediaProtocol.File }.ToMediaSource() },
                    OriginalTitle = item.OriginalTitle
                }
            };
                
            return await Task.FromResult(new ChannelItemResult
            {
                Items = items
            });

        }

        public async Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken)
        {
            var path = GetType().Namespace + ".Images." + type.ToString().ToLower() + ".jpg";

            return await Task.FromResult(new DynamicImageResponse
            {
                Format = ImageFormat.Jpg,
                IsFallbackImage = true,
                Protocol = MediaProtocol.File,
                Stream = GetType().Assembly.GetManifestResourceStream(path)
            });

            
        }

        public IEnumerable<ImageType> GetSupportedChannelImages()
        {
            return new List<ImageType>() { ImageType.Thumb };
        }

        public string Name => Plugin.Instance.Name;
        public string Description => Plugin.Instance.Description;
        public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience; 
        
        public string GetCacheKey(string userId) => Guid.NewGuid().ToString("N");

        public Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private ChannelMediaContentType GetChannelMediaContentType(string type)
        {
            switch (type)
            {
                case "Movie": return ChannelMediaContentType.Movie;
                case "Episode": return ChannelMediaContentType.Episode;
                default: return ChannelMediaContentType.Movie;
            }
        }
    }
}
