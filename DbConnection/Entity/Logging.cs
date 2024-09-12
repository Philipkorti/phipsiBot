using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnection.Entity
{
    public class Logging
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime date { get; set; }

        public string? LogLevel { get; set; }

        public string? Logger { get; set; }

        public string? Message { get; set; }

        public string? Exception { get; set; }
    }
}
