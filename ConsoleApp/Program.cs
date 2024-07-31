using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Birthday
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; }
}

   public class Program
{
    private const string FilePath = "birthdays.json";
    private static int _nextId = 1; // Для генерации уникальных ID

    public static void Main(string[] args)
    {
        Console.Title="Поздравлятор";
        LoadNextId(); // Загружаем следующий ID при запуске
        DisplayUpcomingBirthdays();
        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Показать все ДР");
            Console.WriteLine("2. Показать сегодняшние и ближайшие ДР");
            Console.WriteLine("3. Добавить ДР");
            Console.WriteLine("4. Редактировать ДР");
            Console.WriteLine("5. Удалить ДР");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayAllBirthdays();
                    break;
                case "2":
                    DisplayUpcomingBirthdays();
                    break;
                case "3":
                    AddBirthday();
                    break;
                case "4":
                    EditBirthday();
                    break;
                case "5":
                    DeleteBirthday();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("\nНеверный выбор. Попробуйте снова.\n");
                    break;
            }
        }
    }

    private static List<Birthday> GetBirthdays()
    {
        if (!File.Exists(FilePath))
        {
            return new List<Birthday>();
        }

        var jsonString = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Birthday>>(jsonString) ?? new List<Birthday>();
    }

    private static void SaveBirthdays(List<Birthday> birthdays)
    {
        var jsonString = JsonSerializer.Serialize(birthdays, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, jsonString);
    }

    private static void LoadNextId()
    {
        var birthdays = GetBirthdays();
        if (birthdays.Count > 0)
        {
            _nextId = birthdays.Max(b => b.Id) + 1; // Устанавливаем следующий ID
        }
    }

    private static void DisplayAllBirthdays()
    {
        var birthdays = GetBirthdays();
        Console.WriteLine("\nСписок всех дней рождения:");
        foreach (var birthday in birthdays)
        {
            Console.WriteLine($"ID: {birthday.Id}, Имя: {birthday.Name}, ДР: {birthday.DateOfBirth.ToShortDateString()}, Роль: {birthday.Role}");
        }
        Console.WriteLine();
    }

    private static void DisplayUpcomingBirthdays()
    {
        var birthdays = GetBirthdays();
        var today = DateTime.Today;
        var upcomingBirthdays = birthdays
            .Where(b =>
            {
                var birthdayThisYear = new DateTime(today.Year, b.DateOfBirth.Month, b.DateOfBirth.Day);
                return birthdayThisYear >= today && birthdayThisYear <= today.AddDays(10);
            })
            .OrderBy(b => new DateTime(today.Year, b.DateOfBirth.Month, b.DateOfBirth.Day))
            .ToList();

        Console.WriteLine("\nСписок сегодняшних и ближайших дней рождения:");
        foreach (var birthday in upcomingBirthdays)
        {
            Console.WriteLine($"ID: {birthday.Id}, Имя: {birthday.Name}, ДР: {birthday.DateOfBirth.ToShortDateString()}, Роль: {birthday.Role}");
        }
        Console.WriteLine();
    }

    private static void AddBirthday()
    {
        var newBirthday = new Birthday { Id = _nextId++ }; // Присваиваем новый уникальный ID

        Console.Write("Введите имя: ");
        newBirthday.Name = Console.ReadLine();

        Console.Write("Введите дату рождения (yyyy-mm-dd): ");
        newBirthday.DateOfBirth = DateTime.Parse(Console.ReadLine());

        Console.Write("Введите роль: ");
        newBirthday.Role = Console.ReadLine();

        var birthdays = GetBirthdays();
        birthdays.Add(newBirthday);
        SaveBirthdays(birthdays);

        Console.WriteLine("День рождения добавлен.\n");
    }

    private static void EditBirthday()
    {
        var birthdays = GetBirthdays();
        Console.Write("Введите ID для редактирования: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var birthdayToEdit = birthdays.FirstOrDefault(b => b.Id == id);

            if (birthdayToEdit != null)
            {
                Console.Write("Введите новое имя: ");
                birthdayToEdit.Name = Console.ReadLine();

                Console.Write("Введите новую дату рождения (yyyy-mm-dd): ");
                birthdayToEdit.DateOfBirth = DateTime.Parse(Console.ReadLine());

                Console.Write("Введите новую роль: ");
                birthdayToEdit.Role = Console.ReadLine();

                SaveBirthdays(birthdays);
                Console.WriteLine("День рождения обновлен.\n");
            }
            else
            {
                Console.WriteLine("День рождения не найден.\n");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ввод ID.\n");
        }
    }

    private static void DeleteBirthday()
    {
        var birthdays = GetBirthdays();
        Console.Write("Введите ID для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var birthdayToDelete = birthdays.FirstOrDefault(b => b.Id == id);

            if (birthdayToDelete != null)
            {
                birthdays.Remove(birthdayToDelete);
                SaveBirthdays(birthdays);
                Console.WriteLine("День рождения удален.\n");
            }
            else
            {
                Console.WriteLine("День рождения не найден.\n");
            }
        }
        else
        {
            Console.WriteLine("Некорректный ввод ID.\n");
        }
    }
}