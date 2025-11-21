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
        private List<Client> Clients;

        public ClientsPage()
        {
            InitializeComponent();
            _clientsService = new ClientsLocalService();
            Clients = new List<Client>();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                Clients = _clientsService.LoadClients();
                if (Clients.Count > 0)
                {
                    ClientsList.Visibility = Visibility.Visible;
                    EmptyState.Visibility = Visibility.Collapsed;
                    ListClients.ItemsSource = Clients;
                }
                else
                {
                    ClientsList.Visibility = Visibility.Collapsed;
                    EmptyState.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditClient_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var client = btn?.DataContext as Client;
            if (client != null)
            {
                ShowClientDialog(client);
            }
        }

        private void DeleteClient_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var client = btn?.DataContext as Client;
            if (client != null)
            {
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le client '{client.Name}'?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _clientsService.DeleteClient(client);
                        MessageBox.Show("Client supprimé avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadClients();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            ShowClientDialog(null);
        }

        private void ShowClientDialog(Client? clientToEdit)
        {
            var dialog = new Window
            {
                Title = clientToEdit == null ? "Nouveau Client" : "Modifier Client",
                Width = 600,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                ResizeMode = ResizeMode.NoResize
            };
            var dialogContent = CreateClientFormContent(clientToEdit, dialog);
            dialog.Content = dialogContent;
            dialog.ShowDialog();
        }

        private UIElement CreateClientFormContent(Client? client, Window dialog)
        {
            var grid = new Grid { Margin = new Thickness(25) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            var scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            var formStack = new StackPanel();
            var txtName = CreateFormField(formStack, "Nom du client *", client?.Name ?? "");
            var txtAddress = CreateFormField(formStack, "Adresse *", client?.Address ?? "");
            var txtRc = CreateFormField(formStack, "RC", client?.Rc ?? "");
            var txtNif = CreateFormField(formStack, "NIF *", client?.Nif ?? "");
            var txtNis = CreateFormField(formStack, "NIS", client?.Nis ?? "");
            var txtAi = CreateFormField(formStack, "AI", client?.Ai ?? "");
            var txtEmail = CreateFormField(formStack, "Email", client?.Email ?? "");
            var txtPhone = CreateFormField(formStack, "Téléphone", client?.Phone ?? "");
            scrollViewer.Content = formStack;
            Grid.SetRow(scrollViewer, 0);
            grid.Children.Add(scrollViewer);
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
                    var clientObj = client ?? new Client();
                    clientObj.Name = txtName.Text.Trim();
                    clientObj.Address = txtAddress.Text.Trim();
                    clientObj.Rc = txtRc.Text.Trim();
                    clientObj.Nif = txtNif.Text.Trim();
                    clientObj.Nis = txtNis.Text.Trim();
                    clientObj.Ai = txtAi.Text.Trim();
                    clientObj.Email = txtEmail.Text.Trim();
                    clientObj.Phone = txtPhone.Text.Trim();
                    if (client == null)
                    {
                        _clientsService.AddClient(clientObj);
                        MessageBox.Show("Client ajouté avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        _clientsService.UpdateClient(clientObj);
                        MessageBox.Show("Client modifié avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    LoadClients();
                    dialog.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'enregistrement: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            buttonStack.Children.Add(btnCancel);
            buttonStack.Children.Add(btnSave);
            grid.Children.Add(buttonStack);
            return grid;
        }

        private TextBox CreateFormField(StackPanel parent, string label, string defaultValue)
        {
            var labelText = new TextBlock {
                Text = label,
                FontSize = 13,
                Foreground = System.Windows.Media.Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 8)
            };
            var border = new Border {
                Background = System.Windows.Media.Brushes.White,
                BorderBrush = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom("#E5E7EB")!,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(12, 10, 12, 10),
                Margin = new Thickness(0, 0, 0, 15)
            };
            var textBox = new TextBox {
                Text = defaultValue,
                BorderThickness = new Thickness(0),
                Background = System.Windows.Media.Brushes.Transparent,
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
