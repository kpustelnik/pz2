using System.Text.Json;
using System.Xml.Serialization;
using System.Globalization;
using System.Text.RegularExpressions;

CultureInfo c = CultureInfo.GetCultureInfo("en-UK");
Thread.CurrentThread.CurrentCulture = c;
Thread.CurrentThread.CurrentUICulture = c;

// Zadanie 1
StreamReader sr = new StreamReader("favorite-tweets.jsonl");
List<Tweet> tweetList = new List<Tweet>();
while (!sr.EndOfStream)
{
    String line = sr.ReadLine()!;
    Tweet? tweet = JsonSerializer.Deserialize<Tweet>(line);
    if (tweet != null) tweetList.Add(tweet);
}
sr.Close();
Console.WriteLine(tweetList[0].ToString());
Console.WriteLine(tweetList.Count);

// Zadanie 2
void xmlSaveLoad(Boolean save) {
    XmlSerializer serializer = new XmlSerializer(tweetList.GetType());
    if (save) {
        using (StreamWriter writer = File.CreateText("tweets.xml")) {
            serializer.Serialize(writer, tweetList);
        }
    } else {
        using (StreamReader reader = new StreamReader("tweets.xml")) {
            tweetList = (List<Tweet>) serializer.Deserialize(reader)!;
        }
        Console.WriteLine("Load result:");
        Console.WriteLine(tweetList[0].ToString());
        Console.WriteLine(tweetList.Count);
    }
}

xmlSaveLoad(true);
xmlSaveLoad(false);

// Zadanie 3
int CompareNames(Tweet? t1, Tweet? t2) {
    return (t1?.UserName ?? "").CompareTo(t2?.UserName ?? "");
}
int CompareDates(Tweet? t1, Tweet? t2) {
    // or using format "MMMM dd, yyyy 'at' hh:mmtt"
    DateTime? t1_date = DateTime.Parse((t1?.CreatedAt ?? "").Replace("at ", ""));
    DateTime? t2_date = DateTime.Parse((t2?.CreatedAt ?? "").Replace("at ", ""));
    return t1_date.Value.Date.CompareTo(t2_date.Value.Date);
}

Console.WriteLine("Posortowanie listy po nazwach użytkownika i datach tweetów");
tweetList.Sort(CompareNames);
Console.WriteLine(tweetList[0].UserName);
tweetList.Sort(CompareDates);
Console.WriteLine(tweetList[0].CreatedAt);

// 04
Console.WriteLine("Najnowszy i najstarszy tweet");
Console.WriteLine(tweetList[0]);
Console.WriteLine(tweetList[tweetList.Count - 1]);

// 5
SortedDictionary<String, List<Tweet>> userTweets = new SortedDictionary<String, List<Tweet>>();
foreach (Tweet tweet in tweetList) {
    String? userName = tweet?.UserName;
    if (userName == null || tweet == null) continue;
    if (userTweets.ContainsKey(userName)) {
        userTweets[userName].Add(tweet);
    } else {
        List<Tweet> t = new List<Tweet>();
        t.Add(tweet);
        userTweets.Add(userName, t);
    }
}

Console.WriteLine("\nTweety użytkownika michaelharriot");
foreach (Tweet tweet in userTweets["michaelharriot"]){
    Console.WriteLine(tweet);
}

// 6
Regex wordRegex = new Regex(@"[A-Z]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
SortedDictionary<String, int> wordFreq = new SortedDictionary<String, int>();

foreach (Tweet tweet in tweetList) {
    MatchCollection tweetWords = wordRegex.Matches(tweet?.Text?.ToUpper() ?? "");
    foreach (Match word in tweetWords) {
        string key = word.ToString();
        if (!wordFreq.ContainsKey(key)) wordFreq.Add(key, 0);
        wordFreq[key] += 1;
    }
}

void writeWordFreq(string key) {
    string upperKey = key.ToUpper();
    int count = 0;
    wordFreq.TryGetValue(upperKey, out count);
    Console.WriteLine(string.Format("Występowania słowa `{0}` - {1}", key, count));
}
Console.WriteLine("\nCzęstotliwość występowania słów");
writeWordFreq("Marvel");
writeWordFreq("watching");
writeWordFreq("Disney");
writeWordFreq("Poland");

// 7
List<string> sortedWords = new List<string>();
foreach (string word in wordFreq.Keys) sortedWords.Add(word);

sortedWords.Sort( // Sort the words descending by frequency
    (string a, string b) => {
        // Get values
        int val_a = wordFreq[a];
        int val_b = wordFreq[b];

        if (val_a == val_b) return a.CompareTo(b); // Compare keys if value is the same
        return -val_a.CompareTo(val_b); // Compare the values (in descending order)
    }
);

int am = 0;
int lastcount = 0;

int limit = 10;
int min_length = 5;
Console.WriteLine("\nNajczęstsze >= 10 słów");
foreach (string word in sortedWords) {
    if (word.Length >= min_length) {
        if (am >= limit && wordFreq[word] < lastcount) break;
        Console.WriteLine(string.Format("{0} {1}", word, wordFreq[word]));
        lastcount = wordFreq[word];
        am += 1;
    }
}

// 8
Console.WriteLine("\n10 słów o największym IDF");
SortedDictionary<String, int> idfWordTweetCount = new SortedDictionary<String, int>();
SortedDictionary<String, double> idfWord = new SortedDictionary<String, double>();

foreach (Tweet tweet in tweetList) {
    List<string> handledWords = new List<string>();
    MatchCollection tweetWords = wordRegex.Matches(tweet?.Text?.ToUpper() ?? "");
    foreach (Match word in tweetWords) {
        string key = word.ToString();
        
        if (!handledWords.Contains(key)) {
            handledWords.Add(key);
            if (!idfWordTweetCount.ContainsKey(key)) idfWordTweetCount.Add(key, 0);
            idfWordTweetCount[key] += 1;
        }
    }
}
foreach (string word in idfWordTweetCount.Keys) idfWord[word] = Math.Log(tweetList.Count / (double) idfWordTweetCount[word]);

List<string> sortedIdfWords = new List<string>();
foreach (string word in idfWord.Keys) sortedIdfWords.Add(word);

sortedIdfWords.Sort(
    (string a, string b) => {
        // Get values
        double val_a = idfWord[a];
        double val_b = idfWord[b];

        if (val_a == val_b) return a.CompareTo(b); // Compare keys if value is the same
        return -val_a.CompareTo(val_b); // Compare the values (in descending order)
    }
);

am = 0;
double lastidf = 0;
limit = 10;
foreach (string word in sortedIdfWords) {
    if (am >= limit) break; // && idfWord[word] < lastidf) break;
    Console.WriteLine(string.Format("{0} {1}", word, idfWord[word]));
    lastidf = idfWord[word];
    am += 1;
}


public class Tweet {
    public String? CreatedAt { get; set; }
    public String? FirstLinkUrl {get; set;}
    public String? LinkToTweet {get; set;}
    public String? Text {get; set;}
    public String? TweetEmbedCode {get; set;}
    public String? UserName {get; set;}

    public override String ToString() {
        return String.Format("{0} - @{1} ({2})", Text, UserName, CreatedAt);
    }
}