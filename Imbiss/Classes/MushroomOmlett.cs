using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Classes
{
    public class MushroomOmlett : Omlett
    {
        #region Fields
        /// <summary>
        /// This field contains the price at which the Omlett is being sold.
        /// </summary>
        private double purchaseAmount;

        /// <summary>
        /// This is the number of mushrooms it takes to cook one mushroom Omlett.
        /// </summary>
        private int ingredientsMushroom;
        #endregion

        #region Constructor
        public MushroomOmlett()
        {
            this.purchaseAmount = 10;
            this.ingredientsMushroom = 1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets method of the Property of purchaseAmount.
        /// </summary>
        new public double PurchaseAmount
        {
            get { return this.purchaseAmount; }
        }

        public int IngredientsMushroom
        {
            get { return this.ingredientsMushroom; }
        }
        #endregion

        #region Public Method
        override public List<(int,int)> RemoveProduct(IAddFood[] food, out bool gameOver)
        {
            bool food1 = false;
            bool food2 = false;
            gameOver = false;
            List<(int,int)> ingredients = new List<(int,int)>();
            food1 = food[0].RemoveFood(ingredients, this.IngredientsEggs);
            food2 = food[1].RemoveFood(ingredients, this.IngredientsMushroom);
            if(food1|| food2)
            {
                gameOver = true;
            }
            return ingredients;
        }
        #endregion
    }
}
