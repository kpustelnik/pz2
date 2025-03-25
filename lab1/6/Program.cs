// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.IO;

CultureInfo c = CultureInfo.GetCultureInfo("en-US");
Thread.CurrentThread.CurrentCulture = c;
Thread.CurrentThread.CurrentUICulture = c;

if (args.Count() == 0) {
  Console.WriteLine("Wprowadź jako argument nazwę pliku!");
  return;
}

static double? ReadNext(StreamReader sr) {
  string? o;
  while ((o = sr.ReadLine()) != null) {
    try {
      return double.Parse(o);
    } catch (FormatException e) {
      Console.WriteLine(string.Format("Nieprawidłowa wartość - {0}", o));
    }
  }
  return null;
}

while (true) {
  {
    StreamReader sr = new StreamReader(args[0]);
    StreamWriter sw_a = new StreamWriter("a.txt");
    StreamWriter sw_b = new StreamWriter("b.txt");
    bool write_sw = false; // false = a; true = b
    bool any_switches = false;
    double? last = null;
    double? liczba = null;
    while ((liczba = ReadNext(sr)) != null) {
      if (last != null && liczba < last) {
        write_sw = !write_sw;
        any_switches = true;
      }
      last = liczba;

      if (write_sw) sw_b.WriteLine(liczba);
      else sw_a.WriteLine(liczba);
    }
    sr.Close();
    sw_a.Close();
    sw_b.Close();
    if (!any_switches) break;
  }

  {
    StreamWriter sw = new StreamWriter(args[0]);
    StreamReader sr_a = new StreamReader("a.txt");
    StreamReader sr_b = new StreamReader("b.txt");
    double? last_a = null;
    double? last_b = null;
    double? cur_a = null; // null = last was handled
    double? cur_b = null; // null = last was handled
    bool sec_fin_a = false;
    bool sec_fin_b = false;
    while (true) {
      // Try to load what's possible
      cur_a = cur_a ?? ReadNext(sr_a);
      cur_b = cur_b ?? ReadNext(sr_b);
      if (cur_a == null && cur_b == null) break; // Nothing more to load - end
      if (cur_a == null || cur_b == null) { // Only one exists - write the remaining section(s) to target
        sw.WriteLine(cur_a ?? cur_b);
        cur_a = null;
        cur_b = null;
        continue; // Retry loop
      }

      // Check sections finish (both cur_a & cur_b are guaranteed to exist)
      if (last_a != null && cur_a < last_a) sec_fin_a = true; // Prev value from a finished section
      last_a = cur_a;
      if (last_b != null && cur_b < last_b) sec_fin_b = true; // Prev value from b finished section
      last_b = cur_b;

      //
      if (sec_fin_a != sec_fin_b) { // One of the sections has finished and other did not
        if (sec_fin_a) { // section from file a has been finished, so we need to finish section b
          sw.WriteLine(cur_b);
          cur_b = null; // mark as processed
        } else { // section from file b has therefore been finished, so we need to finish the a one
          sw.WriteLine(cur_a);
          cur_a = null; // mark as processed
        }
      } else {
        if (sec_fin_a) { // Both of the sections have finished
          // Start actually processing new two sections
          sec_fin_a = false;
          sec_fin_b = false;
        }
        // Both sections are unfinished
        if (cur_a < cur_b) { // a goes first
          sw.WriteLine(cur_a);
          cur_a = null;
        } else { // b goes first
          sw.WriteLine(cur_b);
          cur_b = null;
        }
      }
    }
    sw.Close();
    sr_a.Close();
    sr_b.Close();
  }
}
File.Delete("a.txt");
File.Delete("b.txt");