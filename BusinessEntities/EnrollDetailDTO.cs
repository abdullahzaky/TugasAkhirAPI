using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace BusinessEntities
{
    public class EnrollDetailDTO
    {
        public int IdEnroll { get; set; }
        public string KodeMakul { get; set; }
        public string NamaMakul { get; set; }
        public int? Sks { get; set; }
        public string Sifat { get; set; }
        public int IdMahasiswa { get; set; }
        public string NamaMahasiswa { get; set; }
        public double? NilaiTotal { get; set; }
        public string GradeNilai { get; set; }
        public string PeriodeEnroll { get; set; }
        public int? Kehadiran { get; set; }
        public int? Pertemuan { get; set; }

        //public virtual MataKuliah MatakuliahEnroll { get; set; }
        //public virtual Mahasiswa MahasiswaEnroll { get; set; }
    }
}
