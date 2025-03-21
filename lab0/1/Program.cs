// See https://aka.ms/new-console-template for more information

try {
  int length = args.Count();
  if (length == 0) {
    Console.WriteLine("Wprowadź argumeny w linii poleceń!");
    return;
  }
  int amount = Int32.Parse(args[length - 1]);
  for (int j = 0; j < amount; j++) {
    for (int i = 0; i < length - 1; i++) {
      Console.WriteLine(args[i]);
    }
  }
} catch (FormatException e) {
  Console.WriteLine(e);
}