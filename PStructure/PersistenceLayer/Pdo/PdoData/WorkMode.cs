namespace PStructure.PersistenceLayer.Pdo.PdoData
{
    /// <summary>
    ///     Konfiguration, für die verschiedenen Verhaltensweisen des PdoManagers.
    /// </summary>
    /// <remarks>
    ///     Trivia: Eine WorkMode beschreibt, wie die Pdo-Persistenzschicht auf die Datenbank abgebildet wird.
    ///     Die Konfiguration wird in dem ItemManager als Indexierung des Caches genutzt, um die Möglichkeit zu erhalten
    ///     WorkModes dynamisch zu wechseln.
    ///     Beispiel: Konfigurationen sind z.b. Test, bei der die TableLocation auf die Testtabellen einer Anwendung zeigen.
    ///     Einschränkungen:
    ///     - Für den WorkMode wurde absichtlich ein Enum gewählt, weil hier eine überschaubare Menge an sinnvollen Szenarien
    ///     hierfür gibt.
    ///     Menge an Szenarien:
    ///     -   Testumgebung: Eine Entität pro Datenbank. Bei mehreren Testumgebungen sollten klone von Datenbanken angelegt
    ///     werden. Das ist eine umgänglichere Art
    ///     durch Systeme der Datenbankadministration, die man nicht redundant in Software verstecken muss)
    ///     -   Produktivumgebung: Eine Entität pro Datenbank.
    ///     -   Dummyumgebung: Eine Entität pro Datenbank. Entspricht dem Konstrukt eine Datenbank simulieren zu können.</remarks>
    public enum WorkMode
    {
        Live,
        Test,
        Dummy
    }
}