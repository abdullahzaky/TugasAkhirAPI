using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;

namespace BusinessServices
{
    /// <summary>
    /// Offers services for Mahasiswa
    /// </summary>
    public class MahasiswaServices : IMahasiswaServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public MahasiswaServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches product details by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public MahasiswaEntity GetMahasiswaById(int idMahasiswa)
        {
            var mahasiswa = _unitOfWork.MahasiswaRepository.GetByID(idMahasiswa);
            if (mahasiswa != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Mahasiswa, MahasiswaEntity>(); });
                var mahasiswaModel = Mapper.Map<Mahasiswa, MahasiswaEntity>(mahasiswa);
                return mahasiswaModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the mahasiswas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MahasiswaEntity> GetAllMahasiswa(int? angkatan, string status, string prodi, string dosbing)
        {
            var mahasiswas = _unitOfWork.MahasiswaRepository.GetMany(x => (angkatan == null || x.Angkatan.Equals(angkatan))
            && (status == null || x.StatusMahasiswa.Equals(status)) && (prodi == null || x.Prodi.Equals(prodi))
            && (dosbing == null || x.DosenPembimbing.Equals(dosbing))).ToList();
            if (mahasiswas.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Mahasiswa, MahasiswaEntity>(); });
                var mahasiswasModel = Mapper.Map<List<Mahasiswa>, List<MahasiswaEntity>>(mahasiswas);
                return mahasiswasModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all nilai dan kehadiran dalam enroll di setiap makul atau periode.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnrollEntity> GetNilai(int? nim, string periodeEnroll, int? idMakul, int? angkatan)
        {
            var nilai = _unitOfWork.EnrollRepository.GetMany(x => (nim == null || x.Mahasiswa.Nim.Equals(nim))
            && (angkatan == null || x.Mahasiswa.Angkatan.Equals(angkatan)) && (periodeEnroll == null || x.PeriodeEnroll.Equals(periodeEnroll)
            && (idMakul == null || x.IdMakul.Equals(idMakul)))).OrderBy(x=>x.PeriodeEnroll).ThenBy(x=>x.IdMakul).ToList();
            if (nilai.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, EnrollEntity>(); });
                var mahasiswasModel = Mapper.Map<List<Enroll>, List<EnrollEntity>>(nilai);
                return mahasiswasModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all nilai dan kehadiran dalam enroll di setiap makul atau periode.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnrollEntity> GetNilaiMahasiswa(int idMahasiswa)
        {
            var nilaiMahasiswa = _unitOfWork.EnrollRepository.GetMany(x => x.IdMahasiswa.Equals(idMahasiswa))
                .OrderBy(x => x.PeriodeEnroll).ThenBy(x => x.IdMakul).ToList();
            if (nilaiMahasiswa.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, EnrollEntity>(); });
                var mahasiswasModel = Mapper.Map<List<Enroll>, List<EnrollEntity>>(nilaiMahasiswa);
                return mahasiswasModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches nilai praktikum
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnrollDetailDTO> GetNilaiPraktikumMhs(int idMahasiswa)
        {
            var nilaiPraktMahasiswa = _unitOfWork.EnrollRepository.GetMany(x => x.IdMahasiswa.Equals(idMahasiswa) && x.MataKuliah.NamaMakul.StartsWith("Prakt."))
                .OrderBy(x => x.PeriodeEnroll).ThenBy(x => x.IdMakul).ToList();
            if (nilaiPraktMahasiswa.Any())
            {
                //var daftarPraktikum = _unitOfWork.MatakuliahRepository.GetMany(x => (x.RekomendasiPengambilan <= nilaiPraktMahasiswa.Select(y => y.Mahasiswa.Semester).First())).ToList();
                List<EnrollDetailDTO> listEnrollDetail = new List<EnrollDetailDTO>();
                foreach (var b in nilaiPraktMahasiswa)
                {
                    var enrollDetail = new EnrollDetailDTO()
                    {
                        IdEnroll = b.IdEnroll,
                        KodeMakul = b.MataKuliah.KodeMakul,
                        NamaMakul = b.MataKuliah.NamaMakul,
                        Sks = b.MataKuliah.Sks,
                        Sifat = b.MataKuliah.Sifat,
                        IdMahasiswa = b.Mahasiswa.IdMahasiswa,
                        NamaMahasiswa = b.Mahasiswa.NamaMahasiswa,
                        NilaiTotal = (float)b.NilaiTotal,
                        GradeNilai = b.GradeNilai,
                        PeriodeEnroll = b.PeriodeEnroll,
                        Kehadiran = b.Kehadiran,
                        Pertemuan = b.Pertemuan
                    };
                    listEnrollDetail.Add(enrollDetail);
                }
                return listEnrollDetail;
            }
            return null;
        }

        /// <summary>
        /// Creates a mahasiswa
        /// </summary>
        /// <param name="mahasiswaEntity"></param>
        /// <returns></returns>
        public int CreateMahasiswa(MahasiswaEntity mahasiswaEntity)
        {
            using (var scope = new TransactionScope())
            {
                var mahasiswa = new Mahasiswa
                {
                    NamaMahasiswa = mahasiswaEntity.NamaMahasiswa,
                    Nim = mahasiswaEntity.Nim,
                    Angkatan = mahasiswaEntity.Angkatan,
                    StatusMahasiswa = mahasiswaEntity.StatusMahasiswa,
                    Prodi = mahasiswaEntity.Prodi,
                    AlamatTinggal = mahasiswaEntity.AlamatTinggal,
                    Handphone = mahasiswaEntity.Handphone,
                    DosenPembimbing = mahasiswaEntity.DosenPembimbing
                };
                _unitOfWork.MahasiswaRepository.Insert(mahasiswa);
                _unitOfWork.Save();
                scope.Complete();
                return mahasiswa.IdMahasiswa;
            }
        }

        /// <summary>
        /// Updates a mahasiswa
        /// </summary>
        /// <param name="idMahasiswa"></param>
        /// <param name="mahasiswaEntity"></param>
        /// <returns></returns>
        public bool UpdateMahasiswa (int idMahasiswa, MahasiswaEntity mahasiswaEntity)
        {
            var success = false;
            if (mahasiswaEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var mahasiswa = _unitOfWork.MahasiswaRepository.GetByID(idMahasiswa);
                    if (mahasiswa != null)
                    {
                        mahasiswa.NamaMahasiswa = mahasiswaEntity.NamaMahasiswa;
                        mahasiswa.Nim = mahasiswaEntity.Nim;
                        mahasiswa.Angkatan = mahasiswaEntity.Angkatan;
                        mahasiswa.StatusMahasiswa = mahasiswaEntity.StatusMahasiswa;
                        mahasiswa.Prodi = mahasiswaEntity.Prodi;
                        mahasiswa.AlamatTinggal = mahasiswaEntity.AlamatTinggal;
                        mahasiswa.Handphone = mahasiswaEntity.Handphone;
                        mahasiswa.DosenPembimbing = mahasiswaEntity.DosenPembimbing;
                        _unitOfWork.MahasiswaRepository.Update(mahasiswa);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public bool DeleteMahasiswa(int idMahasiswa)
        {
            var success = false;
            if (idMahasiswa > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var mahasiswa = _unitOfWork.MahasiswaRepository.GetByID(idMahasiswa);
                    if (mahasiswa != null)
                    {

                        _unitOfWork.MahasiswaRepository.Delete(mahasiswa);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
    }
}