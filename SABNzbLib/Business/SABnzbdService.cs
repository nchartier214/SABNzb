using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Nzb.DataModel;
using Nzb.System;
using System.Threading;

namespace Nzb.Business
{
#pragma warning disable CA1822, CA1307

    public class SABnzbdService
    {
        private static readonly Lazy<IDictionary<string, string>> _configurationNames = new Lazy<IDictionary<string, string>>(() =>
                    ((IDictionary)ConfigurationManager.GetSection("sabnzbd"))
                    .Cast<DictionaryEntry>()
                    .ToDictionary(elt => (string)elt.Key, elt => (string)elt.Value));
        private Lazy<InterfaceWebManager> _interfaceWebManager = 
                    new Lazy<InterfaceWebManager>(() => new InterfaceWebManager( new Uri(SABnzbdConfiguration.Current.Server), SABnzbdConfiguration.Current.UserAgent));

        private InterfaceWebManager InterfaceWebManager { get { return this._interfaceWebManager.Value; } }


        public bool IsWorkingProcess()
        {
            var sabNzbd = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
            return sabNzbd.Any();
        }

        public bool IsAlive()
        {
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("mode", "fullstatus");
            postParameters.Add("skip_dashboard", "0");
            postParameters.Add("apikey", SABnzbdConfiguration.Current.ApiKey);
            var reponse = this.InterfaceWebManager.DataPost(postParameters);
            var ret1 = reponse.StatusCode == HttpStatusCode.OK;
            return ret1;
        }

        public void StartProcess()
        {
            Process retour = null;
            var processes = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
            if (!processes.Any())
            {
                using (var mutex = new Mutex(false, SABnzbdConfiguration.Current.ProgramPath))
                {
                    var mutexAcquired = false;
                    try
                    {
                        // acquire the mutex (or timeout after 60 seconds)
                        // will return false if it timed out
                        mutexAcquired = mutex.WaitOne(NzbConfiguration.Current.TimeoutWaiting);

                        processes = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
                        if (!processes.Any())
                        {
                            Process.Start(SABnzbdConfiguration.Current.ProgramPath);
                            Thread.Sleep(NzbConfiguration.Current.Sleep);
                            processes = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
                            retour = processes.First();
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        // abandoned mutexes are still acquired, we just need
                        // to handle the exception and treat it as acquisition
                        mutexAcquired = true;
                    }
                }

            }
        }
        
        public bool Import(NzbDocumentWrapper document)
        {
            Contract.Requires(document != null);
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            var parameter = new FileParameter(document.Buffer, $"{document.Name}.nzb", "application/x-nzb");
            postParameters.Add("mode", "addfile");
            postParameters.Add("nzbname", document.Name);
            postParameters.Add("nzbfile", parameter);
            postParameters.Add("apikey", SABnzbdConfiguration.Current.ApiKey);
            var reponse = this.InterfaceWebManager.DataPost(postParameters);
            var st1 = reponse.StringValue();
            var retour = reponse.StatusCode == HttpStatusCode.OK;
            return retour;
        }
    }
}
