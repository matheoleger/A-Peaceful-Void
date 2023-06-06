using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Vector3 gravityDirection;

	private Node3D neck;
	private Camera3D camera;

	private StaticBody3D planet;

	public override void _Ready()
	{
		neck = GetNode<Node3D>("Neck");
		camera = GetNode<Camera3D>("Neck/Camera3D"); 
		planet = GetNode<StaticBody3D>("../StaticBody3D");

		GD.Print(planet);
	
		// GameManager.instance.player = this;
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if(inputEvent is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		} 
		else if (inputEvent.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if(Input.MouseMode == Input.MouseModeEnum.Captured && inputEvent is InputEventMouseMotion)
		{
			InputEventMouseMotion mouseMotion = (InputEventMouseMotion)inputEvent;
			neck.RotateY(-mouseMotion.Relative.X * 0.01f);
			camera.RotateX(mouseMotion.Relative.Y * 0.01f);

			// lookRotation.Y -= (mouseMotion.Relative.X * 0.2f);
			// lookRotation.X -= (mouseMotion.Relative.Y * 0.2f);


			// Vector3 rotation = new Vector3();

			double downAngle = (Math.PI / 180) * -60;
			double upAngle = (Math.PI / 180) * 60;

			// double downAngle = -60;
			// double upAngle = 60;

			Vector3 rotation = camera.Rotation;
			rotation.X = (float)Math.Clamp(camera.Rotation.X, downAngle, upAngle);
			camera.Rotation = rotation;
		}
	}

	private void SetGravityDirection()
	{
		// gravityDirection = (Vector3.Zero - this.GlobalTransform.Origin).Normalized();
		gravityDirection = (GameManager.instance.currentPlayerCelestialBody.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		// gravityDirection = (planet.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		// GD.Print((GameManager.instance.currentPlayerCelestialBody.GlobalTransform.Origin - GlobalTransform.Origin));
	}

	public override void _PhysicsProcess(double delta)
	{
		SetGravityDirection();

		// Rotate player according to gravity direction
		Transform3D transform = Transform;
		transform.Basis.Y = -gravityDirection;
		transform.Basis.X = -Transform.Basis.Z.Cross(-gravityDirection); // ?
		transform.Basis = transform.Basis.Orthonormalized(); // ?
		Transform = transform;

		// Redefine floorMaxAngle for planets
		// this.FloorMaxAngle = 180;

		// // Add the gravity.
		// if (!IsOnFloor()) {
		// 	velocity.Y -= GameManager.instance.currentPlayerCelestialBody.gravityForce * (float)delta;
		// 	// GD.Print("IM NOT ON THE FLOOR, OMG");
		// }

		// Vector3 gravityForceLocal = gravityDirection*GameManager.instance.currentPlayerCelestialBody.gravityForce*GlobalTransform.Basis.Transposed();
		// Vector3 gravityForceLocal = gravityDirection*GameManager.instance.currentPlayerCelestialBody.gravityForce*GlobalTransform.Basis;

		Vector3 velocity = Velocity;

		// velocity += gravityDirection*GameManager.instance.currentPlayerCelestialBody.gravityForce * (float)delta;
		// velocity += gravityForceLocal * (float)delta;
		velocity += gravityDirection * GameManager.instance.currentPlayerCelestialBody.gravityForce * (float)delta;
		// velocity.Y -= 10.0f * (float)delta;

		// GD.Print(velocity.Y);

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		// Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");

		float inputDirX = Input.GetActionStrength("left") - Input.GetActionStrength("right");
		float inputDirZ = Input.GetActionStrength("forward") - Input.GetActionStrength("back");

		Vector3 direction = (neck.Transform.Basis * new Vector3(inputDirX, 0, inputDirZ)).Normalized();
		direction *= GlobalTransform.Basis;
		
		// Vector3 direction = (new Vector3(inputDirX, 0, inputDirZ)).Normalized();
		
		
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		UpDirection = -gravityDirection;

		Velocity = velocity;

		GD.Print(Velocity);
		MoveAndSlide();
		// MoveAndCollide(Velocity);
	}
}
