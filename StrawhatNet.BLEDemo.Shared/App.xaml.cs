using StrawhatNet.BLEDemo.Services;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Unity;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace StrawhatNet.BLEDemo
{
    sealed partial class App : MvvmAppBase
    {
        IUnityContainer _container = new UnityContainer();
        public App()
        {
            InitializeComponent();
        }

        protected override Task OnLaunchApplication(LaunchActivatedEventArgs args)
        {
            NavigationService.Navigate("Main", null);
            return Task.FromResult<object>(null);
        }

        protected override Task OnInitialize(IActivatedEventArgs args)
        {
            _container.RegisterInstance<ISessionStateService>(SessionStateService);
            _container.RegisterInstance<INavigationService>(NavigationService);
            _container.RegisterType<IDataRepository, DataRepository>();

            ViewModelLocationProvider.SetDefaultViewModelFactory((viewModelType) => _container.Resolve(viewModelType));            
            return Task.FromResult<object>(null);
        }
    }
}
