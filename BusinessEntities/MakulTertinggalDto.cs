using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class MakulTertinggalDto
    {
        //public int? IdMakul { get; set; }
        public int? IdMahasiswa { get; set; }
        public string NamaMahasiswa { get; set; }
        public int? MakulPilihan { get; set; }
        public List<MatakuliahEntity> Makul { get; set; }
        //public string KodeMakul { get; set; }
        //public string NamaMakul { get; set; }
        //public int? Sks { get; set; }
        //public string Sifat { get; set; }
        //public int? RekomendasiPengambilan { get; set; }
    }
}
