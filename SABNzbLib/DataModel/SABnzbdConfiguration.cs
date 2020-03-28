using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.DataModel
{
    public class SABnzbdConfiguration
    {
        private static readonly Lazy<SABnzbdConfiguration> _current = new Lazy<SABnzbdConfiguration>(() =>
        {
            var section = ((Hashtable)ConfigurationManager.GetSection("sabnzbd"))
                          .Cast<DictionaryEntry>()
                          .ToDictionary (kvp => (string)kvp.Key, kvp => (string)kvp.Value);

            return new SABnzbdConfiguration(
                section["server"],
                section["programPath"],
                Path.GetFileNameWithoutExtension(section["programPath"]),
                section["apiKey"],
                section["nzbKey"],
                section["userAgent"]);
        });

        public static SABnzbdConfiguration Current { get { return SABnzbdConfiguration._current.Value; } }
        public string Server { get; private set; }
        public string ProcessName { get; private set; }
        public string ProgramPath { get; private set; }
        public string ApiKey { get; private set; }
        public string NzbKey { get; private set; }
        public string UserAgent { get; private set; }
        private SABnzbdConfiguration(string server, string programName, string processName, string apiKey, string nzbKey, string userAgent)
        {
            this.Server = server;
            this.ProgramPath = programName;
            this.ProcessName = processName;
            this.ApiKey = apiKey;
            this.NzbKey = nzbKey;
            this.UserAgent = userAgent;
        }
    }
}
