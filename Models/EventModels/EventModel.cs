﻿using Kalendarzyk.Models.EventTypesModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalendarzyk.Models.EventModels
{
	public class EventModel
    {
        private TimeSpan _defaulteventRemindertime = TimeSpan.FromHours(24);

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }  // Use int for autoincrement

        [MaxLength(55)]
        public string Title { get; set; }
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
        [MaxLength(255)]
        public string Description { get; set; }

        public bool WasShown { get; set; }

        public bool IsCompleted { get; set; }

        public int EventTypeId { get; set; }

        // Ignored properties (not stored directly in the database)
        [Ignore]
        public EventTypeModel EventType { get; set; } 

        public TimeSpan ReminderTime { get; set; }

        public EventModel() { }

        public EventModel(string title, string description, DateTime startTime, DateTime endTime, EventTypeModel eventType,
                                  bool isCompleted = false, TimeSpan? remindTime = null, bool wasShown = false, Guid? id = null, int? notificationID = null, bool usesNotification = false)
        {
            ReminderTime = remindTime ?? _defaulteventRemindertime;
            Title = title;
            Description = description;
            StartDateTime = startTime;
            EndDateTime = endTime;
            EventType = eventType; // Use concrete type
            IsCompleted = isCompleted;
            WasShown = wasShown;
        }
    }
}
