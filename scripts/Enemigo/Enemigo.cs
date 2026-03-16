using Godot;
using System.Threading.Tasks;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 150f;
    [Export] public int Vida = 2;
    
    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 40f;

    public override void _Process(double delta)
    {
        if (_estaHundiendose)
        {
            // Se mueve hacia abajo y rota como si zozobrara
            Position += Vector2.Down * _velocidadHundimiento * (float)delta;
            Rotation += 0.4f * (float)delta; // Rota un poco
            
            // Se desvanece (Modulate controla la transparencia)
            Color c = Modulate;
            c.A -= 0.5f * (float)delta; 
            Modulate = c;
            return;
        }

        // Movimiento normal a la izquierda
        Position += Vector2.Left * Velocidad * (float)delta;
    }

    public async void RecibirDmg(int cantidad)
    {
        if (_estaHundiendose) return;

        Vida -= cantidad;
        if (Vida <= 0)
        {
            await Hundirse();
        }
    }

    private async Task Hundirse()
    {
        _estaHundiendose = true;
        
        // Importante: Desactivar colisión para que no estorbe a otros barcos
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        
        // Esperamos 2.5 segundos de "animación" de hundido antes de borrarlo
        await Task.Delay(2500);
        QueueFree();
    }
}
