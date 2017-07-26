#region Using Namespaces...

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Entity.Validation;
using DataModel.GenericRepository;

#endregion

namespace DataModel.UnitOfWork
{
    /// <summary>
    /// Unit of Work class responsible for DB transactions
    /// </summary>
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        #region Private member variables...

        private readonly WebApiDbEntities _context = null;
        private GenericRepository<User> _userRepository;
        private GenericRepository<Token> _tokenRepository;
        private GenericRepository<Mahasiswa> _mahasiswaRepository;
        private GenericRepository<Enroll> _enrollRepository;
        private GenericRepository<MataKuliah> _matakuliahRepository;
        private GenericRepository<ApiKey> _developerRepository;

        #endregion

        public UnitOfWork()
        {
            _context = new WebApiDbEntities();
        }

        #region Public Repository Creation properties...

        /// <summary>
        /// Get/Set Property for mahasiswa repository.
        /// </summary>
        public GenericRepository<Mahasiswa> MahasiswaRepository
        {
            get
            {
                if (this._mahasiswaRepository == null)
                    this._mahasiswaRepository = new GenericRepository<Mahasiswa>(_context);
                return _mahasiswaRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for enroll repository.
        /// </summary>
        public GenericRepository<Enroll> EnrollRepository
        {
            get
            {
                if (this._enrollRepository == null)
                    this._enrollRepository = new GenericRepository<Enroll>(_context);
                return _enrollRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for matakuliah repository.
        /// </summary>
        public GenericRepository<MataKuliah> MatakuliahRepository
        {
            get
            {
                if (this._matakuliahRepository == null)
                    this._matakuliahRepository = new GenericRepository<MataKuliah>(_context);
                return _matakuliahRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public GenericRepository<User> UserRepository
        {
            get
            {
                if (this._userRepository == null)
                    this._userRepository = new GenericRepository<User>(_context);
                return _userRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for token repository.
        /// </summary>
        public GenericRepository<Token> TokenRepository
        {
            get
            {
                if (this._tokenRepository == null)
                    this._tokenRepository = new GenericRepository<Token>(_context);
                return _tokenRepository;
            }
        }

        /// <summary>
        /// Get/Set Property for user repository.
        /// </summary>
        public GenericRepository<ApiKey> DeveloperRepository
        {
            get
            {
                if (this._developerRepository == null)
                    this._developerRepository = new GenericRepository<ApiKey>(_context);
                return _developerRepository;
            }
        }
        #endregion

        #region Public member methods...
        /// <summary>
        /// Save method.
        /// </summary>
        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {

                var outputLines = new List<string>();
                foreach (var eve in e.EntityValidationErrors)
                {
                    outputLines.Add(string.Format(
                        "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:", DateTime.Now,
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        outputLines.Add(string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage));
                    }
                }
                System.IO.File.AppendAllLines(@"C:\errors.txt", outputLines);

                throw e;
            }

        }

        #endregion

        #region Implementing IDiosposable...

        #region private dispose variable declaration...
        private bool disposed = false;
        #endregion

        /// <summary>
        /// Protected Virtual Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Debug.WriteLine("UnitOfWork is being disposed");
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}