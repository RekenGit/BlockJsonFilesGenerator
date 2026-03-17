# MinecraftJsonGenerator

Gotowy projekt Windows Forms w C# do generowania plików JSON dla Minecrafta.

## Co generuje

- `assets/<namespace>/models/block/<block_name>/...`
- `assets/<namespace>/items/...`
- `assets/<namespace>/blockstates/<block_name>.json`

Obsługiwane typy bloków:

- Block
- Slab
- Stairs
- Wall
- Door
- Leaves
- Trapdoor
- Column
- Column 2
- Fence Gate
- Fence

## Wymagania

- Visual Studio 2022 / 2026 z workloadem **.NET desktop development**
- .NET 8 Windows Desktop

Microsoft zaleca tworzenie nowych aplikacji WinForms na szablonie **Windows Forms App** dla .NET, a nie na starym szablonie .NET Framework. citeturn0search4turn0search10

## Jak uruchomić

1. Otwórz plik `MinecraftJsonGenerator.csproj` w Visual Studio.
2. Uruchom projekt.
3. Uzupełnij pola formularza.
4. Kliknij **Generuj wszystko**.

## Tekstury

Program pozwala pracować w 2 trybach:

- **Jedna tekstura** – jeden wpis trafia do wszystkich wymaganych kluczy modelu.
- **Ręczne pola tekstur** – osobne pola zależne od typu bloku.

Możesz używać np.:

- `block/{variant}`
- `custom_folder/{variant}`
- `othernamespace:block/some_texture`

`{variant}` zostanie zastąpione nazwą aktualnego wariantu, np. `test_block_broken1`.

## Miniaturki

Jeśli wskażesz folder z PNG, program spróbuje wczytać miniatury po nazwach wariantów:

- `test_block.png`
- `test_block_broken0.png`
- `test_block_broken1.png`
- itd.

## Portable publish

Dodałem też skrypt `publish-portable.bat`, który tworzy publikację folderową typu portable.
