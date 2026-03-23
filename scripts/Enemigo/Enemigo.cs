using Godot;
using System.Threading.Tasks;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 100f;
    [Export] public int Vida;
    
    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 40f;

    public override void _PhysicsProcess(double delta)
    {
        if (_estaHundiendose)
        {
            Position += Vector2.Down * _velocidadHundimiento * (float)delta;
            Rotation += 0.4f * (float)delta;

            Color c = Modulate;
            c.A -= 0.5f * (float)delta;
            Modulate = c;

            return;
        }

        Velocity = Vector2.Left * Velocidad;
        MoveAndSlide();

        DetectarColisionJugador();
    }

    public void RecibirDmg(int cantidad)
    {
        if (_estaHundiendose) return;
        Vida -= cantidad;
        if (Vida <= 0) _ = Hundirse(); // Llamada asíncrona segura
    }

    // NUEVO: Lógica de daño por fuego
    public async void Quemarse(int dañoPorSegundo, float duracion)
    {
        float tiempo = 0;
        while (tiempo < duracion && !_estaHundiendose && Vida > 0)
        {
            RecibirDmg(dañoPorSegundo);
            Modulate = new Color(1, 0.5f, 0.5f); // Tinte rojizo al arder
            await Task.Delay(1000); // Espera 1 segundo entre tics
            tiempo += 1.0f;
            Modulate = new Color(1, 1, 1); // Vuelve al color original
        }
    }

    private async Task Hundirse()
    {
        _estaHundiendose = true;
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        GetNode<CollisionShape2D>("CollisionShape2D2").SetDeferred("disabled", true);
        await Task.Delay(2500);
        QueueFree();
    }

   private async Task DetectarColisionJugador()
{
    for (int i = 0; i < GetSlideCollisionCount(); i++)
    {
        var collision = GetSlideCollision(i);

        if (collision.GetCollider() is Jugador jugador)
        {
            int daño = Mathf.Max(Vida, 0); // Aseguramos que sea >= 0
            jugador.RecibirDmg(daño);
            Hundirse();
            await Task.CompletedTask; // Para evitar advertencias de async sin await
            break; // Solo colisionamos con el jugador una vez
        }
    }
}
}
