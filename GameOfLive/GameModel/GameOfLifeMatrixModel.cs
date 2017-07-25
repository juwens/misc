using System;
using Accord.Math;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GameModel
{
    /// <summary>
    /// Inspired by:
    /// - http://www.labri.fr/perso/nrougier/teaching/numpy/numpy.html#the-game-of-life
    /// - http://www.petrkeil.com/?p=236
    /// - http://ocw.mit.edu/courses/mathematics/18-s997-introduction-to-matlab-programming-fall-2011/conway-game-of-life/conway-game-of-life-implementation/
    /// 
    /// Implementation using https://github.com/accord-net/framework/wiki/Mathematics
    /// 
    /// TODO: Implementation using http://designengrlab.github.io/StarMath/
    /// </summary>
    public class GameOfLifeMatrixModel : IGameOfLifeModel
    {
        private int height;
        private int width;
        private double[,] board;

        public GameOfLifeMatrixModel()
        {
        }

        public static void print<T>(T[,] matrix)
        {
            Trace.WriteLine("------------------------------------------");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Trace.Write(matrix[i, j] + "\t");
                }
                Trace.WriteLine("");
            }
            Trace.WriteLine("------------------------------------------");
        }

        public bool[,] Board => this.board.Apply(x => x > 0);

        public int Height => this.height;

        public int Width => this.width;

        public int Generation { get; private set; }

        private object stepLock = new object();

        public void SingleStep()
        {
            lock (stepLock)
            {
                Generation++;
                // https://github.com/accord-net/framework/wiki/Mathematics
                var oldBoardClone = (double[,])this.board.Clone();
                var offsetVectors = new List<Coord>
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


                var xMax = oldBoardClone.Columns() - 2;
                var yMax = oldBoardClone.Rows() - 2;
                var sumOfNeighbours = new double[xMax, yMax];
                foreach (var vector in offsetVectors)
                {
                    sumOfNeighbours = sumOfNeighbours.Add(oldBoardClone.Submatrix(1 + vector.Y, yMax + vector.Y, 1 + vector.X, xMax + vector.X));
                }

                var sumOfNeighboursWithPadding = sumOfNeighbours.Pad(1);

                print(oldBoardClone);
                print(sumOfNeighboursWithPadding);

                // aus alive wird +1; aus dead -1
                oldBoardClone = oldBoardClone.Multiply(2.0).Add(-1.0);

                var nextGenBoard = oldBoardClone.ElementwiseMultiply(sumOfNeighboursWithPadding).Apply(i =>
                {
                    if (i > 0)
                    {
                        // alive
                        // Rule 1: Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                        // !!! Rule 2: Any live cell with two or three live neighbours lives on to the next generation. !!!
                        // Rule 3: Any live cell with more than three live neighbours dies, as if by over-population.
                        return (i == 2 || i == 3) ? 1.0 : 0.0;
                    }
                    else if (i < 0)
                    {
                        // Rule 4: Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                        return (i == -3.0) ? 1.0 : 0.0;
                    }
                    else
                    {
                        return 0.0;
                    }
                });

                this.board = nextGenBoard;
            }
        }

        public void Init(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.Generation = 0;

            board = new double[width, height];

            var rng = new Random();
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    this.board[x, y] = (double)rng.Next(2);
                }
            }
        }
    }
}