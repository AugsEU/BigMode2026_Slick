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
		MAnimation carAnim = MData.I.LoadAnimation("Sprites/car");
		Car player = new Car(new Vector2(0.0f, 0.0f), new Point(16, 32), carAnim);
		mGameScene.GO.Add(player);

		mGameScene.GO.LoadLevel(new SimpleLevel());

		MGameObjectFocus focus = new(player);
		focus.pSpeed = new Vector4(10.0f);
		mCanvas.GetCamera().SetFocus(focus);
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
