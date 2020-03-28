using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.Business
{
    public class NzbEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public NzbEventArgs(string message)
        {
            this.Message = message;
        }

    }
}
