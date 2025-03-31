using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberClub.Infrastructure.Interfaces
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string name = "DefaultConnection");
    }
}
