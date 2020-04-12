using Newtonsoft.Json;
using System;
using System.IO;

namespace Commodore.Engine.Persistence.JsonConfig
{
    public class JsonSettings : Section
    {
        public string FilePath { get; }
        
        public JsonSettings(string filePath)
        {
            FilePath = filePath;

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(FilePath))
                {
                    var json = sr.ReadToEnd();
                    Section sec = null;

                    try
                    {
                        sec = JsonConvert.DeserializeObject<Section>(json);
                    }
                    catch (JsonException je)
                    {
                        DebugLog.Error("Couldn't deserialize JSON settings - probably a syntax error. Check exception below.");
                        DebugLog.Exception(je);
                    }
                    catch (Exception e)
                    {
                        DebugLog.Exception(e);
                    }

                    if (sec != null)
                    {
                        foreach (string k in sec.Keys)
                            Add(k, sec[k]);
                    }
                }
            }
            Dirty = false;
        }

        public void Save(bool formatJson = true)
        {
            try
            {
                using (var sw = new StreamWriter(FilePath, false))
                    sw.WriteLine(JsonConvert.SerializeObject(this));

                Dirty = false;
            }
            catch (JsonException je)
            {
                DebugLog.Error($"Couldn't serialize the settings object back to JSON.");
                DebugLog.Exception(je);
            }
            catch (Exception e)
            {
                DebugLog.Exception(e);
            }
        }
    }
}
