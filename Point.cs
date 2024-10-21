using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    internal class Point
    {
        public string Field = "B"; // Текстурка ХД
        public int Close = 0; // 0 - Пусто / 1 - Судно
        public int xPos;
        public int yPos;
        public bool Alive = true; // false - Попали
        public bool SideDestroy = false;

        public Point(int x, int y) 
        {
            xPos = x;
            yPos = y;
        }
        public virtual void TakeDamage(bool Hide = false)
        {
            if (Alive) 
            { 
                Field = "*";
                if (Hide) // тот же Hide, Делим на ИИ и Игрока
                {
                    Maps.EnemyMapToShow[xPos, yPos] = "*";
                }
            }
            Alive = false;
        }

        public override string ToString() => $"{Field}"; // Напечатай себя

        public virtual bool Check( ) { return false; } // Нужен чтобы не делать явное приведеие Point к ComponentOfShip
        public virtual void StatusOfComponent(bool Hide = false) { } // Нужен чтобы не делать явное приведеие Point к ComponentOfShip

    }
}
