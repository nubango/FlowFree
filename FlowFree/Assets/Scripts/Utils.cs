using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public struct Wall
    {
        public Wall(Coord init, Coord end)
        {
            this.init = init;
            this.end = end;
        }

        public Coord init { get; set; }
        public Coord end { get; set; }
    };

    public struct Coord
    {
        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int x { get; set; }
        public int y { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Coord coord &&
                   x == coord.x &&
                   y == coord.y;
        }

        public static bool operator ==(Coord a, Coord b) => (a.x == b.x && a.y == b.y);
        public static bool operator !=(Coord a, Coord b) => (a.x != b.x || a.y != b.y);
        public static Coord operator -(Coord a, Coord b) => new Coord(a.x - b.x, a.y - b.y);
    }
}
