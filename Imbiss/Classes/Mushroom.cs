using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class Mushroom : IAddFood
    {
        #region Fields
        /// <summary>
        /// This is the purchase price of the mushroom per mushroom.
        /// You can only call them in the class.
        /// </summary>
        private double purchasePrice;

        /// <summary>
        /// This field indicates how many rounds an mushroom lasts.
        /// You can only call them in this class.
        /// </summary>
        private int cycle;

        /// <summary>
        /// This value indicates how many mushroom a pallet contains.
        /// </summary>
        private int pallets;

        private List<int> inStock;
        #endregion

        #region Constructor
        public Mushroom()
        {
            this.purchasePrice = 2.5;
            this.cycle = 10;
            this.pallets = 10;
            inStock = new List<int>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets for the price of mushroom.
        /// </summary>
        public double PurchasePrice
        {
            get { return this.purchasePrice; }
        }

        /// <summary>
        /// Gets for the field cycle.
        /// </summary>
        public int Cycle
        {
            get { return this.cycle; }
        }

        /// <summary>
        /// Gets for the field pallets.
        /// </summary>
        public int Pallets
        {
            get { return this.pallets; }
        }

        /// <summary>
        /// Gets or sets for the List with mushrooms.
        /// </summary>
        public List<int> InStock
        {
            get { return this.inStock; }
            set { this.inStock = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// In this method, new mushrooms are added to the list and calculated how much it costs in total.
        /// </summary>
        /// <param name="person">The current user.</param>
        /// <returns>Returns true wehen the user is game over otherwise false.</returns>
        public bool AddFood(Person person)
        {
            int count = person.InStock + this.Pallets;
            if(count <= 60)
            {
                person.Money -= this.PurchasePrice * this.Pallets;
                for(int i = 0; i < this.Pallets; i++)
                {
                    this.InStock.Add(0);
                }
                person.InStock = count;
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method looks at which mushroom have already expired and removes them from list of eggs.
        /// </summary>
        /// <param name="person">The current user.</param>
        public void ControlFood(Person person)
        {
            for(int i = this.InStock.Count - 1; i >= 0; i--)
            {
                if (this.InStock[i] > this.Cycle)
                {
                    this.InStock.RemoveAt(i);
                    person.InStock--;
                }
            }
        }

        /// <summary>
        /// This method count up the rounds of mushroom.
        /// </summary>
        public void AddRounds()
        {
            for(int i = 0; i < this.InStock.Count; i++)
            {
                this.InStock[i]++;
            }
        }

        /// <summary>
        /// This method removes the mushroom from the list as they are consumed.
        /// </summary>
        /// <param name="ingredients">Is a list of groceries that are needed right now.</param>
        /// <param name="count">is the number of mushroom needed for the recipe.</param>
        /// <returns>returned true wthen the player is game over otherwise false.</returns>
        public bool RemoveFood(List<(int,int)> ingredients, int count)
        {
            int num = this.InStock.Count - count;
            for(int i = this.InStock.Count - 1; i > num - 1; i--)
            {
                if(this.InStock.Count != 0)
                {
                    ingredients.Add((Cycle: this.Cycle, Item2: this.InStock[i]));
                    this.InStock.RemoveAt(i);
                }
                else
                {
                    return true;
                }
            }
            return false ;
        }

        /// <summary>
        /// This method calculates the new purchase value.
        /// </summary>
        /// <param name="inf">The new no-purchase value.</param>
        public void Inflation(double inf)
        {
            this.purchasePrice += inf;
            if(this.PurchasePrice < 0)
            {
                this.purchasePrice = 0.01;
            }
        }

        /// <summary>
        /// Returns the price.
        /// </summary>
        /// <returns></returns>
        public double GetPrice()
        {
            return this.PurchasePrice;
        }

        public int GetInStock()
        {
            return this.InStock.Count;
        }
        #endregion
    }
}
