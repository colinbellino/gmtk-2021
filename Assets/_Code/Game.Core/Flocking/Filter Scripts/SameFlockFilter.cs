using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
	public override List<Transform> Filter(Entity entity, List<Transform> original)
	{
		List<Transform> filtered = new List<Transform>();
		foreach (Transform item in original)
		{
			var itemAgent = item.GetComponentInParent<EntityComponent>();
			if (itemAgent != null && itemAgent.Entity.Flock == entity.Flock)
			{
				filtered.Add(item);
			}
		}
		return filtered;
	}
}
