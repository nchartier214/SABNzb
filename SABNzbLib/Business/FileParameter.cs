using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.Business
{
    public class FileParameter
    {
        public IEnumerable<byte> File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public FileParameter(IEnumerable<byte> file) : this(file, null) { }
        public FileParameter(IEnumerable<byte> file, string filename) : this(file, filename, null) { }
        public FileParameter(IEnumerable<byte> file, string filename, string contenttype)
        {
            this.File = file;   
            this.FileName = filename;
            this.ContentType = contenttype;
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
