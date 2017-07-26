using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;
using System.Security.Cryptography;
using System;
using System.Text;

namespace BusinessServices
{
    /// <summary>
    /// Offers services for product specific CRUD operations
    /// </summary>
    public class DeveloperServices : IDeveloperServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public DeveloperServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Public method to authenticate user by user name and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Authenticate(string email, string password)
        {
            var user = _unitOfWork.DeveloperRepository.Get(u => u.Email == email && u.Password == password);
            if (user != null && user.Email != "")
            {
                return user.Email;
            }
            return null;
        }

        /// <summary>
        /// Public method to get secret key.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string GetSecretKey(string appId)
        {
            if(appId != null)
            {
                var user = _unitOfWork.DeveloperRepository.Get(u => u.AppId == appId);
                if (user != null)
                {
                    var secret = Convert.ToBase64String(user.SecretKey);
                    return secret;
                }
            }
            return null;
        }

        /// <summary>
        /// Fetches developer details by id
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public DeveloperEntity GetDeveloperById(string appId)
        {
            var developer = _unitOfWork.DeveloperRepository.GetByID(appId);
            if (developer != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<ApiKey, DeveloperEntity>(); });
                var developerModel = Mapper.Map<ApiKey, DeveloperEntity>(developer);
                return developerModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a developer
        /// </summary>
        /// <param name="developerEntity"></param>
        /// <returns></returns>
        public string CreateDev(DeveloperEntity developerEntity)
        {
            using (var scope = new TransactionScope())
            {
                var developer = new ApiKey
                {
                    Email = developerEntity.Email,
                    Password = developerEntity.Password
                };
                _unitOfWork.DeveloperRepository.Insert(developer);
                _unitOfWork.Save();
                scope.Complete();
                return developer.Email + " created";
            }
        }

        /// <summary>
        /// Creates an acess
        /// </summary>
        /// <param name="developerEntity"></param>
        /// <returns></returns>
        public DeveloperEntity RequestApikey(string email)
        {
            var developerModel = new DeveloperEntity();
            if (email != null)
            {
                using (var scope = new TransactionScope())
                {
                    var developer = _unitOfWork.DeveloperRepository.GetByID(email);
                    if (developer != null)
                    {
                        RandomNumberGenerator randomizer = RandomNumberGenerator.Create();

                        byte[] secretKey = new byte[32];
                        randomizer.GetBytes(secretKey);
                        string appId = Guid.NewGuid().ToString();

                        developer.AppId = appId;
                        developer.SecretKey = secretKey;

                        _unitOfWork.DeveloperRepository.Update(developer);
                        _unitOfWork.Save();
                        scope.Complete();
                        Mapper.Initialize(cfg => { cfg.CreateMap<ApiKey, DeveloperEntity>(); });
                        developerModel = Mapper.Map<ApiKey, DeveloperEntity>(developer);
                    }
                }
            }
            return developerModel;
        }

        /// <summary>
        /// Updates a developer
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="developerEntity"></param>
        /// <returns></returns>
        public bool UpdateDev(string email, DeveloperEntity developerEntity)
        {
            var success = false;
            if (developerEntity != null && email != null)
            {
                using (var scope = new TransactionScope())
                {
                    var developer = _unitOfWork.DeveloperRepository.GetByID(email);
                    if (developer != null)
                    {
                        developer.Email = developerEntity.Email;
                        developer.Password = developerEntity.Password;
                        _unitOfWork.DeveloperRepository.Update(developer);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Delete a developer
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteDev(string email)
        {
            var success = false;
            if (email != null)
            {
                using (var scope = new TransactionScope())
                {
                    var developer = _unitOfWork.DeveloperRepository.GetByID(email);
                    if (developer != null)
                    {
                        _unitOfWork.DeveloperRepository.Delete(developer);
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
