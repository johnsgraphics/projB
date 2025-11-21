using System;
using System.Collections.Generic;

namespace CabinetDocProWpf.Models
{
    public class DocumentModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; } = "NOTE_HONORAIRES";
        public string ClientId { get; set; } = string.Empty;
        public string Theme { get; set; } = "BlueWave";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Note d'Honoraires fields
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);
        public string ServicePeriod { get; set; } = string.Empty;
        public List<LineItem> LineItems { get; set; } = new List<LineItem>();
        public double TvaRate { get; set; } = 0;
        public string PaymentTerms { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Rapport Special fields
        public string ReportNumber { get; set; } = string.Empty;
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public string MissionObject { get; set; } = "Augmentation du capital social";
        public double CapitalIncrease { get; set; } = 0;
        public string Preambule { get; set; } = string.Empty;
        public string ExposMotifs { get; set; } = string.Empty;
        public string ReferencesReglementaires { get; set; } = string.Empty;
        public string ReferencesInternes { get; set; } = string.Empty;
        public string ModaliteAugmentation { get; set; } = string.Empty;
        public string Conclusion { get; set; } = string.Empty;

        // Rapport Special Data
        public RapportSpecialData? RapportData { get; set; }
    }

    public class RapportSpecialData
    {
        public double CapitalActuel { get; set; }
        public double NouveauCapital { get; set; }
        public double Augmentation { get; set; }
        public List<PassifItem> Table1 { get; set; } = new List<PassifItem>();
        public List<CapitalItem> Table2 { get; set; } = new List<CapitalItem>();
    }

    public class PassifItem
    {
        public string Designation { get; set; } = string.Empty;
        public double PassifAvant { get; set; }
        public double PassifApres { get; set; }
        public double Variation { get; set; }
    }

    public class CapitalItem
    {
        public string Nature { get; set; } = string.Empty;
        public int NombreAvant { get; set; }
        public double ValeurAvant { get; set; }
        public int NombreApres { get; set; }
        public double ValeurApres { get; set; }
    }
}
