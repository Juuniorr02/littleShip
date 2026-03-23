using Godot;

public partial class Enemigo : CharacterBody2D
{
    [Export] public float Velocidad = 100f;
    [Export] public int Vida;

    public static Enemigo Instance;

    // Animación de hundimiento
    private bool _estaHundiendose = false;
    private float _velocidadHundimiento = 40f;
    private float _tiempoHundimiento = 0f;
    private float _duracionHundimiento = 2.5f;

    // Daño por fuego (DoT)
    [Export] public int DañoPorSegundo = 5;
    [Export] public float DuracionFuego = 3f;
    private float _tiempoFuego = 0f;
    private Timer _timerFuego;

    public override void _Ready()
    {
        // Configuramos el timer para daño por fuego
        _timerFuego = new Timer();
        _timerFuego.WaitTime = 1f; // se dispara cada segundo
        _timerFuego.OneShot = false;
        _timerFuego.Connect("timeout", new Callable(this, "OnTimerFuegoTimeout"));
        AddChild(_timerFuego);

        Instance = this;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_estaHundiendose)
        {
            // Animación de hundimiento
            Position += Vector2.Down * _velocidadHundimiento * (float)delta;
            Rotation += 0.4f * (float)delta;

            Color c = Modulate;
            c.A -= 0.5f * (float)delta;
            Modulate = c;

            // Liberamos el nodo cuando termina la animación
            _tiempoHundimiento += (float)delta;
            if (_tiempoHundimiento >= _duracionHundimiento)
            {
                QueueFree();
            }

            return;
        }

        // Movimiento normal
        Velocity = Vector2.Left * Velocidad;
        MoveAndSlide();

        // Detectar colisión con jugador
        DetectarColisionJugador();
    }

    public void RecibirDmg(int cantidad)
    {
        if (_estaHundiendose) return;

        Vida -= cantidad;

        if (Vida <= 0 && !_estaHundiendose)
        {
            _estaHundiendose = true;

            // Desactivar colisiones
            GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
            GetNode<CollisionShape2D>("CollisionShape2D2").SetDeferred("disabled", true);
        }
    }

    public void Limpieza()
    {
        foreach (Node nodo in GetTree().Root.GetChildren())
        {
            if (nodo is Enemigo enemigo)
            {
                enemigo.QueueFree();
            }
        }
    }

    // Aplicar daño por fuego (DoT) seguro usando Timer
    public void Quemarse(int dañoPorSegundo, float duracion)
    {
        if (_estaHundiendose) return;

        DañoPorSegundo = dañoPorSegundo;
        DuracionFuego = duracion;
        _tiempoFuego = 0f;

        Modulate = new Color(1, 0.5f, 0.5f); // color rojizo
        _timerFuego.Start();
    }

    private void OnTimerFuegoTimeout()
    {
        if (_estaHundiendose)
        {
            _timerFuego.Stop();
            return;
        }

        RecibirDmg(DañoPorSegundo);

        _tiempoFuego += 1f;
        if (_tiempoFuego >= DuracionFuego)
        {
            _timerFuego.Stop();
            Modulate = new Color(1, 1, 1); // volver a color normal
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
                int daño = Mathf.Max(Vida, 0);
                jugador.RecibirDmg(daño);

                // Iniciar hundimiento tras golpear al jugador
                _estaHundiendose = true;
                GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
                GetNode<CollisionShape2D>("CollisionShape2D2").SetDeferred("disabled", true);

                break; // solo se aplica una vez
            }
        }
    }
}