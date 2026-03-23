using Godot;
using System.Collections.Generic;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg = 1;
    [Export] public bool EsDeFuego = false;
    [Export] public bool EsRafaga = false;
    [Export] public float DuracionFuego = 3.0f;

    // --- NUEVO: Lista de enemigos que ya han sido golpeados por ESTA bala ---
    private HashSet<Node2D> _enemigosYaDañados = new HashSet<Node2D>();

    public override void _Ready()
    {
        // Importante: Asegúrate de que 'Contact Monitor' sea True 
        // y 'Max Contacts Reported' sea al menos 5 en el Inspector.
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        // 1. Verificamos si es un enemigo
        if (body is Enemigo enemigo)
        {
            // 2. Verificamos si ya le hemos hecho daño con esta bala concreta
            if (!_enemigosYaDañados.Contains(enemigo))
            {
                // 3. Añadimos el enemigo a la "lista negra" de esta bala
                _enemigosYaDañados.Add(enemigo);

                // 4. Aplicamos el daño (Normal o Fuego)
                if (EsDeFuego)
                {
                    enemigo.Quemarse(Dmg, DuracionFuego);
                }
                else
                {
                    enemigo.RecibirDmg(Dmg);
                }

                GD.Print($"¡Impacto único registrado en {enemigo.Name}!");
            }
        }
        
        // No llamamos a QueueFree() aquí para que el proyectil 
        // siga su camino realista a través del barco.
    }

    private void OnScreenExited()
    {
        QueueFree();
    }
}
