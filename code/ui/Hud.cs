using Sandbox;
using Sandbox.UI;

[Library]
public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !IsClient )
			return;

		RootPanel.StyleSheet.Load( "/ui/Hud.scss" );

		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<VoiceList>();
		RootPanel.AddChild<VoiceSpeaker>();
		//RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<Health>();
		//RootPanel.AddChild<InventoryBar>();
		RootPanel.AddChild<BuildingInfo>();
		//RootPanel.AddChild<SpawnMenu>();
		RootPanel.AddChild<Crosshair>();
	}
}
