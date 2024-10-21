using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    static class Maps
    {
        // Размеры
        readonly static public int xSize = 10;
        readonly static public int ySize = 10;
        // Далее набор карт
        static public Point[,] MapOfPlayer = new Point[xSize, ySize];
        static public Point[,] MapOfEnemy = new Point[xSize, ySize];
        static public string[,] EnemyMapToShow = new string[xSize, ySize];
        static public string[,] PlayerMapToBuild = new string[xSize, ySize];

        // здоровье всех
        private static int _PlayerHp;
        public static int PlayerHp
        {
            get { return _PlayerHp; }
            set
            {
                _PlayerHp = value;
                if (_PlayerHp <= 0)
                {
                    Program.AdditionMessageForGameScreen = "";
                    Program.TextForScreenMain = "";
                    Program.GameScreen(false);
                    Program.CoolString("Противник уничтожил послеедний корабль!");
                    Console.ReadKey();
                    Program.Win("Вы Проиграли!", Program.EnemyShip, false);
                }
            }
        }
        private static int _EnemyHP;
        public static int EnemyHP
        {
            get { return _EnemyHP; }
            set
            {
                _EnemyHP = value;
                if (_EnemyHP <= 0)
                {
                    Program.AdditionMessageForGameScreen = "";
                    Program.TextForScreenMain = "";
                    Program.GameScreen(false);
                    Program.CoolString("Вы уничтожили послеедний корабль!");
                    Console.ReadKey();
                    Program.Win("Поздравляю, вы уничтожили врага!", Program.PlayerShip, true);
                }
            }
        }

        public static void Build()
        {
            for (int i = 0; i < ySize; i++) 
            {
                for (int j = 0; j < xSize; j++)
                {
                    MapOfPlayer[j,i] = new Point(j, i);
                    MapOfEnemy[j,i] = new Point(j, i);
                    EnemyMapToShow[j, i] = "Q";
                    PlayerMapToBuild[j, i] = "Q";
                }
            }
        }
    }
}
