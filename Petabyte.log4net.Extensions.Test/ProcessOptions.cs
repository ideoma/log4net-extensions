using CommandLine;

namespace Petabyte.log4net.Extensions.Test
{
    public class ProcessOptions
    {
        [Option('m', "mappingBufferSize", Required = false)]
        public string MappingBufferSize { get; set; }
        
        [Option('s', "maximumFileSize", Required = false)]
        public string MaximumFileSize { get; set; }
        
        [Option('f', "file", Required = true)]
        public string File { get; set; }
        
        [Option('w', "writeBytes", Required = false)]
        public string WriteBytes { get; set; }
        
        [Option('a', "abort", Required = false)]
        public bool Abort { get; set; }
        
        [Option('t', "text", Required = true)]
        public string Text { get; set; }
        
        [Option('r', "maxSizeRollBackups", Required = true)]
        public int MaxSizeRollBackups { get; set; }
        
        [Option('h', "throw", Required = false, Default = false)]
        public bool Throw { get; set; }
    }
}