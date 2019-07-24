using System;

namespace log4net.Petabyte.Extensions
{
    public interface IRawBinaryWriter : IDisposable
    {
        long BufferSize { get; }
        long BufferOffset { get; }
        void WriteBytes(long offset, byte[] array, int arrayOffset, int sizeBytes);
        void ReadBytes(long offset, byte[] array, int arrayOffset, int sizeBytes);
        void Flush();
        long FindInt(int i);
    }
}