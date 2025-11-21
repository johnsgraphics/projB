using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using CabinetDocProWpf.Models;
using CabinetDocProWpf.Services;

namespace CabinetDocProWpf.Views.Pages
{
    public partial class SettingsPage : Page
    {
        private readonly SettingsService _settingsService;
        private FirmSettings _currentSettings = null!;
        private string _selectedLogoPath = "";

        public SettingsPage()
        {
            InitializeComponent();
            _settingsService = new SettingsService();
            LoadSettings();
        }

        // DEFECT FIX 1: Smooth mouse wheel scrolling with animation
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MainScrollViewer == null) return;

            e.Handled = true;

            double currentOffset = MainScrollViewer.VerticalOffset;
            double delta = e.Delta > 0 ? -120 : 120; // Scroll amount per notch
            double targetOffset = Math.Max(0, Math.Min(MainScrollViewer.ScrollableHeight, currentOffset + delta));

            var animation = new DoubleAnimation
            {
                From = currentOffset,
                To = targetOffset,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, MainScrollViewer);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehavior.VerticalOffsetProperty));

            storyboard.Begin();
        }

        private void LoadSettings()
        {
            _currentSettings = _settingsService.GetFirmSettings();
            // Load data into UI
            TxtCabinetName.Text = _currentSettings.CabinetName ?? "";
            TxtAccountantName.Text = _currentSettings.AccountantName ?? "";
            TxtAddress.Text = _currentSettings.Address ?? "";
            TxtAgrement.Text = _currentSettings.Agrement ?? "";
            TxtNif.Text = _currentSettings.Nif ?? "";
            TxtNis.Text = _currentSettings.Nis ?? "";
            TxtAi.Text = _currentSettings.Ai ?? "";
            TxtBankName.Text = _currentSettings.BankName ?? "";
            TxtBankAccount.Text = _currentSettings.BankAccount ?? "";
            TxtBankAddress.Text = _currentSettings.BankAddress ?? "";

            // Load logo if exists
            if (!string.IsNullOrEmpty(_currentSettings.LogoPath) && File.Exists(_currentSettings.LogoPath))
            {
                LoadLogoImage(_currentSettings.LogoPath);
            }
        }

        private void BtnTabFirm_Click(object sender, RoutedEventArgs e)
        {
            BtnTabFirm.Style = (Style)FindResource("PrimaryButtonStyle");
            BtnTabThemes.Style = (Style)FindResource("SecondaryButtonStyle");
            BtnTabPreferences.Style = (Style)FindResource("SecondaryButtonStyle");
            FirmInfoTab.Visibility = Visibility.Visible;
            ThemesTab.Visibility = Visibility.Collapsed;
            PreferencesTab.Visibility = Visibility.Collapsed;
        }

        private void BtnTabThemes_Click(object sender, RoutedEventArgs e)
        {
            BtnTabFirm.Style = (Style)FindResource("SecondaryButtonStyle");
            BtnTabThemes.Style = (Style)FindResource("PrimaryButtonStyle");
            BtnTabPreferences.Style = (Style)FindResource("SecondaryButtonStyle");
            FirmInfoTab.Visibility = Visibility.Collapsed;
            ThemesTab.Visibility = Visibility.Visible;
            PreferencesTab.Visibility = Visibility.Collapsed;
        }

        private void BtnTabPreferences_Click(object sender, RoutedEventArgs e)
        {
            BtnTabFirm.Style = (Style)FindResource("SecondaryButtonStyle");
            BtnTabThemes.Style = (Style)FindResource("SecondaryButtonStyle");
            BtnTabPreferences.Style = (Style)FindResource("PrimaryButtonStyle");
            FirmInfoTab.Visibility = Visibility.Collapsed;
            ThemesTab.Visibility = Visibility.Collapsed;
            PreferencesTab.Visibility = Visibility.Visible;
        }

        private void BtnUploadLogo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Sélectionner le logo du cabinet",
                Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Tous les fichiers (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string selectedFile = openFileDialog.FileName;
                    // Copy to app directory
                    string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CabinetDocPro");
                    Directory.CreateDirectory(appDataPath);
                    string fileName = "cabinet_logo" + Path.GetExtension(selectedFile);
                    string destPath = Path.Combine(appDataPath, fileName);
                    File.Copy(selectedFile, destPath, true);

                    _selectedLogoPath = destPath;
                    LoadLogoImage(destPath);
                    TxtNoLogo.Visibility = Visibility.Collapsed;
                    ImgLogo.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors du chargement du logo: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadLogoImage(string path)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImgLogo.Source = bitmap;
                ImgLogo.Visibility = Visibility.Visible;
                TxtNoLogo.Visibility = Visibility.Collapsed;
                _selectedLogoPath = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'image: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteLogo_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer le logo?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ImgLogo.Source = null;
                ImgLogo.Visibility = Visibility.Collapsed;
                TxtNoLogo.Visibility = Visibility.Visible;
                _selectedLogoPath = "";
            }
        }

        private void BtnSaveFirmInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFirmInfo())
                return;

            try
            {
                _currentSettings.CabinetName = TxtCabinetName.Text;
                _currentSettings.AccountantName = TxtAccountantName.Text;
                _currentSettings.Address = TxtAddress.Text;
                _currentSettings.Agrement = TxtAgrement.Text;
                _currentSettings.Nif = TxtNif.Text;
                _currentSettings.Nis = TxtNis.Text;
                _currentSettings.Ai = TxtAi.Text;
                _currentSettings.BankName = TxtBankName.Text;
                _currentSettings.BankAccount = TxtBankAccount.Text;
                _currentSettings.BankAddress = TxtBankAddress.Text;
                _currentSettings.LogoPath = _selectedLogoPath;
                _currentSettings.UpdatedAt = DateTime.Now;

                _settingsService.SaveFirmSettings(_currentSettings);

                MessageBox.Show("Les informations du cabinet ont été enregistrées avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateFirmInfo()
        {
            if (string.IsNullOrWhiteSpace(TxtAccountantName.Text))
            {
                MessageBox.Show("Le nom du comptable est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtAccountantName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtAddress.Text))
            {
                MessageBox.Show("L'adresse est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtAddress.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtAgrement.Text))
            {
                MessageBox.Show("L'agrément est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtAgrement.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNif.Text))
            {
                MessageBox.Show("Le NIF est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNif.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNis.Text))
            {
                MessageBox.Show("Le NIS est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNis.Focus();
                return false;
            }

            return true;
        }
    }

    // Helper class to enable animating ScrollViewer vertical offset
    public static class ScrollViewerBehavior
    {
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.RegisterAttached("VerticalOffset", typeof(double), typeof(ScrollViewerBehavior),
                new PropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static double GetVerticalOffset(DependencyObject obj)
        {
            return (double)obj.GetValue(VerticalOffsetProperty);
        }

        public static void SetVerticalOffset(DependencyObject obj, double value)
        {
            obj.SetValue(VerticalOffsetProperty, value);
        }

        private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }
    }
}
