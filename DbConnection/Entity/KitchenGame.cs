using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnection.Entity
{
    public class KitchenGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Rounds { get; set; }
        public double Money {  get; set; }
        public int ProductsSold { get; set; }
        public int CountGames { get; set; }
    }
}
