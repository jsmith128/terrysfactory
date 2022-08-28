using Sandbox;


namespace Sandbox;

public class FactoryCamera : CameraMode
{

	public virtual float CameraHeight => 400;
	public virtual float CameraAngle => 80;
	public virtual float FOV => 70;
	private Angles ang;
	private Angles tarAng;
	public override void Update()
	{
		if ( Local.Pawn is not AnimatedEntity pawn )
			return;

		var CamOffset = new Vector3( -120, 0, CameraHeight );
		var CamRot = Rotation.FromPitch( CameraAngle );

		float MouseX = Mouse.Position.x.Clamp( 0, Screen.Size.x );
		float MouseY = Mouse.Position.y.Clamp( 0, Screen.Size.y );



		Position = pawn.Position + CamOffset;
		Rotation = CamRot;

		FieldOfView = FOV;

		Viewer = null;
	}

	public override void BuildInput( InputBuilder input )
	{
		var pawn = Local.Pawn;

		if ( pawn == null )
		{
			return;
		}

		Angles angles;

		// always set movement input
		input.InputDirection = input.AnalogMove;

		// handle look input

		var direction = Screen.GetDirection(
			new Vector2( Mouse.Position.x, Mouse.Position.y ),
			FOV,
			Rotation,
			Screen.Size );
		//var HitPosition = LinePlaneIntersectionWithHeight( Position, direction, pawn.EyePosition.z - 20 );

		// since we got our cursor in world space because of the plane intersect above, we need to set it for the crosshair
		//var mouse = HitPosition.ToScreen();
		//Crosshair.UpdateMouse( new Vector2( mouse.x * Screen.Width, mouse.y * Screen.Height ) );

		//trace from camera into mouse direction, essentially gets the world location of the mouse
		var targetTrace = Trace.Ray( Position, Position + (direction * 1000) )
			.UseHitboxes()
			//.EntitiesOnly()
			.Size( 1 )
			.Ignore( pawn )
			.Run();

		angles = ((pawn.EyePosition - (Vector3.Up * 20))).EulerAngles;

		// FIX, THIS NOTHING WORKS
		tarAng = angles;

		ang = Angles.Lerp( ang, tarAng, 24 * Time.Delta );

		input.ViewAngles = ang;

		//base.BuildInput( input );
	}
}
