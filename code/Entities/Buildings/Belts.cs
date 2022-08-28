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
	private Vector3 NextPointLocalL = new Vector3 ( 32, -12f, 2 );
	private Vector3 NextPointLocalR = new Vector3 ( 32, 12f, 2 );

	private Vector3 ThcknsOffset = new Vector3( 0, 0, 6 );

	private float StartDistFromWP = 64f;

	public Vector3 NextPointL;
	public Vector3 NextPointR;

	public List<Item> TrackL = new List<Item>();
	public List<Item> TrackR = new List<Item>();

	private float ItmSpacing = 8f;

	// Itemtype, distance to next item/stoppoint
	public List<(string ItemCls, float NextItmD)> items = new List<(string ItemCls, float NextItmD)>();
	

	public float Speed = 0.3f; // units/tick each item moves at

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

		NextPointL = Position + NextPointLocalL.x * Rotation.Forward + NextPointLocalL.y * Rotation.Right + ThcknsOffset;
		NextPointR = Position + NextPointLocalR.x * Rotation.Forward + NextPointLocalR.y * Rotation.Right + ThcknsOffset;

		Log.Info( "after "+ NextPointL + " " + NextPointR );

		DebugOverlay.Text( "NextPointL", NextPointL, 200, 200 );
		DebugOverlay.Text( "NextPointR", NextPointR, 200, 200 );


	}

	// TODO: make it so the item's type is copied... also add item types
	public void SendItem( Item item, TransportBelt output, int track )
	{
		Log.Info( "Belt " + this + " sent: " + item + " to: " + NextBelt );

		if (track == 0)
		{
			TrackL.Remove( item );
		} 
		else if (track == 1)
		{
			TrackR.Remove( item );
		} 
		else
		{
			Log.Error( item + " didnt get removed from anything??" );
		}

		output.RecieveItem( item, NextBelt, track );

		DebugOverlay.Text( "Sent an item", Position, 1, 100 );
	}

	// Only call this if the position for the item is already set and wont be set again
	public void RecieveItem( Item item, TransportBelt input, int track )
	{
		Log.Info( "Belt " + this + " recieved: " + item);

		if (input == null)
		{
			// item is probably coming from item tool or whatever
			TrackR.Add( item );
			item.Position = NextPointR - 64f * Rotation.Forward;
			item.DistFromWP = StartDistFromWP;
			Log.Info( "finished recieving item " + item + " from itemtool probably");

			return;
		}

		// Input belt's Yaw, and this belt's Y
		float inY = input.Rotation.Yaw();
		float thisY = Rotation.Yaw();

		if ( inY == thisY -90 ) 
		{
			// if it's coming from the left, meaning it's turned 90 deg clockwise
			TrackL.Add( item );
			item.Position = NextPointL - 64f * Rotation.Forward;
			item.DistFromWP = StartDistFromWP/2;
			Log.Info( "added " + item + " to ItemsL" );
		}
		else if ( inY == thisY +90 ) 
		{
			// it's coming from the right
			TrackR.Add( item );
			item.Position = NextPointR - 64f * Rotation.Forward;
			item.DistFromWP = StartDistFromWP/2;
			Log.Info( "added " + item + " to ItemsR" );
		}
		else if ( inY == thisY ) 
		{
			// it's coming from behind
			if (track == 0)
			{
				TrackL.Add( item );
				item.Position = NextPointL - 64f * Rotation.Forward;
			}
			else if (track == 1)
			{
				TrackR.Add( item );
				item.Position = NextPointR - 64f * Rotation.Forward;
			}
			item.DistFromWP = StartDistFromWP;
			Log.Info( "added " + item + " to ItemsR" );
		}
		else // idk
		{
			Log.Warning( "not sure where item is coming from: " + this );
			Log.Warning( "adding item to ItemsR because not sure where it should go" );
			TrackR.Add( item );
			item.Position = NextPointR - 64f * Rotation.Forward;
			item.DistFromWP = StartDistFromWP;
		}


		Log.Info( "finished recieving item "+ item);
		//DebugOverlay.Text( "Recieved an item", Position, 2, 100 );

	}

	public TransportBelt CheckForBelt( string Dir )
	{
		Vector3 offset = new();
		if ( Dir == "forward" )
		{
			offset = Rotation.Forward * 64;
		}
		else if ( Dir == "left" )// Probably dont need all the directions, just forward and MAYBE backward
		{
			offset = Rotation.Left * 64;
		}
		else if ( Dir == "right" )
		{
			offset = Rotation.Right * 64;
		}
		else if ( Dir == "backward" )
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

	public void MoveTracks()
	{

	}

	public void TickBelt()
	{
		// do nothing if there's no items
		if (TrackL.Count == 0 && TrackR.Count == 0)
			return;

		Log.Info( "ticking belt inside belts.cs" );
		// For every item on this belt, subtract speed from distance and then set position accordingly

		// Right track loop
		for ( int i = 0; i < TrackR.Count; i++ )
		{
			Log.Info( "Ticking item from tickbelt loop" );
			Item item = TrackR[i];
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
				SendItem( item, NextBelt, 0 );
				Log.Info( "sent item" );

			} else if ( NextBelt == null ) // This if statement is redundant but i'm leaving it for now so it doesnt cause issues
			{
				Log.Info( "item is at the end of the belt, doing nothing" );
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
				Log.Info("distfromWP: " + item.DistFromWP);
			}
		}

		// Left track loop
		for ( int i = 0; i < TrackL.Count; i++ )
		{
			Log.Info( "Ticking item from tickbelt loop" );
			Item item = TrackL[i];
			Log.Info( "Item item = ItemsL[i];" );
			if ( item.DistFromWP > 0 ) // Item is NOT at the end of the belt, move it
			{
				Log.Info( "distance from WP is more than 0" );
				item.DistFromWP -= Speed;
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
				Log.Info( "distfromWP: " + item.DistFromWP );

			}
			else if ( NextBelt != null ) // If item IS at the end of the belt, and NextBelt exists
			{
				//Log.Info( "item.DistFromWP < 0 && NextBelt != null" );
				SendItem( item, NextBelt, 1 );
				Log.Info( "sent item" );

			}
			else if ( NextBelt == null ) // This if statement is redundant but i'm leaving it for now so it doesnt cause issues
			{
				Log.Info( "item is at the end of the belt, doing nothing" );
				item.Position = NextPointR - item.DistFromWP * Rotation.Forward;
				Log.Info( "distfromWP: " + item.DistFromWP );
			}
		}
	}
}
