using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Services
{
    public class Gameover
    {
        public static bool FoodBad(List<(int,int)> ingredients, CommandContext ctx)
        {
            bool gameover = false;

            for(int i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i].Item2 >= ingredients[i].Item1)
                {
                    gameover = true;
                }
            }

            if(gameover)
            {
                ctx.Channel.SendMessageAsync("You are Game over for selling bad groveries.");
            }

            return gameover;
        }

        public static bool OutOfMoney(double money,  CommandContext ctx)
        {
            bool gameover = false;

            if(money < 0)
            {
                gameover = true;
                ctx.Channel.SendMessageAsync("You are Game over because your money went into the minus!");
            }

            return gameover;
        }
    }
}
