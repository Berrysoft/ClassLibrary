using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Reflection;

namespace Berrysoft.Console
{
    public abstract class JsonSettings : SettingsBase
    {
        public JsonSettings()
            : base()
        { }
        public override void Open(string fileName)
        {
            JsonValue json;
            using (StreamReader reader = new StreamReader(fileName))
            {
                json = JsonValue.Load(reader);
            }
            foreach (string name in Settings)
            {
                if(json.ContainsKey(name))
                {
                    SetValue(name, json[name]);
                }
            }
        }
        public override void Save(string fileName)
        {
            Dictionary<string, JsonValue> dic = new Dictionary<string, JsonValue>();
            foreach (string name in Settings)
            {
                if (GetValue(name) is JsonValue propValue)
                {
                    dic.Add(name, propValue);
                }
            }
            JsonObject json = new JsonObject(dic);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                json.Save(writer);
            }
        }
    }
}
