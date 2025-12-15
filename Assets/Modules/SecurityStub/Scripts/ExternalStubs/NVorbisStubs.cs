// Stubs for missing NVorbis library
using System;
using System.IO;

namespace NVorbis
{
    public class VorbisReader : IDisposable
    {
        public VorbisReader(Stream stream, bool closeStreamOnDispose = false) { }
        
        public int Channels => 2;
        public int SampleRate => 44100;
        public long TotalSamples => 0;
        public System.TimeSpan TotalTime =>  System.TimeSpan.Zero;
        
        public int ReadSamples(float[] buffer, int offset, int count) => 0;
        public void SeekTo(long position) { }
        
        public void Dispose() { }
    }
}
