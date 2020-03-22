using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.System
{
    public static class Environment
    {
        public static DirectoryInfo ProgramDirectory()
        {
            return new FileInfo(Assembly.GetExecutingAssembly().FullName).Directory;
        }
    }
}
