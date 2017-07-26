using DataModel.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public interface IUnitOfWork
    {
        #region Properties
        //GenericRepository<Product> ProductRepository { get; }
        //GenericRepository<User> UserRepository { get; }
        //GenericRepository<Token> TokenRepository { get; }
        /// <summary>
        /// Save method.
        /// </summary>
        void Save();
        #endregion
    }
}
