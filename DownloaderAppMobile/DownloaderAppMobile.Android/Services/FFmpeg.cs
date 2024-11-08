﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Com.Arthenica.Ffmpegkit;
using DownloaderAppMobile.Droid;
using DownloaderAppMobile.Models;
using DownloaderAppMobile.Services;

[assembly: Xamarin.Forms.Dependency(typeof(FFmpeg))]
namespace DownloaderAppMobile.Droid
{
    public class FFmpeg : IFFmpeg
    {
        private const string VIDEO_TO_AUDIO =
            "-hide_banner -loglevel error -y -i \"{0}\" -i \"{1}\"" +
            " -map 0:a -c:a libmp3lame -id3v2_version 3 {2}" +
            " -map 1 -metadata:s:v comment=\"Cover (front)\" \"{3}\"";

        private const string METADATA =
            "-metadata artist=\"{0}\"" +
            " -metadata title=\"{1}\"" +
            " -metadata date=\"{2}\"" +
            " -metadata album=\"{3}\"" +
            " -metadata album_artist=\"{4}\"";

        private const string VIDEO = 
            "-hide_banner -loglevel error -y -i \"{0}\" -c copy {1} \"{2}\"";

        private const string MERGE_VIDEO_AUDIO =
            "-hide_banner -loglevel error -y -i \"{0}\" -i \"{1}\" -c copy \"{2}\"";

        public async Task ExecuteAsync(string command, CancellationToken cancellationToken = default)
        {
            FFmpegKitConfig.IgnoreSignal(Signal.Sigxcpu);
            string[] args = FFmpegKitConfig.ParseArguments(command);
            
            using var session = FFmpegSession.Create(args);
            session.AddLog(new Log(session.SessionId, Level.AvLogVerbose, string.Empty));

            await Task.Run
            (
                () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using var asyncFFmpegTask = new AsyncFFmpegExecuteTask(session);
                    asyncFFmpegTask.Run();
                }, cancellationToken
            );

            Console.WriteLine($"{{FFmpeg session '{session.SessionId}': Execution is over > {session.Logs[0].Message}.}}");
        }

        public async Task MergeVideoAndAudioAsync(string videoFilePath, string audioFilePath,
            string destinationFilePath, FFmpegOptions options,
            bool deleteVideo = true, bool deleteAudio = true, CancellationToken cancellationToken = default)
        {
            try
            {
                string command = string.Format(MERGE_VIDEO_AUDIO, 
                    videoFilePath, 
                    audioFilePath, 
                    destinationFilePath
                );
                
                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteVideo)
                {
                    File.Delete(videoFilePath);
                }

                if (deleteAudio)
                {
                    File.Delete(audioFilePath);
                }
            }
        }

        public async Task CreateAudioAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default)
        {
            string tempCoverFilePath = Path.GetTempFileName();

            try
            {
                // create valid front cover for audio
                await File.WriteAllBytesAsync(tempCoverFilePath, options.Thumbnail);

                string command = string.Format(VIDEO_TO_AUDIO, tempVideoFilePath, tempCoverFilePath,
                    string.Format(METADATA, new object[]
                    {
                        options.Author,
                        options.Title,
                        options.Date,
                        options.Album,
                        options.AlbumArtist
                    }), destinationFilePath);

                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteTemp)
                {
                    File.Delete(tempVideoFilePath);
                    File.Delete(tempCoverFilePath);
                }
            }
        }

        public async Task CreateVideoAsync(string destinationFilePath, string tempVideoFilePath,
            FFmpegOptions options, bool deleteTemp = false, CancellationToken cancellationToken = default)
        {
            try
            {
                string command = string.Format(VIDEO, tempVideoFilePath,
                    string.Format(METADATA, new object[]
                    {
                        options.Author,
                        options.Title,
                        options.Date,
                        options.Album,
                        options.AlbumArtist
                    }), destinationFilePath);

                if (!File.Exists(destinationFilePath))
                {
                    File.Create(destinationFilePath).Dispose();
                }

                await ExecuteAsync(command, cancellationToken);
            }
            finally
            {
                if (deleteTemp)
                {
                    File.Delete(tempVideoFilePath);
                }
            }
        }
    }
}