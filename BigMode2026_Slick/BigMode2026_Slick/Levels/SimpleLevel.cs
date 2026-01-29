using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigMode2026_Slick;

/// <summary>
/// Level with a single floor collider.
/// </summary>
class SimpleLevel : MLevel
{
	Rectangle mCollider;

	public SimpleLevel(Rectangle r)
	{
		mCollider = r;
	}

	public override void Update(MUpdateInfo info)
	{
	}

	public override void Draw(MDrawInfo info)
	{
		info.mCanvas.DrawRect(mCollider, Color.AliceBlue, Layer.BACKGROUND);
	}

	public override bool QueryCollides(Rectangle bounds, MCardDir travelDir, MCollisionFlags flags)
	{
		return bounds.Intersects(mCollider);
	}


}
