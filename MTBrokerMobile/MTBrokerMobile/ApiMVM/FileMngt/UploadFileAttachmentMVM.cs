namespace MTBrokerMobile.ApiMVM.FileMngt
{
    public class UploadFileAttachmentMVM
    {
        public int ID { get; set; }

        public string ContentType { get; set; }

        public string FileExtension { get; set; }

        public string FileName { get; set; }

        public string FileUri { get; set; }


        public ulong FileSize { get; set; }


        public string FileSizeReadable { get; set; }
    }
}
