using Godot;
using System;
using System.IO;

public partial class OptionsPausa : CanvasLayer
{
	private menu_pausa pausaMenu;
    private OptionButton resolucionOption;
    private OptionButton modoOption;
    private HSlider volumeOption;

    private const string ConfigPath = "user://config.cfg";
    
    // Rutas base
    private const string BasePath = "CenterContainer/PanelContainer/MarginContainer/PanelContainer/VBoxContainer/VBoxContainer";
    private const string ResolucionPath = BasePath + "/Resolucion/Resolucion/Resolucion/ResolucionOption";
    private const string ModoPath = BasePath + "/Modo/Modo/Modo/ModoOption";
    private const string VolumenPath = BasePath + "/Volumen/Volumen/Volumen/VolumenOption";
    private const string ApplyButtonPath = BasePath + "/Botones/Botones/Aplicar";
    private const string BackButtonPath = BasePath + "/Botones/Botones/Volver";


    public override void _Ready()
    {
        resolucionOption = GetNode<OptionButton>(ResolucionPath);
        modoOption = GetNode<OptionButton>(ModoPath);
        volumeOption = GetNode<HSlider>(VolumenPath);

        // Añadir resoluciones disponibles
        resolucionOption.AddItem("1280x720");
        resolucionOption.AddItem("1920x1080");
        resolucionOption.AddItem("2560x1440");
        resolucionOption.AddItem("3840x2160");

        // Añadir modos de pantalla
        modoOption.AddItem("Ventana");
        modoOption.AddItem("Pantalla completa");
        modoOption.AddItem("Sin bordes");

        // Conectar botones
        GetNode<Button>(ApplyButtonPath).Pressed += OnApply;
        GetNode<Button>(BackButtonPath).Pressed += OnBack;

        // Cargar configuración si existe
        LoadConfig();

		Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (keyEvent.Keycode == Key.Escape)
            {
                OnBack();
            }
        }
    }

    private void OnApply()
    {
        // Cambiar resolución
        string res = resolucionOption.GetItemText(resolucionOption.Selected);
        string[] parts = res.Split('x');
        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);

        // Cambiar modo de pantalla
        string mode = modoOption.GetItemText(modoOption.Selected);
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.Windowed;

        if (mode == "Pantalla completa")
            windowMode = DisplayServer.WindowMode.Fullscreen;
        else if (mode == "Sin bordes")
            windowMode = DisplayServer.WindowMode.ExclusiveFullscreen;

        DisplayServer.WindowSetMode(windowMode);
        DisplayServer.WindowSetSize(new Vector2I(width, height));

        // Cambiar volumen general, SFX y música
        float volume = (float)volumeOption.Value;
        
        int masterBus = AudioServer.GetBusIndex("Master");
        if (masterBus >= 0)
            AudioServer.SetBusVolumeDb(masterBus, Linear2Db(volume));
        
        // Guardar configuración
        SaveConfig(width, height, mode, volume);
    }

    private void OnExit()
    {
        string res = resolucionOption.GetItemText(resolucionOption.Selected);
        string[] parts = res.Split('x');
        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);

        // Cambiar modo de pantalla
        string mode = modoOption.GetItemText(modoOption.Selected);
        DisplayServer.WindowMode windowMode = DisplayServer.WindowMode.Windowed;

        if (mode == "Pantalla completa")
            windowMode = DisplayServer.WindowMode.Fullscreen;
        else if (mode == "Sin bordes")
            windowMode = DisplayServer.WindowMode.ExclusiveFullscreen;

        DisplayServer.WindowSetMode(windowMode);
        DisplayServer.WindowSetSize(new Vector2I(width, height));

        float volume = (float)volumeOption.Value;
        
        int masterBus = AudioServer.GetBusIndex("Master");
        if (masterBus >= 0)
            AudioServer.SetBusVolumeDb(masterBus, Linear2Db(volume));

        // Guardar configuración
        SaveConfig(width, height, mode, volume);
        GetTree().ChangeSceneToFile("res://scenes/ui/Menu.tscn");
    }

    private void OnBack()
	{
    	Visible = false;

    	if (pausaMenu != null)
        	pausaMenu.Visible = true;
	}

    // Función auxiliar para convertir de 0..1 a decibelios
    private float Linear2Db(float linear)
    {
        if (linear <= 0) return -80;
        return 20f * (Mathf.Log(linear) / Mathf.Log(10f));
    }

    // Guarda la configuración en un archivo
	private void SaveConfig(int width, int height, string mode, float volume)
	{
    	var config = new ConfigFile();

    	// Usar ruta user://
    	const string path = "user://config.cfg";

    	// Cargar si existe
    	Error err = config.Load(path);
    	if (err != Error.Ok)
        	config = new ConfigFile(); // archivo nuevo

    	// Guardar valores
    	config.SetValue("display", "width", width);
    	config.SetValue("display", "height", height);
    	config.SetValue("display", "mode", mode);
    	config.SetValue("audio", "volume", volume);

    	// Guardar archivo
    	config.Save(path);
	}

    // Carga la configuración desde el archivo si existe
    public void LoadConfig()
    {
        var config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok)
            return; // no existe el archivo, usar valores por defecto

        int width = (int)config.GetValue("display", "width", 1920);
        int height = (int)config.GetValue("display", "height", 1080);
        string mode = (string)config.GetValue("display", "mode", "Ventana");
        float volumen = (float)config.GetValue("audio", "volume", 1f);

        // Aplicar configuración
        DisplayServer.WindowSetSize(new Vector2I(width, height));
        DisplayServer.WindowSetMode(mode switch
        {
            "Pantalla completa" => DisplayServer.WindowMode.Fullscreen,
            "Sin bordes" => DisplayServer.WindowMode.ExclusiveFullscreen,
            _ => DisplayServer.WindowMode.Windowed
        });

        int masterBus = AudioServer.GetBusIndex("Master");
        if (masterBus >= 0)
            AudioServer.SetBusVolumeDb(masterBus, Linear2Db(volumen));

        // Reflejar en UI
        string resText = $"{width}x{height}";
        for (int i = 0; i < resolucionOption.GetItemCount(); i++)
        {
            if (resolucionOption.GetItemText(i) == resText)
            {
                resolucionOption.Selected = i;
                break;
            }
        }

        for (int i = 0; i < modoOption.GetItemCount(); i++)
        {
            if (modoOption.GetItemText(i) == mode)
            {
                modoOption.Selected = i;
                break;
            }
        }

        volumeOption.Value = volumen;
    }

	public void MostrarOpciones(menu_pausa menu)
	{
    	pausaMenu = menu;

    	Visible = true;
    	ProcessMode = ProcessModeEnum.Always;
	}	
}