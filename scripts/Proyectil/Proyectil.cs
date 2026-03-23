using Godot;

public partial class Proyectil : RigidBody2D
{
    [Export] public int Dmg;
    [Export] public bool EsDeFuego = false;
    [Export] public float DuracionFuego = 3.0f;

    public override void _Ready()
    {
        // Importante: En el Inspector de la bala, 'Contact Monitor' debe ser ON 
        // y 'Max Contacts Reported' debe ser al menos 1.
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is Enemigo enemigo)
        {
            if (EsDeFuego)
                enemigo.Quemarse(Dmg, DuracionFuego);
            else
                enemigo.RecibirDmg(Dmg);
            
            GD.Print("¡Impacto!");
        }
    }

    private void OnScreenExited()
    {
        QueueFree();
    }
}
