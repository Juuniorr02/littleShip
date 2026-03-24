using Godot;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 100f;
    [Export] public int Vida = 10;
    
    private Vector2 _retrocesoActual = Vector2.Zero;
    [Export] public float FriccionRetroceso = 5f; // Bajada para que el golpe se deslice más

    private float _yOriginal; 

    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 40f;
    private float _tiempoHundimiento = 0f;
    private float _duracionHundimiento = 2.5f;

    [Export] public int DañoPorSegundo = 5;
    [Export] public float DuracionFuego = 3f;
    private float _tiempoFuego = 0f;
    private Timer _timerFuego;
    public static Enemigo Instance;

    public override void _Ready()
    {
        _yOriginal = GlobalPosition.Y;

        _timerFuego = new Timer();
        _timerFuego.WaitTime = 1f;
        _timerFuego.OneShot = false;
        _timerFuego.Connect("timeout", new Callable(this, "OnTimerFuegoTimeout"));
        AddChild(_timerFuego);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_estaHundiendose)
        {
            Position += Vector2.Down * _velocidadHundimiento * (float)delta;
            Rotation += 0.4f * (float)delta;
            Color c = Modulate;
            c.A -= 0.5f * (float)delta;
            Modulate = c;

            _tiempoHundimiento += (float)delta;
            if (_tiempoHundimiento >= _duracionHundimiento) QueueFree();
            return;
        }

        // 1. Movimiento base hacia la IZQUIERDA
        Vector2 movimientoBase = Vector2.Left * Velocidad;

        // 2. Fuerza de flotación (Efecto "Muelle")
        // Esta fuerza solo actúa en el eje Y para devolverlo a su carril
        float diferenciaY = _yOriginal - GlobalPosition.Y;
        Vector2 fuerzaRetornoY = new Vector2(0, diferenciaY * 4.0f); // 4.0f es la suavidad del retorno

        // 3. VELOCIDAD FINAL
        // Sumamos el movimiento constante + el golpe (en cualquier dirección) + la flotación
        Velocity = movimientoBase + _retrocesoActual + fuerzaRetornoY;

        // 4. Aplicamos fricción al retroceso
        _retrocesoActual = _retrocesoActual.Lerp(Vector2.Zero, (float)delta * FriccionRetroceso);

        MoveAndSlide();
        DetectarColisionJugador();
    }

public void AplicarRetroceso(Vector2 fuerza)
{
    if (_estaHundiendose) return;
    
    // Resetear el retroceso anterior si quieres que el impacto sea seco,
    // o sumarlo si quieres que varios impactos lo manden muy lejos.
    _retrocesoActual = fuerza; 
}


    public void RecibirDmg(int cantidad)
    {
        if (_estaHundiendose) return;
        Vida -= cantidad;
        if (Vida <= 0) Hundirse();
    }

    private void Hundirse()
    {
        _estaHundiendose = true;
        if (HasNode("CollisionShape2D")) GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        if (HasNode("CollisionShape2D2")) GetNode<CollisionShape2D>("CollisionShape2D2").SetDeferred("disabled", true);
    }

    public void Quemarse(int dañoPorSegundo, float duracion)
    {
        if (_estaHundiendose) return;
        DañoPorSegundo = dañoPorSegundo;
        DuracionFuego = duracion;
        _tiempoFuego = 0f;
        Modulate = new Color(1, 0.5f, 0.5f);
        _timerFuego.Start();
    }

    private void OnTimerFuegoTimeout()
    {
        if (_estaHundiendose) { _timerFuego.Stop(); return; }
        RecibirDmg(DañoPorSegundo);
        _tiempoFuego += 1f;
        if (_tiempoFuego >= DuracionFuego)
        {
            _timerFuego.Stop();
            Modulate = new Color(1, 1, 1);
        }
    }

    private void DetectarColisionJugador()
    {
        if (_estaHundiendose) return;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Jugador jugador)
            {
                jugador.RecibirDmg(Vida);
                Hundirse();
                break;
            }
        }
    }
    

    public void Limpieza() => QueueFree();
}
