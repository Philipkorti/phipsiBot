using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class Egg : IAddFood
    {
        #region Fields
        /// <summary>
        /// This is the purchase price of the eggs per egg.
        /// You can only call them in this class.
        /// </summary>
        private double purchasePrice;

        /// <summary>
        /// This field indicates how many rounds an egg lasts.
        /// You can only call them in this class.
        /// </summary>
        private int cycle;

        /// <summary>
        /// This value indicates how many eggs a pallet contains.
        /// </summary>
        private int pallets;

        /// <summary>
        /// The eggs are stored in this list.
        /// </summary>
        private List<int> inStock;
        #endregion
        #region Constructor
        public Egg()
        {
            purchasePrice = 0.30;
            cycle = 30;
            pallets = 25;
            inStock = new List<int>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets for the price of eggs.
        /// </summary>
        public double PurchasePrice
        {
            get { return this.purchasePrice; }
        }

        /// <summary>
        /// Gets for the field Cycle.
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
        /// Gets or sets for the List with eggs
        /// </summary>
        public List<int> InStock
        {
            get { return this.inStock; }
            set { this.inStock = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// In this method, new eggs are added to the list and calculated how it costs in total.
        /// </summary>
        /// <param name="person">The current user</param>
        /// <returns></returns>
        public bool AddFood(Person person)
        {
            int count = person.InStock + this.Pallets;
            if(count <= 60)
            {
                person.Money -= this.PurchasePrice * this.Pallets;
                for(int i = 0; i < this.Pallets; i++)
                {
                    this.inStock.Add(0);
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
        /// This method looks at which eggs have already expired and removes them from the list of eggs.
        /// </summary>
        /// <param name="person">The current user.</param>
        public void ControlFood(Person person)
        {
            for(int i = this.InStock.Count - 1; i >= 0; i--)
            {
                if (this.InStock[0] > this.Cycle)
                {
                    this.InStock.RemoveAt(i);
                    person.InStock--;
                }
            }
        }

        /// <summary>
        /// This method count up the rounds of the eggs.
        /// </summary>
        public void AddRounds()
        {
            for (int i = 0;i<this.InStock.Count;i++)
            {
                this.InStock[i] ++;
            }
        }

        /// <summary>
        /// This method removes the eggs from the list as they are consumed.
        /// </summary>
        /// <param name="ingredients">Is a list of groceries that are needed right now.</param>
        /// <param name="count">Is the number of eggs needed for the recipe.</param>
        /// <returns>Is the user game over true otherwise false.</returns>
        public bool RemoveFood(List<(int,int)> ingredients, int count)
        {
            int num = this.InStock.Count-count;
            for (int i = this.InStock.Count - 1; i > num - 1; i--)
            {
                if(this.InStock.Count != 0)
                {
                    ingredients.Add((Cycle: this.Cycle, Item: this.InStock[i]));
                    this.InStock.RemoveAt(i);
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// this method calculates the new purchase value.
        /// </summary>
        /// <param name="inf">The new no-purchase value.</param>
        public void Inflation(double inf)
        {
            this.purchasePrice += inf;
            if(this.purchasePrice < 0)
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
