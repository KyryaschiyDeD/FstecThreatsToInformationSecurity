using System.Collections.Generic;

namespace FstecThreatsToInformationSecurity.classes
{
    public class Report // Класс для создания "отчёта" после обновления
    {
        public static int AllOperation { get { return countUpdate + countNew + countDel; } }
        public static int countUpdate = 0; // Количество обновлённых записей
        public static int countNew = 0; // Количество новых записей
        public static int countDel = 0; // Количество удалённых записей
        public int Id { get; set; } // Id записи
        public int type; // 0 - новая запись, 1 - обновлённая, 2 - удалили
        public List<string> ParamName { get; set; }  // Список параметров
        public List<string> Data { get; set; } // Список новых данных
        public List<string> OldData { get; set; } // Список старых данных
    }
}
