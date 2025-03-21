// See https://aka.ms/new-console-template for more information
using System.Globalization;

CultureInfo c = CultureInfo.GetCultureInfo("en-US");
Thread.CurrentThread.CurrentCulture = c;
Thread.CurrentThread.CurrentUICulture = c;

if (args.Count() == 0) {
  Console.WriteLine("Wprowadź jako argument nazwę pliku!");
  return;
}

StreamReader sr;
try {
  sr = new StreamReader(args[0]);
} catch (FileNotFoundException _) {
  Console.WriteLine("Nie udało się otworzyć pliku.");
  return;
}
int line = 1;
int liczbaZnakow = 0;

double? maxLiczba = null;
double? minLiczba = null;

double suma = 0;
int ilosc = 0;
for (; !sr.EndOfStream; line++) {
  string? napis = sr.ReadLine();
  if (napis == null) continue;

  liczbaZnakow += napis.Length;
  try {
    double liczba = double.Parse(napis);
    suma += liczba;
    ilosc += 1;

    if (minLiczba == null || liczba < minLiczba) minLiczba = liczba;
    if (maxLiczba == null || liczba > maxLiczba) maxLiczba = liczba;
  } catch (FormatException e) {
    Console.WriteLine(string.Format("Nieprawidłowa wartość na linii {0}", line));
  }
}
sr.Close();

Console.WriteLine("Liczba linii: {0}", line - 1);
Console.WriteLine("Liczba znaków: {0}", liczbaZnakow);
Console.WriteLine("Największa liczba: {0}", maxLiczba);
Console.WriteLine("Najmniejsza liczba: {0}", minLiczba);
Console.WriteLine("Średnia liczb: {0}", suma / ilosc);