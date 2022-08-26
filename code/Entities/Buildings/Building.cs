using Sandbox;
using System.Collections.Generic;
using System;

partial class Building : ModelEntity
{
	//public virtual int ID => 0;
	public virtual Model WorldModel => Model.Load( "models/buildings/transportbelt.vmdl" );

	//public bool IsFactoryPart = true;
	
	//Vector3 GridOffset = new Vector3(0,0,0);
	//Rotation AngOffset = Rotation.Parse("0,0,0,0");
	public virtual int Width { get; set; } = 1;
	public virtual int Height { get; set; } = 1;
	//public float BreakSpeed = 0.65f;
	public bool Rotates = false;

	public override void Spawn()
	{
		base.Spawn();

		Scale = Width;

		Model = WorldModel;

		PhysicsEnabled = false;
		UsePhysicsCollision = true;

		//CollisionLayer = 
		//SetInteractsAs( CollisionLayer.Debris );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
	}

	public void SetGridPos(int x, int y)
	{
		//self: SetGridX( x )
		//self: SetGridY( y )
	}
	public (int, int) GetGridPos( int x, int y )
	{
		return (0,0);
		//return self:GetGridX(), self:GetGridY()
	}


	/*	public virtual void SnapToGrid(Vector3 pos)
		{
			Position = RoundV3ToGrid( pos );
		}

		public static float RoundToGrid( float value )
		{
			return MathF.Round( value / terrysfactory.gridSize ) * terrysfactory.gridSize;
		}

		public static Vector3 RoundV3ToGrid( Vector3 v )
		{
			return new Vector3( RoundToGrid( v.x ), RoundToGrid( v.y ), v.z );
		}*/
}
