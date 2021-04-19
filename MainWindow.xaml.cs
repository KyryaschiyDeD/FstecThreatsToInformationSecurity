using ClosedXML.Excel;
using FstecThreatsToInformationSecurity.classes;
using FstecThreatsToInformationSecurity.Windows;
using LiteDB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Text.Json;

namespace FstecThreatsToInformationSecurity
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private int numberOfRecPerPage; // Кол-во эл-тов на странице
        static Paging PagedTable = new Paging(); // Постраничный вывод информации
        private List<Threat> list; // Список угроз, необходим для постраничного вывода информации
        private DataGrid tableNow = new DataGrid(); // Хранение текущей таблицы, для её отображения после изменения данных

        public MainWindow() 
        {
            InitializeComponent();
            Threats.InitialVerificationStart();  // Проверяем обновление
            ShowShort_MenuItem.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent)); // вызываем первичную таблицу через симуляцию нажатия
            int[] RecordsToShow = { 15, 30, 60, 120, 240 };
            foreach (int RecordGroup in RecordsToShow)
            {
                NumberOfRecords.Items.Add(RecordGroup);
            }
        }

        private void SaveTXT_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {

                    myStream.Close();
                }
            }
            List<Threat> threats = Threats.GetAll();
            FileStream fileUpdateInfo = new FileStream(saveFileDialog.FileName, FileMode.Create);
            using (StreamWriter writer = new StreamWriter(fileUpdateInfo))
            {
                writer.WriteLine("-------------------------");
                writer.WriteLine("Импорт от " + DateTime.Now.ToString());
                writer.WriteLine("-----------------------");
                writer.WriteLine($"Всего данных: {threats.Count}");
                writer.WriteLine("-----------------------");
                writer.WriteLine("-----------------------");
                foreach (var item in threats)
                {
                    writer.WriteLine($"\t\t Id: {item.Id}");
                    writer.WriteLine($"\t\t Наименование: {item.Name}");
                    writer.WriteLine($"\t\t Описание: {item.Description}");
                    writer.WriteLine($"\t\t Источник: {item.Source}");
                    writer.WriteLine($"\t\t Объект воздействия: {item.ObjectThreat}");
                    writer.WriteLine($"\t\t Нарушение конфиденциальности: {item.PrivacyPolicy}");
                    writer.WriteLine($"\t\t Нарушение целостности: {item.Integrity}");
                    writer.WriteLine($"\t\t Нарушение доступности: {item.Availability}");
                    writer.WriteLine($"\t\t Дата включения угрозы в БнД УБИ: {item.DateCreate}");
                    writer.WriteLine($"\t\t Дата последнего изменения данных в БнД УБИ: {item.DateUpdate}");
                    writer.WriteLine("//////////////////////////////////////////////");
                }
            }
        }
        private void SaveXLSX_Click(object sender, RoutedEventArgs e)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");

            worksheet.Cell("A" + 1).Value = "Id";
            worksheet.Cell("B" + 1).Value = "Наименование";
            worksheet.Cell("C" + 1).Value = "Описание";
            worksheet.Cell("D" + 1).Value = "Источник";
            worksheet.Cell("E" + 1).Value = "Объект воздействия";
            worksheet.Cell("F" + 1).Value = "Нарушение конфиденциальности";
            worksheet.Cell("G" + 1).Value = "Нарушение целостности";
            worksheet.Cell("H" + 1).Value = "Нарушение доступности";
            worksheet.Cell("I" + 1).Value = "Дата включения угрозы в БнД УБИ";
            worksheet.Cell("J" + 1).Value = "Дата последнего изменения данных в БнД УБИ";

            int row = 1;
            foreach (var item in Threats.GetAll().ToList())
            {
                row++;
                worksheet.Cell("A" + row).Value = item.Id;
                worksheet.Cell("B" + row).Value = item.Name;
                worksheet.Cell("C" + row).Value = item.Description;
                worksheet.Cell("D" + row).Value = item.Source;
                worksheet.Cell("E" + row).Value = item.ObjectThreat;
                if(item.PrivacyPolicy)
                    worksheet.Cell("F" + row).Value = 1;
                else
                    worksheet.Cell("F" + row).Value = 0;

                if (item.Integrity)
                    worksheet.Cell("G" + row).Value = 1;
                else
                    worksheet.Cell("G" + row).Value = 0;

                if (item.Availability)
                    worksheet.Cell("H" + row).Value = 1;
                else
                    worksheet.Cell("H" + row).Value = 0;

                worksheet.Cell("I" + row).Value = item.DateCreate;
                worksheet.Cell("J" + row).Value = item.DateUpdate;
            }
            worksheet.Columns().AdjustToContents();
            worksheet.Column("B").Width = 50;
            worksheet.Column("C").Width = 75;
            worksheet.Column("D").Width = 60;
            worksheet.Column("E").Width = 60;
            worksheet.Column("F").Width = 10;
            worksheet.Column("G").Width = 10;
            worksheet.Column("H").Width = 10;
            worksheet.Column("I").Width = 15;
            worksheet.Column("J").Width = 15;
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {
                    
                    myStream.Close();
                }
            }
            workbook.SaveAs(saveFileDialog.FileName);

        }

        private async void SaveJSON_ClickAsync(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null)
                {

                    myStream.Close();
                }
            }
            List<Threat> threats = Threats.GetAll();
            // Не знаю, прочитает ли обратно он то, что записал, к сожалению, нет времени проверять
            using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate)) 
            {
                foreach (var item in threats)
                {
                    if (item.Name != null)
                        item.Name = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(item.Name));
                    if (item.Description != null)
                        item.Description = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(item.Description));
                    if (item.Source != null)
                        item.Source = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(item.Source));
                    if (item.ObjectThreat != null)
                        item.ObjectThreat = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(item.ObjectThreat));
                    await System.Text.Json.JsonSerializer.SerializeAsync<Threat>(fs, item);
                }
            }
        }

        private void ChangeById_Click(object sender, RoutedEventArgs e)
        {
            ChangeById changeById = new ChangeById();
            if (changeById.ShowDialog() == true)
            {
                Show(tableNow);
            }
        }
        private void DeleteById_Click(object sender, RoutedEventArgs e)
        {
            DeleteById deleteById = new DeleteById();
            if (deleteById.ShowDialog() == true)
            {
                Show(tableNow);
            }
        }
        private void RefreshData_Click(object sender, RoutedEventArgs e)
        {
            Threats.CheckUpdate(true);
            Show(tableNow);
        }

        private void HiddenAllGrids()
        {
            ShowGrid.Visibility = Visibility.Hidden;
            ShowShortGrid.Visibility = Visibility.Hidden;
            ShowShortPlusGrid.Visibility = Visibility.Hidden;
        }

        private void ShowShort_Click(object sender, RoutedEventArgs e)
        {
            tableNow = ShowShortGrid;
            Show(tableNow);
        }
        private void ShowShortPlus_Click(object sender, RoutedEventArgs e)
        {
            tableNow = ShowShortPlusGrid;
            Show(ShowShortPlusGrid);
        }
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            tableNow = ShowGrid;
            Show(ShowGrid);
        }
        private void Show(DataGrid grid)
        {
            HiddenAllGrids();
            grid.Visibility = Visibility.Visible;
            list = Threats.GetAll();
            PagedTable.PageIndex = 1;
            NumberOfRecords.SelectedItem = 15;
            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);
            DataTable firstTable = PagedTable.SetPaging(list, numberOfRecPerPage);
            grid.ItemsSource = firstTable.DefaultView;
            First.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }


        public string PageNumberDisplay()
        {
            int PagedNumber = numberOfRecPerPage * (PagedTable.PageIndex + 1);
            if (PagedNumber > list.Count)
            {
                PagedNumber = list.Count;
            }
            return  PagedNumber + " / " + list.Count;
        }
        private void Backwards_Click(object sender, RoutedEventArgs e)
        {
            tableNow.ItemsSource = PagedTable.Previous(list, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }
        private void First_Click(object sender, RoutedEventArgs e)
        {
            tableNow.ItemsSource = PagedTable.First(list, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }
        private void NumberOfRecords_SelectionChanged(object sender, RoutedEventArgs e)
        {
            numberOfRecPerPage = Convert.ToInt32(NumberOfRecords.SelectedItem);
            tableNow.ItemsSource = PagedTable.First(list, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }
        private void Last_Click(object sender, RoutedEventArgs e)
        {
            tableNow.ItemsSource = PagedTable.Last(list, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }
        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            tableNow.ItemsSource = PagedTable.Next(list, numberOfRecPerPage).DefaultView;
            PageInfo.Content = PageNumberDisplay();
        }
    }
}
