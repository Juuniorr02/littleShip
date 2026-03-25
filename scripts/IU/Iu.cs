using Godot;
using System;

public partial class Iu : Control
{
    private Label healthLabel;
    private Label cannonNormal;
    private Label cannonFire;
    private Label cannonRafaga;
    private Label cannonSonic;

    private Jugador _jugador;

    public override void _Ready()
    {
        healthLabel = GetNode<Label>("%HealthLabel");
        cannonNormal = GetNode<Label>("%CannonNormal");
        cannonFire = GetNode<Label>("%CannonFire");
        cannonRafaga = GetNode<Label>("%CannonRafaga");
        cannonSonic = GetNode<Label>("%CannonSonic");
        
        // Buscamos al jugador en la escena actual
        _jugador = GetTree().Root.FindChild("Jugador", true, false) as Jugador;
        
        if (_jugador == null)
            GD.PrintErr("Error: No se encontró al Jugador desde la UI.");
    }

    public override void _Process(double delta)
    {
        if (_jugador != null && healthLabel != null)
        {
            healthLabel.Text = _jugador.GetHealth().ToString();
        }

        if (cannonNormal != null)
            cannonNormal.Text = _jugador.GetCooldownRestante(0).ToString("F1"); // 1 decimal
        if (cannonFire != null)
            cannonFire.Text = _jugador.GetCooldownRestante(1).ToString("F1");
        if (cannonRafaga != null)
            cannonRafaga.Text = _jugador.GetCooldownRestante(2).ToString("F1");
        if (cannonSonic != null)
            cannonSonic.Text = _jugador.GetCooldownRestante(3).ToString("F1");
    }
    public override void _GuiInput(InputEvent @event)
{
    if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
    {
        GetViewport().SetInputAsHandled();
    }
}

    // Asegúrate de conectar estas funciones a las señales 'pressed' de tus botones en el editor
    public void _on_boton_1_pressed() => _jugador?.CambiarMunicion(0);
    public void _on_boton_2_pressed() => _jugador?.CambiarMunicion(1);
    public void _on_boton_3_pressed() => _jugador?.CambiarMunicion(2);
    public void _on_boton_4_pressed() => _jugador?.CambiarMunicion(3);
}
