// See https://aka.ms/new-console-template for more information
string? last = null;

StreamWriter sw = new StreamWriter("zad2_result.txt", append: true);
do {
  string? input = Console.ReadLine();
  if (input == null) {
    Console.WriteLine("Read null result");
    continue;
  }
  
  if (input == "koniec!") break;

  sw.WriteLine(input);
  if (last == null || String.Compare(input, last) > 0) {
    last = input;
  }
} while (true);
sw.Close();
Console.WriteLine(String.Format("Ostatni w porządku leksykograficznym: {0}", last));