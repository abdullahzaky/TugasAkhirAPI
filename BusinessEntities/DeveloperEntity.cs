using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class DeveloperEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string AppId { get; set; }
        public byte[] SecretKey { get; set; }
    }
}
