using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
//namespace Sandbox;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class TerrysFactory : Sandbox.Game
{
	public static float gridSize = 64;
	public TimeUntil beltTickTimer = 0.2f;
	private float timertime = 0.1f;

	public TerrysFactory()
	{
		if ( IsServer )
		{
			// Create the HUD
			_ = new Hud();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public static float RoundToGrid( float value )
	{
		return MathF.Round( value / gridSize ) * gridSize;
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );
		var player = new FactPlayer( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	/*NOT WORKING/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new FactPlayer();
		client.Pawn = pawn;

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
			pawn.Transform = tx;
		}
	}*/

	/*[ConCmd.Server( "spawnbld" )]
	public static async Task SpawnBuilding( string buildingclass, int facing )
	{
		var owner = ConsoleSystem.Caller?.Pawn;

		if ( ConsoleSystem.Caller == null )
			return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( owner )
			.Run();

		//var modelRotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
		//var modelRotation = new Rotation();
		//modelRotation.z = 90;


		*//*var ent = Entity.CreateByName( buildingclass );
		var entPos = new Vector3( tr.EndPosition.x % 64, tr.EndPosition.y % 64, tr.EndPosition.z );
		ent.Position = entPos;
		ent.Rotation = modelRotation;*//*

		var ent = TypeLibrary.Create<Entity>( buildingclass );

		ent.Position = tr.EndPosition;
		ent.Position = new Vector3( RoundToGrid(tr.EndPosition.x), RoundToGrid( tr.EndPosition.y), tr.EndPosition.z );
		//ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) );
		ent.Rotation = Rotation.FromYaw( facing * -90 );
	}*/

	[ConCmd.Server( "spawn_entity" )]
	public static void SpawnEntity( string entName )
	{
		Player owner = ConsoleSystem.Caller.Pawn as Player;

		if ( owner == null )
			return;

		var entityType = TypeLibrary.GetTypeByName<Entity>( entName );
		if ( entityType == null )

			if ( !TypeLibrary.Has<SpawnableAttribute>( entityType ) )
				return;

		var tr = Trace.Ray( owner.EyePosition, owner.EyePosition + owner.EyeRotation.Forward * 200 )
			.UseHitboxes()
			.Ignore( owner )
			.Size( 2 )
			.Run();

		var ent = TypeLibrary.Create<Entity>( entityType );
		if ( ent is BaseCarriable && owner.Inventory != null )
		{
			if ( owner.Inventory.Add( ent, true ) )
				return;
		}

		

		ent.Position = tr.EndPosition;
		ent.Rotation = Rotation.From( new Angles( 0, owner.EyeRotation.Angles().yaw, 0 ) );

		if ( ent is Item )
		{
			Log.Info( "It's an item, looking for a belt" );
			TransportBelt belt = Help.CheckForNearbyBelt( tr.EndPosition + Vector3.Down * 2f);
			if ( belt != null )
			{
				Log.Info( "belt isnt null (from Game.cs)" );
				/*ent.Position = belt.Position + belt.Rotation.Right * 0.25f;*/
				belt.RecieveItem( ent as Item );
			}
		}

		//Log.Info( $"ent: {ent}" );
	}

	public override void DoPlayerNoclip( Client player )
	{
		if ( player.Pawn is Player basePlayer )
		{
			if ( basePlayer.DevController is NoclipController )
			{
				Log.Info( "Noclip Mode Off" );
				basePlayer.DevController = null;
			}
			else
			{
				Log.Info( "Noclip Mode On" );
				basePlayer.DevController = new NoclipController();
			}
		}
	}

	[ClientRpc]
	public override void OnKilledMessage( long leftid, string left, long rightid, string right, string method )
	{
		//KillFeed.Current?.AddEntry( leftid, left, rightid, right, method );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( IsClient )
			return;
		//Log.Info( beltTickTimer );
		if ( beltTickTimer <= 0)
		{
			Log.Info( "##########################TICK" );
			beltTickTimer = timertime;
			foreach ( Entity ent in VarStore.AllBelts )
			{
				Log.Info( "foreach" );
				//if ( ent.ClassName == "TransportBelt" )
				//{
				TransportBelt belt = ent as TransportBelt;
				if ( belt.ItemsR.Count > 0 || belt.ItemsL.Count > 0 )
				{
					Log.Info( "ticking belt" );
					belt.TickBelt();

				}
				//}
			}
		}
		
	}
}
