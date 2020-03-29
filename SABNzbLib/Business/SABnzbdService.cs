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
using Nzb.System.Types;
using System.Threading;
using System.IO;

namespace Nzb.Business
{
#pragma warning disable CA1822, CA1307

    public class SABnzbdService
    {

        private readonly Lazy<InterfaceWebManager> _interfaceWebManager =
                    new Lazy<InterfaceWebManager>(() => new InterfaceWebManager(new Uri(SABnzbdConfiguration.Current.Server), SABnzbdConfiguration.Current.UserAgent));

        private InterfaceWebManager InterfaceWebManager { get { return this._interfaceWebManager.Value; } }


        public bool IsWorkingProcess()
        {
            var sabNzbd = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
            return sabNzbd.Any();
        }

        public bool IsAlive()
        {
            var postParameters = new List<Tuple<string, object>>() {
                Tuple.Create( "mode", (object)"fullstatus"),
                Tuple.Create("skip_dashboard", (object)"0"),
                Tuple.Create("apikey", (object)SABnzbdConfiguration.Current.ApiKey) }
            .ToDictionary(elt => elt.Item1, elt => elt.Item2);

            var reponse = this.InterfaceWebManager.DataPost(postParameters);
            var ret1 = reponse.StatusCode == HttpStatusCode.OK;
            return ret1;
        }

        public void StartProcess()
        {
            LogManager.Current.Debug("Enter in StartProcess");
            var processes = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
            if (!processes.Any())
            {
                Process.Start(SABnzbdConfiguration.Current.ProgramPath);
                Thread.Sleep(NzbConfiguration.Current.Sleep);
                processes = Process.GetProcessesByName(SABnzbdConfiguration.Current.ProcessName);
                processes.First();
                LogManager.Current.Info($"Démarrage du serveur : {SABnzbdConfiguration.Current.ProcessName}");
            }

            LogManager.Current.Debug($"End StartProcess :{SABnzbdConfiguration.Current.ProcessName}");
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
            LogManager.Current.Debug($"Import de  : {document.Name}  ");

            var retour = false;
            using (var reponse = this.InterfaceWebManager.DataPost(postParameters))
            {
                var returnStatus = string.Empty;
                Contract.Requires(reponse != null);
                using (var responseReader = new StreamReader(reponse.GetResponseStream()))
                {
                    returnStatus = responseReader.ReadToEnd().TrimFullToEnd();
                }

                LogManager.Current.Debug($"Aprés import, {reponse.StatusCode}, Chaine retour: {returnStatus}");
                retour = reponse.StatusCode == HttpStatusCode.OK;
            }

            return retour;
        }
    }
}
