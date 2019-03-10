using System;
using System.Collections.Generic;
using System.Text;

namespace RTS_Server
{
    public class Node
    {
        public bool walkable;
        public Pos2D pos;

        public Node came_from;

        // cost from start to this
        public double g_cost;
        // heuristic cost from this to target
        public double h_cost;
        public double f_cost
        {
            get
            {
                return g_cost + h_cost;
            }
        }

        public Node(bool _walkable, Pos2D _pos)
        {
            walkable = _walkable;
            pos = _pos;
        }
    }
}
