using Nzb.Business;
using Nzb.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nzb.DataModel;
using System.Diagnostics;
using SABPut.Properties;
using System.Threading;

namespace SABNzbPut
{
    class Program
    {
        private static Lazy<NzbPut> _nzbBusiness = new Lazy<NzbPut>(() =>
        {
            var retour = new NzbPut();
            retour.NzbEventHander += (s, e) =>
            {
                var stopWatch = NzbContextDictionary.Current.Get<Stopwatch>(NzbContextDictionary.PrincipaleStopwath);
                var msg = $"{stopWatch.ElapsedMilliseconds}ms - {e.Message}";
                Console.WriteLine(msg);
                Debug.Print(msg);
            };

            return retour;
        });

        private static NzbPut NzbBusiness { get { return Program._nzbBusiness.Value; } }
        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            var context = NzbContextDictionary.Create();
            context.Add(NzbContextDictionary.PrincipaleStopwath, stopWatch);
            log4net.Config.XmlConfigurator.Configure();

            if (!Directory.Exists(NzbConfiguration.Current.CompleteDirectory))
                throw new DirectoryNotFoundException(Resources.DirectoryNotFound);

            if (args.Count() == 0)
            {
                var msg = Resources.NZBDontPassed;
                LogManager.Current.Error(msg);
                throw new ArgumentException(msg);
            }

            var fileName = args.First();

            using (var mutex = new Mutex(false, NzbConfiguration.Current.MutexName))
            {
                var mutexAcquired = false;
                try
                {
                    // acquire the mutex (or timeout after 60 seconds)
                    // will return false if it timed out
                    mutexAcquired = mutex.WaitOne(NzbConfiguration.Current.TimeoutWaiting);

                    try
                    {
                        Program.Traitement(fileName);
                    }
                    catch (FileNotFoundException ex)
                    {
                        LogManager.Current.Error(ex);
                        throw;
                    }
                    catch (NzbZipException ex)
                    {
                        LogManager.Current.Error(ex);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        File.Copy(fileName, Path.Combine(KnownFolders.GetPath(KnownFolder.Downloads), Path.GetFileName(fileName)), true);
                        LogManager.Current.Error(ex);
                        throw;
                    }
                }

                catch (AbandonedMutexException ex)
                {
                    LogManager.Current.Error(ex);
                    throw;
                }
            }
        }

        static void Traitement(string fileName)
        {
            LogManager.Current.Info($"Traitement du fichier: {fileName}");
            if (!Program.NzbBusiness.IsManagedFile(fileName))
                throw new NzbException("No managed extension");

            Program.NzbBusiness.Do(fileName);

            LogManager.Current.Info($"Fin de Traitement: {NzbContextDictionary.Current.Get<Stopwatch>(NzbContextDictionary.PrincipaleStopwath).ElapsedMilliseconds}ms");
        }
    }
}
