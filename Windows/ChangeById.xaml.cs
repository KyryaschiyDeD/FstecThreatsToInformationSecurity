using FstecThreatsToInformationSecurity.classes;
using System;
using System.Windows;

namespace FstecThreatsToInformationSecurity.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangeById.xaml
    /// </summary>
    public partial class ChangeById : Window
    {
        Threat obj;
        public ChangeById()
        {
            InitializeComponent();
        }

        private void SaveChanged_Click(object sender, RoutedEventArgs e)
        {
            Threats.UpdateOneThreat(
                new Threat(
                    obj.Id,
                    Name.Text, 
                    Description.Text, 
                    Source.Text,
                    ObjectThreat.Text, 
                    Convert.ToBoolean(PrivacyPolicy.IsChecked),
                    Convert.ToBoolean(Integrity.IsChecked),
                    Convert.ToBoolean(Availability.IsChecked),
                    obj.DateCreate,
                    obj.DateUpdate,
                    obj.DateUpload
                    )
                );
            this.DialogResult = true;
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
            obj = Threats.GetById(objId);
            if (obj != null)
            {
                DoChanged.Visibility = Visibility.Hidden;
                IdChengedText.Visibility = Visibility.Hidden;
                GrBRed.Visibility = Visibility.Visible;
                Id.Content = "Id :" + obj.Id;
                Name.Text = obj.Name;
                Description.Text = obj.Description;
                Source.Text = obj.Source;
                ObjectThreat.Text = obj.ObjectThreat;
                PrivacyPolicy.IsChecked = obj.PrivacyPolicy;
                Integrity.IsChecked = obj.Integrity;
                Availability.IsChecked = obj.Availability;
                DateCreate.Content = obj.DateCreate;
                DateUpdate.Content = obj.DateUpdate;
               // DateUpload.Content = obj.DateUpload;
            }
            else
            {
                MessageBox.Show("Записи с данным Id не существует!!!");
            }
        }
    }
}
