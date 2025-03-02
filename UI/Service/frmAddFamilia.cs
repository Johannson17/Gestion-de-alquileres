using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddFamilia : Form
    {
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmAddFamilia()
        {
            InitializeComponent();

            InitializeHelpMessages(); // Cargar las ayudas traducidas
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ToolTips

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadAccesos(); // Cargar patentes y familias existentes en los controles
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddPerson_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, LanguageService.Translate("Ingrese el nombre del rol que desea crear.") },
                { txtDescription, LanguageService.Translate("Ingrese una descripción para el rol.") },
                { chlbAccesos, LanguageService.Translate("Seleccione los permisos que desea asignar al rol.") },
                { btnSave, LanguageService.Translate("Haga clic para guardar el rol con los permisos seleccionados.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeHelpMessagesEvents()
        {
            if (helpMessages != null)
            {
                foreach (var control in helpMessages.Keys)
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }
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

        /// <summary>
        /// Carga las patentes y familias existentes en el `CheckedListBox`.
        /// </summary>
        private void LoadAccesos()
        {
            try
            {
                chlbAccesos.DisplayMember = "Nombre"; // Mostrar nombres en la lista

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    chlbAccesos.Items.Add(patente, false);
                }

                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    chlbAccesos.Items.Add(familia, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los accesos:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Evento para guardar la nueva familia.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show(
                        LanguageService.Translate("El campo 'Nombre' es obligatorio."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (chlbAccesos.CheckedItems.Count == 0)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Debe seleccionar al menos un permiso para el rol."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var nuevaFamilia = new Familia
                {
                    Nombre = txtName.Text,
                    Descripcion = txtDescription.Text
                };

                foreach (var acceso in chlbAccesos.CheckedItems)
                {
                    if (acceso is Acceso accesoSeleccionado)
                    {
                        nuevaFamilia.Add(accesoSeleccionado);
                    }
                }

                UserService.AddFamilia(nuevaFamilia);

                MessageBox.Show(
                    LanguageService.Translate("Rol agregado con éxito."),
                    LanguageService.Translate("Registro de Rol"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al agregar el rol:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmAddFamilia_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de creación de roles."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingrese un nombre y una descripción para el nuevo rol.")}",
                $"- {LanguageService.Translate("Seleccione los permisos que desea asignar al rol.")}",
                $"- {LanguageService.Translate("Presione 'Guardar' para registrar el rol en el sistema.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
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
                LanguageService.Translate("MÓDULO DE CREACIÓN DE ROLES (FAMILIAS)").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite crear un nuevo rol (denominado 'Familia') en el sistema, ").ToString() +
                LanguageService.Translate("asignándole un nombre, una descripción y los permisos (accesos) correspondientes. ").ToString() +
                LanguageService.Translate("De esta manera, podrás controlar qué acciones o secciones estarán disponibles ").ToString() +
                LanguageService.Translate("para los usuarios que posean este rol.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Ingresa el nombre del rol en el campo ‘Nombre’ (por ejemplo, ‘Administrador’, ‘Supervisor’, etc.).").ToString() + "\r\n" +
                LanguageService.Translate("2. Proporciona una breve descripción del rol en el campo ‘Descripción’ para detallar su propósito o alcance.").ToString() + "\r\n" +
                LanguageService.Translate("3. Selecciona los permisos que desees asignar al rol en la lista de ‘Accesos’ (pueden ser patentes específicas u otros roles/familias).").ToString() + "\r\n" +
                LanguageService.Translate("4. Haz clic en ‘Agregar Rol’ para guardar la nueva familia (rol) en el sistema.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, habrás creado un rol con los permisos elegidos, listo para asignarse a los usuarios que lo requieran. ").ToString() +
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
