using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace RTS_Server
{
    class Playground
    {
        public static Playground instance;

        public Pos2D size;
        public Node[,] playground;
        public List<Unit> units;

        public void SpawnUnit(int x, int y)
        {
            units.Add(new Unit(units.Count, playground[x, y]));
        }

        public Playground(int size_x, int size_y)
        {
            if (instance != null)
                throw new Exception("Singleton already instantiated");
            else
                instance = this;


            size = new Pos2D()
            {
                X = size_x,
                Y = size_y
            };

            playground = new Node[size.X, size.Y];
            for (int i = 0; i < size.X; i++)
            {
                for (int j = 0; j < size.Y; j++)
                {
                    playground[i, j] = new Node(true, new Pos2D { X = i, Y = j});
                }
            }

            units = new List<Unit>();
        }

        public List<Node> GetNeighbours(Node node)
        {
            var neighbours = new List<Node>();

            if (node.pos.X + 1 < size.X)
                neighbours.Add(playground[node.pos.X + 1, node.pos.Y]);
            if (node.pos.X - 1 >= 0)
                neighbours.Add(playground[node.pos.X - 1, node.pos.Y]);
            if (node.pos.Y + 1 < size.Y)
                neighbours.Add(playground[node.pos.X, node.pos.Y + 1]);
            if (node.pos.Y - 1 >= 0)
                neighbours.Add(playground[node.pos.X, node.pos.Y - 1]);

            return neighbours;
        }

        public List<Node> FindPath(Node start, Node target)
        {
            // The set of currently discovered nodes that are not evaluated yet.
            var open_set = new HashSet<Node>();
            // The set of nodes already evaluated
            var closed_set = new HashSet<Node>();

            start.came_from = null;
            start.g_cost = 0;
            start.h_cost = GetHeuristicDistance(start, target);

            open_set.Add(start);

            while(open_set.Count > 0)
            {
                var current_node = open_set.OrderBy(Node => Node.f_cost).First();

                if (current_node == target)
                {
                    return RetracePath(target);
                }

                open_set.Remove(current_node);
                closed_set.Add(current_node);

                foreach(var neighbour in GetNeighbours(current_node))
                {
                    if (neighbour.walkable == false || closed_set.Contains(neighbour))
                        continue;

                    // The distance from start to a neighbor
                    double tentative_gScore = current_node.g_cost + GetHeuristicDistance(current_node, neighbour);

                    if (open_set.Contains(neighbour) == false)
                        open_set.Add(neighbour);
                    else if (tentative_gScore >= neighbour.g_cost)
                        continue;

                    neighbour.came_from = current_node;
                    neighbour.g_cost = tentative_gScore;
                    neighbour.h_cost = GetHeuristicDistance(neighbour, target);
                }
            }
            return null;
        }

        private List<Node> RetracePath(Node target)
        {
            var path = new List<Node>();
            var current_node = target;
            while (current_node != null)
            {
                path.Add(current_node);
                current_node = current_node.came_from;
            }
            path.Reverse();
            return path;
        }

        // Manhattan Distance
        private double GetHeuristicDistance(Node a, Node b)
        {
            double x = Math.Pow(a.pos.X - b.pos.X, 2);
            double y = Math.Pow(a.pos.Y - b.pos.Y, 2);

            return Math.Sqrt(x + y);
        }

        public void SetTarget(Pos2D pos, int[] ids)
        {
            foreach (var u in units)
            {
                foreach(var id in ids)
                {
                    if (id == u.id)
                        u.SetTarget(playground[pos.X, pos.Y]);
                }
            }
        }
    }
}
