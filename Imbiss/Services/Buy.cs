using DSharpPlus.CommandsNext;
using Imbiss.Classes;
using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Services
{
    public class Buy
    {
        public static bool BuyOmlet(Person person, IAddFood[] food, CommandContext ctx)
        {
            bool gameOver = false;
            Omlett omlett = new Omlett();
            MushroomOmlett mushroomOmlett = new MushroomOmlett();
            TomatoesOmlet tomatoesOmlet = new TomatoesOmlet();
            Random rand = new Random();
            List<(int, int)> ingredients = new List<(int, int)>();
            int randnum = rand.Next(1,10);
            
            if(randnum >=1 && randnum <= 5)
            {
                person.Money += omlett.PurchaseAmount;
                person.EarnedMoney += omlett.PurchaseAmount;
                ingredients.AddRange(omlett.RemoveProduct(food, out gameOver));
                person.InStock -= 3;
            }
            else
            {
                if(randnum > 5 && randnum < 10)
                {
                    person.Money += tomatoesOmlet.PurchaseAmount;
                    person.EarnedMoney += tomatoesOmlet.PurchaseAmount;
                    ingredients.AddRange(tomatoesOmlet.RemoveProduct(food, out gameOver));
                    person.InStock -= 4;
                }
                else
                {
                    person.Money += mushroomOmlett.PurchaseAmount;
                    person.EarnedMoney += mushroomOmlett.PurchaseAmount;
                    ingredients.AddRange(mushroomOmlett.RemoveProduct(food,out gameOver));
                    person.InStock -= 4;
                }
            }

            if (!gameOver)
            {
                gameOver = Gameover.FoodBad(ingredients, ctx);
            }
            else
            {
                ctx.Channel.SendMessageAsync("You have no more products. That's why you are game over.");
            }
            person.SoldProducts++;
            return gameOver;
        }
    }
}
