using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Nzb;
using Nzb.DataModel;
using Nzb.System;
using System.IO.Compression;

namespace Nzb.Business
{
    public class NzbPut
    {
        public event EventHandler<NzbEventArgs> NzbEventHander;

        public SABnzbdService SABnzbdService { get; } = new SABnzbdService();
        public ManagedFile GetManagedFile(string fichierFullPath)
        {
            ManagedFile retour = ManagedFile.Other;
            var extension = Path.GetExtension(fichierFullPath).ToUpperInvariant();
            switch (extension)
            {
                case ".NZB":
                    retour = ManagedFile.Nzb;
                    break;
                case ".ZIP":
                    retour = ManagedFile.Zip;
                    break;
            }

            return retour;
        }

        public bool IsManagedFile(string fichierFullPath)
        {
            var extension = this.GetManagedFile(fichierFullPath);
            return extension != ManagedFile.Other;
        }
        public void Do(string fichierFullPath)
        {
            if (!this.SABnzbdService.IsWorkingProcess())
            {
                this.FireNzbEvent("préparation du lancement de SABnzdb");
                this.SABnzbdService.StartProcess();
                this.FireNzbEvent("lancement de SABnzbd effectué");
            }

            var extension = this.GetManagedFile(fichierFullPath);
            switch (extension)
            {
                case ManagedFile.Nzb:
                    DoNzb(fichierFullPath);
                    break;
                case ManagedFile.Zip:
                    DoZip(fichierFullPath);
                    break;
            }
        }

        public void DoZip(string nzbFullPath)
        {
            bool erreur = false;
            var erreurFiles = new List<string>();
            using (var zip = ZipFile.OpenRead(nzbFullPath))
            {
                foreach (var fil1 in zip.Entries)
                {
                    var extension = this.GetManagedFile(fil1.FullName);
                    if (extension == ManagedFile.Other)
                    {
                        var fileName = Path.Combine(KnownFolders.GetPath(KnownFolder.Downloads), Path.GetFileName(fil1.FullName));
                        fil1.ExtractToFile(fileName, true);
                        erreurFiles.Add(fileName);
                        erreur = true;
                    }
                    else
                    {
                        var fileName = Path.GetTempFileName();
                        fil1.ExtractToFile(fileName, true);
                        var contents = File.ReadAllBytes(fileName);
                        var nzbDocument = NzbPut.GetNzbDocument(contents);
                        var key = NzbPut.GetKeyName(nzbFullPath, nzbDocument);

                        var xDocument = new NzbDocumentWrapper(key, nzbFullPath, contents, nzbDocument);
                        this.DoNzb(xDocument);
                        File.Delete(fileName);
                    }
                }
            }

            if (erreur)
                throw new NzbZipException($"copy bad file ({String.Join(", ", erreurFiles)} in {KnownFolders.GetPath(KnownFolder.Downloads)}");
        }

        public void DoNzb(string nzbFullPath)
        {
            var contents = File.ReadAllBytes(nzbFullPath);
            var nzbDocument = NzbPut.GetNzbDocument(contents);
            var key = NzbPut.GetKeyName(nzbFullPath, nzbDocument);

            var xDocument = new NzbDocumentWrapper(key, nzbFullPath, contents, nzbDocument);
            this.DoNzb(xDocument);
        }

        public void DoNzb(NzbDocumentWrapper xDocument)
        {
            Contract.Requires(xDocument != null);

            if (!(this.SABnzbdService.IsWorkingProcess() && this.SABnzbdService.IsAlive()))
                throw new NzbException("state exception");

            this.FireNzbEvent($"préparation du démarage de {xDocument.Name}");
            this.SABnzbdService.Import(xDocument);
            this.FireNzbEvent($"lancement de {xDocument.Name}");
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