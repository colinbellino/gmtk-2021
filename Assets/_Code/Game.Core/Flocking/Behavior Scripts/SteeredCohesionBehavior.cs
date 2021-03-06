using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Steered Cohesion")]
public class SteeredCohesionBehavior : FilteredFlockBehavior
{

	Vector2 currentVelocity;
	public float agentSmoothTime = 0.5f;

	public override Vector2 CalculateMove(Entity entity, List<Transform> context, Flock flock)
	{
		//if no neighbors, return no adjustment
		if (context.Count == 0)
			return Vector2.zero;

		//add all points together and average
		Vector2 cohesionMove = Vector2.zero;
		List<Transform> filteredContext = (filter == null) ? context : filter.Filter(entity, context);
		foreach (Transform item in filteredContext)
		{
			cohesionMove += (Vector2)item.position;
		}
		cohesionMove /= context.Count;

		//create offset from agent position
		cohesionMove -= (Vector2)entity.Component.transform.position;
		cohesionMove = Vector2.SmoothDamp(entity.FlockDirection, cohesionMove, ref currentVelocity, agentSmoothTime);
		return cohesionMove;
	}
}
