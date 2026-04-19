using System;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
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

            if (args.Length == 0)
            {
                MessageBox.Show("Por favor, arrastra un archivo .mat sobre este programa o usa 'Abrir con...'", "Visor MAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filePath = args[0];
            string fileName = Path.GetFileNameWithoutExtension(filePath); 

            try
            {
                var matrices = MatlabReader.ReadAll<double>(filePath);
                var targetMatrix = matrices.FirstOrDefault(m => !m.Key.StartsWith("__")).Value;

                if (targetMatrix == null)
                {
                    MessageBox.Show("No se encontró una tabla válida en el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Form form = new Form
                {
                    Text = $"Visor MAT - {fileName}",
                    Height = 500, // Alto fijo por defecto
                    StartPosition = FormStartPosition.CenterScreen,
                    Font = new Font("Segoe UI", 10),
                    BackColor = Color.White
                };

                // 1. Etiqueta con el nuevo formato limpio (Sin comillas)
                Label lblTitulo = new Label
                {
                    Text = $"Nombre: {fileName} | Tamaño: ({targetMatrix.RowCount}, {targetMatrix.ColumnCount})",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Height = 40,
                    AutoSize = false 
                };

                DataGridView grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    ReadOnly = true,
                    
                    RowHeadersVisible = false, 
                    ColumnHeadersVisible = true, 
                    
                    ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
                    SelectionMode = DataGridViewSelectionMode.CellSelect, 
                    
                    // Ajustar las celdas al contenido
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells, 
                    
                    BackgroundColor = Color.White,
                    
                    // 2. Estilo "Borderless" tipo Python/Pandas
                    BorderStyle = BorderStyle.None,
                    CellBorderStyle = DataGridViewCellBorderStyle.None, // Cero líneas entre celdas
                    ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None // Encabezados limpios
                };

                // Centrar texto y configurar la fila alterna (vital para no perderse si no hay bordes)
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                // Un gris muuuuuy suave, más elegante que el WhiteSmoke por defecto
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248); 

                Panel panelContenedor = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(15) 
                };
                panelContenedor.Controls.Add(grid);

                for (int i = 0; i < targetMatrix.ColumnCount; i++)
                {
                    grid.Columns.Add($"Col{i}", $"Col {i + 1}"); 
                }

                for (int i = 0; i < targetMatrix.RowCount; i++)
                {
                    var rowValues = new object[targetMatrix.ColumnCount];
                    for (int j = 0; j < targetMatrix.ColumnCount; j++)
                    {
                        rowValues[j] = Math.Round(targetMatrix[i, j], 1);
                    }
                    grid.Rows.Add(rowValues);
                }

                form.Controls.Add(panelContenedor);
                form.Controls.Add(lblTitulo);

                // 3. LA MAGIA DEL TAMAÑO DE LA VENTANA
                // Esto se ejecuta justo antes de mostrar la ventana, cuando la tabla ya sabe cuánto mide
                form.Load += (s, e) =>
                {
                    // Calculamos el ancho real de todas las columnas visibles
                    int anchoColumnas = grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
                    
                    // Le sumamos 60 píxeles para compensar el Padding del panel (15+15) y la barra de desplazamiento vertical (~20)
                    int anchoDeseado = anchoColumnas + 60;

                    // Ajustamos el ancho de la ventana, pero le ponemos un tope para que no se salga de tu pantalla
                    form.Width = Math.Min(anchoDeseado, Screen.PrimaryScreen.WorkingArea.Width - 100);
                };
                
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al leer el archivo:\n{ex.Message}", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}