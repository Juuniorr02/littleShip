using Godot;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg = 1;

    public override void _Ready()
    {
        // Conectamos la señal de colisión con cuerpos
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        // El 'body' es el objeto con el que chocamos
        if (body is Enemigo enemigo)
        {
            enemigo.RecibirDmg(Dmg);
            GD.Print("¡Impacto crítico!");
        }
        
        // La bala se destruye al chocar (con el enemigo o el suelo si lo hubiera)
    }
	    private void OnScreenExited()
    {
        QueueFree();
    }
}
