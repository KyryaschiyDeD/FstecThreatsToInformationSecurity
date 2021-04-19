using System;

namespace FstecThreatsToInformationSecurity.classes
{
    public class Threat
    {
        public int Id { get; set; } // Индинтификатор
        public string Name { get; set; } // Наименование
        public string Description { get; set; } // Описание
        public string Source { get; set; } // Источник
        public string ObjectThreat { get; set; } // Объект воздействия
        public bool PrivacyPolicy { get; set; } // Нарушение конфиденциальности
        public bool Integrity { get; set; } // Нарушение целостности 
        public bool Availability { get; set; } // Нарушение доступности
        public DateTime DateCreate { get; set; } // Дата включения угрозы в БнД УБИ
        public DateTime DateUpdate { get; set; } // Дата последнего изменения данных в БнД УБИ
        public DateTime DateUpload { get; set; } // Дата последнего обновления в локальной БД
        public Threat() { }
        public Threat
            (
            int id, string name, 
            string description, string source, 
            string objectThreat, bool privacyPolicy, 
            bool integrity, bool availability, 
            DateTime dateCreate, DateTime dateUpdate,
            DateTime dateUpload
            )
        {
            Id = id;
            Name = name;
            Description = description;
            Source = source;
            ObjectThreat = objectThreat;
            PrivacyPolicy = privacyPolicy;
            Integrity = integrity;
            Availability = availability;
            DateCreate = dateCreate;
            DateUpdate = dateUpdate;
            DateUpload = dateUpload;
        }
    }
}