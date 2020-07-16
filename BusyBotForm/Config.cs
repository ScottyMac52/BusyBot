using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace BusyBotForm
{
    /// <summary>
    /// Singleton to hold configuration information
    /// </summary>
    public class Config
    {
        private static readonly Lazy<Config>
            lazy = new Lazy<Config>(() => new Config());

        public static Config Instance => lazy.Value;

        public  IConfigurationRoot Configuration { get; private set; }

        private Config()
        {
        }

        public void Initialize(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Interval
        /// </summary>
        public string Interval => GetConfigurationValueSafely("Interval", "00:00:10");

        /// <summary>
        /// Class name of window to find
        /// </summary>
        /// <remarks>http://www.mhavila.com.br/topicos/mm/winclass.html</remarks>
        public string FindClass => GetConfigurationValueSafely("FindClass", "Notepad");

        /// <summary>
        /// Message to print
        /// </summary>
        public string Message => GetConfigurationValueSafely("Message", "None");

        /// <summary>
        /// How many lines to write for each set
        /// </summary>
        public int LinesToWrite => GetConfigurationValueSafely<int>("LinesToWrite", 10);

        /// <summary>
        /// Template function that can convert any types that implement <see cref="IConvertible"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private T GetConfigurationValueSafely<T>(string key, T defaultValue) where T : IConvertible
        {
            try
            {
                var configValue = Configuration[key];
                return (T)Convert.ChangeType(configValue, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }




    }
}
