using System.Windows;

namespace FstecThreatsToInformationSecurity.Windows
{
    /// <summary>
    /// Логика взаимодействия для DoAnUpdate.xaml
    /// </summary>
    public partial class DoAnUpdate : Window
    {
        public DoAnUpdate()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
