# LogInterpreter WinForms - Pipeline Integration

## Áttekintés

A LogInterpreter WinForms alkalmazás egy grafikus felületet biztosít log fájlok elemzéséhez és megjelenítéséhez. Az alkalmazás a LogInterpreter.Abstractions könyvtárban definiált Pipeline architektúrát használja az adatok feldolgozásához.

## Fõbb funkciók

### ? Implementált funkciók

1. **Log fájl megnyitása**
   - File ? Open Log File (Ctrl+O)
   - Támogatott formátumok: .log, .txt
   - Automatikus Pipeline feldolgozás

2. **Pipeline integráció**
   - CompactJsonLogParser használata
   - Automatikus szûrés (Rental státusz üzenetek)
   - Counter és ErrorAggregator statisztikák
   - LogEntryCollector eredmények gyûjtése

3. **DataGridView megjelenítés**
   - 9 oszlop (Time, Level, LineId, Category, PF, Op, Status, Duration, Message)
   - Automatikus sor színezés log szint alapján
   - Teljes sor kijelölés
   - Csak olvasható mód

4. **Frissítés (F5)**
   - Jelenleg betöltött fájl újra feldolgozása
   - Hasznos élõ log fájlok követéséhez

5. **Törlés (Ctrl+L)**
   - Összes megjelenített bejegyzés törlése
   - DataGridView kiürítése

6. **Export funkció (Ctrl+E)**
   - CSV formátum (Excel kompatibilis)
   - Text formátum (ember által olvasható)
   - Automatikus fájlnév generálás

7. **Státusz információk**
   - Betöltött bejegyzések száma
   - Összes feldolgozott bejegyzés
   - Hibák és figyelmeztetések száma

8. **UI elemek**
   - Menu bar (File, View, Tools, Help)
   - Toolbar (ikonokkal)
   - Status bar (státusz és progress bar)

## Architektúra

### Pipeline folyamat

```
???????????????
?  LogSource  ? - Log fájl kiválasztása
???????????????
       ?
????????????????????????
?OneLineLogFileReader  ? - Soronkénti olvasás
????????????????????????
       ?
???????????????????????
?CompactJsonLogParser ? - JSON parseolás
???????????????????????
       ?
???????????????
?   Counter   ? - Statisztikák gyûjtése
???????????????
       ?
??????????????????????
? ErrorAggregator    ? - Hibák aggregálása
??????????????????????
       ?
???????????????
?   Filter    ? - Rental üzenetek szûrése
???????????????
       ?
???????????????????????
?LogEntryCollector    ? - Eredmények gyûjtése
???????????????????????
       ?
???????????????????
?  DataGridView   ? - Megjelenítés
???????????????????
```

### Adatkötés

```
BindingList<ILogEntry> ?? BindingSource ?? DataGridView
        ?
   logEntries
   (adatforrás)
```

## Használat

### Alapvetõ workflow

1. **Alkalmazás indítása**
   ```bash
   dotnet run --project LogInterpreter.WinForms
   ```

2. **Log fájl megnyitása**
   - File ? Open Log File vagy Ctrl+O
   - Válassz ki egy .log vagy .txt fájlt
   - A Pipeline automatikusan feldolgozza az adatokat

3. **Eredmények megtekintése**
   - A DataGridView megjeleníti a szûrt bejegyzéseket
   - Színkódolt sorok a log szint alapján
   - Státusz bar mutatja a statisztikákat

4. **Export (opcionális)**
   - File ? Export vagy Ctrl+E
   - Válassz formátumot (CSV vagy Text)
   - Mentsd el a fájlt

### Gyorsbillentyûk

| Billentyû | Funkció |
|-----------|---------|
| Ctrl+O | Log fájl megnyitása |
| F5 | Frissítés |
| Ctrl+L | Törlés |
| Ctrl+E | Export |
| Alt+F4 | Kilépés |

## Konfiguráció

### Pipeline szûrõ testreszabása

A `MainForm.cs` fájlban a `LoadFromPipeline` metódusban:

```csharp
.AddItem(new Filter<LogEntry>(e =>
{
    // Egyedi szûrési logika
    if (e.Message.StartsWith("Updating rental status"))
        return true;
    if (e.Message.StartsWith("Rental {RentalId} started"))
        return true;
    return false;
}))
```

### Oszlopok testreszabása

A `MainForm.cs` fájlban a `SetupColumns` metódusban módosítható:
- Oszlopok sorrendje
- Oszlopok szélessége
- Formázás
- Láthatóság

## Projekt struktúra

```
LogInterpreter.WinForms/
??? MainForm.cs                    - Fõablak logika
??? MainForm.Designer.cs           - Fõablak UI design
??? MainForm.resx                  - Erõforrások
??? IconGenerator.cs               - Toolbar ikonok generálása
??? PipelineHelpers.cs             - Counter és ErrorAggregator
??? LogEntryCollector.cs           - Pipeline eredmények gyûjtése
??? DataGridView_Hasznalat.md      - Részletes dokumentáció
??? README.md                      - Ez a fájl
```

## Függõségek

- **LogInterpreter.Abstractions**: Pipeline architektúra és alaposztályok
- **System.Drawing.Common**: Ikonok generálásához
- **.NET 9.0**: Cél framework

## Fejlesztési lehetõségek

### Rövid távon
- [ ] Aszinkron fájl betöltés (UI nem fagy be nagy fájloknál)
- [ ] Konfigurálható szûrõk (UI-ból szerkeszthetõ)
- [ ] Részletes nézet (Properties megjelenítése)
- [ ] Keresés funkcionalitás

### Közép távon
- [ ] Több fájl egyidejû betöltése
- [ ] Összefoglaló statisztikák ablak
- [ ] Mentés/Betöltés (Pipeline konfiguráció)
- [ ] Élõ log követés (tail -f szerû funkció)

### Hosszú távon
- [ ] Bõvítmény rendszer (plugins)
- [ ] Diagram nézetek (idõsor, pie chart)
- [ ] Log fájl összehasonlítás
- [ ] Regex alapú szûrõk

## Verzió történet

### v1.0.0 (aktuális)
- ? Alapvetõ Pipeline integráció
- ? DataGridView megjelenítés
- ? Szûrési funkciók
- ? Export (CSV, Text)
- ? Frissítés és törlés
- ? Státusz információk

## Licenc

Lásd a projekt gyökér könyvtárában a LICENSE fájlt.

## Kapcsolat

Hibák és javaslatok: https://github.com/kavics/LogInterpreter/issues
