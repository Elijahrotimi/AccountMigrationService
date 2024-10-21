using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMigrationService.Producer.Utilities
{
    public class TimeStampHandler
    {
        public static void UpdateTimeStamp(string timeStamp)
        {

            string filePath = "timestamp.json";

            JObject jsonObj;
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                jsonObj = JObject.Parse(json);
            }
            else
            {
                jsonObj = new JObject();
            }

            jsonObj["LastCreateDateValue"] = timeStamp;

            File.WriteAllText(filePath, jsonObj.ToString());

        }

        public static string GetTimeStamp()
        {
            string filePath = "timestamp.json";

            var json = File.ReadAllText(filePath);

            JObject jsonObj = JObject.Parse(json);

            string timestampString = jsonObj["LastCreateDateValue"].ToString();

            return timestampString;
        }
    }
}
