// See https://aka.ms/new-console-template for more information
using System.Globalization;

CultureInfo c = CultureInfo.GetCultureInfo("en-US");
Thread.CurrentThread.CurrentCulture = c;
Thread.CurrentThread.CurrentUICulture = c;

float suma = 0;
int ilosc = 0;

do {
  string? input = Console.ReadLine();
  if (input == null) {
    Console.WriteLine("Read null result");
    continue;
  }
  try {
    float inputNumber = float.Parse(input);
    if (inputNumber == 0) break;
    suma += inputNumber;
    ilosc += 1;
  } catch (FormatException _) {
    Console.WriteLine("Failed to parse the number");
    continue;
  }
} while (true);

float srednia = suma / ilosc;
Console.WriteLine(String.Format("Średnia: {0}", srednia));
StreamWriter sw = new StreamWriter("zad2_result.txt", append: true);
sw.WriteLine(String.Format("Średnia: {0}\n", srednia));
sw.Close();