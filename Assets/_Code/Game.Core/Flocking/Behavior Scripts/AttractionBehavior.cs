using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Attraction")]
public class AttractionBehavior : FilteredFlockBehavior
{
	public override Vector2 CalculateMove(Entity entity, List<Transform> context, Flock flock)
	{
		//if no neighbors, return no adjustment
		if (context.Count == 0)
			return Vector2.zero;

		//add all points together and average
		Vector2 attractionMove = Vector2.zero;
		int nAvoid = 0;
		List<Transform> filteredContext = (filter == null) ? context : filter.Filter(entity, context);
		foreach (Transform item in filteredContext)
		{
			if (Vector2.SqrMagnitude(item.position - entity.Component.transform.position) < flock.SquareAttractionRadius)
			{
				nAvoid++;
				attractionMove -= (Vector2)(entity.Component.transform.position - item.position).normalized;
			}
		}
		if (nAvoid > 0)
			attractionMove /= nAvoid;

		return attractionMove;
	}
}
