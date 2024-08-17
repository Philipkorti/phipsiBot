using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class Tomatoes :IAddFood
    {
        #region Fields
        /// <summary>
        /// Is the purchase price of tomatoes.
        /// </summary>
        private double purchasePrice;

        /// <summary>
        /// Is the number when they are bed.
        /// </summary>
        private int cycle;

        /// <summary>
        /// Is the number how much you buy at once.
        /// </summary>
        private int pallets;

        /// <summary>
        /// Is a list of the products.
        /// </summary>
        private List<int> inStock;
        #endregion

        #region Constructor
        public Tomatoes()
        {
            this.purchasePrice = 0.20;
            this.cycle = 20;
            this.pallets = 25;
            inStock = new List<int>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets method of purchaseprice.
        /// </summary>
        public double PurchasePrice
        {
            get { return this.purchasePrice; }
        }

        /// <summary>
        /// Gets method of cycle.
        /// </summary>
        public int Cycle
        {
            get { return this.cycle; }
        }

        /// <summary>
        /// Gets method of pallets.
        /// </summary>
        public int Pallets
        {
            get { return this.pallets; }
        }

        /// <summary>
        /// Gets method of inStock.
        /// </summary>
        public List<int> InStock
        {
            get { return this.inStock; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// With this method you can add new products.
        /// </summary>
        /// <param name="person">Are all data about the player.</param>
        /// <returns>Returns if the player is game over.</returns>
        public bool AddFood(Person person)
        {
            int count = person.InStock + this.Pallets;
            if(count <= 60)
            {
                person.Money -= this.PurchasePrice * this.Pallets;
                for (int i = 0; i < this.Pallets; i++)
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
        /// It removes food of your InStock List.
        /// </summary>
        /// <param name="person">The current user.</param>
        public void ControlFood(Person person)
        {
            for(int i = this.InStock.Count - 1; i >= 0; i--)
            {
                if (this.InStock[i]> this.Cycle)
                {
                    this.InStock.RemoveAt(i);
                    person.InStock--;
                }
            }
        }

        /// <summary>
        /// This method count up the rounds of the tomatoes.
        /// </summary>
        public void AddRounds()
        {
            for (int i = 0;i < this.InStock.Count; i++)
            {
                this.inStock[i]++;
            }
        }

        /// <summary>
        /// This method removes the tomatoes from the list as they are consumed.
        /// </summary>
        /// <param name="ingredients">Is a list og groceies that are needed right now.</param>
        /// <param name="count">Is the number of eggs needed for the recipe.</param>
        /// <returns>The user is game over than true otherwise false.</returns>
        public bool RemoveFood(List<(int,int)> ingredients, int count)
        {
            int num = InStock.Count - count;
            for(int i = this.InStock.Count-1; i > num -1; i--)
            {
                if(this.InStock.Count != 0)
                {
                    ingredients.Add((Cycle: this.cycle, Item: this.InStock[i]));
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
        /// This method calculates the new purchase value.
        /// </summary>
        /// <param name="inf">The new no-purchase value.</param>
        public void Inflation(double inf)
        {
            purchasePrice += inf;
            if(purchasePrice < 0)
            {
                this.purchasePrice = 0.01;
            }
            purchasePrice = Math.Round(purchasePrice, 2);
        }

        /// <summary>
        /// Returns the price;
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
