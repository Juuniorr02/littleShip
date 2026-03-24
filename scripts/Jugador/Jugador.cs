using Godot;
using System.Collections.Generic;
using System;

public partial class Jugador : CharacterBody2D
{   [Export] public PackedScene[] EscenasProyectiles;
    [Export] public float[] CooldownsBase = {};
    
    private int _proyectilSeleccionado = 0;
    private Dictionary<int, double> _proximosDisparos = new Dictionary<int, double>();

    [Export] public float MinForce = 400f;
    [Export] public float MaxForce = 1500f;
    [Export] public float ChargeTimeMax = 1.2f; 
    
    public static Jugador Instance;

    private float _currentCharge = 0f;
    private bool _isCharging = false;
    [Export] private int health = 100;
    
    private Node2D _pivoteCañon;
    private Marker2D _puntoDisparo;

    // --- NUEVO: Referencia a la línea visual ---
    private Line2D _lineaTrayectoria;

    public override void _Ready()
    {
    _pivoteCañon = GetNode<Node2D>("PivoteCañon");
    _puntoDisparo = _pivoteCañon.GetNode<Marker2D>("PuntoDisparo");
    
    // Ruta corregida según tu imagen:
    _lineaTrayectoria = _puntoDisparo.GetNode<Line2D>("LineaTrayectoria");
        
        for (int i = 0; i < 4; i++) 
        {
            _proximosDisparos[i] = 0;
        }
        
        Instance = this;
    }

    public override void _Process(double delta)
    {
        Vector2 direccion = GetGlobalMousePosition() - _pivoteCañon.GlobalPosition;
        float anguloEnGrados = Mathf.RadToDeg(direccion.Angle());
        anguloEnGrados = Mathf.Clamp(anguloEnGrados, -75f, 10f);
        _pivoteCañon.Rotation = Mathf.DegToRad(anguloEnGrados);

        if (Input.IsKeyPressed(Key.Key1)) CambiarMunicion(0);
        if (Input.IsKeyPressed(Key.Key2)) CambiarMunicion(1);
        if (Input.IsKeyPressed(Key.Key3)) CambiarMunicion(2);
        if (Input.IsKeyPressed(Key.Key4)) CambiarMunicion(3);

        ManejarDisparo(delta);
    }

    private void ManejarDisparo(double delta)
    {
        double tiempoActual = Time.GetTicksMsec() / 1000.0;

        if (Input.IsActionPressed("disparar"))
        {
            if (tiempoActual >= _proximosDisparos[_proyectilSeleccionado])
            {
                _isCharging = true;
                _currentCharge = Mathf.Min(_currentCharge + (float)delta, ChargeTimeMax);
                
                // --- NUEVO: Mostrar la trayectoria mientras cargamos ---
                ActualizarVisualizacionTrayectoria();
            }
        }
        else if (_isCharging)
        {
            // --- NUEVO: Ocultar la línea al disparar ---
            if (_lineaTrayectoria != null) _lineaTrayectoria.Visible = false;

            Fire(_proyectilSeleccionado);
            _proximosDisparos[_proyectilSeleccionado] = tiempoActual + CooldownsBase[_proyectilSeleccionado];
            _isCharging = false;
            _currentCharge = 0f;
        }
    }

    // --- NUEVO MÉTODO DE TRAYECTORIA ---
private void ActualizarVisualizacionTrayectoria()
{
    if (_lineaTrayectoria == null) return;

    _lineaTrayectoria.Visible = true;
    _lineaTrayectoria.ClearPoints();

    // Importante para que dibuje en el sitio correcto
    _lineaTrayectoria.GlobalPosition = Vector2.Zero;
    _lineaTrayectoria.Rotation = 0;

    // 1. Calculamos la fuerza actual
    float ratio = _currentCharge / ChargeTimeMax;
    float fuerza = Mathf.Lerp(MinForce, MaxForce, ratio);
    
    Vector2 velocidadInicial = _pivoteCañon.GlobalTransform.X * fuerza;
    Vector2 posActual = _puntoDisparo.GlobalPosition;
    
    float gravedad = (float)ProjectSettings.GetSetting("physics/2d/default_gravity");

    // --- CONFIGURACIÓN PARA GUÍA CORTA ---
    int numPuntos = 8;        // Pocos puntos para que la línea sea corta
    float pasoTiempo = 0.03f; // Tiempo muy corto entre puntos para que parezca casi recta

    for (int i = 0; i < numPuntos; i++)
    {
        float t = i * pasoTiempo;
        
        // Ecuación de trayectoria (aunque sea corta, si hay mucha gravedad se curvará un poco)
        Vector2 puntoEnElMundo = posActual + (velocidadInicial * t) + new Vector2(0, 0.5f * gravedad * t * t);
        
        _lineaTrayectoria.AddPoint(puntoEnElMundo);
    }
}


    // (El resto de tus funciones: CambiarMunicion, Fire, ConfigurarBala... se mantienen igual)

    public void CambiarMunicion(int indice)
    {
        if (indice >= 0 && indice < EscenasProyectiles.Length && _proyectilSeleccionado != indice)
        {
            _proyectilSeleccionado = indice;
            string nombreProyectil = indice switch {
                0 => "NORMAL", 1 => "FUEGO", 2 => "RAFAGA", 3 => "SONICO", _ => "DESCONOCIDO"
            };
            GD.PrintRich($"[color=yellow][MUNICIÓN][/color] Cambiado a: [b]{nombreProyectil}[/b]");
        }
    }

    private void Fire(int tipo)
    {
        if (tipo >= EscenasProyectiles.Length || EscenasProyectiles[tipo] == null) return;

        var instancia = EscenasProyectiles[tipo].Instantiate();
        GetTree().Root.AddChild(instancia);

        if (instancia is Proyectil primeraBala)
        {
            ConfigurarBala(primeraBala);
            if (primeraBala.EsRafaga)
            {
                for (int i = 0; i < 4; i++)
                {
                    var extra = EscenasProyectiles[tipo].Instantiate() as Proyectil;
                    GetTree().Root.AddChild(extra);
                    ConfigurarBala(extra, true);
                }
            }
        }
    }

    private void ConfigurarBala(Proyectil bala, bool conDispersion = false)
    {
        bala.GlobalTransform = _puntoDisparo.GlobalTransform;
        float ratio = _currentCharge / ChargeTimeMax;
        float fuerza = Mathf.Lerp(MinForce, MaxForce, ratio);
        Vector2 direccion = _pivoteCañon.GlobalTransform.X;

        if (conDispersion || bala.EsRafaga)
        {
            float desvio = Mathf.DegToRad((float)GD.RandRange(-15.0, 15.0));
            direccion = direccion.Rotated(desvio);
            fuerza *= (float)GD.RandRange(0.8, 1.2);
        }

        bala.LinearVelocity = direccion * fuerza;
    }

    public float GetCooldownRestante(int indice)
    {
        double tiempoActual = Time.GetTicksMsec() / 1000.0;
        double restante = _proximosDisparos[indice] - tiempoActual;
        return (float)Math.Max(restante, 0); // Nunca negativo
    }

    public void RecibirDmg(int cantidad)
    {
        health -= cantidad;
        health = Mathf.Max(health, 0);
    }

    public void Revivir() => health = 100;
    public int GetHealth() => health;
}
