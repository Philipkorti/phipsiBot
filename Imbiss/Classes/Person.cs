using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class Person
    {
        #region Fields
        /// <summary>
        /// Is the player's name.
        /// </summary>
        private string name;

        /// <summary>
        /// Is the money what the player has.
        /// </summary>
        private double money;

        /// <summary>
        /// is the number of products the player has.
        /// </summary>
        private int inStock;

        /// <summary>
        /// Is the number of rounds the player has survived.
        /// </summary>
        private int rounds;

        private double earnedMoney;

        private int soldProducts;
        #endregion

        #region Properties
        /// <summary>
        /// Gets and sets method of name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double EarnedMoney
        {
            get { return earnedMoney; }
            set { earnedMoney = value; }
        }

        public int SoldProducts
        {
            get { return soldProducts; }
            set { soldProducts = value; }
        }

        /// <summary>
        /// Gets and sets method of Money.
        /// </summary>
        public double Money
        {
            get { return money; }
            set { money = value; }
        }

        /// <summary>
        /// Gets and sets method of InStock.
        /// </summary>
        public int InStock
        {
            get { return inStock; }
            set { inStock = value; }
        }

        /// <summary>
        /// Gets and sets method of Rounds.
        /// </summary>
        public int Rounds
        {
            get { return rounds; }
            set { rounds = value; }
        }
        #endregion

        #region Constructor
        public Person(string name, double money, int inStock)
        {
            this.name = name;
            this.money = money;
            this.inStock = inStock;
            this.Rounds = 0;
        }
        #endregion
    }
}
