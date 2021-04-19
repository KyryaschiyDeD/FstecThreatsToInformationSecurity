using FstecThreatsToInformationSecurity.Windows;
using LiteDB;
using System;
using System.Net;
using System.Windows;
using ClosedXML.Excel;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace FstecThreatsToInformationSecurity.classes
{
    public class Threats
    {
        private static void Download()
        {
            string remoteUri = "https://bdu.fstec.ru/files/documents/";
            string fileName = "thrlist.xlsx", WebResource = null;
            WebClient webClient = new WebClient();
            WebResource = remoteUri + fileName;
            try
            {
                webClient.DownloadFile(WebResource, fileName);
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка загрузки файла \"{fileName}\" с \"{WebResource}\"");
            }
        } // Загрузка файла

        private static void AddOnDataBase() // Добавление данных из файла в базу данных
        {
            using (XLWorkbook workbook = new XLWorkbook("thrlist.xlsx"))
            {
                var ws = workbook.Worksheets.First();
                var rows = ws.RangeUsed().RowsUsed();
                int rowsCount = rows.Count();
                using (var db = new LiteDatabase(@"Data.db"))
                {
                    var col = db.GetCollection<Threat>("threats");

                    for (int i = 3; i <= rowsCount; ++i)
                    {
                        col.Insert(new Threat
                        (
                             Convert.ToInt32(ws.Row(i).Cell(1).Value),
                             ws.Row(i).Cell(2).Value.ToString(),
                             ws.Row(i).Cell(3).Value.ToString(),
                             ws.Row(i).Cell(4).Value.ToString(),
                             ws.Row(i).Cell(5).Value.ToString(),
                             Convert.ToBoolean(ws.Row(i).Cell(6).Value),
                             Convert.ToBoolean(ws.Row(i).Cell(7).Value),
                             Convert.ToBoolean(ws.Row(i).Cell(8).Value),
                             Convert.ToDateTime(ws.Row(i).Cell(9).Value),
                             Convert.ToDateTime(ws.Row(i).Cell(10).Value),
                             DateTime.Now
                        ));



                    }

                }
            }
            File.Delete("thrlist.xlsx");
        }

        public static void InitialVerificationStart()
        {
            if (GetAll().Count == 0)
            {
                InitialVerification initialVerification = new InitialVerification();
                if (initialVerification.ShowDialog() == true)
                {
                    Download();
                    AddOnDataBase();
                }
                else
                    Application.Current.Shutdown();
            }
            else
            {
                CheckUpdate(false);
            }
        }

        public static List<Threat> GetAll()
        {
            List<Threat> list;
            var db = new LiteDatabase(@"Data.db");
            var col = db.GetCollection<Threat>("threats");
            list = col.FindAll().ToList();
            db.Dispose();
            return list;
        }

        private static bool CheckTwoValue(int a, int b)
        {
            if (a != b)
                return false;
            return true;
        }

        private static bool CheckTwoValue(bool a, bool b)
        {
            if (a != b)
                return false;
            return true;
        }

        private static bool CheckTwoValue(string a, string b)
        {
            bool check = true;
            if (a != null && b.Length != 0)
                if(!a.Equals(b))
                    check = false;

            if ((a != null && b.Length == 0) || (a == null && b.Length != 0))
                check = false;

            return check;
        }

        private static void AddOneThreat(Threat obj)
        {
            obj.DateUpload = DateTime.Now;
            var db = new LiteDatabase(@"Data.db");
            var col = db.GetCollection<Threat>("threats");
            col.Insert(obj);
            db.Dispose();
        }

        private static bool IsTheItemIncluded(List<Threat> list, Threat data)
        {
            bool vxodic = false;
            foreach (var item in list)
            {
                if (item.Id == data.Id)
                {
                    vxodic = true;
                }
            }
            return vxodic;
        }

        public static List<Report> DoUpdate(List<Threat> newThreats)
        {
            bool OneUpdateQuestion = true;  // Спрашивали ли мы разрешение на обновление?
            bool ValueOneUpdateQuestion = false; // Дали ли нам разрешение на обновление
            List<Report> reports = new List<Report>();
            List<Threat> list = GetAll();
            if (list.Count != newThreats.Count)
            {
                if (list.Count < newThreats.Count)
                    foreach (var item in newThreats)
                    {
                        if (!IsTheItemIncluded(list, item))
                        {
                            if (OneUpdateQuestion)
                            {
                                DoAnUpdate doAnUpdate = new DoAnUpdate();
                                if (doAnUpdate.ShowDialog() == true)
                                {
                                    ValueOneUpdateQuestion = true;
                                }
                                OneUpdateQuestion = false;
                            }
                            if (ValueOneUpdateQuestion)
                            {
                                AddOneThreat(item);
                                Report.countNew++;
                                reports.Add(new Report
                                {
                                    Id = item.Id,
                                    ParamName = new List<string> { "Name", "Source", "ObjectThreat" },
                                    Data = new List<string> { item.Name, item.Source, item.ObjectThreat },
                                    type = 0
                                });
                            }

                        }

                    }
                if (list.Count > newThreats.Count)
                    foreach (var item in list)
                    {
                        if (!IsTheItemIncluded(newThreats, item))
                        {
                            if (OneUpdateQuestion)
                            {
                                DoAnUpdate doAnUpdate = new DoAnUpdate();
                                if (doAnUpdate.ShowDialog() == true)
                                {
                                    ValueOneUpdateQuestion = true;
                                }
                                OneUpdateQuestion = false;
                            }
                            if (ValueOneUpdateQuestion)
                            {
                                DelById(item.Id);
                                Report.countDel++;
                                reports.Add(new Report
                                {
                                    Id = item.Id,
                                    type = 2
                                });
                            }

                        }

                    }
            }
            if (!OneUpdateQuestion && ValueOneUpdateQuestion || OneUpdateQuestion)
            {
                list = GetAll();
                for (int i = 0; i < list.Count; i++)
                {
                    bool needAnUpdate = false;
                    List<string> paramsNeedAnUpdate = new List<string>();
                    List<string> newDataUpdate = new List<string>();
                    List<string> oldData = new List<string>();

                    if (!CheckTwoValue(list[i].Name, newThreats[i].Name))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("Name");
                        newDataUpdate.Add(newThreats[i].Name);
                        oldData.Add(list[i].Name);
                    }

                    if (!CheckTwoValue(list[i].Description, newThreats[i].Description))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("Description");
                        newDataUpdate.Add(newThreats[i].Description);
                        oldData.Add(list[i].Description);
                    }

                    if (!CheckTwoValue(list[i].Source, newThreats[i].Source))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("Source");
                        newDataUpdate.Add(newThreats[i].Source);
                        oldData.Add(list[i].Source);
                    }

                    if (!CheckTwoValue(list[i].ObjectThreat, newThreats[i].ObjectThreat))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("ObjectThreat");
                        newDataUpdate.Add(newThreats[i].ObjectThreat);
                        oldData.Add(list[i].ObjectThreat);
                    }

                    if (!CheckTwoValue(list[i].PrivacyPolicy, newThreats[i].PrivacyPolicy))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("PrivacyPolicy");
                        newDataUpdate.Add(newThreats[i].PrivacyPolicy.ToString());
                        oldData.Add(list[i].PrivacyPolicy.ToString());
                    }

                    if (!CheckTwoValue(list[i].Integrity, newThreats[i].Integrity))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("Integrity");
                        newDataUpdate.Add(newThreats[i].Integrity.ToString());
                        oldData.Add(list[i].Integrity.ToString());
                    }

                    if (!CheckTwoValue(list[i].Availability, newThreats[i].Availability))
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("Availability");
                        newDataUpdate.Add(newThreats[i].Availability.ToString());
                        oldData.Add(list[i].Availability.ToString());
                    }

                    if (list[i].DateCreate != newThreats[i].DateCreate)
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("DateCreate");
                        newDataUpdate.Add(newThreats[i].DateCreate.ToString());
                        oldData.Add(list[i].DateCreate.ToString());
                    }

                    if (list[i].DateUpdate != newThreats[i].DateUpdate)
                    {
                        needAnUpdate = true;
                        paramsNeedAnUpdate.Add("DateUpdate");
                        newDataUpdate.Add(newThreats[i].DateUpdate.ToString());
                        oldData.Add(list[i].DateUpdate.ToString());
                    }

                    if (needAnUpdate)
                    {
                        if (OneUpdateQuestion)
                        {
                            DoAnUpdate doAnUpdate = new DoAnUpdate();
                            if (doAnUpdate.ShowDialog() == true)
                            {
                                ValueOneUpdateQuestion = true;
                            }
                            OneUpdateQuestion = false;
                        }
                        if (ValueOneUpdateQuestion)
                        {
                            Report.countUpdate++;
                            reports.Add(new Report
                            {
                                Id = i,
                                ParamName = paramsNeedAnUpdate,
                                Data = newDataUpdate,
                                OldData = oldData,
                                type = 1
                            });
                            UpdateOneThreat(newThreats[i]);
                        }
                           
                    }
                }
            }
            return reports;
        }

        public static void CheckUpdate( bool isRefreshData_Click = false)
        {
            Download();
            if (File.Exists("thrlist.xlsx"))
            {
                List<Threat> threats = new List<Threat>();
                XLWorkbook workbook = new XLWorkbook("thrlist.xlsx");

                var excelFile = workbook.Worksheets.First();
                var rows = excelFile.RangeUsed().RowsUsed();
                int rowsCount = rows.Count();
                for (int i = 3; i <= rowsCount; ++i)
                {
                    threats.Add(new Threat
                    (
                        Convert.ToInt32(excelFile.Row(i).Cell(1).Value),
                        excelFile.Row(i).Cell(2).Value.ToString(),
                        excelFile.Row(i).Cell(3).Value.ToString(),
                        excelFile.Row(i).Cell(4).Value.ToString(),
                        excelFile.Row(i).Cell(5).Value.ToString(),
                        Convert.ToBoolean(excelFile.Row(i).Cell(6).Value),
                        Convert.ToBoolean(excelFile.Row(i).Cell(7).Value),
                        Convert.ToBoolean(excelFile.Row(i).Cell(8).Value),
                        Convert.ToDateTime(excelFile.Row(i).Cell(9).Value),
                        Convert.ToDateTime(excelFile.Row(i).Cell(10).Value),
                        DateTime.Now
                    ));
                }
                workbook.Dispose();
                List<Report> report = DoUpdate(threats);
                if (report.Count == 0 && isRefreshData_Click)
                    MessageBox.Show("Обновлений не найдено!");

                if (Report.AllOperation != 0)
                {
                    FileStream fileUpdateInfo = new FileStream("lastUpdateInfo.txt", FileMode.Create);
                    using (StreamWriter writer = new StreamWriter(fileUpdateInfo))
                    {
                        writer.WriteLine("-----------------------");
                        writer.WriteLine("Обновление от " + DateTime.Now.ToString() + " Статус: Успешно");
                        writer.WriteLine("-----------------------");
                        writer.WriteLine($"Всего операций: {Report.AllOperation}");
                        writer.WriteLine("-----------------------");
                        writer.WriteLine($"Всего добавлено записей: {Report.countNew} " +
                            $"\nВсего Обновлено записей: {Report.countUpdate} " +
                            $"\nВсего удалено записей: {Report.countDel}");
                        writer.WriteLine("-----------------------");
                        if (Report.countNew != 0)
                        {
                            writer.WriteLine("-----------------------");
                            writer.WriteLine("Добавлено:");
                            foreach (var item in report)
                            {
                                if (item.type == 0)
                                {
                                    writer.WriteLine("\t\t-----------------------");
                                    writer.WriteLine($"\t\t\t Id: {item.Id}");
                                    for (int i = 0; i < item.ParamName.Count; i++)
                                    {
                                        writer.WriteLine($"\t\t\t {item.ParamName[i]}: {item.Data[i]}");
                                    }
                                    writer.WriteLine("\t\t-----------------------");
                                }
                            }
                            writer.WriteLine("-----------------------");
                        }
                        if (Report.countDel != 0)
                        {
                            writer.WriteLine("-----------------------");
                            writer.WriteLine("Удалено:");
                            foreach (var item in report)
                            {
                                if (item.type == 2)
                                {
                                    writer.WriteLine("\t\t-----------------------");
                                    writer.WriteLine($"\t\t\t Id: {item.Id}");
                                    writer.WriteLine("\t\t-----------------------");
                                }
                            }
                            writer.WriteLine("-----------------------");
                        }
                        if (Report.countUpdate != 0)
                        {
                            writer.WriteLine("Обвновлено:");
                            foreach (var item in report)
                            {
                                if (item.type == 1)
                                {
                                    writer.WriteLine("\t\t-----------------------");
                                    writer.WriteLine($"\t\t\t Id: {item.Id}");
                                    writer.WriteLine("\t\t\t Данные:");
                                    writer.WriteLine("\t\t\t-----------------------");
                                    int j = 0;
                                    for (int i = 0; i < item.ParamName.Count; i++)
                                    {
                                        writer.WriteLine($"\t\t\t {item.ParamName[i]}: ");
                                        writer.WriteLine("\t\t\t\t Было:");
                                        writer.WriteLine($"\t\t\t\t\t {item.OldData[j]}");
                                        writer.WriteLine("\t\t\t\t Стало:");
                                        writer.WriteLine($"\t\t\t\t\t {item.Data[i]}");
                                        j++;
                                    }
                                    writer.WriteLine("\t\t\t-----------------------");
                                    writer.WriteLine("\t\t\t-----------------------");
                                }
                            }
                        }
                    }
                    Process.Start("C:\\Windows\\System32\\notepad.exe", "lastUpdateInfo.txt");
                }
                
                File.Delete("thrlist.xlsx");
                threats.Clear();
            }
            else
            {
                MessageBox.Show("Ошибка! \n Файл не найден!");
            }
            
        }

        public static void UpdateOneThreat(Threat obj)
        {
            var db = new LiteDatabase(@"Data.db");
            var col = db.GetCollection<Threat>("threats");
            col.Update(obj);
            db.Dispose();
        }

        public static Threat GetById(int objid)
        {
            var db = new LiteDatabase(@"Data.db");
            var col = db.GetCollection<Threat>("threats");
            Threat obj = (Threat)col.FindOne(x => x.Id == objid);
            db.Dispose();
            return obj;
        }

        public static void DelById(int objid)
        {
            var db = new LiteDatabase(@"Data.db");
            var col = db.GetCollection<Threat>("threats");
            if (!col.Delete(objid))
                MessageBox.Show("Ошибка удаления! \n Записи с таким Id не существует!");
            db.Dispose();
        }
    }
}
