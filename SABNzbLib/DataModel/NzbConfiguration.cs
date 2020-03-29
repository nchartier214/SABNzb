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
            Func<string, TimeSpan> parseValeur = delegate (string value)
            {
                var ts = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                return ts;
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
        public TimeSpan Sleep { get; private set; }
        public string CompleteDirectory { get; private set; }

        public TimeSpan TimeoutWaiting { get; private set; }
        public string MutexName { get; private set; }

        private NzbConfiguration(TimeSpan sleep, string completeDirectory, TimeSpan timeoutWaiting, string mutexName)
        {
            this.MutexName = mutexName;
            this.Sleep = sleep;
            this.CompleteDirectory = completeDirectory;
            this.TimeoutWaiting = timeoutWaiting;
        }

    }
}
