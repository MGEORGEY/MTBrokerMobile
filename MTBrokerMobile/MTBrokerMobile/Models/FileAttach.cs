using System.IO;

namespace MTBrokerMobile.Models
{
    public class FileAttach
    {
        public int ID { get; set; }

        public string ContentType { get; set; }

        public Stream FileBytes { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileSize { get; set; }
    }
}
