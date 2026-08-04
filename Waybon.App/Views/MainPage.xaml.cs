namespace Waybon.App.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(ViewModels.MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            LoadMapAsync();
        }

        private async void LoadMapAsync()
        {
            try
            {
                var html = await ReadResourceAsync("map.html");
                var css = await ReadResourceAsync("maplibre-gl.css");
                var js = await ReadResourceAsync("maplibre-gl.js");
                var styleJson = await ReadResourceAsync("style.json");

                html = html.Replace("<link href=\"maplibre-gl.css\" rel=\"stylesheet\" />", $"<style>{css}</style>");
                html = html.Replace("<script src=\"maplibre-gl.js\"></script>", $"<script>{js}</script>");
                html = html.Replace("MAP_STYLE_JSON_PLACEHOLDER", styleJson);

                MapWebView.Source = new HtmlWebViewSource
                {
                    Html = html,
                    BaseUrl = "https://localhost"
                };
            }
            catch (Exception ex)
            {
                MapWebView.Source = new HtmlWebViewSource
                {
                    Html = $"<html><body style='font-family:Georgia;padding:20px;color:#78716c;'><h1>Error</h1><p>{ex.Message}</p><p>{ex.StackTrace}</p></body></html>"
                };
            }
        }

        private static async Task<string> ReadResourceAsync(string filename)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}