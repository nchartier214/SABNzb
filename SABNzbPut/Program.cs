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

            LogManager.Current.Info($"Traitement du fichier: {args[0]}");

            try
            {
                var nzbBusiness = new NzbPut();
                nzbBusiness.NzbEventHander += (s, e) =>
                {
                    Console.WriteLine($"{e.Message}  == {stopWatch.ElapsedMilliseconds}ms") ;
                    Debug.Print($"{e.Message}  == {stopWatch.ElapsedMilliseconds}ms");
                };

                nzbBusiness.Do((IEnumerable<string>)args);
            }
            catch (Exception ex)
            {
                LogManager.Current.Error(ex);
                throw;
            }

            LogManager.Current.Info($"Fin de Traitement: {NzbContextDictionary.Current.Get<Stopwatch>(NzbContextDictionary.PrincipaleStopwath).ElapsedMilliseconds}ms");
        }
    }
}
