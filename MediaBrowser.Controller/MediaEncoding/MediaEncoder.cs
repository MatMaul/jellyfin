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
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;

namespace MediaBrowser.Controller.MediaEncoding
{
    public class FFProbeResult
    {
        public class Stream
        {
            public class Disposition {
                [JsonProperty("default")]
                public bool _default { get; set; }
                public bool forced { get; set; }
            }
            public Disposition disposition { get; set; }
            public int index { get; set; }
            public string codec_type { get; set; }
            public string codec_name { get; set; }
            public string codec_long_name { get; set; }
            public string profile { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public int channels { get; set; }
            public string channel_layout { get; set; }
            public Dictionary<string, string> tags { get; set; }
        }
        public class Format
        {
            public string filename { get; set; }
            public string format_long_name { get; set; }
            public string format_name { get; set; }
            public double duration { get; set; }
            public long size { get; set; }
            public int bit_rate { get; set; }

        }
        public IList<Stream> streams { get; set; }
        public Format format { get; set; }
    }

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
            logger.Error("GetInputArgument");
            if (inputFiles.Length <= 0) {
                return null;
            }
            if (protocol == MediaProtocol.File) {
                return "file:" + inputFiles[0];
            }
            return null;
        }

        Task<MediaInfo> IMediaEncoder.GetMediaInfo(MediaInfoRequest request, CancellationToken cancellationToken)
        {
            return Task.Run(function: () => {
                Process p = new Process();
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName   = "ffprobe";
                p.StartInfo.Arguments = "-i file:" + Quote(request.MediaSource.Path);
                p.StartInfo.Arguments += " -threads 0 -v info -print_format json -show_streams -show_chapters -show_format";
                p.Start();
                p.WaitForExit(1000);

                FFProbeResult res = JsonConvert.DeserializeObject<FFProbeResult>(p.StandardOutput.ReadToEnd());

                MediaInfo info = new MediaInfo();

                info.Bitrate = res.format.bit_rate;
                info.Size = res.format.size;
                info.Container = res.format.format_name;

                foreach (FFProbeResult.Stream s in res.streams) {
                    MediaStream stream = new MediaStream();

                    if (s.disposition != null) {
                        stream.IsDefault = s.disposition._default;
                        stream.IsForced = s.disposition.forced;
                    }

                    if ("video".Equals(s.codec_type)) {
                        stream.Type = MediaStreamType.Video;
                        stream.Height = s.height;
                        stream.Width = s.width;
                    } else if ("audio".Equals(s.codec_type)) {
                        stream.Type = MediaStreamType.Audio;
                        stream.Channels = s.channels;
                        stream.ChannelLayout = s.channel_layout;
                    } else if ("subtitle".Equals(s.codec_type)) {
                        stream.Type = MediaStreamType.Subtitle;
                    }

                    stream.Index = s.index;
                    stream.Codec = s.codec_name;
                    stream.Profile = s.profile;
                    if (s.tags.ContainsKey("language"))
                        stream.Language = s.tags["language"];
                    info.MediaStreams.Add(stream);
                }
                return info;
            }, cancellationToken: cancellationToken);
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

        public static string Quote(string arg) {
            StringBuilder sb = new StringBuilder();
            //foreach (string arg in args) {
                int backslashes = 0;

                // Add a space to separate this argument from the others
                if (sb.Length > 0) {
                    sb.Append(" ");
                }

                bool needquote = arg.Length == 0 || arg.Contains(" ") || arg.Contains("\t");
                if (needquote) {
                    sb.Append('"');
                }

                foreach (char c in arg) {
                    if (c == '\\') {
                        // Don't know if we need to double yet.
                        backslashes++;
                    }
                    else if (c == '"') {
                        // Double backslashes.
                        sb.Append(new String('\\', backslashes*2));
                        backslashes = 0;
                        sb.Append("\\\"");
                    } else {
                        // Normal char
                        if (backslashes > 0) {
                            sb.Append(new String('\\', backslashes));
                            backslashes = 0;
                        }
                        sb.Append(c);
                    }
                }

                // Add remaining backslashes, if any.
                if (backslashes > 0) {
                    sb.Append(new String('\\', backslashes));
                }

                if (needquote) {
                    sb.Append(new String('\\', backslashes));
                    sb.Append('"');
                }
            //}
            return sb.ToString();
        }
    }
}