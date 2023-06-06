using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	#region Singleton instance 

		public static GameManager instance;

	#endregion

	public PlayerRigidBody player;

	public List<CelestialBody> celestialBodies = new List<CelestialBody>();

	public CelestialBody currentPlayerCelestialBody;
	public const float GravitationalConstant = 0.0000000000674f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
