using MugEngine.Scene;
using MugEngine.Screen;

namespace BigMode2026_Slick;
internal class GameScreen : MScreen
{
	MScene mGameScene;

	public GameScreen(Point resolution) : base(resolution)
	{
		CreateScene();
	}

	public override void OnActivate()
	{
		CreateScene();

		base.OnActivate();
	}

	private void CreateScene()
	{
		mGameScene = new MScene();
		mGameScene.AddUnique(new MGameObjectManager());

		// Player
		mGameScene.GO.Add(new Car(new Vector2(0.0f, 0.0f), new Point(20, 40), null));
	}

	public override void Update(MUpdateInfo info)
	{
		// Update the game's state.
		mGameScene.Update(info);

		base.Update(info);
	}

	public override void Draw(MDrawInfo info)
	{
		MDrawInfo canvasInfo = mCanvas.BeginDraw(info.mDelta);

		// Draw the scene...
		mGameScene.Draw(canvasInfo);

		MugDebug.FinalizeDebug(canvasInfo, Layer.FRONT);

		mCanvas.EndDraw();
	}
}
