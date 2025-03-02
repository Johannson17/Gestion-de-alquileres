using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmModifyFamily : Form
    {
        private Familia _selectedFamily;
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmModifyFamily()
        {
            InitializeComponent();
            InitializeHelpMessages(); // Inicializa los mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribe los eventos de ToolTips

            // Configurar el Timer para ToolTips
            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadFamilies(); // Cargar todas las familias en el DataGridView

            // Vincular eventos adicionales
            dgvFamilies.SelectionChanged += dgvFamilies_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
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
                { txtName, LanguageService.Translate("Ingrese el nombre de la familia que desea modificar.") },
                { chlbAccesos, LanguageService.Translate("Seleccione las patentes y familias que desea asociar a esta familia.") },
                { btnSave, LanguageService.Translate("Haga clic para guardar los cambios realizados en la familia seleccionada.") },
                { btnDelete, LanguageService.Translate("Haga clic para eliminar la familia seleccionada.") },
                { dgvFamilies, LanguageService.Translate("Seleccione una familia de la lista para modificar sus detalles.") }
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
                toolTip.Show(helpMessages[currentControl], currentControl, currentControl.Width / 2, currentControl.Height / 2, 3000);
            }
            toolTipTimer.Stop();
        }

        /// <summary>
        /// Cargar todas las familias en el DataGridView.
        /// </summary>
        private void LoadFamilies()
        {
            try
            {
                var familias = UserService.GetAllFamilias();

                var familiasData = familias.Select(f => new
                {
                    f.Id,
                    f.Nombre,
                    Patentes = string.Join(", ", f.Accesos.OfType<Patente>().Select(p => p.Nombre))
                }).ToList();

                dgvFamilies.DataSource = familiasData;

                dgvFamilies.Columns["Id"].Visible = false;
                dgvFamilies.Columns["Nombre"].HeaderText = LanguageService.Translate("Nombre de Familia");
                dgvFamilies.Columns["Patentes"].HeaderText = LanguageService.Translate("Permisos (Patentes)");
                dgvFamilies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar las familias")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvFamilies_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFamilies.SelectedRows.Count > 0)
            {
                try
                {
                    var selectedFamilyId = (Guid)dgvFamilies.SelectedRows[0].Cells["Id"].Value;
                    _selectedFamily = UserService.GetAllFamilias().FirstOrDefault(f => f.Id == selectedFamilyId);

                    if (_selectedFamily != null)
                    {
                        txtName.Text = _selectedFamily.Nombre;
                        LoadAccesos(_selectedFamily);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"{LanguageService.Translate("Error al seleccionar la familia")}: {ex.Message}",
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void LoadAccesos(Familia family)
        {
            try
            {
                chlbAccesos.DisplayMember = "Nombre";
                chlbAccesos.Items.Clear();

                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    bool isChecked = family.Accesos.Any(a => a is Patente && a.Id == patente.Id);
                    chlbAccesos.Items.Add(patente, isChecked);
                }

                var familias = UserService.GetAllFamilias().Where(f => f.Id != family.Id);
                foreach (var otraFamilia in familias)
                {
                    bool isChecked = family.Accesos.Any(a => a is Familia && a.Id == otraFamilia.Id);
                    chlbAccesos.Items.Add(otraFamilia, isChecked);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al cargar los accesos")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFamily != null)
                {
                    _selectedFamily.Nombre = txtName.Text;

                    _selectedFamily.Accesos.Clear();
                    foreach (var item in chlbAccesos.CheckedItems)
                    {
                        if (item is Acceso acceso)
                        {
                            _selectedFamily.Add(acceso);
                        }
                    }

                    UserService.UpdateFamilia(_selectedFamily);

                    MessageBox.Show(
                        LanguageService.Translate("Familia modificada con éxito."),
                        LanguageService.Translate("Modificación de Familia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadFamilies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al modificar la familia")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_selectedFamily != null)
                {
                    var confirmResult = MessageBox.Show(
                        LanguageService.Translate("¿Está seguro de que desea eliminar esta familia?"),
                        LanguageService.Translate("Confirmar eliminación"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirmResult == DialogResult.Yes)
                    {
                        UserService.DeleteFamilia(_selectedFamily.Id);

                        MessageBox.Show(
                            LanguageService.Translate("Familia eliminada con éxito."),
                            LanguageService.Translate("Eliminación de Familia"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        LoadFamilies();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{LanguageService.Translate("Error al eliminar la familia")}: {ex.Message}",
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE MODIFICACIÓN DE ROLES (FAMILIAS)").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite editar y administrar los roles (familias) existentes en el sistema, ").ToString() +
                LanguageService.Translate("permitiéndote cambiar su nombre, descripción y los permisos (accesos) asociados, ").ToString() +
                LanguageService.Translate("como patentes u otras familias.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Selecciona un rol de la lista de la parte izquierda. ").ToString() +
                LanguageService.Translate("   Los datos del rol (nombre, accesos) aparecerán en los campos de la derecha.").ToString() + "\r\n" +
                LanguageService.Translate("2. Ajusta el nombre de la familia en el campo ‘Nombre’ y, si corresponde, ").ToString() +
                LanguageService.Translate("   modifica o agrega una descripción.").ToString() + "\r\n" +
                LanguageService.Translate("3. En la lista de accesos, marca las patentes y/o subfamilias que desees asignar a este rol, ").ToString() +
                LanguageService.Translate("   y desmarca las que quieras revocar.").ToString() + "\r\n" +
                LanguageService.Translate("4. Haz clic en ‘Guardar cambios’ para confirmar las modificaciones en el rol seleccionado.").ToString() + "\r\n" +
                LanguageService.Translate("5. Si deseas eliminar el rol por completo, haz clic en ‘Eliminar’. Se te pedirá confirmación ").ToString() +
                LanguageService.Translate("   antes de borrarlo definitivamente.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás mantener actualizados los roles y sus permisos en el sistema. ").ToString() +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.").ToString());

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
