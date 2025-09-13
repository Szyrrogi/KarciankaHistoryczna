# KarciankaHistoryczna

### Logika gry / Serwer / GameEngine
Utrzymuje jeden spójny stan gry. Jest jedynym źródłem informacji o aktualnym stanie gry. Wysyłane są do niej żądania, które są walidowane i w przypadku, kiedy są one poprawne, serwer aktualizuje swój stan i wysyła eventy do klienta.

### Klient (Unity)
Klientem określana jest instancja gry, która komunikuje się z serwerem. Klient zgłasza do serwera akcje które chce wykonać, np. koniec tury. Serwer przetwarza to żądanie i generuje event do klienta. Klient interpretuje ten event i aktualizuje widok gry, np. resetuje timer, zmienia kolor jednostki.
##### Game manager
Za odbieranie i wysyłanie komunikatów i zarządzaniem stanem obiektów w grze odpowiedzialny jest game manager. Zajmuje się on np. tworzeniem nowych obiektów kart


### Dane kart
Dane kart: nazwa, statystyki, efekty karty będą przechowywane wewnątrz scriptable object. Każda karta będzie miała swój unikalny GUID. 


### Tymczasowe działanie
Dopóki nie zostanie zaimplementowana komunikacja online, architektura będzie działała w uproszczony sposób. Logika gry będzie działać bezpośrednio wewnątrz game managera.