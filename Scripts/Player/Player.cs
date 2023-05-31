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

	Vector3 lookRotation = new Vector3();

	public override void _Ready()
	{
		neck = GetNode<Node3D>("Neck");
		camera = GetNode<Camera3D>("Neck/Camera3D"); 
	
		GameManager.instance.player = this;
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
			// neck.RotateY(-mouseMotion.Relative.X * 0.01f);
			// camera.RotateX(-mouseMotion.Relative.Y * 0.01f);

			lookRotation.Y -= (mouseMotion.Relative.X * 0.2f);
			lookRotation.X -= (mouseMotion.Relative.Y * 0.2f);


			// Vector3 rotation = new Vector3();

			// double downAngle = (Math.PI / 180) * -60;
			// double upAngle = (Math.PI / 180) * 60;

			double downAngle = -60;
			double upAngle = 60;

			lookRotation.X = (float)Math.Clamp(lookRotation.X, downAngle, upAngle);
		}
	}

	private void SetGravityDirection()
	{
		// GD.Print(GameManager.instance.currentPlayerCelestialBody);
		gravityDirection = (GameManager.instance.currentPlayerCelestialBody.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		GD.Print(gravityDirection.ToString());
	}

	public override void _PhysicsProcess(double delta)
	{
		//Camera
		Vector3 neckRotationDegrees = neck.RotationDegrees;
		neckRotationDegrees.X = -lookRotation.X;
		neck.RotationDegrees = neckRotationDegrees;
		
		Vector3 rotationDegrees = RotationDegrees;
		rotationDegrees.Y = lookRotation.Y;
		RotationDegrees = rotationDegrees;

		SetGravityDirection();

		Transform3D transform = Transform;
		transform.Basis.Y = -gravityDirection;
		Transform = transform;

		// camera.Transform.Basis.Y = -gravityDirection;

		Transform3D cameraTransform = camera.Transform;
		cameraTransform.Basis.Y = -gravityDirection;
		camera.Transform = cameraTransform;


		GD.Print(Transform.Basis.Y);

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor()) {
			velocity.Y -= GameManager.instance.currentPlayerCelestialBody.gravityForce * (float)delta;
		}



		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");

		float inputDirX = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		float inputDirZ = Input.GetActionStrength("forward") - Input.GetActionStrength("back");

		Vector3 direction = (neck.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
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

		Velocity = velocity;
		MoveAndSlide();
	}
}
