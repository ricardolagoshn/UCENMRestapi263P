using System.Collections.ObjectModel;
using UCENMRestapi263P.Controllers;
using UCENMRestapi263P.Models;

namespace UCENMRestapi263P.Views;

public partial class PageListEmple : ContentPage
{
    private readonly EmpleadoController controller = new();
    public ObservableCollection<Empleado> Empleados { get; } = new();

    public PageListEmple()
    {
        InitializeComponent();
        listaEmpleados.ItemsSource = Empleados;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Se recarga cada vez que aparece (por ejemplo al volver de crear un empleado)
        cargando.IsVisible = cargando.IsRunning = true;
        await CargarEmpleadosAsync();
        cargando.IsVisible = cargando.IsRunning = false;
    }

    private async void OnNuevoEmpleado(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageCrearEmple());
    }

    private async Task CargarEmpleadosAsync()
    {
        var lista = await controller.GetEmpleadosAsync();
        Empleados.Clear();
        foreach (var emp in lista)
            Empleados.Add(emp);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await CargarEmpleadosAsync();
        refreshView.IsRefreshing = false;
    }

    private async void OnEmpleadoSeleccionado(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Empleado emp)
            return;

        await DisplayAlert("Empleado", $"{emp.NombreCompleto}\nEdad: {emp.Edad}", "OK");
        listaEmpleados.SelectedItem = null;
    }
}
