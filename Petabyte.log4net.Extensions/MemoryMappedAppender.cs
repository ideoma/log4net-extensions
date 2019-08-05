using System;
using System.Globalization;
using System.IO;
using log4net.Appender;
using log4net.Petabyte.Extensions;
using log4net.Util;

namespace log4net.Petabyte
{
    public class MemoryMappedAppender : RollingFileAppender
    {
        public override void ActivateOptions()
        {
            base.LockingModel = new MemoryMapLockingModel();
            
            // Buffer sizes under 32k are inefficient. Replace them with a bigger default size (16Mb).
            if (MapBufferSize < 32 * 1024 || MapBufferSize > MaxFileSize)
            {
                MapBufferSize = (int)Math.Min(16 * 1025 * 1024, MaxFileSize);
            }
            base.ActivateOptions();
        }
        
        public new LockingModelBase LockingModel
        {
            get => base.LockingModel;
            // ReSharper disable once ValueParameterNotUsed
            set { }
        }

        public int MapBufferSize { get; set; }

        public string MappingBufferSize
        {
            get => MapBufferSize.ToString(NumberFormatInfo.InvariantInfo);
            set => MapBufferSize = (int)OptionConverter.ToFileSize(value, 32 * 1024 * 1024);
        }
        protected override void OpenFile(string fileName, bool append)
        {
            lock (this)
            {
                base.OpenFile(fileName, append);
                ((CountingQuietTextWriter) QuietWriter).Count = ((MemoryMapLockingModel)LockingModel).Stream.Length;
            }
        }
    }
}