// filename: F:\CabinetDoc Pro\Views\Pages\RapportSpecialPage.xaml.cs

using CabinetDocProWpf.Models;
using CabinetDocProWpf.Services;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CabinetDocProWpf.Views.Pages
{
    public partial class RapportSpecialPage : Page
    {
        private RapportAugmentationModel Model;
        private readonly SettingsIntegrationService _settingsService;

        public RapportSpecialPage()
        {
            InitializeComponent();
            _settingsService = new SettingsIntegrationService();
            Model = new RapportAugmentationModel();
            this.DataContext = Model;

            // Initialize default text sections
            InitializeDefaultSections();

            // Wire up events
            BtnAddAssocie.Click += BtnAddAssocie_Click;
            BtnRemoveAssocie.Click += BtnRemoveAssocie_Click;
            BtnCalculer.Click += BtnCalculer_Click;
            BtnApercu.Click += BtnApercu_Click;
            BtnImprimer.Click += BtnImprimer_Click;
            BtnEnregistrer.Click += BtnEnregistrer_Click;

            // Auto-calculate variation when capital changes
            TxtCapitalAvant.LostFocus += (s, e) => UpdateVariations();
            TxtCapitalApres.LostFocus += (s, e) => UpdateVariations();
            TxtCompteAvant.LostFocus += (s, e) => UpdateVariations();
            TxtCompteApres.LostFocus += (s, e) => UpdateVariations();
        }

        private void InitializeDefaultSections()
        {
            TxtPreambule.Text = "Nous avons l'honneur de vous présenter notre rapport spécial concernant le projet d'augmentation du capital social de votre société. Cette mission s'inscrit dans le cadre de nos prestations d'expertise comptable et vise à analyser la faisabilité et les implications de cette opération.";

            TxtExposeMotifs.Text = "Les raisons qui motivent cette augmentation de capital sont multiples :\n\n1. Renforcement de la structure financière de l'entreprise\n2. Amélioration de la capacité d'autofinancement\n3. Préparation aux investissements futurs\n4. Optimisation de la répartition du capital entre associés\n\nCette opération s'inscrit dans une stratégie de développement à long terme.";

            TxtReferencesReglementaires.Text = "Les références réglementaires applicables sont :\n\n• Article 691 du Code de commerce algérien relatif aux augmentations de capital\n• Article 573 du Code de commerce concernant les assemblées générales extraordinaires\n• Ordonnance n° 75-59 du 26 septembre 1975 portant code de commerce\n• Dispositions légales et réglementaires en vigueur";

            TxtReferencesInternes.Text = "Les références internes à la société comprennent :\n\n• Statuts de la société dans leur version actuellement en vigueur\n• Procès-verbaux des délibérations antérieures du conseil d'administration\n• États financiers certifiés des trois derniers exercices\n• Extrait des comptes sur les comptes courants associés";

            TxtModaliteAugmentation.Text = "L'augmentation du capital social s'effectuera selon les modalités suivantes :\n\n1. MODALITE RETENUE : Augmentation par capitalisation du compte courant d'associé\n2. MONTANT : L'augmentation portera le capital de [montant actuel] DA à [nouveau montant] DA\n3. PROCEDURE : Incorporation au capital des sommes inscrites au compte courant des associés\n4. REPARTITION : Attribution de nouvelles parts sociales aux associés au prorata de leurs apports";

            TxtConclusion.Text = "En conclusion, nous certifions que l'augmentation du capital social proposée est :\n\n• Conforme aux dispositions légales et réglementaires\n• Compatible avec les statuts de la société\n• Techniquement réalisable dans les conditions proposées\n• Favorable aux intérêts de la société et de ses associés";
        }

        private void UpdateVariations()
        {
            try
            {
                // Capital variation
                if (decimal.TryParse(TxtCapitalAvant.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal capitalAvant) &&
                    decimal.TryParse(TxtCapitalApres.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal capitalApres))
                {
                    var variationCapital = capitalApres - capitalAvant;
                    TxtVariationCapital.Text = $"{variationCapital:N2} DA";
                }

                // Compte courant variation
                if (decimal.TryParse(TxtCompteAvant.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal compteAvant) &&
                    decimal.TryParse(TxtCompteApres.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal compteApres))
                {
                    var variationCompte = compteApres - compteAvant;
                    TxtVariationCompte.Text = $"{variationCompte:N2} DA";
                }
            }
            catch { }
        }

        private void BtnAddAssocie_Click(object sender, RoutedEventArgs e)
        {
            var associe = new Associe
            {
                NomAssocie = "Nouvel associé",
                NbPartsAvant = 0,
                ValeurPartsAvant = 0m,
                NbPartsApres = 0,
                ValeurPartsApres = 0m,
                Pourcentage = 0m
            };
            Model.Associes.Add(associe);
        }

        private void BtnRemoveAssocie_Click(object sender, RoutedEventArgs e)
        {
            if (DgAssocies?.SelectedItem is Associe associe)
            {
                Model.Associes.Remove(associe);
            }
        }

        private void BtnCalculer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var totalApres = Model.Associes.Sum(a => a.ValeurPartsApres);

                foreach (var associe in Model.Associes)
                {
                    if (totalApres > 0)
                    {
                        associe.Pourcentage = Math.Round((associe.ValeurPartsApres / totalApres) * 100, 2);
                    }
                    else
                    {
                        associe.Pourcentage = 0;
                    }
                }

                UpdateVariations();

                MessageBox.Show("Calculs effectués avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur dans les calculs: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnApercu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var previewWindow = new Window
                {
                    Title = "Aperçu du Rapport",
                    Width = 900,
                    Height = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };

                var flowDocument = GenerateFlowDocument();
                var viewer = new FlowDocumentPageViewer { Document = flowDocument };
                previewWindow.Content = viewer;
                previewWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'aperçu: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    var flowDocument = GenerateFlowDocument();
                    IDocumentPaginatorSource idpSource = flowDocument;
                    printDialog.PrintDocument(idpSource.DocumentPaginator, "Rapport Spécial");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'impression: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = $"Rapport_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var json = JsonSerializer.Serialize(Model, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(saveDialog.FileName, json);
                    MessageBox.Show("Rapport enregistré avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'enregistrement: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument GenerateFlowDocument()
        {
            var doc = new FlowDocument
            {
                PageWidth = 793,
                PageHeight = 1123,
                PagePadding = new Thickness(60, 80, 60, 80),
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 11,
                ColumnWidth = double.PositiveInfinity
            };

            // Header with Logo and Firm Info
            var headerTable = new Table { CellSpacing = 0, Margin = new Thickness(0, 0, 0, 20) };
            headerTable.Columns.Add(new TableColumn { Width = new GridLength(100) });
            headerTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            var headerGroup = new TableRowGroup();
            var headerRow = new TableRow();

            // Logo Cell
            var logoCell = new TableCell { Padding = new Thickness(0) };
            if (!string.IsNullOrEmpty(Model.LogoPath) && File.Exists(Model.LogoPath))
            {
                var logoPara = new Paragraph { Margin = new Thickness(0) };
                var logoImage = new System.Windows.Controls.Image
                {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(Model.LogoPath)),
                    Width = Model.LogoSize,
                    Height = Model.LogoSize
                };
                var container = new InlineUIContainer(logoImage);
                logoPara.Inlines.Add(container);
                logoCell.Blocks.Add(logoPara);
            }
            headerRow.Cells.Add(logoCell);

            // Firm Info Cell
            var firmInfoCell = new TableCell { Padding = new Thickness(0) };
            var firmInfoPara = new Paragraph { Margin = new Thickness(0), FontSize = 9, LineHeight = 1.4 };

            firmInfoPara.Inlines.Add(new Bold(new Run(Model.CabinetName ?? "") { FontSize = 10 }));
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new Run(Model.ComptableTitre ?? ""));
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new Bold(new Run(Model.ComptableNom ?? "")));
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new Run(Model.Adresse ?? ""));
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new Run($"NIF: {Model.NIF ?? "N/A"} - NIS: {Model.NIS ?? "N/A"}"));
            firmInfoPara.Inlines.Add(new LineBreak());
            firmInfoPara.Inlines.Add(new Run($"AI: {Model.AI ?? "N/A"} - AGRÉMENT: {Model.Agrement ?? "N/A"}"));

            firmInfoCell.Blocks.Add(firmInfoPara);
            headerRow.Cells.Add(firmInfoCell);

            headerGroup.Rows.Add(headerRow);
            headerTable.RowGroups.Add(headerGroup);
            doc.Blocks.Add(headerTable);

            // Title
            var titlePara = new Paragraph(new Bold(new Run(Model.Titre ?? "RAPPORT SPÉCIAL")))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 5),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 58, 138))
            };
            doc.Blocks.Add(titlePara);

            // Document Number
            if (!string.IsNullOrEmpty(TxtNumeroRapport.Text))
            {
                var numPara = new Paragraph(new Run($"N° {TxtNumeroRapport.Text}"))
                {
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 15),
                    FontSize = 10,
                    Foreground = Brushes.Gray
                };
                doc.Blocks.Add(numPara);
            }

            // Date
            var datePara = new Paragraph(new Run($"Date: {_settingsService.FormatDate(Model.DateDocument)}"))
            {
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 0, 20),
                FontSize = 10
            };
            doc.Blocks.Add(datePara);

            // PRÉAMBULE
            AddSection(doc, "PRÉAMBULE", TxtPreambule.Text);

            // EXPOSÉ DES MOTIFS
            AddSection(doc, "EXPOSÉ DES MOTIFS", TxtExposeMotifs.Text);

            // RÉFÉRENCES RÉGLEMENTAIRES
            AddSection(doc, "RÉFÉRENCES RÉGLEMENTAIRES", TxtReferencesReglementaires.Text);

            // RÉFÉRENCES INTERNES
            AddSection(doc, "RÉFÉRENCES INTERNES", TxtReferencesInternes.Text);

            // MODALITÉ D'AUGMENTATION
            AddSection(doc, "MODALITÉ D'AUGMENTATION DU CAPITAL SOCIAL", TxtModaliteAugmentation.Text);

            // Capital Table
            AddCapitalTable(doc);

            // Associés Table
            if (Model.Associes.Count > 0)
            {
                AddAssociesTable(doc);
            }

            // CONCLUSION
            AddSection(doc, "CONCLUSION", TxtConclusion.Text);

            // Signature
            var signaturePara = new Paragraph(new Run("Signature et cachet"))
            {
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 40, 0, 0),
                FontSize = 10
            };
            doc.Blocks.Add(signaturePara);

            // Footer
            var footerPara = new Paragraph(new Run(Model.CabinetName ?? ""))
            {
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 40, 0, 0),
                FontSize = 9,
                Foreground = Brushes.Gray
            };
            doc.Blocks.Add(footerPara);

            return doc;
        }

        private void AddSection(FlowDocument doc, string title, string content)
        {
            var titlePara = new Paragraph(new Bold(new Run(title)))
            {
                Margin = new Thickness(0, 15, 0, 5),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 58, 138))
            };
            doc.Blocks.Add(titlePara);

            var contentPara = new Paragraph(new Run(content))
            {
                Margin = new Thickness(0, 0, 0, 10),
                TextAlignment = TextAlignment.Justify,
                FontSize = 10,
                LineHeight = 1.5
            };
            doc.Blocks.Add(contentPara);
        }

        private void AddCapitalTable(FlowDocument doc)
        {
            var sectionTitle = new Paragraph(new Bold(new Run("AUGMENTATION DU CAPITAL SOCIAL PAR CAPITALISATION DU COMPTE COURANT")))
            {
                Margin = new Thickness(0, 15, 0, 10),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 58, 138))
            };
            doc.Blocks.Add(sectionTitle);

            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 20)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(180) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });
            table.Columns.Add(new TableColumn { Width = new GridLength(140) });

            var rowGroup = new TableRowGroup();

            // Header
            var headerRow = new TableRow { Background = new SolidColorBrush(Color.FromRgb(243, 244, 246)) };
            AddTableCell(headerRow, "Libellé", true);
            AddTableCell(headerRow, "Passif avant", true);
            AddTableCell(headerRow, "Passif après", true);
            AddTableCell(headerRow, "Variation", true);
            rowGroup.Rows.Add(headerRow);

            // Data rows
            var row1 = new TableRow();
            AddTableCell(row1, "Capital social", false);
            AddTableCell(row1, $"{Model.CapitalActuel:N2} DA", false);
            AddTableCell(row1, $"{Model.CapitalNouveaux:N2} DA", false);
            AddTableCell(row1, $"{(Model.CapitalNouveaux - Model.CapitalActuel):N2} DA", false);
            rowGroup.Rows.Add(row1);

            var row2 = new TableRow();
            AddTableCell(row2, "Compte courant associés", false);
            AddTableCell(row2, TxtCompteAvant.Text + " DA", false);
            AddTableCell(row2, TxtCompteApres.Text + " DA", false);
            AddTableCell(row2, TxtVariationCompte.Text, false);
            rowGroup.Rows.Add(row2);

            table.RowGroups.Add(rowGroup);
            doc.Blocks.Add(table);
        }

        private void AddAssociesTable(FlowDocument doc)
        {
            var sectionTitle = new Paragraph(new Bold(new Run("LE DÉTAIL DE L'AUGMENTATION PAR ASSOCIÉS")))
            {
                Margin = new Thickness(0, 15, 0, 10),
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 58, 138))
            };
            doc.Blocks.Add(sectionTitle);

            var table = new Table
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(0, 0, 0, 20)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(100) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(105) });
            table.Columns.Add(new TableColumn { Width = new GridLength(80) });
            table.Columns.Add(new TableColumn { Width = new GridLength(105) });
            table.Columns.Add(new TableColumn { Width = new GridLength(70) });

            var rowGroup = new TableRowGroup();

            // Header
            var headerRow = new TableRow { Background = new SolidColorBrush(Color.FromRgb(243, 244, 246)) };
            AddTableCell(headerRow, "Nom", true);
            AddTableCell(headerRow, "Nbr parts avant", true);
            AddTableCell(headerRow, "Valeur avant", true);
            AddTableCell(headerRow, "Nbr parts après", true);
            AddTableCell(headerRow, "Valeur après", true);
            AddTableCell(headerRow, "Pourcentage", true);
            rowGroup.Rows.Add(headerRow);

            // Data rows
            foreach (var associe in Model.Associes)
            {
                var row = new TableRow();
                AddTableCell(row, associe.NomAssocie, false);
                AddTableCell(row, associe.NbPartsAvant.ToString(), false);
                AddTableCell(row, $"{associe.ValeurPartsAvant:N2} DA", false);
                AddTableCell(row, associe.NbPartsApres.ToString(), false);
                AddTableCell(row, $"{associe.ValeurPartsApres:N2} DA", false);
                AddTableCell(row, $"{associe.Pourcentage:N2}%", false);
                rowGroup.Rows.Add(row);
            }

            table.RowGroups.Add(rowGroup);
            doc.Blocks.Add(table);
        }

        private void AddTableCell(TableRow row, string text, bool isHeader)
        {
            var cell = new TableCell(new Paragraph(new Run(text)))
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Padding = new Thickness(6),
                FontSize = 9
            };

            if (isHeader)
            {
                cell.Blocks.Clear();
                cell.Blocks.Add(new Paragraph(new Bold(new Run(text))) { FontSize = 9, Margin = new Thickness(0) });
            }
            else
            {
                cell.Blocks.Clear();
                cell.Blocks.Add(new Paragraph(new Run(text)) { FontSize = 9, Margin = new Thickness(0) });
            }

            row.Cells.Add(cell);
        }
    }
}
