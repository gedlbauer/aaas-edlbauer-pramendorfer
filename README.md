# Analytics as a Service
> Andreas Pramendorfer, Georg Edlbauer

## Aufwand
Georg Edlbauer: 18h

Andreas Pramendorer: 18h

## Testdaten
Die Testdaten befinden sich in folgendem OneDrive Ordne: [OneDrive](https://1drv.ms/u/s!Apst1So_O6yE13HIkjOjpzPSjM5i?e=5JMQgM)

Sie waren zu groß für die Moodle Abgabe. :wink: 
## Projektstruktur
![Projektstruktur Screenshot](/images/project.PNG)

Die Solution wurde in folgende Projekte unterteilt:
1. **AaaS.Domain**
Hier befinden sich die Klassen aller Objekte, die in der Datenbank persistiert werden sollen.
2. **AaaS.Common**
Hier befinden sich allgemeine Hilfsklassen, die von mehreren anderen Projekten verwendet werden.
3. **AaaS.Dal.Interface**
Hier befinden sich die Interfaces, welche die Datenzugriffsschicht definieren. Möglicherweise müssen diese Interfaces im Laufe des Projekts noch erweitert werden.
4. **AaaS.Dal.Ado**
In diesem Projekt werden die in AaaS.Dal.Interface definierten Interfaces mit ADO.NET implementiert. Für jede DAO Klasse gibt es zusätzlich eine MSSQL-Klasse, welchen den Konkreten Zugriff für MSSQL-Server implementieren.
Auch andere Datenbankbezogene Daten sind in diesem Projekt. Konkret sind das beispielsweise die Statements zum Erzeugen und Löschen des Datenbankschemas. Auch ein einfaches Seed-Script ist hier hinterlegt.
5. **AaaS.Dal.Tests**
Hier befinden siche die Unittests für die Datenzugriffschicht. Diese wurden mit dem Testframework XUnit in Verbindung mit Fluent Assertions umgesetzt.
Dieses Projekt arbeitet mit der Datenbank `C:\temp\AaaSTestDb.mdf`.
6. **AaaS.SeederClient**
Hier befindet sich ein einfacher Demo Client, welcher die Datenbank mit den vorgegebenen Datensätzen füllt und danach als Demo einfache Statistiken der Datenbank ausgibt.
Damit dieser Client funktioniert, müssen die beigelegten [CSV Testdaten](https://1drv.ms/u/s!Apst1So_O6yE13HIkjOjpzPSjM5i?e=5JMQgM) in das Verzeichnis `C:\temp\AaaS\seeding` gelegt werden. Dabei dürfen die Namen der Files nicht verändert werden.
7. **AaaS.Core**
Hier befindet sich später die eigentliche Business Logic. Im Moment befinden sich in diesem Verzeichnis nur die vorgegebenen Detectoren und Aktionen. Die eigentliche Logik wird erst im Verlauf der 2. Ausbaustufe umgesetzt.

## Datenhaltung

### Datenbankmodell
![Datenbankmodell](/images/database.png)

Die gemeinsamen Eigenschaften aller Telemetrien werden in der Tabelle *Telemetry* gespeichert. Für jeden konkreten Telemetrie Datentyp (Metric, Log und Time Measurement) gibt es eine weitere Tabelle, welche die zusätzlichen Eigenschaften enthält. Dieser Ansatz wird auch als *Class Table Inheritance* bezeichnet. 

Um die Detektoren und Aktionen einfach erweitern zu können, ohne das DB-Schema ändern zu müssen, werden diese Daten über Reflection gespeichert.
Sowohl Detector, als auch Action erweitern die Datenbanktabelle Object.
Action hat nur die Object-ID. Diese Tabelle dient rein der klaren Trennung von Action und Detector.
Die Tabelle Detector beinhaltet die zugehörige Object-ID, sowie sämtliche Properties, welche in C# in der Basisklasse Detector vorhanden sind. Auch der Datentyp der Implementierung ist in dieser Klasse hinterlegt. So kann später beim Programmstart wieder der richtige Detector erzeugt werden.
Properties, welche in den Ableitungen von IAction bzw. Detector definiert werden, werden dynamisch per Reflection ausgelesen und im Anschluss in die Tabelle ObjectProperty gemeinsam mit der zugehörigen Object-ID, ihrem Datentyp und Namen abgelegt.

Dieser Ansatz wurde gewählt, da die Detektoren und Aktionen nur beim Programmstart ausgelesen werden müssen. Da das in einer Realen Serverumgebung nur sehr selten der Fall ist, ist es verkraftbar, dass die Erzeugung der Objekte mittels Reflection langsamer ist, als würde man sie "direkt" erzeugen. Daher wurde im Design eher auf einfache Erweiterbarkeit gesetzt, als auf Performanz.
### Test-/Produktivdatenbank
Die Tests aus AaaS.Dal.Tests arbeiten mit einer Separaten Tsetdatenbank. Diese Testdatenbank muss sich unter `C:\temp\AaaSTestDb.mdf` befinden. Sie ist genau gleich aufgebaut wie die Produktivdatenbank. Der Zweck dieser Datenbank ist, dass die Produktivdaten nicht durch die Unittests zerstört werden.
Das MDF-File für die Produktivdatenbank muss unter `C:\temp\AaaSDb.mdf` verfügbar sein.

Die oben angegebenen Pfade sind die Standardpfade. Sie können in den appsettings der Projekte `Aaas.SeederClient` bzw. `AaaS.Dal.Tests` angepasst werden.

## Struktur der Datenbankzugriffsschicht
Bei der Implementierung der Datenbankzugriffsschicht wurde sich an die Vorgehensweise aus der Übung bzw. aus der Vorlesung gehalten.
Es wurde nach dem Design Pattern *Program to an Interface, not to an Implementation* vorgegangen.
Das heißt, es gibt für jedes DAO ein Interface, welches die öffentliche Schnittstelle beschreibt. So könnte die Datenbankzugriffsschicht später einfach ausgetauscht werden.

Die DAO-Klassen selbst sind wiederum Abstrakt. Von diesen muss geerbt werden, um die Implemierung für einen Konkreten Datenbankanbieter zu schaffen. Es wurde der Zugriff für MSSQL Server implementiert, da in diesem Projekt mit der LocalDb von Microsoft gearbeitet wird.

Um Codeduplizierung zu vermeiden, greifen die DAO-Klassen auf die Template-Methoden von AdoTemplate zu. Hierbei handelt es sich um generische Methoden, welchen das jeweilge SQL-Statement, sowie ein Delegate zum Mapping auf ein Domänenobjekt übergeben werden muss. Die jeweilige Mapping-Methode wird von den einzelnen DAO-Klassen spezifisch implementiert.

## Erste Schritte
### Datenbanken
Um die Anwendungen Lauffähig zu machen, müssen zuerst die beiden MDF-Datenbankdateien `C:\temp\AaaSDb.mdf` und `C:\temp\AaaSTestDb.mdf` erzeugt werden. In dem beigefügten OneDrive liegen zwei leere MDF-Files. Diese können in das angegebene Verzeichnis kopiert werden.

### Unit Tests
Nachdem die Datenbankdatei in das richtige Verzeichnis gelegt wurden, können die Tests bereits ausgeführt werden.

### Demo Client
Damit der Demo Client funktioniert, müssen zusätzlich zu den MDF-Dateien auch noch die im OneDrive-Ordner befindlichen CSV-Dateien (Subverzeichnis `/seeding/`) nach `C:\temp\AaaS\seeding` kopiert werden. Danach kann der Demo Client ausgeführt werden. Dieser befüllt die Datenbank mit den in der Angabe geforderten Datensätzen. Danach gibt er Statistiken über die angelegten Dateien aus.

**ACHTUNG:** Der Demo Client löscht bei Ausführung die Datenbank und legt diese neu an. Bereits vorher eingefügte Dateien gehen also gegebenenfalls verloren. Dieser Client dient ausschließlich dazu, bereits in dieser Ausbaustufe zu zeigen, dass die bereits implementierten Teile des Projektes funktionieren.