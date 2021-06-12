using System.Collections.Generic;
using Game.Core;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behavior/Stay In Radius")]
public class StayInRadiusBehavior : FlockBehavior
{
	public Vector2 center;
	public float radius = 15f;

	public override Vector2 CalculateMove(Entity entity, List<Transform> context, Flock flock)
	{
		Vector2 centerOffset = center - (Vector2)entity.Component.transform.position;
		float t = centerOffset.magnitude / radius;
		if (t < 0.9f)
		{
			return Vector2.zero;
		}

		return centerOffset * t * t;
	}
}
