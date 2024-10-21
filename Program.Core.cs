using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    partial class Program
    {
        private static int posX_last = 0;
        private static int posY_last = 0;
        public static void Win(string messgae, Ship[] Ships, bool Player = true)  // Метод финального ,,Меню,, принимает массив судов победителя, сообщение(Win|Defeat), булевское значение( игрок или ии)
        {
            int Deck1 = 0, Deck2 = 0, Deck3 = 0, Deck4 = 0;
            bool BDeck1 = false, BDeck2 = false, BDeck3 = false, BDeck4 = false;
            foreach (Ship ship in Ships)
            {
                if (ship.Health <= 0)
                    continue;
                if (ship.StartSize == 4)
                {
                    Deck4 += 1;
                    BDeck4 = true;
                }
                else if (ship.StartSize == 3)
                {
                    Deck3 += 1;
                    BDeck3 = true;
                }
                else if (ship.StartSize == 2)
                {
                    Deck2 += 1;
                    BDeck2 = true;
                }
                else
                {
                    Deck1 += 1;
                    BDeck1 = true;
                }
            }
            string CountShip;
            if (Player)
            {
                CountShip = "У вас осталось - ";
            }
            else
            {
                CountShip = "У врага осталось - ";
            }

            if (BDeck1)
            {
                CountShip += "1 Палубный = ";
                CountAliveShip(Deck1, ref CountShip);
                if (BDeck2 || BDeck3 || BDeck4)
                {
                    CountShip += " | ";
                }
            }

            if (BDeck2)
            {
                CountShip += $"2 Палубный = ";
                CountAliveShip(Deck2, ref CountShip);
                if (BDeck3 || BDeck4)
                {
                    CountShip += " | ";
                }
            }

            if (BDeck3)
            {
                CountShip += $"3 Палубный = ";
                CountAliveShip(Deck3, ref CountShip);
                if (BDeck4)
                {
                    CountShip += " | ";
                }
            }

            if (BDeck4)
            {
                CountShip += $"4 Палубный = {Deck4} корабль";
            }

            Console.Clear();
            CoolString(messgae, ConsoleColor.Magenta);
            CoolString(CountShip, ConsoleColor.Magenta);
            CoolString($" Ходов сделано - {CountStep}", ConsoleColor.Green);
            while (isPlaying)
            {
                Console.WriteLine("1 - Сыграть еще раз / 2 - Показать карту Игрока /3 - Показать карту врага / 0 - EXIT");
                int choise = GetDigit(0, 3);
                if (choise == 0)
                {
                    playGame = false;
                    isPlaying = false;
                }
                else if (choise == 3)
                {
                    Console.Clear();
                    DrawMapForWinScreen(Maps.MapOfEnemy);
                }
                else if (choise == 2)
                {
                    Console.Clear();
                    DrawMapForWinScreen(Maps.MapOfPlayer);
                }
                else
                {
                    isPlaying = false;
                    break;
                }
                Console.ReadKey();
            }

        }

        private static void CountAliveShip(int Deck1, ref string CountShip) // Считает оставшиеся суда(дочерний метод Win()
        {
            if (Deck1 == 1)
            {
                CountShip += $"{Deck1} корабль";
            }
            else
            {
                CountShip += $"{Deck1} корабля";
            }
        }
        static void GetCoords(out int xPos, out int yPos)  // Выбор координаты игроком
        {
            xPos = posX_last;
            yPos = posY_last;
            ConsoleKey Choise;
            string lastPosition = Maps.EnemyMapToShow[xPos, yPos];
            Maps.EnemyMapToShow[xPos, yPos] = "Aim";
            while (true)
            {
                GameScreen(false);
                Maps.EnemyMapToShow[xPos, yPos] = lastPosition;
                Choise = Console.ReadKey().Key;
                if (Choise == ConsoleKey.DownArrow && yPos <= 8)
                {
                    yPos += 1;
                }
                else if (Choise == ConsoleKey.LeftArrow && xPos > 0)
                {
                    xPos -= 1;
                }
                else if (Choise == ConsoleKey.RightArrow && xPos <= 8)
                {
                    xPos += 1;
                }
                else if (Choise == ConsoleKey.UpArrow && yPos > 0)
                {
                    yPos -= 1;
                }
                else if (Choise == ConsoleKey.Enter)
                {
                    return;
                }
                lastPosition = Maps.EnemyMapToShow[xPos, yPos];
                Maps.EnemyMapToShow[xPos, yPos] = "Aim";
            }
            

        }
        static void Attack()  // Ход Игрока(булл только для задержки
        {
            _CountStep += 1;
            GameScreen(false);
            bool Alive = false;
            Console.WriteLine("Выбери клетку!");
            GetCoords(out int xPos, out int yPos);
            posX_last = xPos;
            posY_last = yPos;
            if (Maps.MapOfEnemy[xPos, yPos].Alive)
                Alive = true;

            Maps.MapOfEnemy[xPos, yPos].TakeDamage(true);
            
            if (Maps.MapOfEnemy[xPos, yPos].Close == 1 && Alive)
            {
                TextForScreenMain = "Попал!!";
                MassageConsoleColor = ConsoleColor.Green;
                AdditionMessageForGameScreen = " ";
                GameScreen(false);
                Maps.MapOfEnemy[xPos, yPos].StatusOfComponent(true);
                Console.ReadKey();
                if (isPlaying) { Attack(); }
            }
            else
            {
                TextForScreenMain = "Промах!!";
                MassageConsoleColor = ConsoleColor.Red;
                AdditionMessageForGameScreen = " ";
                GameScreen();
            }
        }
        static void EnemyAttack() // Ход ИИ
        {
            Ship.EnemyII(out int xPos, out int yPos);

            Maps.MapOfPlayer[xPos, yPos].TakeDamage();

            if (Maps.MapOfPlayer[xPos, yPos].Close == 1)
            {
                TextForScreenMain = "Противник Попал!!!!";
                MassageConsoleColor = ConsoleColor.Red;
                Maps.MapOfEnemy[xPos, yPos].StatusOfComponent();
                GameScreen(false);
                Maps.MapOfPlayer[xPos, yPos].StatusOfComponent(true);
                AdditionMessageForGameScreen = " ";
                Console.ReadKey();
                if (isPlaying) { EnemyAttack(); }
            }
            else
            {
                TextForScreenMain = "Противник Промазал!!!!";
                MassageConsoleColor = ConsoleColor.Green;
            }
        }
        public static void CoolString(string str, ConsoleColor col = ConsoleColor.White)   // Крутая стринг
        {
            ConsoleColor oldCol = Console.ForegroundColor;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < 50 - ((str.Length + 5) / 2); i++) { Console.Write(" "); }
            for (int i = 0; i < str.Length + 5; i++) { Console.Write("*"); }
            Console.WriteLine();
            for (int i = 0; i < 50 - (str.Length / 2); i++) { Console.Write(" "); }
            Console.ForegroundColor = col;
            Console.Write(str);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            for (int i = 0; i < 50 - ((str.Length + 5) / 2); i++) { Console.Write(" "); }
            for (int i = 0; i < str.Length + 5; i++) { Console.Write("*"); }
            Console.WriteLine();
            Console.ForegroundColor = oldCol;
        }
        static void RELOAD(Ship[] Ships, bool Player)  // Пересобирает массив кораблей, булл чтобы передать в дочерний метод 
        {                                              
            if (RandomPosition(Ships, Player) == -1) //( часть кода работает только если рандом в дочернем методе ломается( вроде уже не ломается, но пусть будет)
            {
                Ships = new Ship[10]
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

                int Hp = 0;
                if (Player)
                {
                    
                    foreach (var ship in Ships)
                    {
                        Hp += ship._Size;
                    }
                    Maps.PlayerHp = Hp;
                    for (int i = 0; i < Maps.ySize; i++)
                    {
                        for (int j = 0; j < Maps.xSize; j++)
                        {
                            Maps.MapOfPlayer[j, i] = new Point(j, i);
                        }
                    }
                }
                else
                {
                    foreach (var ship in Ships)
                    {
                         Hp += ship._Size;
                    }
                    Maps.EnemyHP = Hp;
                    for (int i = 0; i < Maps.ySize; i++)
                    {
                        for (int j = 0; j < Maps.xSize; j++)
                        {
                            Maps.MapOfEnemy[j, i] = new Point(j, i);
                        }
                    }
                }

                RELOAD(Ships, Player);                              // запускает цепочку заново
            }
            else if (Player)
            {
                CoolString("Успешно Сгенерировано!", ConsoleColor.Green);
                Console.WriteLine("Нажмите ENTER");
                Console.ReadKey();
            }

        }
        static int RandomPosition(Ship[] Ships, bool Player = false)  // Дочерний метод RELOAD(), Ставит масив судов по рандомным координатам
        {
            int Compleate = 0;
            foreach (Ship ship in Ships)
            {
                int Restart = 0;
                while (Restart < 100)
                {
                    bool Up = rnd.Next(0, 2) == 0;
                    int maxPosx;
                    int maxPosy;
                    if (Up)
                    {
                        maxPosx = Maps.xSize;
                        maxPosy = Maps.ySize - (ship._Size - 1);
                    }
                    else
                    {
                        maxPosx = Maps.xSize - (ship._Size - 1);
                        maxPosy = Maps.ySize;
                    }


                    int xPos = rnd.Next(0, maxPosx);
                    int yPos = rnd.Next(0, maxPosy);
                    try
                    {
                        ship.Place(xPos, yPos, Up, Player);
                        break;
                    }

                    catch
                    {
                        Restart += 1;
                    }
                }
                if (Restart >= 50)   
                {
                    return -1;
                }
            }
            return Compleate;
        }
        static int GetDigit(int LowDigit, int HightDigit)  //  Осуществляет выбор рандомной цыфры 
        {
            while (true)
            {
                Console.WriteLine("Введите число от {0} до {1}", LowDigit, HightDigit);

                if (int.TryParse(Console.ReadLine(), out int x) == false)
                {
                    CoolString("Ошибка ! Введите число!", ConsoleColor.Red);
                    continue;
                }
                if (x >= LowDigit && x <= HightDigit)
                {
                    return x;
                }
                CoolString("Ошибка ! Число не в заданном диапазоне!", ConsoleColor.Red);
            }
        }

        //static void GetSide(out bool a) // Рандомный булл ( просто надо )
        //{
        //    if (GetDigit(1, 2) == 1)
        //    {
        //        a = true;
        //    }
        //    else
        //    {
        //        a = false;
        //    }

        //}

        //static void Show<T>(T[,] mapForShow, bool stop = false) // Обобщенный метод для показа карты (Игрока, ИИ(Туман войны)), ИИ) устарел
        //{
        //    Console.Clear();
        //    Console.Write("\t");
        //    for (int i = 0; i < Maps.xSize; i++)
        //    {
        //        Console.Write($"{i + 1}\t");
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine();

        //    for (int i = 0; i < Maps.ySize; i++)
        //    {
        //        Console.Write($"{i + 1}\t");
        //        for (int j = 0; j < Maps.xSize; j++)
        //        {
        //            Console.Write($"{mapForShow[j, i].ToString()}\t");
        //        }
        //        Console.WriteLine();
        //    }
        //    if (stop)
        //    {
        //        CoolString("Нажмите Enter");
        //        Console.ReadKey();
        //    }
        //}

        private static void TextureSelect(Point Map = null, string Texture = null) // Выбираем цвет
        {
            ConsoleColor consoleDigit = Console.ForegroundColor;
            ConsoleColor consoleBack = Console.BackgroundColor;

            if(Map != null)
            {
                if (Map.Close == 1 && Map.Alive)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write("   ");
                }
                else if (Map.Close == 0 && Map.Alive)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("   ");
                }
                else if (Map.SideDestroy == true)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("   ");
                }
                else if (Map.Close == 1 && !Map.Alive)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(" X ");
                }
                else if (Map.Close == 0 && !Map.Alive)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write(" X ");
                }

                Console.ForegroundColor = consoleDigit;
                Console.BackgroundColor = consoleBack;

                return;
            }

            if( Texture != null) 
            {
                if (Texture == "Q")
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("   ");
                }
                else if (Texture == "S")
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.Write("   ");
                }
                else if (Texture == "B")
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write("   ");
                }
                else if (Texture == "X")
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(" X ");
                }
                else if (Texture == "+")
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write("   ");
                }
                else if (Texture == "*")
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.Write(" X ");
                }
                else if (Texture == "Aim")
                {
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.Write("   ");
                }


                Console.ForegroundColor = consoleDigit;
                Console.BackgroundColor = consoleBack;

                return;
            }
        }  

        private static void MapNetDraw(int ?Digit = null, char ?Text = null, int Repeat = 1) // Рисуем цыферки карты
        {
            ConsoleColor consoleDigit = Console.ForegroundColor;
            ConsoleColor consoleBack = Console.BackgroundColor;


            if(Digit != null && Text == null) 
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                if (Digit == 10)
                {
                    Console.Write($" {Digit}");
                }
                else
                {
                    Console.Write($" {Digit} ");
                }
                Console.ForegroundColor = consoleDigit;
                Console.BackgroundColor = consoleBack;

                return;
            }
            
            if(Text != null)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.Green;
                if (Digit == 10)
                {
                    Console.Write($" {Text}");
                }
                else
                {
                    Console.Write($" {Text} ");
                }
                Console.ForegroundColor = consoleDigit;
                Console.BackgroundColor = consoleBack;

                return;
            }

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;

            for (int i = 0; i < Repeat; i++)
            {
                Console.Write(" ");
            }

            Console.ForegroundColor = consoleDigit;
            Console.BackgroundColor = consoleBack;
        }
        public static void GameScreen(bool Stop = true) // Рисуем все карты и экран впринципи
        {
            Console.Clear();
            ConsoleColor OldconsoleColor = Console.ForegroundColor;

            for (int i = 0; i < 3; i++)
            {
                Console.Write(" ");
            }

            MapNetDraw(Repeat:3);


            for (int i = 0; i < 11; i++)
            {
                if (i <= 9)
                {
                    MapNetDraw( i + 1);
                }
                else
                {
                    MapNetDraw(Repeat: 3);
                }
            }

            if( _CountStep >= 10)
            {
                for (int i = 0; i < 1; i++)
                {
                    Console.Write(" ");
                }
                Console.Write($"Ход - {CountStep}");
                for (int i = 0; i < 1; i++)
                {
                    Console.Write(" ");
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    Console.Write(" ");
                }
                Console.Write($"Ход - {CountStep}");
                for (int i = 0; i < 1; i++)
                {
                    Console.Write(" ");
                }
            }

            MapNetDraw(Repeat: 3);

            for (int i = 0; i < 11; i++)
            {
                if (i <= 9)
                {
                    MapNetDraw(i + 1);
                }
                else
                {
                    MapNetDraw(Repeat: 3);
                }
            }

            Console.WriteLine();

            for (int i = 0; i < 3; i++)
            {
                Console.Write(" ");
            }
            

            for (int j = 0; j < Maps.ySize; j++)
            {
                MapNetDraw(Text : ABC[j]);
                for (int i = 0; i < Maps.ySize + 1; i++)
                {
                    if(i <= Maps.ySize - 1)
                    {
                        TextureSelect(Map: Maps.MapOfPlayer[i, j]);
                    }   
                    else
                    {
                        MapNetDraw(Repeat: 3);
                    }                    
                }

                Console.Write("          "); // 10
                MapNetDraw(Text: ABC[j]);
                for (int i = 0; i < Maps.ySize + 1; i++)
                {
                    
                    if(i <= Maps.ySize - 1)
                    {
                        TextureSelect(Texture: Maps.EnemyMapToShow[i, j]);
                    }
                    else
                    {
                        MapNetDraw(Repeat: 3);
                    }
                }

                Console.WriteLine();
                for (int i = 0; i < 3; i++)
                {
                    Console.Write(" ");
                }
            }

            MapNetDraw(Repeat: 36);

            Console.Write("          "); // 10

            MapNetDraw(Repeat: 36);

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine();
            }
            if(TextForScreenMain.Length >= 3)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int i = 0; i < 40 - (TextForScreenMain.Length + 4) / 2; i++) { Console.Write(" "); }
                for (int i = 0; i < TextForScreenMain.Length + 5; i++) { Console.Write("*"); }
                Console.WriteLine();

                Console.ForegroundColor = MassageConsoleColor;
                for (int i = 0; i < 40 - (TextForScreenMain.Length) / 2; i++) { Console.Write(" "); }
                Console.Write(TextForScreenMain);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Magenta;
                for (int i = 0; i < 40 - (TextForScreenMain.Length + 4) / 2; i++) { Console.Write(" "); }
                for (int i = 0; i < TextForScreenMain.Length + 5; i++) { Console.Write("*"); }

                Console.ForegroundColor = OldconsoleColor;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(AdditionMessageForGameScreen);
            }
            

            if(Stop)
                Console.ReadKey();
        }

        private static void DrawMapForWinScreen(Point[,] Map = null, string[,] Mapp = null) // Рисуем нужную карту
        {
            Console.Clear();
            for (int i = 0; i < 3; i++)
            {
                Console.Write(" ");
            }

            MapNetDraw(Repeat: 3);


            for (int i = 0; i < 11; i++)
            {
                if (i <= 9)
                {
                    MapNetDraw(i + 1);
                }
                else
                {
                    MapNetDraw(Repeat: 3);
                }
            }

            for (int j = 0; j < Maps.ySize; j++)
            {
                Console.WriteLine();

                for (int i = 0; i < 3; i++)
                {
                    Console.Write(" ");
                }
                MapNetDraw(j + 1);
                for (int i = 0; i < Maps.ySize + 1; i++)
                {
                    if (i <= Maps.ySize - 1)
                    {
                        if(Map == null)
                        {
                            TextureSelect(Texture: Mapp[i, j]);
                        }
                        else
                        {
                            TextureSelect(Map: Map[i, j]);
                        }
                        
                    }
                    else
                    {
                        MapNetDraw(Repeat: 3);
                    }

                }
            }
            Console.WriteLine();
            for (int i = 0; i < 3; i++)
            {
                Console.Write(" ");
            }

            MapNetDraw(Repeat: 36);

            Console.WriteLine();
            Console.WriteLine();
        }

        private static void ManualBuild(bool Up, int Size, int xPos, int yPos, string Texture = "S") 
        {
            if(Maps.PlayerMapToBuild[xPos, yPos] != "B")
            {
                if (Up)
                {
                    if (Size == 1) { Maps.PlayerMapToBuild[xPos, yPos] = Texture; }
                    else if (Size == 2)
                    {
                        Maps.PlayerMapToBuild[xPos, yPos] = Texture;
                        Maps.PlayerMapToBuild[xPos, yPos + 1] = Texture;
                    }
                    else
                    {
                        for (int i = 0; i < Size - 1; i++)
                        {
                            if (i > 1)
                                Maps.PlayerMapToBuild[xPos, yPos + i] = Texture;
                            else
                                Maps.PlayerMapToBuild[xPos, yPos + i] = Texture;
                        }
                        Maps.PlayerMapToBuild[xPos, yPos + Size - 1] = Texture;
                    }

                }
                else if (!Up)
                {
                    if (Size == 1) { Maps.PlayerMapToBuild[xPos, yPos] = Texture; }
                    else if (Size == 2)
                    {
                        Maps.PlayerMapToBuild[xPos, yPos] = Texture;
                        Maps.PlayerMapToBuild[xPos + 1, yPos] = Texture;
                    }
                    else
                    {
                        for (int i = 0; i < Size - 1; i++)
                        {
                            if (i > 1)
                                Maps.PlayerMapToBuild[xPos + i, yPos] = Texture;
                            else
                                Maps.PlayerMapToBuild[xPos + i, yPos] = Texture;
                        }
                        Maps.PlayerMapToBuild[xPos + Size - 1, yPos] = Texture;
                    }
                }
            }
        }
    }
}
