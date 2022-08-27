using Sandbox;
using System;
using System.Collections.Generic;

[Spawnable]
[Library( "tool_item", Title = "Item Tool" )]
partial class ItemTool : Weapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float ReloadTime => 0.1f;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 15f;

	public TimeSince TimeSinceDischarge { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.PrimaryAttack );
	}

	public override void AttackPrimary()
	{
		if ( TimeSinceDischarge < 0.1f )
			return;
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimatedEntity)?.SetAnimParameter( "b_attack", true );

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var rot = muzzle.Rotation;

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );

		ApplyAbsoluteImpulse( rot.Backward * 200.0f );

		PlaceItem();
	}

	private void PlaceItem()
	{
		if ( IsClient )
			return;

		TimeSinceDischarge = 0;

		var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * 500 )
			.WithAllTags( "solid" )
			.Ignore( Owner )
			.Run();

		

		/*if ( tr.Entity.ClassName != "worldent" )
			return;*/


		var item = new Item();
		item.Position = tr.EndPosition;

		if ( tr.Entity.ClassName == "TransportBelt" )
		{
			Log.Info( "Hit a belt, going to put the item on it now" );
			var tb = tr.Entity as TransportBelt;
			tb.RecieveItem( item, null, 1 );
		}
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
			if ( Ent == "Item" )
			{
				Item item = tr.Entity as Item;
				VarStore.Items.Remove( item );
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

		if ( Input.Pressed( InputButton.Flashlight ) )
		{
			PlaySound( "flashlight-on" );
		}
	}

}
