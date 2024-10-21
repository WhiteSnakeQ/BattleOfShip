using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BattleOfShip
{
    abstract class StructOfShip
    {
        protected int xPos;

        protected int yPos;

        protected ComponentOfShip[] Body; // Масив клеток корабля, с его помощью строится судно

        protected int _Health;
        
        protected int Size; 
        public int _Size { get { return Size; } }

        public readonly int StartSize; // для подсчитывания хп(знаю можно было каждый параход 1 жизнь, а не по размеру, но так захотелось

        protected bool Up; // поворот судна

        protected bool Player; // определить ИИ или Игрок

        public StructOfShip(int size)
        {
            Size = size;
            StartSize = Size;
            _Health = Size;
        }
    }
}
