using FstecThreatsToInformationSecurity.classes;
using System;
using System.Windows;

namespace FstecThreatsToInformationSecurity.Windows
{
    /// <summary>
    /// Логика взаимодействия для DeleteById.xaml
    /// </summary>
    public partial class DeleteById : Window
    {
        public DeleteById()
        {
            InitializeComponent();
        }

        private void DoChanged_Click(object sender, RoutedEventArgs e)
        {
            int objId;
            try
            {
                int.TryParse(IdChengedText.Text, out objId);
            }
            catch (Exception)
            {
                MessageBox.Show("Должно быть введено число!!!");
                throw;
            }
            this.DialogResult = true;
            Threats.DelById(objId);
        }
    }
}
