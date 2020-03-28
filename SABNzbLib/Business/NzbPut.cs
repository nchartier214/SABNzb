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
using Nzb.System;

namespace Nzb.Business
{
    public class NzbPut
    {
        public event EventHandler<NzbEventArgs> NzbEventHander;

        public void Do(IEnumerable<string> nzbFullPaths)
        {
            if (nzbFullPaths == null)
                throw new ArgumentNullException(nameof(nzbFullPaths));

            foreach (var nzbFullPath in nzbFullPaths)
                this.Do(nzbFullPath);
        }

        public void Do(string nzbFullPath)
        {

            var contents = File.ReadAllBytes(nzbFullPath);
            var nzbDocument = NzbPut.GetNzbDocument(contents);
            var key = NzbPut.GetKeyName(nzbFullPath, nzbDocument);

            var xDocument = new NzbDocumentWrapper(key, nzbFullPath, contents, nzbDocument);

            var sabNzbdService = new SABnzbdService();
            if (!sabNzbdService.IsWorkingProcess())
            {
                this.FireNzbEvent("préparation du lancement de SABnzdb");
                sabNzbdService.StartProcess();
                this.FireNzbEvent("lancement de SABnzbd effectué");
            }

            if (sabNzbdService.IsWorkingProcess() && sabNzbdService.IsAlive())
            {
                this.FireNzbEvent($"préparation du démarage de {xDocument.Name}");
                sabNzbdService.Import(xDocument);
                this.FireNzbEvent($"lancement de {xDocument.Name}");
            }
        }

        private static long GetValChrono()
        {
            return NzbContextDictionary.Current.Get<Stopwatch>(NzbContextDictionary.PrincipaleStopwath).ElapsedMilliseconds;
        }
        private void FireNzbEvent(string message)
        {
            var msg = new NzbEventArgs($"{NzbPut.GetValChrono()}ms - {message}");
            this.NzbEventHander?.Invoke(this, msg);
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
                foreach (var nzb in document.Files)
                {
                    var pattern = @"""(.+?)\.\w+""";
                    var match = Regex.Match(nzb.Subject, pattern);
                    if (match.Success)
                    {
                        retour = match.Groups[1].Value;
                        break;
                    }
                }

                return retour;
            }

            return retour;
        }
    }
}