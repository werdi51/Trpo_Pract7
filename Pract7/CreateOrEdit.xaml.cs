using Pract7.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace Pract7
{
    public partial class CreateOrEdit : Window, INotifyPropertyChanged
    {
        private readonly HttpClient client;
        private readonly bool isNew;
        private readonly Furniture original;
        private bool saving;

        public Furniture Item { get; }



        private string status = "";
        public string Status
        {
            get => status;
            set { status = value; OnPropertyChanged(nameof(Status)); }
        }

        public CreateOrEdit(HttpClient client, Furniture? item)
        {
            InitializeComponent();

            this.client = client;
            isNew = item == null;

            original = Clone(item);
            Item = Clone(original);

            DataContext = this;
        }

        private static Furniture Clone(Furniture? inside) => inside == null
            ? new Furniture()
            : new Furniture
            {
                Id = inside.Id,
                Name = inside.Name,
                Material = inside.Material,
                Price = inside.Price,
                Quantity = inside.Quantity,
                IsAvailable = inside.IsAvailable
            };

        private async void SaveClick(object sender, RoutedEventArgs e)
        {
            if (saving) return;

            if (string.IsNullOrWhiteSpace(Item.Name) || string.IsNullOrWhiteSpace(Item.Material))
            {
                Status = "Заполните Название и Материал";
                return;
            }

            saving = true;
            try
            {
                HttpResponseMessage response = isNew
                    ? await client.PostAsJsonAsync("furniture", Item)
                    : await client.PutAsJsonAsync($"furniture/{Item.Id}", Item);

                response.EnsureSuccessStatusCode();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                saving = false;
            }
        }

        private async void PatchClick(object sender, RoutedEventArgs e)
        {
            if (saving) return;

            var changes = new Dictionary<string, object>();

            if (Item.Name != original.Name) changes["name"] = Item.Name;
            if (Item.Material != original.Material) changes["material"] = Item.Material;
            if (Item.Price != original.Price) changes["price"] = Item.Price;
            if (Item.Quantity != original.Quantity) changes["quantity"] = Item.Quantity;
            if (Item.IsAvailable != original.IsAvailable) changes["isAvailable"] = Item.IsAvailable;

            if (changes.Count == 0)
            {
                Status = "Изменений нет";
                return;
            }

            saving = true;
            try
            {
                HttpResponseMessage response = await client.PatchAsJsonAsync($"furniture/{Item.Id}", changes);

                response.EnsureSuccessStatusCode();

                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                saving = false;
            }
        }

        private void ShowError(Exception ex)
        {
            if (ex is HttpRequestException http && http.StatusCode.HasValue)
            {
                Status = "Сервер вернул ошибку " + (int)http.StatusCode.Value +
                         " (" + http.StatusCode.Value + ")";
            }
            else if (ex is HttpRequestException)
            {
                Status = "Сервер недоступен";
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
                Status = "Ошибка: " + ex.Message;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
