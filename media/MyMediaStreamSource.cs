using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace pacman
{
    public class MyMediaStreamSource : MediaStreamSource
    {
        private WaveFormatEx _waveFormat;
        private MediaStreamDescription _audioDesc;
        private long _currentPosition;
        private long _startPosition;
        private long _currentTimeStamp;

        private const int SampleRate = 44100;
        private const int ChannelCount = 2;
        private const int BitsPerSample = 16;
        private const int ByteRate = 
            SampleRate * ChannelCount * BitsPerSample / 8;

        private MemoryStream _stream;
        private Random _random = new Random();

        // you only need sample attributes for video
        private Dictionary<MediaSampleAttributeKeys, string> _emptySampleDict =
            new Dictionary<MediaSampleAttributeKeys, string>();


        public MyMediaStreamSource()
        {

            _waveFormat = new WaveFormatEx();
            _waveFormat.BitsPerSample = 16;
            _waveFormat.AvgBytesPerSec = (int)ByteRate;
            _waveFormat.Channels = ChannelCount;
            _waveFormat.BlockAlign = ChannelCount * (BitsPerSample / 8);
            _waveFormat.ext = null; // ??
            _waveFormat.FormatTag = WaveFormatEx.FormatPCM;
            _waveFormat.SamplesPerSec = SampleRate;
            _waveFormat.Size = 0; // must be zero

            _waveFormat.ValidateWaveFormat();

            _stream = new System.IO.MemoryStream();
        }


        protected override void OpenMediaAsync()
        {
            System.Diagnostics.Debug.WriteLine("Started OpenMediaAsync");

            _startPosition = _currentPosition = 0;


            // Init
            Dictionary<MediaStreamAttributeKeys, string> streamAttributes =
                new Dictionary<MediaStreamAttributeKeys, string>();
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes =
                new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams =
                new List<MediaStreamDescription>();

            // Stream Description and WaveFormatEx
            streamAttributes[MediaStreamAttributeKeys.CodecPrivateData] =
                _waveFormat.ToHexString(); // wfx
            MediaStreamDescription msd =
                new MediaStreamDescription(MediaStreamType.Audio,
                                            streamAttributes);
            _audioDesc = msd;

            // next, add the description so that Silverlight will
            // actually request samples for it
            availableStreams.Add(_audioDesc);

            // Tell silverlight we have an endless stream
            sourceAttributes[MediaSourceAttributesKeys.Duration] =
                TimeSpan.FromMinutes(0).Ticks.ToString(
                                    CultureInfo.InvariantCulture);

            // we don't support seeking on our stream
            sourceAttributes[MediaSourceAttributesKeys.CanSeek] =
                false.ToString();

            // tell Silverlight we're done opening our media
            ReportOpenMediaCompleted(sourceAttributes, availableStreams);

            //System.Diagnostics.Debug.WriteLine("Completed OpenMediaAsync");
        }

        protected override void CloseMedia()
        {
            System.Diagnostics.Debug.WriteLine("CloseMedia");
            // Close the stream
            _startPosition = _currentPosition = 0;
            _audioDesc = null;
        }

        protected override void GetDiagnosticAsync(
            MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }

        //private int AlignUp(int a, int b)
        //{
        //    int tmp = a + b - 1;
        //    return tmp - (tmp % b);
        //}

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            int numSamples = ChannelCount * 256;
            int bufferByteCount = BitsPerSample / 8 * numSamples;

            // fill the stream with noise
            for (int i = 0; i < numSamples; i++)
            {
                short sample = (short)_random.Next(
                    short.MinValue, short.MaxValue);

                _stream.Write(BitConverter.GetBytes(sample),
                              0,
                              sizeof(short));
            }


            // Send out the next sample
            MediaStreamSample msSamp = new MediaStreamSample(
                _audioDesc,
                _stream,
                _currentPosition,
                bufferByteCount,
                _currentTimeStamp,
                _emptySampleDict);

            // Move our timestamp and position forward
            _currentTimeStamp += _waveFormat.AudioDurationFromBufferSize(
                                    (uint)bufferByteCount);
            _currentPosition += bufferByteCount;

            ReportGetSampleCompleted(msSamp);
        }

        protected override void SeekAsync(long seekToTime)
        {
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(
            MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }


    }

}
