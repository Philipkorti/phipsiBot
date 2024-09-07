using DSharpPlus.CommandsNext;
using Imbiss.Classes;
using Imbiss.Interfaces;
using Services.DbServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Services
{
    public class Game
    {
        private readonly CommandContext ctx;
        public Game(CommandContext ctx) 
        {
            this.ctx = ctx;
            GameStart(ctx);
        }

        private static void GameStart(CommandContext ctx)
        {
            bool gameend = false;
            
            
            do
            {
                Egg eggs = new Egg();
                Mushroom mushroom = new Mushroom();
                Tomatoes tomatoes = new Tomatoes();
                IAddFood[] food = new IAddFood[3];

                food[0] = eggs;
                food[1] = mushroom;
                food[2] = tomatoes;
                int num = Menu.ShowMenu(ctx);
                Person person = new Person(ctx.User.Username, 50, 0);
                switch (num)
                {
                    case 1:
                        {
                            Rounds.RoundsManagement(food, person, ctx);
                            break;
                        }
                    case 2:
                        {
                            gameend = true;
                            break;
                        }
                }
                KitchenGameService.UpdateKitchenGameByUsername(person.Name, person.EarnedMoney, person.Rounds, person.SoldProducts);
            } while (!gameend);
          
        }
    }
}
