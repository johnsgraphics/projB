// filename: F:\CabinetDoc Pro\Services\SettingsIntegrationService.cs

using System;
using System.IO;
using System.Text.Json;
using CabinetDocProWpf.Models;

namespace CabinetDocProWpf.Services
{
    public class SettingsIntegrationService
    {
        private readonly SettingsService _settingsService;
        private FirmSettings _cachedSettings;
        private DocumentPreferences _cachedPreferences;

        public SettingsIntegrationService()
        {
            _settingsService = new SettingsService();
            LoadSettings();
        }

        private void LoadSettings()
        {
            _cachedSettings = _settingsService.GetFirmSettings();
            _cachedPreferences = LoadDocumentPreferences();
        }

        public FirmSettings GetFirmSettings()
        {
            if (_cachedSettings == null)
            {
                _cachedSettings = _settingsService.GetFirmSettings();
            }
            return _cachedSettings;
        }

        public DocumentPreferences GetDocumentPreferences()
        {
            if (_cachedPreferences == null)
            {
                _cachedPreferences = LoadDocumentPreferences();
            }
            return _cachedPreferences;
        }

        public void RefreshSettings()
        {
            _cachedSettings = null;
            _cachedPreferences = null;
            LoadSettings();
        }

        public string FormatNumber(decimal value)
        {
            var prefs = GetDocumentPreferences();

            return prefs.NumberFormat switch
            {
                "french" => value.ToString("N2").Replace(",", "X").Replace(".", ",").Replace("X", "."),
                "english" => value.ToString("N2"),
                "space" => value.ToString("N2").Replace(".", " "),
                _ => value.ToString("N2")
            };
        }

        public string FormatDate(DateTime date)
        {
            var prefs = GetDocumentPreferences();

            return prefs.DateFormat switch
            {
                "dd/mm/yyyy" => date.ToString("dd/MM/yyyy"),
                "mm/dd/yyyy" => date.ToString("MM/dd/yyyy"),
                "yyyy-mm-dd" => date.ToString("yyyy-MM-dd"),
                _ => date.ToString("dd/MM/yyyy")
            };
        }

        public string FormatCurrency(decimal value)
        {
            var prefs = GetDocumentPreferences();
            var formattedNumber = FormatNumber(value);
            return $"{formattedNumber} {prefs.Currency}";
        }

        public (double Width, double Height) GetPageSize()
        {
            var prefs = GetDocumentPreferences();

            return prefs.PageSize switch
            {
                "Letter" => (816, 1056),
                _ => (793, 1123) // A4
            };
        }

        public (double Left, double Top, double Right, double Bottom) GetPrintMargins()
        {
            var prefs = GetDocumentPreferences();

            return prefs.PrintMargins switch
            {
                "narrow" => (48, 48, 48, 48),
                "wide" => (144, 144, 144, 144),
                _ => (96, 96, 96, 96) // normal
            };
        }

        private DocumentPreferences LoadDocumentPreferences()
        {
            try
            {
                var prefsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "CabinetDocPro",
                    "document_preferences.json"
                );

                if (File.Exists(prefsPath))
                {
                    var json = File.ReadAllText(prefsPath);
                    return JsonSerializer.Deserialize<DocumentPreferences>(json)
                        ?? DocumentPreferences.Default();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading preferences: {ex.Message}");
            }

            return DocumentPreferences.Default();
        }
    }

    public class DocumentPreferences
    {
        public string NumberFormat { get; set; } = "french";
        public string DateFormat { get; set; } = "dd/mm/yyyy";
        public string Currency { get; set; } = "DA";
        public bool AutoSave { get; set; } = true;
        public bool AutoNumbering { get; set; } = true;
        public string PrintMargins { get; set; } = "normal";
        public string PageSize { get; set; } = "A4";
        public string DefaultLanguage { get; set; } = "fr";
        public string DefaultNoteTemplate { get; set; } = "";
        public string DefaultRapportTemplate { get; set; } = "";

        public static DocumentPreferences Default()
        {
            return new DocumentPreferences
            {
                NumberFormat = "french",
                DateFormat = "dd/mm/yyyy",
                Currency = "DA",
                AutoSave = true,
                AutoNumbering = true,
                PrintMargins = "normal",
                PageSize = "A4",
                DefaultLanguage = "fr"
            };
        }
    }
}
