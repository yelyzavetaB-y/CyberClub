using CyberClub.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Infrastructure.DBService
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string name = "DefaultConnection")
        {
            return _configuration.GetConnectionString(name);
        }
    }
}
