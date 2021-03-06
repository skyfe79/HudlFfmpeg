﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hudl.FFmpeg.BaseTypes;
using Hudl.FFmpeg.Common;
using Hudl.FFmpeg.Filters.BaseTypes;
using Hudl.FFmpeg.Metadata;
using Hudl.FFmpeg.Metadata.BaseTypes;
using Hudl.FFmpeg.Resources.BaseTypes;

namespace Hudl.FFmpeg.Filters
{
    /// <summary>
    /// Filter that mixes multiple audio signals into a single audio source 
    /// </summary>
    [ForStream(Type = typeof(AudioStream))]
    public class AMix : BaseFilter, IMetadataManipulation
    {
        private const int FilterMaxInputs = 4;
        private const string FilterType = "amix";

        public AMix() 
            : base(FilterType, FilterMaxInputs)
        {
        }
        public AMix(int? inputs, double? dropoutTransition, DurationType duration)
            : this()
        {
            Inputs = inputs;
            Duration = duration; 
            DropoutTransition = dropoutTransition;
        }

        public int? Inputs { get; set; }
        
        public double? DropoutTransition { get; set; }

        public DurationType Duration { get; set; }

        public override void Validate()
        {
            if (Inputs.HasValue && Inputs < 2)
            {
                throw new InvalidOperationException("Number of inputs cannot be less than defualt of 2");
            }
            if (DropoutTransition.HasValue && DropoutTransition <= 0)
            {
                throw new InvalidOperationException("Dropout transition cannot be less than 0");
            }
        }

        public override string ToString() 
        {
            var filterParameters = new StringBuilder(100);

            if (Inputs.HasValue)
            {
                FilterUtility.ConcatenateParameter(filterParameters, "inputs", Inputs.GetValueOrDefault());
            }

            if (Duration != DurationType.Longest)
            {
                FilterUtility.ConcatenateParameter(filterParameters, "duration", Formats.EnumValue(Duration));
            }

            if (DropoutTransition.HasValue)
            {
                FilterUtility.ConcatenateParameter(filterParameters, "dropout_transition", DropoutTransition.GetValueOrDefault());
            }

            return FilterUtility.JoinTypeAndParameters(this, filterParameters);
        }

        public MetadataInfoTreeContainer EditInfo(MetadataInfoTreeContainer infoToUpdate, List<MetadataInfoTreeContainer> suppliedInfo)
        {
            switch (Duration)
            {
                case DurationType.First:
                    return suppliedInfo.FirstOrDefault();
                case DurationType.Shortest:
                    return suppliedInfo.OrderBy(r => r.AudioStream.Duration).FirstOrDefault();
                default:
                    return suppliedInfo.OrderByDescending(r => r.AudioStream.Duration).FirstOrDefault();
            }
        }
    }
}
