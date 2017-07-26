using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessEntities;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;
using System.Web.Http.OData;

namespace WebAPI.Controllers
{
    [Authenticate]
    public class MakulController : ApiController
    {
        private readonly IMatakuliahServices _makulServices;

        public MakulController(IMatakuliahServices matakuliahServices)
        {
            _makulServices = matakuliahServices;
        }

        [Route("api/makul/periode")]
        [Queryable]
        public HttpResponseMessage GetAllPeriode()
        {
            var matakuliahPeriode = _makulServices.GetAllPeriode().AsQueryable();
            var matakuliahEntity = matakuliahPeriode as List<string> ?? matakuliahPeriode.ToList();
            if (matakuliahEntity.Any())
                return Request.CreateResponse(HttpStatusCode.OK, matakuliahEntity);
            throw new ApiDataException(404, "Matakuliah not found", HttpStatusCode.NotFound);
        }

        [Queryable]
        // GET: Makul
        public HttpResponseMessage Get(string dosen=null, string sifat=null, int? rekomendasi = null)
        {
            var matakuliahs = _makulServices.GetAllMatakuliah(dosen, sifat, rekomendasi).AsQueryable();
            var matakuliahEntity = matakuliahs as List<MatakuliahEntity> ?? matakuliahs.ToList();
            if (matakuliahEntity.Any())
                return Request.CreateResponse(HttpStatusCode.OK, matakuliahEntity);
            throw new ApiDataException(404, "Matakuliah not found", HttpStatusCode.NotFound);
        }

        //// GET: Makul/Details/5
        //public HttpResponseMessage Get(int id)
        //{
        //    if (id != null && id > 0)
        //    {
        //        var matakuliah = _makulServices.GetMatakuliahById(id);
        //        if (matakuliah != null)
        //            return Request.CreateResponse(HttpStatusCode.OK, matakuliah);
        //        throw new ApiDataException(404, "No matakuliah found for this id.", HttpStatusCode.NotFound);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}

        [AuthorizationRequired]
        [Route("api/makul/nilai/{idMakul}/{param?}")]
        [Queryable]
        public HttpResponseMessage GetEnrollByMakul(int idMakul, string periode = null)
        {
            if (idMakul != 0)
            {
                var makul = _makulServices.GetEnrollMakulById(idMakul, periode);
                var enrollMakul = makul as List<EnrollEntity> ?? makul.ToList();
                if (enrollMakul != null && enrollMakul.Count !=0)
                    return Request.CreateResponse(HttpStatusCode.OK, enrollMakul);
                throw new ApiDataException(404, "No enroll makul found for this makul id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [AuthorizationRequired]
        [Queryable]
        //Get makul seluruh mahasiswa yang tertinggal
        [Route("api/makul/tertinggal/{param?}")]
        public HttpResponseMessage GetMakulTertinggal(int? angkatan = null)
        {
            var makulTertinggal = _makulServices.GetMakulTertinggal(angkatan).AsQueryable();
            var makulTertinggalEnt = makulTertinggal as List<MakulTertinggalDto> ?? makulTertinggal.ToList();
            if (makulTertinggalEnt.Any())
                return Request.CreateResponse(HttpStatusCode.OK, makulTertinggalEnt);
            throw new ApiDataException(404, "Makul tertinggal mahasiswa not found  for this param", HttpStatusCode.NotFound);
        }

        [Queryable]
        //Get makul tertinggal seorang mahasiswa
        [Route("api/makul/tertinggal/{nim}")]
        public HttpResponseMessage GetMakulMhsTertinggal(int nim)
        {
            if (nim.Equals(nim))
            {
                var makulMhsTertinggal = _makulServices.GetMakulMhsTertinggal(nim).AsQueryable();
                var makulMhsTertinggalEnt = makulMhsTertinggal as List<MatakuliahEntity> ?? makulMhsTertinggal.ToList();
                if (makulMhsTertinggalEnt != null)
                    return Request.CreateResponse(HttpStatusCode.OK, makulMhsTertinggalEnt);
                throw new ApiDataException(404, "No makul tertinggal found for this mahasiswa id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [Queryable]
        //Get jadwal
        [Route("api/makul/jadwal/{nim}")]
        public HttpResponseMessage GetJadwalMahasiswa(int nim)
        {
            if (nim.Equals(nim))
            {
                var jadwal = _makulServices.GetJadwalMahasiswa(nim).AsQueryable();
                var jadwalEnt = jadwal as List<MatakuliahEntity> ?? jadwal.ToList();
                if (jadwalEnt != null)
                    return Request.CreateResponse(HttpStatusCode.OK, jadwalEnt);
                throw new ApiDataException(404, "Matakuliah not found for this id", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [AuthorizationRequired]
        [Queryable]
        //Get jadwal
        [Route("api/makul/jadwal/")]
        public HttpResponseMessage GetAllJadwal()
        {
            var jadwal = _makulServices.GetAllJadwal().AsQueryable();
            var jadwalEnt = jadwal as List<MatakuliahEntity> ?? jadwal.ToList();
            if (jadwalEnt != null)
                return Request.CreateResponse(HttpStatusCode.OK, jadwalEnt);
            throw new ApiDataException(404, "Matakuliah not found", HttpStatusCode.NotFound);
        }

        //[AuthorizationRequired]
        //// POST: Makul/Create
        //public int Post([FromBody] MatakuliahEntity matakuliahEntity)
        //{
        //    return _makulServices.CreateMatakuliah(matakuliahEntity);
        //}

        //[AuthorizationRequired]
        //// PUT: Makul/Edit/5
        //public bool Put(int id, [FromBody]MatakuliahEntity matakuliahEntity)
        //{
        //    if (id > 0)
        //    {
        //        return _makulServices.UpdateMatakuliah(id, matakuliahEntity);
        //    }
        //    return false;
        //}

        //[AuthorizationRequired]
        //// DELETE: Makul/Delete/5
        //public bool Delete(int id)
        //{
        //    if (id != null && id > 0)
        //    {
        //        var isSuccess = _makulServices.DeleteMatakuliah(id);
        //        if (isSuccess)
        //        {
        //            return isSuccess;
        //        }
        //        throw new ApiDataException(1002, "Matakuliah is already deleted or not exist in system.", HttpStatusCode.NoContent);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}
    }
}
