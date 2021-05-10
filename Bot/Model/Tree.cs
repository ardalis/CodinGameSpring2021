namespace Bot.Model
{
    class Tree
    {
        public int cellIndex;
        public int size;
        public bool isMine;
        public bool isDormant;

        public Tree(int cellIndex, int size, bool isMine, bool isDormant)
        {
            this.cellIndex = cellIndex;
            this.size = size;
            this.isMine = isMine;
            this.isDormant = isDormant;
        }
    }
}