using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.DataModel
{
    public class NzbConfiguration
    {
        private readonly static Lazy<NzbConfiguration> _current = new Lazy<NzbConfiguration>(() =>
        {
            Func<string, int> parseValeur = delegate (string value)
            {
                var ts = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                return Convert.ToInt32(ts.TotalMilliseconds);
            };

            var section = ((Hashtable)ConfigurationManager.GetSection("nzb"))
                         .Cast<DictionaryEntry>()
                         .ToDictionary(kvp => (string)kvp.Key, kvp => (string)kvp.Value);

            return new NzbConfiguration(
                parseValeur( section["sleep"]),
                section["completeDirectory"],
                parseValeur(section["timeoutWaiting"]),
                section["mutexName"]
            );
        });

        public static NzbConfiguration Current { get { return NzbConfiguration._current.Value; } }
        public int Sleep { get; private set; }
        public string CompleteDirectory { get; private set; }

        public int TimeoutWaiting { get; private set; }
        public string MutexName { get; private set; }

        private NzbConfiguration(int sleep, string completeDirectory, int timeoutWaiting, string mutexName)
        {
            this.MutexName = mutexName;
            this.Sleep = sleep;
            this.CompleteDirectory = completeDirectory;
            this.TimeoutWaiting = timeoutWaiting;
        }

    }
}
