using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Pract7.Model;
using Hvorenkov_Trpo_6_Var_22.Models;
using System.Net;

namespace Pract7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        static async void  GHb()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            var responce = await httpClient.GetAsync("api/products");

            var json = await responce.Content.ReadAsStringAsync();

            Console.WriteLine(json);
        }
        //Пример асинхронного HTTP-запроса

        static async Task GetProdAsync()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage responce = await client.GetAsync("https://localhost:7038/api/products");
            Console.WriteLine(responce.StatusCode);
        }

        //Пример выполнения GET-запроса
        static async Task GetProd()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            var Response = await httpClient.GetAsync("api/products");

            if (Response.IsSuccessStatusCode)
            {
                var json = await Response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Error: {Response.StatusCode}");
            }
        }
        //Получение объектов из JSON
        static async Task GetFromJson()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            var responce = await httpClient.GetFromJsonAsync<List<Furniture>>("api/products");
        }

        //Отправка данных на сервер
        static async Task GoToServer()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            HttpResponseMessage responseMessage = await httpClient.PostAsJsonAsync("api/products", new Furniture
            {
                Id = 4,
                Name = "Test",
                Price = 140
            });

        }
        //Изменение объекта
        static async Task RedactFurniture()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            HttpResponseMessage responseMessage = await httpClient.PutAsJsonAsync("api/products/4", new Furniture
            {
                Id = 4,
                Name = "Test",
                Price = 130
            }
          );
            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine($"Furniture Changed Su {responseMessage.StatusCode}");
            }
        }
        //Частичное Изменение объекта
        static async Task SomeRedactFurniture()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            HttpResponseMessage responseMessage = await httpClient.PutAsJsonAsync("api/products/4", new Furniture
            {
                Name = "Some_Test",
            }
          );
            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine($"Furniture Path Changed  {responseMessage.StatusCode}");
            }
        }

        //Удаление обьекта
        static async Task DeleteFurniture()
        {
            HttpClient httpClient = new HttpClient();

            httpClient.BaseAddress = new Uri("https://localhost:7038/");

            HttpResponseMessage resp = await httpClient.DeleteAsync("api/product/4");

            if (resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"Furniture Deleted {resp.StatusCode}");
            }
        }

        //Обработка HTTP-ошибок. Проверка //IsSuccessStatusCode

        static async Task Resp()
        {
            using HttpResponseMessage response = await client.GetAsync("api/products/100");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Console.WriteLine("Furniture Found");
                    break;

                case HttpStatusCode.NotFound:
                    Console.WriteLine("Furniture NotFound");
                    break;

                case HttpStatusCode.InternalServerError:
                    Console.WriteLine("Server Error");
                    break;
                default:
                    Console.WriteLine($"Code {response.StatusCode}");
                    break;
            }
        }




    }
}