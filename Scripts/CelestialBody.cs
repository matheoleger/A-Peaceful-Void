using Godot;
using System;

public partial class CelestialBody : RigidBody3D
{

	float G = 6.6743f * Mathf.Pow(10, -2);

	public float gravityForce = 10.0f;

	[Export]
	public Vector3 initialVelocity = Vector3.Zero;
	
	[Export]
	private bool isStar = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		LinearVelocity = initialVelocity;
		GameManager.instance.celestialBodies.Add(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if(!isStar)
		{
			Gravity(delta);
		} 
		else
		{
			LinearVelocity = Vector3.Zero;
		}
	}

	private void Gravity(double delta)
	{
		foreach(CelestialBody otherBody in GameManager.instance.celestialBodies)
		{
			if(otherBody == this) return;

			float otherBodyMass = otherBody.Mass;
			Vector3 direction = otherBody.GlobalTransform.Origin - this.GlobalTransform.Origin;
			float distance = direction.Length();

			float forceMagnitude = G * ((this.Mass * otherBodyMass) / (distance*distance));

			Vector3 force = direction.Normalized() * forceMagnitude;

			ApplyCentralForce(force);
		}
	}

	public void _OnAreaBodyEntered(Node3D body) //Signal from Area
	{
		// GD.Print(body.Name);

		if(body == GameManager.instance.player)
			GameManager.instance.currentPlayerCelestialBody = this;
		
		// GD.Print(GameManager.instance.currentPlayerCelestialBody.Name);
	} 

	public void _OnAreaBodyExited(Node3D body) //Signal from Area
	{
		// GD.Print(body.Name);
		
		if(body == GameManager.instance.player)
			GameManager.instance.currentPlayerCelestialBody = null; // TODO: Find better way to avoid null reference
	} 
}
