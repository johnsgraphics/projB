using System.Collections.Generic;

namespace CabinetDocProWpf.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Nif { get; set; } = string.Empty;
        public string Nis { get; set; } = string.Empty;
        public string Ai { get; set; } = string.Empty;
        public string Rc { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public static List<Client> GetSampleClients()
        {
            return new List<Client>
            {
                new Client
                {
                    Id = 1,
                    Name = "SARL TECH INNOVATIONS",
                    Address = "15 Rue Didouche Mourad, Alger",
                    Nif = "099916000123456",
                    Nis = "000116000123456",
                    Ai = "16000123456001",
                    Rc = "16/00-1234567",
                    Email = "contact@techinnovations.dz",
                    Phone = "+213 23 45 67 89"
                },
                new Client
                {
                    Id = 2,
                    Name = "EURL ALPHA SERVICES",
                    Address = "42 Boulevard Mohamed V, Oran",
                    Nif = "099931000234567",
                    Nis = "000131000234567",
                    Ai = "31000234567001",
                    Rc = "31/00-2345678",
                    Email = "info@alphaservices.dz",
                    Phone = "+213 41 12 34 56"
                }
            };
        }
    }
}
