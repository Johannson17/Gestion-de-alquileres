using Domain;
using LOGIC.Facade;
using Services.Facade;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace UI
{
    public partial class frmModifyPerson : Form
    {
        private readonly PersonService _personService;

        private Timer toolTipTimer;
        private Control currentControl;
        private Dictionary<Control, string> helpMessages;

        public frmModifyPerson()
        {
            InitializeComponent();
            _personService = new PersonService();
            LoadPersonData();

            // Asigna el evento de clic de celda al DataGridView
            dgvPerson.CellClick += dgvPerson_CellClick;

            // Inicializar el Timer
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            RegisterHelpEvents(this); // Vincular eventos de ToolTip
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddProperty_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, LanguageService.Translate("Ingrese el nombre de la persona.") },
                { txtLastName, LanguageService.Translate("Ingrese el apellido de la persona.") },
                { txtDomicile, LanguageService.Translate("Ingrese el domicilio legal de la persona.") },
                { txtEmail, LanguageService.Translate("Ingrese el correo electrónico de la persona.") },
                { txtPhoneNumber, LanguageService.Translate("Ingrese el número de teléfono de la persona.") },
                { txtDocumentNumber, LanguageService.Translate("Ingrese el número de documento de la persona.") },
                { dgvPerson, LanguageService.Translate("Seleccione una persona para modificar o eliminar sus datos.") },
                { btnSave, LanguageService.Translate("Guarde los cambios realizados en la persona seleccionada.") },
                { btnDelete, LanguageService.Translate("Elimine a la persona seleccionada de la lista.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void RegisterHelpEvents(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (helpMessages.ContainsKey(control))
                {
                    control.MouseEnter += Control_MouseEnter;
                    control.MouseLeave += Control_MouseLeave;
                }

                if (control.HasChildren)
                {
                    RegisterHelpEvents(control);
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
        /// Carga los datos de las personas y los muestra en el `DataGridView`.
        /// </summary>
        private void LoadPersonData()
        {
            try
            {
                var persons = _personService.GetAllPersons();

                dgvPerson.DataSource = persons.Select(p => new
                {
                    p.IdPerson,
                    Name = p.NamePerson,
                    LastName = p.LastNamePerson,
                    Address = p.DomicilePerson,
                    ElectronicAddress = p.ElectronicDomicilePerson,
                    PhoneNumber = p.PhoneNumberPerson,
                    DocumentNumber = p.NumberDocumentPerson
                }).ToList();

                dgvPerson.Columns["IdPerson"].Visible = false;

                dgvPerson.Columns["Name"].HeaderText = LanguageService.Translate("Nombre");
                dgvPerson.Columns["LastName"].HeaderText = LanguageService.Translate("Apellido");
                dgvPerson.Columns["Address"].HeaderText = LanguageService.Translate("Domicilio legal");
                dgvPerson.Columns["ElectronicAddress"].HeaderText = LanguageService.Translate("Domicilio Electrónico");
                dgvPerson.Columns["PhoneNumber"].HeaderText = LanguageService.Translate("Numero telefónico");
                dgvPerson.Columns["DocumentNumber"].HeaderText = LanguageService.Translate("Número de Documento");

                dgvPerson.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar los datos")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvPerson_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    dynamic selectedPerson = dgvPerson.Rows[e.RowIndex].DataBoundItem;

                    txtName.Text = selectedPerson.Name;
                    txtLastName.Text = selectedPerson.LastName;
                    txtDomicile.Text = selectedPerson.Address;
                    txtEmail.Text = selectedPerson.ElectronicAddress;
                    txtPhoneNumber.Text = selectedPerson.PhoneNumber.ToString();
                    txtDocumentNumber.Text = selectedPerson.DocumentNumber.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{LanguageService.Translate("Error al seleccionar la persona")}: {ex.Message}",
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Actualiza las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmModifyPerson_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de personas."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar una persona de la lista para modificar sus datos.")}",
                $"- {LanguageService.Translate("Modificar los datos de una persona y guardar los cambios.")}",
                $"- {LanguageService.Translate("Eliminar una persona de la lista seleccionada.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedPerson = (Person)dgvPerson.CurrentRow?.DataBoundItem;

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para modificar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                selectedPerson.NamePerson = txtName.Text;
                selectedPerson.LastNamePerson = txtLastName.Text;
                selectedPerson.DomicilePerson = txtDomicile.Text;
                selectedPerson.ElectronicDomicilePerson = txtEmail.Text;
                selectedPerson.PhoneNumberPerson = int.Parse(txtPhoneNumber.Text);
                selectedPerson.NumberDocumentPerson = int.Parse(txtDocumentNumber.Text);

                _personService.UpdatePerson(selectedPerson);

                MessageBox.Show(
                    LanguageService.Translate("Datos actualizados correctamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LoadPersonData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar los cambios") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Verifica que haya una fila seleccionada
                if (dgvPerson.CurrentRow == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione una persona de la lista para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Obtener el ID de la persona seleccionada desde el DataGridView
                Guid idPerson = (Guid)dgvPerson.CurrentRow.Cells["IdPerson"].Value;

                // Buscar la persona en el servicio para obtener un objeto `Person`
                var selectedPerson = _personService.GetPersonById(idPerson);

                if (selectedPerson == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("No se encontró la persona en la base de datos."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Confirmación antes de eliminar
                var confirmation = MessageBox.Show(
                    LanguageService.Translate("¿Está seguro de que desea eliminar esta persona?"),
                    LanguageService.Translate("Confirmación"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmation == DialogResult.Yes)
                {
                    _personService.DeletePerson(selectedPerson.IdPerson);

                    MessageBox.Show(
                        LanguageService.Translate("Persona eliminada correctamente."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadPersonData(); // Recargar la lista
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la persona") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                txtDocumentNumber.Text = string.Empty;
                txtDomicile.Text = string.Empty;
                txtEmail.Text = string.Empty;  
                txtLastName.Text = string.Empty;
                txtName.Text = string.Empty;
                txtPhoneNumber.Text = string.Empty;
            }
        }

        private void FrmAddProperty_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Construimos el nombre del archivo de imagen según el formulario
            string imageFileName = $"{this.Name}.png";
            // 2. Ruta completa de la imagen (ajusta la carpeta si difiere)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda específico para "Agregar propiedad"
            var helpMessage =
                LanguageService.Translate("MÓDULO DE MODIFICACIÓN DE PERSONAS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite consultar, editar o eliminar los datos de las personas ") +
                LanguageService.Translate("registradas en el sistema. Para realizar cambios, primero debes seleccionar a la persona ") +
                LanguageService.Translate("en la lista superior y, posteriormente, modificar sus datos en los campos inferiores.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Selecciona una persona de la lista (DataGridView). Sus datos aparecerán en los campos de edición.") + "\r\n" +
                LanguageService.Translate("2. Modifica la información que necesites: nombre, apellido, domicilio, teléfono, etc.") + "\r\n" +
                LanguageService.Translate("3. Haz clic en ‘Guardar cambios’ para actualizar los datos de la persona en la base de datos.") + "\r\n" +
                LanguageService.Translate("4. Si deseas eliminar a la persona seleccionada por completo, haz clic en ‘Eliminar persona’. ") +
                LanguageService.Translate("   Se te pedirá confirmación antes de borrarla definitivamente.") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.");

            // 4. Creamos el formulario de ayuda
            using (Form helpForm = new Form())
            {
                // El formulario se autoajusta a su contenido
                helpForm.AutoSize = true;
                helpForm.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                helpForm.StartPosition = FormStartPosition.CenterScreen;
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // (Opcional) Limitamos el tamaño máximo para no salirnos de la pantalla
                // Esto hace que aparezca scroll si el contenido excede este tamaño
                helpForm.MaximumSize = new Size(
                    (int)(Screen.PrimaryScreen.WorkingArea.Width * 0.9),
                    (int)(Screen.PrimaryScreen.WorkingArea.Height * 0.9)
                );

                // Para permitir scroll si el contenido excede el tamaño máximo
                helpForm.AutoScroll = true;

                // 5. Creamos un FlowLayoutPanel que también se autoajuste
                FlowLayoutPanel flowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,   // Apilar texto e imagen verticalmente
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,                    // No “romper” en más columnas
                    Padding = new Padding(10)
                };

                // 6. Label para el texto de ayuda
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Margin = new Padding(5)
                };

                // (Opcional) Si deseas forzar que el texto no exceda cierto ancho y haga wrap:
                lblHelp.MaximumSize = new Size(800, 0);

                flowPanel.Controls.Add(lblHelp);

                // 7. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Margin = new Padding(5),
                    // Si quieres mostrar la imagen a tamaño real:
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    // O si prefieres que se ajuste pero mantenga proporción:
                    // SizeMode = PictureBoxSizeMode.Zoom,
                };

                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // (Opcional) Si usas Zoom, el PictureBox por defecto no hace auto-size, 
                // puedes darle un tamaño inicial y dejar que el form se ajuste
                // pbHelpImage.Size = new Size(600, 400);

                flowPanel.Controls.Add(pbHelpImage);

                // 8. Agregamos el FlowLayoutPanel al formulario
                helpForm.Controls.Add(flowPanel);

                // 9. Mostramos el formulario
                helpForm.ShowDialog();
            }
        }
    }
}
