using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class TomatoesOmlet : Omlett
    {
        #region Fields
        /// <summary>
        /// This field contains the price at which the OMlett is being sold.
        /// </summary>
        private double purchaseAmount;

        /// <summary>
        /// This is the number of mushrooms it takes to cook one tomatoes omlett.
        /// </summary>
        private int ingredientsTomatoes;
        #endregion

        #region Constructor
        public TomatoesOmlet()
        {
            this.purchaseAmount = 2.50;
            this.ingredientsTomatoes = 1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets method of the field of purchaseAmount.
        /// </summary>
        new public double PurchaseAmount
        {
            get { return this.purchaseAmount; }
        }

        /// <summary>
        /// Gets method of the firld Property of ingredientsMushroom.
        /// </summary>
        public int IngredientsTomatoes
        {
            get { return this.ingredientsTomatoes; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// With this method, the foodstuffs that are needed for this dish are consumed.
        /// </summary>
        /// <param name="food">Is an array from the interface with methods.</param>
        /// <param name="gameOver"></param>
        /// <returns>A list goes back with the groceries.</returns>
        override public List<(int,int)> RemoveProduct(IAddFood[] food, out bool gameOver)
        {
            bool food1 = false;
            bool food2 = false;
            gameOver = false;
            List<(int,int)> ingredients = new List<(int, int)> ();
            food1 = food[0].RemoveFood(ingredients, this.IngredientsEggs);
            food2 = food[2].RemoveFood(ingredients, this.IngredientsTomatoes);

            if(food1 || food2)
            {
                gameOver = true;
            }
            return ingredients;
        }
        #endregion
    }
}
