using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Diagnostics;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Reflection;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.System;

namespace MediaBrowser.Controller.MediaEncoding
{
    public class MediaEncoder : IMediaEncoder
    {
        private ILogger logger;
        private IJsonSerializer jsonSerializer;
        private string encoderPath;
        private string probePath;
        private bool hasExternalEncoder;
        private IServerConfigurationManager serverConfigurationManager;
        private IFileSystem fileSystemManager;
        private ILiveTvManager liveTvManager;
        private IIsoManager isoManager;
        private ILibraryManager libraryManager;
        private IChannelManager channelManager;
        private ISessionManager sessionManager;
        private Func<ISubtitleEncoder> p1;
        private Func<IMediaSourceManager> p2;
        private IHttpClient httpClient;
        private IZipClient zipClient;
        private IProcessFactory processFactory;
        private IEnvironmentInfo environmentInfo;
        private IBlurayExaminer blurayExaminer;

        private IServerApplicationHost applicationHost;

        public MediaEncoder(ILogger logger, IJsonSerializer jsonSerializer, string encoderPath, string probePath, bool hasExternalEncoder,
        IServerConfigurationManager serverConfigurationManager, IFileSystem fileSystemManager, ILiveTvManager liveTvManager, IIsoManager isoManager,
        ILibraryManager libraryManager, IChannelManager channelManager, ISessionManager sessionManager, Func<ISubtitleEncoder> p1, Func<IMediaSourceManager> p2,
        IHttpClient httpClient, IZipClient zipClient, IProcessFactory processFactory, IEnvironmentInfo environmentInfo, IBlurayExaminer blurayExaminer, IServerApplicationHost applicationHost)
        {
            this.logger = logger;
            this.jsonSerializer = jsonSerializer;
            this.encoderPath = encoderPath;
            this.probePath = probePath;
            this.hasExternalEncoder = hasExternalEncoder;
            this.serverConfigurationManager = serverConfigurationManager;
            this.fileSystemManager = fileSystemManager;
            this.liveTvManager = liveTvManager;
            this.isoManager = isoManager;
            this.libraryManager = libraryManager;
            this.channelManager = channelManager;
            this.sessionManager = sessionManager;
            this.p1 = p1;
            this.p2 = p2;
            this.httpClient = httpClient;
            this.zipClient = zipClient;
            this.processFactory = processFactory;
            this.environmentInfo = environmentInfo;
            this.blurayExaminer = blurayExaminer;
            this.applicationHost = applicationHost;
        }

        string IMediaEncoder.EncoderLocationType => "Internal";

        string IMediaEncoder.EncoderPath => "/usr/bin/ffmpeg";

        bool ITranscoderSupport.CanEncodeToAudioCodec(string codec)
        {
            return true;
        } 

        bool ITranscoderSupport.CanEncodeToSubtitleCodec(string codec)
        {
            return true;
        }

        bool ITranscoderSupport.CanExtractSubtitles(string codec)
        {
            return true;
        }

        Task IMediaEncoder.ConvertImage(string inputPath, string outputPath)
        {
            throw new NotImplementedException();
        }

        Task<string> IMediaEncoder.EncodeAudio(EncodingJobOptions options, IProgress<double> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IMediaEncoder.EncodeVideo(EncodingJobOptions options, IProgress<double> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
            //EncodingHelper.GetProgressiveVideoFullCommandLine();
        }

        string IMediaEncoder.EscapeSubtitleFilterPath(string path)
        {
            throw new NotImplementedException();
        }

        Task<string> IMediaEncoder.ExtractAudioImage(string path, int? imageStreamIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IMediaEncoder.ExtractVideoImage(string[] inputFiles, string container, MediaProtocol protocol, MediaStream videoStream, Video3DFormat? threedFormat, TimeSpan? offset, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IMediaEncoder.ExtractVideoImage(string[] inputFiles, string container, MediaProtocol protocol, MediaStream imageStream, int? imageStreamIndex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IMediaEncoder.ExtractVideoImagesOnInterval(string[] inputFiles, string container, MediaStream videoStream, MediaProtocol protocol, Video3DFormat? threedFormat, TimeSpan interval, string targetDirectory, string filenamePrefix, int? maxWidth, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        string IMediaEncoder.GetInputArgument(string[] inputFiles, MediaProtocol protocol)
        {
            throw new NotImplementedException();
        }

        Task<MediaInfo> IMediaEncoder.GetMediaInfo(MediaInfoRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        string[] IMediaEncoder.GetPlayableStreamFileNames(string path, VideoType videoType)
        {
            throw new NotImplementedException();
        }

        IEnumerable<string> IMediaEncoder.GetPrimaryPlaylistVobFiles(string path, IIsoMount isoMount, uint? titleNumber)
        {
            throw new NotImplementedException();
        }

        string IMediaEncoder.GetTimeParameter(long ticks)
        {
            throw new NotImplementedException();
        }

        void IMediaEncoder.Init()
        {
            //throw new NotImplementedException();
        }

        bool IMediaEncoder.SupportsDecoder(string decoder)
        {
            return true;
        }

        bool IMediaEncoder.SupportsEncoder(string encoder)
        {
            return true;
        }

        void IMediaEncoder.UpdateEncoderPath(string path, string pathType)
        {
        }

        public Task<Stream> GetSubtitles(BaseItem item, string mediaSourceId, int subtitleStreamIndex, string outputFormat, long startTimeTicks, long endTimeTicks, bool preserveOriginalTimestamps, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSubtitleFileCharacterSet(string path, string language, MediaProtocol protocol, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}