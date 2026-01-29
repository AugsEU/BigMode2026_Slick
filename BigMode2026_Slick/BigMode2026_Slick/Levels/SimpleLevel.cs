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
	}

	public override bool QueryCollides(Rectangle bounds, MCardDir travelDir, MCollisionFlags flags)
	{
		return false;
	}


}
