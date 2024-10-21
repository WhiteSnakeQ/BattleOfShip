using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    internal class ComponentOfShip : Point
    {
        private string DestroyDeck = "X";
        private Ship Ship;

        
        public ComponentOfShip(string Deck, int x, int y, bool Player, Ship ship) : base(x, y)
        {
            Field = Deck;
            Close = 1;
            if (Player) { Maps.MapOfPlayer[x, y] = this; } // Дележка между Игроком и ИИ
                else { Maps.MapOfEnemy[x, y] = this;  }
            Ship = ship;
        }

        public override bool Check() // Проверка жив ли параход
        {
            if(this.Ship.Health <= 0) { return true; }
            else
                return false;
        }
        public override void TakeDamage(bool Hide = false) // Попадание  / Hide - дележка между Игроком и ИИ
        {
            Field = DestroyDeck;
            if(Alive) 
            {
                if (Hide)
                {
                    Maps.EnemyMapToShow[xPos, yPos] = DestroyDeck;
                }
                Ship.Health -= 1;
            }
            Alive = false;
        }
        public override void StatusOfComponent(bool Hide = false) // Отчет о состоянии парахода
        {
            if (Ship.Health >= 1 && Hide == false) // дележка между Игроком и ИИ
            {
                Program.AdditionMessageForGameScreen += $"{Ship._Size} - палубный поврежден!\n\n"; // При попадании в ИИ, Не выводится в консоль
            }
            else if (Ship.Health <= 0)
            {
                Program.CoolString($"{Ship._Size} - палубный корабль уничтожен!!!!", ConsoleColor.Magenta);
            }
        }

        public override string ToString() => Field; // Напечатай себя
    }
}
