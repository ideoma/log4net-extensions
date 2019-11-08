using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;

namespace log4net.Petabyte.Extensions
{
    public class AccessorBinaryWriter : IRawBinaryWriter
    {
        private readonly long _bufferOffset;
        private readonly MemoryMappedViewAccessor _view;
        private unsafe byte* _memoryPtr;

        private static readonly int ALLOCATION_GRANULARITY = AccessorHelper.DwAllocationGranularity;
        
        public  AccessorBinaryWriter(MemoryMappedViewAccessor view, long bufferOffset,
            long bufferSize)
        {
            if (bufferOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferOffset),"bufferOffset must be positive");
            }

            _view = view;
            _bufferOffset = bufferOffset;
            BufferSize = bufferSize;
            ResolveMemoryPtr(view);
        }
        
        private void ResolveMemoryPtr(MemoryMappedViewAccessor view)
        {
            unsafe
            {
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref _memoryPtr);
                _memoryPtr += view.PointerOffset;
            }
        }
        
        public long BufferSize { get; }

        public long BufferOffset => _bufferOffset;

        public void WriteBytes(long offset, byte[] array, int arrayOffset, int sizeBytes)
        {
            unsafe
            {
                var writePtr = _memoryPtr + offset;
                fixed (byte* dest = array)
                {
                    AccessorHelper.Memcpy(writePtr, dest + arrayOffset, sizeBytes);
                }
            }
        }

        public void ReadBytes(long offset, byte[] array, int arrayOffset, int sizeBytes)
        {
            unsafe
            {
                var readPtr = _memoryPtr + offset;
                fixed (byte* dest = array)
                {
                    AccessorHelper.Memcpy(dest + arrayOffset, readPtr, sizeBytes);
                }
            }
        }

        public void Flush()
        {
            _view.Flush();
        }

        public unsafe long FindInt(int i)
        {
            var readPtr = _memoryPtr;
            var endAddr = _memoryPtr + BufferSize - sizeof(int);
            while (readPtr < endAddr)
            {
                int read = ((int*) readPtr)[0];
                if (read == i) return readPtr - _memoryPtr + _bufferOffset;
                readPtr += 1;
            }

            return BufferSize + 1;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposed)
        {
            unsafe
            {
                if (_memoryPtr != null)
                {
                    _memoryPtr = null;
                    _view.SafeMemoryMappedViewHandle.ReleasePointer();
                    _view.SafeMemoryMappedViewHandle.Dispose();
                    _view.Dispose();
                }

                if (disposed)
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        ~AccessorBinaryWriter()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error in finalization thread." + ex);
            }
        }
    }
}