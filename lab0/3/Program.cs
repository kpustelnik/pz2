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
  Console.WriteLine("Nie udało się otworzyć pliku");
  return;
}
int? maxLine = null;
float? maxValue = null;
for (int i = 1; !sr.EndOfStream; i++) {
  string? napis = sr.ReadLine();
  if (napis == null) continue;
  try {
    float liczba = float.Parse(napis);
    if (maxValue == null || liczba > maxValue) {
      maxValue = liczba;
      maxLine = i;
    }
  } catch (FormatException _) {
    Console.WriteLine(string.Format("Nieprawidłowa wartość na linii {0}", i));
  }
}
sr.Close();

if (maxLine != null) {
  Console.WriteLine(string.Format("{0}, linijka: {1}", maxValue, maxLine));
} else {
  Console.WriteLine("Brak liczb!");
}