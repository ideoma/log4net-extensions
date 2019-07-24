using System;
using System.Text;
using log4net.Core;

namespace Petabyte.log4net.Extensions.Test
{
    public class SilentErrorHandler : IErrorHandler
    {
        private StringBuilder m_buffer = new StringBuilder();

        public string Message
        {
            get { return m_buffer.ToString(); }
        }

        public void Error(string message)
        {
            m_buffer.Append(message + "\n");
        }

        public void Error(string message, Exception e)
        {
            m_buffer.Append(message + "\n" + e.Message + "\n");
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            m_buffer.Append(message + "\n" + e.Message + "\n");
        }
    }
}