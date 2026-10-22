using Pract7.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pract7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        HttpClient client = new HttpClient();

        public ObservableCollection<Furniture> Items { get; } = new ObservableCollection<Furniture>();

        private Furniture? selectedItem;
        public Furniture? SelectedItem
        {
            get => selectedItem;
            set { selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        public string FilterMaterial { get; set; } = "";
        public string FilterMaxPrice { get; set; } = "";

        private string status = "";
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged("Status"); }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            client.BaseAddress = new Uri("https://localhost:7038/api/");

            client.Timeout = TimeSpan.FromSeconds(10);

            _ = LoadAsync();
        }


        

        private async void LoadClick(object sender, RoutedEventArgs e)
        {
            await LoadAsync();
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            SelectedItem = new Furniture(); 

            Status = "Заполните поля и нажмите Сохранить";
        }

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            var item = SelectedItem;

            if (item == null) 
            {
                Status = "Нет выбранной записи"; return; 
            }
            try
            {
                if (item.Id == 0) 
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync("furniture", item);

                    response.EnsureSuccessStatusCode();

                    var created = await response.Content.ReadFromJsonAsync<Furniture>();

                    if (created != null)
                    {
                        Items.Add(created);
                    }
                    Status = "Запись создана";
                }
                else
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync($"furniture/{item.Id}", item);

                    response.EnsureSuccessStatusCode();

                    Status = "Изменения сохранены puut";

                    await LoadAsync();
                }
            }
            catch (Exception ex) 
            {
                ShowError(ex); 
            }
        }

        private async void PatchClick(object sender, RoutedEventArgs e)
        {
            var item = SelectedItem;

            if (item == null || item.Id == 0) 
            { 
                Status = "Выберите существующую запись"; return; 
            }
            try
            {
                HttpResponseMessage response = await client.PatchAsJsonAsync(
                    $"furniture/{item.Id}",
                    new { item.Name, item.Price });

                response.EnsureSuccessStatusCode();

                Status = "Частичное изменение выполнено PATCH";

                await LoadAsync();
            }
            catch (Exception ex) 
            {
                ShowError(ex); 
            }
        }

        private async void DeleteClick(object sender, RoutedEventArgs e)
        {
            var item = SelectedItem;

            if (item == null) 
            {
                Status = "Нет выбранной записи."; return; 
            }
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"furniture/{item.Id}");

                response.EnsureSuccessStatusCode();

                Items.Remove(item);  

                SelectedItem = null;

                Status = "Запись у";
            }
            catch (Exception ex) 
            {
                ShowError(ex); 
            }
        }



        private async Task LoadAsync()
        {
            try
            {
                string rFu = "furniture";


                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(FilterMaterial))
                {
                    parts.Add("material=" + Uri.EscapeDataString(FilterMaterial));
                }
                if (!string.IsNullOrWhiteSpace(FilterMaxPrice))
                {
                    parts.Add("maxPrice=" + FilterMaxPrice);

                }
                if (parts.Count > 0)
                {
                    rFu += "?" + string.Join("&", parts);
                }

                HttpResponseMessage response = await client.GetAsync(rFu);

                response.EnsureSuccessStatusCode();

                var list = await response.Content.ReadFromJsonAsync<List<Furniture>>();

                Items.Clear();

                if (list != null)
                {
                    foreach (var f in list) Items.Add(f);
                }
                    
                SelectedItem = null;

                Status = "Загружено " + Items.Count;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ShowError(Exception ex)
        {
            if (ex is HttpRequestException)
            {
                Status = "Сервер недоступе";
            }
            else if (ex is TaskCanceledException)
            {
                Status = "Сервер не ответил";
            }
            else if (ex is JsonException)
            {
                Status = "Сервер вернул некорректные данные";
            }
            else
            {
                Status = "Ошибка " + ex.Message;
            }
        }

        //нотифай проперти
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}


//static async void  GHb()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    var responce = await httpClient.GetAsync("api/products");

//    var json = await responce.Content.ReadAsStringAsync();

//    Console.WriteLine(json);
//}
////Пример асинхронного HTTP-запроса

//static async Task GetProdAsync()
//{
//    HttpClient client = new HttpClient();
//    HttpResponseMessage responce = await client.GetAsync("https://localhost:7038/api/products");
//    Console.WriteLine(responce.StatusCode);
//}

////Пример выполнения GET-запроса
//static async Task GetProd()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    var Response = await httpClient.GetAsync("api/products");

//    if (Response.IsSuccessStatusCode)
//    {
//        var json = await Response.Content.ReadAsStringAsync();
//        Console.WriteLine(json);
//    }
//    else
//    {
//        Console.WriteLine($"Error: {Response.StatusCode}");
//    }
//}
////Получение объектов из JSON
//static async Task GetFromJson()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    var responce = await httpClient.GetFromJsonAsync<List<Furniture>>("api/products");
//}

////Отправка данных на сервер
//static async Task GoToServer()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync("api/products", new Furniture
//    {
//        Id = 4,
//        Name = "Test",
//        Price = 140
//    });

//}
////Изменение объекта
//static async Task RedactFurniture()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    HttpResponseMessage responseMessage = await httpClient.PutAsJsonAsync("api/products/4", new Furniture
//    {
//        Id = 4,
//        Name = "Test",
//        Price = 130
//    }
//  );
//    if (responseMessage.IsSuccessStatusCode)
//    {
//        Console.WriteLine($"Furniture Changed Su {responseMessage.StatusCode}");
//    }
//}
////Частичное Изменение объекта
//static async Task SomeRedactFurniture()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    HttpResponseMessage responseMessage = await httpClient.PutAsJsonAsync("api/products/4", new Furniture
//    {
//        Name = "Some_Test",
//    }
//  );
//    if (responseMessage.IsSuccessStatusCode)
//    {
//        Console.WriteLine($"Furniture Path Changed  {responseMessage.StatusCode}");
//    }
//}

////Удаление обьекта
//static async Task DeleteFurniture()
//{
//    HttpClient httpClient = new HttpClient();

//    httpClient.BaseAddress = new Uri("https://localhost:7038/");

//    HttpResponseMessage resp = await httpClient.DeleteAsync("api/product/4");

//    if (resp.IsSuccessStatusCode)
//    {
//        Console.WriteLine($"Furniture Deleted {resp.StatusCode}");
//    }
//}

////Обработка HTTP-ошибок. Проверка //IsSuccessStatusCode

//static async Task Resp()
//{
//    using HttpResponseMessage response = await client.GetAsync("api/products/100");

//    switch (response.StatusCode)
//    {
//        case HttpStatusCode.OK:
//            Console.WriteLine("Furniture Found");
//            break;

//        case HttpStatusCode.NotFound:
//            Console.WriteLine("Furniture NotFound");
//            break;

//        case HttpStatusCode.InternalServerError:
//            Console.WriteLine("Server Error");
//            break;
//        default:
//            Console.WriteLine($"Code {response.StatusCode}");
//            break;
//    }
//}