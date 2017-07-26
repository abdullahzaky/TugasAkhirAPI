using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public interface IEnrollServices
    {
        EnrollEntity GetEnrollById(int idEnroll);
        IEnumerable<EnrollEntity> GetAllEnroll();
        IEnumerable<EnrollDetailDTO> GetEnrollDetailById(int nim);
        IEnumerable<EnrollDetailDTO> GetEnrollDetailMahasiswa(int? nim, string periode, int? angkatan);
        IEnumerable<EnrollPeriodSummaryDTO> GetMahasiswaSummary(string periode, int? angkatan);
        EnrollPeriodSummaryDTO GetEnrollSummaryById(int nim);
        IEnumerable<MahasiswaKritisDto> GetMahasiswaKritisDuaTahun();
        MahasiswaKritisDto GetMahasiswaKritisDuaTahunById(int id);
        IEnumerable<MahasiswaKritisDto> GetAllMahasiswaAkhirKritis();
        MahasiswaKritisDto GetMahasiswaAkhirKritisById(int id);
        int CreateEnroll(EnrollEntity enrollEntity);
        bool UpdateEnroll(int idEnroll, EnrollEntity enrollEntity);
        bool DeleteEnroll(int idEnroll);
    }
}
