using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI.Service
{
    public partial class frmModifyFamily : Form
    {
        private Familia _selectedFamily;
        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmModifyFamily()
        {
            InitializeComponent();
            LoadFamilies(); // Cargar todas las familias en el DataGridView

            // Inicializar el diccionario con mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, "Ingrese el nombre de la familia que desea modificar." },
                { chlbAccesos, "Seleccione las patentes y familias que desea asociar a esta familia." },
                { btnSave, "Haga clic para guardar los cambios realizados en la familia seleccionada." },
                { btnDelete, "Haga clic para eliminar la familia seleccionada." },
                { dgvFamilies, "Seleccione una familia de la lista para modificar sus detalles." }
            };

            // Configurar el Timer para el ToolTip
            toolTipTimer = new Timer { Interval = 1000 }; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Vincular eventos de MouseEnter y MouseLeave para mostrar las ayudas
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }

            // Vincular el evento de selección en el DataGridView
            dgvFamilies.SelectionChanged += dgvFamilies_SelectionChanged;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
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
                toolTip.Show(helpMessages[currentControl], currentControl, currentControl.Width / 2, currentControl.Height / 2, 3000); // Mostrar por 3 segundos
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
                    Patentes = string.Join(", ", f.Accesos.OfType<Patente>().Select(p => p.Nombre)) // Concatenar los nombres de las patentes
                }).ToList();

                dgvFamilies.DataSource = familiasData;

                // Ajustar columnas del DataGridView
                dgvFamilies.Columns["Id"].Visible = false; // Ocultar la columna de Id
                dgvFamilies.Columns["Nombre"].HeaderText = LanguageService.Translate("Nombre de Familia");
                dgvFamilies.Columns["Patentes"].HeaderText = LanguageService.Translate("Permisos (Patentes)");
                dgvFamilies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar las familias:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Al seleccionar una fila en el DataGridView, cargar los datos de la familia en los TextBox y CheckedListBox.
        /// </summary>
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
                        LanguageService.Translate("Error al seleccionar la familia:") + " " + ex.Message,
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        /// <summary>
        /// Cargar los accesos (patentes y familias) en el CheckedListBox y marcar los seleccionados.
        /// </summary>
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
                    LanguageService.Translate("Error al cargar los accesos:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Guardar los cambios realizados a la familia seleccionada.
        /// </summary>
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
                        MessageBoxIcon.Information
                    );
                    LoadFamilies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al modificar la familia:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Eliminar la familia seleccionada.
        /// </summary>
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
                        MessageBoxIcon.Warning
                    );

                    if (confirmResult == DialogResult.Yes)
                    {
                        UserService.DeleteFamilia(_selectedFamily.Id);

                        MessageBox.Show(
                            LanguageService.Translate("Familia eliminada con éxito."),
                            LanguageService.Translate("Eliminación de Familia"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        LoadFamilies();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar la familia:") + " " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}