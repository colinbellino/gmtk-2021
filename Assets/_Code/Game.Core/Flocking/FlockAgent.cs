using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
	[HideInInspector] public Flock AgentFlock;
	[HideInInspector] public Vector2 Direction;

	public Collider2D AgentCollider;

	public void Move(Vector2 velocity)
	{
		// transform.up = velocity;
		transform.position += (Vector3)velocity * Time.deltaTime;
		Direction = velocity;
	}
}
