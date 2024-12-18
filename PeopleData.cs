using System;
using System.Collections.Generic;
using System.IO;

namespace PeopleData
{
    public class Person
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public double Height { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Person> people = new List<Person>
            {
                new Person { LastName = "Иванов", FirstName = "Иван", Gender = "М", Height = 180 },
                new Person { LastName = "Сидорова", FirstName = "Мария", Gender = "Ж", Height = 165 },
                new Person { LastName = "Петров", FirstName = "Петр", Gender = "М", Height = 175 },
                new Person { LastName = "Иванова", FirstName = "Елена", Gender = "Ж", Height = 170 },
                new Person { LastName = "Смирнов", FirstName = "Алексей", Gender = "М", Height = 190 },
                new Person { LastName = "Кузнецова", FirstName = "Анна", Gender = "Ж", Height = 160 }
            };

            SaveToCsv("people_data.csv", people);

            List<Person> loadedPeople = LoadFromCsv("people_data.csv");

            CalculateAndDisplayStats(loadedPeople);
        }

        static void SaveToCsv(string fileName, List<Person> people)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine("LastName,FirstName,Gender,Height");
                foreach (var person in people)
                {
                    writer.WriteLine($"{person.LastName},{person.FirstName},{person.Gender},{person.Height}");
                }
            }
        }

        static List<Person> LoadFromCsv(string fileName)
        {
            List<Person> people = new List<Person>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                reader.ReadLine(); 
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    people.Add(new Person
                    {
                        LastName = values[0],
                        FirstName = values[1],
                        Gender = values[2],
                        Height = double.Parse(values[3])
                    });
                }
            }
            return people;
        }

        static void CalculateAndDisplayStats(List<Person> people)
        {
            double totalMaleHeight = 0, totalFemaleHeight = 0;
            int maleCount = 0, femaleCount = 0;
            Person tallestMale = null, tallestFemale = null;

            foreach (var person in people)
            {
                if (person.Gender == "М")
                {
                    totalMaleHeight += person.Height;
                    maleCount++;

                    if (tallestMale == null || person.Height > tallestMale.Height)
                    {
                        tallestMale = person;
                    }
                }
                else if (person.Gender == "Ж")
                {
                    totalFemaleHeight += person.Height;
                    femaleCount++;

                    if (tallestFemale == null || person.Height > tallestFemale.Height)
                    {
                        tallestFemale = person;
                    }
                }
            }

            double averageMaleHeight = maleCount > 0 ? totalMaleHeight / maleCount : 0;
            double averageFemaleHeight = femaleCount > 0 ? totalFemaleHeight / femaleCount : 0;
            Console.WriteLine($"Средний рост мужчин: {averageMaleHeight:F2} см");
            Console.WriteLine($"Средний рост женщин: {averageFemaleHeight:F2} см");

            if (tallestMale != null)
            {
                Console.WriteLine($"Самый высокий мужчина: {tallestMale.LastName} {tallestMale.FirstName}, рост {tallestMale.Height} см");
            }

            if (tallestFemale != null)
            {
                Console.WriteLine($"Самая высокая женщина: {tallestFemale.LastName} {tallestFemale.FirstName}, рост {tallestFemale.Height} см");
            }
        }
    }
}
