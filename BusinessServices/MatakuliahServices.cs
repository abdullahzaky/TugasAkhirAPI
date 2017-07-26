using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;
using System;

namespace BusinessServices
{
    public class MatakuliahServices : IMatakuliahServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public MatakuliahServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches matakuliah details by id
        /// </summary>
        /// <param name="idMatakuliah"></param>
        /// <returns></returns>
        public MatakuliahEntity GetMatakuliahById(int idMatakuliah)
        {
            var matakuliah = _unitOfWork.MatakuliahRepository.GetByID(idMatakuliah);
            if (matakuliah != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<MataKuliah, MatakuliahEntity>(); });
                var matakuliahModel = Mapper.Map<MataKuliah, MatakuliahEntity>(matakuliah);
                return matakuliahModel;
            }
            return null;
        }

        public IEnumerable<EnrollEntity> GetEnrollMakulById(int idMakul, string periode)
        {
            var enrollByIdMakul = _unitOfWork.EnrollRepository.GetMany(x => (x.IdMakul == idMakul)
            && (periode == null || x.PeriodeEnroll.Equals(periode))).ToList();
            if (enrollByIdMakul != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, EnrollEntity>(); });
                var enrollModel = Mapper.Map<List<Enroll>, List<EnrollEntity>>(enrollByIdMakul);
                return enrollModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the matakuliahs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MatakuliahEntity> GetAllMatakuliah(string dosen, string sifat, int? rekomendasi)
        {
            var matakuliahs = _unitOfWork.MatakuliahRepository.GetMany(x => (dosen == null || x.DosenPengajar.Equals(dosen)) &&
            (sifat == null || x.Sifat.Equals(sifat)) && (rekomendasi == null || x.RekomendasiPengambilan.Equals(rekomendasi))).ToList();
            if (matakuliahs.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<MataKuliah, MatakuliahEntity>(); });
                var matakuliahsModel = Mapper.Map<List<MataKuliah>, List<MatakuliahEntity>>(matakuliahs);
                return matakuliahsModel;
            }
            return null;
        }


        /// <summary>
        /// Fetches periode enroll
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllPeriode()
        {
            var matakuliahPeriode = _unitOfWork.EnrollRepository.GetAll().GroupBy(x=>x.PeriodeEnroll).ToList();
            if (matakuliahPeriode.Any())
            {
                List<string> listPeriode = new List<string>();
                foreach (var row in matakuliahPeriode)
                {
                    var periode = row.Select(x => x.PeriodeEnroll).First();
                    listPeriode.Add(periode);
                }
                return listPeriode;
                //Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, PeriodeDto>(); });
                //var matakuliahsModel = Mapper.Map<List<Enroll>, List<PeriodeDto>>(matakuliahPeriode);
                //return matakuliahsModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all mhs matakuliah tertinggal.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MakulTertinggalDto> GetMakulTertinggal(int? angkatan)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                                  .GetMany(x => (angkatan == null || x.Mahasiswa.Angkatan.Equals(angkatan))
                                  && (x.GradeNilai != "K" || x.GradeNilai != " " || x.GradeNilai != null))
                                  .GroupBy(x => new { x.IdMahasiswa, x.MataKuliah.KodeMakul }).Select(x => x.OrderBy(p => p.GradeNilai).First())
                                  .ToList();
            if (enrollMahasiswa.Any())
            {
                var groups = enrollMahasiswa.GroupBy(x => new { x.IdMahasiswa, x.Mahasiswa.NamaMahasiswa, x.Mahasiswa.Semester });
                List<MakulTertinggalDto> listMakulMhs = new List<MakulTertinggalDto>();
                foreach (var group in groups)
                {
                    var daftarMakulWajib = _unitOfWork.MatakuliahRepository
                        .GetMany(y => (y.Sifat == "Wajib" && y.RekomendasiPengambilan <= group.Key.Semester))
                        .GroupBy(x=>x.KodeMakul).Select(y=>y.First()).ToList();
                    int makulPilihanMahasiswa = 0;
                    foreach (var row in group)
                    {
                        daftarMakulWajib.RemoveAll(x => row.MataKuliah.KodeMakul.Contains(x.KodeMakul));
                        if (row.Mahasiswa.Semester > 5 && row.MataKuliah.Sifat == "Pilihan")
                        {
                            makulPilihanMahasiswa += 1;
                        }
                    }
                    //daftarMakulWajib.Where(x => !enrollMahasiswa.Select(y=>y.MataKuliah.KodeMakul).Contains(x.KodeMakul));
                    //var makulTertinggal = daftarMakulWajib.Select(k => k.NamaMakul).Except(group.Select(j => j.MataKuliah.NamaMakul));

                    if (daftarMakulWajib.Count != 0 || (makulPilihanMahasiswa < 4 && group.Key.Semester > 7))
                    {
                        var daftarMakul = new MakulTertinggalDto()
                        {
                            IdMahasiswa = group.Key.IdMahasiswa,
                            NamaMahasiswa = group.Key.NamaMahasiswa,
                            MakulPilihan = makulPilihanMahasiswa,
                            Makul = daftarMakulWajib.Select(r => new MatakuliahEntity()
                            {
                                IdMakul = r.IdMakul,
                                KodeMakul = r.KodeMakul,
                                NamaMakul = r.NamaMakul,
                                Kelas = r.Kelas,
                                Sks = r.Sks,
                                Sifat = r.Sifat,
                                Jadwal = r.Jadwal,
                                RekomendasiPengambilan = r.RekomendasiPengambilan,
                                DosenPengajar = r.DosenPengajar
                            }).ToList()
                        };
                        listMakulMhs.Add(daftarMakul);
                    }
                }
                return listMakulMhs;
            }
            return null;
        }

        /// <summary>
        /// Fetch matakuliah mahasiswa tertinggal.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MatakuliahEntity> GetMakulMhsTertinggal(int nim)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                                  .GetMany(x => (x.IdMahasiswa.Equals(nim)) && x.MataKuliah.Sifat == "Wajib" && (x.GradeNilai != "K" || x.GradeNilai != " "))
                                  .GroupBy(x => new { x.MataKuliah.KodeMakul }).Select(x => x.OrderBy(p => p.GradeNilai).First())
                                  .OrderBy(x => x.PeriodeEnroll).ToList();
            var daftarMakulWajib = _unitOfWork.MatakuliahRepository
                      .GetMany(y => (y.Sifat == "Wajib" && y.RekomendasiPengambilan <= enrollMahasiswa.Select(x=>x.Mahasiswa.Semester).First()))
                      .GroupBy(z=>z.KodeMakul).Select(k=>k.First()).ToList();
            var makulTertinggal = daftarMakulWajib.Where(i => !enrollMahasiswa.Select(x => x.MataKuliah.KodeMakul).Contains(i.KodeMakul)).ToList();

            if (makulTertinggal.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<MataKuliah, MatakuliahEntity>(); });
                var enrollsModel = Mapper.Map<List<MataKuliah>, List<MatakuliahEntity>>(makulTertinggal);
                return enrollsModel;
            }
            return null;
        }

        public IEnumerable<MatakuliahEntity> GetJadwalMahasiswa(int nim)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                                  .GetMany(x => x.IdMahasiswa == nim).GroupBy(x => x.PeriodeEnroll).Last()
                                  .ToList();
            var jadwalMakul = _unitOfWork.MatakuliahRepository.GetMany(x => enrollMahasiswa.Any(c => c.IdMakul.Equals(x.IdMakul))).ToList();
            

            if (jadwalMakul.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<MataKuliah, MatakuliahEntity>(); });
                var jadwalModel = Mapper.Map<List<MataKuliah>, List<MatakuliahEntity>>(jadwalMakul);
                return jadwalModel;
            }
            return null;
        }

        public IEnumerable<MatakuliahEntity> GetAllJadwal()
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                                  .GetMany(x=>x.PeriodeEnroll!="" || x.PeriodeEnroll==null).GroupBy(x => x.PeriodeEnroll).Last()
                                  .ToList();
            var jadwalMakul = _unitOfWork.MatakuliahRepository.GetMany(x => enrollMahasiswa.Any(c => c.IdMakul.Equals(x.IdMakul))).ToList();

            if (jadwalMakul.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<MataKuliah, MatakuliahEntity>(); });
                var jadwalModel = Mapper.Map<List<MataKuliah>, List<MatakuliahEntity>>(jadwalMakul);
                return jadwalModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a matakuliah
        /// </summary>
        /// <param name="matakuliahEntity"></param>
        /// <returns></returns>
        public int CreateMatakuliah(MatakuliahEntity matakuliahEntity)
        {
            using (var scope = new TransactionScope())
            {
                var matakuliah = new MataKuliah
                {
                    NamaMakul = matakuliahEntity.NamaMakul,
                    Kelas = matakuliahEntity.Kelas,
                    Sks = matakuliahEntity.Sks,
                    Sifat = matakuliahEntity.Sifat,
                    Jadwal = matakuliahEntity.Jadwal,
                    RekomendasiPengambilan = matakuliahEntity.RekomendasiPengambilan,
                    DosenPengajar = matakuliahEntity.DosenPengajar
            };
                _unitOfWork.MatakuliahRepository.Insert(matakuliah);
                _unitOfWork.Save();
                scope.Complete();
                return matakuliah.IdMakul;
            }
        }

        /// <summary>
        /// Updates a matakuliah
        /// </summary>
        /// <param name="idMatakuliah"></param>
        /// <param name="matakuliahEntity"></param>
        /// <returns></returns>
        public bool UpdateMatakuliah(int idMatakuliah, MatakuliahEntity matakuliahEntity)
        {
            var success = false;
            if (matakuliahEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var matakuliah = _unitOfWork.MatakuliahRepository.GetByID(idMatakuliah);
                    if (matakuliah != null)
                    {
                        matakuliah.NamaMakul = matakuliahEntity.NamaMakul;
                        matakuliah.Kelas = matakuliahEntity.Kelas;
                        matakuliah.Sks = matakuliahEntity.Sks;
                        matakuliah.Sifat = matakuliahEntity.Sifat;
                        matakuliah.Jadwal = matakuliahEntity.Jadwal;
                        matakuliah.RekomendasiPengambilan = matakuliahEntity.RekomendasiPengambilan;
                        matakuliah.DosenPengajar = matakuliahEntity.DosenPengajar;
                        _unitOfWork.MatakuliahRepository.Update(matakuliah);
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
        public bool DeleteMatakuliah(int idMatakuliah)
        {
            var success = false;
            if (idMatakuliah > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var matakuliah = _unitOfWork.MatakuliahRepository.GetByID(idMatakuliah);
                    if (matakuliah != null)
                    {

                        _unitOfWork.MatakuliahRepository.Delete(matakuliah);
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