using Sandbox;
using System.Collections.Generic;

partial class BeltSystem : Entity
{
	//public List<Entity> ItemIDs => new List<Entity>();


	// The positions relative to center of tile of where items need to end up.
	// When they reach this point, give them the waypoint of the next belt on the correct side.
	// These should be on the very edge of the tile.
	// Convert this to world coordinates before giving it to item.
	public Vector3 LLocalWaypoint = new Vector3 ( 0, 0, 0 );
	public Vector3 RLocalWaypoint = new Vector3 ( 0, 0, 0 );

	// Offset of each belt in the belt system, at the center
	public Dictionary<TransportBelt, float> BeltOffsets = new Dictionary<TransportBelt, float> ();

	public void PlaceItem( TransportBelt belt, string ClassName )
	{
		float offset = GetBeltOffset( belt );

		var newItem = TypeLibrary.Create<ModelEntity>( ClassName );
		newItem.Position = Position + Rotation.Left * 0.5f;
	}


	public float GetBeltOffset(TransportBelt belt)
	{
		return BeltOffsets [belt];
	}


	/*public void TickBelt()
	{
		if ( IsClient )
			return;

		foreach ( var ent in VarStore.AllBelts )
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
