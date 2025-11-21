using System;
using System.IO;
using System.Text.Json;
using CabinetDocProWpf.Models;

namespace CabinetDocProWpf.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private readonly string _settingsFile;

        public SettingsService()
        {
            _settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CabinetDocPro"
            );
            _settingsFile = Path.Combine(_settingsPath, "firm_settings.json");

            // Create directory if doesn't exist
            Directory.CreateDirectory(_settingsPath);
        }

        public FirmSettings GetFirmSettings()
        {
            try
            {
                if (File.Exists(_settingsFile))
                {
                    string json = File.ReadAllText(_settingsFile);
                    var settings = JsonSerializer.Deserialize<FirmSettings>(json);
                    return settings ?? new FirmSettings();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new FirmSettings();
        }

        public void SaveFirmSettings(FirmSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(_settingsFile, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'enregistrement des paramètres: {ex.Message}");
            }
        }

        public void ResetToDefaults()
        {
            var defaultSettings = new FirmSettings();
            SaveFirmSettings(defaultSettings);
        }

        public bool SettingsExist()
        {
            return File.Exists(_settingsFile);
        }
    }
}
