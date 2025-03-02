using Domain;
using System;
using Services.Facade;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace UI
{
    public partial class frmAddPropertyInventory : Form
    {
        private Property _property;  // Referencia al objeto Property existente
        private readonly Timer toolTipTimer;
        private Control currentControl;
        private Dictionary<Control, string> helpMessages; // Mensajes de ayuda traducibles

        public frmAddPropertyInventory(Property property)
        {
            InitializeComponent();
            _property = property;

            // Inicializar el temporizador para ToolTips
            toolTipTimer = new Timer { Interval = 1000 };
            toolTipTimer.Tick += ToolTipTimer_Tick;

            InitializeHelpMessages(); // Cargar los mensajes de ayuda traducidos
            SubscribeToMouseEvents(); // Suscribir eventos a los controles
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
                { txtName, LanguageService.Translate("Ingrese el nombre del elemento del inventario. Este campo es obligatorio.") },
                { txtDescription, LanguageService.Translate("Ingrese una descripción detallada del elemento del inventario. Este campo es obligatorio.") },
                { btnSave, LanguageService.Translate("Presione este botón para guardar el elemento del inventario en la propiedad actual.") }
            };
        }

        /// <summary>
        /// Suscribe los eventos MouseEnter y MouseLeave a los controles.
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
                toolTip.Show(helpMessages[currentControl], currentControl, 3000); // Mostrar durante 3 segundos
            }
            toolTipTimer.Stop();
        }

        /// <summary>
        /// Maneja el evento de clic en el botón de guardar, donde se agregan los datos del inventario a la propiedad.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show(
                        LanguageService.Translate("Por favor, complete todos los campos del inventario."),
                        LanguageService.Translate("Campos incompletos"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Crear un nuevo objeto InventoryProperty con los datos del formulario
                var newInventoryItem = new InventoryProperty
                {
                    IdInventoryProperty = Guid.NewGuid(),
                    NameInventory = txtName.Text,
                    DescriptionInventory = txtDescription.Text
                };

                // Verificar si ya existe el inventario antes de agregarlo
                if (_property.InventoryProperty == null)
                {
                    _property.InventoryProperty = new List<InventoryProperty>();
                }

                if (!_property.InventoryProperty.Any(i => i.NameInventory == newInventoryItem.NameInventory && i.DescriptionInventory == newInventoryItem.DescriptionInventory))
                {
                    _property.InventoryProperty.Add(newInventoryItem);
                }

                // Preguntar si desea agregar más
                var result = MessageBox.Show(
                    LanguageService.Translate("¿Desea agregar más elementos de inventario?"),
                    LanguageService.Translate("Agregar más"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    ClearForm(); // Limpiar el formulario para agregar otro elemento
                }
                else
                {
                    this.Close(); // Cerrar el formulario si no se desean agregar más elementos
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    LanguageService.Translate("Error al agregar el inventario") + ": " + ex.Message,
                    LanguageService.Translate("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Limpia los campos del formulario.
        /// </summary>
        private void ClearForm()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtName.Focus();
        }

        /// <summary>
        /// Actualiza los mensajes de ayuda cuando se cambia el idioma.
        /// </summary>
        public void UpdateHelpMessages()
        {
            InitializeHelpMessages();
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
            var helpMessage =
                LanguageService.Translate("FORMULARIO DE AGREGAR INVENTARIO A LA PROPIEDAD").ToString() + "\r\n\r\n" +
                LanguageService.Translate("Este formulario te permite añadir nuevos elementos de inventario ") +
                LanguageService.Translate("a la propiedad que estás registrando o editando. Asegúrate de llenar ") +
                LanguageService.Translate("los campos obligatorios (nombre y descripción) para cada elemento.") + "\r\n\r\n" +
                LanguageService.Translate("INSTRUCCIONES PASO A PASO:") + "\r\n" +
                LanguageService.Translate("1. Ingresa el nombre del elemento de inventario (por ejemplo, ‘Mesa’, ‘Televisor’, etc.).") + "\r\n" +
                LanguageService.Translate("2. Proporciona una descripción detallada, incluyendo características, estado o cualquier información relevante.") + "\r\n" +
                LanguageService.Translate("3. Haz clic en ‘Guardar cambios’ para añadir el elemento al inventario de la propiedad.") + "\r\n" +
                LanguageService.Translate("4. Si deseas agregar más elementos, selecciona ‘Sí’ cuando se te pregunte y repite el proceso.") + "\r\n\r\n" +
                LanguageService.Translate("Cada elemento de inventario quedará asociado a la propiedad y se registrará ") +
                LanguageService.Translate("en el sistema para futuras consultas o gestiones.") + "\r\n\r\n" +
                LanguageService.Translate("Si necesitas más ayuda, por favor contacta al administrador del sistema.");

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

        private void frmAddPropertyInventory_Load(object sender, EventArgs e)
        {

        }
    }
}
