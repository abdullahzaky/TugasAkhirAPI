//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Enroll
    {
        public int IdEnroll { get; set; }
        public int IdMakul { get; set; }
        public int IdMahasiswa { get; set; }
        public Nullable<double> NilaiTotal { get; set; }
        public string GradeNilai { get; set; }
        public string PeriodeEnroll { get; set; }
        public Nullable<int> Kehadiran { get; set; }
        public Nullable<int> Pertemuan { get; set; }
    
        public virtual Mahasiswa Mahasiswa { get; set; }
        public virtual MataKuliah MataKuliah { get; set; }
    }
}