using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using Imbiss.Classes;
using Imbiss.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imbiss.Services
{
    public class Rounds
    {
        private static int RoundShow(Person person, IAddFood[] food, CommandContext ctx)
        {
            int num;
            bool check;
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.AddField("Preise", $"Egg: {food[0].GetPrice()} Euro\nMushroom: {food[1].GetPrice()} Euro\nTomatoes: {food[2].GetPrice()} Euro");
            embedBuilder.AddField("User Information", $"Name: {person.Name}\nRounds: {person.Rounds}\nMoney: {person.Money} Euro");
            if(person.Rounds < 50)
            {
                embedBuilder.AddField("Lager", $"Eggs: {food[0].GetInStock()}\nMushroom: {food[1].GetInStock()}\nTomatoes: {food[2].GetInStock()}\nGesamtes Lager: {person.InStock}");
            }
            embedBuilder.AddField("Benutzer Befehle", "0 Buy a pallet of eggs\n1 Buy a pack of mushrooms\n2 Buy a pallet of tomatoes\n3 To serve customers\n4 Discard: Throw away all spoiled food\n 5 nothing");
            ctx.Channel.SendMessageAsync(embedBuilder);
            do
            {
                var interactivity = ctx.Client.GetInteractivity();
                var message = interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Channel.Id == ctx.Channel.Id);
                check = int.TryParse(message.Result.Result.Content, out num);

                if(!check || num < 0 || num > 5)
                {
                    check = false;
                    ctx.Channel.SendMessageAsync("Die Eingabe muss eine Zahl sein und zwischen 0 und 5 sein!");
                }

            } while (!check);

            return num;
           
        }

        public static void RoundsManagement(IAddFood[] food, Person person, CommandContext ctx)
        {
            int input;
            bool check;
            bool gameover = false;
            int inflation = 10;
            double inf;

            do
            {
                if (inflation == person.Rounds)
                {
                    inflation += 10;
                    Random rand = new Random();
                    inf = rand.NextDouble();
                    inf -= 0.35;
                    inf = Math.Round(inf,2);
                    foreach(IAddFood foods in food)
                    {
                        foods.Inflation(inf);
                    }
                    ctx.Channel.SendMessageAsync($"Die Inflation beträgt {inf} Euro pro Produkt!");
                }

                input = RoundShow(person, food,ctx);
                person.Rounds++;
                gameover = Action(input,food,person,ctx);
            } while (!gameover);
        }

        private static bool Action(int inputnum, IAddFood[] food, Person person, CommandContext ctx)
        {
            bool gameover = false;

            switch(inputnum)
            {
                case 0:
                case 1:
                case 2:
                    {
                        if (food[inputnum].AddFood(person))
                        {
                            ctx.Channel.SendMessageAsync("Your storage is full!");
                        }
                        gameover = Gameover.OutOfMoney(person.Money, ctx);
                        break;
                    }
                case 3:
                    {
                        gameover = Buy.BuyOmlet(person, food, ctx);
                        break;
                    }
                case 4:
                    {
                        foreach (var item in food)
                        {
                            item.ControlFood(person);
                        }
                        break;
                    }
                case 5:
                    {
                        break;
                    }
            }
            foreach (var item in food)
            {
                item.AddRounds();
            }

            return gameover;
        }
    }
}
