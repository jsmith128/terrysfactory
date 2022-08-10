using Sandbox;

partial class BaseInserter : Building
{
	public virtual int Facing { get; set; } = 0; // 0,1,2,3 > North, East, South, West
	public virtual int StackSize => 1;
	public Entity HeldItem => null;
	public override Model WorldModel => Model.Load( "models/buildings/inserter.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}
}
