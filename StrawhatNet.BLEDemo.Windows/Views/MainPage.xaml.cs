using Microsoft.Practices.Prism.StoreApps;

namespace StrawhatNet.BLEDemo.Views
{
    public sealed partial class MainPage : VisualStateAwarePage
    {
        new object DataContext { get; set; }

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
