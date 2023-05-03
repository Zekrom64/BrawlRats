using System;
using System.Numerics;
using Box2D.NetStandard.Collision.Shapes;
using BrawlRats.Util;

namespace BrawlRats.Physics {
	public static class Shapes {

		public static PolygonShape Box(float width, float height, Vector2 center = default, float angle = 0) {
			var shape = new PolygonShape();
			shape.SetAsBox(width, height, center, MathUtil.ToRadians(angle));
			return shape;
		}

		public static PolygonShape Box(float width, float height, float angle = 0) {
			var shape = new PolygonShape();
			shape.SetAsBox(width, height, Vector2.Zero, MathUtil.ToRadians(angle));
			return shape;
		}
		
		public static PolygonShape RegularPolygon(float radius, int sides, Vector2 center = default, float angle = 0) {
			if (sides < 3) throw new ArgumentException("Polygon must have at least 3 sides");
			float step = MathUtil.TwoPi / sides;
			Vector2[] points = new Vector2[sides];
			for(int i = 0; i < sides; i++) {
				points[i] = center + new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * radius;
				angle += step;
			}
			var shape = new PolygonShape();
			shape.Set(points);
			return shape;
		}

		public static CircleShape Circle(float radius) => new() { Radius = radius };

		public static ChainShape Chain(Vector2[] points) {
			if (points.Length < 3) throw new ArgumentException("Chain must have at least 3 points");
			var shape = new ChainShape();
			shape.CreateChain(points[1..^2], points[0], points[^1]);
			return shape;
		}

		public static ChainShape Chain(float segmentLength, int segments) {
			if (segments < 2) throw new ArgumentException("Chain must have at least 2 segments");
			Vector2[] points = new Vector2[segments + 1];
			points[0] = Vector2.Zero;
			for (int i = 0; i < segments; i++) points[i + 1] = points[i] - new Vector2(0, segmentLength);
			return Chain(points);
		}

		public static ChainShape ChainLoop(Vector2[] points) {
			if (points.Length < 3) throw new ArgumentException("Chain must have at least 3 points");
			var shape = new ChainShape();
			shape.CreateLoop(points);
			return shape;
		}

		public static ChainShape ChainLoop(float radius, int segments, float angle = 0) {
			if (segments < 3) throw new ArgumentException("Chain loop must have at least 3 segments");
			Vector2[] points = new Vector2[segments];
			float step = MathUtil.TwoPi / segments;
			for(int i = 0; i < segments; i++) {
				points[i] = new Vector2(MathF.Sin(angle), MathF.Cos(angle)) * radius;
				angle += step;
			}
			return ChainLoop(points);
		}

		public static EdgeShape Edge(Vector2 v1, Vector2 v2) {
			EdgeShape shape = new();
			shape.SetTwoSided(v1, v2);
			return shape;
		}

	}
}
