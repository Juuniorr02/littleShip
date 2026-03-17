using Godot;

public partial class GameSettings : Node
{
    private const string ConfigPath = "user://config.cfg";

    // Display
    public int Width { get; private set; } = 1920;
    public int Height { get; private set; } = 1080;
    public string Mode { get; private set; } = "Ventana";
    public string Calidad { get; private set; } = "Media";
    
    // Audio
    public float Volume { get; private set; } = 1f;
    public float SFX { get; private set; } = 1f;
    public float Musica { get; private set; } = 1f;
    
    // Gameplay
    public float Sensibilidad { get; private set; } = 1f;
    public bool InvertirY { get; private set; } = false;

    public void LoadConfig()
    {
        var config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok)
            return;

        // Display
        Width = (int)config.GetValue("display", "width", 1920);
        Height = (int)config.GetValue("display", "height", 1080);
        Mode = (string)config.GetValue("display", "mode", "Ventana");
        Calidad = (string)config.GetValue("display", "calidad", "Media");
        
        // Audio
        Volume = (float)config.GetValue("audio", "volume", 1f);
        SFX = (float)config.GetValue("audio", "sfx", 1f);
        Musica = (float)config.GetValue("audio", "musica", 1f);
        
        // Gameplay
        Sensibilidad = (float)config.GetValue("gameplay", "sensibilidad", 1f);
        string invertirYStr = (string)config.GetValue("gameplay", "invertirY", "false");
        InvertirY = invertirYStr == "true";

        GD.Print($"Configuración cargada: {Width}x{Height} {Mode} {Calidad} Vol={Volume} SFX={SFX} Musica={Musica} Sens={Sensibilidad} InvertY={InvertirY}");
    }

    public void SaveConfig()
    {
        var config = new ConfigFile();
        
        // Display
        config.SetValue("display", "width", Width);
        config.SetValue("display", "height", Height);
        config.SetValue("display", "mode", Mode);
        config.SetValue("display", "calidad", Calidad);
        
        // Audio
        config.SetValue("audio", "volume", Volume);
        config.SetValue("audio", "sfx", SFX);
        config.SetValue("audio", "musica", Musica);
        
        // Gameplay
        config.SetValue("gameplay", "sensibilidad", Sensibilidad);
        config.SetValue("gameplay", "invertirY", InvertirY ? "true" : "false");
        
        config.Save(ConfigPath);
    }

    public void ApplySettings()
    {
        DisplayServer.WindowSetSize(new Vector2I(Width, Height));
        DisplayServer.WindowSetMode(Mode switch
        {
            "Pantalla completa" => DisplayServer.WindowMode.Fullscreen,
            "Sin bordes" => DisplayServer.WindowMode.ExclusiveFullscreen,
            _ => DisplayServer.WindowMode.Windowed
        });

        int masterBus = AudioServer.GetBusIndex("Master");
        if (masterBus >= 0)
            AudioServer.SetBusVolumeDb(masterBus, Linear2Db(Volume));
        
        int sfxBus = AudioServer.GetBusIndex("SFX");
        if (sfxBus >= 0)
            AudioServer.SetBusVolumeDb(sfxBus, Linear2Db(SFX));
        
        int musicaBus = AudioServer.GetBusIndex("Musica");
        if (musicaBus >= 0)
            AudioServer.SetBusVolumeDb(musicaBus, Linear2Db(Musica));
    }

    private float Linear2Db(float linear)
    {
        if (linear <= 0) return -80;
        return 20f * Mathf.Log(linear) / Mathf.Log(10f);
    }
}