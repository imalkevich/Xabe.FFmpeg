﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xabe.FFmpeg.Streams.SubtitleStream;

namespace Xabe.FFmpeg
{
    /// <inheritdoc />
    public class SubtitleStream : ISubtitleStream
    {
        private readonly List<ConversionParameter> _parameters = new List<ConversionParameter>();
        private string _language;
        private string _codec;

        /// <inheritdoc />
        public string Codec { get; internal set; }

        /// <inheritdoc />
        public string Path { get; internal set; }

        /// <inheritdoc />
        public string Build()
        {
            var builder = new StringBuilder();
            builder.Append(BuildLanguage());
            builder.Append(BuildSubtitleCodec());
            return builder.ToString();
        }

        internal SubtitleStream()
        {

        }

        /// <inheritdoc />
        public string BuildParameters(ParameterPosition forPosition)
        {
            IEnumerable<ConversionParameter> parameters = _parameters?.Where(x => x.Position == forPosition);
            if (parameters != null &&
                parameters.Any())
            {
                return string.Join(string.Empty, parameters.Select(x => x.Parameter));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public int Index { get; internal set; }

        /// <inheritdoc />
        public string Language { get; internal set; }

        /// <inheritdoc />
        public int? Default { get; internal set; }

        /// <inheritdoc />
        public int? Forced { get; internal set; }

        /// <inheritdoc />
        public string Title { get; internal set; }

        /// <inheritdoc />
        public StreamType StreamType => StreamType.Subtitle;

        /// <inheritdoc />
        public string BuildSubtitleCodec()
        {
            if (_codec != null)
                return $"-c:s {_codec.ToString()} ";
            else
                return string.Empty;
        }

        /// <inheritdoc />
        public ISubtitleStream SetLanguage(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                _language = lang;
            }
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetSource()
        {
            return new[] { Path };
        }

        private string BuildLanguage()
        {
            string language = !string.IsNullOrEmpty(_language) ? _language : Language;
            if (!string.IsNullOrEmpty(language))
            {
                language = $"-metadata:s:s:{Index} language={language} ";
            }
            return language;
        }

        /// <inheritdoc />
        public ISubtitleStream SetCodec(SubtitleCodec codec)
        {
            string input = codec.ToString();

            return SetCodec($"{input}");
        }

        /// <inheritdoc />
        public ISubtitleStream SetCodec(string codec)
        {
            _codec = codec;
            return this;
        }

        /// <inheritdoc />
        public ISubtitleStream UseNativeInputRead(bool readInputAtNativeFrameRate)
        {
            _parameters.Add(new ConversionParameter($"-re", ParameterPosition.PreInput));
            return this;
        }

        /// <inheritdoc />
        public ISubtitleStream SetStreamLoop(int loopCount)
        {
            _parameters.Add(new ConversionParameter($"-stream_loop {loopCount}", ParameterPosition.PreInput));
            return this;
        }
    }
}
