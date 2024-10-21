using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BattleOfShip
{
    sealed class Ship : StructOfShip, IShip
    {
        // Далее/ Координаты последего выстрела
        static int newXPos;
        static int newYPos;

        static bool oldShoot = false;

        // Далее/ Координаты второго выстрела
        static int positionXForBack;
        static int positionYForBack;

        static bool doubleShoot = false;

        // Далее/ Координаты первого выстрела
        static int startX;
        static int startY;
        // Далее/ Координаты предпоследего выстрела
        static int posXForForward;
        static int posYForForward; 
        public int Health  // Условие победы
        {
            get { return _Health; }
            set
            {
                _Health = value;
                if (_Health <= 0)
                
                {
                    Deconstruct();
                }
            }
        }
        public Ship(int size) : base(size) { Body = new ComponentOfShip[Size]; }


        static bool ShootUp(ref int xPos, ref int yPos) // Стреляет вверх
        {
            if (startY - 1 >= 0)
            {
                if ((Maps.MapOfPlayer[startX, startY - 1].Alive))
                {
                    xPos = startX;
                    yPos = startY - 1;
                    return true;
                }
            }
            return false;
        }

        static bool ShootDown(ref int xPos, ref int yPos) // Стреляет вниз
        {
            if (Maps.MapOfPlayer[startX, startY + 1].Alive)
            {
                xPos = startX;
                yPos = startY + 1;
                return true;
            }
            return false;
        }

        static bool ShootRight(ref int xPos, ref int yPos) // Стреляет вправо
        {
            if (Maps.MapOfPlayer[startX + 1, startY].Alive)
            {
                xPos = startX + 1;
                yPos = startY;
                return true;
            }
            return false;
        }

        static bool ShootLeft(ref int xPos, ref int yPos) // Стреляет влево
        {
            if(startX - 1 >= 0)
            {
                if (Maps.MapOfPlayer[startX - 1, startY].Alive)
                {
                    xPos = startX - 1;
                    yPos = startY;
                    return true;
                }
            }
            return false;
        }
        public static void EnemyII(out int xPos, out int yPos) // Чудовище науки и вообще лютая штука (ИИ(подоби ИИ))
        {
            if(oldShoot && doubleShoot) // если судно больше 2 клеток( и есть 2 попадания )
            {
                if (Maps.MapOfPlayer[startX, startY].Check()) // проверка живо ли судно, если нет рандомный выстрел
                {
                    GetCoordinate(out xPos, out yPos);
                    return;
                }
                // Координаты !! второе попадание + (последее попадание - предпоследнее попадание)
                int xNext = newXPos + (newXPos - posXForForward);
                int yNext = newYPos + (newYPos - posYForForward);
                // Координаты !! первое попадание + (второе попадание - первое попадание)
                int xPrev = startX - (positionXForBack - startX);
                int yPrev = startY - (positionYForBack - startY);
                if (xNext < 10 && yNext < 10 && xNext >= 0 && yNext >= 0) // Что бы не впихивать блок try\catch
                {
                    if(Maps.MapOfPlayer[xNext, yNext].Alive)
                    {
                        if (Maps.MapOfPlayer[xNext, yNext].Close == 1)
                        {
                            xPos = xNext;
                            yPos = yNext;
                            posXForForward = newXPos; 
                            posYForForward = newYPos;
                            newXPos = xPos;
                            newYPos = yPos;
                        }
                        else
                        {
                            xPos = xNext;
                            yPos = yNext;
                        }
                        return;
                    }
                }
                xPos = xPrev;
                yPos = yPrev;
                newXPos = xPos;
                newYPos = yPos;
                return;
            }
            else if(oldShoot) // если судно больше 1 клетки( и есть 1 попадание )
            {
                if(Maps.MapOfPlayer[startX, startY].Check())
                {
                    GetCoordinate(out xPos, out yPos);
                    return;
                }
                xPos = startX;
                yPos = startY;
                while (true)
                {
                    int rand = Program.rnd.Next(0, 4);
                    try
                    {
                        if (rand == 0)
                        {
                            if(ShootDown(ref xPos, ref yPos))
                                break;
                        }
                        else if (rand == 1)
                        {
                            if (ShootRight(ref xPos, ref yPos))
                                break;
                        }
                        else if (rand == 2)
                        {
                            if(ShootLeft(ref xPos, ref yPos))
                                break;
                        }
                        else
                        {
                            if(ShootUp(ref xPos, ref yPos))
                                break;
                        }
                    }
                    catch (Exception) { }
                }
                
                if(Maps.MapOfPlayer[xPos, yPos].Close == 1)
                {
                    newXPos = xPos;
                    newYPos = yPos;
                    positionXForBack = newXPos;
                    positionYForBack = newYPos;
                    posXForForward = startX;
                    posYForForward = startY;
                    doubleShoot = true;
                }
                else
                {
                    doubleShoot = false; // На всякий случай
                }
                if(Maps.MapOfPlayer[newXPos, newYPos].Check())
                {
                    oldShoot = false;
                }
                return;
            }
            GetCoordinate(out xPos, out yPos);
        }

        private static void GetCoordinate(out int xPos, out int yPos) // Берет рандомную координату 
        {
            while (true)
            {
                xPos = Program.rnd.Next(0, Maps.xSize);
                yPos = Program.rnd.Next(0, Maps.xSize);
                if (!Maps.MapOfPlayer[xPos, yPos].Alive) // Проверяет ее, Alive значит снаряд еще не прилетал
                {
                    continue;
                }
                else if (Maps.MapOfPlayer[xPos, yPos].Alive)
                {
                    if (Maps.MapOfPlayer[xPos, yPos].Close == 1 && !Maps.MapOfPlayer[xPos, yPos].Check()) // Close == 1 Значит есть клетка судна
                    {                                                                                     // Проверка наперед
                        startX = xPos;                                                                    // Метод Check() Гуглит живое ли СУДНО
                        startY = yPos;                                                                    // Координаты обьясняются при обьявлении
                        oldShoot = true;
                        newXPos = xPos;
                        newYPos = yPos;
                        doubleShoot = false;
                    }
                    else
                    {
                        oldShoot = false;
                        doubleShoot = false;
                    }
                    return;
                }
            }
        }
        public void Place(int x, int y, bool Up, bool Player) // Метод ставит корабль на его место
        {
            xPos = x;
            yPos = y;
            this.Up = Up;
            this.Player = Player;
            BuildShip();
        }

        private void DeconstructMain(Point ShipToDestroy, bool Player = false) // Скажем так, уничтожает судно и округу Part2
        {
            if(Player)
            {
                    if(ShipToDestroy.Close == 1)
                    {
                        ShipToDestroy.Field = "X";
                        ShipToDestroy.Alive = false;
                    }
                    else
                    {
                        ShipToDestroy.SideDestroy = true;
                        ShipToDestroy.Field = "+";
                        ShipToDestroy.Alive = false;
                    }
            }
            else if(!Player)
            {
                    if(ShipToDestroy.Close == 1)
                    {
                        ShipToDestroy.Field = "X";
                        Maps.EnemyMapToShow[ShipToDestroy.xPos, ShipToDestroy.yPos] = "X";
                        ShipToDestroy.Alive = false;
                    }
                    else
                    {
                        ShipToDestroy.SideDestroy = true;
                        ShipToDestroy.Field = "+";
                        Maps.EnemyMapToShow[ShipToDestroy.xPos, ShipToDestroy.yPos] = "+";
                        ShipToDestroy.Alive = false;
                    }
            }
            
        }
        public void Deconstruct() // Скажем так, уничтожает судно и округу Part1
        {
            if (Player && Up)
            {
                for (int i = yPos - 1; i < yPos + 1 + Size; i++)  // yPos - 1 , Может быть отрицательным
                {
                    for (int j = xPos - 1; j < xPos + 2; j++)
                    {
                        try
                        {
                            DeconstructMain(Maps.MapOfPlayer[j, i], Player); 
                        }
                        catch (Exception) { }         // IndexOutOfRangeException , но я обобщил, на всякий
                    }
                }
            }
            else if ((Player && !Up))
            {
                for (int i = yPos - 1; i < yPos + 2; i++)
                {
                    for (int j = xPos - 1; j < xPos + Size + 1; j++)
                    {
                        try
                        {
                            DeconstructMain(Maps.MapOfPlayer[j, i], Player);
                        }
                        catch (Exception) { }                        
                    }
                }
            }

            if (!Player && Up)
            {
                for (int i = yPos - 1; i < yPos + 1 + Size; i++)
                {
                    for (int j = xPos - 1; j < xPos + 2; j++)
                    {
                        try
                        {
                            DeconstructMain(Maps.MapOfEnemy[j, i], Player);
                        }
                        catch (Exception) { }
                    }
                }
            }
            else if ((!Player && !Up))
            {
                for (int i = yPos - 1; i < yPos + 2; i++)
                {
                    for (int j = xPos - 1; j < xPos + Size + 1; j++)
                    {
                        try
                        {
                            DeconstructMain(Maps.MapOfEnemy[j, i], Player);
                        }
                        catch (Exception) { }
                    }
                }
            }
            if (Player)
                Maps.PlayerHp -= StartSize;
            else
                Maps.EnemyHP -= StartSize;
        }
        private void CheakForMain(bool closePos) // Проверяем на наличие судна Part3
        {
            try
            {
                if (closePos)
                {
                    PositionNotFreeException ex = new PositionNotFreeException();
                    throw ex;
                }
            }
            catch (IndexOutOfRangeException) { }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void CheakMain(int j, int i, bool Player = false)  // Проверяем на наличие судна Part2
        {
            try
            {
                if (Player)
                {
                    CheakForMain(Maps.MapOfPlayer[j, i].Close == 1);
                }
                else
                {
                    CheakForMain(Maps.MapOfEnemy[j, i].Close == 1);
                }
            }
            catch (IndexOutOfRangeException) { }      
        }

        private void CheckShip()   // Проверяем на наличие судна Part1
        {
            if (Player && Up)
            {
                if(yPos + Size-1 >= 10)
                {
                    PositionNotFreeException ex = new PositionNotFreeException();
                    throw ex;
                }
                for (int i = yPos - 1; i < yPos + 1 + Size; i++)
                {
                    for (int j = xPos - 1; j < xPos + 2; j++)
                    {
                        CheakMain(j, i, Player);
                    }
                }
            }
            else if ((Player && !Up))
            {
                if(xPos + Size - 1 >= 10)
                {
                    PositionNotFreeException ex = new PositionNotFreeException();
                    throw ex;
                }
                for (int i = yPos - 1; i < yPos + 2; i++)
                {
                    for (int j = xPos - 1; j < xPos + Size + 1; j++)
                    {
                        CheakMain(j, i, Player);
                    }
                }
            }

            if (!Player && Up)
            {
                if (yPos + Size - 1 >= 10)
                {
                    PositionNotFreeException ex = new PositionNotFreeException();
                    throw ex;
                }
                for (int i = yPos - 1; i < yPos + 1 + Size; i++)
                {
                    for (int j = xPos - 1; j < xPos + 2; j++)
                    {
                        CheakMain(j, i);
                    }
                }
            }
            else if ((!Player && !Up))
            {
                if (xPos + Size - 1 >= 10)
                {
                    PositionNotFreeException ex = new PositionNotFreeException();
                    throw ex;
                }
                for (int i = 0; i < yPos + 2; i++)
                {
                    for (int j = 0; j < xPos + Size + 1; j++)
                    {
                        CheakMain(j, i);
                    }
                }
            }
        }
        private void BuildShip() // Метод ставит корабль на его место Part2
        {
            CheckShip(); // Проверяем место
            if (Up && Player)   // Хотел как то отрефакторить последующий код, но хз
            {
                if (Size == 1) { Body[0] = new ComponentOfShip("1", xPos, yPos, true, this); }
                else if (Size == 2)
                {
                    Body[0] = new ComponentOfShip("1", xPos, yPos, true, this);
                    Body[1] = new ComponentOfShip("1", xPos, yPos + 1, true, this);
                }
                else
                {
                    for (int i = 0; i < Size - 1; i++)
                    {
                        if (i > 1)
                            Body[i] = new ComponentOfShip("1", xPos, yPos + i, true, this);
                        else
                            Body[i] = new ComponentOfShip("1", xPos, yPos + i, true, this);
                    }
                    Body[Size - 1] = new ComponentOfShip("1", xPos, yPos + Size - 1, true, this);
                }

            }
            else if(!Up && Player)
            {
                if (Size == 1) { Body[0] = new ComponentOfShip("1", xPos, yPos, true, this); }
                else if (Size == 2)
                {
                    Body[0] = new ComponentOfShip("1", xPos, yPos, true, this);
                    Body[1] = new ComponentOfShip("1", xPos + 1, yPos, true, this);
                }
                else
                {
                    for (int i = 0; i < Size - 1; i++)
                    {
                        if (i > 1)
                            Body[i] = new ComponentOfShip("1", xPos + i, yPos, true, this);
                        else
                            Body[i] = new ComponentOfShip("1", xPos + i, yPos, true, this);
                    }
                    Body[Size - 1] = new ComponentOfShip("1", xPos + Size - 1, yPos, true, this);
                }
            }

            if (Up && !Player)
            {
                if (Size == 1) { Body[0] = new ComponentOfShip("1", xPos, yPos, false, this); }
                else if (Size == 2)
                {
                    Body[0] = new ComponentOfShip("1", xPos, yPos, false, this);
                    Body[1] = new ComponentOfShip("1", xPos, yPos + 1, false, this);
                }
                else
                {
                    for (int i = 0; i < Size - 1; i++)
                    {
                        if (i > 1)
                            Body[i] = new ComponentOfShip("1", xPos, yPos + i, false, this);
                        else
                            Body[i] = new ComponentOfShip("1", xPos, yPos + i, false, this);
                    }
                    Body[Size - 1] = new ComponentOfShip("1", xPos, yPos + Size - 1, false, this);
                }

            }
            else if (!Up && !Player)
            {
                if (Size == 1) { Body[0] = new ComponentOfShip("1", xPos, yPos, false, this); }
                else if (Size == 2)
                {
                    Body[0] = new ComponentOfShip("1", xPos, yPos, false, this);
                    Body[1] = new ComponentOfShip("1", xPos + 1, yPos, false, this);
                }
                else
                {
                    for (int i = 0; i < Size - 1; i++)
                    {
                        if (i > 1)
                            Body[i] = new ComponentOfShip("1", xPos + i, yPos, false, this);
                        else
                            Body[i] = new ComponentOfShip("1", xPos + i, yPos, false, this);
                    }
                    Body[Size - 1] = new ComponentOfShip("1", xPos + Size - 1, yPos, false, this);
                }
            }
        }
    }
}
