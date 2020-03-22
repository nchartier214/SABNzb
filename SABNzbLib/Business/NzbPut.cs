using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Nzb;
using Nzb.DataModel;

namespace Nzb.Business
{
    public class NzbPut
    {
        public void Do(IEnumerable< string> nzbFullPaths)
        {
            foreach (var nzbFullPath in nzbFullPaths)
                this.Do(nzbFullPath);
        }

        public void Do(string nzbFullPath)
        {
            // ""C:\Program Files\SABnzbd\SABnzbd.exe"" ""C:\Users\nicol\AppData\Local\Temp\GOlL1GMv2TdxJsf5L0yRuOVRFFz5GN.part1.nzb""

            var contents = File.ReadAllBytes(nzbFullPath);
            var nzbDocument = this.GetNzbDocument(contents);
            var key = GetKeyName(nzbFullPath, nzbDocument);
            var xDocument = new NzbDocumentWrapper(key, nzbFullPath, contents, nzbDocument);

            var sabNzbdService = new SABnzbdService();
            if (!sabNzbdService.IsWorkingProcess())
                sabNzbdService.StartProcess();
        
            if (sabNzbdService.IsWorkingProcess() && sabNzbdService.IsAlive()) 
            {
                sabNzbdService.Import(xDocument);
            }
        }

        private NzbDocument GetNzbDocument(byte[] contents)
        {
            NzbDocument retour = null;
            using (var file = new BufferedStream(new MemoryStream(contents)))
            {
                retour = NzbDocument.Load(file).Result;
            }

            return retour;
        }
 
        private string GetKeyName(string nzbFullPath, NzbDocument document)
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