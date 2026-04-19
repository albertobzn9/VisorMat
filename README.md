# VisorMat

Una herramienta de escritorio ultra rápida diseñada para investigadores que necesitan inspeccionar archivos `.mat` sin tener que abrir MATLAB.

## El Problema
Abrir MATLAB solo para revisar si los datos de un entrenamiento son correctos consume tiempo y memoria RAM. VisorMat resuelve esto permitiendo una inspección instantánea.

## Características
- **Apertura Instantánea:** Compilado de forma nativa para Windows (C#/.NET).
- **Interoperabilidad:** Permite seleccionar datos y copiarlos (`Cmd/Ctrl + C`) con formato de celdas para pegarlos directamente en Excel.
- **Visualización Limpia:** Formateo automático de decimales y resaltado de filas para mejorar la legibilidad de matrices grandes.
- **Portabilidad:** No requiere MATLAB instalado en la computadora.

## Instalación
Descarga el ejecutable desde la sección **[Releases]**. Es un archivo único "Self-Contained" que funciona en cualquier PC del laboratorio.

## Cómo compilar para Windows (Para Desarrolladores)

Si deseas modificar el código y volver a generar el ejecutable (`.exe`), abre tu terminal en la carpeta del proyecto y ejecuta el siguiente comando:

```bash
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained true -p:EnableCompressionInSingleFile=true
