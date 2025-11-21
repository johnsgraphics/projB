using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CabinetDocProWpf.Models;
using CabinetDocProWpf.Services;

namespace CabinetDocProWpf.Views.Pages
{
    public partial class ClientsPage : Page
    {
        private readonly ClientsLocalService _clientsService;
        private List<Client> _allClients;
        private ListView? _listView;

        public ClientsPage()
        {
            InitializeComponent();
            _clientsService = new ClientsLocalService();
            _allClients = new List<Client>();

            // Find the ListView inside ClientsList Border
            if (ClientsList?.Child is ListView lv)
            {
                _listView = lv;
            }

            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                _allClients = _clientsService.LoadClients();

                if (_allClients.Any())
                {
                    if (_listView != null)
                    {
                        _listView.ItemsSource = _allClients;
                    }
                    ClientsList.Visibility = Visibility.Visible;
                    EmptyState.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ClientsList.Visibility = Visibility.Collapsed;
                    EmptyState.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // DEFECT FIX 2: Working client creation flow (matches XAML event names)
        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            ShowClientDialog(null);
        }

        private void ShowClientDialog(Client? existingClient)
        {
            var dialog = new Window
            {
                Title = existingClient == null ? "Nouveau Client" : "Modifier Client",
                Width = 600,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };

            var dialogContent = CreateClientFormContent(existingClient, dialog);
            dialog.Content = dialogContent;

            dialog.ShowDialog();
        }

        private UIElement CreateClientFormContent(Client? existingClient, Window dialog)
        {
            var grid = new Grid { Margin = new Thickness(25, 25, 25, 25) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var formStack = new StackPanel();

            // Form fields
            var txtName = CreateFormField(formStack, "Nom du client *", existingClient?.Name ?? "");
            var txtAddress = CreateFormField(formStack, "Adresse *", existingClient?.Address ?? "");
            var txtRc = CreateFormField(formStack, "RC", existingClient?.Rc ?? "");
            var txtNif = CreateFormField(formStack, "NIF *", existingClient?.Nif ?? "");
            var txtNis = CreateFormField(formStack, "NIS", existingClient?.Nis ?? "");
            var txtAi = CreateFormField(formStack, "AI", existingClient?.Ai ?? "");
            var txtEmail = CreateFormField(formStack, "Email", existingClient?.Email ?? "");
            var txtPhone = CreateFormField(formStack, "Téléphone", existingClient?.Phone ?? "");

            scrollViewer.Content = formStack;
            Grid.SetRow(scrollViewer, 0);
            grid.Children.Add(scrollViewer);

            // Buttons
            var buttonStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 15, 0, 0)
            };
            Grid.SetRow(buttonStack, 1);

            var btnCancel = new Button
            {
                Content = "Annuler",
                Padding = new Thickness(16, 9, 16, 9),
                Margin = new Thickness(0, 0, 10, 0),
                Style = (Style)FindResource("SecondaryButtonStyle")
            };
            btnCancel.Click += (s, e) => dialog.Close();

            var btnSave = new Button
            {
                Content = "Enregistrer",
                Padding = new Thickness(16, 9, 16, 9),
                Style = (Style)FindResource("PrimaryButtonStyle")
            };
            btnSave.Click += (s, e) =>
            {
                if (!ValidateClientForm(txtName.Text, txtAddress.Text, txtNif.Text))
                    return;

                try
                {
                    var client = existingClient ?? new Client();
                    client.Name = txtName.Text.Trim();
                    client.Address = txtAddress.Text.Trim();
                    client.Rc = txtRc.Text.Trim();
                    client.Nif = txtNif.Text.Trim();
                    client.Nis = txtNis.Text.Trim();
                    client.Ai = txtAi.Text.Trim();
                    client.Email = txtEmail.Text.Trim();
                    client.Phone = txtPhone.Text.Trim();

                    if (existingClient == null)
                    {
                        _clientsService.AddClient(client);
                        MessageBox.Show("Client ajouté avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _clientsService.UpdateClient(client);
                        MessageBox.Show("Client modifié avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    LoadClients(); // Refresh list
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            buttonStack.Children.Add(btnCancel);
            buttonStack.Children.Add(btnSave);
            grid.Children.Add(buttonStack);

            return grid;
        }

        private TextBox CreateFormField(StackPanel parent, string label, string defaultValue)
        {
            var labelText = new TextBlock
            {
                Text = label,
                FontSize = 13,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E5E7EB")!,
                BorderThickness = new Thickness(1, 1, 1, 1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 15)
            };

            var textBox = new TextBox
            {
                Text = defaultValue,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Background = Brushes.Transparent,
                FontSize = 13
            };

            border.Child = textBox;
            parent.Children.Add(labelText);
            parent.Children.Add(border);

            return textBox;
        }

        private bool ValidateClientForm(string name, string address, string nif)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Le nom du client est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("L'adresse est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(nif))
            {
                MessageBox.Show("Le NIF est obligatoire!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
