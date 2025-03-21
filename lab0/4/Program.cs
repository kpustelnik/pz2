// See https://aka.ms/new-console-template for more information
string[] notes = {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "B", "H"};
int notesLen = notes.Length;
int[] nextNotes = {2, 2, 1, 2, 2, 2, 1};

string? input = Console.ReadLine();
if (input == null) {
  Console.WriteLine("Failed to read the note.");
  return;
}

int pos = Array.IndexOf(notes, input);
if (pos == -1) {
  Console.WriteLine("Invalid note.");
  return;
}

foreach (int j in nextNotes) {
  Console.Write(string.Format("{0} ", notes[pos]));
  pos = (pos + j) % notesLen;
}
Console.Write(string.Format("{0} ", notes[pos]));