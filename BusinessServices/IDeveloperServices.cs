using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public interface IDeveloperServices
    {
        string Authenticate(string email, string password);
        string CreateDev(DeveloperEntity developerEntity);
        string GetSecretKey(string appId);
        DeveloperEntity GetDeveloperById(string appId);
        DeveloperEntity RequestApikey(string email);
        bool UpdateDev(string email, DeveloperEntity developerEntity);
        bool DeleteDev(string email);
    }
}
