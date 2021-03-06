﻿using System;
using System.Runtime.InteropServices;
using Hudl.FFmpeg.Common;
using Hudl.FFmpeg.Filters.Templates;
using Hudl.FFmpeg.Resources;
using Hudl.FFmpeg.Resources.BaseTypes;
using Hudl.FFmpeg.Settings.BaseTypes;
using Hudl.FFmpeg.Sugar;
using Hudl.FFmpeg.Command;
using Hudl.FFmpeg.Settings;
using Xunit;

namespace Hudl.FFmpeg.Tests.Render
{
    public class RenderTests
    {
        [Fact]
        public void RenderVideoWEffects()
        {
#if DEBUG
            ResourceManagement.CommandConfiguration  = CommandConfiguration.Create(
                "c:/source/ffmpeg/bin/temp",
                "c:/source/ffmpeg/bin/ffmpeg.exe",
                "c:/source/ffmpeg/bin/ffprobe.exe");

            var outputSettings = SettingsCollection.ForOutput(
                new OverwriteOutput(),
                new CodecVideo(VideoCodecType.Libx264),
                new BitRateVideo(3000),
                new BitRateTolerance(3000),
                new FrameRate(30));

            var factory = CommandFactory.Create();

            factory.CreateOutputCommand()
                   .WithInput<VideoStream>(Assets.Utilities.GetVideoFile())
                   .WithInput<VideoStream>(Assets.Utilities.GetVideoFile())
                   .Filter(new CrossfadeConcatenate(1))
                   .MapTo<Mp4>("c:/source/ffmpeg/bin/temp/output-test.mp4", outputSettings);

            factory.Render();
#endif
        }
    }
}
