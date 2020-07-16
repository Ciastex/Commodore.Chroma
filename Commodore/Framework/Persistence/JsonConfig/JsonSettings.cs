using System;
using System.IO;
using System.Text.Json;
using Chroma.Diagnostics.Logging;

namespace Commodore.Framework.Persistence.JsonConfig
{
    public class JsonSettings : Section
    {
        private Log Log { get; } = LogManager.GetForCurrentAssembly();

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
                        sec = JsonSerializer.Deserialize<Section>(json);
                    }
                    catch (JsonException je)
                    {
                        Log.Error(
                            "Couldn't deserialize JSON settings - probably a syntax error. Check exception below.");
                        Log.Exception(je);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }

                    if (sec != null)
                    {
                        foreach (var k in sec.Keys)
                            Add(k, sec[k]);
                    }
                }
            }

            Dirty = false;
        }

        public void Save()
        {
            try
            {
                using (var sw = new StreamWriter(FilePath, false))
                {
                    var str = JsonSerializer.Serialize(this, 
                        new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }
                    );
                    
                    sw.WriteLine(str);
                }

                Dirty = false;
            }
            catch (JsonException je)
            {
                Log.Error($"Couldn't serialize the settings object back to JSON.");
                Log.Exception(je);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}