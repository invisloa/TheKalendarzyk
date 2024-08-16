using SQLite;

namespace Kalendarzyk.Models.EventModels
{
    [Table("MicroTask")]
    public partial class MicroTask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Primary Key

        [Indexed]
        public int EventTypeModelId { get; set; } // Foreign Key to EventTypeModel

        [MaxLength(255)]
        public string Title { get; set; }

        public bool IsCompleted { get; set; }

        public MicroTask() { }

        public MicroTask(string title, int eventTypeModelId, bool isCompleted = false)
        {
            Title = title;
            EventTypeModelId = eventTypeModelId;
            IsCompleted = isCompleted;
        }
    }
}
