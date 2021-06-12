using System.Collections;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

public class Flock : MonoBehaviour
{
	public FlockAgent agentPrefab;
	// List<FlockAgent> agents = new List<FlockAgent>();
	public FlockBehavior behavior;
	public Transform FollowTarget;

	[Range(10, 500)]
	public int startingCount = 250;
	// const float AgentDensity = 0.08f;

	[Range(1f, 100f)]
	public float driveFactor = 10f;
	[Range(1f, 100f)]
	public float maxSpeed = 5f;
	[Range(1f, 10f)]
	public float neighborRadius = 1.5f;
	[Range(0f, 1f)]
	public float avoidanceRadiusMultiplier = 0.5f;

	float squareMaxSpeed;
	float squareNeighborRadius;
	float squareAvoidanceRadius;
	public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

	// Start is called before the first frame update
	void Start()
	{
		squareMaxSpeed = maxSpeed * maxSpeed;
		squareNeighborRadius = neighborRadius * neighborRadius;
		squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
	}

	// Update is called once per frame
	public void Tick(List<Entity> entities)
	{
		foreach (var entity in entities)
		{
			List<Transform> context = GetNearbyObjects(entity.Component.FlockAgent);

			Vector2 move = behavior.CalculateMove(entity.Component.FlockAgent, context, this);
			move *= driveFactor;
			if (move.sqrMagnitude > squareMaxSpeed)
			{
				move = move.normalized * maxSpeed;
			}
			entity.Component.FlockAgent.Move(move);
		}
	}

	List<Transform> GetNearbyObjects(FlockAgent agent)
	{
		List<Transform> context = new List<Transform>();
		Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius, LayerMask.GetMask("Entity", "Obstacle"));
		foreach (Collider2D c in contextColliders)
		{
			if (c != agent.AgentCollider)
			{
				context.Add(c.transform);
			}
		}
		return context;
	}

}
