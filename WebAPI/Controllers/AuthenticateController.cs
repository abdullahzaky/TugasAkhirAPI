using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttributeRouting.Web.Http;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;
using System.Web.Http.Controllers;
using System.Linq;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    [Authenticate]
    public class AuthenticateController : ApiController
    {
        #region Private variable.

        private readonly ITokenServices _tokenServices;

        #endregion

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize product service instance
        /// </summary>
        public AuthenticateController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }

        #endregion

        /// <summary>
        /// Authenticates user and returns token with expiry.
        /// </summary>
        /// <returns></returns>
        [ApiAuthenticationFilter]
        [POST("authenticate")]
        public HttpResponseMessage Authenticate()
        {
            if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                var basicAuthenticationIdentity = System.Threading.Thread.CurrentPrincipal.Identity as BasicAuthenticationIdentity;
                if (basicAuthenticationIdentity != null)
                {
                    var userId = basicAuthenticationIdentity.UserId;
                    return GetAuthToken(userId);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns auth token for the validated user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ApiAuthenticationFilter]
        private HttpResponseMessage GetAuthToken(int userId)
        {
            var token = _tokenServices.GenerateToken(userId);
            var response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
            response.Headers.Add("Token", token.AuthToken);
            response.Headers.Add("TokenExpiry", ConfigurationManager.AppSettings["AuthTokenExpiry"]);
            response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
            return response;
        }
        
        ////DELETE api/authenticate/1
        //public HttpResponseMessage Delete(int id)
        //{
        //    //if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
        //    //{
        //        if (id > 0)
        //        {
        //            _tokenServices.DeleteByUserId(id);
        //            var response = Request.CreateResponse(HttpStatusCode.OK, "Token Cleaned");
        //            return response;
        //        }
        //    //}
        //    return null;
        //}



        [DELETE("api/logout")]
        public bool DeleteToken(HttpRequestMessage request)
        {
            IEnumerable<string> headerValues = request.Headers.GetValues("Token");
            var token = headerValues.FirstOrDefault();
            if (token != null)
            {
                var isSuccess = _tokenServices.Kill(token);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(204, "Token is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}