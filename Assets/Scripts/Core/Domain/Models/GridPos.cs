public readonly struct GridPos : System.IEquatable<GridPos>
{
    public readonly int X;
    public readonly int Y;

    public GridPos(int x, int y) { X = x; Y = y; }

    public static GridPos operator +(GridPos a, GridPos b) => new GridPos(a.X + b.X, a.Y + b.Y);

    public bool Equals(GridPos other) => X == other.X && Y == other.Y;
    public override bool Equals(object obj) => obj is GridPos g && Equals(g);
    public override int GetHashCode() => X * 1000 + Y;
    public override string ToString() => $"({X}, {Y})";
}
