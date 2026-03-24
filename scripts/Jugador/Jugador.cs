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

    public override void _Ready()
    {
        _pivoteCañon = GetNode<Node2D>("PivoteCañon");
        _puntoDisparo = _pivoteCañon.GetNode<Marker2D>("PuntoDisparo");
        
        // Inicializar los tiempos de disparo para evitar errores de diccionario
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

        // Atajos de teclado para cambiar munición
        if (Input.IsKeyPressed(Key.Key1)) CambiarMunicion(0);
        if (Input.IsKeyPressed(Key.Key2)) CambiarMunicion(1);
        if (Input.IsKeyPressed(Key.Key3)) CambiarMunicion(2);
        if (Input.IsKeyPressed(Key.Key4)) CambiarMunicion(3);

        ManejarDisparo(delta);
    }

    public void CambiarMunicion(int indice)
    {
        if (indice >= 0 && indice < EscenasProyectiles.Length && _proyectilSeleccionado != indice)
        {
            _proyectilSeleccionado = indice;
            
            string nombreProyectil = indice switch {
                0 => "NORMAL",
                1 => "FUEGO",
                2 => "RAFAGA",
                3 => "SONICO",
                _ => "DESCONOCIDO"
            };

            GD.PrintRich($"[color=yellow][MUNICIÓN][/color] Cambiado a: [b]{nombreProyectil}[/b]");
        }
    }

    private void ManejarDisparo(double delta)
    {
        double tiempoActual = Time.GetTicksMsec() / 1000.0;

        if (Input.IsActionPressed("disparar"))
        {
            // Solo empezamos a cargar si el cooldown ha pasado
            if (tiempoActual >= _proximosDisparos[_proyectilSeleccionado])
            {
                _isCharging = true;
                _currentCharge = Mathf.Min(_currentCharge + (float)delta, ChargeTimeMax);
            }
        }
        else if (_isCharging)
        {
            Fire(_proyectilSeleccionado);
            _proximosDisparos[_proyectilSeleccionado] = tiempoActual + CooldownsBase[_proyectilSeleccionado];
            _isCharging = false;
            _currentCharge = 0f;
        }
    }

    private void Fire(int tipo)
    {
    
    if (tipo >= EscenasProyectiles.Length) {
        GD.PrintErr("¡El índice está fuera de rango!");
        return;
    }

    if (EscenasProyectiles[tipo] == null) {
        GD.PrintErr($"¡La casilla {tipo} del Inspector está VACÍA!");
        return;
    }
        if (tipo >= EscenasProyectiles.Length || EscenasProyectiles[tipo] == null)
        {
            GD.PrintErr($"[ERROR] No hay escena asignada en el índice {tipo}");
            return;
        }

        // 1. Instanciamos la primera bala
        var instancia = EscenasProyectiles[tipo].Instantiate();
        GetTree().Root.AddChild(instancia);

        if (instancia is Proyectil primeraBala)
        {
            ConfigurarBala(primeraBala);

            // 2. Si es ráfaga, disparamos 4 adicionales
            if (primeraBala.EsRafaga)
            {
                for (int i = 0; i < 4; i++)
                {
                    var extra = EscenasProyectiles[tipo].Instantiate() as Proyectil;
                    GetTree().Root.AddChild(extra);
                    ConfigurarBala(extra, true); // Aplicar dispersión extra
                }
                GD.Print("¡Ráfaga de 5 proyectiles!");
            }
        }
        else
        {
            GD.PrintErr($"[ERROR] El objeto instanciado no es de tipo 'Proyectil'");
            instancia.QueueFree();
        }
    }

    private void ConfigurarBala(Proyectil bala, bool conDispersion = false)
    {
        // Posicionamiento inicial
        bala.GlobalTransform = _puntoDisparo.GlobalTransform;
        
        // Cálculo de fuerza según la carga
        float ratio = _currentCharge / ChargeTimeMax;
        float fuerza = Mathf.Lerp(MinForce, MaxForce, ratio);
        Vector2 direccion = _pivoteCañon.GlobalTransform.X;

        // Si es ráfaga o una bala extra, aplicamos aleatoriedad
        if (conDispersion || bala.EsRafaga)
        {
            float desvio = Mathf.DegToRad((float)GD.RandRange(-15.0, 15.0));
            direccion = direccion.Rotated(desvio);
            fuerza *= (float)GD.RandRange(0.8, 1.2);
        }

        // Aplicamos el movimiento al RigidBody2D
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
        GD.Print($"Jugador recibió {cantidad} de daño. Vida restante: {health}");
        
        if (health <= 0)
        {
        }
    }

    public void Revivir()
    {
        health = 100; // Restablece la vida a 100 o al valor que desees
        GD.Print("Jugador ha sido revivido. Vida restaurada a: " + health);
    }

    public int GetHealth() => health;
}
