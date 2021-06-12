using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Different Alliance")]
public class DifferentAllianceFilter : ContextFilter
{
	public override List<Transform> Filter(Entity entity, List<Transform> original)
	{
		List<Transform> filtered = new List<Transform>();
		foreach (Transform item in original)
		{
			var itemEntity = item.GetComponentInParent<EntityComponent>();
			if (itemEntity != null && itemEntity.Entity.Alliance != entity.Alliance)
			{
				filtered.Add(item);
			}
		}
		return filtered;
	}
}
