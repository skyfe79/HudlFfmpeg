﻿using Hudl.FFmpeg.BaseTypes;
using Hudl.FFmpeg.Resources.BaseTypes;

namespace Hudl.FFmpeg.Resources
{
    [ContainsStream(Type = typeof(AudioStream))]
    [ContainsStream(Type = typeof(VideoStream))]
    public class Txt : BaseContainer
    {
        private const string FileFormat = ".txt";

        public Txt()
            : base(FileFormat)
        {
        }

        protected override IContainer Clone()
        {
            return new Txt();
        }
    }
}