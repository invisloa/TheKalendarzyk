using Kalendarzyk.Data;
using Kalendarzyk.Models.EventTypesModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKalendarzyk.Services
{
    class Factory
    {
        // Event Repository Singleton Pattern
        private static IEventRepository _eventRepository;

        public static IEventRepository GetEventRepository => _eventRepository;
        
        public static async Task<IEventRepository> InitializeEventRepository()  // TODO xxx USE IT SOMEWHERE AT STARTUP
        {
            if (_eventRepository == null)
                _eventRepository = await SQLiteRepository.CreateAsync();
            return _eventRepository;
        }
    }
}
