# Analytics as a Service
> Andreas Pramendorfer, Georg Edlbauer

## Aufwand

### Ausbaustufe 1
Georg Edlbauer: 18h

Andreas Pramendorer: 18h

### Ausbaustufe 2
Georg Edlbauer: 42h

Andreas Pramendorfer: 43h

### Gesamt
Georg Edlbauer: 60h

Andreas Pramendorfer: 61h

## Testdaten
Die Testdaten befinden sich in folgendem OneDrive-Ordner: [OneDrive](https://1drv.ms/u/s!Apst1So_O6yE13HIkjOjpzPSjM5i?e=5JMQgM)

Sie waren zu groß für die Moodle Abgabe. :wink: 
## Projektstruktur!




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
Hier befindet sich die eigentliche Business Logic. Es ist die zentrale Stelle im System, welche Daten von verbundenen Anwendungen verarbeitet. Das Projekt kapselt die Datendankzugriffschicht in Form von Repositories und beinhaltet die Detektoren und Aktionen mit den dazugehörigen Interfaces. 
8. **AaaS.Core.Test**
Hier befinden sich die Tests für die Geschäftlogik. Um diese Tests von der Datenzugriffsschicht zu entkoppeln, wurde ein Mocking Framework für das mocken der Dao Objekte verwendet. Es wurde darauf geachtet, so weit wie möglich die geamte Funktionalität der Anwendung abzudecken. 
9. **AaaS.Api**
In diesem Projekt befindet sich das auf ASP .NET basierende REST Web Service. Die API bietet Schnittstellen für das AaaS.Web Projekt und für das AaaS.ClientSDK. Es ist also möglich Telemetriedaten zu erstellen und Detektoren bzw. Aktionen zu verwalten. Samtliche Schnittstellen sind mittels API Key abgesichert. 
10. **AaaS.ClientSDK**
Bündelt die Funktionalität zum Versenden von Telemetriedaten. 
11. **AaaS.DemoClient**
Eine einfache Konsolenanwendung welche das CLientSDK verwendet, um Beispieldaten an das Rest Service zu senden. 

Der folgende Graph stellt die Abhängigkeiten der Projekte dar:

![Projektabhängigkeiten](/images/dependencies.png)

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

## AaaS.Core
### Detectors
Die unterschiedlichen Detektorarten werden durch unterschiedliche Klassen implementiert. Die Gemeinsame Schnittstelle aller Detektoren wird in `IDetector` beschrieben. Sie beschränkt sich auf `Start()` und `Stop()`.

Die Einzelnen Detektoren leiten jedoch nicht direkt von diesem Interface ab. Die gemeinsame Basisklasse aller Detektoren bildet die abstrakte Klasse `BaseDetector`. Diese leitet wiederum von `IDetector` und `AaaS.Domain.Detector` ab. `IDetector` gibt die Schnittstelle für die Logik vor, `AaaS.Domain.Detector` beinhaltet jene Properties, welche von allen Detectors in der Datenbank persistiert werden müssen. BaseDetector gibt die abstrakte Methode `Detect()` vor, welche von den Kindklassen implementiert werden muss. In `Start()` wird ein Task gestartet, welcher in dem vorgegebenen Intervall `Detect()` aufruft. `Start()` ist also eine Template Method.

Um eine strikte Trennung von AaaS.Domain und AaaS.Core zu ermöglichen, muss `AaaS.Domain.Detector` generisch mit dem Typen der Action ausgeprägt werden. Das ist deswegen nötig, weil in `AaaS.Core` Klassen benötigt werden, die von `IDetector` erben. Da `IDetector` in `Aaas.Dal.*` jedoch nur reine Datenklassen leben, muss der Rückgabetyp per Typparameter angegeben werden. Zu beachten ist, dass dieser Typ von `AaaS.Domain.Detector` ableiten muss, um sicherzustellen, dass der Typ persistiert werden kann.

`MinMaxDetector` leitet direkt von `BaseDetector` ab, da es sich hier (lt. Angabe) um einen speziellen Detector handelt. Theoretisch hätte er auch von `SlidingWindowDetector` ableiten können, jedoch ist es lt. Angabe explizit gewünscht, dass es sich hierbei nicht um einen Schiebefensterdetektor handelt.

Die drei Detektoren `SumSlidingWindowDetector`, `CurrentValueSlidingWindowDetector` und `AverageSlidingWindowDetector` leiten alle von der abstrakten Klasse `SlidingWindowDetector` ab. In `SlidingWindowDetector` wurde `Detect()` wiederum als Template Method implementiert. Diese ruft die abstrakte Methode `CalculateCheckValue()` auf, welche von den einzelnen Schiebefensterdetektoren implementiert werden muss. Das ist deswegen möglich, weil der Vergleich immer gleich ist. Nur die Aggregatsfunktion zur Berechnung des Vergleichswertes ist unterschiedlich.

![Klassendiagramm Detektoren](/images/classdiagram-detectors.png)

### Actions
Bei den Actions ist eine ähnliche Vererbungsstruktur zu finden wie bei den Detektoren. Die logische Schnittstelle wird wiederum von `IAction` vorgegeben. Diese Besteht einzig und allein aus der Methode `Execute()`, welche die Action ausführt.

`BaseAction` bildet analog zu `BaseDetector` die gemeinsame abstrakte Basisklasse für alle Aktionen. Sie leitet einerseits von `IAction` ab, um die logische Schnittstelle zu implementieren. Anderersetits Leitet `BaseAction` von `AaaS.Domain.AaaSAction` ab. `AaaS.Domain.AaaSAction` gibt die Properties vor, welche von allen Actions in der Datenbank Persistiert werden müssen.

Auch bei den Action-Klassen ist aus den gleichen Gründen wie bei den Detektoren eine generische Ausprägung nötig.

![Klassendiagramm Actions](/images/classdiagram-actions.png)

### Heartbeat Service
Das Hearbeat Service ist ein BackgroundService, welches die Funktionalität der Heartbeats abbildet. Clients können an dieses Service regelmäßig Nachrichten senden. Fallen mehrere Heartbeats aus, ohne dass sich der Client voher über die entsprechende Methode abgemeldet hat, wird eine Warnmail an eine konfigurierbare E-Mail Adresse gesendet. Zum Versenden der Mails wird das SendGrid NuGet Package verwendet. Das zugehörige Interface und die Optionen für das Service werden über Dependency Injection injiziert. Dies ermöglicht es auch, das Service relativ einfach zu Testen, da dafür die Intervalle und Schwellwerte entsprechend niedriger zu setzen. 

### Repositories 
Die Repositories dienen dazu, die Datenbankzugriffschicht über das Core Projekt verfügbar zu machen. Für jeden Telemetrietyp gibt es ein eigenes Repository mit jeweils einem zugehörigen Interface. Die meisten Methoden dieser Klassen sind recht simpel und delegieren nur and die zugehörigen Dao Objekten. 

### Managers
Managers haben grundsätzlich die gleiche Funktionalität wie Repositories. Sie haben jedoch zusätzliche Aufgaben. Der `DetectorManager` muss beispielsweise beim Applikationsstart alle Detektoren aus der Datenbank laden und starten. Der `ActionManager` achtet darauf, dass pro Action nur eine Instanz existiert, und dass die Benutzer nicht auf fremde Actions zugreifen dürfen. Beide Klassen werden durch ein entsprechendes Interface definiert.

## AaaS.Api
### Dependency Injection
Um Abhängigkeiten zwischen Objekten an einer zentralen Stelle relgeln zu können, wurde das Entwurfsmuster Dependency Injection verwendet. Dependency Injection wird von .NET sehr gut unterstützt. Eine ASP .NET Web Applikation bietet in der Methode `Startup.ConfigureServices()` die Möglichkeit Klassen als Singelton, Transient oder Scoped zu registieren. 

Vorzugsweise werden bei der Registrierung lediglich die Interfaces angegeben, damit ander Objekte auch nur von den Interfaces abhängig sind und die Implementierungen einfach ausgetauscht werden können.
```
services.AddTransient<ILogDao, MSSQLLogDao>();
services.AddTransient<IMetricDao, MSSQLMetricDao>();
services.AddTransient<ITimeMeasurementDao, MSSQLTimeMeasurementDao>();
services.AddTransient<IClientDao, MSSQLClientDao>();
```

Ist ein bestimmter Objekttyp registriert, kann dieser von anderen Objekten im Konstruktor angefordert werden:
```
public MetricRepository(IMetricDao metricDao)
{
     _metricDao = metricDao;
}
```

### Authentifizierung und Autorisierung
Sämtliche Schnittstellen der API sollten über eine API Key Authentifizierung bzw. Autorisierung abgesichert werden. Um dies zu ermöglichen, wurde ein `ApiKeyAuthenticationHandler` entworfen, welcher von der Klasse `AuthenticationHandler` aus `Microsoft.AspNetCore.Authentication` ableitet. In der `HandleAuthenticateAsync()` Methode, welche später von jedem Web Request aufgerufen wird, wird der mitgelieferte API Key aus dem Header extrahiert. Über diesen API Key wir der dazugörige Client aus der Datenbank geladen. Existiert kein solcher Client, oder wird kein Key mitgeschickt, ist der User nicht authentifiziert und es wird eine ensprechende Fehlermeldung geworfen. Wurde ein Client gefunden, werden Name und Id des Clients über ein `AuthenticationTicket` gesetzt. Dadurch sind diese Attribute später in den Controller Klassen abrufbar, wodurch eine gewissen Autorisierung möglich ist, da beispielsweise nur Telemetriedaten mit der Id des autentifzieren Clients geliefert werden. 

### Swagger
Die AaaS.Api liefert zwei Swagger Dokumente aus. Zwischen den Dokumenten kann in der Swagger Oberfläche rechts oben ausgewählt werden. 
#### AaaS.Api Web
Liefert die Schnittstelllen für AaaS.Web.

![Web Swagger](/images/swagger-web-collapsed.PNG)

#### Aaas.Api Clients
Liefert die Schnittstellen für externe Anwendungen.

![External Swagger](/images/swagger-external-clients.PNG)

### Controller
#### Telemtrie
Um die Daten verschiedenen Telemtrietypen möglichst einfach exportieren zu können und den Code daurch *DRY* zu halten, wurde bei den Controllern eine Ableitungshierarchie entworfen. Alle geteilten Schnittstellen befinden sich in der abstrakten Klasse `TelemetryController`. Diese muss mit den generischen Typen ausgeprägt werden, um ein Mapping über den AutoMapper zu ermöglichen. Die Ableitungen liefern dann das ensprechende Repository und können den Controller um zusätliche Schnittstellen erweitern. 
![Telemetrie Controller](/images/classdiagram-controllers.png)

#### Commands
Die Schnittstellen für die Clients wurden in Command Controllern gesammelt. Daruch werden die Read und Write Operationen getrennt. Es ist dadurch auch möglich, sich einen Client über den Swagger zu erstellen, welcher nur die Operationen die von den externen Anwendungen benötigt werden exportiert.

## AaaS.ClientSDK
Das ClientSDK bündelt die Funktionen von AaaS für externe Anwendungen. Zuerst wurde ein Client über die Swagger Definition automatisch generiert. Um die Handhabung zu vereinfachen wurde die Klasse `AaaSService` entworfen. Dieses Service kann mit einem ApiKey und einer Creator Id konfiguriert werden. Es übernimmt auch automatisch das Senden von Heartbeats. Über die Methoden `StartTiming()` und `StopAndInsertTiming()`, können CLients komfortabel Zeitmessungen durchführen. Um diese Funktionalitäten zu ermöglichen, musste diese Klasse mit dem Singelton Pattern umgesetzt werden. 

## Erste Schritte
### Datenbanken
Um die Anwendungen Lauffähig zu machen, müssen zuerst die beiden MDF-Datenbankdateien `C:\temp\AaaSDb.mdf` und `C:\temp\AaaSTestDb.mdf` erzeugt werden. In dem beigefügten OneDrive liegen zwei leere MDF-Files. Diese können in das angegebene Verzeichnis kopiert werden.

### Unit Tests
Nachdem die Datenbankdatei in das richtige Verzeichnis gelegt wurden, können die Tests bereits ausgeführt werden.

### Seeder Client
Damit der Seeder Client funktioniert, müssen zusätzlich zu den MDF-Dateien auch noch die im OneDrive-Ordner befindlichen CSV-Dateien (Subverzeichnis `/seeding/`) nach `C:\temp\AaaS\seeding` kopiert werden. Danach kann der Demo Client ausgeführt werden. Dieser befüllt die Datenbank mit den in der Angabe geforderten Datensätzen. Danach gibt er Statistiken über die angelegten Dateien aus.

Um in den folgenden Schritten mit einer geseedeten Datenbank zu starten, wird es empfohlen den Seeder Client zu Beginn einmal zu starten. 

**ACHTUNG:** Der Seeder Client löscht bei Ausführung die Datenbank und legt diese neu an. Bereits vorher eingefügte Dateien gehen also gegebenenfalls verloren. Dieser Client dient ausschließlich dazu, bereits in dieser Ausbaustufe zu zeigen, dass die bereits implementierten Teile des Projektes funktionieren.

### API
Nun kann die API gestartet werden. Nach dem starten ist unter [localhost](https://localhost:5001/swagger/index.html) eine Swagger Oberfläche zu finden. Mit dem `Authorize` Button der Swagger Oberfläche muss zuerst ein gültiger API Key hinterlegt werden. Nun können die REST Operationen getestet werden. Gültige API Keys können dem File `clients.csv` aus den Testdaten entnommen werden. 

### Demo Client
Um das Erstellen von Telemetriedaten zu testen, kann der Demo Client gestartet werden. Dieser ist eine Konsolenanwendung, welche nur unter Windows lauffähig ist. 

**ACHTUNG:** Der Demo Client funktioniert nur, wenn die API auf der lokalen Maschiene läuft. 
