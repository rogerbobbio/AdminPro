## ADDED Requirements

### Requirement: Notas section on Application Detail
`ApplicationDetail` SHALL render a "Notas" section showing a count badge with the number of notes, an empty-state message ("No hay notas todavía.") when there are none, and otherwise a collapsible list where each row shows only the note's `Titulo` in bold with an expand/collapse chevron; expanding a row reveals its `Descripcion` beneath the title. A "+ Agregar Nota" pill action opens a modal form (`Titulo`, `Descripcion`) to create a note via `POST /api/applications/{id}/notas`. Each row SHALL offer edit (opens the same modal pre-filled, saving via `PUT /api/notas/{id}`) and delete (`DELETE /api/notas/{id}`) actions.

#### Scenario: Notas section shows an empty state
- **GIVEN** an application has no notes
- **WHEN** the user views its detail page
- **THEN** the "Notas" section shows "No hay notas todavía." alongside the "+ Agregar Nota" action

#### Scenario: Note rows are collapsed by default and expand on click
- **GIVEN** an application has a note titled "nvm use 14.16.0" with a description
- **WHEN** the user views its detail page
- **THEN** the note row shows only the title "nvm use 14.16.0", and clicking it reveals the description below

#### Scenario: Adding a note updates the count and list
- **GIVEN** an application has 0 notes
- **WHEN** the user clicks "+ Agregar Nota", fills `Titulo` and `Descripcion`, and saves
- **THEN** the "Notas" badge shows "1" and the new note appears (collapsed) in the list

#### Scenario: Deleting a note removes it from the list
- **GIVEN** an application has a note "Borrar bin/obj"
- **WHEN** the user deletes that note
- **THEN** it no longer appears in the "Notas" section and the count badge decrements

### Requirement: Reportes section on Application Detail
`ApplicationDetail` SHALL render a "Reportes" section showing a count badge, an empty-state message ("No hay reportes todavía.") when there are none, and otherwise a list/table of reports (`ReportCode`, `ReportName`) with edit and delete actions per row. A "+ Agregar Reporte" pill action opens a modal form to create a report via `POST /api/applications/{id}/reportes`.

#### Scenario: Reportes section shows an empty state
- **GIVEN** an application has no reports
- **WHEN** the user views its detail page
- **THEN** the "Reportes" section shows "No hay reportes todavía." alongside the "+ Agregar Reporte" action

#### Scenario: Adding a report updates the count and list
- **GIVEN** an application has 0 reports
- **WHEN** the user clicks "+ Agregar Reporte", fills `ReportCode` and `ReportName`, and saves
- **THEN** the "Reportes" badge shows "1" and the new report appears in the list

### Requirement: Documentos section on Application Detail
`ApplicationDetail` SHALL render a "Documentos" section showing a count badge, an empty-state message ("No hay documentos todavía.") when there are none, and otherwise a list of documents (`NombreArchivo`, `Tipo`) each with a link that opens `UrlOneDrive` in a new tab, plus edit and delete actions. A "+ Agregar Documento" pill action opens a modal form (`NombreArchivo`, `UrlOneDrive`, `Tipo` as a select of manual/diagrama/codigo/otro, `Descripcion`) to create a document via `POST /api/applications/{id}/documentos`.

#### Scenario: Documentos section shows an empty state
- **GIVEN** an application has no documents
- **WHEN** the user views its detail page
- **THEN** the "Documentos" section shows "No hay documentos todavía." alongside the "+ Agregar Documento" action

#### Scenario: Opening a document link
- **GIVEN** an application has a document "Manual de Usuario" with `urlOneDrive = "https://onedrive.example.com/manual"`
- **WHEN** the user clicks its link
- **THEN** the URL opens in a new browser tab

### Requirement: FixDatas section on Application Detail
`ApplicationDetail` SHALL render a "FixDatas" section showing a count badge, an empty-state message ("No hay fix datas todavía.") when there are none, and otherwise a list of fix data rows (`Nombre`) each with edit, delete, and a "copy script" action (copies `Script` to the clipboard). A "+ Agregar FixData" pill action opens a modal form (`Nombre`, `Descripcion`, `Script` as a textarea) to create a fix data via `POST /api/applications/{id}/fixdatas`.

#### Scenario: FixDatas section shows an empty state
- **GIVEN** an application has no fix datas
- **WHEN** the user views its detail page
- **THEN** the "FixDatas" section shows "No hay fix datas todavía." alongside the "+ Agregar FixData" action

#### Scenario: Copying a fix data's script
- **GIVEN** an application has a fix data "Fix duplicate customers" with a non-empty `Script`
- **WHEN** the user clicks its "copy script" action
- **THEN** the script text is copied to the clipboard
