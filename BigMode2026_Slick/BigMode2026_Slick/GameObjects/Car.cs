
namespace BigMode2026_Slick;

internal class Car : MGameObject
{
	MAnimation mAnimation = null;
	MRotRect mRect = new();

	public Car(Vector2 pos, Point size, MAnimation anim)
	{
		mPosition = pos;
		mSize = size;

		mRect = new MRotRect(size: size.ToVector2());
		mSize = mRect.BoundsRect().Size;
	}

	public override void Update(MUpdateInfo info)
	{
		mRect.SetCenter(mPosition);

		if(MugInput.I.ButtonDown(GInput.Left))
		{
			mRect.mRot += 2.0f * info.mDelta;
		}
		else if (MugInput.I.ButtonDown(GInput.Right))
		{
			mRect.mRot -= 2.0f * info.mDelta;
		}
	
		if (MugInput.I.ButtonDown(GInput.Forward))
		{
			mPosition += mRect.GetForwardVec() * 5.0f * info.mDelta;
		}
		else if (MugInput.I.ButtonDown(GInput.Backward))
		{
			mPosition -= mRect.GetForwardVec() * 5.0f * info.mDelta;
		}
	}

	public override void Draw(MDrawInfo info)
	{
		foreach(Vector2 pt in mRect.GetPoints())
		{
			info.mCanvas.DrawDot(pt, Color.Red, 0);
		}
	}
}
