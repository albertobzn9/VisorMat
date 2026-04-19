using System;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.Data.Matlab;

namespace VisorMat
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. Revisar si arrastraron un archivo
            if (args.Length == 0)
            {
                MessageBox.Show("Por favor, arrastra un archivo .mat sobre este programa o usa 'Abrir con...'", "Visor MAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filePath = args[0];

            try
            {
                // 2. Leer el archivo .mat
                var matrices = MatlabReader.ReadAll<double>(filePath);
                
                // 3. Buscar la primera matriz válida (ignorando metadatos internos)
                var targetMatrix = matrices.FirstOrDefault(m => !m.Key.StartsWith("__")).Value;

                if (targetMatrix == null)
                {
                    MessageBox.Show("No se encontró una tabla válida en el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 4. Crear la ventana principal (Form)
                Form form = new Form
                {
                    Text = $"Visor MAT Ligero - {filePath}",
                    Width = 800,
                    Height = 500,
                    StartPosition = FormStartPosition.CenterScreen
                };

                // 5. Crear la cuadrícula de datos (DataGridView)
                DataGridView grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    RowHeadersVisible = true, // Muestra la columna vacía a la izquierda
                    SelectionMode = DataGridViewSelectionMode.RowHeaderSelect, // Permite seleccionar arrastrando
                    MultiSelect = true, // Permite seleccionar múltiples celdas
                    
                    // ¡LA MAGIA PARA COPIAR A EXCEL!
                    // Esto permite que al hacer Ctrl+C se copien los datos y los nombres de las columnas
                    ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText,
                    
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells
                };

                // 6. Agregar las columnas
                for (int i = 0; i < targetMatrix.ColumnCount; i++)
                {
                    grid.Columns.Add($"Col{i}", $"Col {i + 1}");
                }

                // 7. Llenar las filas
                for (int i = 0; i < targetMatrix.RowCount; i++)
                {
                    var rowValues = new object[targetMatrix.ColumnCount];
                    for (int j = 0; j < targetMatrix.ColumnCount; j++)
                    {
                        // Redondear a 4 decimales para que se vea limpio
                        rowValues[j] = Math.Round(targetMatrix[i, j], 4);
                    }
                    grid.Rows.Add(rowValues);
                }

                // 8. Mostrar todo
                form.Controls.Add(grid);
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo:\n{ex.Message}", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}