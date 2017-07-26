using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class EnrollPeriodSummaryDTO
    {
        public int? IdMahasiswa { get; set; }
        public string NamaMahasiswa { get; set; }
        public float? Ipk { get; set; }
        public int? Sks { get; set; }
        public List<EnrollIpsSummaryDto> EnrollIps { get; set; }
    }
}
