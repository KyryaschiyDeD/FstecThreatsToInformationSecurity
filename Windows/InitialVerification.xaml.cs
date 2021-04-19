using System.Windows;

namespace FstecThreatsToInformationSecurity.Windows
{
    /// <summary>
    /// Логика взаимодействия для InitialVerification.xaml
    /// </summary>
    public partial class InitialVerification : Window
    {
        public InitialVerification()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
