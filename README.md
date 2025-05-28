# AppCollection
Školní projekt pro 3. ročník. Obsahuje užitečné nástroje, osobní úložiště dokumentů a napojení na databázi.

## Instalace projektu:

1. Naklonujte repozitář - git clone či v IDE.
2. dotnet restore
3. dotnet ef database update - connection string v appsettings.json a musí být nainstalováno Entity Framework pokud nefunguje použít skript create db.
4. databázi pojmenujte ProgramDataMVC.
5. Spustit na IIS nebo https.

## Funkce

### Odjezdy PID

- Zobrazení aktuálních odjezdů pražské MHD
- Podpora všech zastávek v Praze
- Informace o zpoždění spojů
- Zobrazení nástupiště
- Doplňkové informace (klimatizace, bezbariérovost)
- Responzivní design pro mobily i počítače
- Automatická aktualizace času

### Úložiště dokumentů

- Správa osobních dokumentů.
- Bezpečné nahrávání souborů.
- Kategorizace dokumentů.
- Kontrola digitálních podpisů.

### Databázové funkce

- Napojení na SQL Server
- Perzistentní ukládání dat
- Zabezpečené zpracování dat

### Galerie a přehrávač skladeb
- Soubory z dokumetů se zde zobrazí.
- Lze si přehrát audio soubory.

### Slovní hodiny a počasí
- Zajímavý typ hodin a aktuální počasí.

### Křížovkářský slovník
- Vyhledávání slov v něm a možnost si je uložit.