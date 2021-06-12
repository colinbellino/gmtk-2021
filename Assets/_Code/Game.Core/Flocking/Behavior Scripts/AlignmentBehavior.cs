using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Alignment")]
public class AlignmentBehavior : FilteredFlockBehavior
{
	public override Vector2 CalculateMove(Entity entity, List<Transform> context, Flock flock)
	{
		//if no neighbors, maintain current alignment
		if (context.Count == 0)
			return entity.Component.transform.up;

		//add all points together and average
		Vector2 alignmentMove = Vector2.zero;
		List<Transform> filteredContext = (filter == null) ? context : filter.Filter(entity, context);
		foreach (Transform item in filteredContext)
		{
			alignmentMove += (Vector2)item.transform.up;
		}
		alignmentMove /= context.Count;

		return alignmentMove;
	}
}
