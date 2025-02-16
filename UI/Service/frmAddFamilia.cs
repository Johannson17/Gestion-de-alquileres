using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
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
    }
}
