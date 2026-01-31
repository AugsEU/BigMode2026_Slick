using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace BigMode2026_Slick;

public class Main : MugMainGame
{
	public Main() : base(CreateGameSettings())
	{
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{
		Window.AllowUserResizing = true;
		Window.Title = "BigMode2026_Slick";
		Content.RootDirectory = "@Data";
		

		base.Initialize();

		InputConfig.SetDefaultButtons();
		SetWindowSize(2.0f);

#if DEBUG
		DRectLayer.NameLayers();
		InitTuner<Tune>(Tuning.I, "@Data/Tune/Values.xml");
#endif // DEBUG
	}

	private static MugEngineSettings CreateGameSettings()
	{
		MugEngineSettings settings = new MugEngineSettings();
		settings.mFPS = 60;
		settings.mNumLayers = Layer.MAX_LAYERS;
		settings.mResolution = new Point(640, 360);

		settings.mScreenTypes =
			[
				typeof(TitleScreen),
				typeof(GameScreen),
				typeof(WinScreen)
			];


		settings.mStartScreen = typeof(GameScreen);

		return settings;
	}
}
