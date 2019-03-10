using System;
using System.Threading;

namespace RTS_Server
{
    public class Unit
    {
        public int id;
        public Node parent;
        public Node next;
        public Node target;

        public bool moving;
        public float move_progress;

        public Unit(int _id, Node _parent)
        {
            id = _id;
            parent = _parent;
            parent.walkable = false;
            target = parent;
        }

        public void Update()
        {
            HandleMovement();
        }

        public void HandleMovement()
        {
            if (moving)
            {
                move_progress += (Program.delta_time/1000f) / 1f; // 1 sec transition
                if (move_progress >= 1f)
                {
                    moving = false;
                    move_progress = 0f;
                    parent = next;
                    next = null;
                }
            }
            else if (parent != target)
            {
                if (target.walkable == false)
                {
                    target = Playground.instance.GetNeighbours(target).Find(node => node.walkable == true);
                }

                var path = Playground.instance.FindPath(parent, target);
                if (path != null && path.Count > 1)
                {
                    MoveTo(path[1]);
                }
            }
        }

        public void MoveTo(Node node)
        {
            next = node;

            parent.walkable = true;
            next.walkable = false;

            moving = true;
        }

        public void SetTarget(Node _target)
        {
            target = _target;
        }
    }
}
