using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Diagnostics;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Text;

namespace MediaBrowser.Controller.MediaEncoding
{
    public class SubtitleEncoder : ISubtitleEncoder
    {
        private ILibraryManager libraryManager;
        private ILogger logger;
        private IServerApplicationPaths applicationPaths;
        private IFileSystem fileSystemManager;
        private IMediaEncoder mediaEncoder;
        private IJsonSerializer jsonSerializer;
        private IHttpClient httpClient;
        private IMediaSourceManager mediaSourceManager;
        private IProcessFactory processFactory;
        private ITextEncoding textEncoding;

        public SubtitleEncoder(ILibraryManager libraryManager, ILogger logger, IServerApplicationPaths applicationPaths, IFileSystem fileSystemManager, IMediaEncoder mediaEncoder, IJsonSerializer jsonSerializer, IHttpClient httpClient, IMediaSourceManager mediaSourceManager, IProcessFactory processFactory, ITextEncoding textEncoding)
        {
            this.libraryManager = libraryManager;
            this.logger = logger;
            this.applicationPaths = applicationPaths;
            this.fileSystemManager = fileSystemManager;
            this.mediaEncoder = mediaEncoder;
            this.jsonSerializer = jsonSerializer;
            this.httpClient = httpClient;
            this.mediaSourceManager = mediaSourceManager;
            this.processFactory = processFactory;
            this.textEncoding = textEncoding;
        }

        public Task<string> GetSubtitleFileCharacterSet(string path, string language, MediaProtocol protocol, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Stream> GetSubtitles(BaseItem item, string mediaSourceId, int subtitleStreamIndex, string outputFormat, long startTimeTicks, long endTimeTicks, bool preserveOriginalTimestamps, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

    }
}