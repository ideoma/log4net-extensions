using System;
using System.IO;

namespace log4net.Petabyte.Extensions
{
    public class MemoryMappedStream : Stream
    {
        private readonly MemoryFile _file;
        private long _currentOffset;
        private IRawBinaryWriter _currentView;
        private int _bufferSize;

        public MemoryMappedStream(MemoryFile file, int bufferSize)
        {
            _file = file;
            _bufferSize = bufferSize;
        }

        public override void Flush()
        {
            // _currentView?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int initialCount = count;
            while (count > 0 && EnsureViewMapped(_currentOffset, false))
            {
                var readCount = (int) Math.Min(count, _currentView.BufferOffset + _currentView.BufferSize);
                _currentView.ReadBytes(_currentOffset - _currentView.BufferOffset, buffer, offset, readCount);
                _currentOffset += readCount;
                offset += readCount;
                count -= readCount;
            }

            return initialCount - count;
        }

        public void SeekAppendEnd()
        {
            _currentOffset = 0L;
            while (EnsureViewMapped(_currentOffset, _currentOffset == 0 || _currentOffset < _file.Length))
            {
                var offset = _currentView.FindInt(0);
                if (offset < _currentView.BufferSize)
                {
                    _currentOffset = _currentView.BufferOffset + offset;
                    break;
                }

                _currentOffset += _currentView.BufferSize;
            }
        }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return _currentOffset = offset;
                case SeekOrigin.Current:
                    return _currentOffset += offset;
                case SeekOrigin.End:
                    return _currentOffset = _file.Length - offset;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }
        }

        public override void SetLength(long value)
        {
            if (value <= _currentOffset)
            {
                _currentOffset = value - 1;
            }

            if (_currentView != null && _currentView.BufferOffset + _currentView.BufferSize >= value)
            {
                _currentView.Dispose();
                _currentView = null;
            }

            _file.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                EnsureViewMapped(_currentOffset, true);
                var writeCount = (int) Math.Min(count, _currentView.BufferSize - (_currentOffset - _currentView.BufferOffset));
                _currentView.WriteBytes(_currentOffset - _currentView.BufferOffset, buffer, offset, writeCount);
                _currentOffset += writeCount;
                offset += writeCount;
                count -= writeCount;
            }
        }

        private bool EnsureViewMapped(long writeOffset, bool allowFileAppend)
        {
            if (_currentView != null)
            {
                if (_currentView.BufferOffset <= writeOffset &&
                    writeOffset < _currentView.BufferOffset + _currentView.BufferSize)
                {
                    // Do nothing, view is good.
                    return true;
                }

                _currentView.Dispose();
                if (allowFileAppend || _file.Length > writeOffset)
                {
                    _currentView = MapView(writeOffset);
                    return true;
                }

                return false;
            }

            if (allowFileAppend || _file.Length > writeOffset)
            {
                _currentView = MapView(writeOffset);
                return true;
            }

            return false;
        }

        private IRawBinaryWriter MapView(long writeOffset)
        {
            var viewOffset = _bufferSize * (writeOffset / _bufferSize);
            return _file.CreateViewAccessor(viewOffset, _bufferSize);
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _currentOffset;

        public override long Position
        {
            get => _currentOffset;
            set => _currentOffset = value;
        }

        protected override void Dispose(bool disposing)
        {
            _currentView?.Dispose();
            _currentView = null;
        }
    }
}