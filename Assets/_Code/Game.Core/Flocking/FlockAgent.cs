using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
	[HideInInspector] public Flock AgentFlock;
	[HideInInspector] public Vector2 Direction;

	Collider2D agentCollider;
	public Collider2D AgentCollider { get { return agentCollider; } }


	// Start is called before the first frame update
	void Start()
	{
		agentCollider = GetComponent<Collider2D>();
	}

	public void Move(Vector2 velocity)
	{
		// transform.up = velocity;
		transform.position += (Vector3)velocity * Time.deltaTime;
		Direction = velocity;
	}
}
