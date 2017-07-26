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
    //[Authenticate]
    public class EnrollController : ApiController
    {
        private readonly IEnrollServices _enrollServices;

        public EnrollController(IEnrollServices enrollServices)
        {
            _enrollServices = enrollServices;
        }

        //[AuthorizationRequired]
        //[Queryable]
        //// GET: Mahasiswa
        //public HttpResponseMessage Get()
        //{
        //    var enrolls = _enrollServices.GetAllEnroll().AsQueryable();
        //    var enrollEntity = enrolls as List<EnrollEntity> ?? enrolls.ToList();
        //    if (enrollEntity.Any())
        //        return Request.CreateResponse(HttpStatusCode.OK, enrollEntity.AsQueryable());
        //    throw new ApiDataException(1000, "Enroll not found", HttpStatusCode.NotFound);
        //}

        ////GET: Mahasiswa/enroll/5
        //public HttpResponseMessage Get(int id)
        //{
        //    if (id != 0)
        //    {
        //        var enroll = _enrollServices.GetEnrollById(id);
        //        if (enroll != null)
        //            return Request.CreateResponse(HttpStatusCode.OK, enroll);
        //        throw new ApiDataException(1001, "No enroll found for this enroll id.", HttpStatusCode.NotFound);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}

        [Route("api/mahasiswa/nilai/{nim}")]
        [Queryable]
        public HttpResponseMessage GetMahasiswaEnrollDetail(int nim)
        {
            var enroll = _enrollServices.GetEnrollDetailById(nim);
            var enrollEntity = enroll as List<EnrollDetailDTO> ?? enroll.ToList();
            if (enrollEntity != null)
                return Request.CreateResponse(HttpStatusCode.OK, enrollEntity);
            throw new ApiDataException(1001, "No enroll found for this mahasiswa id.", HttpStatusCode.NotFound);
        }

        [AuthorizationRequired]
        [Route("api/mahasiswa/nilai/{param?}")]
        [Queryable]
        public HttpResponseMessage GetMahasiswaEnrollDetail(int? nim = null, string periode = null, int? angkatan = null)
        {
            var enroll = _enrollServices.GetEnrollDetailMahasiswa(nim, periode, angkatan);
            var enrollEntity = enroll as List<EnrollDetailDTO> ?? enroll.ToList();
            if (enrollEntity != null)
                return Request.CreateResponse(HttpStatusCode.OK, enrollEntity);
            throw new ApiDataException(1001, "No enroll found for this mahasiswa id.", HttpStatusCode.NotFound);
        }
        
        [Route("api/mahasiswa/summary/{nim}")]
        public HttpResponseMessage GetMahasiswaSummaryById(int nim)
        {
            var mahasiswaSummary = _enrollServices.GetEnrollSummaryById(nim);
            if (mahasiswaSummary != null)
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaSummary);
            throw new ApiDataException(1001, "No enroll found for this mahasiswa id.", HttpStatusCode.NotFound);
        }

        [AuthorizationRequired]
        [Queryable]
        [Route("api/mahasiswa/summary/{param?}")]
        public HttpResponseMessage GetMahasiswaSummary(string periode = null, int? angkatan = null,
            int? sksTerendah = null, int? sksTertinggi = null, float? ipkTerendah = null, float? ipkTertinggi = null)
        {
            var allSummary = _enrollServices.GetMahasiswaSummary(periode, angkatan).Where(x=>(x.Ipk >= ipkTerendah || ipkTerendah == null)
            && (x.Ipk <= ipkTertinggi || ipkTertinggi == null) && (x.Sks >= sksTerendah || sksTerendah == null)
            && (x.Sks <= sksTertinggi || sksTertinggi == null)).AsQueryable();
            var allSummaryEnt = allSummary as List<EnrollPeriodSummaryDTO> ?? allSummary.ToList();
            if (allSummaryEnt != null)
                return Request.CreateResponse(HttpStatusCode.OK, allSummaryEnt);
            throw new ApiDataException(1001, "No enroll found for this mahasiswa id.", HttpStatusCode.NotFound);
        }

        [Route("api/mahasiswa/kritisdua/{nim}")]
        public HttpResponseMessage GetMahasiswaKritisDuaTahunById(int nim)
        {
            var mahasiswaKritisDua = _enrollServices.GetMahasiswaKritisDuaTahunById(nim);
            if (mahasiswaKritisDua != null)
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaKritisDua);
            throw new ApiDataException(1001, "No mahasiswa kritis found", HttpStatusCode.NotFound);
        }

        [AuthorizationRequired]
        [Queryable]
        [Route("api/mahasiswa/kritisdua/{param?}")]
        public HttpResponseMessage GetMahasiswaKritisDuaTahun(int? angkatan = null, bool? kelolosan = null)
        {
            var mahasiswaKritisDua = _enrollServices.GetMahasiswaKritisDuaTahun().Where(x=> (x.Angkatan.Equals(angkatan) || angkatan == null)
            && (x.Kelolosan.Equals(kelolosan) || kelolosan == null)).AsQueryable();
            var mahasiswaKritisDuaEnt = mahasiswaKritisDua as List<MahasiswaKritisDto> ?? mahasiswaKritisDua.ToList();
            if (mahasiswaKritisDuaEnt != null)
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaKritisDuaEnt);
            throw new ApiDataException(1001, "No mahasiswa kritis found", HttpStatusCode.NotFound);
        }

        [Route("api/mahasiswa/kritisakhir/{nim}")]
        public HttpResponseMessage GetMahasiswaKritisAkhirById(int nim)
        {
            var mahasiswaKritis = _enrollServices.GetMahasiswaAkhirKritisById(nim);
            if (mahasiswaKritis != null)
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaKritis);
            throw new ApiDataException(1001, "No mahasiswa kritis found", HttpStatusCode.NotFound);
        }

        [AuthorizationRequired]
        [Queryable]
        [Route("api/mahasiswa/kritisakhir/{param?}")]
        public HttpResponseMessage GetMahasiswaKritisAkhir(int? angkatan = null, bool? kelolosan = null,
            float? ipkTerendah = null, float? ipkTertinggi = null, int? sksTerendah = null, int? sksTertinggi = null )
        {
            var mahasiswaKritis = _enrollServices.GetAllMahasiswaAkhirKritis()
                .Where(x=> ((x.Angkatan.Equals(angkatan) || angkatan == null) && (x.Kelolosan.Equals(kelolosan)||kelolosan == null)
                && (x.Ipk >= ipkTerendah || ipkTerendah == null ) && (x.Ipk <= ipkTertinggi || ipkTertinggi == null) 
                && (x.Sks >= sksTerendah || sksTerendah == null) && (x.Sks <= sksTertinggi || sksTertinggi == null))).AsQueryable();
            var mahasiswaKritisEnt = mahasiswaKritis as List<MahasiswaKritisDto> ?? mahasiswaKritis.ToList();
            if (mahasiswaKritisEnt != null)
                return Request.CreateResponse(HttpStatusCode.OK, mahasiswaKritisEnt);
            throw new ApiDataException(1001, "No mahasiswa kritis found", HttpStatusCode.NotFound);
        }

        //[AuthorizationRequired]
        //// POST: Mahasiswa/Create
        //public int Post([FromBody] EnrollEntity enrollEntity)
        //{
        //    return _enrollServices.CreateEnroll(enrollEntity);
        //}

        //[AuthorizationRequired]
        //// PUT: Mahasiswa/Edit/5
        //public bool Put(int id, [FromBody]EnrollEntity enrollEntity)
        //{
        //    if (id > 0)
        //    {
        //        return _enrollServices.UpdateEnroll(id, enrollEntity);
        //    }
        //    return false;
        //}

        //[AuthorizationRequired]
        //// DELETE: Mahasiswa/Delete/5
        //public bool Delete(int id)
        //{
        //    if (id != null && id > 0)
        //    {
        //        var isSuccess = _enrollServices.DeleteEnroll(id);
        //        if (isSuccess)
        //        {
        //            return isSuccess;
        //        }
        //        throw new ApiDataException(1002, "Enroll is already deleted or not exist in system.", HttpStatusCode.NoContent);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}
    }
}

