﻿using System;
using Hudl.FFmpeg.Common;
using Hudl.FFmpeg.Settings.BaseTypes;
using Hudl.FFmpeg.Settings;
using Xunit;

namespace Hudl.FFmpeg.Tests.Setting
{
    public class SettingCollectionTests
    {
        [Fact]
        public void SettingsCollection_ForInput_Valid()
        {
            var settingsCollection = SettingsCollection.ForInput(new StartAt(1));

            Assert.True(settingsCollection.Count == 1);
        }

        [Fact]
        public void SettingsCollection_ForInput_Invalid()
        {
            Assert.Throws<ArgumentException>(() => SettingsCollection.ForInput(new OverwriteOutput())); 
        }

        [Fact]
        public void SettingsCollection_ForOutput_Valid()
        {
            var settingsCollection = SettingsCollection.ForOutput(new OverwriteOutput());

            Assert.True(settingsCollection.Count == 1);
        }

        [Fact]
        public void SettingsCollection_ForOutput_Invalid()
        {
            Assert.Throws<ArgumentException>(() => SettingsCollection.ForOutput(new StartAt(1))); 
        }

        [Fact]
        public void SettingsCollection_AllowMultiple()
        {
            var settingsCollectionO = SettingsCollection.ForOutput();

            Assert.DoesNotThrow(() => settingsCollectionO.Add(new Map("test1")));

            Assert.DoesNotThrow(() => settingsCollectionO.Add(new Map("test2")));

            Assert.True(settingsCollectionO.Count == 2);
        }

        [Fact]
        public void SettingsCollection_Add()
        {
            var settingsCollectionI = SettingsCollection.ForInput();
            var settingsCollectionO = SettingsCollection.ForOutput();
            
            Assert.Throws<ArgumentException>(() => settingsCollectionI.Add(new OverwriteOutput())); 

            Assert.Throws<ArgumentException>(() => settingsCollectionO.Add(new StartAt(1))); 

            Assert.DoesNotThrow(() => settingsCollectionI.Add(new StartAt(1)));

            Assert.DoesNotThrow(() => settingsCollectionO.Add(new OverwriteOutput()));

            Assert.True(settingsCollectionI.Count == 1);

            Assert.True(settingsCollectionO.Count == 1);
        }

        [Fact]
        public void SettingsCollection_AddRange()
        {
            var settingsCollectionI = SettingsCollection.ForInput();
            var settingsCollectionO = SettingsCollection.ForOutput();
            var settingsCollectionAddI = SettingsCollection.ForInput(new StartAt(1), new DurationInput(1));
            var settingsCollectionAddO = SettingsCollection.ForOutput(new OverwriteOutput(), new RemoveAudio());
            
            Assert.Throws<ArgumentNullException>(() => settingsCollectionI.AddRange(null)); 
            Assert.Throws<ArgumentNullException>(() => settingsCollectionO.AddRange(null)); 

            Assert.DoesNotThrow(() => settingsCollectionI.AddRange(settingsCollectionAddI));
            Assert.DoesNotThrow(() => settingsCollectionO.AddRange(settingsCollectionAddO));

            Assert.Throws<ArgumentException>(() => settingsCollectionI.AddRange(settingsCollectionAddO)); 
            Assert.Throws<ArgumentException>(() => settingsCollectionO.AddRange(settingsCollectionAddI)); 

            Assert.Throws<ArgumentException>(() => settingsCollectionI.AddRange(settingsCollectionAddI)); 
            Assert.Throws<ArgumentException>(() => settingsCollectionO.AddRange(settingsCollectionAddO)); 

            Assert.True(settingsCollectionI.Count == 2);
            Assert.True(settingsCollectionO.Count == 2);
        }

        [Fact]
        public void SettingsCollection_Merge()
        {
            var settingsCollectionI = SettingsCollection.ForInput(new StartAt(1));
            var settingsCollectionO = SettingsCollection.ForOutput(new CodecVideo(VideoCodecType.Libx264));
            var startAtDefault = new StartAt(2);
            var vcodecDefault = new CodecVideo(VideoCodecType.Copy);

            Assert.DoesNotThrow(() => settingsCollectionI.Merge(startAtDefault, FFmpegMergeOptionType.OldWins));

            var startAtSetting = settingsCollectionI.Items[0] as StartAt; 
            Assert.False(startAtSetting != null && startAtSetting.Length == startAtDefault.Length);
            
            Assert.DoesNotThrow(() => settingsCollectionI.Merge(startAtDefault, FFmpegMergeOptionType.NewWins));

            startAtSetting = settingsCollectionI.Items[0] as StartAt; 
            Assert.True(startAtSetting != null && startAtSetting.Length == startAtDefault.Length);

            Assert.Throws<ArgumentException>(() => settingsCollectionI.Merge(vcodecDefault, FFmpegMergeOptionType.OldWins));
            Assert.Throws<ArgumentException>(() => settingsCollectionI.Merge(vcodecDefault, FFmpegMergeOptionType.NewWins));


            Assert.DoesNotThrow(() => settingsCollectionO.Merge(vcodecDefault, FFmpegMergeOptionType.OldWins));

            var vcodecSetting = settingsCollectionO.Items[0] as CodecVideo; 
            Assert.False(vcodecSetting != null && vcodecSetting.Codec == vcodecDefault.Codec);
            
            Assert.DoesNotThrow(() => settingsCollectionO.Merge(vcodecDefault, FFmpegMergeOptionType.NewWins));

            vcodecSetting = settingsCollectionO.Items[0] as CodecVideo; 
            Assert.True(vcodecSetting != null && vcodecSetting.Codec == vcodecDefault.Codec);

            Assert.Throws<ArgumentException>(() => settingsCollectionO.Merge(startAtDefault, FFmpegMergeOptionType.OldWins));
            Assert.Throws<ArgumentException>(() => settingsCollectionO.Merge(startAtDefault, FFmpegMergeOptionType.NewWins));
        }

        [Fact]
        public void SettingsCollection_MergeRange()
        {
            var startAtDefault = new StartAt(2);
            var vcodecDefault = new CodecVideo(VideoCodecType.Copy);
            var settingsCollectionI = SettingsCollection.ForInput(new StartAt(1));
            var settingsCollectionO = SettingsCollection.ForOutput(new CodecVideo(VideoCodecType.Libx264));
            var settingsCollectionMergeI =  SettingsCollection.ForInput(startAtDefault);
            var settingsCollectionMergeO = SettingsCollection.ForOutput(vcodecDefault);

            
            Assert.Throws<ArgumentException>(() => settingsCollectionI.MergeRange(settingsCollectionMergeO, FFmpegMergeOptionType.OldWins));
            Assert.Throws<ArgumentException>(() => settingsCollectionO.MergeRange(settingsCollectionMergeI, FFmpegMergeOptionType.OldWins));
            Assert.Throws<ArgumentException>(() => settingsCollectionI.MergeRange(settingsCollectionMergeO, FFmpegMergeOptionType.NewWins));
            Assert.Throws<ArgumentException>(() => settingsCollectionO.MergeRange(settingsCollectionMergeI, FFmpegMergeOptionType.NewWins));

            Assert.DoesNotThrow(() => settingsCollectionI.MergeRange(settingsCollectionMergeI, FFmpegMergeOptionType.OldWins));
            Assert.DoesNotThrow(() => settingsCollectionO.MergeRange(settingsCollectionMergeO, FFmpegMergeOptionType.OldWins));

            var startAtSetting = settingsCollectionI.Items[0] as StartAt;
            var vcodecSetting = settingsCollectionO.Items[0] as CodecVideo;
            Assert.False(startAtSetting != null && startAtSetting.Length == startAtDefault.Length);
            Assert.False(vcodecSetting != null && vcodecSetting.Codec == vcodecDefault.Codec);
            
            Assert.DoesNotThrow(() => settingsCollectionI.MergeRange(settingsCollectionMergeI, FFmpegMergeOptionType.NewWins));
            Assert.DoesNotThrow(() => settingsCollectionO.MergeRange(settingsCollectionMergeO, FFmpegMergeOptionType.NewWins));

            startAtSetting = settingsCollectionI.Items[0] as StartAt;
            vcodecSetting = settingsCollectionO.Items[0] as CodecVideo;
            Assert.True(startAtSetting != null && startAtSetting.Length == startAtDefault.Length);
            Assert.True(vcodecSetting != null && vcodecSetting.Codec == vcodecDefault.Codec);
        }

        [Fact]
        public void SettingsCollection_Remove()
        {
            var settingsCollectionI = SettingsCollection.ForInput(new StartAt(1), new DurationInput(2));
            var settingsCollectionO = SettingsCollection.ForOutput(new RemoveAudio(), new OverwriteOutput());

            Assert.True(settingsCollectionI.Count == 2);
            Assert.True(settingsCollectionO.Count == 2);

            settingsCollectionI.Remove<StartAt>();
            settingsCollectionO.Remove<StartAt>();
            settingsCollectionI.Remove<RemoveAudio>();
            settingsCollectionO.Remove<RemoveAudio>();

            Assert.True(settingsCollectionI.Count == 1);
            Assert.True(settingsCollectionO.Count == 1);
        }

        [Fact]
        public void SettingsCollection_RemoveAt()
        {
            var settingsCollectionI = SettingsCollection.ForInput(new StartAt(1), new DurationInput(2));
            var settingsCollectionO = SettingsCollection.ForOutput(new RemoveAudio(), new OverwriteOutput());

            Assert.True(settingsCollectionI.Count == 2);
            Assert.True(settingsCollectionO.Count == 2);
            
            settingsCollectionI.RemoveAt(0);
            settingsCollectionO.RemoveAt(0);

            Assert.True(settingsCollectionI.Count == 1);
            Assert.True(settingsCollectionO.Count == 1);
        }

        [Fact]
        public void SettingsCollection_RemoveAll()
        {
            var settingsCollectionI = SettingsCollection.ForInput(new StartAt(1), new DurationInput(2));
            var settingsCollectionO = SettingsCollection.ForOutput(new RemoveAudio(), new OverwriteOutput());

            Assert.True(settingsCollectionI.Count == 2);
            Assert.True(settingsCollectionO.Count == 2);
            
            settingsCollectionI.RemoveAll(s => true);
            settingsCollectionO.RemoveAll(s => true);

            Assert.True(settingsCollectionI.Count == 0);
            Assert.True(settingsCollectionO.Count == 0);
        }
    }
}
