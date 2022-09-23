using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Common.Extensions;

namespace Emby.KidsChannel
{
    public class KidsChannel : IChannel, IHasCacheKey, IRequiresMediaInfoCallback
    {
        private ILibraryManager LibraryManager { get; set; }
        private IServerApplicationHost ApplicationHost { get; set; }
        public KidsChannel(ILibraryManager libraryManager, IServerApplicationHost host)
        {
            LibraryManager = libraryManager;
            ApplicationHost = host;
        }
        public string DataVersion => "4";

        public bool IsEnabledFor(string userId)
        {
            return true;
        }
        
        public InternalChannelFeatures GetChannelFeatures()
        {
            return new InternalChannelFeatures
            {
                ContentTypes = new List<ChannelMediaContentType>
                {
                    ChannelMediaContentType.Movie
                },

                MediaTypes = new List<ChannelMediaType>
                {
                    ChannelMediaType.Video,

                },

                SupportsContentDownloading = true,
                SupportsSortOrderToggle    = true,
                DefaultSortFields = new List<ChannelItemSortField> { ChannelItemSortField.PremiereDate }
            };
        }

        public async Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query, CancellationToken cancellationToken)
        {
            var result = LibraryManager.GetItemsResult(new InternalItemsQuery()
            {
                IncludeItemTypes = new []{ "Movie" },
                Recursive = true,
                DtoOptions = new DtoOptions(true),
                MinParentalRating = 1,
                MaxParentalRating = 1,

            });

            var items = new List<ChannelItemInfo>();
            
            foreach (var item in result.Items) 

            {
                if (item.Path == string.Empty) continue;

                if (items.Exists(i => i.Name == item.Name && i.ProductionYear == item.ProductionYear))
                {
                    var channelItem = items.FirstOrDefault(i => i.Name == item.Name && i.ProductionYear == item.ProductionYear);

                    if(channelItem is null) continue;

                    var mediaSources = item.GetMediaSources(false, false, LibraryManager.GetLibraryOptions(item)); 
                    
                    foreach(var source in mediaSources)
                    {
                        if(channelItem.MediaSources.Exists(s => s.Path == source.Path)) continue;

                        source.Id = $"kids_{source.Path}".GetMD5().ToString("N");
                        channelItem.MediaSources.Add(source);
                    }

                    continue;
                }

                var sources = item.GetMediaSources(false, false, LibraryManager.GetLibraryOptions(item));
                var sourceList = new List<MediaSourceInfo>();
                foreach(var source in sources)
                {
                    source.Id = $"kids_{source.Path}".GetMD5().ToString("N");
                    source.Protocol = MediaProtocol.File;
                    if(sourceList.Exists(s => s.Path == source.Path)) continue;
                    sourceList.Add(source);
                }

                items.Add(
                    new ChannelItemInfo()
                    {
                        DateCreated     = item.DateCreated,
                        Name            = item.Name,
                        Id              = $"kids_{item.InternalId}".GetMD5().ToString("N"),                         
                        RunTimeTicks    = item.RunTimeTicks,                       
                        ProductionYear  = item.ProductionYear,
                        ImageUrl        = item.PrimaryImagePath, 
                        Type            = ChannelItemType.Media,
                        ContentType     = ChannelMediaContentType.Movie,
                        MediaType       = ChannelMediaType.Video,
                        IsLiveStream    = false,
                        OfficialRating  = item.OfficialRating,
                        Overview        = item.Overview,
                        PremiereDate    = item.PremiereDate,
                        Genres          = item.Genres.ToList(),
                        CommunityRating = item.CommunityRating,
                        OriginalTitle   = item.OriginalTitle,
                        ProviderIds     = item.ProviderIds,
                        Studios         = item.Studios.ToList(),
                        People          = LibraryManager.GetItemPeople(item),
                        MediaSources    = sources                       
                        
                    });
            }

            return new ChannelItemResult
            {
                Items = items

            };

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
            return new List<ImageType>() { ImageType.Thumb, ImageType.Backdrop };
        }

        public string Name => Plugin.Instance.Name;
        public string Description => Plugin.Instance.Description;
        public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience; 
        
        public string GetCacheKey(string userId) => Guid.NewGuid().ToString("N");

        public async Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id, CancellationToken cancellationToken)
        {
            var channel = await GetChannelItems(new InternalChannelItemQuery(), cancellationToken);

            var item = channel.Items.FirstOrDefault(i => i.Id == id);
            
            if(item.MediaSources.Count <= 1)
            {
                return item.MediaSources.GetRange(0, 0);
            }            
            
            return item.MediaSources.GetRange(1, item.MediaSources.Count -1);    
        }

        
    }
}
