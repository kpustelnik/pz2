// See https://aka.ms/new-console-template for more information
using System.Globalization;

CultureInfo c = CultureInfo.GetCultureInfo("en-US");
Thread.CurrentThread.CurrentCulture = c;
Thread.CurrentThread.CurrentUICulture = c;

int argscnt = args.Count();
if (argscnt == 0) {
  Console.WriteLine("Wprowadź nazwę pliku jako pierwszy argument.");
  return;
}
if (argscnt == 1) {
  Console.WriteLine("Wprowadź liczbę losowych liczb jako drugi argument.");
  return;
}
if (argscnt == 2) {
  Console.WriteLine("Wprowadź początek przedziału losowanych wartości jako trzeci argument.");
  return;
}
if (argscnt == 3) {
  Console.WriteLine("Wprowadź koniec przedziału losowanych wartości jako czwarty argument.");
  return;
}
if (argscnt == 4) {
  Console.WriteLine("Wprowadź seed jako piąty argument.");
  return;
}
if (argscnt == 5) {
  Console.WriteLine("Wprowadź, czy losowane liczby powinny być rzeczywiste (R) [DEFAULT], czy całkowite (C) jako szósty argument.");
  return;
}

string nazwaPliku = args[0];

int n = 0;
try {
  n = Int32.Parse(args[1]);
} catch (FormatException _) {
  Console.WriteLine("Nieprawidłowy format liczby losowanych liczb.");
  return;
}

double poczPrzedzialu;
try {
  poczPrzedzialu = double.Parse(args[2]);
} catch (FormatException _) {
  Console.WriteLine("Nieprawidłowy format początku przedziału.");
  return;
}

double koniecPrzedzialu;
try {
  koniecPrzedzialu = double.Parse(args[3]);
} catch (FormatException _) {
  Console.WriteLine("Nieprawidłowy format końca przedziału.");
  return;
}

double przedzial = koniecPrzedzialu - poczPrzedzialu;
if (przedzial < 0) {
  Console.WriteLine("Koniec powinien być po początku przedziału.");
  return;
}

int seed = 0;
try {
  seed = Int32.Parse(args[4]);
} catch (FormatException _) {
  Console.WriteLine("Nieprawidłowy format seed.");
  return;
}

bool czyRzeczywiste = (args[5] == "C") ? false : true;

StreamWriter sw = new StreamWriter(nazwaPliku);
Random rand = new Random(seed);
for (int i = 0; i < n; i++) {
  if (czyRzeczywiste) {
    double num = rand.NextDouble();
    num = (num * przedzial) + poczPrzedzialu;
    sw.WriteLine(num);
  } else {
    int num = rand.Next((int) Math.Ceiling(poczPrzedzialu), (int) Math.Floor(koniecPrzedzialu) + 1);
    sw.WriteLine(num);
  }
}
sw.Close();