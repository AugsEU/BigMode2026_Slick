#define CAR_DEBUG_DRAW

namespace BigMode2026_Slick;

internal class Car : MGameObject
{
	#region Constants

	const int NUM_WHEELS = 4;
	const float COM_SHIFT = 0.3f;

	#endregion Constants



	#region Members

	MAnimation mAnimation = null;

	// Physics
	Vector2 mCenterOfMass;
	Vector2 mVelocity = Vector2.Zero;
	float mAngularVel = 0.0f;

	// Inputs
	float mWheelAngleDelta = 0.0f;
	float mEngineTorque = 0.0f;

	MRotRect mRect = new();
	Wheel[] mWheels = new Wheel[4];
	Vector2[] mWheelRelativePositions = new Vector2[4];

	#endregion Members


	#region Init

	/// <summary>
	/// Initialise car object
	/// </summary>
	public Car(Vector2 pos, Point size, MAnimation anim)
	{
		mCenterOfMass = pos;
		mRect = new MRotRect(size: size.ToVector2());
		UpdatePosAndBounds();

		// Init wheels
		for (int w = 0; w < NUM_WHEELS; w++)
			mWheels[w] = new Wheel();

		// Front wheels
		mWheelRelativePositions[0] = new Vector2(mSize.X * 0.5f, -mSize.Y * COM_SHIFT);
		mWheelRelativePositions[1] = new Vector2(-mSize.X * 0.5f, -mSize.Y * COM_SHIFT);

		// Back wheels
		mWheelRelativePositions[2] = new Vector2(mSize.X * 0.5f, mSize.Y * (1.0f - COM_SHIFT));
		mWheelRelativePositions[3] = new Vector2(-mSize.X * 0.5f, mSize.Y * (1.0f - COM_SHIFT));

	}

	#endregion Init





	#region Update

	/// <summary>
	/// Update car phyiscs
	/// </summary>
	public override void Update(MUpdateInfo info)
	{
		UpdateInputs(info);
		UpdatePhysics(info);
	}

	void UpdateInputs(MUpdateInfo info)
	{
		float torque = Tuning.I.Car.EngineTorque;
		float turnAngle = Tuning.I.Car.WheelTurnAngle;

		mEngineTorque = 0.0f;
		if (MugInput.I.ButtonDown(GInput.Forward))
		{
			mEngineTorque = Tuning.I.Car.EngineTorque;
		}
		
		if (MugInput.I.ButtonDown(GInput.Left))
		{
			mWheelAngleDelta = turnAngle;
		}
		else if(MugInput.I.ButtonDown(GInput.Right))
		{
			mWheelAngleDelta = -torque;
		}
	}

	/// <summary>
	/// Update the car's physics
	/// </summary>
	void UpdatePhysics(MUpdateInfo info)
	{
		float frontFric = Tuning.I.Car.FrontWheelFriction;
		float backFric = Tuning.I.Car.BackWheelFriction;
		float carMoI = Tuning.I.Car.MoI;
		float carMass = Tuning.I.Car.Mass;
		float airResistanceCoef = Tuning.I.Car.AirResistance;

		Vector2 x = mRect.GetSideVec();
		Vector2 y = mRect.GetForwardVec();

		Vector2 xn = x; xn.Normalize();
		Vector2 yn = y; yn.Normalize();

		Vector2 totalForce = Vector2.Zero;
		float totalTorque = 0.0f;
		for(int w = 0; w < mWheels.Length; w++)
		{
			bool backWheel = w >= 2;
			Vector2 relPos = mWheelRelativePositions[w];
			float rd = relPos.Length();

			Vector2 rotationVel = relPos.Perpendicular() * mAngularVel * MathF.Tau;

			Vector2 wheelFacing = new Vector2(0.0f, -1.0f);

			float wheelAngle = mRect.mRot;
			if(!backWheel)//Only front wheels turn
			{
				wheelAngle += mWheelAngleDelta;
			}
			wheelFacing.Rotate(mRect.mRot + mWheelAngleDelta);


			Vector2 wheelPos = mCenterOfMass + xn * relPos.X + yn * relPos.Y;
			Vector2 wheelForce = mWheels[w].ComputeForces(info, pos: wheelPos, 
				groundSpeed: mVelocity + rotationVel,
				wheelFacing: wheelFacing,
				engineTorque: backWheel ? mEngineTorque : 0.0f, // only back wheels have torque
				friction: backWheel ? backFric : frontFric);

			totalForce += wheelForce;

			float wheelTorque = Vector2.Dot(relPos.Perpendicular(), wheelForce);
			totalTorque += wheelTorque;
		}

		// Resistance and losses proportional to L^2
		Vector2 resistance = -mVelocity * mVelocity.Length() * airResistanceCoef;
		totalForce += resistance;

		// A = F/M
		mAngularVel += info.mDelta * (totalTorque / carMoI);
		mVelocity += info.mDelta * totalForce ;

		mCenterOfMass += info.mDelta * mVelocity;
		mRect.mRot += info.mDelta * mAngularVel;

		UpdatePosAndBounds();
	}


	/// <summary>
	/// Update the gameobject's position and bounds
	/// The bounds do not represent the precise hitbox but the bounding
	/// box
	/// </summary>
	void UpdatePosAndBounds()
	{
		Vector2 x = mRect.GetSideVec();
		Vector2 y = mRect.GetForwardVec();

		mRect.mPos = mCenterOfMass - x * 0.5f - y * COM_SHIFT;

		Rectangle bounds = mRect.BoundsRect();
		mSize = bounds.Size;
		mPosition = bounds.Location.ToVector2();
	}

	#endregion Update





	#region Draw

	/// <summary>
	/// Draw the car
	/// </summary>
	public override void Draw(MDrawInfo info)
	{
#if CAR_DEBUG_DRAW

		foreach(Vector2 pt in mRect.GetPoints())
		{
			info.mCanvas.DrawDot(pt, Color.Red, 0);
		}

		info.mCanvas.DrawDot(mPosition, Color.AliceBlue, 1);
		info.mCanvas.DrawDot(mCenterOfMass, Color.Yellow, 1);
#endif // CAR_DEBUG_DRAW
	}

	#endregion Draw

}
