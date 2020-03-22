using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.DataModel
{
    public class NzbDocumentWrapper
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public IEnumerable<byte> Buffer { get; private set; }

        public NzbDocument Document { get; private set; }
        public NzbDocumentWrapper(string name, string path, IEnumerable<byte> buffer, NzbDocument document)
        {
            this.Path = path;
            this.Document = document;
            this.Buffer = buffer;
            this.Name = name;
        }

    }
}
