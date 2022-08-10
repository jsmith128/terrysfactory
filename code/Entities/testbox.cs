using Sandbox;

partial class testbox : ModelEntity
{
	public virtual Model WorldModel => Model.Load( "models/testbox.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = false;
		UsePhysicsCollision = false;
	}
}
