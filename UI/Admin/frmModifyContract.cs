using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LOGIC.Facade;
using static Domain.Person;
using System.Drawing;
using System.IO;

namespace UI
{
    public partial class frmModifyContract : Form
    {
        private readonly ContractService _contractService;
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private List<Contract> _originalContracts;

        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmModifyContract()
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();

            // Inicializa los mensajes de ayuda traducidos
            InitializeHelpMessages();
            // Suscribe los eventos de ToolTips
            SubscribeHelpMessagesEvents();

            // Configurar el Timer
            toolTipTimer = new Timer
            {
                Interval = 1000 // 1 segundo
            };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            this.Load += frmModifyContract_Load;
            dgvContracts.CellClick += dgvContracts_CellClick;
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
                { cmbProperty, LanguageService.Translate("Seleccione la propiedad asociada al contrato.") },
                { cmbTenant, LanguageService.Translate("Seleccione el inquilino asociado al contrato.") },
                { txtPrice, LanguageService.Translate("Ingrese el precio mensual del contrato en números.") },
                { cldStartDate, LanguageService.Translate("Seleccione la fecha de inicio del contrato.") },
                { cldFinalDate, LanguageService.Translate("Seleccione la fecha de finalización del contrato.") },
                { btnSave, LanguageService.Translate("Guarda los cambios realizados en el contrato seleccionado.") },
                { btnDelete, LanguageService.Translate("Elimina el contrato seleccionado.") },
                { dgvContracts, LanguageService.Translate("Lista de contratos disponibles. Haga clic en un contrato para editarlo.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos de ayuda (ToolTips) a cada control definido en <see cref="helpMessages"/>.
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
                currentControl = control; // Guardar el control actual
                toolTipTimer.Start();      // Iniciar el temporizador
            }
        }

        private void Control_MouseLeave(object sender, EventArgs e)
        {
            toolTipTimer.Stop(); // Detener el temporizador
            currentControl = null; // Limpiar el control actual
        }

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

        private void frmModifyContract_Load(object sender, EventArgs e)
        {
            try
            {
                LoadContracts();
                LoadProperties();
                LoadTenants();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadContracts()
        {
            try
            {
                _originalContracts = _contractService.GetAllContracts();
                var properties = _propertyService.GetAllProperties()
                    .ToDictionary(p => p.IdProperty, p => p.AddressProperty);

                var displayContracts = _originalContracts.Select(contract => new
                {
                    IdContract = contract.IdContract,
                    PropertyAddress = properties.ContainsKey(contract.FkIdProperty) ? properties[contract.FkIdProperty] : LanguageService.Translate("Dirección no encontrada"),
                    StartDate = contract.DateStartContract,
                    EndDate = contract.DateFinalContract,
                    AnnualRentPrice = contract.AnnualRentPrice,
                    IsActive = contract.StatusContract
                }).ToList();

                dgvContracts.DataSource = displayContracts;

                dgvContracts.Columns["IdContract"].Visible = false;
                dgvContracts.Columns["PropertyAddress"].HeaderText = LanguageService.Translate("Propiedad");
                dgvContracts.Columns["StartDate"].HeaderText = LanguageService.Translate("Fecha de Inicio");
                dgvContracts.Columns["EndDate"].HeaderText = LanguageService.Translate("Fecha de Finalización");
                dgvContracts.Columns["AnnualRentPrice"].HeaderText = LanguageService.Translate("Precio Mensual");
                dgvContracts.Columns["IsActive"].HeaderText = LanguageService.Translate("Estado");
                dgvContracts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los contratos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadProperties()
        {
            try
            {
                var properties = _propertyService.GetPropertiesByStatus(PropertyStatusEnum.Disponible);
                cmbProperty.DataSource = properties;
                cmbProperty.DisplayMember = "AddressProperty";
                cmbProperty.ValueMember = "IdProperty";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar las propiedades") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadTenants()
        {
            try
            {
                var tenants = _personService.GetAllPersonsByType(PersonTypeEnum.Tenant);
                cmbTenant.DataSource = tenants;
                cmbTenant.DisplayMember = "NumberDocumentPerson";
                cmbTenant.ValueMember = "IdPerson";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los inquilinos") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dgvContracts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedContractId = (Guid)dgvContracts.Rows[e.RowIndex].Cells["IdContract"].Value;
                var selectedContract = _originalContracts.FirstOrDefault(c => c.IdContract == selectedContractId);
                if (selectedContract != null)
                {
                    LoadContractDetails(selectedContract);
                }
            }
        }

        private void LoadContractDetails(Contract contract)
        {
            cmbProperty.SelectedValue = contract.FkIdProperty;
            cmbTenant.SelectedValue = contract.FkIdTenant;
            txtPrice.Text = contract.AnnualRentPrice.ToString();
            cldStartDate.Value = contract.DateStartContract;
            cldFinalDate.Value = contract.DateFinalContract;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!int.TryParse(txtPrice.Text, out int price))
                {
                    MessageBox.Show(
                        LanguageService.Translate("El precio debe ser numérico."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;
                var contract = _originalContracts.FirstOrDefault(c => c.IdContract == selectedContractId);

                if (contract != null)
                {
                    contract.FkIdProperty = (Guid)cmbProperty.SelectedValue;
                    contract.FkIdTenant = (Guid)cmbTenant.SelectedValue;
                    contract.AnnualRentPrice = price;
                    contract.DateStartContract = cldStartDate.Value;
                    contract.DateFinalContract = cldFinalDate.Value;

                    _contractService.UpdateContract(contract);

                    var result = MessageBox.Show(
                        LanguageService.Translate("¿Desea modificar las cláusulas del contrato?"),
                        LanguageService.Translate("Modificar Cláusulas"),
                        MessageBoxButtons.YesNo
                    );
                    if (result == DialogResult.Yes)
                    {
                        var modifyContractClause = new frmModifyContractClause(contract)
                        {
                            MdiParent = this.MdiParent
                        };
                        modifyContractClause.FormClosed += (s, args) => this.Show();
                        modifyContractClause.Show();
                    }

                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar el contrato") + ": " + ex.Message,
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
                if (dgvContracts.CurrentRow == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Seleccione un contrato para eliminar."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                var selectedContractId = (Guid)dgvContracts.CurrentRow.Cells["IdContract"].Value;
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea eliminar el contrato y sus cláusulas asociadas?"),
                    LanguageService.Translate("Confirmar"),
                    MessageBoxButtons.YesNo
                );

                if (result == DialogResult.Yes)
                {
                    _contractService.DeleteContract(selectedContractId);
                    MessageBox.Show(
                        LanguageService.Translate("Contrato eliminado con éxito."),
                        LanguageService.Translate("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    LoadContracts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al eliminar el contrato") + ": " + ex.Message,
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

        private void FrmModifyContract_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de modificación de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccione un contrato de la lista para editarlo o eliminarlo.")}",
                $"- {LanguageService.Translate("Modifique la propiedad, inquilino, precio y fechas del contrato.")}",
                $"- {LanguageService.Translate("Presione 'Guardar' para actualizar los cambios.")}",
                $"- {LanguageService.Translate("Presione 'Eliminar' para borrar el contrato seleccionado.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
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
                LanguageService.Translate("MÓDULO DE MODIFICACIÓN DE CONTRATOS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite editar y actualizar la información de un contrato existente, ") +
                LanguageService.Translate("o eliminarlo si ya no es necesario. Asegúrate de seleccionar correctamente el contrato ") +
                LanguageService.Translate("en la lista de la izquierda antes de modificar sus datos.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Selecciona el contrato que deseas modificar en la lista de contratos. Sus datos se ") +
                LanguageService.Translate("   mostrarán automáticamente en los controles de la derecha.") + "\r\n" +
                LanguageService.Translate("2. Ajusta la propiedad, el inquilino, el precio mensual y/o las fechas de inicio y fin del contrato según corresponda.") + "\r\n" +
                LanguageService.Translate("3. Haz clic en ‘Guardar contrato’ para aplicar los cambios. Se te preguntará si deseas ") +
                LanguageService.Translate("   modificar también las cláusulas del contrato.") + "\r\n" +
                LanguageService.Translate("4. Si decides modificar las cláusulas, se abrirá una ventana adicional donde podrás ") +
                LanguageService.Translate("   editar o agregar nuevas cláusulas. Al terminar, regresa a este formulario.") + "\r\n" +
                LanguageService.Translate("5. Para eliminar un contrato por completo, haz clic en ‘Eliminar contrato’. ") +
                LanguageService.Translate("   Se te pedirá confirmación antes de borrarlo definitivamente.") + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás mantener al día la información de tus contratos en el sistema. ") +
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
