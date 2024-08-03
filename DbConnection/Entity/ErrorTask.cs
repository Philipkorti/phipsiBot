using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnection.Entity
{
    public class ErrorTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? Status { get; set; }

        public int UserId { get; set; }
    }
}
