using Newtonsoft.Json.Serialization;
using Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Data
{
    public class ErrorTaskData
    {
        public delegate void ObjectCreatedEventHandler(object sender, ErrorTaskArgs e);

        // Definiere ein Event basierend auf dem Delegat
        public static event ObjectCreatedEventHandler ObjectCreated;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public string Username { get; set; }

        public ErrorTaskData(string ? title, string? description, DateTime createdDate, string? status, string username)
        {
            Title = title;
            Description = description;
            CreatedDate = createdDate;
            Status = status;
            Username = username;
            OnObjectCreated(new ErrorTaskArgs() { Title = title, Description = description, CreatedDate = createdDate, Status = status, Username = username});
        }

        protected virtual void OnObjectCreated(ErrorTaskArgs e)
        {
            ObjectCreated?.Invoke(this, e);
        }
    }
}
