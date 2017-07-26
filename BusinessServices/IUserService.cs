using System.Collections.Generic;
using BusinessEntities;

namespace BusinessServices
{
    public interface IUserServices
    {
        int Authenticate(string userName, string password);
        UserEntity GetUserById(int userId);
        IEnumerable<UserEntity> GetAllUsers();
        int CreateUser(UserEntity userEntity);
        bool UpdateUser(int userId, UserEntity userEntity);
        bool DeleteUser(int userId);
    }
}