using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Follow Target")]
public class FollowTargetBehavior : FilteredFlockBehavior
{
	public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
	{
		if (flock.FollowTarget == null)
		{
			return Vector2.zero;
		}

		//if no neighbors, maintain current alignment
		if (context.Count == 0)
			return (flock.FollowTarget.transform.position - agent.transform.position).normalized;

		//add all points together and average
		Vector2 alignmentMove = Vector2.zero;
		List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
		foreach (Transform item in filteredContext)
		{
			alignmentMove += (Vector2)(flock.FollowTarget.transform.position - item.transform.position).normalized;
		}
		alignmentMove /= context.Count;

		return alignmentMove;
	}
}
