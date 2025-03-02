using Domain;
using LOGIC.Facade;
using Services.Domain;
using Services.Facade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static Domain.Person;

namespace UI
{
    public partial class frmAddContract : Form
    {
        private readonly ContractService _contractFacade;
        private readonly PropertyService _propertyService;
        private readonly PersonService _personService;
        private readonly Timer toolTipTimer;
        private Control currentControl;

        public frmAddContract()
        {
            InitializeComponent();
            _contractFacade = new ContractService();
            _propertyService = new PropertyService();
            _personService = new PersonService();

            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            InitializeHelpMessages();
            SubscribeToMouseEvents();

            this.Load += frmAddContract_Load;
            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddPerson_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Diccionario de mensajes de ayuda para los controles.
        /// </summary>
        private Dictionary<Control, string> helpMessages = new Dictionary<Control, string>();

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { cmbProperty, LanguageService.Translate("Seleccione la propiedad disponible para asociarla al contrato.") },
                { cmbTenant, LanguageService.Translate("Seleccione el arrendatario para este contrato.") },
                { txtPrice, LanguageService.Translate("Ingrese el precio mensual del contrato (solo números).") },
                { cldStartDate, LanguageService.Translate("Seleccione la fecha de inicio del contrato.") },
                { cldFinalDate, LanguageService.Translate("Seleccione la fecha de finalización del contrato.") },
                { btnSave, LanguageService.Translate("Presione para guardar el contrato con los datos ingresados.") }
            };
        }

        /// <summary>
        /// Suscribe eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
        /// </summary>
        private void SubscribeToMouseEvents()
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

        private void frmAddContract_Load(object sender, EventArgs e)
        {
            try
            {
                LoadProperties();
                LoadTenants();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos iniciales") + ": " + ex.Message,
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProperty.SelectedItem == null || cmbTenant.SelectedItem == null)
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, seleccione una propiedad y un arrendatario."),
                        LanguageService.Translate("Advertencia"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                if (!int.TryParse(txtPrice.Text, out int annualRentPrice))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, ingrese un precio válido (solo números)."),
                        LanguageService.Translate("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                var property = (Property)cmbProperty.SelectedItem;
                var tenant = (Person)cmbTenant.SelectedItem;

                var owner = _personService.GetPersonByPropertyAndType(property.IdProperty, PersonTypeEnum.Owner);
                var contract = new Contract
                {
                    FkIdProperty = property.IdProperty,
                    FkIdTenant = tenant.IdPerson,
                    AnnualRentPrice = annualRentPrice,
                    DateStartContract = cldStartDate.Value,
                    DateFinalContract = cldFinalDate.Value,
                    Clauses = new List<ContractClause>()
                };

                OpenAddClauseForm(contract, owner, tenant, property);

                var contractId = _contractFacade.CreateContract(contract, owner, tenant, property);
                MessageBox.Show(
                    LanguageService.Translate("Contrato creado con éxito") + $". {LanguageService.Translate("ID")}: {contractId}",
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al crear el contrato") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private Contract OpenAddClauseForm(Contract contract, Person owner, Person tenant, Property property)
        {
            var clauseIndex = 1;
            var addMore = true;

            while (addMore)
            {
                using (var frmClause = new frmAddContractClause(contract, owner, tenant, property, clauseIndex))
                {
                    var result = frmClause.ShowDialog();
                    if (result == DialogResult.OK && frmClause.NewClause != null)
                    {
                        contract.Clauses.Add(frmClause.NewClause);

                        clauseIndex++;
                        addMore = clauseIndex > 3 ? MessageBox.Show(
                            LanguageService.Translate("¿Desea añadir más cláusulas?"),
                            LanguageService.Translate("Agregar Cláusula"),
                            MessageBoxButtons.YesNo
                        ) == DialogResult.Yes : true;
                    }
                    else
                    {
                        addMore = false;
                    }
                }
            }
            return contract;
        }

        private void FrmAddContract_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de creación de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar una propiedad y un arrendatario para crear un contrato.")}",
                $"- {LanguageService.Translate("Ingresar el precio del alquiler.")}",
                $"- {LanguageService.Translate("Seleccionar la fecha de inicio y fin del contrato.")}",
                $"- {LanguageService.Translate("Añadir cláusulas al contrato.")}",
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
                LanguageService.Translate("FORMULARIO DE CREACIÓN DE CONTRATOS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite crear un nuevo contrato de arrendamiento entre ") +
                LanguageService.Translate("una propiedad disponible y un arrendatario. Asegúrate de completar toda la ") +
                LanguageService.Translate("información para que el contrato sea válido.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Selecciona la propiedad que deseas arrendar.") + "\r\n" +
                LanguageService.Translate("2. Selecciona el arrendatario (inquilino) para el contrato.") + "\r\n" +
                LanguageService.Translate("3. Ingresa el precio mensual (solo números).") + "\r\n" +
                LanguageService.Translate("4. Define la fecha de inicio y la fecha de finalización.") + "\r\n" +
                LanguageService.Translate("5. Haz clic en 'Generar contrato' para guardar y crear el contrato.") + "\r\n\r\n" +
                LanguageService.Translate("A continuación, podrás añadir una o varias cláusulas al contrato, ") +
                LanguageService.Translate("hasta que decidas finalizar. Una vez completado, el sistema registrará el contrato ") +
                LanguageService.Translate("y te mostrará el ID correspondiente.") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, contacta al administrador del sistema."));

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
