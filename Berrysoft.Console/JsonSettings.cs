using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Reflection;

namespace Berrysoft.Console
{
    public abstract class JsonSettings : SettingsBase<JsonValue, object>
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
            foreach (var prop in properties)
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = null;
                if (json.ContainsKey(attr.Name))
                {
                    propValue = ChangeType(attr.Name, json[attr.Name], p.PropertyType);
                }
                if (propValue != null)
                {
                    p.SetValue(this, propValue);
                }
            }
        }
        public override void Save(string fileName)
        {
            Dictionary<string, JsonValue> dic = new Dictionary<string, JsonValue>();
            foreach (var prop in properties)
            {
                SettingsAttribute attr = prop.Key;
                PropertyInfo p = prop.Value;
                object propValue = p.GetValue(this);
                JsonValue value = ChangeBackType(attr.Name, propValue, p.PropertyType);
                dic.Add(attr.Name, value);
            }
            JsonObject json = new JsonObject(dic);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                json.Save(writer);
            }
        }
    }
}
