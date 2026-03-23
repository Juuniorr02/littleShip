using Godot;
using System.Collections.Generic;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg = 1;
    [Export] public bool EsDeFuego = false;
    [Export] public bool EsRafaga = false; // ESTE debe estar marcado en la escena de ráfaga
    [Export] public float DuracionFuego = 3.0f;

    private List<Node2D> _enemigosGolpeados = new List<Node2D>();

    public override void _Ready()
    {
        // Importante para RigidBody2D
        ContactMonitor = true;
        MaxContactsReported = 5;
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Enemigo enemigo && !_enemigosGolpeados.Contains(enemigo))
        {
            _enemigosGolpeados.Add(enemigo);
            if (EsDeFuego) enemigo.Quemarse(Dmg, DuracionFuego);
            else enemigo.RecibirDmg(Dmg);
        }
    }

    private void OnScreenExited() => QueueFree();
}
