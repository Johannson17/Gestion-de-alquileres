using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Services.Facade;
using Domain;
using Services.Domain;
using LOGIC.Facade;

namespace UI.Tenant
{
    public partial class frmContract : Form
    {
        private readonly ContractService _contractService;
        private readonly PropertyService _propertyService;
        private readonly Guid _loggedInTenantId; // ID del arrendatario logueado
        private Dictionary<Control, string> helpMessages;
        private Timer toolTipTimer;
        private Control currentControl; // Control actual donde está el mouse

        public frmContract(Guid loggedInTenantId)
        {
            InitializeComponent();
            _contractService = new ContractService();
            _propertyService = new PropertyService();
            _loggedInTenantId = loggedInTenantId;

            InitializeHelpMessages(); // Inicializar mensajes de ayuda traducidos
            SubscribeHelpMessagesEvents(); // Suscribir eventos de ToolTips

            // Configurar el Timer
            toolTipTimer = new Timer { Interval = 2000 }; // 2 segundos
            toolTipTimer.Tick += ToolTipTimer_Tick;

            LoadContracts(); // Cargar contratos al abrir el formulario
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
                { dgvContracts, LanguageService.Translate("Seleccione un contrato de la lista para firmarlo o descargarlo.") },
                { btnSave, LanguageService.Translate("Presione este botón para guardar la imagen del contrato firmado.") },
                { btnDownload, LanguageService.Translate("Presione este botón para descargar el contrato en formato PDF.") }
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
        /// Carga los contratos activos del inquilino logueado y los muestra en el DataGridView.
        /// </summary>
        private void LoadContracts()
        {
            List<Contract> contracts = _contractService.GetContractsByTenantIdAndStatus(_loggedInTenantId, "Inactivo");

            var properties = _propertyService.GetAllProperties().ToDictionary(p => p.IdProperty, p => p.AddressProperty);

            var displayContracts = contracts.Select(contract => new
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
        }

        /// <summary>
        /// Guarda la imagen del contrato firmado en la base de datos.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para guardar."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvContracts.CurrentRow.DataBoundItem;
            var idContract = (Guid)selectedRow.GetType().GetProperty("IdContract").GetValue(selectedRow);

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = LanguageService.Translate("Archivos de Imagen") + "|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = LanguageService.Translate("Seleccione la imagen del contrato firmado");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] contractImage = File.ReadAllBytes(openFileDialog.FileName);
                    _contractService.SaveContractImage(idContract, contractImage);

                    MessageBox.Show(LanguageService.Translate("Contrato e imagen guardados exitosamente."), LanguageService.Translate("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Descarga el contrato seleccionado en formato PDF.
        /// </summary>
        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (dgvContracts.CurrentRow == null)
            {
                MessageBox.Show(LanguageService.Translate("Seleccione un contrato para descargar."), LanguageService.Translate("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedRow = dgvContracts.CurrentRow.DataBoundItem;
            var idContract = (Guid)selectedRow.GetType().GetProperty("IdContract").GetValue(selectedRow);

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = LanguageService.Translate("Seleccione la carpeta de destino para el contrato");

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string outputPath = Path.Combine(folderDialog.SelectedPath, $"Contrato_{idContract}.pdf");
                    _contractService.GenerateContractPDF(idContract, outputPath);

                    MessageBox.Show(LanguageService.Translate("Contrato descargado en") + $": {outputPath}", LanguageService.Translate("Descarga Completa"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// Traduce las ayudas cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
        }

        private void FrmContract_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de contratos."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Seleccionar un contrato de la lista para firmarlo o descargarlo.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Guardar' para subir una imagen de un contrato firmado.")}",
                $"- {LanguageService.Translate("Presionar el botón 'Descargar' para obtener una copia del contrato en PDF.")}",
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
            var helpMessage = string.Format(
                LanguageService.Translate("MÓDULO DE GESTIÓN DE CONTRATOS PARA ARRENDATARIOS").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite ver los contratos asociados a tu arrendamiento, ").ToString() +
                LanguageService.Translate("así como firmarlos (subiendo una imagen con tu firma) y descargarlos en formato PDF.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:").ToString() + "\r\n" +
                LanguageService.Translate("1. Observa la lista de contratos en la parte superior. Aparecerán aquellos que tienes pendientes.").ToString() + "\r\n" +
                LanguageService.Translate("2. Selecciona un contrato de la lista.").ToString() + "\r\n" +
                LanguageService.Translate("3. Para **firmar** el contrato, haz clic en ‘Guardar Firmado’ y selecciona la imagen que contenga tu firma o el contrato ya firmado. ").ToString() +
                LanguageService.Translate("   El sistema guardará esta imagen como prueba de la firma.").ToString() + "\r\n" +
                LanguageService.Translate("4. Para **descargar** el contrato en formato PDF, haz clic en ‘Descargar contrato’ y elige la carpeta donde deseas guardarlo.").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Con estos pasos, podrás gestionar fácilmente tus contratos: **firmarlos** digitalmente y **descargarlos** para conservar una copia. ").ToString() +
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
                                    LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
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
