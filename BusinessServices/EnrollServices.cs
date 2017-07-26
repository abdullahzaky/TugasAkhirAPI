using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;
using System.Data.Entity;
using System;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace BusinessServices
{
    public class EnrollServices : IEnrollServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public EnrollServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches enroll details by id
        /// </summary>
        /// <param name="idEnroll"></param>
        /// <returns></returns>
        public EnrollEntity GetEnrollById(int idEnroll)
        {
            var enroll = _unitOfWork.EnrollRepository.GetByID(idEnroll);
            if (enroll != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, EnrollEntity>(); });
                var enrollModel = Mapper.Map<Enroll, EnrollEntity>(enroll);
                return enrollModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the enrolls.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EnrollEntity> GetAllEnroll()
        {
            var enrolls = _unitOfWork.EnrollRepository.GetAll().ToList();
            if (enrolls.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<Enroll, EnrollEntity>(); });
                var enrollsModel = Mapper.Map<List<Enroll>, List<EnrollEntity>>(enrolls);
                return enrollsModel;
            }
            return null;
        }

        public IEnumerable<EnrollDetailDTO> GetEnrollDetailById(int nim)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository.GetMany(x=> (x.IdMahasiswa == nim)).ToList();
            if (enrollMahasiswa.Any())
            {
                List<EnrollDetailDTO> listEnrollDetail = new List<EnrollDetailDTO>();
                foreach(var b in enrollMahasiswa) { 
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

        public IEnumerable<EnrollDetailDTO> GetEnrollDetailMahasiswa(int? nim, string periode, int? angkatan)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository.GetMany(x => (nim == null || x.IdMahasiswa.Equals(nim))
            && (periode == null || x.PeriodeEnroll.Equals(periode)) && (angkatan == null || x.Mahasiswa.Angkatan.Equals(angkatan))
            && (x.Mahasiswa.StatusMahasiswa != "Lulus")).OrderBy(x=>x.IdMahasiswa).ThenBy(x=>x.IdMahasiswa).ToList();

            if (enrollMahasiswa.Any())
            {
                List<EnrollDetailDTO> listEnrollDetail = new List<EnrollDetailDTO>();
                foreach (var b in enrollMahasiswa)
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


        private class DetailMhs
        {
            public int TotalNilai { get; set; }
            public int TotalSks { get; set; }
            public int JmlhNilaiE { get; set; }
            public int SksNilaiD { get; set; }
        }

        //private static void switcherFunc(string grade, int? sks, ref int? nilaiTotal, ref int? totalSKS)
        //{
        //    switch (grade)
        //    {
        //        case "A":
        //            nilaiTotal += (sks * 4).Value;
        //            totalSKS += sks;
        //            break;
        //        case "B":
        //            nilaiTotal += sks * 3;
        //            totalSKS += sks;
        //            break;
        //        case "C":
        //            nilaiTotal += sks * 2;
        //            totalSKS += sks;
        //            break;
        //        case "D":
        //            nilaiTotal += sks * 1;
        //            totalSKS += sks;
        //            break;
        //        case "E":
        //            nilaiTotal += sks * 0;
        //            totalSKS += sks;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        private void insertStatus(int id, string statusMahasiswa) { 
            using (var scope = new TransactionScope()) {
                var mahasiswa = _unitOfWork.MahasiswaRepository.GetByID(id);
                if (mahasiswa != null)
                {
                    mahasiswa.StatusMahasiswa = statusMahasiswa;
                    _unitOfWork.MahasiswaRepository.Update(mahasiswa);
                    _unitOfWork.Save();
                    scope.Complete();
                }
            }
        }

        private static void switcherFunc(string grade, int? sks, DetailMhs obj)
        {
            switch (grade)
            {
                case "A":
                    obj.TotalNilai += (sks * 4).Value;
                    obj.TotalSks += sks.Value;
                    break;
                case "B":
                    obj.TotalNilai += (sks * 3).Value;
                    obj.TotalSks += sks.Value;
                    break;
                case "C":
                    obj.TotalNilai += (sks * 2).Value;
                    obj.TotalSks += sks.Value;
                    break;
                case "D":
                    obj.TotalNilai += (sks * 1).Value;
                    obj.TotalSks += sks.Value;
                    obj.SksNilaiD += sks.Value;
                    break;
                case "E":
                    obj.TotalNilai += (sks * 0).Value;
                    obj.TotalSks += sks.Value;
                    obj.JmlhNilaiE += 1;
                    break;
                default:
                    break;
            }
        }

        public EnrollPeriodSummaryDTO GetEnrollSummaryById(int nim)
        {
            /**
            * Use Group By in order to remove duplicates of mata kuliah when mahasiswa mengulang
            * .GroupBy(x => x.IdMakul).Select(x => x.OrderBy(p => p.GradeNilai).First())
            * 
            **/
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                .GetMany(x => x.IdMahasiswa.Equals(nim)).OrderBy(x => x.PeriodeEnroll).ToList();

            if (enrollMahasiswa.Any())
            {
                var mahasiswa = enrollMahasiswa.Select(x => x.Mahasiswa).First();
                List<EnrollIpsSummaryDto> listIps = new List<EnrollIpsSummaryDto>();
                foreach (var period in enrollMahasiswa.GroupBy(x => new { x.PeriodeEnroll }))
                {
                    DetailMhs detailPeriod = new DetailMhs();
                    foreach (var row in period)
                    {
                        switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailPeriod);
                    }
                    var ipsModel = new EnrollIpsSummaryDto()
                    {
                        Sks = detailPeriod.TotalSks,
                        Ips = (float)Math.Round((double)detailPeriod.TotalNilai / detailPeriod.TotalSks,2),
                        PeriodeEnroll = period.Key.PeriodeEnroll
                    };
                    listIps.Add(ipsModel);
                    //summaryModel.EnrollIps.Add(ipsModel);
                }

                DetailMhs detailMhs = new DetailMhs();    
                foreach (var row in enrollMahasiswa.GroupBy(x => x.MataKuliah.KodeMakul).Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList())
                {
                    switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                }
                var summaryModel = new EnrollPeriodSummaryDTO()
                {
                    IdMahasiswa = mahasiswa.IdMahasiswa,
                    NamaMahasiswa = mahasiswa.NamaMahasiswa,
                    Ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks,2),
                    Sks = detailMhs.TotalSks,
                    EnrollIps = listIps
                };
                return summaryModel;
            }
            return null;
        }

        public IEnumerable<EnrollPeriodSummaryDTO> GetMahasiswaSummary(string periode, int? angkatan)
        {
            var enrollMahasiswa = _unitOfWork.EnrollRepository
                .GetMany(x => (periode == null || x.PeriodeEnroll.Equals(periode)) && (angkatan == null || x.Mahasiswa.Angkatan.Equals(angkatan)) && (x.Mahasiswa.StatusMahasiswa != "Lulus"))
                .OrderBy(x => x.IdMahasiswa).ThenBy(x => x.PeriodeEnroll).ThenBy(x => x.Mahasiswa.Angkatan).ToList();

            if (enrollMahasiswa.Any()) {
                List<EnrollPeriodSummaryDTO> listSummaryModel = new List<EnrollPeriodSummaryDTO>();
                foreach (var groups in enrollMahasiswa.GroupBy(x=>new { x.IdMahasiswa, x.Mahasiswa.NamaMahasiswa }))
                { 
                    List<EnrollIpsSummaryDto> listIps = new List<EnrollIpsSummaryDto>();
                    foreach (var period in groups.GroupBy(x => x.PeriodeEnroll))
                    {
                        DetailMhs detailPeriod = new DetailMhs();
                        foreach (var row in period)
                        {
                            switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailPeriod);
                        }
                        var ipsModel = new EnrollIpsSummaryDto()
                        {
                            Sks = detailPeriod.TotalSks,
                            Ips = (float)Math.Round((double)detailPeriod.TotalNilai / detailPeriod.TotalSks, 2),
                            PeriodeEnroll = period.Key
                        };
                        listIps.Add(ipsModel);
                    }

                    DetailMhs detailMhs = new DetailMhs();
                    foreach (var row in groups.GroupBy(x => x.MataKuliah.KodeMakul).Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList())
                    {
                        switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                    }
                    var summaryModel = new EnrollPeriodSummaryDTO()
                    {
                        IdMahasiswa = groups.Key.IdMahasiswa,
                        NamaMahasiswa = groups.Key.NamaMahasiswa,
                        Ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks,2),
                        Sks = detailMhs.TotalSks,
                        EnrollIps = listIps
                    };
                    listSummaryModel.Add(summaryModel);
                }
                return listSummaryModel;
            }
            return null;
        }


        /// <summary>
        /// Fetch evaluasi dua tahun seorang mahasiswa.
        /// </summary>
        /// <returns></returns>
        public MahasiswaKritisDto GetMahasiswaKritisDuaTahunById(int id)
        {
            int maxSks = 24, maxNilai = 4;
            int semesterDo = 5;

            //normalisasi nilai terbaik dari makul yang diambil secara duplikasi
            var enrolls = _unitOfWork.EnrollRepository.GetMany(x => x.Mahasiswa.Semester > 2 && x.Mahasiswa.Semester < 7 && x.IdMahasiswa.Equals(id))
                .GroupBy(x => x.MataKuliah.KodeMakul).Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList();

            if (enrolls.Any())
            {
                DetailMhs detailMhs = new DetailMhs();

                foreach (var row in enrolls)
                {
                    //pemenuhan syarat kelolosan dua tahun evaluasi awal setiap mahasiswa
                    switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                }
                var ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks,2);
                //syarat lolos evaluasi tidak terpenuhi
                if (detailMhs.TotalSks < 30 || ipk < 2.00 || detailMhs.JmlhNilaiE != 0)
                {
                    bool kelolosan = false;
                    string statusMhs = "Aktif";

                    var mahasiswa = enrolls.Select(x=>x.Mahasiswa).First();
                    switch (mahasiswa.Semester)
                    {
                        case 3:
                            statusMhs = "SP-1";
                            break;
                        case 4:
                            statusMhs = "SP-2";
                            break;
                        case 5:
                            statusMhs = "SP-3";
                            break;
                        case 6:
                            statusMhs = "Drop Out";
                            break;
                        default:
                            break;
                    };
                    //insertStatus(group.Key.IdMahasiswa, statusMhs);
                    var sisaSemester = semesterDo - mahasiswa.Semester;
                    //check survivabilitas
                    if (detailMhs.TotalSks + sisaSemester * maxSks >= 30
                        && (detailMhs.TotalNilai + sisaSemester * maxSks * maxNilai) / (detailMhs.TotalSks + sisaSemester * maxSks) >= 2.00
                        && sisaSemester * maxSks > detailMhs.JmlhNilaiE)
                    {
                        kelolosan = true;
                    }
                    var mhsKritisDua = new MahasiswaKritisDto()
                    {
                        IdMahasiswa = id,
                        Ipk = ipk,
                        Sks = detailMhs.TotalSks,
                        Angkatan = mahasiswa.Angkatan,
                        Status = statusMhs,
                        Kelolosan = kelolosan
                    };
                    return mhsKritisDua;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the evaluasi dua tahun.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MahasiswaKritisDto> GetMahasiswaKritisDuaTahun()
        {
            int maxSks = 24, maxNilai = 4;
            int semesterDo = 5;
            //var regex = "1,2".Split(',');

            //normalisasi nilai terbaik dari makul yang diambil secara duplikasi
            var enrolls = _unitOfWork.EnrollRepository.GetMany(x => x.Mahasiswa.Semester > 2 && x.Mahasiswa.Semester < 7)
                .GroupBy(x => new { x.MataKuliah.KodeMakul, x.IdMahasiswa })
                .Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList();

            if (enrolls.Any())
            {
                List<MahasiswaKritisDto> listKritisModel = new List<MahasiswaKritisDto>();
                foreach (var group in enrolls.GroupBy(x => new { x.IdMahasiswa, x.Mahasiswa.Semester, x.Mahasiswa.Angkatan }))
                {
                    //int? totalSks = 0, jmlNilaiE = 0;
                    //int? totalNilai = 0;
                    bool kelolosan = false;
                    //var mhsKritisDua = new MahasiswaKritisDto();
                    string statusMhs = "Aktif";
                    DetailMhs detailMhs = new DetailMhs();

                    foreach (var row in group)
                    {
                        //pemenuhan syarat kelolosan dua tahun evaluasi awal setiap mahasiswa
                        //switcherFunc(row.GradeNilai, row.MataKuliah.Sks, ref totalNilai, ref totalSks);
                        switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                    }
                    var ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks,2);
                    //syarat lolos evaluasi tidak terpenuhi
                    if (detailMhs.TotalSks < 30 || ipk < 2.00 || detailMhs.JmlhNilaiE != 0)
                    {
                        switch (group.Key.Semester)
                        {
                            case 3:
                                statusMhs = "SP-1";
                                break;
                            case 4:
                                statusMhs = "SP-2";
                                break;
                            case 5:
                                statusMhs = "SP-3";
                                break;
                            case 6:
                                statusMhs = "Drop Out";
                                break;
                            default:
                                break;
                        };
                        //insertStatus(group.Key.IdMahasiswa, statusMhs);
                        var sisaSemester = semesterDo - group.Key.Semester;
                        //check survivabilitas
                        if (detailMhs.TotalSks + sisaSemester * maxSks >= 30
                            && (detailMhs.TotalNilai + sisaSemester * maxSks * maxNilai) / (detailMhs.TotalSks + sisaSemester * maxSks) >= 2.00
                            && sisaSemester * maxSks > detailMhs.JmlhNilaiE)
                        {
                            kelolosan = true;
                        }
                        var mhsKritisDua = new MahasiswaKritisDto()
                        {
                            IdMahasiswa = group.Key.IdMahasiswa,
                            Ipk = ipk,
                            Sks = detailMhs.TotalSks,
                            Angkatan = group.Key.Angkatan,
                            Status = statusMhs,
                            Kelolosan = kelolosan
                        };
                        listKritisModel.Add(mhsKritisDua);
                    }
                }
                return listKritisModel;
            }
            return null;
        }


        /// <summary>
        /// Fetch mahasiswa akhir kritis by id.
        /// </summary>
        /// <returns></returns>
        public MahasiswaKritisDto GetMahasiswaAkhirKritisById(int id)
        {
            int maxSks = 24, maxNilai = 4;
            int semesterDo = 12;
            var enrolls = _unitOfWork.EnrollRepository.GetMany(z => z.Mahasiswa.StatusMahasiswa != "Lulus" && z.Mahasiswa.Semester > 9 && z.IdMahasiswa.Equals(id))
                .GroupBy(x => x.MataKuliah.KodeMakul).Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList();
            if (enrolls.Any())
            {
                var mahasiswa = enrolls.Select(x => x.Mahasiswa).First();
                DetailMhs detailMhs = new DetailMhs();
                int? jmlPilihan = 0;
                bool kelulusan = true;
                string statusMhs = "Aktif";
                var daftarMakulWajib = _unitOfWork.MatakuliahRepository.GetMany(y => (y.Sifat == "Wajib")).ToList();
                foreach (var row in enrolls)
                {
                    //pemenuhan syarat evaluasi hasil akhir setiap mahasiswa
                    if (row.MataKuliah.Sifat == "Pilihan")
                    {
                        jmlPilihan += 1;
                    }
                    else
                    {
                        //remove yang kode makulnya sama
                        daftarMakulWajib.RemoveAll(x => row.MataKuliah.KodeMakul.Contains(x.KodeMakul));
                    }
                    switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                }
                var ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks, 2);
                //syarat lolos evaluasi tidak terpenuhi
                if (detailMhs.TotalSks < 144 || ipk < 2.00 || detailMhs.JmlhNilaiE != 0 || detailMhs.SksNilaiD / detailMhs.TotalSks <= 1 / 4
                    || !daftarMakulWajib.Equals(null) || jmlPilihan < 4)
                {
                    var sisaSemester = semesterDo - mahasiswa.Semester;
                    //check survivabilitas
                    if (detailMhs.TotalSks + sisaSemester * maxSks < 144
                        || (detailMhs.TotalNilai + sisaSemester * maxSks * maxNilai) / (detailMhs.TotalSks + sisaSemester * maxSks) < 2.00
                        || sisaSemester * maxSks < detailMhs.JmlhNilaiE
                        || detailMhs.SksNilaiD - sisaSemester * maxSks > 1 / 4 * detailMhs.TotalSks)
                    {
                        kelulusan = false;
                    }
                    if (mahasiswa.Semester >= 13)
                    {
                        statusMhs = "Drop Out";
                    }
                    else
                    {
                        switch (mahasiswa.Semester)
                        {
                            case 10:
                                statusMhs = "SP-1";
                                break;
                            case 11:
                                statusMhs = "SP-2";
                                break;
                            case 12:
                                statusMhs = "SP-3";
                                break;
                            default:
                                statusMhs = "Aktif";
                                break;
                        };
                    }
                }
                //insertStatus(id, statusMhs);
                var mhsKritisAkhir = new MahasiswaKritisDto()
                {
                    IdMahasiswa = id,
                    Ipk = ipk,
                    Sks = detailMhs.TotalSks,
                    Angkatan = mahasiswa.Angkatan,
                    Status = statusMhs,
                    Kelolosan = kelulusan
                };
                return mhsKritisAkhir;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Fetches all the mahasiswa akhir kritis.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MahasiswaKritisDto> GetAllMahasiswaAkhirKritis()
        {
            int maxSks = 24, maxNilai = 4;
            int semesterDo = 12;
            var enrolls = _unitOfWork.EnrollRepository.GetMany(z => z.Mahasiswa.StatusMahasiswa != "Lulus" && z.Mahasiswa.Semester > 9)
                .GroupBy(x => new { x.MataKuliah.KodeMakul, x.IdMahasiswa }).Select(x => x.OrderBy(p => p.GradeNilai).First()).ToList();

            if (enrolls.Any())
            {
                List<MahasiswaKritisDto> listKritisModel = new List<MahasiswaKritisDto>();
                foreach (var group in enrolls.GroupBy(y => new { y.IdMahasiswa, y.Mahasiswa.Semester, y.Mahasiswa.Angkatan }).OrderBy(y => y.Key.IdMahasiswa))
                {
                    DetailMhs detailMhs = new DetailMhs();
                    int? jmlPilihan = 0;
                    bool kelulusan = true;
                    string statusMhs = "Aktif";
                    var daftarMakulWajib = _unitOfWork.MatakuliahRepository.GetMany(y => (y.Sifat == "Wajib")).ToList();
                    foreach (var row in group)
                    {
                        //pemenuhan syarat evaluasi hasil akhir setiap mahasiswa
                        if (row.MataKuliah.Sifat == "Pilihan")
                        {
                            jmlPilihan += 1;
                        }
                        else
                        {
                            //remove yang kode makulnya sama
                            daftarMakulWajib.RemoveAll(x => row.MataKuliah.KodeMakul.Contains(x.KodeMakul));
                        }
                        switcherFunc(row.GradeNilai, row.MataKuliah.Sks, detailMhs);
                    }
                    var ipk = (float)Math.Round((double)detailMhs.TotalNilai / detailMhs.TotalSks, 2);
                    //syarat lolos evaluasi tidak terpenuhi
                    if (detailMhs.TotalSks < 144 || ipk < 2.00 || detailMhs.JmlhNilaiE != 0 || detailMhs.SksNilaiD / detailMhs.TotalSks <= 1 / 4
                        || !daftarMakulWajib.Equals(null) || jmlPilihan < 4)
                    {
                        var sisaSemester = semesterDo - group.Key.Semester;
                        //check survivabilitas
                        if (detailMhs.TotalSks + sisaSemester * maxSks < 144
                            || (detailMhs.TotalNilai + sisaSemester * maxSks * maxNilai) / (detailMhs.TotalSks + sisaSemester * maxSks) < 2.00
                            || sisaSemester * maxSks < detailMhs.JmlhNilaiE
                            || detailMhs.SksNilaiD - sisaSemester * maxSks > 1 / 4 * detailMhs.TotalSks)
                        {
                            kelulusan = false;
                        }
                        if (group.Key.Semester >= 13)
                        {
                            statusMhs = "Drop Out";
                        }
                        else
                        {
                            switch (group.Key.Semester)
                            {
                                case 10:
                                    statusMhs = "SP-1";
                                    break;
                                case 11:
                                    statusMhs = "SP-2";
                                    break;
                                case 12:
                                    statusMhs = "SP-3";
                                    break;
                                default:
                                    statusMhs = "Aktif";
                                    break;
                            };
                        }
                        //insertStatus(group.Key.IdMahasiswa, statusMhs);
                    }
                    var mhsKritisAkhir = new MahasiswaKritisDto()
                    {
                        IdMahasiswa = group.Key.IdMahasiswa,
                        Ipk = ipk,
                        Sks = detailMhs.TotalSks,
                        Angkatan = group.Key.Angkatan,
                        Status = statusMhs,
                        Kelolosan = kelulusan
                    };
                    listKritisModel.Add(mhsKritisAkhir);
                }
                return listKritisModel;
            }
            return null;
        }

        /// <summary>
        /// Creates an enroll
        /// </summary>
        /// <param name="enrollEntity"></param>
        /// <returns></returns>
        public int CreateEnroll(EnrollEntity enrollEntity)
        {
            using (var scope = new TransactionScope())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<EnrollEntity, Enroll>(); });
                var enroll = Mapper.Map<EnrollEntity, Enroll>(enrollEntity);

                _unitOfWork.EnrollRepository.Insert(enroll);
                _unitOfWork.Save();
                scope.Complete();
                return enroll.IdEnroll;
            }
        }

        /// <summary>
        /// Updates an enroll
        /// </summary>
        /// <param name="idEnroll"></param>
        /// <param name="enrollEntity"></param>
        /// <returns></returns>
        public bool UpdateEnroll(int idEnroll, EnrollEntity enrollEntity)
        {
            var success = false;
            if (enrollEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var enroll = _unitOfWork.EnrollRepository.GetByID(idEnroll);
                    if (enroll != null)
                    {
                        Mapper.Initialize(cfg => { cfg.CreateMap<EnrollEntity, Enroll>(); });
                        enroll = Mapper.Map<EnrollEntity, Enroll>(enrollEntity);
                        //enroll.NilaiTotal = enrollEntity.NilaiTotal;
                        //enroll.GradeNilai = enrollEntity.GradeNilai;
                        //enroll.PeriodeEnroll = enrollEntity.PeriodeEnroll;
                        //enroll.Kehadiran = enrollEntity.Kehadiran;
                        //enroll.Pertemuan = enrollEntity.Pertemuan;
                        //enroll.IdMakul = enrollEntity.IdMakul;
                        //enroll.IdMahasiswa = enrollEntity.IdMahasiswa;
                        _unitOfWork.EnrollRepository.Update(enroll);
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
        public bool DeleteEnroll(int idEnroll)
        {
            var success = false;
            if (idEnroll > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var enroll = _unitOfWork.EnrollRepository.GetByID(idEnroll);
                    if (enroll != null)
                    {
                        _unitOfWork.EnrollRepository.Delete(enroll);
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