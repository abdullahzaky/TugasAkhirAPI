using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class EnrollEntity
    {
        public int IdEnroll { get; set; }
        public int IdMakul { get; set; }
        public int IdMahasiswa { get; set; }
        public float NilaiTotal { get; set; }
        public string GradeNilai { get; set; }
        public string PeriodeEnroll { get; set; }
        public int Kehadiran { get; set; }
        public int Pertemuan { get; set; }

        //public virtual MahasiswaEntity Mahasiswa { get; set; }
        //public virtual MatakuliahEntity Matakuliah { get; set; }
    }
}
