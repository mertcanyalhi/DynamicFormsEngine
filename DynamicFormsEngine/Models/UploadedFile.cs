using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicFormsEngine.Models
{
    [Serializable]
    public class UploadedFile
    {
        public static List<UploadedFile> Files = new List<UploadedFile>();

        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }

        public byte[] File { get; set; }
    }
}
