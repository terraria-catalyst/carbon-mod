namespace TeamCatalyst.Carbon.Assets
{
	public static class CarbonAssets
	{
		// god i hate doing this but tmod's asset organization sucks, so going with this

		// foundational
		public const string AssetDirectory = "Carbon/Assets/";
		public const string TextureDirectory = AssetDirectory + "Textures/";

		#region Main Content
		// general directories
		public const string MainContent = TextureDirectory + "MainContent/";
		public const string MCArmors = MainContent + "Armors/";
		public const string MCBuffs = MainContent + "Buffs/";

		// specific directories
		public const string BunnySet = MCArmors + "Bunny/";
		public const string BunnyBuffs = MCBuffs + "Bunny/";
		#endregion
	}
}