using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Codenjoy.SnakeBattleClient.Enums;

namespace Codenjoy.SnakeBattleClient.Models
{
    public class Board
    {
        private LengthToXY LengthXY;
        public string BoardString;

        /// <summary>
        /// GameBoard size (actual board size is Size x Size cells)
        /// </summary>
        public int Size
        {
            get
            {
                return (int)Math.Sqrt(BoardString.Length);
            }
        }
        public int remember = 1;
        public Board(String boardString, int ticks)
        {
            BoardString = boardString;
            LengthXY = new LengthToXY(Size);
            remember = ticks;
        }

        public Point GetHead()
        {
            return Get(Element.HeadUp)
                .Concat(Get(Element.HeadDown))
                .Concat(Get(Element.HeadLeft))
                .Concat(Get(Element.HeadRight))
                .Concat(Get(Element.HeadEvil))
                .Concat(Get(Element.HeadFly))
                .SingleOrDefault();
        }

        public List<Point> GetApples()
        {
            return Get(Element.Apple);
        }

        public List<Point> GetGold()
        {
            return Get(Element.Gold);
        }

        public List<Point> GetBounty()
        {
            return GetGold().Concat(GetApples()).ToList();
        }
        public List<Point> GetStones()
        {
            return Get(Element.Stone);
        }

        public List<Point> GetFlyingPill()
        {
            return Get(Element.FlyingPill);
        }

        public List<Point> GetFuryPill()
        {
            return Get(Element.FuryPill);
        }

        public List<Point> GetWalls()
        {
            var walls = Get(Element.Wall);
            walls.Concat(Get(Element.StartFloor));
            return walls;
        }

        public List<Point> GetSnake()
        {
            List<Point> snake = new List<Point>();
            Point head = GetHead();
            
            if (!IsSnakeAlive())
            {
                return snake;
            }

            snake.Add(head);
            return snake
                .Concat(Get(Element.TailEndDown))
                .Concat(Get(Element.TailEndLeft))
                .Concat(Get(Element.TailEndUp))
                .Concat(Get(Element.TailEndRight))
                .Concat(Get(Element.BodyHorizontal))
                .Concat(Get(Element.BodyVertical))
                .Concat(Get(Element.BodyLeftDown))
                .Concat(Get(Element.BodyLeftUp))
                .Concat(Get(Element.BodyRightDown))
                .Concat(Get(Element.BodyRightUp))
                .ToList();
        }
        public int updateTicks(int current)
        {
            if (IsSnakeAlive())
            {
                current += 1;
                remember += 1;
            }
            else
            {
                current = 1;
                remember = 1;
            }
            return current;
        }
        public List<Point> GetAllEnemySnakePoints()
        {
            List<Point> snake = new List<Point>();

            return snake
                .Concat(Get(Element.EnemyBodyHorizontal))
                .Concat(Get(Element.EnemyBodyLeftDown))
                .Concat(Get(Element.EnemyBodyLeftUp))
                .Concat(Get(Element.EnemyBodyRightDown))
                .Concat(Get(Element.EnemyBodyRightUp))
                .Concat(Get(Element.EnemyBodyVertical))
                .Concat(Get(Element.EnemyHeadDead))
                .Concat(Get(Element.EnemyHeadDown))
                .Concat(Get(Element.EnemyHeadEvil))
                .Concat(Get(Element.EnemyHeadFly))
                .Concat(Get(Element.EnemyHeadLeft))
                .Concat(Get(Element.EnemyHeadRight))
                .Concat(Get(Element.EnemyHeadSleep))
                .Concat(Get(Element.EnemyHeadUp))
                .Concat(Get(Element.EnemyTailEndDown))
                .Concat(Get(Element.EnemyTailEndLeft))
                .Concat(Get(Element.EnemyTailEndRight))
                .Concat(Get(Element.EnemyTailEndUp))
                .Concat(Get(Element.EnemyTailInactive))
                .ToList();
        }

        public List<Point> GetBarriers()
        {
            return GetSnake()
                .Concat(GetStones())
                .Concat(GetWalls())
                .Concat(GetAllEnemySnakePoints())
                .ToList();
        }

        public bool IsBarrierAt(Point point)
        {
            return GetBarriers().Contains(point);
        }
        
        public bool IsEnemyAliveSnakeAt(Point point)
        {
            var snake = Get(Element.EnemyBodyHorizontal)
                .Concat(Get(Element.EnemyBodyLeftDown))
                .Concat(Get(Element.EnemyBodyLeftUp))
                .Concat(Get(Element.EnemyBodyRightDown))
                .Concat(Get(Element.EnemyBodyRightUp))
                .Concat(Get(Element.EnemyBodyVertical))
                .Concat(Get(Element.EnemyHeadDown))
                .Concat(Get(Element.EnemyHeadLeft))
                .Concat(Get(Element.EnemyHeadRight))
                .Concat(Get(Element.EnemyHeadUp))
                .Concat(Get(Element.EnemyTailEndDown))
                .Concat(Get(Element.EnemyTailEndLeft))
                .Concat(Get(Element.EnemyTailEndRight))
                .Concat(Get(Element.EnemyTailEndUp))
                .ToList();

            return snake.Contains(point);
        }

        public bool IsSnakeAlive()
        {
            var head = GetHead();
            return head.X != 0 && head.Y != 0;
        }

        public Element GetAt(Point point)
        {
            if (point.IsOutOf(Size))
            {
                return Element.None;
            }
            return (Element)BoardString[LengthXY.GetLength(point.X, point.Y)];
        }

        public bool IsAt(Point point, Element element)
        {
            if (point.IsOutOf(Size))
            {
                return false;
            }
            return GetAt(point) == element;
        }

        public List<Point> Get(Element element)
        {
            List<Point> result = new List<Point>();

            for (int i = 0; i < Size * Size; i++)
            {
                Point pt = LengthXY.GetXY(i);

                if (IsAt(pt, element))
                {
                    result.Add(pt);
                }
            }

            return result;
        }
        public List<Point> GetEnemyHeads()
        {
            List<Point> result = new List<Point>();
            for(int i = 0; i < Size * Size; i += 1)
            {
                Point pt = LengthXY.GetXY(i);
                if (IsAt(pt, Element.EnemyHeadEvil) || IsAt(pt, Element.EnemyHeadDown) || IsAt(pt, Element.EnemyHeadFly) || IsAt(pt, Element.EnemyHeadLeft) || 
                    IsAt(pt, Element.EnemyHeadRight) || IsAt(pt, Element.EnemyHeadUp))
                {
                    result.Add(pt);
                }
            }
            return result;
        }
        public string GetDisplay()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < BoardString.Length; i += Size)
            {
                sb.AppendLine(BoardString.Substring(i, Size));
            }

            return sb.ToString();
        }
    }
}