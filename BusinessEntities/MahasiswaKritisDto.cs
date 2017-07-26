using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class MahasiswaKritisDto
    {
        public int? IdMahasiswa { get; set; }
        //public string NamaMahasiswa { get; set; }
        public int? Sks { get; set; }
        public float? Ipk { get; set; }
        public int? Angkatan { get; set; }
        public string Status { get; set; }
        public bool Kelolosan { get; set; }
    }
}
