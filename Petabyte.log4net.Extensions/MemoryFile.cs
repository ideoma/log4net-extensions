using System;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace log4net.Petabyte.Extensions
{
    public class MemoryFile : IDisposable
    {
        public static readonly int CreateFileRetries = 3;
        private readonly string _fullName;
        private FileStream _fileOpened;

        public MemoryFile(string fileName)
        {
            _fullName = Path.GetFullPath(fileName);
        }

        private void CreateFileSize(int retry)
        {
            try
            {
                _fileOpened = File.Open(_fullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch (IOException)
            {
                if (retry > 0)
                {
                    CreateFileSize(retry - 1);
                    return;
                }

                throw;
            }
        }

        public IRawBinaryWriter CreateViewAccessor(long offset, long size)
        {
            if (_fileOpened == null)
            {
                CreateFileSize(CreateFileRetries);
            }

            return MapFile(offset, size, FileAccess.ReadWrite, FileShare.Read);
        }

        private IRawBinaryWriter MapFile(long offset, long size, FileAccess fileAccess, FileShare fileShare)
        {
            try
            {
                return MapFileCore(offset, size);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(
                    "Failed to open file '{0}' with mode '{1}', access '{2}' and sharing '{3}'. Error {4}",
                    _fullName, FileMode.Open, fileAccess, fileShare, ex);
                throw;
            }
        }

        public void SetLength(long length)
        {
            try
            {
                _fileOpened.SetLength(length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IRawBinaryWriter MapFileCore(long offset, long size)
        {
            long targetFileSize = Math.Max(offset + size, _fileOpened.Length);
            var mmAccess = MemoryMappedFileAccess.ReadWrite;

            using (var rmmf1 = MemoryMappedFile.CreateFromFile(_fileOpened, null, targetFileSize,
                mmAccess, HandleInheritability.None, true))
            {
                return new AccessorBinaryWriter(
                    rmmf1.CreateViewAccessor(offset, size, mmAccess),
                    offset,
                    size);
            }
        }

        public long Length => _fileOpened.Length;

        public void Dispose()
        {
            _fileOpened?.Dispose();
            _fileOpened = null;
        }
    }
}