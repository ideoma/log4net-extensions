using System;
using System.IO;
using System.Text;
using log4net.Appender;

namespace log4net.Petabyte.Extensions
{
    public class MemoryMapLockingModel: FileAppender.LockingModelBase
    {
        private string _mmfilename;
        private MemoryFile _mmfile;
        internal MemoryMappedStream Stream { get; private set; }

        public override void OpenFile(string filename, bool append, Encoding encoding)
        {
            try
            {
                _mmfilename = filename;
                if (!append && File.Exists(filename))
                {
                    File.Delete(filename);
                }
            
                _mmfile = new MemoryFile(_mmfilename);
                Stream = new MemoryMappedStream(_mmfile, Appender.MapBufferSize);
                if (append)
                {
                    FastForwardToEnd(Stream);
                }
            }
            catch (Exception ex)
            {
                CurrentAppender.ErrorHandler.Error("Unable to acquire lock on file " + filename + ". " + ex.Message);
            }
        }

        private void FastForwardToEnd(MemoryMappedStream stream)
        {
            stream.SeekAppendEnd();
        }

        public override void CloseFile()
        {
            if (Stream != null)
            {
                var streamPosition = Stream.Position;
                Stream.Dispose();
                Stream = null;
                
                if (_mmfile != null)
                {
                    _mmfile.SetLength(streamPosition);
                    _mmfile.Dispose();
                    _mmfile = null;
                }
            }
        }

        public override void ActivateOptions()
        {
        }

        public override void OnClose()
        {
        }

        public override Stream AcquireLock()
        {
            return Stream;
        }

        public override void ReleaseLock()
        {
        }

        public MemoryMappedAppender Appender => (MemoryMappedAppender) CurrentAppender;
    }
}