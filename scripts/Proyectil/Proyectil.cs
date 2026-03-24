using Godot;
using System.Collections.Generic;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg = 1;
    [Export] public bool EsDeFuego = false;
    [Export] public bool EsRafaga = false;
    [Export] public bool EsSonico = false; // <--- NUEVO: Para el proyectil 4
    [Export] public float FuerzaRetroceso = 800f; // <--- NUEVO: Potencia del empuje
    [Export] public float DuracionFuego = 3.0f;

    private HashSet<Node2D> _enemigosYaDañados = new HashSet<Node2D>();

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }
private void OnBodyEntered(Node body)
{
    if (body is Enemigo enemigo)
    {
        if (!_enemigosYaDañados.Contains(enemigo))
        {
            _enemigosYaDañados.Add(enemigo);

            // Daño
            if (EsDeFuego) enemigo.Quemarse(Dmg, DuracionFuego);
            else enemigo.RecibirDmg(Dmg);

            // RETROCESO SEGURO
            if (EsSonico)
            {
                // Calculamos la dirección real: desde la bala hacia el enemigo
                // Esto garantiza que el barco NUNCA avance hacia la bala
                Vector2 direccionSegura = (enemigo.GlobalPosition - GlobalPosition).Normalized();
                
                // Aplicamos la fuerza en esa dirección
                enemigo.AplicarRetroceso(direccionSegura * FuerzaRetroceso);
            }
        }
    }
}


    private void OnScreenExited()
    {
        QueueFree();
    }
}
