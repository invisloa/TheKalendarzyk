﻿using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kalendarzyk.Models.EventTypesModels
{
    [SQLite.Table("EventGroupModel")]

    public class EventGroupModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary Key
        [MaxLength(55)]
        [Unique]
        public string Title { get; set; }

        public int IconModelId { get; set; } // Foreign Key

        [Ignore]
        public IconModel SelectedVisualElement { get; set; }

        public EventGroupModel() { }

        public EventGroupModel(string title, IconModel icon)
        {
            Title = title;
            SelectedVisualElement = icon;
        }

        public override string ToString()
        {
            return Title;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            EventGroupModel other = (EventGroupModel)obj;
            return Id == other.Id;
        }

    }
}
