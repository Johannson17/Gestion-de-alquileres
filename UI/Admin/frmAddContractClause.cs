using Domain;
using Services.Facade;
using Services.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace UI
{
    public partial class frmAddContractClause : Form
    {
        private readonly Contract _contract;
        private readonly int _clauseIndex;
        private readonly Person _owner;
        private readonly Person _tenant;
        private readonly Property _property;
        private readonly ContractService _contractService;

        // Propiedad para devolver la cláusula creada al formulario principal
        public ContractClause NewClause { get; private set; }

        private readonly Timer toolTipTimer;
        private Control currentControl;

        // Diccionario para almacenar los mensajes de ayuda
        private Dictionary<Control, string> helpMessages;

        public frmAddContractClause(Contract contract, Person owner, Person tenant, Property property, int clauseIndex)
        {
            InitializeComponent();
            _contract = contract;
            _clauseIndex = clauseIndex;
            _owner = owner;
            _tenant = tenant;
            _property = property;
            _contractService = new ContractService();

            toolTipTimer = new Timer { Interval = 1000 }; // 1 segundo
            toolTipTimer.Tick += ToolTipTimer_Tick;

            TranslateControls(); // Traducir todos los controles
            InitializeHelpMessages(); // Inicializa los mensajes de ayuda traducidos
            SubscribeToMouseEvents(); // Suscribe los eventos de ToolTips

            LoadClauseData(); // Cargar datos de la cláusula predefinida si aplica

            this.KeyPreview = true; // Permite que el formulario intercepte teclas como F1
            this.HelpRequested += FrmAddContractClause_HelpRequested_f1; // <-- Asignación del evento
        }

        /// <summary>
        /// Traduce todos los textos de los controles del formulario.
        /// </summary>
        private void TranslateControls()
        {
            this.Text = LanguageService.Translate("Gestión de Cláusulas");

            label1.Text = LanguageService.Translate("Título de la Cláusula:");
            label1.Text = LanguageService.Translate("Descripción de la Cláusula:");
            btnSave.Text = LanguageService.Translate("Guardar Cláusula");

            this.Text = LanguageService.Translate("Añadir clausulas del contrato");
        }

        /// <summary>
        /// Inicializa los mensajes de ayuda con la traducción actual.
        /// </summary>
        private void InitializeHelpMessages()
        {
            helpMessages = new Dictionary<Control, string>
            {
                { txtTittle, LanguageService.Translate("Ingrese el título de la cláusula. Si es una cláusula predefinida, este campo estará bloqueado.") },
                { txtDescription, LanguageService.Translate("Ingrese la descripción detallada de la cláusula. Si es una cláusula predefinida, este campo estará bloqueado.") },
                { btnSave, LanguageService.Translate("Presione para guardar la cláusula actual en el contrato.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos `MouseEnter` y `MouseLeave` a los controles con mensajes de ayuda.
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

        /// <summary>
        /// Carga los datos de la cláusula según el índice.
        /// </summary>
        private void LoadClauseData()
        {
            try
            {
                // Obtiene las cláusulas predefinidas desde el servicio
                var (Clause1, Clause2) = _contractService.GetPredefinedClause(_clauseIndex, _owner, _tenant, _property);

                if (_clauseIndex == 1)
                {
                    ApplyPredefinedClause(Clause1);
                }
                else if (_clauseIndex == 2)
                {
                    ApplyPredefinedClause(Clause2);
                }
                else
                {
                    EnableCustomClause();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al cargar los datos de la cláusula") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Aplica una cláusula predefinida y deshabilita la edición.
        /// </summary>
        private void ApplyPredefinedClause(ContractClause clause)
        {
            txtTittle.Text = clause.TitleClause;
            txtDescription.Text = clause.DetailClause;
            txtTittle.ReadOnly = true;
            txtDescription.ReadOnly = true;
        }

        /// <summary>
        /// Habilita la edición para cláusulas personalizadas.
        /// </summary>
        private void EnableCustomClause()
        {
            txtTittle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtTittle.ReadOnly = false;
            txtDescription.ReadOnly = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Crear una nueva cláusula con los datos del formulario y almacenarla en la propiedad NewClause
                NewClause = new ContractClause
                {
                    IdContractClause = Guid.NewGuid(),
                    FkIdContract = _contract.IdContract,
                    TitleClause = txtTittle.Text,
                    DetailClause = txtDescription.Text
                };

                MessageBox.Show(
                    LanguageService.Translate("Cláusula guardada exitosamente."),
                    LanguageService.Translate("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al guardar la cláusula") + ": " + ex.Message,
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

        private void FrmAddContractClause_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            var helpMessage = string.Join(Environment.NewLine, new[]
            {
                LanguageService.Translate("Bienvenido al módulo de gestión de cláusulas."),
                "",
                LanguageService.Translate("Opciones disponibles:"),
                $"- {LanguageService.Translate("Ingrese el título y la descripción de la cláusula (si es personalizable).")}",
                $"- {LanguageService.Translate("Si la cláusula es predefinida, los campos estarán bloqueados.")}",
                $"- {LanguageService.Translate("Presione 'Guardar' para registrar la cláusula en el contrato.")}",
                "",
                LanguageService.Translate("Para más ayuda, contacte con el administrador.")
            });

            MessageBox.Show(helpMessage,
                            LanguageService.Translate("Ayuda del sistema"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void FrmAddContractClause_HelpRequested_f1(object sender, HelpEventArgs hlpevent)
        {
            hlpevent.Handled = true;

            // 1. Nombre de la imagen según el formulario (por ejemplo, frmAddContractClause.png)
            string imageFileName = $"{this.Name}.png";

            // 2. Ruta de la imagen (ajusta según tu estructura)
            string imagePath = Path.Combine(Application.StartupPath, "..", "..", "images", imageFileName);
            imagePath = Path.GetFullPath(imagePath);

            // 3. Texto de ayuda (el que te pasé arriba)
            var helpMessage = string.Format(
                LanguageService.Translate("FORMULARIO DE AÑADIR CLÁUSULAS AL CONTRATO").ToString() + "\r\n\r\n" +
                LanguageService.Translate("En este formulario puedes agregar nuevas cláusulas a tu contrato de arrendamiento. ") +
                LanguageService.Translate("Las cláusulas pueden ser predefinidas (campos bloqueados) o personalizadas (campos editables). ") +
                LanguageService.Translate("Asegúrate de incluir toda la información relevante para que el contrato sea completo.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Si la cláusula es predefinida, el título y la descripción aparecerán rellenados y bloqueados.") + "\r\n" +
                LanguageService.Translate("2. Si la cláusula es personalizada, ingresa el título y la descripción según corresponda.") + "\r\n" +
                LanguageService.Translate("3. Haz clic en 'Guardar cláusula' para añadir la cláusula al contrato.") + "\r\n" +
                LanguageService.Translate("4. Puedes repetir este proceso para añadir varias cláusulas, hasta que decidas finalizar.") + "\r\n\r\n" +
                LanguageService.Translate("Una vez guardadas, las cláusulas formarán parte del contrato y se incluirán en la ") +
                LanguageService.Translate("versión final del mismo.") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, contacta al administrador del sistema."));

            // 4. Crear el formulario de ayuda
            using (Form helpForm = new Form())
            {
                helpForm.Text = LanguageService.Translate("Ayuda del sistema");
                helpForm.StartPosition = FormStartPosition.CenterParent;
                helpForm.Size = new Size(700, 500);  // Formulario más pequeño
                helpForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                helpForm.MaximizeBox = false;
                helpForm.MinimizeBox = false;

                // 5. TableLayoutPanel: 1 fila, 2 columnas
                TableLayoutPanel tableLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 1,
                    ColumnCount = 2
                };

                // Columna 0: ancho fijo (300 px) para texto
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
                // Columna 1: resto del espacio para la imagen
                tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                // Fila única que ocupa todo el alto
                tableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

                // 6. Panel para texto con scroll
                Panel textPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    Padding = new Padding(15)
                };

                // 7. Label para el texto, con MaximumSize para que el texto haga wrap
                Label lblHelp = new Label
                {
                    Text = helpMessage,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    MaximumSize = new Size(280, 0),  // Un poco menos que 300 px
                    Font = new Font("Segoe UI", 10, FontStyle.Regular)
                };

                textPanel.Controls.Add(lblHelp);

                // 8. PictureBox para la imagen
                PictureBox pbHelpImage = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Color.WhiteSmoke,
                    Margin = new Padding(5)
                };

                // 9. Cargar la imagen
                if (File.Exists(imagePath))
                {
                    pbHelpImage.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblHelp.Text += "\r\n\r\n" +
                                    "$" + LanguageService.Translate("No se encontró la imagen de ayuda en la ruta: ") + imagePath;
                }

                // 10. Agregar panel de texto (col 0) e imagen (col 1)
                tableLayout.Controls.Add(textPanel, 0, 0);
                tableLayout.Controls.Add(pbHelpImage, 1, 0);

                // 11. Agregar TableLayoutPanel al formulario de ayuda
                helpForm.Controls.Add(tableLayout);

                // 12. Mostrar el formulario
                helpForm.ShowDialog();
            }
        }
    }
}
