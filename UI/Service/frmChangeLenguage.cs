using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UI
{
    public partial class frmChangeLanguage : Form
    {
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;
        private Dictionary<string, string> languageMap;

        public frmChangeLanguage()
        {
            InitializeComponent();
            LoadLanguageFile(); // Cargar los idiomas en el `DataGridView`
            InitializeHelpMessages();
            SubscribeHelpMessagesEvents();
            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asignar el evento HelpRequested para mostrar la ayuda general
            this.HelpRequested += FrmChangeLanguage_HelpRequested;
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddPerson_HelpRequested_f1; // <-- Asignación del evento
        }

        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { dgvLanguages, LanguageService.Translate("Aquí se muestran los idiomas disponibles que puede modificar.\nPuede agregar nuevos idiomas o cambiar sus códigos.") },
                { btnSave, LanguageService.Translate("Guarda los cambios realizados en la lista de idiomas.\nRecuerde reiniciar el sistema para aplicar los cambios.") }
            };
        }

        private void SubscribeHelpMessagesEvents()
        {
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }
        }

        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control;
                toolTipTimer.Start();
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop();
            currentControl = null;
        }

        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000);
            }
            toolTipTimer.Stop();
        }

        private void LoadLanguageFile()
        {
            try
            {
                dgvLanguages.Rows.Clear();
                dgvLanguages.Columns.Clear();

                dgvLanguages.Columns.Add("Key", LanguageService.Translate("Idioma"));
                dgvLanguages.Columns.Add("Value", LanguageService.Translate("Código"));

                languageMap = LanguageService.GetLanguageMap();

                int rowIndex = 0;

                foreach (var entry in languageMap)
                {
                    int newRow = dgvLanguages.Rows.Add(entry.Key, entry.Value);

                    // Bloquear la edición solo para las dos primeras filas (Español e Inglés)
                    if (rowIndex < 2)
                    {
                        dgvLanguages.Rows[newRow].Cells[0].ReadOnly = true; // Idioma
                        dgvLanguages.Rows[newRow].Cells[1].ReadOnly = true; // Código
                    }

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageService.Translate("Error al cargar idiomas")}: {ex.Message}",
                    LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> updatedLanguages = new Dictionary<string, string>();

                // Recorrer todas las filas y guardar TODOS los idiomas
                foreach (DataGridViewRow row in dgvLanguages.Rows)
                {
                    if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                    {
                        string key = row.Cells[0].Value.ToString();
                        string value = row.Cells[1].Value.ToString();

                        updatedLanguages[key] = value;
                    }
                }

                // Guardar el archivo sobrescribiendo completamente el anterior
                LanguageService.SaveLanguageMap(updatedLanguages);

                MessageBox.Show(LanguageService.Translate("Idiomas actualizados correctamente; por favor reinicie el sistema para ver los cambios."),
                    LanguageService.Translate("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageService.Translate("Error al guardar idiomas")}: {ex.Message}",
                    LanguageService.Translate("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Muestra un mensaje de ayuda detallado cuando el usuario solicita ayuda (tecla F1 o botón de ayuda).
        /// </summary>
        private void FrmChangeLanguage_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido a la administración de idiomas."),
                "",
                LanguageService.Translate("Aquí puede gestionar los idiomas disponibles en el sistema."),
                "",
                LanguageService.Translate("📌 Opciones disponibles:"),
                $"- {LanguageService.Translate("Modificar los códigos de idioma existentes.")}",
                $"- {LanguageService.Translate("Agregar nuevos idiomas con sus códigos.")}",
                $"- {LanguageService.Translate("Guardar los cambios y actualizar el archivo de idiomas.")}",
                "",
                LanguageService.Translate("⚠️ Importante:"),
                LanguageService.Translate("1️⃣ Los idiomas 'Español' e 'English' están bloqueados y no pueden modificarse."),
                LanguageService.Translate("2️⃣ Debe reiniciar el sistema después de guardar los cambios para aplicarlos."),
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador del sistema.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
        private void FrmAddPerson_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Nombre de la imagen según el formulario
            string imageFileName = $"{this.Name}.png";

            // 2. Ruta de la imagen (ajusta según tu estructura de carpetas)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE ADMINISTRACIÓN DE IDIOMAS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite gestionar los idiomas disponibles en el sistema. ").ToString() +
                LanguageService.Translate("Podrás agregar nuevos idiomas, modificar los existentes o asignar diferentes códigos a cada uno. ").ToString() +
                LanguageService.Translate("Ten en cuenta que, para que los cambios surtan efecto, generalmente deberás reiniciar la aplicación.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de idiomas y sus códigos en la tabla. Cada fila representa un idioma.").ToString() + "\r\n" +
                LanguageService.Translate("2. Para **agregar** un nuevo idioma, escribe su nombre en la columna ‘Idioma’ y el código correspondiente en la columna ‘Código’ (en una fila nueva o vacía).").ToString() + "\r\n" +
                LanguageService.Translate("3. Para **modificar** un idioma existente, ajusta los valores en las celdas ‘Idioma’ y/o ‘Código’ de la fila correspondiente.").ToString() + "\r\n" +
                LanguageService.Translate("4. Haz clic en ‘Guardar Cambios’ para sobrescribir el archivo de idiomas con tus modificaciones.").ToString() + "\r\n" +
                LanguageService.Translate("5. Tras guardar, se recomienda **reiniciar** la aplicación para que los cambios se apliquen correctamente.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás mantener actualizada la lista de idiomas disponibles en el sistema. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

            // 4. Crear el formulario de ayuda
            using (Form helpForm = new Form())
            {
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.StartPosition = FormStartPosition.CenterParent;
                helpForm.Size = new Size(900, 700);
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // 5. Crear un TableLayoutPanel con 1 fila y 2 columnas
                TableLayoutPanel tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 2
                };

                // Columna 0: ancho fijo (p.ej. 350 px) para el texto
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
                // Columna 1: el resto del espacio para la imagen
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                // Fila única: ocupa todo el alto
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                // 6. Panel de texto con scroll (por si el texto es extenso)
                Panel textPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    Padding = new Padding(15)
                };

                // 7. Label con el texto. Usamos MaximumSize para forzar el wrap del texto
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    MaximumSize = new Size(320, 0),  // Menos que 350 para dejar margen
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };

                textPanel.Controls.Add(lblHelp);

                // 8. PictureBox para la imagen (columna derecha)
                PictureBox pbHelpImage = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.WhiteSmoke, // Opcional, para ver contraste
                    Margin = new Padding(5)
                };

                // 9. Cargamos la imagen si existe
                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // 10. Agregar el panel de texto (columna 0) y la imagen (columna 1) al TableLayoutPanel
                tableLayout.Controls.Add(textPanel, 0, 0);
                tableLayout.Controls.Add(pbHelpImage, 1, 0);

                // 11. Agregar el TableLayoutPanel al formulario
                helpForm.Controls.Add(tableLayout);

                // 12. Mostrar el formulario de ayuda
                helpForm.ShowDialog();
            }
        }
    }
}
