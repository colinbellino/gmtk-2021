using System.Collections.Generic;
using Game.Core;
using UnityEngine;

public class Flock : MonoBehaviour
{
	public FlockBehavior behavior;
	[Range(10, 500)] public int startingCount = 250;
	[Range(1f, 100f)] public float driveFactor = 10f;
	[Range(1f, 100f)] public float maxSpeed = 5f;
	[Range(0.1f, 10f)] public float neighborRadius = 1.5f;
	[Range(0f, 1f)] public float avoidanceRadiusMultiplier = 0.5f;
	[Range(0f, 1f)] public float attractionRadiusMultiplier = 0.5f;

	[HideInInspector] public float SquareAvoidanceRadius;
	[HideInInspector] public float SquareAttractionRadius;
	[HideInInspector] public Entity FollowTarget;

	float squareMaxSpeed;
	float squareNeighborRadius;

	// Start is called before the first frame update
	void Start()
	{
		squareMaxSpeed = maxSpeed * maxSpeed;
		squareNeighborRadius = neighborRadius * neighborRadius;
		SquareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
		SquareAttractionRadius = squareNeighborRadius * attractionRadiusMultiplier * attractionRadiusMultiplier;
	}

	// Update is called once per frame
	public void Tick(Entity entity)
	{
		var context = GetNearbyObjects(entity);

		if (Time.time < entity.AttackTimestamp)
		{
			return;
		}

		Vector2 move = behavior.CalculateMove(entity, context, this);
		move *= driveFactor;
		if (move.sqrMagnitude > squareMaxSpeed)
		{
			move = move.normalized * maxSpeed;
		}
		// entity.Component.FlockAgent.Move(move);
		entity.Component.transform.position += (Vector3)move * Time.deltaTime;
		entity.FlockDirection = move;
	}

	List<Transform> GetNearbyObjects(Entity entity)
	{
		List<Transform> context = new List<Transform>();
		var hits = Physics2D.CircleCastAll(entity.Component.transform.position, neighborRadius, Vector3.zero, 0, LayerMask.GetMask("Entity", "Obstacle"));
		foreach (var hit in hits)
		{
			if (hit.collider != entity.Component.FlockCollider)
			{
				context.Add(hit.collider.transform);
			}
		}
		return context;
	}

}
