using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Legion.Models.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Legion.Helpers
{
    public static class SettingsHelper
    {
        private const string SettingsPath = "settings.json";

        public static Settings? LoadSettings()
        {
            return !File.Exists(SettingsPath) ? null : JsonSerializer.Deserialize<Settings>(File.ReadAllText(SettingsPath));
        }

        public static void SaveSettings(Settings settings)
        {
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings));
        }
    }
}
