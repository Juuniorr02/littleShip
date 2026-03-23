using Godot;
using System.Threading.Tasks;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 100f;
    [Export] public int Vida = 1;
    
    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 40f;

    public override void _Process(double delta)
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
        Position += Vector2.Left * Velocidad * (float)delta;
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
        await Task.Delay(2500);
        QueueFree();
    }
}
