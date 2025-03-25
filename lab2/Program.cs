// See https://aka.ms/new-console-template for more information
public abstract class PosiadaczRachunku {
  public abstract override String ToString();
}

class OsobaFizyczna : PosiadaczRachunku {
  private String _imie;
  public String imie
  { get  { return _imie; } }
  private String _nazwisko;
  public String nazwisko
  { get { return _nazwisko; }}
  private String _drugieImie;
  public String drugieImie
  { get { return _drugieImie; } }
  private string? _PESEL;
  public string? PESEL
  {
    get { return _PESEL; }
    set {
      if (value == null || value.Length != 11) {
        throw new Exception("Nieprawidłowy numer pesel.");
      }
      for (int i = 0; i < value.Length; i++) {
        if (!Char.IsDigit(value[i])) throw new Exception("PESEL musi składać się tylko i wyłącznie z cyfr.");
      }
      _PESEL = PESEL;
    }
  }
  private string? _numerPaszportu;
  public string? numerPaszportu
  { get { return _numerPaszportu; } }

  public override String ToString() {
      return string.Format("Osoba fizyczna - {0}, {1}", imie, nazwisko);
  }

  public OsobaFizyczna(String imie, String nazwisko, String drugieImie = "", String? iPESEL = null, String? numerPaszportu = null) {
    if (iPESEL == null && numerPaszportu == null) {
      throw new Exception("PESEL albo numer paszportu muszą być nie null");
    }
    _imie = imie;
    _nazwisko = nazwisko;
    _drugieImie = drugieImie;
    PESEL = iPESEL;
    _numerPaszportu = numerPaszportu;
  }
}

class OsobaPrawna : PosiadaczRachunku {
  private string _nazwa;
  public string nazwa
  { get { return _nazwa; } }

  private string _siedziba;
  public string siedziba
  { get { return _siedziba; } }

  public override String ToString() {
      return string.Format("Osoba prawna - {0}, {1}", nazwa, siedziba);
  }

  public OsobaPrawna(String nazwa, String siedziba) {
    _nazwa = nazwa;
    _siedziba = siedziba;
  }
}

class RachunekBankowy {
  private string _numer;
  public string numer
  { get { return _numer; } }
  private decimal _stanRachunku;
  public decimal stanRachunku
  { get { return _stanRachunku; } }
  private bool _czyDozwolonyDebet;
  public bool czyDozwolonyDebet
  { get { return _czyDozwolonyDebet; } }
  private List<PosiadaczRachunku> _posiadaczeRachunku = new List<PosiadaczRachunku>();
  public List<PosiadaczRachunku> posiadaczeRachunku
  { get { return _posiadaczeRachunku; } }

  private List<Transakcja> _Transakcje = new List<Transakcja>();
  public List<Transakcja> Transakcje
  { get { return _Transakcje; } }

  public RachunekBankowy(string numer, decimal stanRachunku, bool czyDozwolonyDebet, List<PosiadaczRachunku> posiadaczeRachunku) {
    if (posiadaczeRachunku == null || posiadaczeRachunku.Count == 0) {
      throw new Exception("Nieprawidłowa lista posiadaczy rachunku");
    }
    _numer = numer;
    _stanRachunku = stanRachunku;
    _czyDozwolonyDebet = czyDozwolonyDebet;

    foreach (PosiadaczRachunku p in posiadaczeRachunku) {
      _posiadaczeRachunku.Add(p);
    }
  }

  public static void DokonajTransakcji(RachunekBankowy? zrodlowy, RachunekBankowy? docelowy, decimal kwota, string opis) {
    if (kwota < 0) {
      throw new Exception("Kwota nie może być ujemna");
    }
    if (zrodlowy == null && docelowy == null) {
      throw new Exception("Rachunek źródłowy i docelowy nie istnieje.");
    }
    if (zrodlowy != null && kwota > zrodlowy._stanRachunku && !zrodlowy._czyDozwolonyDebet) {
      throw new Exception("Kwota przekracza stan rachunku źródłowego, który nie pozwala na debet.");
    }

    Transakcja transakcja = new Transakcja(zrodlowy, docelowy, kwota, opis);
    if (zrodlowy != null) {
      zrodlowy._stanRachunku -= kwota;
      zrodlowy._Transakcje.Add(transakcja);
    }
    if (docelowy != null) {
      docelowy._stanRachunku += kwota;
      docelowy._Transakcje.Add(transakcja);
    }
  }

  public static RachunekBankowy operator + (RachunekBankowy rach, PosiadaczRachunku pos) {
    if (rach._posiadaczeRachunku.Contains(pos)) {
      throw new Exception("Osoba jest już na liście posiadaczy rachunku.");
    }
    rach._posiadaczeRachunku.Add(pos);
    return rach;
  }
  public static RachunekBankowy operator - (RachunekBankowy rach, PosiadaczRachunku pos) {
    if (!rach._posiadaczeRachunku.Contains(pos)) {
      throw new Exception("Osoba nie jest na liście posiadaczy rachunku.");
    }
    if (rach._posiadaczeRachunku.Count <= 1) {
      throw new Exception("Nie można usunąć jedynego posiadacza rachunku.");
    }
    rach._posiadaczeRachunku.Remove(pos);
    return rach;
  }

  public override string ToString() {
    string info = string.Format("{0} Stan: {1}\n", _numer, _stanRachunku);
    info += "Posiadacze:\n";
    foreach (PosiadaczRachunku p in _posiadaczeRachunku) {
      info += p.ToString() + "\n";
    }
    info += "Transakcje:\n";
    foreach (Transakcja t in _Transakcje) {
      info += t.ToString() + "\n";
    }
    return info;
  }
}

class Transakcja {
  private RachunekBankowy? _rachunekZrodlowy;
  public RachunekBankowy? rachunekZrodlowy
  { get { return _rachunekZrodlowy; } }
  private RachunekBankowy? _rachunekDocelowy;
  public RachunekBankowy? rachunekDocelowy
  { get { return _rachunekDocelowy; } }
  private decimal _kwota;
  public decimal kwota
  { get { return _kwota; } }
  private string _opis;
  public string opis
  { get { return _opis; } }

  public Transakcja(RachunekBankowy? zrodlowy, RachunekBankowy? docelowy, decimal kwota, string opis) {
    if (zrodlowy == null && docelowy == null) {
      throw new Exception("Rachunki nie zostały wyspecyfikowane.");
    }
    if (zrodlowy == docelowy) {
      throw new Exception("Rachunki nie mogą być takie same.");
    }
    _rachunekZrodlowy = zrodlowy;
    _rachunekDocelowy = docelowy;
    _kwota = kwota;
    _opis = opis;
  }
  
  public override String ToString() {
    return string.Format("{0} -> {1} | {2} ({3})", (_rachunekZrodlowy != null) ? _rachunekZrodlowy.numer : '-', (_rachunekDocelowy != null) ? _rachunekDocelowy.numer : '-', kwota, opis);
  }

  static void Main(string[] args) {
    // 2
    Console.WriteLine("Zadanie 2");
    OsobaFizyczna osFiz = new OsobaFizyczna("Kamil", "Pustelnik", "B", "12312312312");
    Console.WriteLine(osFiz);

    // 3
    Console.WriteLine("Zadanie 3");
    // PosiadaczRachunku osFizNoPeselPaszport = new OsobaFizyczna("Kamil", "Pustelnik", "B");


    // 4, 5
    Console.WriteLine("Zadanie 4,5");
    PosiadaczRachunku osPraw = new OsobaPrawna("Firma 1", "AGH");
    Console.WriteLine(osPraw);

    // 7
    Console.WriteLine("Zadanie 7");
    List<PosiadaczRachunku> posiadacze = new List<PosiadaczRachunku>();
    // RachunekBankowy rachFail = new RachunekBankowy("12345", 15, false, posiadacze);

    posiadacze.Add(osPraw);
    RachunekBankowy rach = new RachunekBankowy("12345", 15, false, posiadacze);
    RachunekBankowy rach2 = new RachunekBankowy("321", 10, true, posiadacze);
    
    // 8
    Console.WriteLine("Zadanie 8");
    // Transakcja transFail = new Transakcja(null, null, 15, "testowy");

    // 10
    Console.WriteLine("Zadanie 10");
    RachunekBankowy.DokonajTransakcji(null, rach, 10, "Test 1");
    Console.WriteLine(rach);
    // RachunekBankowy.DokonajTransakcji(rach, rach2, 30, "Test fail (debet)");
    RachunekBankowy.DokonajTransakcji(rach2, rach, 30, "Test debet");
    Console.WriteLine(rach);
    Console.WriteLine(rach2);
    RachunekBankowy.DokonajTransakcji(rach2, null, 100, "Test wyplata");
    Console.WriteLine(rach2);

    // dodatkowe (Console.WriteLine(rachunek))
    rach2 += osFiz;
    Console.WriteLine(rach2);
    rach2 -= osPraw;
    Console.WriteLine(rach2);
    // rach2 -= osFiz;

    // osFiz.PESEL = "123123123123"; // fail
  }
}