/*using Sandbox;
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
	private Vector3 NextPointLocalL = new Vector3 ( 32, -12f, 2 );
	private Vector3 NextPointLocalR = new Vector3 ( 32, 12f, 2 );

	private Vector3 ThcknsOffset = new Vector3( 0, 0, 6 );

	private float StartDistFromWP = 64f;

	public Vector3 NextPointL;
	public Vector3 NextPointR;


	public Vector3 OffPointL;
	public Vector3 OffPointR;


	// Float is how far from this belt's waypoint it is

	public List<Item> ItemsL = new List<Item>();
	public List<Item> ItemsR = new List<Item>();

	// Itemtype, distance to next item/stoppoint
	public List<(string ItemCls, float NextItmD)> items = new List<(string ItemCls, float NextItmD)>();


	public float Speed = 0.4f; // units/tick each item moves at
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

		

		base.OnActive();
	}

	public void PostSpawnUpdate()
	{
		// FIX THIS
		// MAKE IT ROTATE WITH THE BELTS
		NextPointL = Position + NextPointLocalL.x * Rotation.Forward + NextPointLocalL.y * Rotation.Right + ThcknsOffset;
		NextPointR = Position + NextPointLocalR.x * Rotation.Forward + NextPointLocalR.y * Rotation.Right + ThcknsOffset;
		//NextPointL = Position + NextPointLocalL;// * Rotation.Forward;
		//NextPointR = Position + NextPointLocalR;// * Rotation.Forward;

		//OffPointL = Position + NextPointL + Rotation.Forward * 0.25f;
		//OffPointR = Position + NextPointR + Rotation.Forward * 0.25f;

		Log.Info( "after "+ NextPointL + " " + NextPointR );
		//Item it = new Item();
		//it.Position = NextPointL;
		//it = new Item();
		//it.Position = NextPointR;
		//DebugOverlay.Axis( NextPointL, Rotation, duration: 1000 );
		//DebugOverlay.Axis( NextPointR, Rotation, duration: 1000 );
		DebugOverlay.Text( "NextPointL", NextPointL, 200, 200 );
		DebugOverlay.Text( "NextPointR", NextPointR, 200, 200 );


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
		DebugOverlay.Text( "Sent an item", Position, 1, 100 );
	}

	// Only call this if the position for the item is already set and wont be set again
	public void RecieveItem( Item item )
	{
		Log.Info( "Belt " + this + " recieved: " + item);

		item.Position = NextPointR - 64f * Rotation.Forward;
		item.DistFromWP = StartDistFromWP;
		Log.Info( "added " +item+ " to ItemsR" );
		ItemsR.Add( item);
		Log.Info( "after last message message" );
		DebugOverlay.Text( "Recieved an item", Position, 1, 100 );

	}

	*//*public (Vector3, Vector3) GetVerts()
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
	}*//*


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
		if (ItemsL.Count == 0 && ItemsR.Count == 0)
			return;

		Log.Info( "ticking belt inside belts.cs" );
		// For every item on this belt, subtract speed from distance and then set position accordingly
		Log.Info( "ItemsR.Count == " + ItemsR.Count );
		for ( int i = 0; i < ItemsR.Count; i++ )
		{
			Log.Info( "Ticking item from tickbelt loop" );
			Item item = ItemsR[i];
			Log.Info( "Item item = ItemsR[i];" );
			if ( item.DistFromWP > 0 ) // Item is NOT at the end of the belt, move it
			{
				Log.Info( "distance from WP is more than 0" );
				item.DistFromWP -= Speed;
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
				Log.Info( "distfromWP: " + item.DistFromWP );

			} else if ( NextBelt != null ) // If item IS at the end of the belt, and NextBelt exists
			{
				Log.Info( "item.DistFromWP < 0 && NextBelt != null" );
				SendItem( item );
				Log.Info( "sent item" );

			} else if ( NextBelt == null ) // This if statement is redundant but i'm leaving it for now so it doesnt cause issues
			{
				Log.Info( "item is at the end of the belt, doing nothing" );
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
				Log.Info("distfromWP: " + item.DistFromWP);
			}
		}
	}
}
*/
