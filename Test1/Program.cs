using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nzb;

namespace Test1
{
    class Program
    {
        public static object Nzbdocument { get; private set; }

        static void Main(string[] args)
        {
            var key = @"C:\Users\nicol\source\repos\SABNzb\SABNzbPut\521f0a06fb072ef9c1e7cd87.nzb";
            var contents = File.ReadAllBytes(key);
            var nzb = GetNzbDocument(contents);

            var retour = GetKeyName(key, nzb);
        }
        private static NzbDocument GetNzbDocument(byte[] contents)
        {
            NzbDocument retour = null;
            using (var file = new BufferedStream(new MemoryStream(contents)))
            {
                retour = NzbDocument.Load(file).Result;
            }

            return retour;
        }

        private static string GetKeyName(string nzbFullPath, NzbDocument document)
        {
            var retour = Path.Combine(Path.GetFileNameWithoutExtension(nzbFullPath), ".nzb");
            if (document.Files.Any())
            {
                var nzb = document.Files.First();
                var pattern = @"""(.*?)\.\w+""";
                var match = Regex.Match(nzb.Subject, pattern);
                if (match.Success)
                    retour = match.Groups[1].Value;

                return retour;
            }

            return retour;
        }


    }
}
