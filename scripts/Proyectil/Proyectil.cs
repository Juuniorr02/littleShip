using Godot;
using System.Collections.Generic;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg;
    [Export] public bool EsDeFuego;
    [Export] public bool EsRafaga;
    [Export] public bool EsSonico; // <--- NUEVO: Para el proyectil 4
    [Export] public float FuerzaRetroceso; // <--- NUEVO: Potencia del empuje
    [Export] public float DuracionFuego;

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
                
                enemigo.AplicarRetroceso(direccionSegura * FuerzaRetroceso);
            }
        }
    }
    if (body is Enemigo2 enemigo2)
    {
        if (!_enemigosYaDañados.Contains(enemigo2))
        {
            _enemigosYaDañados.Add(enemigo2);

            // Daño
            if (EsDeFuego) enemigo2.Quemarse(Dmg, DuracionFuego);
            else enemigo2.RecibirDmg(Dmg);

            // RETROCESO SEGURO
            if (EsSonico)
            {
                // Calculamos la dirección real: desde la bala hacia el enemigo
                // Esto garantiza que el barco NUNCA avance hacia la bala
                Vector2 direccionSegura = (enemigo2.GlobalPosition - GlobalPosition).Normalized();
                
                // Aplicamos la fuerza en esa dirección
                enemigo2.AplicarRetroceso(direccionSegura * FuerzaRetroceso);
            }
        }
    }
}


    private void OnScreenExited()
    {
        QueueFree();
    }
}
