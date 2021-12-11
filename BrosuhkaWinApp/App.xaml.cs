using BrosuhkaWinApp.Entities;
using System.Windows;

namespace BrosuhkaWinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            DbUtils.Db?.Dispose();
            base.OnExit(e);
        }
    }
}
