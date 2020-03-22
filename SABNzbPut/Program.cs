using Nzb.Business;
using Nzb.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Nzb.DataModel;

namespace SABNzbPut
{
    class Program
    {

        
        static void Main(string[] args)
        {
            if (!Directory.Exists(NzbConfiguration.Current.CompleteDirectory))
            {
                LogManager.Current.Error("Complete directory is not found");
                System.Environment.Exit(2);
            }


            if (args.Count() == 0)
            {
                LogManager.Current.Error( "nzb missing");
                System.Environment.Exit(2);
            }

            LogManager.Current.Info($"NZB: {args[0]}");

            try
            {
                var nzbBusiness = new NzbPut();
                nzbBusiness.Do((IEnumerable<string>)args);
            }
            catch (Exception ex)
            {
                LogManager.Current.Error(ex);
            }
        }
    }
}
