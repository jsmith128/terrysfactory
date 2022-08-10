using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class BuildingInfo : Panel
{
	public Label Info;
	public Label Title;
	public Label Description;

	public BuildingInfo()
	{
		Info = Add.Label( "Press F to cycle", "info" );
		Title = Add.Label( "Building", "title" );
		Description = Add.Label( "This is a building", "description" );
	}

	public override void Tick()
	{
		var bldClass = GetCurrentBuilding();
		SetClass( "active", true);

		if ( bldClass != null )
		{
			//var display = DisplayInfo.For( tool );

			Title.SetText( bldClass );
			Description.SetText( "Description" );
		}
	}

	string GetCurrentBuilding()
	{
		var player = Local.Pawn as Player;
		if ( player == null ) return null;

		var inventory = player.Inventory;
		if ( inventory == null ) return null;

		if ( inventory.Active is not BuildTool tool ) return null;

		return tool?.BuildingClassname;
	}
}
