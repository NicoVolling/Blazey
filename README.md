# Blazey Framework

Blazey ist ein kleines Beispielprojekt für Blazor WebApps. Es enthält mehrere .NET 8 Bibliotheken und eine Demo-Anwendung. Das Projekt wird nicht weiterentwickelt, kann aber als Ausgangspunkt für eigene Experimente dienen.

## Projektziel

Der Sinn dieses Repositories war es, verschiedene Skills im Umfeld von Blazor, Entity Framework Core und dem Aufbau wiederverwendbarer Komponenten zu vertiefen. Blazey diente daher vor allem Lern- und Experimentierzwecken und soll nicht produktiv eingesetzt werden. Eine Weiterentwicklung ist aktuell nicht vorgesehen.

## Inhalt des Repositories

* **Blazey.Communications** – stellt `OperationStateProvider` bereit, um Status-, Fehler- und Warnmeldungen sowie den Abschluss einer Aktion zu verwalten.
* **Blazey.Components** – grundlegende UI-Komponenten (Buttons, Icons, Layouts).
* **Blazey.Components.Data** – `DataModifier` zum Erstellen, Bearbeiten und Löschen von Objekten.
* **Blazey.Components.Input** – Eingabefelder (Text, Datum, Auswahl).
* **Blazey.Components.Navigation** – Navigationsmenü mit optionalem Login.
* **Blazey.Components.Page** – Seiten-Container für Layouts.
* **Blazey.Components.Table** – sortier- und filterbare Datentabellen.
* **Blazey.Components.Validation** – Validierung von Eingaben über `ValidationMaster` und `ValidationChild`.
* **Blazey.Data** – generische Datenservices (CRUD) auf Basis von Entity Framework Core.
* **Blazey.Email** – Versand von Bestätigungs- und Benachrichtigungs-E-Mails via SMTP.
* **Blazey.Security** – Identity-Integration, Authorisierung, Admin-Code und API-Controller für Login/Registrierung.
* **Blazey.Patchnotes** – Hilfsklassen zum Erstellen und Verwalten von Patchnotes.
* **BlazeyTest.App** – Beispielanwendung, die zeigt, wie die Bibliotheken zusammen eingesetzt werden.

## Hauptfunktionen

- **OperationStateProvider** speichert Fehler, Warnungen und Erfolgsmeldungen einer Aktion und signalisiert deren Abschluss.
- **UI-Komponenten** wie `NavigationMenuWithLogin`, `DataTable` oder `DataModifier` können direkt in Razor-Pages eingebunden werden.
- **Validierung**: Mit `ValidationMaster` lassen sich mehrere `ValidationChild`-Komponenten zentral auswerten.
- **Datenzugriff**: `BaseDataService` und `BaseDataHandler` kapseln CRUD-Operationen mit Entity Framework Core und bieten optionale Sicherheitsabfragen.
- **Sicherheit**: Authorisierungsregeln, Benutzerverwaltung mit Identity, Admin-Code für Erstinstallation, Login/Logout-API.
- **E-Mail-Versand**: `EmailService` nutzt in den Beispiel-„appsettings" definierte SMTP-Daten.
- **Patchnotes**: Klassen zum Erstellen strukturierter Änderungslisten für Releases.

## Beispielanwendung

Die Projektmappe enthält `BlazeyTest.App`, eine vollständige Blazor Server App. Dort werden

1. Datenbank und Identity über `ApplicationDbContext` eingerichtet.
2. Datenservices für Beispieltabellen registriert.
3. Mail- und Admin-Code-Einstellungen aus `appsettings.json` geladen.
4. Razor-Komponenten und Routen definiert (siehe `Components/Pages`).

Die mitgelieferten `appsettings.json` enthalten Test-Zugangsdaten und sollten nicht für Produktivsysteme verwendet werden.

## Voraussetzungen

* .NET 8 SDK
* SQL Server (oder kompatible Datenbank für Entity Framework Core)

## Build und Start

```bash
# Projekt kompilieren
# (im Repository-Verzeichnis ausführen)
dotnet build Blazey.sln

# Beispielanwendung starten
cd BlazeyTest.App
 dotnet run
```

Falls das .NET SDK nicht installiert ist, müssen Sie es vorher von [https://dotnet.microsoft.com/](https://dotnet.microsoft.com/) beziehen. In manchen Umgebungen ist ein Build nicht möglich (z.B. fehlender Internetzugang oder Abhängigkeiten).

## Datenbank

Die Demo nutzt LocalDB (siehe `appsettings.json`). Bei Bedarf die Connection-Strings auf eine eigene SQL Server Instanz anpassen. Migrations befinden sich im Ordner `BlazeyTest.App/Migrations` und werden beim Start angewendet.

## Lizenz

Der gesamte Quellcode steht unter der [Unlicense](LICENSE.txt) und ist damit gemeinfrei. Das Projekt wird nicht weiter gepflegt und dient nur als Beispiel.

## Haftungsausschluss

Dieses Repository enthält keinerlei Gewährleistungen. Nutzung auf eigene Gefahr. Beispielpasswörter und Konfigurationsschlüssel sind lediglich zu Testzwecken hinterlegt und sollten in eigenen Projekten ausgetauscht werden.
