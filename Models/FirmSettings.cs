// filename: F:\CabinetDoc Pro\Models\FirmSettings.cs
using System;

namespace CabinetDocProWpf.Models
{
    public class FirmSettings
    {
        public int Id { get; set; }
        public string? CabinetName { get; set; }
        public string? AccountantName { get; set; }
        public string? Address { get; set; }
        public string? Agrement { get; set; }
        public string? Nif { get; set; }
        public string? Nis { get; set; }
        public string? Ai { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? BankAddress { get; set; }
        public string? LogoPath { get; set; }
        public int LogoSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FirmSettings()
        {
            Id = 1;
            CabinetName = "CABINET DE COMPTABILITÉ\nET COMMISSAIRE AUX COMPTES";
            AccountantName = "MME KEBILI AMEL";
            Address = "15 Rue Didouche Mourad, Alger 16000, Algérie";
            Agrement = "AGR-2023-001";
            Nif = "099916000123456";
            Nis = "000116000123456";
            Ai = "16000123456001";
            BankName = "Banque d'Algérie";
            BankAccount = "007030990000123456789";
            BankAddress = "Agence Centrale, 38 Avenue Franklin Roosevelt, Alger";
            LogoPath = "";
            LogoSize = 90;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
