class Cell
{
    public int index;
    public int richness;
    public int[] neighbours;

    public Cell(int index, int richness, int[] neighbours)
    {
        this.index = index;
        this.richness = richness;
        this.neighbours = neighbours;
    }

    public int[] ShadeFreeLocations()
    {
        // for a given cell index, list the range 2 locations that are not in-line with 
        // it on a hex row (so they can't shade one another)
        switch (index)
        {
            case 0:
                return new[] { 8, 10, 12, 14, 16, 18 };
            case 1:
                return new[] { 20, 9, 3, 5, 17, 36 };
            case 2:
                return new[] { 21, 23, 11, 4, 6, 7 };
            case 3:
                return new[] { 9, 24, 26, 13, 5, 1 };
            case 4:
                return new[] { 2, 11, 27, 29, 15, 6 };
            case 5:
                return new[] { 1, 3, 13, 30, 32, 17 };
            case 6:
                return new[] { 7, 2, 4, 15, 33, 35 };
            case 7:
                return new[] { 21, 2, 6, 35 };
            case 8:
                return new[] { 22, 10, 0, 18, 19 };
            case 9:
                return new[] { 24, 3, 1, 20 };
            case 10:
                return new[] { 25, 12, 0, 8, 22 };
            case 11:
                return new[] { 27, 4, 2, 23 };
            case 12:
                return new[] { 28, 14, 0, 10 };
            case 13:
                return new[] { 30, 5, 3, 26 };
            case 14:
                return new[] { 12, 28, 31, 16, 0 };
            case 15:
                return new[] { 4, 29, 33, 6 };
            case 16:
                return new[] { 0, 14, 31, 34, 18 };
            case 17:
                return new[] { 5, 32, 36, 1 };
            case 18:
                return new[] { 0, 16, 34, 19, 8 };
            case 19:
                return new[] { 8, 18 };
            case 20:
                return new[] { 36, 1, 9 };
            case 21:
                return new[] { 23, 2, 7 };
            case 22:
                return new[] { 10, 8 };
            case 23:
                return new[] { 21, 2, 11 };
            case 24:
                return new[] { 9, 3, 26 };
            case 25:
                return new[] { 10, 2 };
            case 26:
                return new[] { 24, 3, 13 };
            case 27:
                return new[] { 11, 4, 29 };
            case 28:
                return new[] { 12, 14 };
            case 29:
                return new[] { 27, 4, 15 };
            case 30:
                return new[] { 13, 5, 32 };
            case 31:
                return new[] { 14, 16 };
            case 32:
                return new[] { 30, 5, 17 };
            case 33:
                return new[] { 15, 6, 35 };
            case 34:
                return new[] { 16, 18 };
            case 35:
                return new[] { 33, 6, 7 };
            case 36:
                return new[] { 17, 8, 20 };
            default:
                return new int[0];
        }
    }
}
