using DbConnection.Context;
using DbConnection.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DbServices
{
    public class KitchenGameService
    {
        public static void AddKitchenGame(string username, double money, int rounds, int productsSold, int countGames)
        {
            User user = UserServices.GetUerByUsername(username);

            using (var db = new BotContext())
            {
                db.KitchenGames.Add(new KitchenGame() { UserId = user.Id, Money = money, Rounds = rounds, ProductsSold = productsSold, CountGames =  countGames});
                db.SaveChanges();
            }
        }

        public static KitchenGame GetKichenGameByUsername(string username)
        {
            User user = UserServices.GetUerByUsername(username);
            KitchenGame game;
            using(var db = new BotContext())
            {
                game = db.KitchenGames.SingleOrDefault(game => game.UserId == user.Id);
            }

            return game;
        }

        public static void UpdateKitchenGameByUsername(string username, double money, int rounds, int productsSold)
        {
            KitchenGame kitchenGame = GetKichenGameByUsername(username);
           
            if(kitchenGame != null)
            {
                kitchenGame.Money += money;
                kitchenGame.CountGames++;
                kitchenGame.Rounds += rounds;
                kitchenGame.ProductsSold += productsSold;
                using (var db = new BotContext())
                {
                    db.KitchenGames.Update(kitchenGame);
                    db.SaveChanges();
                }
            }
            else
            {
                AddKitchenGame(username,money, rounds, productsSold,1);
            }
            
        }

        public static List<KitchenGame> GetTopKitchenGameUser()
        {
            List<KitchenGame> kitchenGames = new List<KitchenGame>();
            using (var db = new BotContext())
            {
                kitchenGames = db.KitchenGames.OrderByDescending(d => d.Rounds / d.CountGames).Take(5).ToList();
            }
            return kitchenGames;
        }
    }
}
