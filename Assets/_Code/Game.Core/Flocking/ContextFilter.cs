using System.Collections.Generic;
using Game.Core;
using UnityEngine;

public abstract class ContextFilter : ScriptableObject
{
	public abstract List<Transform> Filter(Entity agent, List<Transform> original);
}
