using Services.Facade;
using System;
using System.Collections.Generic;
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
    }
}
