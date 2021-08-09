using System;
using System.Collections.Generic;
using System.IO;

namespace maganora
{
  class Program
  {
    static char[,] BeolvasFoglaltsag(string fajlNev, int sorSzam, int oszlopSzam)
    {
      char[,] foglaltsagTomb = new char[sorSzam, oszlopSzam];
      using (StreamReader sr = new StreamReader(fajlNev))
      {
        int szamlalo = 0;
        while (!sr.EndOfStream)
        {
          string sor = sr.ReadLine();
          for (int i = 0; i < sor.Length; i++)
          {
            foglaltsagTomb[szamlalo, i] = sor[i];
          }
          szamlalo++;
        }
      }
      return foglaltsagTomb;
    }
    static int[,] BeolvasKategoria(string fajlNev, int sorSzam, int oszlopSzam)
    {
      int[,] kategoriaTomb = new int[sorSzam, oszlopSzam];
      using (StreamReader sr = new StreamReader(fajlNev))
      {
        int szamlalo = 0;
        while (!sr.EndOfStream)
        {
          string sor = sr.ReadLine();
          for (int i = 0; i < sor.Length; i++)
          {
            kategoriaTomb[szamlalo, i] = Convert.ToInt32(char.GetNumericValue(sor[i])); // Azért convert-álunk, mert szám karaktereket szeretnénk egészként.
          }
          szamlalo++;
        }
      }
      return kategoriaTomb;
    }
    static void FoglaltE(char[,] tomb)
    {
      Console.Write("A keresett szék sora: ");
      int szekSor = Convert.ToInt32(Console.ReadLine());
      Console.Write("A keresett szék oszlopa: ");
      int szekOszlop = Convert.ToInt32(Console.ReadLine());

      if (tomb[szekSor - 1, szekOszlop - 1] == 'x')
      {
        Console.WriteLine("Ez a szék foglalt!");
      }
      else
      {
        Console.WriteLine("Ez a szék szabad!");
      }
    }
    static void GetEladasiStatisztika(char[,] tomb)
    {
      double foglalt = 0;
      for (int sor = 0; sor < 15; sor++)
      {
        for (int oszlop = 0; oszlop < 20; oszlop++)
        {
          if (tomb[sor, oszlop] == 'x')
          {
            foglalt++;
          }
        }
      }
      double szazalek = foglalt / tomb.Length * 100;
      Console.WriteLine($"Az előadáson {foglalt} jegyet adtak el, ami a nézőtér {szazalek.ToString("00")} százaléka.");
    }
    static int MaxKategoria(Dictionary<int, int> stat)
    {
      int max = int.MinValue;
      int maxkey = 0;
      foreach (var kulcsErtekPar in stat)
      {
        if (kulcsErtekPar.Value > max)
        {
          max = kulcsErtekPar.Value;
          maxkey = kulcsErtekPar.Key;
        }
      }
      return maxkey;
    }
    static Dictionary<int, int> GetKategoriaStatisztika(int[,] tomb)
    {
      // Dictionary adatszerkezet - kulcs-érték párokat tartalmaz

      Dictionary<int, int> kategoriankent = new Dictionary<int, int>();
      for (int sor = 0; sor < 15; sor++)
      {
        for (int oszlop = 0; oszlop < 20; oszlop++) // 2d tömbön végigiterálni
        {
          if (kategoriankent.ContainsKey(tomb[sor, oszlop])) // Megvizsgáljuk, hogy a kulcs szerepel-e már a Dictionary-ban
          {
            kategoriankent[tomb[sor, oszlop]]++; // Ha igen, akkor az adott kulcs értékét (value), növeljük egyel
          }
          else
          {
            kategoriankent.Add(tomb[sor, oszlop], 1); // Különben hozzáadunk a dictionary-hoz egy új kulcsot, és 1 értéket adjuk neki
          }
        }
      }
      return kategoriankent;
    }
    static int OsszBevetel(Dictionary<int,int> stat)
    {
      int osszeg = 0;
      foreach (var kulcsErtekPar in stat)
      {
        if (kulcsErtekPar.Key == 1) osszeg += kulcsErtekPar.Value * 5000;
        else if (kulcsErtekPar.Key == 2) osszeg += kulcsErtekPar.Value * 4000;
        else if (kulcsErtekPar.Key == 3) osszeg += kulcsErtekPar.Value * 3000;
        else if (kulcsErtekPar.Key == 4) osszeg += kulcsErtekPar.Value * 2000;
        else if (kulcsErtekPar.Key == 5) osszeg += kulcsErtekPar.Value * 1500;
      }
      return osszeg;
    }
    static int GetEgyedulalloSzekekSzama(char[,] tomb)
    {
      int egyedulallo = 0;
      // GetLenght(dimenzio): Visszaadja a meghatarozott dimenzioban a tomb hosszat (tobb-dimenzios tomb eseten)
      for(int i = 0; i < tomb.GetLength(0); i++)
      {
        for(int j = 1; j < tomb.GetLength(1)-1; j++)
        {
          if(tomb[i,j] == 'o' && tomb[i,j+1] == 'x' && tomb[i,j-1] == 'x'){
            egyedulallo++;
          }
        }
      }
      for(int i = 0; i < tomb.GetLength(0); i++)
      {
        int balSzel = tomb[i,0];
        int jobbSzel = tomb[i, tomb.GetLength(1)-1];
        if(balSzel == 'o' && tomb[i,1] == 'x') egyedulallo++;
        if(jobbSzel == 'o' && tomb[i,tomb.GetLength(1)-2] == 'x') egyedulallo++;
      }
      return egyedulallo;
    }
    static void Main(string[] args)
    {
      char[,] foglaltsagTomb = BeolvasFoglaltsag("foglaltsag.txt", 15, 20);
      int[,] kategoriaTomb = BeolvasKategoria("kategoria.txt", 15, 20);

      FoglaltE(foglaltsagTomb);
      GetEladasiStatisztika(foglaltsagTomb);

      Dictionary<int,int> kategoriaStat = GetKategoriaStatisztika(kategoriaTomb);
      int maxkey = MaxKategoria(kategoriaStat);
      Console.WriteLine($"A legtöbb jegyet a(z) {maxkey} árkategóriában értékesítették.");

      int osszbevetel = OsszBevetel(kategoriaStat);
      Console.WriteLine($"A szinhaz osszbevetele: {osszbevetel} Ft.");

      int egyeduliSzekekSzama = GetEgyedulalloSzekekSzama(foglaltsagTomb);
      Console.WriteLine($"Az egyedulallo szekek szama: {egyeduliSyekekSzama}db.");

      //Hazi: Keszits egy metodust, ami fajlba irassal megoldja a 7. feladatot!
      //Faljbairas: StreamWriter + using szerkezet https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter?view=netcore-3.1

      //Szorgalmi feladat: Megoldani, hogy leellenorizze, hogy letezik-e a fajl
    }
  }
}
