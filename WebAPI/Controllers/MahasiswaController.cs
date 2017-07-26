using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessEntities;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;

namespace WebAPI.Controllers
{
    [Authenticate]
    public class MahasiswaController : ApiController
    {
        private readonly IMahasiswaServices _mahasiswaServices;

        public MahasiswaController(IMahasiswaServices mahasiswaServices)
        {
            _mahasiswaServices = mahasiswaServices;
        }

        [AuthorizationRequired]
        [Queryable]
        // GET: All mahasiswa and by param
        [Route("api/mahasiswa/profil/{param?}")]
        public HttpResponseMessage Get(int? angkatan = null, string status = null, string prodi = null, string dosbing = null)
        {
            var mahasiswas = _mahasiswaServices.GetAllMahasiswa(angkatan, status, prodi, dosbing).AsQueryable();
            var mahasiswaEntity = mahasiswas as List<MahasiswaEntity> ?? mahasiswas.ToList();
            if(mahasiswaEntity.Any())
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaEntity.AsQueryable());
            throw new ApiDataException(404, "Mahasiswa not found for this param", HttpStatusCode.NotFound);
        }


        // GET: Mahasiswa/profil/id
        [Route("api/mahasiswa/profil/{nim}")]
        public HttpResponseMessage Get(int nim)
        {
            if (nim != null && nim > 0)
            {
                var mahasiswa = _mahasiswaServices.GetMahasiswaById(nim);
                if (mahasiswa != null)
                    return Request.CreateResponse(HttpStatusCode.OK, mahasiswa);
                throw new ApiDataException(404, "Mahasiswa not found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        [AuthorizationRequired]
        [Queryable]
        // GET: nilai by param
        [Route("api/nilai/{param?}")]
        public HttpResponseMessage GetNilaiByParam(int? nim = null, string periode = null, int? idMakul = null, int? angkatan = null)
        {
            var nilai = _mahasiswaServices.GetNilai(nim, periode, idMakul, angkatan).AsQueryable();
            var nilaiEntity = nilai as List<EnrollEntity> ?? nilai.ToList();
            if (nilaiEntity != null)
                return Request.CreateResponse(HttpStatusCode.OK, nilaiEntity);
            throw new ApiDataException(404, "Nilai mahasiswa not found for this param.", HttpStatusCode.NotFound);
        }

        [Queryable]
        // GET: nilai by param
        [Route("api/nilai/{nim}")]
        public HttpResponseMessage GetNilaiById(int nim)
        {
            var nilai = _mahasiswaServices.GetNilaiMahasiswa(nim).AsQueryable();
            var nilaiEntity = nilai as List<EnrollEntity> ?? nilai.ToList();
            if (nilaiEntity != null)
                return Request.CreateResponse(HttpStatusCode.OK, nilaiEntity);
            throw new ApiDataException(404, "Nilai mahasiswa not found for this id.", HttpStatusCode.NotFound);
        }
    
        [Queryable]
        // GET: nilai by param
        [Route("api/mahasiswa/nilai/praktikum/{nim}")]
        public HttpResponseMessage GetNilaiPraktikumById(int nim)
        {
            var nilaiPraktikum = _mahasiswaServices.GetNilaiPraktikumMhs(nim).AsQueryable();
            var nilaiEntity = nilaiPraktikum as List<EnrollDetailDTO> ?? nilaiPraktikum.ToList();
            if (nilaiEntity != null)
                return Request.CreateResponse(HttpStatusCode.OK, nilaiEntity);
            throw new ApiDataException(404, "Nilai mahasiswa not found for this id.", HttpStatusCode.NotFound);
        }

        //[AuthorizationRequired]
        //// POST: Mahasiswa/Create
        //public int Post([FromBody] MahasiswaEntity mahasiswaEntity)
        //{
        //    return _mahasiswaServices.CreateMahasiswa(mahasiswaEntity);
        //}

        //[AuthorizationRequired]
        //// PUT: Mahasiswa/Edit/5
        //public bool Put(int id, [FromBody]MahasiswaEntity mahasiswaEntity)
        //{
        //    if (id > 0)
        //    {
        //        return _mahasiswaServices.UpdateMahasiswa(id, mahasiswaEntity);
        //    }
        //    return false;
        //}

        //[AuthorizationRequired]
        //// DELETE: Mahasiswa/Delete/5
        //public bool Delete(int id)
        //{
        //    if (id != null && id > 0)
        //    {
        //        var isSuccess = _mahasiswaServices.DeleteMahasiswa(id);
        //        if (isSuccess)
        //        {
        //            return isSuccess;
        //        }
        //        throw new ApiDataException(1002, "Mahasiswa is already deleted or not exist in system.", HttpStatusCode.NoContent);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}
    }
}
