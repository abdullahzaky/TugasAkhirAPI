using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class MatakuliahEntity
    {
        public int IdMakul { get; set; }
        public string KodeMakul { get; set; }
        public string NamaMakul { get; set; }
        public string Kelas { get; set; }
        public int? Sks { get; set; }
        public string Sifat { get; set; }
        public string Jadwal { get; set; }
        public int? RekomendasiPengambilan { get; set; }
        public string DosenPengajar { get; set; }

        //public virtual ICollection<EnrollEntity> EnrollEntity { get; set; }
    }
}
