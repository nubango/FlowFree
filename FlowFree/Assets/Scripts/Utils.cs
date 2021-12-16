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
        public static Coord operator +(Coord a, Coord b) => new Coord(a.x + b.x, a.y + b.y);
        public static Coord operator *(Coord b, int a) => new Coord(a * b.x, a * b.y);
    }

    public struct TraceInTile
    {
        public TraceInTile(Coord position, Coord direction, Color color)
        {
            this.position = position;
            this.direction = direction;
            this.color = color;
        }       

        public Coord position { get; set; }
        public Coord direction { get; set; }
        public Color color { get; set; }

        public static bool operator ==(TraceInTile a, TraceInTile b) => (a.position == b.position);
        public static bool operator !=(TraceInTile a, TraceInTile b) => (a.position != b.position);
        public static bool operator ==(TraceInTile a, Coord b) => (a.position.x == b.x && a.position.y == b.y);
        public static bool operator !=(TraceInTile a, Coord b) => (a.position.x != b.x || a.position.y == b.y);
    }
}
