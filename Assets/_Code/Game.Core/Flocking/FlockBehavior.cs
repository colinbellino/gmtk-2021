using System.Collections.Generic;
using Game.Core;
using UnityEngine;

public abstract class FlockBehavior : ScriptableObject
{
	public abstract Vector2 CalculateMove(Entity entity, List<Transform> context, Flock flock);
}
