using Godot;

public partial class MarInfinito : Parallax2D
{
    [Export] public float VelocidadMar = -150f; // Negativo para ir a la izquierda

    public override void _Process(double delta)
    {
        // La propiedad Autoscroll permite que se mueva solo
        // Pero si quieres control manual total, modificamos el ScrollOffset:
        Vector2 nuevoOffset = ScrollOffset;
        nuevoOffset.X += VelocidadMar * (float)delta;
        ScrollOffset = nuevoOffset;
    }
}
