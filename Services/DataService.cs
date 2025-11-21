using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CabinetDocProWpf.Models;

namespace CabinetDocProWpf.Services
{
    public class DataService
    {
        private static readonly string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CabinetDocPro");
        private static readonly string ClientsFile = Path.Combine(DataFolder, "clients.json");
        private static readonly string DocumentsFile = Path.Combine(DataFolder, "documents.json");

        public DataService()
        {
            if (!Directory.Exists(DataFolder))
                Directory.CreateDirectory(DataFolder);
        }

        // Clients
        public List<Client> GetClients()
        {
            if (!File.Exists(ClientsFile)) return new List<Client>();
            var json = File.ReadAllText(ClientsFile);
            return JsonSerializer.Deserialize<List<Client>>(json) ?? new List<Client>();
        }

        public void SaveClients(List<Client> clients)
        {
            var json = JsonSerializer.Serialize(clients, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ClientsFile, json);
        }

        public void AddClient(Client client)
        {
            var clients = GetClients();
            clients.Add(client);
            SaveClients(clients);
        }

        public void UpdateClient(Client client)
        {
            var clients = GetClients();
            var index = clients.FindIndex(c => c.Id == client.Id);
            if (index >= 0)
            {
                clients[index] = client;
                SaveClients(clients);
            }
        }

        public void DeleteClient(string id)
        {
            var clients = GetClients();
            clients.RemoveAll(c => c.Id.Equals(id));
            SaveClients(clients);
        }

        // Documents
        public List<DocumentModel> GetDocuments()
        {
            if (!File.Exists(DocumentsFile)) return new List<DocumentModel>();
            var json = File.ReadAllText(DocumentsFile);
            return JsonSerializer.Deserialize<List<DocumentModel>>(json) ?? new List<DocumentModel>();
        }

        public void SaveDocuments(List<DocumentModel> documents)
        {
            var json = JsonSerializer.Serialize(documents, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DocumentsFile, json);
        }

        public void AddDocument(DocumentModel document)
        {
            var documents = GetDocuments();
            documents.Add(document);
            SaveDocuments(documents);
        }

        public string GenerateDocumentNumber(string type)
        {
            var documents = GetDocuments();
            var year = DateTime.Now.Year;
            var suffix = type == "NOTE D'HONORAIRES" ? "HN" : "RP";
            var count = documents.Count(d => d.Type.Equals(type) && d.CreatedAt.Year == year) + 1;
            return $"{count:D2}/{year}/{suffix}";
        }
    }
}
