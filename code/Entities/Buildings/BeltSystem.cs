using Sandbox;
using System.Collections.Generic;

partial class BeltSystem : Entity
{
	//public List<Entity> ItemIDs => new List<Entity>();


	// Offset of each belt in the belt system, at the center
	public Dictionary<TransportBelt, float> BeltOffsets = new Dictionary<TransportBelt, float> ();
	public Dictionary<string, float> Items = new Dictionary<string, float> ();

	public void PlaceItem( TransportBelt belt, string ClassName )
	{
		float offset = GetBeltOffset( belt );
		// This is all bad please fix this rework it
		// make it work please
		// Needs something to do with offsets, and try not to actually make entities for each item on the belt
		// render separately
		// maybe do it a less efficient way but update once every 3 frames? like minecraft with 20t/s

		//var newItem = TypeLibrary.Create<ModelEntity>( ClassName );
		//newItem.Position = Position + Rotation.Left * 0.5f;
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
