using Sandbox;
using System.Collections.Generic;
using System;

partial class Item : ModelEntity
{
	//public virtual int ID => 0;
	//public List<Entity> ItemIDs => new List<Entity>();
	public virtual Model WorldModel => Model.Load( "models/items/testball.vmdl" );

	// Where we want to end up
	public Vector3 CurrentWP = new Vector3();
	// The most units we can move each tick
	public int Speed = 1;
	public float DistFromWP;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;

		PhysicsEnabled = false;
		UsePhysicsCollision = false;

		//EnableTouch = true;
		//EnableAllCollisions = true;
		//EnableTouchPersists = true;

		//SetupPhysicsFromModel( PhysicsMotionType.Static, true );
		//PhysicsBody.BodyType = PhysicsBodyType.Static;
		//EnableShadowCasting = false;

		VarStore.Items.Add( this );
		//Tags.Add( "trigger" );
	}

	public virtual void Tick()
	{
		if ( IsClient )
			return;

		Position += (Position - CurrentWP).Clamp(-Speed, Speed);
	}
	
	/*public virtual void Tick()
	{
		*//*if ( IsClient )
			return;


		var tr = Trace.Ray( Position + Vector3.Up * 2, Position + Vector3.Down * 20 )
			.WithAllTags( "solid" )
			.Ignore( this )
			.Run();

		Log.Trace( "Hit " + tr.Entity );

		if ( tr.Entity.ClassName != "TransportBelt" )
			return;

		TransportBelt hit = tr.Entity as TransportBelt;
		Position = Position + hit.Rotation.Forward * hit.BeltSpeed;*//*
	}*/

	/*public override void Touch( Entity other )
	{
		if ( IsClient )
			return;
		Log.Trace( "touched "+other );
		if ( other is not TransportBelt )
		{
			return;
		}
		Log.Trace( "BELT " + other );

		var belt = other as TransportBelt;
		Position = Position + belt.Rotation.Forward * belt.BeltSpeed;
	}*/
}
