using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nanoManager.Api
{
    public static class NanoApiConfig
    {
        private static NanoConf _internalConfig;
        private static readonly string Path = Directory.GetCurrentDirectory() + "/apiConfig.json";

        public static NanoConf Config
        {
            get
            {
                TryLoad();
                return _internalConfig;
            }
        }

        static NanoApiConfig() => TryLoad();

        private static void TryLoad()
        {
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        _internalConfig = JsonConvert.DeserializeObject<NanoConf>(json);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            if (_internalConfig != null) return;
            _internalConfig = new NanoConf();
            Save();
        }


        public static void Save()
        {
            try
            {
                File.WriteAllText(Path, JsonConvert.SerializeObject(_internalConfig));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public class NanoConf
        {
            [CanBeNull] public string AuthKey { get; set; }
        }
    }
}