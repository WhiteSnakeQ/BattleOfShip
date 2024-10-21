using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    internal partial class Program            // Тут написана сам ход игры - Чтобы пройти дальше нужно со всем ознакомится
    {
        public static Random rnd = new Random();
        public static Ship[] PlayerShip;
        public static Ship[] EnemyShip;
        private static bool playGame = true;
        private static bool isPlaying = true;
        public static string TextForScreenMain;
        public static string AdditionMessageForGameScreen;
        public static ConsoleColor MassageConsoleColor;
        private static int _CountStep = 0;
        public static int CountStep
        { get { return _CountStep; } }

        private const string ABC = "ABCDEFGHIJ"; 

        static void Main(string[] args)
        {
            while (playGame)
            {                                                                  // Просто вынес сюда а не в отдельный метод, личная заморочка
                PlayerShip = new Ship[10]
            {
                new Ship(4),
                new Ship(3),
                new Ship(3),
                new Ship(2),
                new Ship(2),
                new Ship(2),
                new Ship(1),
                new Ship(1),
                new Ship(1),
                new Ship(1)
            };
                Maps.PlayerHp = 1;
                foreach (var ship in PlayerShip)
                {
                    Maps.PlayerHp += ship._Size;
                }
                Maps.PlayerHp -= 1;

                EnemyShip = new Ship[10]
                {
                new Ship(4),
                new Ship(3),
                new Ship(3),
                new Ship(2),
                new Ship(2),
                new Ship(2),
                new Ship(1),
                new Ship(1),
                new Ship(1),
                new Ship(1)
                };
                Maps.EnemyHP = 1;
                foreach (var ship in EnemyShip)
                {
                    Maps.EnemyHP += ship._Size;
                }
                Maps.EnemyHP -= 1;                            ///////////////////////////////////////////////////

                Maps.Build();  // Создвем карты или обновляем

                CoolString("Вы играете в Морской бой!", ConsoleColor.Magenta);
                Console.WriteLine("Расположите суда на карте {0} Х {1} !", Maps.xSize, Maps.ySize);
                Console.WriteLine("1 - Ручное раставление  / 2 - Автоматическое");
                if (GetDigit(1, 2) == 1)
                {
                    CoolString("Управление стрелочками!", ConsoleColor.Magenta);
                    CoolString("Поверните судно (R) или Нажмите ENTER", ConsoleColor.Magenta);
                    foreach (var ship in PlayerShip)
                    {
                        Console.WriteLine("Расположите судно длиной - {0}", ship._Size);
                        int x = 0, y = 0;
                        bool Up = false;
                        
                        ManualBuild(Up, ship._Size, x, y);
                        DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                        ManualBuild(Up, ship._Size, x, y, "Q");
                        while (true)
                        {
                            ConsoleKey consoleKey = Console.ReadKey(true).Key;
                            if (consoleKey == ConsoleKey.R)
                            {
                                if(x < Maps.xSize - ship._Size && y <= Maps.ySize - ship._Size)
                                {
                                    Up = !Up;
                                    ManualBuild(Up, ship._Size, x, y);
                                    DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                    ManualBuild(Up, ship._Size, x, y, "Q");
                                }                               
                            }
                            else if (consoleKey == ConsoleKey.DownArrow)
                            {
                                if (y < Maps.ySize - ship._Size)
                                {
                                    y += 1;                                    
                                }
                                else if (y <= 8 && !Up)
                                {
                                    y += 1;
                                }
                                else continue;
                                ManualBuild(Up, ship._Size, x, y);
                                DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                ManualBuild(Up, ship._Size, x, y, "Q");
                            }
                            else if (consoleKey == ConsoleKey.UpArrow)
                            {
                                if (y > 0)
                                {
                                    y -= 1;
                                    ManualBuild(Up, ship._Size, x, y);
                                    DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                    ManualBuild(Up, ship._Size, x, y, "Q");
                                }
                            }
                            else if (consoleKey == ConsoleKey.LeftArrow)
                            {
                                if (x >= 1)
                                {
                                    x -= 1;
                                    ManualBuild(Up, ship._Size, x, y);
                                    DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                    ManualBuild(Up, ship._Size, x, y, "Q");
                                }
                            }
                            else if (consoleKey == ConsoleKey.RightArrow)
                            {
                                if (x < Maps.xSize - ship._Size)
                                {
                                    x += 1;
                                }
                                else if (x <= 8 && Up)
                                {
                                    x += 1;
                                }
                                else continue;
                                ManualBuild(Up, ship._Size, x, y);
                                DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                ManualBuild(Up, ship._Size, x, y, "Q");
                            }
                            else if (consoleKey == ConsoleKey.Enter)
                            {
                                try
                                {
                                    ship.Place(x, y, Up, true);
                                    DrawMapForWinScreen(Mapp: Maps.PlayerMapToBuild);
                                    ManualBuild(Up, ship._Size, x, y, "B");
                                    break;
                                }
                                catch (Exception)
                                {
                                    ManualBuild(Up, ship._Size, x, y, "Q");
                                    CoolString("Упс, место уже Занято !", ConsoleColor.Red);
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    RELOAD(PlayerShip, true);
                }
                RELOAD(EnemyShip, false);

                TextForScreenMain = "Игра Началась!!!";
                MassageConsoleColor = ConsoleColor.Yellow;
                AdditionMessageForGameScreen = "Нажмите любую клавишу";
                isPlaying = true;

                while (isPlaying) 
                {
                    Attack();
                    if (isPlaying)
                    {
                        EnemyAttack();
                    }
                }
                Console.Clear();
            }

        CoolString("Взвращайтесь ! ", ConsoleColor.Magenta );
        Console.ReadKey();
        }
    }
}