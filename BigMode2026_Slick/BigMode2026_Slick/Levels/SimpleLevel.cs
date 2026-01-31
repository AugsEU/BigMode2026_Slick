namespace BigMode2026_Slick;

/// <summary>
/// Level with a single floor collider.
/// </summary>
class SimpleLevel : MLevel
{
	public SimpleLevel()
	{
	}

	public override void Update(MUpdateInfo info)
	{
	}

	public override void Draw(MDrawInfo info)
	{
		const int NUM_DOTS = 100;
		const float DOT_SPACING = 100.0f;
		for(int i = 0; i < NUM_DOTS; i++)
		{
			float x = (i - NUM_DOTS / 2) * DOT_SPACING;
			for(int j = 0; j < NUM_DOTS; j++)
			{
				float y = (j - NUM_DOTS / 2) * DOT_SPACING;
				Vector2 pt = new Vector2(x, y);
				info.mCanvas.DrawDot(pt, Color.AliceBlue);
			}
		}
	}

	public override bool QueryCollides(Rectangle bounds, MCardDir travelDir, MCollisionFlags flags)
	{
		return false;
	}
}
