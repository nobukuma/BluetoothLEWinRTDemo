using Microsoft.Practices.Prism.StoreApps;
using Windows.UI.Xaml.Controls;

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
