using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Core
{
	public static class Flock
	{
		public static void UpdateVelocity(List<Entity> entities, Entity followTarget)
		{
			var neighbourRadius = 0.5f;
			var maxSpeed = 3f;
			var squareMaxSpeed = math.sqrt(maxSpeed);

			for (int entityIndex = 0; entityIndex < entities.Count; entityIndex++)
			{
				var entity = entities[entityIndex];

				var neighbours = GetNearbyObjects(entity, neighbourRadius, entities);
				Vector2 move = CalculateCompositeMove(entity, neighbours, followTarget);
				if (move.sqrMagnitude > squareMaxSpeed)
				{
					move = move.normalized * maxSpeed;
				}

				entity.Velocity = move;
				// entity.Velocity = move * Time.deltaTime * entity.MoveSpeed;
			}
		}

		private static float2 CalculateCompositeMove(Entity entity, List<Entity> neighbours, Entity followTarget)
		{
			var move = new Vector2();
			float weight;
			Vector2 partialMove;

			// weight = 1f;
			// partialMove = CalculateCohesionMove(entity, neighbours) * weight;

			// if (partialMove.Equals(float2.zero) == false)
			// {
			// 	if (partialMove.sqrMagnitude > math.sqrt(weight))
			// 	{
			// 		partialMove.Normalize();
			// 		partialMove *= weight;
			// 	}

			// 	move += partialMove;
			// }

			weight = 1f;
			partialMove = CalculateAlignmentMove(entity, neighbours, followTarget) * weight;
			if (partialMove.Equals(float2.zero) == false)
			{
				if (partialMove.sqrMagnitude > math.sqrt(weight))
				{
					partialMove.Normalize();
					partialMove *= weight;
				}

				move += partialMove;
			}

			weight = 1f;
			partialMove = CalculateAvoidanceMove(entity, neighbours) * weight;
			if (partialMove.Equals(float2.zero) == false)
			{
				if (partialMove.sqrMagnitude > math.sqrt(weight))
				{
					partialMove.Normalize();
					partialMove *= weight;
				}

				move += partialMove;
			}

			return move.normalized;
		}

		private static float2 CalculateCohesionMove(Entity entity, List<Entity> neighbours)
		{
			var move = new float2();

			if (neighbours.Count == 0)
			{
				return move;
			}

			foreach (var neighbour in neighbours)
			{
				move += neighbour.Position;
			}

			move /= neighbours.Count;
			move -= entity.Position;

			return move;
		}

		private static float2 CalculateAlignmentMove(Entity entity, List<Entity> neighbours, Entity followTarget)
		{
			var move = new float2();

			if (neighbours.Count == 0)
			{
				return math.normalizesafe(followTarget.Position - entity.Position);
			}

			foreach (var neighbour in neighbours)
			{
				move += math.normalizesafe(followTarget.Position - entity.Position);
			}

			move /= neighbours.Count;

			return move;
		}


		private static float2 CalculateAvoidanceMove(Entity entity, List<Entity> neighbours)
		{
			var avoidanceRadius = 0.5f;
			var squareAvoidanceRadius = math.sqrt(avoidanceRadius);

			var move = new float2();

			if (neighbours.Count == 0)
			{
				return move;
			}

			var numberOfEntitiesToAvoid = 0;
			foreach (var neighbour in neighbours)
			{
				if (Vector2.SqrMagnitude(neighbour.Position - entity.Position) < squareAvoidanceRadius)
				{
					move += entity.Position - neighbour.Position;
					numberOfEntitiesToAvoid += 1;
				}
			}

			if (numberOfEntitiesToAvoid > 0)
			{
				move /= numberOfEntitiesToAvoid;
			}

			return move;
		}

		private static List<Entity> GetNearbyObjects(Entity entity, float neighbourRadius, List<Entity> entities)
		{
			var neighbours = new List<Entity>();
			var colliders = Physics2D.OverlapCircleAll(
				entity.Position,
				neighbourRadius
			);

			foreach (var collider in colliders)
			{
				if (collider == entity.Component.Collider)
				{
					continue;
				}

				var neighbour = entities.Find(e => e.Component.Collider == collider);
				if (neighbour == null)
				{
					continue;
				}
				neighbours.Add(neighbour);
			}

			return neighbours;
		}
	}
}
