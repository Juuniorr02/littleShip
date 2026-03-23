using Godot;
using System;

public partial class Iu : Control
{
    private Label healthLabel;
    private Jugador _jugador;

    public override void _Ready()
    {
        healthLabel = GetNode<Label>("%HealthLabel");
        
        // Buscamos al jugador en la escena actual
        _jugador = GetTree().Root.FindChild("Jugador", true, false) as Jugador;
        
        if (_jugador == null)
            GD.PrintErr("Error: No se encontró al Jugador desde la UI.");
    }

    public override void _Process(double delta)
    {
        if (_jugador != null && healthLabel != null)
        {
            healthLabel.Text = "Vida: " + _jugador.GetHealth().ToString();
        }
    }

    // Asegúrate de conectar estas funciones a las señales 'pressed' de tus botones en el editor
    public void _on_boton_1_pressed() => _jugador?.CambiarMunicion(0);
    public void _on_boton_2_pressed() => _jugador?.CambiarMunicion(1);
    public void _on_boton_3_pressed() => _jugador?.CambiarMunicion(2);
    public void _on_boton_4_pressed() => _jugador?.CambiarMunicion(3);
	public void _on_proyectil_pressed() 
{
    _jugador?.CambiarMunicion(0); // El 1 es el índice de la bala de fuego
}
	public void _on_fire_pressed() 
{
    _jugador?.CambiarMunicion(1); // El 1 es el índice de la bala de fuego
}
	public void _on_rafaga_pressed() 
{
    _jugador?.CambiarMunicion(2); // El 1 es el índice de la bala de fuego
}
	public void _on_sonic_pressed() 
{
    _jugador?.CambiarMunicion(3); // El 1 es el índice de la bala de fuego
}
}
