using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Events
{
    public class ErrorTaskArgs : EventArgs
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Status { get; set; }
        public string Username { get; set; }

    }
}
