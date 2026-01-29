namespace BigMode2026_Slick
{
	enum GInput
	{
		Left,
		Right,
		Forward,
		Backward,
		Confirm,
	}

	static class InputConfig
	{
		public static void SetDefaultButtons()
		{
			// Arrow keys or WASD
			MugInput.I.BindButton(GInput.Left, new MKeyboardButton(Keys.Left), new MKeyboardButton(Keys.A));
			MugInput.I.BindButton(GInput.Right, new MKeyboardButton(Keys.Right), new MKeyboardButton(Keys.D));
			MugInput.I.BindButton(GInput.Forward, new MKeyboardButton(Keys.Up), new MKeyboardButton(Keys.W));
			MugInput.I.BindButton(GInput.Backward, new MKeyboardButton(Keys.Down), new MKeyboardButton(Keys.S));

			// Confirm
			MugInput.I.BindButton(GInput.Confirm, new MKeyboardButton(Keys.Enter));
		}
	}
}
