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
    /// Offers services for user specific operations
    /// </summary>
    public class UserServices : IUserServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public UserServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Public method to authenticate user by user name and password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Authenticate(string userName, string password)
        {
            var user = _unitOfWork.UserRepository.Get(u => u.UserName == userName && u.Password == password);
            if (user != null && user.UserId > 0)
            {
                return user.UserId;
            }
            return 0;
        }

        /// <summary>
        /// Fetches product details by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public UserEntity GetUserById(int userId)
        {
            var user = _unitOfWork.UserRepository.GetByID(userId);
            if (user != null)
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
                var userModel = Mapper.Map<User, UserEntity>(user);
                return userModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the products.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetAllUsers()
        {
            var users = _unitOfWork.UserRepository.GetAll().ToList();
            if (users.Any())
            {
                Mapper.Initialize(cfg => { cfg.CreateMap<User, UserEntity>(); });
                var usersModel = Mapper.Map<List<User>, List<UserEntity>>(users);
                return usersModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public int CreateUser(UserEntity userEntity)
        {
            using (var scope = new TransactionScope())
            {
                var user = new User
                {
                    UserName = userEntity.UserName,
                    Password = userEntity.Password,
                    Name = userEntity.Name
                };
                _unitOfWork.UserRepository.Insert(user);
                _unitOfWork.Save();
                scope.Complete();
                return user.UserId;
            }
        }

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public bool UpdateUser(int userId, UserEntity userEntity)
        {
            var success = false;
            if (userEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var user = _unitOfWork.UserRepository.GetByID(userId);
                    if (user != null)
                    {
                        user.UserName = userEntity.UserName;
                        user.Password = userEntity.Password;
                        user.Name = userEntity.Name;
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteUser(int userId)
        {
            var success = false;
            if (userId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var user = _unitOfWork.UserRepository.GetByID(userId);
                    if (user != null)
                    {
                        _unitOfWork.UserRepository.Delete(user);
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