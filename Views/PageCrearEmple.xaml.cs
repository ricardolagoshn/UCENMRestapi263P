using UCENMRestapi263P.Controllers;
using UCENMRestapi263P.Models;

namespace UCENMRestapi263P.Views;

public partial class PageCrearEmple : ContentPage
{
    private readonly EmpleadoController controller = new();
    private string? fotoBase64;

    public PageCrearEmple()
    {
        InitializeComponent();
    }

    private async void OnTomarFoto(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Foto", "Este dispositivo no permite tomar fotos.", "OK");
                return;
            }

            var foto = await MediaPicker.Default.CapturePhotoAsync();
            await CargarFotoAsync(foto);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo tomar la foto: {ex.Message}", "OK");
        }
    }

    private async void OnSeleccionarFoto(object? sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();
            await CargarFotoAsync(foto);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo seleccionar la foto: {ex.Message}", "OK");
        }
    }

    // Lee el archivo, lo convierte a base64 y lo muestra en pantalla
    private async Task CargarFotoAsync(FileResult? foto)
    {
        if (foto is null)
            return;

        using var stream = await foto.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        byte[] bytes = ReducirImagen(ms.ToArray());

        fotoBase64 = Convert.ToBase64String(bytes);
        imgFoto.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
    }

    // Reduce la foto a máximo 800px y la guarda como JPEG para que no pese demasiado
    // (las fotos de cámara pueden pesar varios MB y la BD/PHP pueden rechazarlas).
    private static byte[] ReducirImagen(byte[] original)
    {
        try
        {
            using var input = new MemoryStream(original);
            var imagen = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(input);
            using var reducida = imagen.Downsize(800, true);
            using var output = new MemoryStream();
            reducida.Save(output, Microsoft.Maui.Graphics.ImageFormat.Jpeg, 0.8f);
            return output.ToArray();
        }
        catch
        {
            return original; // si no se puede reducir, se envía tal cual
        }
    }

    private async void OnGuardar(object? sender, EventArgs e)
    {
        string nombre = txtNombre.Text?.Trim() ?? string.Empty;
        string apellidos = txtApellidos.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellidos))
        {
            await DisplayAlert("Validación", "Ingrese nombre y apellidos.", "OK");
            return;
        }

        if (!int.TryParse(txtEdad.Text, out int edad) || edad <= 0 || edad > 120)
        {
            await DisplayAlert("Validación", "Ingrese una edad válida.", "OK");
            return;
        }

        var empleado = new Empleado
        {
            Nombre = nombre,
            Apellidos = apellidos,
            Edad = edad,
            Foto = fotoBase64
        };

        btnGuardar.IsEnabled = false;
        cargando.IsVisible = cargando.IsRunning = true;

        var (exito, mensaje) = await controller.CrearEmpleadoAsync(empleado);

        cargando.IsVisible = cargando.IsRunning = false;
        btnGuardar.IsEnabled = true;

        if (exito)
        {
            await DisplayAlert("Éxito", mensaje, "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Error", mensaje, "OK");
        }
    }
}
