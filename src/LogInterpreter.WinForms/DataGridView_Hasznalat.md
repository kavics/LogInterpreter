# DataGridView használata a LogInterpreter alkalmazásban

## Áttekintés

A MainForm tartalmaz egy DataGridView komponenst, amely ILogEntry objektumokat jelenít meg. Az adatok egy Pipeline feldolgozási láncból töltõdnek be, amely különbözõ szûréseket és transzformációkat végez a log fájlokon.

## Pipeline integráció

### Architektúra

```
LogSource ? OneLineLogFileReader ? CompactJsonLogParser ? Counter ? ErrorAggregator ? Filter ? LogEntryCollector ? DataGridView
```

### Pipeline elemek

1. **LogSource**: Log fájl beolvasása
2. **OneLineLogFileReader**: Soronkénti olvasás
3. **CompactJsonLogParser**: Compact JSON formátum parseolása
4. **Counter**: Statisztikák gyûjtése
5. **ErrorAggregator**: Hibák és figyelmeztetések aggregálása
6. **Filter**: Szûrés (Rental státusz üzenetek)
7. **LogEntryCollector**: Eredmények összegyûjtése

### Szûrési logika

A Pipeline jelenleg a következõ üzeneteket szûri ki:
- "Updating rental status"
- "Rental {RentalId} started for user {UserEmail}"
- "Rental {RentalId} initiated for user {UserEmail}"

## Komponensek

### 1. DataGridView (`logDataGridView`)
- **Dock**: Fill - kitölti a teljes rendelkezésre álló területet
- **ReadOnly**: true - csak olvasható
- **AutoGenerateColumns**: false - manuálisan konfiguráljuk az oszlopokat
- **SelectionMode**: FullRowSelect - teljes sor kijelölés
- **CellDoubleClick**: Dupla kattintás esemény a Raw log megjelenítéséhez

### 2. BindingSource (`logBindingSource`)
- Közvetítõ réteg a DataGridView és az adatforrás között
- Támogatja a szûrést és rendezést
- Automatikus értesítés adatváltozásokról

### 3. BindingList<ILogEntry> (`logEntries`)
- Az adatforrás, amely tárolja a log bejegyzéseket
- Automatikusan értesíti a BindingSource-t változásokról
- Támogatja az Add, Remove, Clear mûveleteket

## Oszlopok

A DataGridView a következõ oszlopokat jeleníti meg:

| Oszlop | DataPropertyName | Szélesség | Formátum |
|--------|------------------|-----------|----------|
| Idõpont | Time | 150px | yyyy-MM-dd HH:mm:ss.fff |
| Szint | Level | 80px | - |
| Line | LineId | 60px | - |
| Kategória | Category | 120px | - |
| PF | ProgramFlowId | 60px | - |
| Op | OpId | 50px | - |
| Státusz | Status | 80px | - |
| Idõtartam | Duration | 100px | hh:mm:ss.fff |
| Üzenet | Message | Fill | - |

## Színezés

A sorok automatikusan színezõdnek a log szint alapján:

- **Error/Critical**: Világos piros háttér, sötét piros szöveg
- **Warning**: Világos sárga háttér, narancs szöveg
- **Information**: Fehér háttér, fekete szöveg
- **Debug/Trace**: Világos szürke háttér, szürke szöveg

## Funkciók

### 1. Log fájl megnyitása (Ctrl+O)
- Pipeline futtatása a kiválasztott log fájlon
- Automatikus szûrés és feldolgozás
- Eredmények megjelenítése a DataGridView-ban
- Státusz információk (bejegyzések száma, hibák, figyelmeztetések)

### 2. Frissítés (F5)
- A jelenleg betöltött log fájl újra feldolgozása
- Hasznos, ha a log fájl közben frissült

### 3. Törlés (Ctrl+L)
- Az összes betöltött bejegyzés törlése
- DataGridView kiürítése

### 4. Raw Log Entry megjelenítése (Dupla kattintás)
**Új funkció!** Dupla kattintás egy sorra megnyit egy modal ablakot, amely részletesen megjeleníti:

#### Megjelenített információk:
- **Log Entry Details**: Minden mezõ formázott megjelenítése
  - Time, Level, LineId, Category
  - ProgramFlowId, OpId, Status, Duration
  - Message

- **Properties**: Az összes property kulcs-érték párok rendezve
  - Pl.: Application, MachineName, RequestId, stb.

- **Raw Log Data**: Az eredeti, feldolgozatlan log sorok
  - Minden sor külön számozva [0], [1], stb.
  - Eredeti formátumban, szerkesztés nélkül

#### Funkciók a modal ablakban:
- **Görgetés**: Teljes tartalom görget hetõ (vertical és horizontal)
- **Monospace font**: Consolas betûtípus a jobb olvashatóságért
- **Vágólapra másolás**: "Vágólapra" gomb a teljes tartalom másolásához
- **Bezárás**: "Bezárás" gomb vagy Enter billentyû
- **Átméretezhetõ**: Az ablak átméretezhetõ, minimális méret 600x400

#### Példa kimenet:
```
=== LOG ENTRY DETAILS ===

Time:          2025-01-15 10:30:45.123
Level:         Information
LineId:        1
Category:      System
ProgramFlowId: 1001
OpId:          1
Status:        Start
Duration:      00:00:00.150
Message:       Application started

=== PROPERTIES ===

Application                    = MyApp
MachineName                    = SERVER01
RequestId                      = 12345

=== RAW LOG DATA ===

[0] 2025-01-15 10:30:45.123 +00:00 [INF] 1	System	Pf:1001	Op:1	Start	00:00:00.150	Application started

[1] { Application: "MyApp", MachineName: "SERVER01", RequestId: "12345" }
```

### 5. Export (Ctrl+E)

#### CSV Export
```csv
Time,Level,LineId,Category,ProgramFlowId,OpId,Status,Duration,Message
"2025-01-15 10:30:45.123","Information",1,"System",1001,1,"Start","00:00:00.150","Application started"
```

#### Text Export
```
2025-01-15 10:30:45.123 [Information] System          PF:1001     Op:1    Application started
```

## Publikus metódusok

### ShowRawLogEntryDialog(ILogEntry entry)
```csharp
// Megjeleníti a Raw log entry-t egy modal ablakban
private void ShowRawLogEntryDialog(ILogEntry entry);
```

### LoadFromPipeline(string logFilePath)
```csharp
// Belsõ használatú - Pipeline futtatása és eredmények betöltése
private void LoadFromPipeline(string logFilePath);
```

### AddLogEntry(ILogEntry entry)
```csharp
// Egyetlen log bejegyzés hozzáadása
var entry = new LogEntry
{
    Time = DateTime.Now,
    Level = LogLevel.Information,
    Message = "Teszt üzenet"
};
mainForm.AddLogEntry(entry);
```

### ClearLogEntries()
```csharp
// Összes bejegyzés törlése
mainForm.ClearLogEntries();
```

### FilterByLevel(LogLevel level)
```csharp
// Csak hibák megjelenítése
mainForm.FilterByLevel(LogLevel.Error);

// Szûrõ eltávolítása
mainForm.RemoveFilter();
```

## Példa használat

### 1. Normál használat
1. File ? Open Log File (Ctrl+O)
2. Válassz ki egy log fájlt
3. Az adatok automatikusan betöltõdnek és szûrve jelennek meg
4. **Dupla kattintás** egy sorra ? Raw log entry megjelenítése
5. Opcionálisan: Export (Ctrl+E) mentéshez

### 2. Raw log vizsgálata
1. Keresd meg a problémás log bejegyzést a grid-ben
2. Dupla kattintás a sorra
3. Vizsgáld meg a teljes Raw adatot és Properties-t
4. Opcionálisan: "Vágólapra" gomb ? másolás más eszközbe

### 3. Pipeline testreszabása

A `LoadFromPipeline` metódusban módosítható a Pipeline:

```csharp
var pipeline = new Pipeline()
    .AddItem(new LogSource(logFilePath))
    .AddItem(new OneLineLogFileReader())
    .AddItem(new CompactJsonLogParser())
    .AddItem(counter)
    .AddItem(errorAggregator)
    .AddItem(new Filter<LogEntry>(e =>
    {
        // Egyedi szûrési logika
        return e.Level >= LogLevel.Warning;
    }))
    .AddItem(collector);
```

## Interakció

### Egér mûveletek
- **Kattintás**: Sor kijelölése
- **Dupla kattintás**: Raw log entry megjelenítése modal ablakban
- **Görgõ**: Görgetés a grid-ben

### Billentyûzet
- **Nyíl billentyûk**: Navigálás a sorok között
- **Enter**: Kijelölt sor Raw log-jának megjelenítése (jövõbeli fejlesztés)
- **Ctrl+C**: Kijelölt cellaértékek másolása vágólapra

## Teljesítmény

- **Nagy fájlok**: A Pipeline streaming feldolgozást használ
- **Memória**: Csak a szûrt eredmények kerülnek memóriába
- **UI**: ProgressBar jelzi a feldolgozás állapotát
- **Modal**: Könnyû súlyú Form, gyors megjelenítés

## További fejlesztési lehetõségek

1. **Enter billentyû**: Raw log megjelenítése Enter-rel is
2. **Kontextus menü**: Jobb klikk menü (Copy, Raw view, Filter by...)
3. **Szintaxis kiemelés**: JSON Properties színezése
4. **Export Raw**: Raw adat exportálása közvetlenül
5. **Összehasonlítás**: Két Raw log összehasonlítása
6. **Keresés Raw-ban**: Find funkció a Raw adatokban
7. **Linkek**: Klikkel hetõ URL-ek és file path-ek
8. **Elõzõ/Következõ**: Navigálás a Raw log ablakból
