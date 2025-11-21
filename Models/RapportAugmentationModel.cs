// filename: F:\CabinetDoc Pro\Models\RapportAugmentationModel.cs

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CabinetDocProWpf.Services;

namespace CabinetDocProWpf.Models
{
    public class RapportAugmentationModel : INotifyPropertyChanged
    {
        private readonly SettingsIntegrationService _settingsService;

        public RapportAugmentationModel()
        {
            _settingsService = new SettingsIntegrationService();
            InitializeFromSettings();

            Titre = "RAPPORT SPECIAL PROJET D'AUGMENTATION DU CAPITAL SOCIAL";
            DateDocument = DateTime.Today;
            DateAssemblee = DateTime.Today;
            ValeurNominaleParPart = 1000m;
            MontantAugmentation = 0m;
            Associes = new ObservableCollection<Associe>();

            // Sample data
            CapitalActuel = 200000000m;
            CapitalNouveaux = 400000000m;
        }

        private void InitializeFromSettings()
        {
            var firmSettings = _settingsService.GetFirmSettings();
            var prefs = _settingsService.GetDocumentPreferences();

            ComptableNom = firmSettings.AccountantName ?? "MME KEBILI AMEL";
            ComptableTitre = "Commissaire aux Comptes";
            Adresse = firmSettings.Address ?? "15 Rue Didouche Mourad, Alger 16000";
            NIF = firmSettings.Nif ?? "099916000123456";
            NIS = firmSettings.Nis ?? "000116000123456";
            AI = firmSettings.Ai ?? "16000123456001";
            Agrement = firmSettings.Agrement ?? "AGR-2023-001";
            CabinetName = firmSettings.CabinetName ?? "CABINET DE COMPTABILITÉ ET COMMISSAIRE AUX COMPTES";
            LogoPath = firmSettings.LogoPath;
            LogoSize = firmSettings.LogoSize > 0 ? firmSettings.LogoSize : 90;

            NomBanque = firmSettings.BankName ?? "Banque d'Algérie";
            NumeroCompte = firmSettings.BankAccount ?? "00799999000123456789";
            AdresseBanque = firmSettings.BankAddress ?? "Agence Centrale, 38 Avenue Franklin Roosevelt, Alger";

            CurrencySymbol = prefs.Currency;
            DateFormatPattern = prefs.DateFormat;
        }

        // Header fields
        private string _comptableNom;
        public string ComptableNom { get => _comptableNom; set { _comptableNom = value; OnPropertyChanged(nameof(ComptableNom)); } }

        private string _comptableTitre;
        public string ComptableTitre { get => _comptableTitre; set { _comptableTitre = value; OnPropertyChanged(nameof(ComptableTitre)); } }

        private string _cabinetName;
        public string CabinetName { get => _cabinetName; set { _cabinetName = value; OnPropertyChanged(nameof(CabinetName)); } }

        private string _adresse;
        public string Adresse { get => _adresse; set { _adresse = value; OnPropertyChanged(nameof(Adresse)); } }

        private string _nif;
        public string NIF { get => _nif; set { _nif = value; OnPropertyChanged(nameof(NIF)); } }

        private string _nis;
        public string NIS { get => _nis; set { _nis = value; OnPropertyChanged(nameof(NIS)); } }

        private string _ai;
        public string AI { get => _ai; set { _ai = value; OnPropertyChanged(nameof(AI)); } }

        private string _agrement;
        public string Agrement { get => _agrement; set { _agrement = value; OnPropertyChanged(nameof(Agrement)); } }

        private string _logoPath;
        public string LogoPath { get => _logoPath; set { _logoPath = value; OnPropertyChanged(nameof(LogoPath)); } }

        private int _logoSize;
        public int LogoSize { get => _logoSize; set { _logoSize = value; OnPropertyChanged(nameof(LogoSize)); } }

        // Bank fields
        private string _nomBanque;
        public string NomBanque { get => _nomBanque; set { _nomBanque = value; OnPropertyChanged(nameof(NomBanque)); } }

        private string _numeroCompte;
        public string NumeroCompte { get => _numeroCompte; set { _numeroCompte = value; OnPropertyChanged(nameof(NumeroCompte)); } }

        private string _adresseBanque;
        public string AdresseBanque { get => _adresseBanque; set { _adresseBanque = value; OnPropertyChanged(nameof(AdresseBanque)); } }

        // Document fields
        private string _titre;
        public string Titre { get => _titre; set { _titre = value; OnPropertyChanged(nameof(Titre)); } }

        private DateTime _dateDocument;
        public DateTime DateDocument { get => _dateDocument; set { _dateDocument = value; OnPropertyChanged(nameof(DateDocument)); } }

        private DateTime _dateAssemblee;
        public DateTime DateAssemblee { get => _dateAssemblee; set { _dateAssemblee = value; OnPropertyChanged(nameof(DateAssemblee)); } }

        // Capital fields
        private decimal _capitalActuel;
        public decimal CapitalActuel { get => _capitalActuel; set { _capitalActuel = value; OnPropertyChanged(nameof(CapitalActuel)); } }

        private decimal _capitalNouveaux;
        public decimal CapitalNouveaux { get => _capitalNouveaux; set { _capitalNouveaux = value; OnPropertyChanged(nameof(CapitalNouveaux)); } }

        private decimal _montantAugmentation;
        public decimal MontantAugmentation { get => _montantAugmentation; set { _montantAugmentation = value; OnPropertyChanged(nameof(MontantAugmentation)); } }

        private decimal _valeurNominaleParPart;
        public decimal ValeurNominaleParPart { get => _valeurNominaleParPart; set { _valeurNominaleParPart = value; OnPropertyChanged(nameof(ValeurNominaleParPart)); } }

        // Format fields
        private string _currencySymbol;
        public string CurrencySymbol { get => _currencySymbol; set { _currencySymbol = value; OnPropertyChanged(nameof(CurrencySymbol)); } }

        private string _dateFormatPattern;
        public string DateFormatPattern { get => _dateFormatPattern; set { _dateFormatPattern = value; OnPropertyChanged(nameof(DateFormatPattern)); } }

        // Collection
        public ObservableCollection<Associe> Associes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Associe : INotifyPropertyChanged
    {
        private string _nomAssocie;
        public string NomAssocie { get => _nomAssocie; set { _nomAssocie = value; OnPropertyChanged(nameof(NomAssocie)); } }

        private int _nbPartsAvant;
        public int NbPartsAvant { get => _nbPartsAvant; set { _nbPartsAvant = value; OnPropertyChanged(nameof(NbPartsAvant)); } }

        private decimal _valeurPartsAvant;
        public decimal ValeurPartsAvant { get => _valeurPartsAvant; set { _valeurPartsAvant = value; OnPropertyChanged(nameof(ValeurPartsAvant)); } }

        private int _nbPartsApres;
        public int NbPartsApres { get => _nbPartsApres; set { _nbPartsApres = value; OnPropertyChanged(nameof(NbPartsApres)); } }

        private decimal _valeurPartsApres;
        public decimal ValeurPartsApres { get => _valeurPartsApres; set { _valeurPartsApres = value; OnPropertyChanged(nameof(ValeurPartsApres)); } }

        private decimal _pourcentage;
        public decimal Pourcentage { get => _pourcentage; set { _pourcentage = value; OnPropertyChanged(nameof(Pourcentage)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
