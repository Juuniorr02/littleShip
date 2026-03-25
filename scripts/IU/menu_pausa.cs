using Godot;

public partial class menu_pausa : CanvasLayer
{
    private Button btnGuardarSalir;
    private Button btnVolver;
    private Button btnReiniciar;
    private Button btnOpciones  ;

    private bool isPaused = false;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        btnGuardarSalir = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/guardarsalir");
        btnVolver = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/volver");
        btnReiniciar = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/reiniciar");
        btnOpciones = GetNodeOrNull<Button>("PanelContainer/VBoxContainer/opciones");

        ConfigurarBoton(btnGuardarSalir);
        ConfigurarBoton(btnVolver);

        if (btnGuardarSalir != null)
            btnGuardarSalir.Pressed += OnGuardarSalir;

        if (btnVolver != null)
            btnVolver.Pressed += Salir;

        if (btnReiniciar != null)
            btnReiniciar.Pressed += OnReiniciar;

        if (btnOpciones != null)
            btnOpciones.Pressed += OnOpciones;

        Visible = false;
    }

    private void ConfigurarBoton(Button b)
    {
        if (b == null) return;

        b.ProcessMode = ProcessModeEnum.Always;
        b.MouseFilter = Control.MouseFilterEnum.Stop;
    }

    public override void _Input(InputEvent e)
    {
        if (e.IsActionPressed("pausa"))
        {
            if (isPaused) QuitarPausa();
            else Pausar();
        }
    }

    private void Pausar()
    {
        isPaused = true;
        GetTree().Paused = true;
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

	private void Salir()
    {
        QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    private void QuitarPausa()
    {
        isPaused = false;
        GetTree().Paused = false;
        Visible = false;
    }

	private void OnGuardarSalir()
	{
		QuitarPausa();
	}

    private void OnReiniciar()
	{
		QuitarPausa();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Jugador.Instance.Revivir();
        foreach (Node nodo in GetTree().GetNodesInGroup("Enemigos"))
        {
            if (nodo is Enemigo enemigo)
            enemigo.Limpieza();

            if (nodo is Enemigo2 enemigo2)
            enemigo2.Limpieza();
        }
    	GD.Print("Reiniciar partida");
    	GetTree().ReloadCurrentScene();
	}

    private void OnOpciones()
    {
        var optionsMenu = GetTree().CurrentScene.GetNodeOrNull<OptionsPausa>("OptionsPausa");

        if (optionsMenu != null)
        {
            GD.Print("Abrir menú de opciones");

            optionsMenu.MostrarOpciones(this);
            Visible = false;
        }
    }
}