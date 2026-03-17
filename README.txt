Pliki do wklejenia do projektu WinForms
======================================

Ten pakiet zakłada, że:
- formularz nazywa się MainForm
- kontrolki już istnieją w Designerze
- nazwy kontrolek są dokładnie takie:
  txtBlockName
  txtVariantPattern
  numVariantCount
  txtNamespace
  cmbBlockType
  rbSingleTexture
  rbManualTextures
  pnlTextureInputs
  txtOutputPath
  btnBrowseOutput
  lstVariants
  flpWeights
  btnGenerateModels
  btnGenerateItems
  btnGenerateBlockstates
  btnGenerateAll

Co dodać do projektu:
- MainForm.cs
- folder Models
- folder Services

Ważne:
- tego pakietu NIE trzeba używać z gotowym Designerem od nowa
- nie ma tu pliku MainForm.Designer.cs, bo napisałeś, że kontrolki są już utworzone
- jeśli Visual Studio pokaże konflikt namespace, ustaw namespace projektu na MinecraftJsonGenerator
  albo zamień namespace w plikach na swój aktualny

Struktura wyjściowa generowanych plików:
assets/<namespace>/
  blockstates/
  items/
  models/block/<nazwa_bloku>/

Uwaga praktyczna:
- blockstates dla złożonych bloków (door, wall, stairs, trapdoor, fence_gate, fence)
  są już zaimplementowane, ale najlepiej od razu przetestować je w grze na 1-2 przykładach.
