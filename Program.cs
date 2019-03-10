using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RTS_Server
{
    class Program
    {
        public static readonly long tick_time_millis = 1000/30; // 30 ticks per second
        public static long delta_time = 0;
        public static long last_update = 0;

        static void Main(string[] args)
        {
            var r = new Random();
            int size = r.Next(7, 12);
            var pg = new Playground(size, size);

            pg.SpawnUnit(3, 1);
            pg.SpawnUnit(6, 6);
            pg.SpawnUnit(3, 5);
            pg.SpawnUnit(2, 2);
            pg.SpawnUnit(0, 1);

            var server = new Server();

            while (true)
            {
                foreach(var u in pg.units)
                {
                    u.Update();
                }
                //PrintPlayground(pg);

                Thread.Sleep((int)(tick_time_millis));
                UpdateTimer();
            }
        }

        private static void UpdateTimer()
        {
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            delta_time = now - last_update;
            last_update = now;
        }

        public static void PrintPlayground(Playground pg)
        {
            Console.Clear();
            for (int i = 0; i < pg.size.X; i++)
            {
                Console.WriteLine();
                for (int j = 0; j < pg.size.Y; j++)
                {
                    bool unit_stays = false;
                    bool next_point = false;
                    for (int k = 0; k < pg.units.Count; k++)
                    {
                        if (pg.units[k].parent.pos.X == i && pg.units[k].parent.pos.Y == j)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(k);
                            unit_stays = true;
                        }
                        if (pg.units[k].next != null && pg.units[k].next.pos.X == i && pg.units[k].next.pos.Y == j)
                            next_point = true;
                    }
                    if (unit_stays == false)
                    {
                        if (next_point == true)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("N");
                        }
                        else if (pg.playground[i, j].walkable == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("W");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("-");
                        }
                    }
                }
            }
            string lerp_info = "";
            for (int i = 0; i < pg.units.Count; i++)
            {
                lerp_info += "\n" + i + " unit move% = " + pg.units[i].move_progress;
            }
            Console.WriteLine(lerp_info);
        }
    }
}
