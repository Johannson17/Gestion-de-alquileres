using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UI
{
    public partial class frmAddFamilia : Form
    {
        private readonly Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl;

        public frmAddFamilia()
        {
            InitializeComponent();

            // Inicializar mensajes de ayuda
            helpMessages = new Dictionary<Control, string>
            {
                { txtName, "Ingrese el nombre del rol que desea crear." },
                { txtDescription, "Ingrese una descripción para el rol." },
                { chlbAccesos, "Seleccione los permisos que desea asignar al rol." },
                { btnSave, "Haga clic para guardar el rol con los permisos seleccionados." }
            };

            // Configurar el Timer
            toolTipTimer = new Timer();
            toolTipTimer.Interval = 1000; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            // Asociar eventos a los controles
            foreach (var control in helpMessages.Keys)
            {
                control.MouseEnter += Control_MouseEnter;
                control.MouseLeave += Control_MouseLeave;
            }

            LoadAccesos(); // Cargar patentes y familias existentes en los controles
        }

        /// <summary>
        /// Maneja la entrada del ratón en un control para iniciar el temporizador del mensaje de ayuda.
        /// </summary>
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Control control && helpMessages.ContainsKey(control))
            {
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start(); // Iniciar el temporizador
            }
        }

        /// <summary>
        /// Maneja la salida del ratón de un control para detener el temporizador.
        /// </summary>
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

        /// <summary>
        /// Muestra un mensaje de ayuda cuando el temporizador alcanza el tiempo establecido.
        /// </summary>
        private void ToolTipTimer_Tick(object sender, EventArgs e)
        {
            if (currentControl != null && helpMessages.ContainsKey(currentControl))
            {
                // Mostrar el ToolTip para el control actual
                ToolTip toolTip = new ToolTip();
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }

            toolTipTimer.Stop(); // Detener el temporizador
        }

        /// <summary>
        /// Carga las patentes y familias existentes en el CheckedListBox correspondiente.
        /// </summary>
        private void LoadAccesos()
        {
            try
            {
                chlbAccesos.DisplayMember = "Nombre"; // Asegúrate de que las clases Patente y Familia tengan la propiedad 'Nombre'

                // Cargar Patentes
                var patentes = UserService.GetAllPatentes();
                foreach (var patente in patentes)
                {
                    chlbAccesos.Items.Add(patente, false); // Añadir cada patente a la lista sin seleccionar
                }

                // Cargar Familias
                var familias = UserService.GetAllFamilias();
                foreach (var familia in familias)
                {
                    chlbAccesos.Items.Add(familia, false); // Añadir cada familia a la lista sin seleccionar
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
        /// Evento para el botón de guardar nueva familia.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos requeridos
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

                // Crear una nueva instancia de Familia
                var nuevaFamilia = new Familia
                {
                    Nombre = txtName.Text,
                    Descripcion = txtDescription.Text
                };

                // Iterar sobre los elementos seleccionados en CheckedListBox
                foreach (var acceso in chlbAccesos.CheckedItems)
                {
                    if (acceso is Acceso accesoSeleccionado)
                    {
                        nuevaFamilia.Add(accesoSeleccionado);
                    }
                }

                // Registrar la nueva familia en la base de datos usando la lógica del backend
                UserService.AddFamilia(nuevaFamilia);

                // Mostrar un mensaje de éxito
                MessageBox.Show(
                    LanguageService.Translate("Rol agregado con éxito."),
                    LanguageService.Translate("Registro de Rol"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Cerrar el formulario de alta de familia
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
    }
}
