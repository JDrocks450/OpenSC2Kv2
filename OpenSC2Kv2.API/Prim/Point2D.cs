using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSC2Kv2.API
{
    public struct GridCoordinate
    {
        private Point2D point;
        public int Row
        {
            get => point.Y;
            set => point.Y = value;
        }
        public int Column
        {
            get => point.X;
            set => point.X = value;
        }
        public int Z { get; set; } = 0;

        public override string ToString()
        {
            return $"{{Row: {Row}, Column: {Column}}}";
        }

        public GridCoordinate(int Row, int Column)
        {
            point = new Point2D(0,0);
            this.Row = Row;
            this.Column = Column;            
        }

        public static implicit operator Point2D(GridCoordinate coord)
        {
            return coord.point;
        }
        public static implicit operator GridCoordinate(Point2D coord)
        {
            return new GridCoordinate(coord.Y, coord.X);
        }
        public override bool Equals(object obj)
        {
            if (obj is GridCoordinate coord)
                return point == coord.point;
            else return false;
        }
        public static GridCoordinate operator +(GridCoordinate left, GridCoordinate right)
        {
            return new GridCoordinate(left.Row + right.Row, left.Column + right.Column);
        }
        public static GridCoordinate operator -(GridCoordinate left, GridCoordinate right)
        {
            return new GridCoordinate(left.Row - right.Row, left.Column - right.Column);
        }
        public static GridCoordinate operator /(GridCoordinate left, GridCoordinate right)
        {
            return new GridCoordinate(left.Row / right.Row, left.Column / right.Column);
        }
        public static GridCoordinate operator *(GridCoordinate left, GridCoordinate right)
        {
            return new GridCoordinate(left.Row * right.Row, left.Column * right.Column);
        }

        public static bool operator ==(GridCoordinate left, GridCoordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridCoordinate left, GridCoordinate right)
        {
            return !(left == right);
        }
    }
    /// <summary>
    /// Represents a Point in 2D space.
    /// </summary>
    [Serializable]
    public struct Point2D
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public Point2D()
        {
            
        }
        public Point2D(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public static Point2D operator *(Point2D left, int Scalar) => new Point2D(left.X * Scalar, left.Y * Scalar);
        public static Point2D operator +(Point2D left, Point2D right) => new Point2D(left.X + right.X, left.Y + right.Y);
        public static bool operator ==(Point2D left, Point2D right) => left.X == right.X && left.Y == right.Y;
        public static bool operator !=(Point2D left, Point2D right) => left.X != right.X || left.Y != right.Y;
        public override string ToString() => $"P2D: {{ X: {X}, Y:{Y} }}";
    }
}
