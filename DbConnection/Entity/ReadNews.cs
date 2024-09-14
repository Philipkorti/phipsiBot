using Enums.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnection.Entity
{
    public class ReadNews
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public string? Name { get; set; }

        public string? GameLink { get; set; }

        public NewsType NewsType { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
