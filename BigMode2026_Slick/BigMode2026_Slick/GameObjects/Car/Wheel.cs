namespace BigMode2026_Slick;

class Wheel
{
	float mAngleVel = 0.0f;

	/// <summary>
	/// Computes forces on the wheel
	/// </summary>
	public Vector2 ComputeForces(MUpdateInfo info, Vector2 pos, Vector2 groundSpeed, Vector2 wheelFacing, float engineTorque, float friction)
	{
		MugDebug.Assert(MugMath.ApproxEqual(wheelFacing.LengthSquared(), 1.0f), "Non normalised wheel facing vector");
		float moi = Tuning.I.Car.WheelMoI;
		float r = Tuning.I.Car.WheelRadius;
		
		// Speed of tyre at the ground
		float tyreSpeed = mAngleVel * MathF.Tau * r;
		
		// Velocity of tyre in reference frame of wheel
		Vector2 relativeTyreVel = wheelFacing * tyreSpeed;

		// Speed of tyre as it goes along the ground
		Vector2 absoluteTyreVel = relativeTyreVel + groundSpeed;

		// Ground friction 
		float frictionMag = friction;
		float atvMag = absoluteTyreVel.Length();
		if (atvMag < 1.0f) // Near zero velocity reduce friction for simulation stability
		{
			frictionMag = 2.0f ;
		}
		else if(atvMag <= 5.0f) // Static friction is higher
		{
			frictionMag *= 2.0f / atvMag;
		}
		else
		{
			frictionMag /= atvMag;
		}
		// Normalised velocity direction multiplied by the coeficient of friction
		Vector2 groundFriction = -(frictionMag) * absoluteTyreVel;

		// Angular velocity
		// First the torque from the road friction
		float frictionTorque = Vector2.Dot(wheelFacing, groundFriction);

		float totalToque = frictionTorque + engineTorque;

		float angularAcc = totalToque / moi;
		mAngleVel += angularAcc * info.mDelta;

		return groundFriction;
	}
}
