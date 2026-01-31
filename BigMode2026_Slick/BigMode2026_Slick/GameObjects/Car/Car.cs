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
	Vector2[] mWheelPositions = new Vector2[4];

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
		// Front wheels
		mWheelPositions[0] = new Vector2(mSize.X * 0.5f, -mSize.Y * COM_SHIFT);
		mWheelPositions[1] = new Vector2(-mSize.X * 0.5f, -mSize.Y * COM_SHIFT);

		// Back wheels
		mWheelPositions[2] = new Vector2(mSize.X * 0.5f, mSize.Y * (1.0f - COM_SHIFT));
		mWheelPositions[3] = new Vector2(-mSize.X * 0.5f, mSize.Y * (1.0f - COM_SHIFT));

		mRect.mRot = 0.0f;

	}

	#endregion Init





	#region Update

	/// <summary>
	/// Update car phyiscs
	/// </summary>
	public override void Update(MUpdateInfo info)
	{
		UpdateInputs(info);

		// Do physics in substeps to improve stability
		const int NUM_SUB_STEPS = 4;
		MUpdateInfo stepInfo = info;
		stepInfo.mDelta *= 1.0f / NUM_SUB_STEPS;

		for (int i = 0; i < NUM_SUB_STEPS; i++)
		{
			UpdatePhysics(info);
		}
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
			mWheelAngleDelta = -turnAngle;
		}
		else if(MugInput.I.ButtonDown(GInput.Right))
		{
			mWheelAngleDelta = turnAngle;
		}
		else
		{
			mWheelAngleDelta = 0.0f;
		}

#if DEBUG
		if(MugInput.I.ButtonDown(GInput.DebugReset))
		{
			mCenterOfMass = Vector2.Zero;
		}
#endif 
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
		float spinDamp = Tuning.I.Car.SpinDamping;

		Vector2 x = mRect.GetSideVec();
		Vector2 y = mRect.GetForwardVec();

		Vector2 xn = x; xn.Normalize();
		Vector2 yn = y; yn.Normalize();

		Vector2 totalForce = Vector2.Zero;
		float totalTorque = 0.0f;
		for(int w = 0; w < NUM_WHEELS; w++)
		{
			bool backWheel = w >= 2;
			Vector2 relPos = mWheelPositions[w];
			float rd = relPos.Length();

			Vector2 wheelFacing = new Vector2(0.0f, -1.0f);

			float wheelAngle = mRect.mRot;
			if(!backWheel)//Only front wheels turn
			{
				wheelAngle += mWheelAngleDelta;
			}
			wheelFacing.Rotate(wheelAngle);

			Vector2 wheelDelta = xn * relPos.X + yn * relPos.Y;
			Vector2 wheelPos = mCenterOfMass + wheelDelta;
			Vector2 rotationVel = -wheelDelta.Perpendicular() * mAngularVel * MathF.Tau;
			Vector2 wheelForce = ComputeWheelForces(info, 
				groundSpeed: mVelocity + rotationVel,
				wheelFacing: wheelFacing,
				driveForceMag: backWheel ? mEngineTorque : 0.0f,
				friction: backWheel ? backFric : frontFric);

			totalForce += wheelForce;

			float wheelTorque = Vector2.Dot(-wheelDelta.Perpendicular(), wheelForce);
			totalTorque += wheelTorque;

#if CAR_DEBUG_DRAW
			MugDebug.AddDebugRay(wheelPos, wheelForce, Color.White, Layer.FRONT);
			MugDebug.AddDebugRay(wheelPos, wheelFacing * 10.0f, Color.Brown, Layer.FRONT);
			//MugDebug.AddDebugRay(wheelPos, rotationVel, Color.Blue, Layer.FRONT);
#endif // CAR_DEBUG_DRAW
		}

		// Resistance and losses proportional to L^2
		Vector2 resistance = -mVelocity * mVelocity.Length() * airResistanceCoef;
		totalForce += resistance;

		// A = F/M
		mAngularVel += info.mDelta * (totalTorque / (carMoI * 10.0f));
		mAngularVel -= info.mDelta * (mAngularVel * spinDamp);
		mVelocity += info.mDelta * totalForce / carMass;

		mCenterOfMass += info.mDelta * mVelocity;
		mRect.mRot += info.mDelta * mAngularVel;

		UpdatePosAndBounds();
	}

	/// <summary>
	/// Computes forces on the wheel
	/// </summary>
	public Vector2 ComputeWheelForces(MUpdateInfo info, Vector2 groundSpeed, Vector2 wheelFacing, float driveForceMag, float friction)
	{
		MugDebug.Assert(MugMath.ApproxEqual(wheelFacing.LengthSquared(), 1.0f), "Non normalised wheel facing vector");

		Vector2 wheelPerp = wheelFacing.Perpendicular();
		Vector2 sideVel = Vector2.Dot(wheelPerp, groundSpeed) * wheelPerp;

		const float VEL_THRESH = 400.0f;
		float sideVelLen = sideVel.Length();
		if(sideVelLen < VEL_THRESH)
		{
			sideVelLen = VEL_THRESH;
		}
		Vector2 frictionForce = -(friction / sideVelLen) * sideVel;

		return driveForceMag * wheelFacing + frictionForce;
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
