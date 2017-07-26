using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessEntities;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;
using WebApi;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    public class DevelopController : ApiController
    {
        private readonly IDeveloperServices _developerServices;

        #region Public Constructor
        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public DevelopController(IDeveloperServices developerServices)
        {
            _developerServices = developerServices;
        }
        #endregion

        //[AccessAuthenticationFilter]
        //// GET api/product/5
        //public HttpResponseMessage Get(string appId)
        //{
        //    if (appId != null && appId != "")
        //    {
        //        var developer = _developerServices.GetDeveloperById(appId);
        //        if (developer != null)
        //            return Request.CreateResponse(HttpStatusCode.OK, developer);
        //        throw new ApiDataException(1001, "No developer information found for this email.", HttpStatusCode.NotFound);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}

        //[AccessAuthenticationFilter]
        //// GET api/product/5
        //public HttpResponseMessage GetRequestAccess(string email)
        //{
        //    if (email != null && email != "")
        //    {
        //        var apiKey = _developerServices.RequestApikey(email);
        //        if (apiKey != null)
        //            return Request.CreateResponse(HttpStatusCode.OK, apiKey);
        //        throw new ApiDataException(1001, "No developer information found for this email.", HttpStatusCode.NotFound);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}

        //// POST api/product
        //public string Post([FromBody] DeveloperEntity developerEntity)
        //{
        //    return _developerServices.CreateDev(developerEntity);
        //}

        //// PUT api/product/5
        //public bool Put(string email, [FromBody]DeveloperEntity developerEntity)
        //{
        //    if (email != null && email !="")
        //    {
        //        return _developerServices.UpdateDev(email, developerEntity);
        //    }
        //    return false;
        //}

        //[AccessAuthenticationFilter]
        //// DELETE api/product/5
        //public bool Delete(string email)
        //{
        //    if (email != null)
        //    {
        //        var isSuccess = _developerServices.DeleteDev(email);
        //        if (isSuccess)
        //        {
        //            return isSuccess;
        //        }
        //        throw new ApiDataException(1002, "Developer is already deleted or not exist in system.", HttpStatusCode.NoContent);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}
    }
}
