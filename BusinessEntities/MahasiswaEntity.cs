using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class MahasiswaEntity
    {
        public int IdMahasiswa { get; set; }
        public string NamaMahasiswa { get; set; }
        public string Nim { get; set; }
        public int Angkatan { get; set; }
        public int Semester { get; set; }
        public string StatusMahasiswa { get; set; }
        public string Prodi { get; set; }
        public string AlamatTinggal { get; set; }
        public string Handphone { get; set; }
        public string DosenPembimbing { get; set; }


        //public virtual ICollection<EnrollEntity> EnrollEntity { get; set; }
    }
}
