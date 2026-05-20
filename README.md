# Game Launcher – README

## Cíl projektu
Cílem aplikace je vytvořit použitelný, stabilní a snadno rozšiřitelný **launcher her**, který umožňuje uživatelům procházet dostupné herní tituly a přidávat si je do vlastní knihovny. Aplikace klade důraz na:

- Kvalitu kódu
- Technické zpracování
- Uživatelskou přívětivost
- Grafické zpracování
- Stabilitu a validaci vstupů

Aplikace nesmí padat ani zamrzat. Při chybném vstupu je uživatel upozorněn validační hláškou.

---

## Funkcionalita
Aplikace umožňuje:

- Zobrazit seznam všech dostupných herních titulů
- Zobrazit detail hry (název, popis, PEGI rating)
- Přidat hru do uživatelovy knihovny
- Spravovat více knihoven
- Spravovat uživatelský účet
- Filtrování a vyhledávání her  

---

## Datový model

### Herní titul
- Název  
- Popis  
- PEGI rating  
- (Vazba na knihovny)

### Knihovna
- Název  
- Popis *(volitelné)*  
- Počet obsažených položek *(počítá se dynamicky)*  
- (Vazba na herní tituly)

### Uživatel
- Username  
- Email  
- Jméno *(volitelné)*  
- Příjmení *(volitelné)*  
- (Vazba na knihovny)

---

## Architektura aplikace
Aplikace je navržena jako modulární a snadno rozšiřitelná. Klíčové části:

- **Frontend** – UI pro správu her a knihoven  
- **Backend** – API pro práci s daty  
- **Datová vrstva** – databáze / úložiště entit  

Každá entita má vlastní model, repository a dal vrstvu, aby byla aplikace přehledná a udržovatelná.

---

## Validace
Aplikace provádí validaci:

- Povinných polí (např. název hry, username, email)
- Formátu emailu
- Rozsahu PEGI ratingu (0–18)
- Duplicitních názvů knihoven
- Přidání stejné hry do knihovny vícekrát

Při chybě se zobrazí srozumitelná validační hláška.

---

## Spuštění
Aplikace se sestavuje ve visual studiu a následně se spouští StackOverFIT.exe

---

## Autoři
- Chuchlík Lukáš (vedoucí)
- Muzikant Ondřej
- Čislák Dávid
- Lanča Petr David
- Mračna David
