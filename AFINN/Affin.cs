using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Serilog;
using Core;

namespace AFINNService
{
    public class AFINN : IAFINNService
    {
        public Dictionary<string, string> GetDictionary()
        {
            try
            {                
                 var afinnData = File.ReadAllText(@"..\AFINN\AFINN.json");
                var afinnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(afinnData);
                return afinnDictionary;
            }
            catch (Exception ex)
            {
                Log.Error($"AFINNService.AFINN.GetDictionary() error { DateTime.Now}{ Environment.NewLine}{ex}");
                return null;
            }
        }

    }
}
