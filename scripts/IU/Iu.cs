using Godot;
using System;

public partial class Iu : Control
{
    private Label healthLabel;

	public override void _Ready()
	{
		healthLabel = GetNode<Label>("%HealthLabel");
		var jugador = GetTree().Root.GetNode<Jugador>("Main/Jugador");

		if (jugador != null)
		{
			healthLabel.Text = jugador.GetHealth().ToString();
		}
	}
}
