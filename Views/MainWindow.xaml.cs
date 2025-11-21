// filename: F:\CabinetDoc Pro\Views\MainWindow.xaml.cs

using CabinetDoc_Pro.Views.Pages;
using CabinetDocProWpf.Views.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CabinetDocProWpf.Views
{
    public partial class MainWindow : Window
    {
        private bool isCollapsed = false;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DashboardPage());
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            // Create fade animation for smooth visual effect
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            if (isCollapsed)
            {
                // Expand sidebar to 240px
                SidebarColumn.Width = new GridLength(240);

                // Show text labels with fade-in
                opacityAnimation.From = 0;
                opacityAnimation.To = 1;
                TxtDashboard.Visibility = Visibility.Visible;
                TxtCreate.Visibility = Visibility.Visible;
                TxtClients.Visibility = Visibility.Visible;
                TxtDocuments.Visibility = Visibility.Visible;
                TxtSettings.Visibility = Visibility.Visible;
                AppNamePanel.Visibility = Visibility.Visible;

                TxtDashboard.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtCreate.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtClients.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtDocuments.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtSettings.BeginAnimation(OpacityProperty, opacityAnimation);
                AppNamePanel.BeginAnimation(OpacityProperty, opacityAnimation);

                isCollapsed = false;
            }
            else
            {
                // Collapse sidebar to 64px (icon-only mode)
                SidebarColumn.Width = new GridLength(64);

                // Hide text labels with fade-out
                opacityAnimation.From = 1;
                opacityAnimation.To = 0;
                opacityAnimation.Completed += (s, args) =>
                {
                    TxtDashboard.Visibility = Visibility.Collapsed;
                    TxtCreate.Visibility = Visibility.Collapsed;
                    TxtClients.Visibility = Visibility.Collapsed;
                    TxtDocuments.Visibility = Visibility.Collapsed;
                    TxtSettings.Visibility = Visibility.Collapsed;
                    AppNamePanel.Visibility = Visibility.Collapsed;
                };

                TxtDashboard.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtCreate.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtClients.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtDocuments.BeginAnimation(OpacityProperty, opacityAnimation);
                TxtSettings.BeginAnimation(OpacityProperty, opacityAnimation);
                AppNamePanel.BeginAnimation(OpacityProperty, opacityAnimation);

                isCollapsed = true;
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Remove 'Active' tag from all buttons
            BtnDashboard.Tag = null;
            BtnCreate.Tag = null;
            BtnClients.Tag = null;
            BtnDocuments.Tag = null;
            BtnSettings.Tag = null;

            // Set the clicked button as active
            activeButton.Tag = "Active";
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnDashboard);
            MainFrame.Navigate(new DashboardPage());
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Toggle dropdown menu
            CreateDropdown.IsOpen = !CreateDropdown.IsOpen;
        }

        private void BtnNoteHonoraire_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnCreate);
            CreateDropdown.IsOpen = false;
            MainFrame.Navigate(new NoteHonorairePage());
        }

        private void BtnRapportSpecial_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnCreate);
            CreateDropdown.IsOpen = false;
            MainFrame.Navigate(new RapportSpecialPage());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnClients);
            MainFrame.Navigate(new ClientsPage());
        }

        private void BtnDocuments_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnDocuments);
            MainFrame.Navigate(new DashboardPage()); // Navigate to dashboard since DocumentPreview is removed
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnSettings);
            MainFrame.Navigate(new SettingsPage());
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonctionnalité d'exportation en cours de développement", "Exporter", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            // Open the dropdown when clicking header "Nouveau" button
            CreateDropdown.IsOpen = true;
        }
    }
}
