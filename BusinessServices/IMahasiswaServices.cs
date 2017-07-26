using System.Collections.Generic;
using BusinessEntities;

namespace BusinessServices
{
    public interface IMahasiswaServices
    {
        MahasiswaEntity GetMahasiswaById(int idMahasiswa);
        IEnumerable<MahasiswaEntity> GetAllMahasiswa(int? angkatan, string status, string prodi, string dosbing);
        IEnumerable<EnrollEntity> GetNilai(int? nim, string periodeEnroll, int? idMakul, int? angkatan);
        IEnumerable<EnrollEntity> GetNilaiMahasiswa(int idMahasiswa);
        IEnumerable<EnrollDetailDTO> GetNilaiPraktikumMhs(int idMahasiswa);
        int CreateMahasiswa(MahasiswaEntity mahasiswaEntity);
        bool UpdateMahasiswa(int idMahasiswa, MahasiswaEntity mahasiswaEntity);
        bool DeleteMahasiswa(int idMahasiswa);
    }
}
