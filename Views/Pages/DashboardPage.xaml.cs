using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CabinetDocProWpf.Views.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
        }

        // زر إنشاء وثيقة جديدة - الآن يفتح RapportSpecialPage مباشرة
        private void BtnCreateDocument_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RapportSpecialPage());
        }

        // زر إدارة العملاء
        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ClientsPage());
        }

        // زر المكتبة (الوثائق) - الآن يبقى في Dashboard
        private void BtnDocuments_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("استخدم القائمة الجانبية 'Créer' لإنشاء وثيقة جديدة", "المكتبة", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // رابط "Voir tous" في KPI
        private void BtnViewClients_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new ClientsPage());
        }
    }
}
