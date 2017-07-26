using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessEntities;
using BusinessServices;
using WebApi.ActionFilters;
using WebApi.ErrorHelper;
using WebAPI.Filters;

namespace WebAPI.Controllers
{
    [AccessAuthenticationFilter]
    public class UserController : ApiController
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        //[Route("api/User/authenticate/{username}:{password}")]
        //public HttpResponseMessage GetUserCheck(string username, string password)
        //{
        //    if (username != null && password != null)
        //    {
        //        var user = _userServices.Authenticate(username, password);
        //        if (user != 0)
        //            return Request.CreateResponse(HttpStatusCode.OK, user);
        //        throw new ApiDataException(1001, "No user match.", HttpStatusCode.NotFound);
        //    }
        //    throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        //}

        [Queryable(PageSize = 10)]
        //GET: all users found
        public HttpResponseMessage GetAll()
        {
            var users = _userServices.GetAllUsers().AsQueryable();
            var userEntity = users as List<UserEntity> ?? users.ToList();
            if (userEntity.Any())
                return Request.CreateResponse(HttpStatusCode.OK, userEntity.AsQueryable());
            throw new ApiDataException(1000, "Users not found", HttpStatusCode.NotFound);
        }

        //GET: user by id
        public HttpResponseMessage Get(int id)
        {
            if (id != null && id > 0)
            {
                var user = _userServices.GetUserById(id);
                if (user != null)
                    return Request.CreateResponse(HttpStatusCode.OK, user);
                throw new ApiDataException(1001, "No user found for this id.", HttpStatusCode.NotFound);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }

        // POST: User/Create
        public int Post([FromBody] UserEntity userEntity)
        {
            return _userServices.CreateUser(userEntity);
        }

        // PUT: User/Edit/5
        public bool Put(int id, [FromBody]UserEntity userEntity)
        {
            if (id > 0)
            {
                return _userServices.UpdateUser(id, userEntity);
            }
            return false;
        }

        // DELETE: User/Delete/5
        public bool Delete(int id)
        {
            if (id != null && id > 0)
            {
                var isSuccess = _userServices.DeleteUser(id);
                if (isSuccess)
                {
                    return isSuccess;
                }
                throw new ApiDataException(1002, "User is already deleted or not exist in system.", HttpStatusCode.NoContent);
            }
            throw new ApiException() { ErrorCode = (int)HttpStatusCode.BadRequest, ErrorDescription = "Bad Request..." };
        }
    }
}
