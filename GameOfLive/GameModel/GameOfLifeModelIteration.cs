using System;
using System.Collections.Generic;
//using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GameModel
{
    class GameOfLifeModelIteration : IGameOfLifeModel
    {
        private bool[,] board;
        private ReadOnlyDictionary<Coord, ReadOnlyCollection<Coord>> neighbours;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool[,] Board => (bool[,])board.Clone();

        public int Generation
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GameOfLifeModelIteration()
        {
            
        }

        private static long GetObjectSize(object o)
        {
            using (Stream s = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                return s.Length;
            }

        }

        public void SingleStep()
        {
            var newBoard = new bool[this.Width, this.Height];

            foreach (var item in this.neighbours)
            {
                var xy = item.Key;
                var nb = item.Value;
                var currentState = this.board[xy.X, xy.Y];
                var aliveNbCnt = nb.Count(xy2 => this.board[xy2.X, xy2.Y]);
                var newState = false;

                if (currentState == false && aliveNbCnt == 3)
                {
                    newState = true;
                }
                else if (currentState == true && aliveNbCnt == 2 || aliveNbCnt == 3)
                {
                    newState = true;
                }

                newBoard[xy.X, xy.Y] = newState;
            }

            this.board = newBoard;
        }

        public void Init(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.board = new bool[width, height];

            var neighbours = new Dictionary<Coord, ICollection<Coord>>();

            var noGc = GC.TryStartNoGCRegion(1024 * 1024 * 200);
            //GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            //new ReadOnlyCollection<Coord>()
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cr2 = new Coord(x, y);
                    neighbours[cr2] = null;
                }
            }
            var bytes = GetObjectSize(neighbours);

            if (noGc) GC.EndNoGCRegion();

            var o = new List<Coord>
            {
                new Coord(-1,  0),
                new Coord(-1, -1),
                new Coord( 0, -1),
                new Coord(+1, -1),
                new Coord(+1,  0),
                new Coord(+1, +1),
                new Coord( 0, +1),
                new Coord(-1, +1)
            };


            //GCSettings.LatencyMode = GCLatencyMode.Batch;
            foreach (var xy in neighbours.Keys)
            {
                //foreach (var offsets in o)
                for (int i = 0; i < o.Count; i++)
                //Parallel.ForEach(o, (offsets) => // langsamer
                {
                    //var target = xy + o[i];
                    var newX = xy.X + o[i].X;
                    var newY = xy.Y + o[i].Y;

                    if (newX >= 0 && newX < this.Width && newY >= 0 && newY < this.Height)
                    {
                        neighbours[xy].Add(new Coord(newX, newY));
                    }
                }
            }
            //GCSettings.LatencyMode = GCLatencyMode.Interactive;


            var dict = neighbours.ToDictionary(k => k.Key, v => new ReadOnlyCollection<Coord>(v.Value.ToList()));
            this.neighbours = new ReadOnlyDictionary<Coord, ReadOnlyCollection<Coord>>(dict);

            var rnd = new Random();
            foreach (var xy in this.neighbours.Keys)
            {
                board[xy.X, xy.Y] = rnd.Next(2) == 1;
            }

        }
    }
}
