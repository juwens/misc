namespace GameModel
{
    public interface IGameOfLifeModel
    {
        void Init(int width, int height);

        bool[,] Board { get; }
        int Height { get; }
        int Width { get; }
        int Generation { get; }

        void SingleStep();
    }
}