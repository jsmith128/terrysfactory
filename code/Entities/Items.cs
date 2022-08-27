using Sandbox;
using System.Collections.Generic;
using System;

partial class Item : ModelEntity
{
	//public virtual int ID => 0;
	//public List<Entity> ItemIDs => new List<Entity>();
	public virtual Model WorldModel => Model.Load( "models/items/testball.vmdl" );

	// Where we want to end up
	//public Vector3 CurrentWP = new Vector3();
	// The most units we can move each tick
	//public int Speed = 1;
	public float DistFromWP;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = false;
		UsePhysicsCollision = false;
		EnableTouch = false;
		//EnableAllCollisions = false;

		//EnableTouch = true;
		//EnableAllCollisions = true;
		//EnableTouchPersists = true;

		//SetupPhysicsFromModel( PhysicsMotionType.Static, true );
		//PhysicsBody.BodyType = PhysicsBodyType.Static;
		//EnableShadowCasting = false;

		VarStore.Items.Add( this );
		//Tags.Add( "trigger" );
	}
}
