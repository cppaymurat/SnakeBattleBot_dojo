using Codenjoy.SnakeBattleClient.Enums;
using Codenjoy.SnakeBattleClient.Models;
using System;
using System.Collections.Generic;

namespace Codenjoy.SnakeBattleClient.AI
{
    public class SnakeBattleBot
    {
        private readonly Board board;            
        private int[,,] distance;
        private bool[,] isBarrier;
        private Point[,] parent;
        private bool[,] used;
        private int[] dx;
        private int[] dy;
        private int myLength;
        private const int myIndex = 15;
        private Point myHead;
        private List<Point> enemyHeads;
        private List<Point> flyingPills;
        private List<Point> startFloors;
        private List<Point> angryPills;
        private List<Point> stones;
        private List<Point> walls;
        private List<Point> enemyPoints;
        private List<Point> mySnake;
        private static int underFlyingPill = 0;
        private static int underAngryPill = 0;
        public SnakeBattleBot(Board board)
        {
            this.board = board;
            isBarrier = new bool[board.Size, board.Size];
            distance = new int[16, board.Size, board.Size];
            used = new bool[board.Size, board.Size];
            parent = new Point[board.Size, board.Size];
            flyingPills = new List<Point>();
            angryPills = new List<Point>();
            stones = new List<Point>();
            enemyHeads = new List<Point>();
            walls = new List<Point>();
            myHead = new Point();
            enemyPoints = new List<Point>();
            startFloors = new List<Point>();
            mySnake = new List<Point>();
            myLength = 2;
            dx = new int[4];
            dx[0] = -1; dx[1] = 0; dx[2] = 1; dx[3] = 0;
            dy = new int[4];
            dy[0] = 0; dy[1] = -1; dy[2] = 0; dy[3] = 1;
        }
        public bool stones_allowed = false;
        public const int infinity = 10000;
        public bool correctPoint(Point pt)
        {
            return !pt.IsOutOf(board.Size) && !isBarrier[pt.X, pt.Y] && notBarrier(pt) && !mySnake.Contains(pt);
        }
        public bool notBarrier(Point pt)
        {
            return !pt.IsOutOf(board.Size) && !board.IsAt(pt, Element.Wall) && !board.IsAt(pt, Element.StartFloor);
        }
        public void runBFS(Point head, int index)
        {
            distance[index, head.X, head.Y] = 0;
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(head);
            while(q.Count != 0)
            {
                Point from = q.Dequeue();
                Point up = new Point(from.X, from.Y + 1);
                Point down = new Point(from.X, from.Y - 1);
                Point left = new Point(from.X - 1, from.Y);
                Point right = new Point(from.X + 1, from.Y);
                if (correctPoint(left) && distance[index, left.X, left.Y] > distance[index, from.X, from.Y] + 1)
                {
                    distance[index, left.X, left.Y] = distance[index, from.X, from.Y] + 1;
                    if (index == 15)
                    {
                        parent[left.X, left.Y] = from;
                    }
                    q.Enqueue(left);
                }
                if (correctPoint(right) && distance[index, right.X, right.Y] > distance[index, from.X, from.Y] + 1)
                {
                    distance[index, right.X, right.Y] = distance[index, from.X, from.Y] + 1;
                    if (index == 15)
                    {
                        parent[right.X, right.Y] = from;
                    }
                    q.Enqueue(right);
                }
                if (correctPoint(up) && distance[index, up.X, up.Y] > distance[index, from.X, from.Y] + 1)
                {
                    distance[index, up.X, up.Y] = distance[index, from.X, from.Y] + 1;
                    if (index == 15)
                    {
                        parent[up.X, up.Y] = from;
                    }
                    q.Enqueue(up);
                }
                if (correctPoint(down) && distance[index, down.X, down.Y] > distance[index, from.X, from.Y] + 1)
                {
                    distance[index, down.X, down.Y] = distance[index, from.X, from.Y] + 1;
                    if (index == 15)
                    {
                        parent[down.X, down.Y] = from;
                    }
                    q.Enqueue(down);
                }
            }
        }

        public void dfs(Point v, Point pr)
        {
            if (used[v.X, v.Y]) return;
            used[v.X, v.Y] = true;
            for(int i = 0; i < 4; i += 1)
            {
                Point u = new Point(v.X + dx[i], v.Y + dy[i]);
                if (correctPoint(u) && u != pr)
                {
                    dfs(u, v);
                }
            }
        }
        public bool is_bad(Point from, Point to)
        {
            for(int i = 0; i < board.Size; i += 1)
            {
                for(int j = 0; j < board.Size; j += 1)
                {
                    used[i, j] = false;
                }
            }
            foreach (Point pt in mySnake)
            {
                used[pt.X, pt.Y] = true;
            }
            dfs(to, from);
            int res = 0;
            for(int i = 0; i < board.Size; i += 1)
            {
                for(int j = 0; j < board.Size; j += 1)
                {
                    Point pt = new Point(i, j);
                    if (used[i, j] == true && (board.IsAt(pt, Element.Apple) || 
                                               board.IsAt(pt, Element.Gold) || 
                                               board.IsAt(pt, Element.FlyingPill) || 
                                               board.IsAt(pt, Element.FuryPill)) || board.IsAt(pt, Element.Stone))
                    {
                        res += 1;
                    }
                }
            }
            if (res < 6)
                return true;
            return false;
        }
        public string testDirection(Point from, Point to)
        {
            Point up = new Point(from.X, from.Y + 1);
            Point down = new Point(from.X, from.Y - 1);
            Point left = new Point(from.X - 1, from.Y);
            Point right = new Point(from.X + 1, from.Y);
            if (from.X == to.X)
            {
                if (from.Y < to.Y && correctPoint(up))
                {
                    if (!board.IsAt(myHead, Element.HeadDown) && !is_bad(from, up))
                    {
                        return Direction.Up.ToString();
                    }
                }
                if (from.Y > to.Y && correctPoint(down))
                {
                    if (!board.IsAt(myHead, Element.HeadUp) && !is_bad(from, down))
                    {
                        return Direction.Down.ToString();
                    }
                }
            }
            else
            {
                if (from.X < to.X && correctPoint(right))
                {
                    if (!board.IsAt(myHead, Element.HeadLeft) && !is_bad(from, right))
                    {
                        return Direction.Right.ToString();
                    }
                }
                if (from.X > to.X && correctPoint(left))
                {
                    if (!board.IsAt(myHead, Element.HeadRight) && !is_bad(from, left))
                    {
                        return Direction.Left.ToString();
                    }
                }
            }
            return "BAD";
        }
        
        public int min(int a, int b)
        {
            if (a < b)
                return a;
            return b;
        }
        public int min(int a, int b, int c)
        {
            return min(a, min(b, c));
        }
        public int max(int a, int b)
        {
            if (a > b)
                return a;
            return b;
        }
        public int max(int a, int b, int c)
        {
            return max(a, max(b, c));
        }
        public string getDirection(Point from, Point to)
        {
            foreach (Point pt in startFloors)
            {
                isBarrier[pt.X, pt.Y] = true;
            }
            foreach (Point wl in walls)
            {
                isBarrier[wl.X, wl.Y] = true;
            }
            Point up = new Point(from.X, from.Y + 1);
            Point down = new Point(from.X, from.Y - 1);
            Point left = new Point(from.X - 1, from.Y);
            Point right = new Point(from.X + 1, from.Y);
            if (from.X == to.X)
            {
                if (from.Y < to.Y && correctPoint(up))
                {
                    if (!board.IsAt(myHead, Element.HeadDown) && !is_bad(from, up))
                    {
                        if (board.IsAt(up, Element.FuryPill))
                        {
                            underAngryPill = 10;
                        }
                        if (board.IsAt(up, Element.FlyingPill))
                        {
                            underFlyingPill = 10;
                        }
                        return Direction.Up.ToString();
                    }
                }
                if (from.Y > to.Y && correctPoint(down)) {
                    if (!board.IsAt(myHead, Element.HeadUp) && !is_bad(from, down))
                    {
                        if (board.IsAt(down, Element.FuryPill))
                        {
                            underAngryPill = 10;
                        }
                        if (board.IsAt(down, Element.FlyingPill))
                        {
                            underFlyingPill = 10;
                        }
                        return Direction.Down.ToString();
                    }
                }
            } else
            {
                if (from.X < to.X && correctPoint(right))
                {
                    if (!board.IsAt(myHead, Element.HeadLeft) && !is_bad(from, right))
                    {
                        if (board.IsAt(right, Element.FuryPill))
                        {
                            underAngryPill = 10;
                        }
                        if (board.IsAt(right, Element.FlyingPill))
                        {
                            underFlyingPill = 10;
                        }
                        return Direction.Right.ToString();
                    }
                } 
                if (from.X > to.X && correctPoint(left)) {
                    if (!board.IsAt(myHead, Element.HeadRight) && !is_bad(from, left))
                    {
                        if (board.IsAt(left, Element.FuryPill))
                        {
                            underAngryPill = 10;
                        }
                        if (board.IsAt(left, Element.FlyingPill))
                        {
                            underFlyingPill = 10;
                        }
                        return Direction.Left.ToString();
                    }
                }
            }
            return Survive();
        }
        public void initialize()
        {
            for (int k = 0; k < 16; k += 1)
            {
                for (int i = 0; i < board.Size; i += 1)
                {
                    for (int j = 0; j < board.Size; j += 1)
                    {
                        if (k == 0)
                        {
                            isBarrier[i, j] = false;
                            parent[i, j] = new Point(-1, -1);
                        }
                        distance[k, i, j] = infinity;
                    }
                }
            }
            enemyHeads = board.GetEnemyHeads();
            stones = board.GetStones();
            walls = board.Get(Element.Wall);
            myHead = board.GetHead();
            myLength = board.GetSnake().Count;
            enemyPoints = board.GetAllEnemySnakePoints();
            startFloors = board.Get(Element.StartFloor);
            angryPills = board.Get(Element.FuryPill);
            flyingPills = board.Get(Element.FlyingPill);
            mySnake = board.GetSnake();
            foreach(Point point in startFloors)
            {
                isBarrier[point.X, point.Y] = true;
            }
            foreach(Point from in enemyHeads)
            {
                Point up = new Point(from.X, from.Y + 1);
                Point down = new Point(from.X, from.Y - 1);
                Point left = new Point(from.X - 1, from.Y);
                Point right = new Point(from.X + 1, from.Y);
                if (correctPoint(up))
                {
                    isBarrier[up.X, up.Y] = true;
                }
                if (correctPoint(down))
                {
                    isBarrier[down.X, down.Y] = true;
                }
                if (correctPoint(left))
                {
                    isBarrier[left.X, left.Y] = true;
                }
                if (correctPoint(right))
                {
                    isBarrier[right.X, right.Y] = true;
                }
            }
            foreach (Point point in enemyPoints)
            {
                isBarrier[point.X, point.Y] = true;
            }
            foreach(Point point in stones)
            {
                isBarrier[point.X, point.Y] = true;
            }
            foreach(Point point in walls)
            {
                isBarrier[point.X, point.Y] = true;
            }
            foreach(Point point in mySnake)
            {
                isBarrier[point.X, point.Y] = true;
            }
        }
        public string Survive()
        {
            Point up = new Point(myHead.X, myHead.Y + 1);
            Point down = new Point(myHead.X, myHead.Y - 1);
            Point left = new Point(myHead.X - 1, myHead.Y);
            Point right = new Point(myHead.X + 1, myHead.Y);
            if (!board.IsAt(myHead, Element.HeadLeft) && correctPoint(right))
            {
                if (board.IsAt(right, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(right, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && correctPoint(left))
            {
                if (board.IsAt(left, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(left, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadDown) && correctPoint(up))
            {
                if (board.IsAt(up, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(up, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Up.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && correctPoint(right))
            {
                if (board.IsAt(right, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(right, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && correctPoint(left))
            {
                if (board.IsAt(left, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(left, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadUp) && correctPoint(down))
            {
                if (board.IsAt(down, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(down, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Down.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && correctPoint(right))
            {
                if (board.IsAt(right, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(right, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && correctPoint(left))
            {
                if (board.IsAt(left, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(left, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && correctPoint(right))
            {
                if (board.IsAt(right, Element.FuryPill))
                {
                    underAngryPill = 10;
                }
                if (board.IsAt(right, Element.FlyingPill))
                {
                    underFlyingPill = 10;
                }
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && notBarrier(left))
            {
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && notBarrier(right))
            {
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadUp) && notBarrier(down))
            {
                return Direction.Down.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadDown) && notBarrier(up))
            {
                return Direction.Up.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && isHead(left))
            {
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && isHead(right))
            {
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadUp) && isHead(down))
            {
                return Direction.Down.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadDown) && isHead(up))
            {
                return Direction.Up.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadRight) && isMyBody(left))
            {
                return Direction.Left.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadLeft) && isMyBody(right))
            {
                return Direction.Right.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadUp) && isMyBody(down))
            {
                return Direction.Down.ToString();
            }
            if (!board.IsAt(myHead, Element.HeadDown) && isMyBody(up))
            {
                return Direction.Up.ToString();
            }
            return Direction.Up.ToString();
        }
        public bool isHead(Point pt)
        {
            if (board.IsAt(pt, Element.EnemyHeadEvil) || board.IsAt(pt, Element.EnemyHeadDown) || board.IsAt(pt, Element.EnemyHeadFly) || board.IsAt(pt, Element.EnemyHeadLeft) ||
                    board.IsAt(pt, Element.EnemyHeadRight) || board.IsAt(pt, Element.EnemyHeadUp))
            {
                return true;
            }
            return false;
        }
        public bool isEnemyBody(Point pt)
        {
            if (board.IsAt(pt, Element.EnemyBodyHorizontal) || board.IsAt(pt, Element.EnemyBodyLeftDown) || board.IsAt(pt, Element.EnemyBodyLeftUp) ||
                board.IsAt(pt, Element.EnemyBodyRightDown) || board.IsAt(pt, Element.EnemyBodyRightUp) || board.IsAt(pt, Element.EnemyBodyVertical) ||
                board.IsAt(pt, Element.EnemyHeadDown) || board.IsAt(pt, Element.EnemyHeadEvil) ||
                board.IsAt(pt, Element.EnemyHeadLeft) || board.IsAt(pt, Element.EnemyHeadRight) || board.IsAt(pt, Element.EnemyHeadSleep) ||
                board.IsAt(pt, Element.EnemyHeadUp) || board.IsAt(pt, Element.EnemyTailEndDown) || board.IsAt(pt, Element.EnemyTailEndLeft) ||
                board.IsAt(pt, Element.EnemyTailEndRight) || board.IsAt(pt, Element.EnemyTailEndUp))
                return true;
            return false;
        }
        public bool isMyBody(Point pt)
        {
            if (board.IsAt(pt, Element.TailEndUp) || board.IsAt(pt, Element.TailEndRight) || board.IsAt(pt, Element.TailEndLeft) || board.IsAt(pt, Element.TailEndDown) ||
                board.IsAt(pt, Element.BodyVertical) || board.IsAt(pt, Element.BodyRightUp) || board.IsAt(pt, Element.BodyRightDown) || board.IsAt(pt, Element.BodyLeftUp) ||
                board.IsAt(pt, Element.BodyLeftDown) || board.IsAt(pt, Element.BodyHorizontal))
                return true;
            return false;
        }
        int getValue(Point pt)
        {
            if (board.IsAt(pt, Element.Gold))
            {
                return 5;
            }
            if (board.IsAt(pt, Element.Stone))
            {
                return 4;
            }
            if (board.IsAt(pt, Element.FuryPill))
            {
                return 5;
            }
            if (isEnemyBody(pt))
            {
                return 10;
            }
            return 0;
        }
        public string makeTurn()
        {
            if (underFlyingPill > 0)
            {
                underFlyingPill -= 1;
            }
            if (underAngryPill > 0)
            {
                underAngryPill -= 1;
            }
            List<Point> bounty = board.GetBounty();
            foreach(Point point in flyingPills)
            {
                bounty.Add(point);
            }
            foreach(Point point in angryPills)
            {
                bounty.Add(point);
            }
            for (int i = 0; i < enemyHeads.Count; i += 1)
            {
                runBFS(enemyHeads[i], i);
            }
            if (underFlyingPill > underAngryPill)
            {
                foreach(Point point in mySnake)
                {
                    isBarrier[point.X, point.Y] = false;
                }
                foreach(Point enemy in enemyPoints)
                {
                    isBarrier[enemy.X, enemy.Y] = false;
                }
                foreach(Point pill in angryPills)
                {
                    bounty.Remove(pill);
                }
                foreach (Point from in enemyHeads)
                {
                    Point up = new Point(from.X, from.Y + 1);
                    Point down = new Point(from.X, from.Y - 1);
                    Point left = new Point(from.X - 1, from.Y);
                    Point right = new Point(from.X + 1, from.Y);
                    if (notBarrier(up)) 
                    {
                        isBarrier[up.X, up.Y] = false;
                    }
                    if (notBarrier(down))
                    {
                        isBarrier[down.X, down.Y] = false;
                    }
                    if (notBarrier(left))
                    {
                        isBarrier[left.X, left.Y] = false;
                    }
                    if (notBarrier(right))
                    {
                        isBarrier[right.X, right.Y] = false;
                    }
                }
                foreach (Point stone in stones)
                {
                    isBarrier[stone.X, stone.Y] = false;
                    bounty.Remove(stone);
                }
            }
            if (underAngryPill > underFlyingPill)
            {
                foreach (Point from in enemyHeads)
                {
                    Point up = new Point(from.X, from.Y + 1);
                    Point down = new Point(from.X, from.Y - 1);
                    Point left = new Point(from.X - 1, from.Y);
                    Point right = new Point(from.X + 1, from.Y);
                    if (notBarrier(up)) 
                    {
                        isBarrier[up.X, up.Y] = false;
                    }
                    if (notBarrier(down))
                    {
                        isBarrier[down.X, down.Y] = false;
                    }
                    if (notBarrier(left))
                    {
                        isBarrier[left.X, left.Y] = false;
                    }
                    if (notBarrier(right))
                    {
                        isBarrier[right.X, right.Y] = false;
                    }
                }
                foreach(Point stone in stones)
                {
                    isBarrier[stone.X, stone.Y] = false;
                    bounty.Add(stone);
                }
                foreach(Point pt in enemyPoints)
                {
                    isBarrier[pt.X, pt.Y] = false;
                    bounty.Add(pt);
                }
                foreach(Point pt in flyingPills)
                {
                    bounty.Remove(pt);
                }
            }
            if (myLength > 5)
            {
                foreach(Point stone in stones)
                {
                    bounty.Add(stone);
                }
                foreach(Point stone in stones)
                {
                    isBarrier[stone.X, stone.Y] = false;
                }
            }

            runBFS(myHead, myIndex);

            List<Point> nearest = new List<Point>();
            List<Point> apples = new List<Point>();
            List<Point> golds = new List<Point>();

            foreach(Point goal in bounty)
            {
                int minDistance = infinity;
                for(int j = 0; j < 15; j += 1)
                {
                    if (distance[j, goal.X, goal.Y] < minDistance)
                    {
                        minDistance = distance[j, goal.X, goal.Y];
                    }
                }
                if (minDistance > distance[15, goal.X, goal.Y])
                {
                    if (board.IsAt(goal, Element.Gold) || board.IsAt(goal, Element.FuryPill) || ((underAngryPill > underFlyingPill && board.IsAt(goal, Element.Stone)) && distance[15, goal.X, goal.Y] <= underAngryPill) ||
                        (underAngryPill > underFlyingPill && isEnemyBody(goal) && distance[15, goal.X, goal.Y] <= underAngryPill))
                    {
                        golds.Add(goal);
                    } else
                    {
                        apples.Add(goal);
                    }
                }
            }
            var golds_array = golds.ToArray();
            var apples_array = apples.ToArray();
            for (int i = 0; i + 1 < apples_array.Length; i++)
            {
                for(int j = i + 1; j < apples_array.Length; j++)
                {
                    if (distance[15, apples_array[i].X, apples_array[i].Y] > distance[15, apples_array[j].X, apples_array[j].Y])
                    {
                        Point tmp = apples_array[i];
                        apples_array[i] = apples_array[j];
                        apples_array[j] = tmp;
                    }
                }
            }
            for (int i = 0; i + 1 < golds_array.Length; i++)
            {
                for (int j = i + 1; j < golds_array.Length; j++)
                {
                    if (distance[15, golds_array[i].X, golds_array[i].Y] - getValue(golds_array[i]) > distance[15, golds_array[j].X, golds_array[j].Y] - getValue(golds_array[i]))
                    {
                        Point tmp = golds_array[i];
                        golds_array[i] = golds_array[j];
                        golds_array[j] = tmp;
                    }
                }
            }
            int id1 = 0, id2 = 0;
            while (id1 < golds_array.Length || id2 < apples_array.Length)
            {
                if (id1 < golds_array.Length && id2 < apples_array.Length)
                {
                    int d1 = distance[15, golds_array[id1].X, golds_array[id1].Y];
                    int d2 = distance[15, apples_array[id2].X, apples_array[id2].Y];
                    if (d1 - getValue(golds_array[id1]) <= d2)
                    {
                        nearest.Add(golds_array[id1]);
                        id1 += 1;
                    } else
                    {
                        nearest.Add(apples_array[id2]);
                        id2 += 1;
                    }
                } else 
                if (id1 < golds_array.Length)
                {
                    nearest.Add(golds_array[id1]);
                    id1 += 1;
                } else
                {
                    nearest.Add(apples_array[id2]);
                    id2 += 1;
                }
            }
            while (nearest.Count != 0)
            {
                Point best = new Point();
                foreach(Point pt in nearest)
                {
                    best = pt;
                    break;
                }
                Point was = new Point(best.X, best.Y);
                List<Point> changes = new List<Point>();
                while (parent[best.X, best.Y] != myHead)
                {
                    best = parent[best.X, best.Y];
                    isBarrier[best.X, best.Y] = true;
                    changes.Add(best);
                }
                string t = testDirection(parent[was.X, was.Y], was);
                if (t == "BAD")
                {
                    nearest.Remove(was);
                }
                foreach(Point point in changes)
                {
                    isBarrier[point.X, point.Y] = false;
                }
                if (t != "BAD")
                {
                    return getDirection(myHead, best);
                }
            }
            return Survive();
        }
        public string dumbBot()
        {
            if (underFlyingPill > 0)
            {
                underFlyingPill -= 1;
            }
            if (underAngryPill > 0)
            {
                underAngryPill -= 1;
            }
            List<Point> bounty = board.GetBounty();
            foreach (Point stone in stones)
            {
                if (bounty.Contains(stone))
                {
                    bounty.Remove(stone);
                }
                isBarrier[stone.X, stone.Y] = true;
            }
            foreach (Point point in flyingPills)
            {
                bounty.Add(point);
            }
            foreach (Point point in angryPills)
            {
                bounty.Add(point);
            }
            for (int i = 0; i < enemyHeads.Count; i += 1)
            {
                runBFS(enemyHeads[i], i);
            }
            if (underFlyingPill > underAngryPill)
            {
                foreach (Point point in mySnake)
                {
                    isBarrier[point.X, point.Y] = false;
                }
                foreach (Point enemy in enemyPoints)
                {
                    isBarrier[enemy.X, enemy.Y] = false;
                }
                foreach (Point pill in angryPills)
                {
                    bounty.Remove(pill);
                }
                foreach (Point from in enemyHeads)
                {
                    Point up = new Point(from.X, from.Y + 1);
                    Point down = new Point(from.X, from.Y - 1);
                    Point left = new Point(from.X - 1, from.Y);
                    Point right = new Point(from.X + 1, from.Y);
                    if (notBarrier(up))
                    {
                        isBarrier[up.X, up.Y] = false;
                    }
                    if (notBarrier(down))
                    {
                        isBarrier[down.X, down.Y] = false;
                    }
                    if (notBarrier(left))
                    {
                        isBarrier[left.X, left.Y] = false;
                    }
                    if (notBarrier(right))
                    {
                        isBarrier[right.X, right.Y] = false;
                    }
                }
                foreach (Point stone in stones)
                {
                    isBarrier[stone.X, stone.Y] = false;
                    bounty.Remove(stone);
                }
            }
            if (underAngryPill > underFlyingPill)
            {
                foreach (Point from in enemyHeads)
                {
                    Point up = new Point(from.X, from.Y + 1);
                    Point down = new Point(from.X, from.Y - 1);
                    Point left = new Point(from.X - 1, from.Y);
                    Point right = new Point(from.X + 1, from.Y);
                    if (notBarrier(up))
                    {
                        isBarrier[up.X, up.Y] = false;
                    }
                    if (notBarrier(down))
                    {
                        isBarrier[down.X, down.Y] = false;
                    }
                    if (notBarrier(left))
                    {
                        isBarrier[left.X, left.Y] = false;
                    }
                    if (notBarrier(right))
                    {
                        isBarrier[right.X, right.Y] = false;
                    }
                }
                foreach (Point stone in stones)
                {
                    isBarrier[stone.X, stone.Y] = false;
                    bounty.Add(stone);
                }
                foreach (Point pt in enemyPoints)
                {
                    isBarrier[pt.X, pt.Y] = false;
                    bounty.Add(pt);
                }
                foreach (Point pt in flyingPills)
                {
                    bounty.Remove(pt);
                }
            }

            runBFS(myHead, myIndex);

            List<Point> nearest = new List<Point>();

            foreach (Point goal in bounty)
            {
                int minDistance = infinity;
                for (int j = 0; j < 15; j += 1)
                {
                    if (distance[j, goal.X, goal.Y] < minDistance)
                    {
                        minDistance = distance[j, goal.X, goal.Y];
                    }
                }
                if (minDistance > distance[15, goal.X, goal.Y])
                {
                    nearest.Add(goal);
                }
            }
            var nArray = nearest.ToArray();
            for(int i = 0; i < nArray.Length; i++)
            {
                for(int j = i + 1; j < nArray.Length; j++)
                {
                    if (distance[15, nArray[i].X, nArray[i].Y] > distance[15, nArray[j].X, nArray[j].Y])
                    {
                        var tmp = nArray[i];
                        nArray[i] = nArray[j];
                        nArray[j] = tmp;
                    }
                }
            }
            nearest.Clear();
            for(int i = 0; i < nArray.Length; i++)
            {
                nearest.Add(nArray[i]);
            }
            while (nearest.Count != 0)
            {
                Point best = new Point();
                foreach (Point pt in nearest)
                {
                    best = pt;
                    break;
                }
                Point was = new Point(best.X, best.Y);
                List<Point> changes = new List<Point>();
                while (parent[best.X, best.Y] != myHead)
                {
                    best = parent[best.X, best.Y];
                    isBarrier[best.X, best.Y] = true;
                    changes.Add(best);
                }
                string t = testDirection(parent[was.X, was.Y], was);
                if (t == "BAD")
                {
                    nearest.Remove(was);
                }
                foreach (Point point in changes)
                {
                    isBarrier[point.X, point.Y] = false;
                }
                if (t != "BAD")
                {
                    return getDirection(myHead, best);
                }
            }
            return Survive();
        }

        public int pulled = 0;
        public string GetNextMove()
        {
            initialize();
            Console.WriteLine(board.GetDisplay());
            Console.WriteLine("Tick: {0}", board.remember);
            Console.WriteLine("My coords: {0}, {1}", myHead.X, myHead.Y);
            foreach(Point pill in angryPills)
            {
                Console.WriteLine("Pill at: {0}, {1}", pill.X, pill.Y);
            }
            if (board.IsSnakeAlive() == false)
            {
                underFlyingPill = 0;
                underAngryPill = 0;
                return Direction.Right.ToString();
            }

            if (board.IsAt(myHead, Element.StartFloor))
            {
                return Direction.Right.ToString();
            }
            pulled = 0;
            if (board.remember <= 100)
            {
                if (pulled == 0 && underAngryPill > underFlyingPill)
                {
                    return makeTurn() + "," + Direction.Act.ToString();
                }
                return makeTurn();
            } else
            {
                return dumbBot();
            }
        }
    }
    // давай будем рассматривать прямоугольники или квадраты какого то размера
    // будем считать сколько в этом прямоугольнике золота и яблоки
    // можно также считать и камни
    // если можно будет их собирать
    // затем давай рассмотрим ближайшие квадраты и для каждого ближайшего посчитаем
    // сколько максимум оттуда можно будет собрать
    // если нам удастся собрать максимум, то будем вычислять путь
    // скорее всего это не бфс, и не гамильтонов цикл
    // сделаем backtracking и будем идти по одному шагу

    // для furyPill посчитать в радиусе 10 манхэттенского расстояния количество камней

}

