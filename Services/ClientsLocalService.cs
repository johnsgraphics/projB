using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using CabinetDocProWpf.Models;

namespace CabinetDocProWpf.Services
{
    /// <summary>
    /// Local JSON fallback persistence for clients.
    /// Thread-safe implementation for read/write operations.
    /// </summary>
    public class ClientsLocalService
    {
        private readonly string _appDataPath;
        private readonly string _clientsFilePath;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public ClientsLocalService()
        {
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "CabinetDocPro",
                "App_Data"
            );
            _clientsFilePath = Path.Combine(_appDataPath, "clients_local.json");

            Directory.CreateDirectory(_appDataPath);

            // Initialize with sample data if file doesn't exist
            if (!File.Exists(_clientsFilePath))
            {
                SaveClients(Client.GetSampleClients());
            }
        }

        public List<Client> LoadClients()
        {
            _lock.Wait();
            try
            {
                if (!File.Exists(_clientsFilePath))
                {
                    return new List<Client>();
                }

                string json = File.ReadAllText(_clientsFilePath);
                var clients = JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
                return clients;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading clients from JSON: {ex.Message}");
                return new List<Client>();
            }
            finally
            {
                _lock.Release();
            }
        }

        public void SaveClients(List<Client> clients)
        {
            _lock.Wait();
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(clients, options);
                File.WriteAllText(_clientsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving clients to JSON: {ex.Message}");
                throw;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void AddClient(Client client)
        {
            var clients = LoadClients();
            client.Id = clients.Any() ? clients.Max(c => c.Id) + 1 : 1;
            clients.Add(client);
            SaveClients(clients);
        }

        public void UpdateClient(Client client)
        {
            var clients = LoadClients();
            var existing = clients.FirstOrDefault(c => c.Id == client.Id);
            if (existing != null)
            {
                clients.Remove(existing);
                clients.Add(client);
                SaveClients(clients);
            }
        }

        public void DeleteClient(int clientId)
        {
            var clients = LoadClients();
            var existing = clients.FirstOrDefault(c => c.Id == clientId);
            if (existing != null)
            {
                clients.Remove(existing);
                SaveClients(clients);
            }
        }

        public Client? GetClientById(int id)
        {
            var clients = LoadClients();
            return clients.FirstOrDefault(c => c.Id == id);
        }
    }
}
