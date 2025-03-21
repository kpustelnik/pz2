// See https://aka.ms/new-console-template for more information
if (args.Count() == 0) {
  Console.WriteLine("Wprowadź jako argument nazwę pliku oraz szukany ciąg!");
  return;
}
if (args.Count() == 1) {
  Console.WriteLine("Wprowadź szukany ciąg!");
  return;
}

string ciag = args[1];

StreamReader sr;
try {
  sr = new StreamReader(args[0]);
} catch (FileNotFoundException _) {
  Console.WriteLine("Nie udało się otworzyć pliku.");
  return;
}
for (int i = 1; !sr.EndOfStream; i++) {
  string? napis = sr.ReadLine();
  if (napis == null) continue;

  for (int s = 0; s <= napis.Length - ciag.Length; s++) {
    bool matches = true;
    for (int j = 0; j < ciag.Length; j++) {
      if (napis[s + j] != ciag[j]) {
        matches = false;
        break;
      }
    }
    if (matches == true) {
      Console.WriteLine("linijka: {0}, pozycja: {1} do {2}", i, s, s + ciag.Length - 1);
    }
  }
}
sr.Close();