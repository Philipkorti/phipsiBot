using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Agreement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums.Enums;

namespace DbConnection.Entity
{
    [PrimaryKey("Id")]
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Username { get; set; }

        [DefaultValue(Groups.User)]
        public Groups Groups {  get; set; }

        public Int64 TimeInSecond { get; set; }
    }
}
