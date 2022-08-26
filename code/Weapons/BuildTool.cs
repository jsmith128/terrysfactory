using Sandbox;
using System;
using System.Collections.Generic;

[Spawnable]
[Library( "tool_build", Title = "Build Tool" )]
partial class BuildTool : Weapon
{
	public int BuildRotation { get; set; } = 0;
	public string BuildingClassname = "BaseInserter";

	public List<string> BuildingList = new List<string>();
	public int BuildingIndex = 0;

	public bool Active = false;
	

	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float ReloadTime => 0.1f;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 5f;

	public TimeSince TimeSinceDischarge { get; set; }


	public ModelEntity Hologram;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
		SetBuildingList();
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		Active = true;
		SetBuildingList();

		if ( IsServer )
			return;
		Hologram = TypeLibrary.Create<ModelEntity>( BuildingClassname );
		Hologram.Model = Model.Load( "models/buildings/hologram.vmdl" );
		//Hologram = TypeLibrary.Create<ModelEntity>( "models/buildings/hologram.vmdl" );
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		Active = false;

		if ( IsServer || Hologram == null )
			return;
		Hologram.Delete();
	}

	public void SetBuildingList()
	{
		BuildingList.Clear();
		BuildingList.Add( "BaseInserter" );
		BuildingList.Add( "TransportBelt" );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
			TimeSincePrimaryAttack = 0;
			TimeSinceSecondaryAttack = 0;

		(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var rot = muzzle.Rotation;

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );

		ApplyAbsoluteImpulse( rot.Backward * 200.0f );

		Build();
	}

	private void Build()
	{
		if ( IsClient )
			return;
		if ( TimeSinceDischarge < 0.3f )
			return;

		TimeSinceDischarge = 0;

		var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 500 )
			.WithAllTags( "solid" )
			.Ignore( Owner )
			.Run();
		if ( tr.Entity.ClassName != "worldent" )
			return;
		var ent = TypeLibrary.Create<Entity>( BuildingClassname );
		ent.Position = new Vector3( RoundV3ToGrid( tr.EndPosition) );
		ent.Rotation = Rotation.FromYaw( BuildRotation * -90 );
		if (BuildingClassname == "TransportBelt" )
		{
			TransportBelt tb = ent as TransportBelt;
			tb.PostSpawnUpdate();
			VarStore.AllBelts.Add( tb );
		}

		foreach ( TransportBelt tb in VarStore.AllBelts )
		{
			if ( tb.CheckForBelt( "forward" ) != null )
			{
				tb.NextBelt = tb;
			}
		}
	}

	public static float RoundToGrid( float value )
	{
		return MathF.Round( value / TerrysFactory.gridSize ) * TerrysFactory.gridSize;
	}

	public static Vector3 RoundV3ToGrid( Vector3 v )
	{
		return new Vector3( RoundToGrid( v.x ), RoundToGrid( v.y ), v.z );
	}

	public override void AttackSecondary()
	{
		if ( IsClient )
			return;
		if ( !Host.IsServer )
			return;

		using ( Prediction.Off() )
		{
			var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 500 )
			.WithAllTags( "solid" )
			//.UseHitboxes()
			.Ignore( Owner )
			.Run();
			Log.Trace( "hit " + tr.Entity.ClassName );

			if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld )
				return;

			var Ent = tr.Entity.ClassName;
			if ( Ent == "TransportBelt" || Ent == "BaseInserter" )
			{
				if ( Ent == "TransportBelt" )
				{
					TransportBelt belt = tr.Entity as TransportBelt;
					VarStore.AllBelts.Remove( belt );
				}
				tr.Entity.Delete(); 
			}

		}
	}

	public override void Reload()
	{
		// Moved code to Simulate
	}

	public override void Simulate( Client cl )
	{
		if ( cl == null )
			return;

		base.Simulate( cl );

		if ( Input.Pressed( InputButton.Reload ) )
		{
			if ( IsReloading )
				return;

			TimeSinceReload = 0;
			IsReloading = true;

			BuildRotation += 1;
			if ( BuildRotation >= 4 )
			{
				BuildRotation = 0;
			}
		}

		if ( Input.Pressed( InputButton.Flashlight ) )
		{
			SetBuildingList();
			BuildingIndex += 1;
			if ( BuildingIndex > BuildingList.Count - 1 )
			{
				BuildingIndex = 0;
			}

			BuildingClassname = BuildingList[BuildingIndex];

			PlaySound( "flashlight-on" );

			if ( IsServer )
				return;
			using ( Prediction.Off() )
			{
				//Hologram.Delete();
				//Hologram = TypeLibrary.Create<ModelEntity>( BuildingClassname );
				if ( BuildingClassname == "TransportBelt" )
					//Hologram.Model = Model.Load( "models/buildings/transportbelt.vmdl" );
					Hologram.Model = Model.Load( "models/buildings/hologram.vmdl" );

				else if ( BuildingClassname == "BaseInserter" )
					//Hologram.Model = Model.Load( "models/buildings/inserter.vmdl" );
					Hologram.Model = Model.Load( "models/buildings/hologram.vmdl" );
			}
		}

		if (Active)
		{
			var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 500 )
			.UseHitboxes()
			.Ignore( Owner )
			.Run();

			if ( IsServer || Hologram == null )
				return;
			using ( Prediction.Off() )
			{
				Hologram.Position = new Vector3( RoundV3ToGrid( tr.EndPosition ) );
				Hologram.Rotation = Rotation.FromYaw( BuildRotation * -90 );
			}
		}
	}

}
