using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    public class ConfigReader
    {
        public string Token {  get; set; }
        public string Pefix { get; set; }
        public string dbConnection { get; set; }

        public async Task ReadConfig()
        {
            using(StreamReader reader = new StreamReader("config/config.json"))
            {
                string json = await reader.ReadToEndAsync();
                ConfigStructure data = JsonConvert.DeserializeObject<ConfigStructure>(json);

                this.Token = data.Token;
                this.Pefix = data.Prefix;
                this.dbConnection = data.dbConnection;
            }
        }
    }
}
