using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BuildableGingerIslandFarm.Utilities
{
	class PointComparer : IEqualityComparer<Point>
	{
		public bool Equals(Point x, Point y)
		{
			return x.X == y.X && x.Y == y.Y;
		}

		public int GetHashCode(Point obj)
		{
			return (obj.Y << 16) ^ obj.X;
		}
	}
}
