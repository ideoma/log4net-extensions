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
        private MemoryMappedStream _stream;

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
                _stream = new MemoryMappedStream(_mmfile, Appender.MapBufferSize);
                if (append)
                {
                    FastForwardToEnd(_stream);
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
            if (_stream != null)
            {
                var streamPosition = _stream.Position;
                _stream.Dispose();
                _stream = null;
                
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
            return _stream;
        }

        public override void ReleaseLock()
        {
        }

        public MemoryMappedAppender Appender => (MemoryMappedAppender) CurrentAppender;
    }
}