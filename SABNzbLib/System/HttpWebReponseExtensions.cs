using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.System
{
    public static class HttpWebReponseExtensions
    { 
        public static string StringValue(this HttpWebResponse reponse)
        {
            Contract.Requires(reponse != null);
            string retour = null;
            using (var responseReader = new StreamReader(reponse.GetResponseStream()))
            {
                retour = responseReader.ReadToEnd();
                reponse.Close();
            }

            return retour;
        }
    }
}
