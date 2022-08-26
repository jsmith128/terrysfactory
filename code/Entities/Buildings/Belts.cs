using Sandbox;
using System.Collections.Generic;

partial class TransportBelt : Building
{
	//public virtual int ID => 0;
	//public List<Entity> ItemIDs => new List<Entity>();
	public override Model WorldModel => Model.Load( "models/buildings/transportbelt.vmdl" );
	//new public float BreakSpeed = 0.2f;

	// The positions relative to center of tile of where items need to end up.
	// When they reach this point, give them the waypoint of the next belt on the correct side.
	// These should be on the very edge of the tile.
	// Convert this to world coordinates before giving it to item.
	private Vector3 NextPointLocalL = new Vector3 ( 1, 0.25f, 0 );
	private Vector3 NextPointLocalR = new Vector3 ( 1, 0.75f, 0 );

	public Vector3 NextPointL;
	public Vector3 NextPointR;


	public Vector3 OffPointL;
	public Vector3 OffPointR;


	// Float is how far from this belt's waypoint it is

	public List<Item> ItemsL = new List<Item>();
	public List<Item> ItemsR = new List<Item>();

	// Itemtype, distance to next item/stoppoint
	public List<(string ItemCls, float NextItmD)> items = new List<(string ItemCls, float NextItmD)>();


	public float Speed = 0.01f; // units/tick each item moves at
	new public bool Rotates = true;
	//public Vector3 GridOffset = new Vector3( 0, 0, 0 );
	//public bool IsConveyor = true;
	//new public bool Rotates = true;
	public TransportBelt NextBelt;

	public override void Spawn()
	{
		base.Spawn();

		EnableTouch = true;
		EnableAllCollisions = true;

		// TODO: need to do something different depending on rotation
		NextPointL = Position + NextPointLocalL;
		NextPointR = Position + NextPointLocalR;
		OffPointL = Position + NextPointL + Rotation.Forward * 0.25f;
		OffPointR = Position + NextPointR + Rotation.Forward * 0.25f;

		base.OnActive();
	}

	// delete item, then make a new one
	// TODO: make it so the item's type is copied... also add item types
	public void SendItem( Item item )
	{
		Log.Info( "Belt " + this + " sent: " + item + " to: " + NextBelt );

		ItemsR.Remove( item );
		//item.Delete();
		//var newItem = new Item();
		//newItem.Position = OffPointR;
		NextBelt.RecieveItem( item );
		item.Position = NextBelt.Position + Rotation.Right * 0.5f;
	}

	// Only call this if the position for the item is already set and wont be set again
	public void RecieveItem( Item item )
	{
		Log.Info( "Belt " + this + "recieved: " + item);
		//var newItem = new Item();
		item.Position = Position + Rotation.Right * 0.75f;

		item.DistFromWP = 0.5f;
		Log.Info( "added " +item+ " to ItemsR" );
		ItemsR.Add( item);
		Log.Info( "after last message message" );
	}

	/*public (Vector3, Vector3) GetVerts()
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
	}*/


	// PROBABLY DONT NEED THIS HERE
	public TransportBelt CheckForNearbyBelt( Vector3 point )
	{
		BBox beltbox = Help.BBoxFromRadHeightCenter( 16, 12, point );

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

	public TransportBelt GetNextBelt( )
	{
		Vector3 offset = Rotation.Forward * 64;
		BBox beltbox = Help.BBoxFromRadHeightCenter( 16, 6, Position + offset );

		foreach ( var ent in Entity.FindInBox( beltbox ) )
		{
			if ( ent.ClassName == "TransportBelt" )
			{
				Log.Info( "found a belt" );
				return ent as TransportBelt;
			}
		}
		Log.Info( "Tried to find next belt but couldnt" );
		return null;
	}

	public void TickBelt()
	{
		// For every item on this belt, subtract speed from distance and then set position accordingly
		for ( int i = 0; i < ItemsR.Count; )
		{
			Item item = ItemsR[i];
			
			if ( item.DistFromWP > 0 )
			{
				item.DistFromWP -= Speed;
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
			} else if ( NextBelt != null )
			{
				SendItem( item );

			}
		}
	}

	/*public void TickBelt()
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
	}*/
}
