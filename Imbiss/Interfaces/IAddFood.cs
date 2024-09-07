using Imbiss.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Interfaces
{
    public interface IAddFood
    {
        /// <summary>
        /// is to add new products.
        /// </summary>
        /// <param name="person">Are all data from one player.</param>
        /// <returns>Returns a value if the player is GameOver</returns>
        bool AddFood(Person person);

        /// <summary>
        /// Is to see if the products are still good.
        /// </summary>
        /// <param name="person">Are all data from one player</param>
        void ControlFood(Person person);

        /// <summary>
        /// Adds new rounds.
        /// </summary>
        void AddRounds();

        /// <summary>
        /// Revoves products from player.
        /// </summary>
        /// <param name="ingredients">Is a list of products</param>
        /// <param name="count">How many products do you need</param>
        /// <returns>Returns a value if the player is GameOver.</returns>
        bool RemoveFood(List<(int, int)> ingredients, int count);

        /// <summary>
        /// Is for the inflation of purchase prices.
        /// </summary>
        /// <param name="inf">Is the value that is added to the purchase price.</param>
        void Inflation(double inf);

        /// <summary>
        /// Returns the purchase price.
        /// </summary>
        /// <returns>Returns the purchase price.</returns>
        double GetPrice();

        int GetInStock();
    }
}
