using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class Omlett
    {
        #region Fields
        /// <summary>
        /// The retail value of how much an omlet costs.
        /// </summary>
        private double purchaseAmount;

        /// <summary>
        /// The value of how many eggs it takes to make an omlet
        /// </summary>
        private int ingredientsEgg;
        #endregion

        #region Constructor
        public Omlett()
        {
            this.purchaseAmount = 1.50;
            this.ingredientsEgg = 3;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Method for purchaseAmount.
        /// </summary>
        public double PurchaseAmount
        {
            get
            {
                return this.purchaseAmount;
            }
        }

        /// <summary>
        /// Gets Method for ingredientsEggs.
        /// </summary>
        public int IngredientsEggs
        {
            get { return this.ingredientsEgg; }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// This method removes the products from the warehouse.
        /// </summary>
        /// <param name="food">Is the connection to the classes of the products.</param>
        /// <param name="gameover">Is the value if the player is game over</param>
        /// <returns>Returns a list of products.</returns>
        virtual public List<(int,int)> RemoveProduct(IAddFood[] food, out bool gameover)
        {
            List<(int,int)> ingredients = new List<(int, int)> ();
            gameover = food[0].RemoveFood(ingredients, this.ingredientsEgg);
            return ingredients;
        }
        #endregion
    }
}
