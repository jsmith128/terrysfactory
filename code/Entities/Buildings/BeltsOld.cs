/*using Sandbox;

partial class TransportBelt : Building
{
	//public virtual int ID => 0;
	public virtual int BeltSpeed => 1;
	//public List<Entity> ItemIDs => new List<Entity>();
	public override Model WorldModel => Model.Load( "models/buildings/transportbelt.vmdl" );

	public int BEND_NONE = 0;
	public int BEND_LEFT = 1;
	public int BEND_RIGHT = 2;
	public int Bend;

	// The positions relative to center of tile of where items need to end up.
	// When they reach this point, give them the waypoint of the next belt on the correct side.
	// These should be on the very edge of the tile.
	// Convert this to world coordinates before giving it to item.
	public Vector3 LLocalWaypoint = new Vector3 ( 0, 0, 0 );
	public Vector3 RLocalWaypoint = new Vector3 ( 0, 0, 0 );

	new public float BreakSpeed = 0.2f;
	public float Speed = 1f; // Time in seconds it takes to cross the entire conveyor
	//public Vector3 GridOffset = new Vector3( 0, 0, 0 );
	//public bool IsConveyor = true;
	//new public bool Rotates = true;
	public BBox itembox;
	public TransportBelt NextBelt;

	public override void Spawn()
	{
		base.Spawn();

		SetBend( BEND_NONE );

		EnableTouch = true;
		EnableAllCollisions = true;

		base.OnActive();

	}

	public void PostSpawnUpdate()
	{
		(Vector3 Mins, Vector3 Maxs) = GetVerts();
		itembox = new BBox( mins: Mins, maxs: Maxs );

		*//*var newItem = TypeLibrary.Create<ModelEntity>( "testbox" );
		newItem.Position = Mins;
		var newItem2 = TypeLibrary.Create<ModelEntity>( "testbox" );
		newItem2.Position = Maxs;*//*
	}

	public void SetBend( int b )
	{
		//self.Bend = self.Bend == BEND_NONE and b or BEND_NONE //??????
		Bend = b;
	}

	public void PlaceItem( string ClassName )
	{
		var newItem = TypeLibrary.Create<ModelEntity>( ClassName );
		newItem.Position = Position + Rotation.Left * 0.5f;
	}

	public (Vector3, Vector3) GetVerts()
	{
		// This probably works better right here
		//Help.BBoxFromRadHeightCenter( 32, 8.2f, new Vector3( Position.x, Position.y, Position.z + 10.9f ) );
		Log.Info( Position );

		Vector3 mins = new Vector3();
		Vector3 maxs = new Vector3();

		Vector3 minsOffset = new Vector3(-32, -32, 6.8f);
		Vector3 maxsOffset = new Vector3(32, 32, 15f);

		mins += minsOffset + Position;
		maxs += maxsOffset + Position;

		Log.Info( maxs + " " + mins );
		return (mins, maxs);
	}

	public TransportBelt CheckForBelt( string Dir )
	{
		Vector3 offset = new();
		if ( Dir == "forward" )
		{
			offset = Rotation.Forward * 64;
		}
		else if ( Dir == "left" )
		{
			offset = Rotation.Left * 64;
		}
		else if ( Dir == "right" )
		{
			offset = Rotation.Right * 64;
		}
		else if ( Dir == "back" )
		{
			offset = Rotation.Backward * 64;
		}

		BBox beltbox = Help.BBoxFromRadHeightCenter( 16, 6, Position + offset );

		foreach ( var ent in Entity.FindInBox( beltbox ) )
		{
			if ( ent.ClassName == "TransportBelt" )
			{
				Log.Info( "found a belt" );
				return ent as TransportBelt;
			}
		}
		return null;
	}

	public void TickBelt()
	{
		if ( IsClient )
			return;

		foreach ( var ent in Entity.FindInBox( itembox ) )
		{
			//Log.Info( "found entity" );

			if ( ent.ClassName == "Item" )
			{
				Log.Info( "found an item" );
				Item item = ent as Item;

				item.Position += Rotation.Forward * Speed;
			}
		}
	}
}
*/
