using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessEntities;

namespace BusinessServices
{
    public interface IMatakuliahServices
    {
        IEnumerable<string> GetAllPeriode();
        MatakuliahEntity GetMatakuliahById(int idMatakuliah);
        IEnumerable<EnrollEntity> GetEnrollMakulById(int idMakul, string periode);
        IEnumerable<MatakuliahEntity> GetAllMatakuliah(string dosen, string sifat, int? rekomendasi);
        IEnumerable<MakulTertinggalDto> GetMakulTertinggal(int? angkatan);
        IEnumerable<MatakuliahEntity> GetMakulMhsTertinggal(int nim);
        IEnumerable<MatakuliahEntity> GetJadwalMahasiswa(int nim);
        IEnumerable<MatakuliahEntity> GetAllJadwal();
        int CreateMatakuliah(MatakuliahEntity matakuliahEntity);
        bool UpdateMatakuliah(int idMatakuliah, MatakuliahEntity matakuliahEntity);
        bool DeleteMatakuliah(int idMatakuliah);
    }
}
