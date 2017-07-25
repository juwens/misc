using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameModel
{
    [Serializable]
    struct Coord
    {
        public readonly int X;
        public readonly int Y;

        public Coord(Coord xy)
        {
            this.X = xy.X;
            this.Y = xy.Y;
        }

        public Coord(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"{{X: {this.X}, Y: {this.Y}}}";
        }

        public static Coord operator +(Coord c1, Coord c2) => new Coord(c1.X + c2.X, c1.Y + c2.Y);
    }
}
