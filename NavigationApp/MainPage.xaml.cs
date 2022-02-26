using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NavigationApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // creamos una lista de todas las paginas
        private readonly List<(string Etiqueta, Type pagina)> _paginas = new List<(string Etiqueta, Type pagina)>()
        {
            ("Inicio",typeof (Home)),
            ("Aplicaciones",typeof (Aplicaciones)),
            ("Juegos",typeof (Juegos)),
            ("Musica",typeof (Musica))
        };
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void MostrarMensaje(string Title,string Content)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = Title,
                Content = Content,
                CloseButtonText = "OK"
            };
            ContentDialogResult result = await dialog.ShowAsync();

        }
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            // en caso de fallarla navegacion
            MostrarMensaje("NAvegacion fallo","No se pudo cargar la pagina solicitada");
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // cuando carga la navegacion
            // cuando se navega a otra pagina 
            //agregamos un metodo a el evento
            ContentFrame.Navigated += On_Navigated;

            // cargar una pagina por defecto que se mostrara al inicio
            // cargamos la primer opcion
            NavView.SelectedItem = NavView.MenuItems[0]; // inicio

            // cargar el contenido de la pagina en el conten frame
            NavView_Navigate("Inicio");

        }

        private void NavView_Navigate(string etiqueta)
        {
            Type _pagina = null;
            var item = _paginas.FirstOrDefault((p) => p.Etiqueta == etiqueta);//puede ser nulo
            _pagina = item.pagina;

            // verificar que no estamos navegando a la misma pagina
            var paginaPrevia = ContentFrame.CurrentSourcePageType;
            if (!(_pagina is null) && !Equals(paginaPrevia,_pagina))
            {
                ContentFrame.Navigate(_pagina);
                
                
            }

        }
        private void On_Navigated(object sender,NavigationEventArgs e)
        {
            // habilitar el boton de retroceder dado que ya se navego a otra pagina
            // este debe tener una valor boleano
            // asignamos en valor de "se puede retroceder(canGoBack)" del content frame 
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            // cambiar el elemennto selecionado
            if (ContentFrame.SourcePageType != null)
            {
                var item = _paginas.FirstOrDefault(p => p.pagina == e.SourcePageType);
                NavView.SelectedItem = NavView.MenuItems.OfType<NavigationViewItem>().First(p => p.Tag.Equals(item.Etiqueta));
            }


        }
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                MostrarMensaje("Pagina","Seleciona Ajustes");
                //no continuar
                return;
            }
            // al selecionar un elemneto del menu
            if (args.InvokedItemContainer != null)
            {
                var etiqueta = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(etiqueta);
            }
        }
        private bool IntentarVolver()
        {
            if (!ContentFrame.CanGoBack)
            {
                // si no puede volver a tras no continuamos 
                return false; 
            }
            // si el panel esta abierto no continuamos  
            if (NavView.IsPaneOpen && (NavView.DisplayMode == NavigationViewDisplayMode.Compact || NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            // voolver a la pagina anterior
            ContentFrame.GoBack();
            return true;

        }
        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            // al hacer click en el boton de navegar atras
            IntentarVolver();
        }
    }
}
