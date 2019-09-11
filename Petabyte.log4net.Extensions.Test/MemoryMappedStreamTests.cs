using System.IO;
using System.Linq;
using System.Text;
using log4net.Petabyte.Extensions;
using NUnit.Framework;

namespace log4net.Tests.Appender
{
    [TestFixture]
    public class MemoryMappedStreamTests
    {
        private static readonly string FileName = "mmstreamtest.log";
        private static readonly string TestString = "abc def asldfkj alskdjfaldfjal;sjf";
        
        [Test]
        public void AppendsFromBeginning()
        {
            AppendStringToNewFile(TestString, FileName, 1024);

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(TestString));
        }
        
        [Test]
        public void AppendRollsOverTwoBuffers()
        {
            int bufferSize = 1024;
            var sb = new StringBuilder();
            while (sb.Length < bufferSize)
            {
                sb.Append(TestString);
            }

            var longString = sb.ToString();
            AppendStringToNewFile(longString, FileName, bufferSize);

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(longString));
        }
        
        [Test]
        public void AppendRollsOverThreeBuffers()
        {
            int bufferSize = 1024;
            var sb = new StringBuilder();
            while (sb.Length < bufferSize * 2)
            {
                sb.Append(TestString);
            }

            var longString = sb.ToString();
            AppendStringToNewFile(longString, FileName, bufferSize);

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(longString));
        }
        
        [Test]
        public void AppendToWriteEndOfExistingFile()
        {
            AppendStringToNewFile(TestString, FileName, 1024);
            using (var mfile = CreateMFile(FileName,false))
            {
                long pos = 0;
                using (var stream = CreateStream(mfile, 1024))
                {
                    stream.SeekAppendEnd();
                    var buffer = Encoding.UTF8.GetBytes(TestString);
                    stream.Write(buffer, 0, buffer.Length);
                    pos = stream.Position;
                }

                mfile.SetLength(pos);
            }

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(TestString + TestString));
        }
        
        
        [Test]
        public void AppendToWriteEndOfExistingFileWhenItsOnTheFirstBufferEdge()
        {
            var buffSize = 1024;
            var testStr = string.Join("", Enumerable.Range(0, buffSize).Select(i => (i % 10).ToString()));
            AppendStringToNewFile(testStr, FileName, buffSize);
            using (var mfile = CreateMFile(FileName,false))
            {
                long pos = 0;
                using (var stream = CreateStream(mfile, buffSize))
                {
                    stream.SeekAppendEnd();
                    var buffer = Encoding.UTF8.GetBytes(TestString);
                    stream.Write(buffer, 0, buffer.Length);
                    pos = stream.Position;
                }

                mfile.SetLength(pos);
            }

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(testStr + TestString));
        }
        
        [Test]
        public void AppendToWriteEndOfExistingFileWhenItsOnTheSecodBufferEdge()
        {
            var buffSize = 256;
            var testStr = string.Join("", Enumerable.Range(0, 2*buffSize).Select(i => (i % 10).ToString()));
            AppendStringToNewFile(testStr, FileName, buffSize);
            using (var mfile = CreateMFile(FileName,false))
            {
                long pos = 0;
                using (var stream = CreateStream(mfile, buffSize))
                {
                    stream.SeekAppendEnd();
                    var buffer = Encoding.UTF8.GetBytes(TestString);
                    stream.Write(buffer, 0, buffer.Length);
                    pos = stream.Position;
                }

                mfile.SetLength(pos);
            }

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(testStr + TestString));
        }
        
        [Test]
        public void AppendWhenBufferSizeIsOdd()
        {
            var testStr = string.Join("", Enumerable.Range(0, 1024).Select(i => (i % 10).ToString()));
            testStr += TestString;
            
            AppendStringToNewFile(testStr, FileName, 325);

            var result = ReadToEnd(FileName);
            Assert.That(result, Is.EqualTo(testStr));
        }

        private void AppendStringToNewFile(string text, string fileName, int buffSize)
        {
            using (var mfile = CreateMFile(fileName))
            {
                long pos = 0;
                using (var stream = CreateStream(mfile, buffSize))
                {
                    var buffer = Encoding.UTF8.GetBytes(text);
                    stream.Write(buffer, 0, buffer.Length);
                    pos = stream.Position;
                }

                mfile.SetLength(pos);
            }
        }


        private string ReadToEnd(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        private MemoryFile CreateMFile(string fileName, bool clear = true)
        {
            if (clear && File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            return new MemoryFile(fileName);
        }

        private MemoryMappedStream CreateStream(MemoryFile file, int buffSize)
        {
            return new MemoryMappedStream(file, buffSize);
        }
    }
}