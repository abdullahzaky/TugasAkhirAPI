using DataModel;
using DataModel.UnitOfWork;
using Resolver;
using System.ComponentModel.Composition;

namespace BusinessServices
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<IUserServices, UserServices>();
            registerComponent.RegisterType<ITokenServices, TokenServices>();
            registerComponent.RegisterType<IMahasiswaServices, MahasiswaServices>();
            registerComponent.RegisterType<IMatakuliahServices, MatakuliahServices>();
            registerComponent.RegisterType<IEnrollServices, EnrollServices>();
            registerComponent.RegisterType<IDeveloperServices, DeveloperServices>();
        }
    }
}
